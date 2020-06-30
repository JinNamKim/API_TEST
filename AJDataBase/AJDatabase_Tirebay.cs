using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;

namespace AJParkLib
{
    namespace AJDataBase
    {
        public static class AJDatabase_Tirebay
        {
            public static string strConnectionString = "Server=222.122.15.239;Database=zzase_v1;Uid=shop_t1;Pwd=)#@@shop_t10322;Charset=utf8;";
            static MySqlConnection myConnection = null;
            public static string ID = "shop_t1";
            public static string PW = ")#@@shop_t10322";
            public static string DataSource = "zzase_v1";

            public static void SetConnectionString()
            {
                strConnectionString = string.Format("Server=222.122.15.239;Database={2};Uid={0};Pwd={1};Charset=utf8;", ID, PW, DataSource);
            }

            #region Database Open/Close
            public static bool DatabaseOpen()
            {
                bool bResult = true;
                try
                {
                    System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(delegate()
                    {
                        try
                        {
                            myConnection = new MySqlConnection(strConnectionString);
                            myConnection.Open();
                        }
                        catch (System.Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }

                    }));
                    t.Start();
                    bool completed = t.Join(3000); //half a sec of timeout
                    if (!completed)
                    {
                        bResult = false;
                        t.Interrupt();
                    }

                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    bResult = false;
                }
                finally
                {
                }
                return bResult;
            }

            public static void DatabaseClose()
            {
                try
                {
                    myConnection.Close();
                }
                catch (System.Exception ex)
                {

                }
                finally
                {
                    myConnection = null;
                }
            }

            #endregion

            /// <summary>
            /// 입구 데이터 삽입(타이어베이)
            /// </summary>
            /// <param name="dpd"></param>
            /// <returns></returns>
            public static int InsertVehicleInfo(string carinfo, string shop_code, string regDate)
            {
                int num = 0;
                
                try
                {
                    if (DatabaseOpen())
                    {
                        using (MySqlCommand mySqlCommand = new MySqlCommand())
                        {
                            mySqlCommand.Connection = myConnection;
                            mySqlCommand.CommandText = "INSERT INTO ch_direct_shop (carinfo,shop_code,state,regdate) VALUES (@carinfo,@shop_code,@state,@regdate)";
                            mySqlCommand.Parameters.AddWithValue("@carinfo", (object)carinfo);
                            mySqlCommand.Parameters.AddWithValue("@shop_code", (object)shop_code);
                            mySqlCommand.Parameters.AddWithValue("@state", (object)0);
                            mySqlCommand.Parameters.AddWithValue("@regdate", (object)regDate);
                            num = mySqlCommand.ExecuteNonQuery();
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    DatabaseClose();
                }

                return num;
            }
        }
    }
}
