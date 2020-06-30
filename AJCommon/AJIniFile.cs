using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;           //INI파일 읽고 쓰기 위함
using System.IO;                                //FileInfo 쓰기 위함

//환경 설정 파일을 INI 형태로 저장, 읽기
namespace AJParkLib
{
    namespace AJCommon
    {
        public static class AJIniFile
        {
            /*
             * 
             * 16년 12월 15일
             * 1차 업데이트
             * 김진남
             * DLL_Import
             * INI_Write
             * INI_Read
             * INI_Show
             * 
             * * 16년 12월 26일
             * 2차 업데이트
             * 김진남
             * AJ_INI_DELETE
             * AJ_INI_GetPathName
             * try ~ catch 처리
             * throw 전부 주석 처리.
             * 
             */


            #region DLL_Import
            /*
             * WritePrivateProfileString 관련 MSDN
             * Return value
             * If the function successfully copies the string to the initialization file, the return value is nonzero.
             * If the function fails, or if it flushes the cached version of the most recently accessed initialization file,
             * the return value is zero. To get extended error information, call GetLastError.
             * */
            [DllImport("kernel32")]
            private static extern bool WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);

            [DllImport("kernel32")]
            private static extern int GetPrivateProfileInt(string lpAppName, string lpKeyName, int nDefault, string lpFileName);

            [DllImport("kernel32")]
            private static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);

            //이건 사용안함..
            [DllImport("kernel32")]
            private static extern int GetPrivateProfileSection(string appName, byte[] returnValue, int size, String filePath);
            #endregion

            #region Path 관련(Get, Set)
            private static string m_strFilePath = "";

            /// <summary>
            /// 
            /// </summary>
            /// <param name="str_Path"></param>
            public static void INI_SetPathName(string str_Path)
            {
                m_strFilePath = str_Path;
            }


            /// <summary>
            /// SetPathName으로 설정된 경로 반환
            /// </summary>
            /// <returns></returns>
            public static string AJ_INI_GetPathName()
            {
                return m_strFilePath;
            }

            /// <summary>
            /// SetPathName으로 설정된 경로 반환
            /// </summary>
            /// <param name="Is_Ok">Ref로 경로의 유효성 파악, TRUE : OK, FALSE : NG(설정된 경로 없음)</param>
            /// <returns></returns>
            public static string AJ_INI_GetPathName(ref bool b_Is_Ok)
            {
                if (m_strFilePath.Length < 1)                       //path의 문자열이 1 미만인 경우.
                {
                    b_Is_Ok = false;
                }
                b_Is_Ok = true;
                return m_strFilePath;
            }
            #endregion

            #region INI_Write
            /*
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
             * 작성일 : 2016.12.13
             * INI 쓰기
             * 섹션, 키 :  string
             * 값 : int 또는 string
             * 경로 : SetPathName()에서 설정한 경로를 그대로 사용하거나 새로 작성 가능(string)
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
             */

            // WritePrivateProfileString("섹션1", "키1", "값1", FilePath);



            /// <summary>
            /// <para>
            /// 섹션 : string,  키 : string,  값 : string,  경로 : SetPathName(string str)함수의 경로 반영
            /// </para>
            /// <para>
            /// int로 리턴, 0 -> 비정상, 1 -> 정상
            /// </para>
            /// </summary>
            /// <param name="section">섹션</param>
            /// <param name="key">키</param>
            /// <param name="value">값</param>
            /// <returns></returns>
            public static int AJ_INI_Write(string section, string key, string value)
            {
                try
                {
                    if (m_strFilePath.Length < 1)       //File Path의 길이가 1보다 작은 경우
                    {
                        Console.WriteLine("File Path is not corrected");
                        //throw new FileNotFoundException();
                        return 0;
                    }

                    else
                    {
                        if (WritePrivateProfileString(section, key, value, m_strFilePath))
                        {
                            return 1;
                        }
                        else
                        {
                            Console.WriteLine("INI_Write function Fail");
                            return 0;
                        }
                    }
                }

                catch (Exception ex)
                {
                    return 0;
                }
                

            }

            /// <summary>
            /// <para>
            /// 섹션 : string,  키 : string,  값 : int,  경로 : SetPathName(String str)함수의 경로 반영
            /// </para>
            /// <para>
            /// int로 리턴, 0 -> 비정상, 1 -> 정상
            /// </para>
            /// </summary>
            /// <param name="section">섹션</param>
            /// <param name="key">키</param>
            /// <param name="value">값</param>
            /// <returns></returns>
            public static int AJ_INI_Write(string section, string key, int value)
            {
                try
                {
                    if (m_strFilePath.Length < 1)       //File Path의 길이가 1보다 작은 경우
                    {
                        Console.WriteLine("File Path is not corrected");
                        //throw new FileNotFoundException();
                        return 0;
                    }

                    else
                    {
                        if (WritePrivateProfileString(section, key, Convert.ToString(value), m_strFilePath))
                        {
                            return 1;
                        }

                        else
                        {
                            Console.WriteLine("INI_Write function Fail");
                            return 0;
                        }
                    }
                }

                catch (Exception ex)
                {
                    return 0;
                }
                

            }

            /// <summary>
            /// <para>
            /// 섹션 : string,  키 : string,  값 : string,  경로 : string
            /// </para>
            /// <para>
            /// int로 리턴, 0 -> 비정상, 1 -> 정상
            /// </para>
            /// </summary>
            /// <param name="section">섹션</param>
            /// <param name="key">키</param>
            /// <param name="value">값</param>
            /// <param name="str_filepath">INI 파일 경로</param>
            /// <returns></returns>
            public static int AJ_INI_Write(string section, string key, string value, string str_filepath)
            {

                try
                {
                    if (str_filepath.Length < 1)            //File Path의 길이가 1보다 작은 경우
                    {
                        Console.WriteLine("File Path is not corrected");
                        
                        //throw new FileNotFoundException();
                        return 0;
                    }

                    else
                    {
                        if (WritePrivateProfileString(section, key, value, str_filepath))
                        {
                            return 1;
                        }
                        else
                        {
                            Console.WriteLine("INI_Write function Fail");
                            return 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    return 0;
                }
                

            }

            /// <summary>
            /// <para>
            /// 섹션 : string,  키 : string,  값 : int,  경로 : string
            /// </para>
            /// <para>
            /// int로 리턴, 0 -> 비정상, 1 -> 정상
            /// </para>
            /// </summary>
            /// <param name="section">섹션</param>
            /// <param name="key">키</param>
            /// <param name="value">값</param>
            /// <param name="str_filepath">INI 파일 경로</param>
            /// <returns></returns>
            public static int AJ_INI_Write(string section, string key, int value, string str_filepath)
            {
                try
                {
                    if (str_filepath.Length < 1)        //File Path의 길이가 1보다 작은 경우
                    {
                        Console.WriteLine("File Path is not corrected");
                        //throw new FileNotFoundException();
                        return 0;
                    }

                    else
                    {
                        if (WritePrivateProfileString(section, key, Convert.ToString(value), str_filepath))
                        {
                            return 1;
                        }

                        else
                        {
                            Console.WriteLine("INI_Write function Fail");
                            return 0;
                        }
                    }
                }

                catch (Exception ex)
                {
                    return 0;
                }
            }
            #endregion

            #region INI_Read

            /*
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
             * 작성일 : 2016.12.13
             * INI 읽기
             * 섹션, 키 :  string
             * 경로 : SetPathName()에서 설정한 경로를 그대로 사용하거나 새로 작성 가능(string)
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
             */

            /*
             * 참고
             * StringBuilder temp = new StringBuilder(255);
             * int ret = GetPrivateProfileString("섹션1", "키1", "", temp, 255, FilePath);
             */

            /// <summary>
            /// <para>
            /// INI파일의 해당 섹션, 해당 키의 값을 읽어서 string으로 리턴
            /// </para>
            /// <para>
            /// INI파일의 경로는 SetPathName(string str)함수의 경로 반영
            /// </para>
            /// </summary>
            /// <param name="section">섹션</param>
            /// <param name="key">키</param>
            /// <returns>-1 -> 파일 경로 에러 -2 -> 파일 없음 -3 -> 알수 없는 에러(MSG 참조) 0 -> 성공</returns>
            
            public static int AJ_INI_Read(string section, string key, ref string ResultValue,ref string MSG)
            {
                int iReturnValue = -3;
                try
                {
                    if (m_strFilePath.Length < 1)                       //path의 문자열이 1 미만인 경우.
                    {
                        iReturnValue = -1;
                        //return "ERROR : File Path is not correct";
                    }
                    else if (Is_file_exist(m_strFilePath) == 0)          //path는 있지만, 해당 경로에 파일이 없는 경우
                    {
                        Console.WriteLine("File is not exists");
                        iReturnValue = -2;
                        //return "ERROR : File is not exists";
                    }
                    else                                                //경로도 있고, 해당 경로에 파일이 있는 경우
                    {
                        StringBuilder str_temp = new StringBuilder();
                        GetPrivateProfileString(section, key, "", str_temp, 255, m_strFilePath);
                        ResultValue = str_temp.ToString();
                        iReturnValue = 0;
                    }
                }
                catch (Exception ex)
                {
                    MSG = ex.ToString();
                    iReturnValue = -3;
                }

                return iReturnValue;
            }

            /*
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
             * 작성일 : 2016.12.13
             * INI 읽기
             * 섹션, 키 :  string
             * 경로 : SetPathName()에서 설정한 경로를 그대로 사용하거나 새로 작성 가능(string)
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
             */

            /*
             * 참고
             * StringBuilder temp = new StringBuilder(255);
             * int ret = GetPrivateProfileString("섹션1", "키1", "", temp, 255, FilePath);
             */

            /// <summary>
            /// <para>
            /// INI파일의 해당 섹션, 해당 키의 값을 읽어서 string으로 리턴
            /// </para>
            /// <para>
            /// INI파일의 경로는 SetPathName(string str)함수의 경로 반영
            /// </para>
            /// </summary>
            /// <param name="section">섹션</param>
            /// <param name="key">키</param>
            /// <returns>-1 -> 파일 경로 에러 -2 -> 파일 없음 -3 -> 알수 없는 에러(MSG 참조) 0 -> 성공</returns>

            public static int   AJ_INI_Read(string section, string key, ref int ResultValue, ref string MSG)
            {
                int iReturnValue = -3;
                try
                {
                    if (m_strFilePath.Length < 1)                       //path의 문자열이 1 미만인 경우.
                    {
                        iReturnValue = -1;
                        //return "ERROR : File Path is not correct";
                    }
                    else if (Is_file_exist(m_strFilePath) == 0)          //path는 있지만, 해당 경로에 파일이 없는 경우
                    {
                        Console.WriteLine("File is not exists");
                        iReturnValue = -2;
                        //return "ERROR : File is not exists";
                    }
                    else                                                //경로도 있고, 해당 경로에 파일이 있는 경우
                    {
                        //StringBuilder str_temp = new StringBuilder();
                        ResultValue = GetPrivateProfileInt(section, key, 0, m_strFilePath);
                        iReturnValue = 0;
                    }
                }
                catch (Exception ex)
                {
                    MSG = ex.ToString();
                    iReturnValue = -3;
                }

                return iReturnValue;
            }

            /// <summary>
            /// <para>
            /// INI파일의 해당 섹션, 해당 키의 값을 읽어서 string으로 리턴
            /// </para>
            /// <para>
            /// INI파일의 경로는 직접 입력
            /// </para>
            /// </summary>
            /// <param name="section">섹션</param>
            /// <param name="key">키</param>
            /// <param name="file_path">INI파일 경로</param>
            /// <returns>-1 -> 파일 경로 에러 -2 -> 파일 없음 -3 -> 알수 없는 에러(MSG 참조) 0 -> 성공</returns>
            //public static string AJ_INI_Read_String(string section, string key, string file_path, ref string ResultValue, ref string MSG)
            public static int AJ_INI_Read(string section, string key, string file_path, ref string ResultValue, ref string MSG)
            {
                int iReturnValue = -3;
                try
                {
                    if (file_path.Length < 1)                       //path의 문자열이 1 미만인 경우.
                    {
                        iReturnValue = -1;
                        //return "ERROR : File Path is not correct";
                    }
                    else if (Is_file_exist(file_path) == 0)          //path는 있지만, 해당 경로에 파일이 없는 경우
                    {
                        Console.WriteLine("File is not exists");
                        iReturnValue = -2;
                        //return "ERROR : File is not exists";
                    }
                    else                                                //경로도 있고, 해당 경로에 파일이 있는 경우
                    {
                        StringBuilder str_temp = new StringBuilder();
                        GetPrivateProfileString(section, key, "", str_temp, 255, file_path);
                        ResultValue = str_temp.ToString();
                        iReturnValue = 0;
                    }
                }
                catch (Exception ex)
                {
                    MSG = ex.ToString();
                    iReturnValue = -3;
                }

                return iReturnValue;
            }

            /// <summary>
            /// <para>
            /// INI파일의 해당 섹션, 해당 키의 값을 읽어서 int로 리턴
            /// </para>
            /// <para>
            /// INI파일의 경로는 SetPathName(string str)함수의 경로 반영
            /// </para>
            /// </summary>
            /// <param name="section">섹션</param>
            /// <param name="key">키</param>
            /// <returns>리턴 밸류 (-441144) -> 오류 </returns>
            public static int AJ_INI_Read(string section, string key)
            {
                try
                {
                    if (m_strFilePath.Length < 1)                       //path의 문자열이 1 미만인 경우.
                    {
                        Console.WriteLine("File Path is not correct");
                        //throw new FileNotFoundException();
                        return -441144;
                    }

                    else if (Is_file_exist(m_strFilePath) == 0)          //path는 있지만, 해당 경로에 파일이 없는 경우
                    {
                        Console.WriteLine("File is not exists");
                        //throw new FileNotFoundException();
                        return -441144;
                    }

                    else                                                //경로도 있고, 해당 경로에 파일이 있는 경우
                    {
                        return GetPrivateProfileInt(section, key, -441144, m_strFilePath);
                    }
                }
                catch (Exception ex)
                {
                    return -441144;
                }
                

            }

            /// <summary>
            /// <para>
            /// INI파일의 해당 섹션, 해당 키의 값을 읽어서 int로 리턴
            /// </para>
            /// <para>
            /// INI파일의 경로는 직접 입력
            /// </para>
            /// </summary>
            /// <param name="section">섹션</param>
            /// <param name="key">키</param>
            /// <param name="file_path">INI파일 경로</param>
            /// <returns>리턴 밸류 (-441144) -> 오류 </returns>
            public static int AJ_INI_Read(string section, string key, string file_path)
            {
                try
                {
                    if (file_path.Length < 1)                        //path의 문자열이 1 미만인 경우.
                    {
                        Console.WriteLine("File Path is not correct");
                        //throw new FileNotFoundException();
                        return -441144;
                    }

                    else if (Is_file_exist(file_path) == 0)          //path는 있지만, 해당 경로에 파일이 없는 경우
                    {
                        Console.WriteLine("File is not exists");
                        //throw new FileNotFoundException();
                        return -441144;
                    }

                    else                                            //경로도 있고, 해당 경로에 파일이 있는 경우
                    {
                        return GetPrivateProfileInt(section, key, -441144, file_path);
                    }
                }
                catch (Exception ex)
                {
                    return -441144;
                }
                

            }



            #endregion

            #region INI_DELETE

            /* 16년 12월 26일 삭제 관련 작성
             * 김진남
             * 해당 경로의 INI파일 백업 후 삭제
             * EX)
             * c:\test.ini   ->   c:\test.bak 생성 ( test.bak가 이미 존재하면 덮어씌움 )
             * c:\test.ini삭제.
             * 
             * 
             * 
             * */

            /// <summary>
            /// <para>
            /// INI_SetPathName로 설정된 경로의 INI파일 지움
            /// </para>
            /// <para>
            /// 같은 경로에 [FileName.bak] 백업본 생성
            /// </para>
            /// </summary>
            /// <returns>리턴 -> TRUE : 동작 성공, FALSE : 동작 실패(해당경로에 파일 없음)</returns>
            public static bool AJ_INI_DELETE()
            {

                try
                {
                    FileInfo INI_File = new FileInfo(m_strFilePath);
                    string str_temp;
                    str_temp = m_strFilePath.Substring(0, m_strFilePath.Length - 3);            //INI파일명 . 까지 가져옴 EX) "c:\\test.ini" -> "c:\\test." 까지
                    str_temp += "bak";
                    FileInfo BAK_File = new FileInfo(str_temp);

                    if (INI_File.Exists)                //지울 INI 파일이 존재하면
                    {
                        
                        if (BAK_File.Exists)            //BAK 파일이 있는지 확인하고 존재하면, 이전 BAK지우고 현재 INI를 BAK로 
                        {
                            
                            //INI_File.Replace(m_strFilePath, str_temp);        //에러 발생.. 으음
                            BAK_File.Delete();                          //이전 BAK를 지우고
                            INI_File.CopyTo(str_temp);                  //INI를 BAK에 복사
                            INI_File.Delete();                          //최종적으로 INI를 지운다
                            return true;
                        }
                        else                            //BAK없으면 생성
                        {
                            //INI_File.Replace(file_path, str_temp);
                            INI_File.CopyTo(str_temp);                  //INI를 BAK에 복사하고
                            INI_File.Delete();                          //INI를 지운다
                            return true;
                        }

                    }
                    else
                    {
                        return false;
                    }
                }

                catch (Exception ex)
                {
                    return false;
                }
                
            }
            /// <summary>
            /// <para>
            /// 파라미터 경로의 INI파일 지움
            /// </para>
            /// /// <para>
            /// 같은 경로에 [FileName.bak] 백업본 생성
            /// </para>
            /// </summary>
            /// <returns>리턴 -> TRUE : 동작 성공, FALSE : 동작 실패(해당경로에 파일 없음)</returns>
            public static bool AJ_INI_DELETE(string file_path)
            {
                try
                {
                    FileInfo INI_File = new FileInfo(file_path);
                    string str_temp;
                    str_temp = file_path.Substring(0, file_path.Length - 3);            //INI파일명 . 까지 가져옴 EX) "c:\\test.ini" -> "c:\\test." 까지
                    str_temp += "bak";
                    FileInfo BAK_File = new FileInfo(str_temp);

                    if (INI_File.Exists)                //지울 INI 파일이 존재하면
                    {
                        if (BAK_File.Exists)            //BAK 파일이 있는지 확인하고 존재하면, 이전 BAK지우고 현재 INI를 BAK로 
                        {
                            
                            //INI_File.Replace(m_strFilePath, str_temp);        //에러 발생.. 으음
                            BAK_File.Delete();                          //이전 BAK를 지우고
                            INI_File.CopyTo(str_temp);                  //INI를 BAK에 복사
                            INI_File.Delete();                          //최종적으로 INI를 지운다
                            return true;
                        }
                        else                            //BAK없으면 생성
                        {
                            //INI_File.Replace(file_path, str_temp);
                            INI_File.CopyTo(str_temp);                  //INI를 BAK에 복사하고
                            INI_File.Delete();                          //INI를 지운다
                            return true;
                        }

                    }

                    else
                    {
                        return false;
                    }

                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }
            }
            #endregion

            #region INI_Show

            /// <summary>
            /// <para>
            /// INI 파일의 섹션을 구해서 스트링 배열로 리턴
            /// </para>
            /// <para>
            /// INI의 경로는 INI_SetPathName(string FilePath)에서 설정된 경로를 사용
            /// </para>
            /// <para>
            /// return -> [sectionmae]
            /// </para>
            /// <code>
            /// <para>
            /// ----------------------------------------------------
            /// </para>
            /// <para>
            /// CODE Example
            /// </para>
            /// <para>
            /// string[] section_name = new string[100];
            /// </para>
            /// <para>
            /// section_name = INI_show_section();
            /// </para>
            /// <para>
            /// ----------------------------------------------------
            /// </para>
            /// </code>
            /// </summary>
            /// <returns>string[100]으로 리턴, 중간에 실패하면 "ERROR : 내용" 리턴</returns>
            public static string[] AJ_INI_show_section()
            {
                string [] status = new string[1];     //에러났을 때 상태를 보내주기 위해서
                status[0] = "ERROR : AJ_INI_show_section에서 에러";

                try
                {
                    if (m_strFilePath.Length < 1)                        //path의 문자열이 1 미만인 경우.
                    {
                        Console.WriteLine("File Path is not correct");
                        //throw new FileNotFoundException();
                        status[0] = "ERROR : File Path is not correct";
                        return status;
                    }

                    else if (Is_file_exist(m_strFilePath) == 0)          //path는 있지만, 해당 경로에 파일이 없는 경우
                    {
                        Console.WriteLine("File is not exists");
                        //throw new FileNotFoundException();
                        status[0] = "ERROR : File is not exists";
                        return status;
                    }

                    else
                    {
                        StreamReader sr = new StreamReader(m_strFilePath, Encoding.Default);     //한글 깨짐 방지
                        Encoding en = sr.CurrentEncoding;                                        //한글 깨짐 방지
                        string data;

                        string[] section_data = new string[100];
                        int i = 0;

                        while ((data = sr.ReadLine()) != null)
                        {
                            if (data.Contains('['))
                            {
                                //section = section + data;
                                section_data[i] = data;
                                i += 1;
                            }
                        }
                        sr.Close();
                        return section_data;
                    }
                }

                catch (Exception ex)
                {
                    return status;
                }

                

            }


            /// <summary>
            /// <para>
            /// INI 파일의 섹션을 구해서 스트링 배열로 리턴
            /// </para>
            /// <para>
            /// return -> [sectionmae]
            /// </para>
            /// <code>
            /// <para>
            /// ----------------------------------------------------
            /// </para>
            /// <para>
            /// CODE Example
            /// </para>
            /// <para>
            /// string[] section_name = new string[100];
            /// </para>
            /// <para>
            /// section_name = INI_show_section(string FilePath);
            /// </para>
            /// <para>
            /// ----------------------------------------------------
            /// </para>
            /// </code>
            /// </summary>
            /// <param name="pathfile">INI파일 경로</param>
            /// <returns>string[100]으로 리턴, 중간에 실패하면 "ERROR : 내용" 리턴</returns>
            public static string[] AJ_INI_show_section(string pathfile)
            {
                string[] status = new string[1];     //에러났을 때 상태를 보내주기 위해서
                status[0] = "ERROR : AJ_INI_show_section에서 에러";

                try
                {
                    if (pathfile.Length < 1)                        //path의 문자열이 1 미만인 경우.
                    {
                        Console.WriteLine("File Path is not correct");
                        //throw new FileNotFoundException();
                        status[0] = "ERROR : File Path is not correct";
                        return status;
                    }

                    else if (Is_file_exist(pathfile) == 0)          //path는 있지만, 해당 경로에 파일이 없는 경우
                    {
                        Console.WriteLine("File is not exists");
                        //throw new FileNotFoundException();
                        status[0] = "ERROR : File is not exists";
                        return status;
                    }

                    else
                    {
                        StreamReader sr = new StreamReader(pathfile, Encoding.Default);     //한글 깨짐 방지
                        Encoding en = sr.CurrentEncoding;                                        //한글 깨짐 방지
                        string data;

                        string[] section_data = new string[100];
                        int i = 0;

                        while ((data = sr.ReadLine()) != null)
                        {
                            if (data.Contains('['))
                            {
                                //section = section + data;
                                section_data[i] = data;
                                i += 1;
                            }
                        }
                        sr.Close();
                        return section_data;
                    }
                }

                catch (Exception ex)
                {
                    return status;
                }

            }


            /// <summary>
            /// <para>
            /// INI 파일의 키와 값을 구해서 스트링 배열로 리턴
            /// </para>
            /// <para>
            /// INI의 경로는 INI_SetPathName(string FilePath)에서 설정된 경로를 사용
            /// </para>
            /// <para>
            /// SectionName 파라미터에 ""(공백) 입력시 모든 섹션의 키, 밸류 리턴
            /// </para>
            /// <para>
            /// return -> key=value
            /// </para>
            /// <code>
            /// <para>
            /// ----------------------------------------------------
            /// </para>
            /// <example>
            /// CODE Example
            /// </example>
            /// <para>
            /// string[] value_data = new string[100];
            /// </para>
            /// <para>
            /// value_data = INI_show_value(string SectionName);
            /// </para>
            /// <para>
            /// ----------------------------------------------------
            /// </para>
            /// </code>
            /// </summary>
            /// <param name="section">섹션 이름</param>
            /// <returns>string[100]으로 리턴, 중간에 실패하면 ERROR : 내용으로 리턴</returns>
            public static string[] AJ_INI_show_value(string section)
            {
                string[] status = new string[1];     //에러났을 때 상태를 보내주기 위해서
                status[0] = "ERROR : AJ_INI_show_value에서 에러";

                try
                {
                    if (m_strFilePath.Length < 1)                        //path의 문자열이 1 미만인 경우.
                    {
                        Console.WriteLine("File Path is not correct");
                        //throw new FileNotFoundException();
                        status[0] = "ERROR : File Path is not correct";
                        return status;
                    }

                    else if (Is_file_exist(m_strFilePath) == 0)          //path는 있지만, 해당 경로에 파일이 없는 경우
                    {
                        Console.WriteLine("File is not exists");
                        //throw new FileNotFoundException();
                        status[0] = "ERROR : File is not exists";
                        return status;
                    }

                    else
                    {
                        StreamReader sr = new StreamReader(m_strFilePath, Encoding.Default);     //한글 깨짐 방지
                        Encoding en = sr.CurrentEncoding;                                        //한글 깨짐 방지
                        string data;

                        string[] value_data = new string[100];
                        int i = 0;
                        int section_flag = 0;

                        while ((data = sr.ReadLine()) != null)
                        {
                            if (section_flag == 1)
                            {
                                if (data.Contains('['))
                                {
                                    section_flag = 0;
                                }
                                else
                                {
                                    value_data[i] = data;
                                    i += 1;
                                }

                            }

                            if (data.Contains('['))
                            {
                                if (data.Contains(section))
                                {
                                    section_flag = 1;
                                }
                            }
                        }
                        sr.Close();
                        if (i == 0)
                        {
                            Console.WriteLine("해당 섹션이 없습니다.");
                            //throw new Exception("해당 섹션이 없습니다.");
                            status[0] = "ERROR : 해당 섹션이 없습니다.";
                            return status;

                        }
                        return value_data;
                    }
                }
                catch (Exception ex)
                {
                    return status;
                }

                
            }


            /// <summary>
            /// <para>
            /// INI 파일의 키와 값을 구해서 스트링 배열로 리턴
            /// </para>
            /// <para>
            /// SectionName 파라미터에 ""(공백) 입력시 모든 섹션의 키, 밸류 리턴
            /// </para>
            /// <para>
            /// return -> key=value
            /// </para>
            /// <code>
            /// <para>
            /// ----------------------------------------------------
            /// </para>
            /// <para>
            /// CODE Example
            /// </para>
            /// <para>
            /// string[] value_data = new string[100];
            /// </para>
            /// <para>
            /// value_data = INI_show_value(string FilePath, string SectionName);
            /// </para>
            /// <para>
            /// ----------------------------------------------------
            /// </para>
            /// </code>
            /// </summary>
            /// <param name="pathfile">INI파일 경로</param>
            /// <param name="section">섹션 이름</param>
            /// <returns>string[100]으로 리턴</returns>
            public static string[] AJ_INI_show_value(string pathfile, string section)
            {
                string[] status = new string[1];     //에러났을 때 상태를 보내주기 위해서
                status[0] = "ERROR : AJ_INI_show_value에서 에러";

                try
                {
                    if (pathfile.Length < 1)                        //path의 문자열이 1 미만인 경우.
                    {
                        Console.WriteLine("File Path is not correct");
                        //throw new FileNotFoundException();
                        status[0] = "ERROR : File Path is not correct";
                        return status;
                    }

                    else if (Is_file_exist(pathfile) == 0)          //path는 있지만, 해당 경로에 파일이 없는 경우
                    {
                        Console.WriteLine("File is not exists");
                        //throw new FileNotFoundException();
                        status[0] = "ERROR : File is not exists";
                        return status;
                    }

                    else
                    {
                        StreamReader sr = new StreamReader(pathfile, Encoding.Default);     //한글 깨짐 방지
                        Encoding en = sr.CurrentEncoding;                                        //한글 깨짐 방지
                        string data;

                        string[] value_data = new string[100];
                        int i = 0;
                        int section_flag = 0;

                        while ((data = sr.ReadLine()) != null)
                        {
                            if (section_flag == 1)
                            {
                                if (data.Contains('['))
                                {
                                    section_flag = 0;
                                }
                                else
                                {
                                    value_data[i] = data;
                                    i += 1;
                                }

                            }

                            if (data.Contains('['))
                            {
                                if (data.Contains(section))
                                {
                                    section_flag = 1;
                                }
                            }
                        }
                        sr.Close();
                        if (i == 0)
                        {
                            Console.WriteLine("해당 섹션이 없습니다.");
                            //throw new Exception("해당 섹션이 없습니다.");
                            status[0] = "ERROR : 해당 섹션이 없습니다.";
                            return status;

                        }
                        return value_data;
                    }
                }
                catch (Exception ex)
                {
                    return status;
                }
            }
            #endregion

            

            /*
             * 
             * FileInfo MSDN
             * Exists 반환 값
             * Type: System.Boolean 호출자에게 필요한 권한이 있고 true에 기존 파일의 이름이 포함되면 path이고, 
             * 그렇지 않으면 false입니다. 또한 이 메서드는 false가 path이거나 잘못된 경로이거나 빈 문자열이면 null를 반환합니다. 
             * 호출자에게 지정된 파일을 읽을 권한이 없는 경우 예외가 throw되지 않으며 false가 있는지 여부와 관계없이 path를 반환합니다.
             * 
             */

            //라이브러리 내부에서만 사용
            private static int Is_file_exist(string filepath)
            {
                FileInfo my_file = new FileInfo(filepath);
                if (my_file.Exists)                         //파일이 존재하는지 체크
                {
                    return 1;
                }
                else
                {
                    return 0;
                }

            }


        }
    }
}