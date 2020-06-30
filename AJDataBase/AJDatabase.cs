using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
namespace AJParkLib
{
    namespace AJDataBase
    {
        public class AJ_MSSQL
        {
            public int m_iConnectionPort = 6240;
            public string m_strConnectionID = "sa";
            public string m_strConnectionIP = "127.0.0.1";
            public string m_strConnectionPWD = "aj1234";
            public string m_strDatabaseName = "AJPMS_ENG";
            private string m_strConnectionString = "";
            public SqlConnection m_sqlConn = null;

            public string IP
            {
                get { return m_strConnectionIP; }
                set { m_strConnectionIP = value; }
            }

            public int PORT
            {
                get { return m_iConnectionPort; }
                set { m_iConnectionPort = value; }
            }

            public string PWD
            {
                get { return m_strConnectionPWD; }
                set { m_strConnectionPWD = value; }
            }
            public string ID
            {
                get { return m_strConnectionID; }
                set { m_strConnectionID = value; }
            }
            public string NAME
            {
                get { return m_strDatabaseName; }
                set { m_strDatabaseName = value; }
            }

            #region Database open,close

            public void SetDatabaseConnectString()
            {
                if(m_iConnectionPort == 0)
                {
                    m_strConnectionString = string.Format("SERVER={0};DATABASE={1};UID={2};PWD={3}",
                        m_strConnectionIP, m_strDatabaseName, m_strConnectionID, m_strConnectionPWD);
                }
                else
                {
                    m_strConnectionString = string.Format("SERVER={0},{1};DATABASE={2};UID={3};PWD={4}",
                        m_strConnectionIP, m_iConnectionPort, m_strDatabaseName, m_strConnectionID, m_strConnectionPWD);
                }
                m_sqlConn = new SqlConnection(m_strConnectionString);
            }

            /// <summary>
            /// Database Connection Open
            /// </summary>
            /// <returns>true:open success,false:open fail</returns>
            public bool DatabaseOpen()
            {
                bool bReturn = false;
                try
                {
                    SetDatabaseConnectString();
                    if (m_sqlConn.State == ConnectionState.Closed)
                    {
                        m_sqlConn.ConnectionString = m_strConnectionString;
                        m_sqlConn.Open();
                        bReturn = true;
                    }
                }
                catch (Exception e)
                {
                }
                finally
                {
                }
                return bReturn;
                //return false;
            }
            /// <summary>
            /// Database Connection Close
            /// </summary>
            /// <returns>true:close success,false:close fail</returns>
            public bool DatabaseClose()
            {
                bool bReturn = false;
                try
                {
                    if (m_sqlConn.State == ConnectionState.Open)
                    {
                        m_sqlConn.Close();
                        bReturn = true;
                    }
                }
                catch (Exception e)
                {
                }
                finally
                {
                }
                return bReturn;
            }
            #endregion

            public DataTable GetDataTable(string strQuery)
            {
                DataTable dtResult = null;

                try
                {
                    if (DatabaseOpen())
                    {
                        SqlDataAdapter adpter = null;
                        adpter = new SqlDataAdapter();
                        dtResult = new DataTable();
                        adpter.SelectCommand = new SqlCommand(strQuery, m_sqlConn);
                        adpter.Fill(dtResult);
                    }
                }
                catch (System.Exception ex)
                {

                }
                finally
                {
                    DatabaseClose();
                }
                return dtResult;
            }

            public int GetScala(string strQuery)
            {
                int iResult = -99;

                try
                {
                    if (DatabaseOpen())
                    {
                        SqlCommand ScalaCommand = new SqlCommand(strQuery, m_sqlConn);
                        iResult = (int)ScalaCommand.ExecuteScalar();
                        
                    }
                }
                catch (System.Exception ex)
                {

                }
                finally
                {
                    DatabaseClose();
                }

                return iResult;
            }

            public int ExcuteNonQuery(string strQuery)
            {
                int iResult = -99;
                try
                {
                    if(DatabaseOpen())
                    {
                        SqlCommand ScalaCommand = new SqlCommand(strQuery, m_sqlConn);
                        iResult = ScalaCommand.ExecuteNonQuery();
                    }
                }
                catch(System.Exception ex)
                {

                }
                finally
                {
                    DatabaseClose();
                }

                return iResult;
            }
        }
    }
}