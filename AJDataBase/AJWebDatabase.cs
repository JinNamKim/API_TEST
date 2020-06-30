using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AJParkLib.AJWebdataClass;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.IO;


namespace AJParkLib
{
    //namespace AJWebDatabase
    namespace AJDataBase
    {
        public static class AJWebDatabase
        {
            public static string AJ_IP;
            public static string AJ_PORT;
            
            public static bool use_Log_Write = true;        //0224 진남 추가, 로그 쓰기 플래그

            public static void AJ_Set_IP(string str_ip)
            {
                AJ_IP = str_ip;
            }

            public static void AJ_Set_PORT(string str_port)
            {
                AJ_PORT = str_port;
            }

            public static string AJ_Get_IP()
            {
                return AJ_IP;
            }

            public static string AJ_Get_PORT()
            {
                return AJ_PORT;
            }

            private static string AJ_Web_Access(RestClient client, RestRequest request)
            {
                IRestResponse response = null;
                string str_response = "";
                try
                {
                    response = client.Execute(request);
                    str_response = response.Content;
                    return str_response;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(e.StackTrace.ToString());

                    str_response = "ERROR";
                    return str_response;
                }
                /*
                 //쓰레드로 타임아웃 추가
                IRestResponse response = null;
                System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(delegate()
                {
                    try
                    {
                        response = client.Execute(request);
                    }
                    catch (System.Exception ex)
                    {

                    }
                }));

                t.Start();
                bool completed = t.Join(3000); //half a sec of timeout
                string str_response="";
                if (!completed)
                {
                    t.Interrupt();                                                  //실패

                }
                else                                                               //성공
                {
                    //response = client.Execute(request);                             
                    str_response = response.Content;
                }

                return str_response;
                */
            }


            #region 1번
            public static AJ_RESPONSE_initializedInfo AJ_Get_Initial_Info(int localEquipmentId)
            {
               
                ///var client = new RestClient("http://112.216.153.186:2080//api/equipment/get/initializedInfo");
                string Server_path = "http://" + AJ_IP + ":" + AJ_PORT;
                Server_path += "//api/equipment/get/initializedInfo";

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 01. 초기 장비 세팅 정보 시작 === ");
                    AJParkLib.AJCommon.CommonClass.SendLog("접속 주소 : " + Server_path);
                }
                //로그 기록

                AJ_RESPONSE_initializedInfo temp = new AJ_RESPONSE_initializedInfo();

                try
                {
                    var client = new RestClient(Server_path);
                    var request = new RestRequest(Method.POST);

                    request.AddParameter("localEquipmentId", localEquipmentId);     //170110 파라미터 추가

                    //파라미터 로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog(""); 
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 ------ ");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("localEquipmentId(장비 UID) : {0}", localEquipmentId));
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 끝 ------ ");
                    }
                    //파라미터 로그 기록 끝.
                    
                    
                    string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드
                    if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                    {
                        temp.code = "181818";
                        //서버에서 응답 자체를 못받았을 경우
                        AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                    }

                    else                                //정보를 받아왔을 때
                    {
                        try
                        {
                            JObject obj = JObject.Parse(str_response);

                            //여기부터 데이터 파싱

                            temp.code = (string)obj["code"];                        //ini에 code 정보 붙임
                            temp.errmsgvariable = (string)obj["errmsgvariable"];    //ini에 errmsgvariable 정보 붙임
                            temp.errmsg = (string)obj["errmsg"];                    //ini에 errmsg 정보 붙임
                            temp.key = (string)obj["key"];                          //ini에 key 정보 붙임

                            //로그 기록
                            if (use_Log_Write == true)
                            {
                                AJParkLib.AJCommon.CommonClass.SendLog("");
                                AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 ------ ");
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("code : {0}", temp.code));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsgvariable : {0}", temp.errmsgvariable));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsg : {0}", temp.errmsg));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("key : {0}", temp.key));
                            }
                            //로그 기록

                            int lpr_count = obj["lprInfo"].Count();
                            lprInfo_RESPONSE[] lprInfo_list = new lprInfo_RESPONSE[lpr_count];    //lprinfo 구조체 선언

                            if (use_Log_Write == true)
                            {
                                AJParkLib.AJCommon.CommonClass.SendLog("");
                                AJParkLib.AJCommon.CommonClass.SendLog(" ------ lprInfo 리스트 관련 응답 ------ ");
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("lprInfo의 개수 : {0}", lpr_count));
                            }
                            

                            for (int i = 0; i < lpr_count; i++)                 //엘피알인포 탐색
                            {
                                lprInfo_list[i].localEquipmentId = (string)obj["lprInfo"][i]["localEquipmentId"];
                                lprInfo_list[i].type = (string)obj["lprInfo"][i]["type"];
                                lprInfo_list[i].name = (string)obj["lprInfo"][i]["name"];
                                lprInfo_list[i].equipmentStatus = (string)obj["lprInfo"][i]["equipmentStatus"];
                                lprInfo_list[i].equipmentNo = (string)obj["lprInfo"][i]["equipmentNo"];
                                lprInfo_list[i].equipmentIp = (string)obj["lprInfo"][i]["equipmentIp"];
                                lprInfo_list[i].equipmentPort = (string)obj["lprInfo"][i]["equipmentPort"];
                                lprInfo_list[i].displayIp = (string)obj["lprInfo"][i]["displayIp"];
                                lprInfo_list[i].displayPort = (string)obj["lprInfo"][i]["displayPort"];
                                lprInfo_list[i].barrierPort = (string)obj["lprInfo"][i]["barrierPort"];
                                lprInfo_list[i].location = (string)obj["lprInfo"][i]["location"];
                                lprInfo_list[i].barrierControlOption = (string)obj["lprInfo"][i]["barrierControlOption"];
                                lprInfo_list[i].carInfoSave = (string)obj["lprInfo"][i]["carInfoSave"];
                                lprInfo_list[i].localCkId = (string)obj["lprInfo"][i]["localCkId"];

                                //lprInfo 로그기록 시작
                                if(use_Log_Write == true)
                                {
                                    AJParkLib.AJCommon.CommonClass.SendLog("");       //로그 기록
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 lprInfo -> localEquipmentId : {1}", i, lprInfo_list[i].localEquipmentId));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 lprInfo -> type : {1}", i, lprInfo_list[i].type));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 lprInfo -> name : {1}", i, lprInfo_list[i].name));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 lprInfo -> equipmentStatus : {1}", i, lprInfo_list[i].equipmentStatus));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 lprInfo -> equipmentNo : {1}", i, lprInfo_list[i].equipmentNo));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 lprInfo -> equipmentIp : {1}", i, lprInfo_list[i].equipmentIp));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 lprInfo -> equipmentPort : {1}", i, lprInfo_list[i].equipmentPort));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 lprInfo -> displayIp : {1}", i, lprInfo_list[i].displayIp));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 lprInfo -> displayPort : {1}", i, lprInfo_list[i].displayPort));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 lprInfo -> barrierPort : {1}", i, lprInfo_list[i].barrierPort));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 lprInfo -> location : {1}", i, lprInfo_list[i].location));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 lprInfo -> barrierControlOption : {1}", i, lprInfo_list[i].barrierControlOption));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 lprInfo -> carInfoSave : {1}", i, lprInfo_list[i].carInfoSave));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 lprInfo -> localCkId : {1}", i, lprInfo_list[i].localCkId));
                                }
                                //lprInfo 로그기록 끝
                            }

                            //로그 기록
                            if(use_Log_Write == true)
                            {
                                AJParkLib.AJCommon.CommonClass.SendLog(" ------ lprInfo 리스트 관련 응답 끝 ------ ");
                            }
                            //로그 기록
                            temp.lprInfo_List = lprInfo_list;                        //ini에 lpr 정보 붙임



                            int kiosk_count = obj["kioskInfo"].Count();

                            //로그 기록
                            if(use_Log_Write == true)
                            {
                                AJParkLib.AJCommon.CommonClass.SendLog("");
                                AJParkLib.AJCommon.CommonClass.SendLog(" ------ kioskInfo 리스트 관련 응답 ------ ");
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("kioskInfo 개수 : {0}", kiosk_count));
                            }
                            //로그 기록
                            

                            kioskInfo_RESPONSE[] kioskInfo_list = new kioskInfo_RESPONSE[kiosk_count];    //kioskinfo 구조체 선언

                            for(int i = 0; i < kiosk_count; i++)               //키오스크 인포 탐색
                            {

                                kioskInfo_list[i].localEquipmentId = (string)obj["kioskInfo"][i]["localEquipmentId"];
                                kioskInfo_list[i].type = (string)obj["kioskInfo"][i]["type"];
                                kioskInfo_list[i].name = (string)obj["kioskInfo"][i]["name"];
                                kioskInfo_list[i].equipmentStatus = (string)obj["kioskInfo"][i]["equipmentStatus"];
                                kioskInfo_list[i].equipmentNo = (string)obj["kioskInfo"][i]["equipmentNo"];
                                kioskInfo_list[i].powerIp = (string)obj["kioskInfo"][i]["powerIp"];             //1.0 버전 추가
                                kioskInfo_list[i].powerPort = (string)obj["kioskInfo"][i]["powerPort"];         //1.0 버전 추가
                                kioskInfo_list[i].equipmentIp = (string)obj["kioskInfo"][i]["equipmentIp"];
                                kioskInfo_list[i].equipmentPort = (string)obj["kioskInfo"][i]["equipmentPort"];
                                kioskInfo_list[i].displayIp = (string)obj["kioskInfo"][i]["displayIp"];
                                kioskInfo_list[i].displayPort = (string)obj["kioskInfo"][i]["displayPort"];
                                kioskInfo_list[i].barrierPort = (string)obj["kioskInfo"][i]["barrierPort"];
                                kioskInfo_list[i].location = (string)obj["kioskInfo"][i]["location"];
                                kioskInfo_list[i].dvrIp = (string)obj["kioskInfo"][i]["dvrIp"];
                                kioskInfo_list[i].dvrPort = (string)obj["kioskInfo"][i]["dvrPort"];
                                kioskInfo_list[i].won500Price = (string)obj["kioskInfo"][i]["won500Price"];
                                kioskInfo_list[i].won100Price = (string)obj["kioskInfo"][i]["won100Price"];

                                //kioskInfo 로그기록 시작
                                if(use_Log_Write == true)
                                {
                                    AJParkLib.AJCommon.CommonClass.SendLog("");
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 kioskInfo -> localEquipmentId : {1}", i, kioskInfo_list[i].localEquipmentId));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 kioskInfo -> type : {1}", i, kioskInfo_list[i].type));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 kioskInfo -> name : {1}", i, kioskInfo_list[i].name));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 kioskInfo -> equipmentStatus : {1}", i, kioskInfo_list[i].equipmentStatus));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 kioskInfo -> equipmentNo : {1}", i, kioskInfo_list[i].equipmentNo));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 kioskInfo -> powerIp : {1}", i, kioskInfo_list[i].powerIp));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 kioskInfo -> powerPort : {1}", i, kioskInfo_list[i].powerPort));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 kioskInfo -> equipmentIp : {1}", i, kioskInfo_list[i].equipmentIp));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 kioskInfo -> equipmentPort : {1}", i, kioskInfo_list[i].equipmentPort));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 kioskInfo -> displayIp : {1}", i, kioskInfo_list[i].displayIp));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 kioskInfo -> displayPort : {1}", i, kioskInfo_list[i].displayPort));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 kioskInfo -> barrierPort : {1}", i, kioskInfo_list[i].barrierPort));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 kioskInfo -> location : {1}", i, kioskInfo_list[i].location));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 kioskInfo -> dvrIp : {1}", i, kioskInfo_list[i].dvrIp));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 kioskInfo -> dvrPort : {1}", i, kioskInfo_list[i].dvrPort));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 kioskInfo -> won500Price : {1}", i, kioskInfo_list[i].won500Price));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 kioskInfo -> won100Price : {1}", i, kioskInfo_list[i].won100Price));
                                }
                                //kioskInfo 로그기록 끝

                                //로그 기록
                                if(use_Log_Write == true)
                                {
                                    AJParkLib.AJCommon.CommonClass.SendLog("");
                                    AJParkLib.AJCommon.CommonClass.SendLog(" ------ kiosk child 리스트 관련 응답 ------ ");
                                }
                                //로그 기록
                                
                                int kiosk_child_count = obj["kioskInfo"][i]["kioskList"].Count();

                                //로그 기록
                                if (use_Log_Write == true)
                                {
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("kiosk child 개수 : {0}", kiosk_child_count));
                                }
                                //로그 기록
                                

                                kioskChild_RESPONSE[] kioskChild_list = new kioskChild_RESPONSE[kiosk_child_count];       //kioschild 구조체 선언

                                for (int j = 0; j < kiosk_child_count; j++)         //키오스크 차일드 리스트 탐색
                                {
                                    kioskChild_list[j].localEquipmentKioskId = (string)obj["kioskInfo"][i]["kioskList"][j]["localEquipmentKioskId"];
                                    kioskChild_list[j].name = (string)obj["kioskInfo"][i]["kioskList"][j]["name"];
                                    kioskChild_list[j].port = (string)obj["kioskInfo"][i]["kioskList"][j]["port"];
                                    kioskChild_list[j].localEquipmentId = (string)obj["kioskInfo"][i]["kioskList"][j]["localEquipmentId"];
                                    kioskChild_list[j].equipmentKioskStatus = (string)obj["kioskInfo"][i]["kioskList"][j]["equipmentKioskStatus"];
                                    kioskChild_list[j].isUse = (string)obj["kioskInfo"][i]["kioskList"][j]["isUse"];

                                    //kiosk child 로그기록 시작
                                    if(use_Log_Write == true)
                                    {
                                        AJParkLib.AJCommon.CommonClass.SendLog("");
                                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 kiosk child ->  localEquipmentKioskId : {1}", j, kioskChild_list[j].localEquipmentKioskId));
                                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 kiosk child ->  name : {0}", j, kioskChild_list[j].name));
                                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 kiosk child ->  port : {0}", j, kioskChild_list[j].port));
                                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 kiosk child ->  localEquipmentId : {0}", j, kioskChild_list[j].localEquipmentId));
                                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 kiosk child ->  equipmentKioskStatus : {0}", j, kioskChild_list[j].equipmentKioskStatus));
                                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 kiosk child ->  isUse : {0}", j, kioskChild_list[j].isUse));
                                    }
                                    //kiosk child 로그기록 끝

                                }
                                kioskInfo_list[i].kioskchildList = kioskChild_list;                 //키오스크 인포에 키오스크 차일드 정보 붙임
                                //로그 기록
                                if (use_Log_Write == true)
                                {
                                    AJParkLib.AJCommon.CommonClass.SendLog(" ------ kiosk child 리스트 관련 응답 끝 ------ ");
                                }
                                //로그 기록
                                
                            }
                            temp.kioskInfo_List = kioskInfo_list;                                //ini에 키오스크 인포 정보 붙임
                            //로그 기록
                            if (use_Log_Write == true)
                            {
                                AJParkLib.AJCommon.CommonClass.SendLog(" ------ kioskInfo 리스트 관련 응답 끝 ------ ");
                                AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 끝------ ");
                                AJParkLib.AJCommon.CommonClass.SendLog("전체 동작 성공");
                            }
                            //로그 기록
                            
                            //return temp;
                        }

                        catch (Exception e)         //파싱 중 에러
                        {
                            Console.WriteLine(e.ToString());
                            Console.WriteLine(e.StackTrace.ToString());

                            //로그 기록
                            if(use_Log_Write == true)
                            {
                                AJParkLib.AJCommon.CommonClass.SendLog("동작 실패");
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e : {0}", e));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e.StackTrace : {0}", e.StackTrace));
                            }
                            //로그 기록
                            temp.code = "8818";

                            
                            //return temp;
                        }

                        finally
                        {
                            
                        }
                        return temp;

                    }

                }

                catch (Exception e)               //통신 중 에러
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(e.StackTrace.ToString());

                    //로그 기록
                    if(use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("동작 실패");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e : {0}", e));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e.StackTrace : {0}", e.StackTrace));
                    }
                    //로그 기록
                    

                    temp.code = "1818";
                    //return temp;
                }

                finally
                {
                    //로그 기록
                    if(use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("모든 파싱 종료");       //로그 기록
                    }
                    //로그 기록
                }
                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 01. 초기 장비 세팅 정보 끝 ===");       //로그 기록
                }
                //로그 기록
                return temp;
                
                 

                
            }
            #endregion

            #region 2번
            public static AJ_RESPONSE_CloseInfo temp_name_2(int localEquipmentId, string fromTimestamp_date, DateTime toTimestamp_date)
            {
                //var client = new RestClient("http://112.216.153.186:2080//api/equipment/get/closeInfo");
                string Server_path = "http://" + AJ_IP + ":" + AJ_PORT;
                Server_path += "//api/equipment/get/closeInfo";

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 02. 마감 정보 시작 === ");
                    AJParkLib.AJCommon.CommonClass.SendLog("접속 주소 : " + Server_path);
                }
                //로그 기록

                AJ_RESPONSE_CloseInfo temp = new AJ_RESPONSE_CloseInfo();             //리스폰스 변수 생성
                try
                {
                    var client = new RestClient(Server_path);
                    var request = new RestRequest(Method.POST);

                    //파라미터 추가
                    request.AddParameter("localEquipmentId", localEquipmentId);
                    double fromTimestamp;
                    string str_fromTimestamp = "";
                    if (fromTimestamp_date != "")
                    {
                        fromTimestamp = AJParkLib.AJCommon.Time.DateTime_To_MilliSec(Convert.ToDateTime(fromTimestamp_date));
                        str_fromTimestamp = Math.Floor(fromTimestamp).ToString();
                    }
                    str_fromTimestamp = str_fromTimestamp.Replace(".", "");
                    request.AddParameter("fromTimestamp", str_fromTimestamp);

                    double toTimestamp = AJParkLib.AJCommon.Time.DateTime_To_MilliSec(toTimestamp_date);
                    string str_toTimestamp = Math.Floor(toTimestamp).ToString();
                    //string str_toTimestamp = toTimestamp.ToString();
                    //str_toTimestamp = str_toTimestamp.Replace(".", "");
                    request.AddParameter("toTimestamp", str_toTimestamp);
                    //파라미터 추가 끝.

                    //파라미터 로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("");
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 ------ ");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("localEquipmentId : {0}", localEquipmentId));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("fromTimestamp(TimeStamp) : {0}", str_fromTimestamp));
                        if (str_fromTimestamp != "")
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("fromTimestamp(DateTime) : {0}", AJParkLib.AJCommon.Time.MilliSec_To_DateTime(Convert.ToDouble(str_fromTimestamp))));
                        }
                        else
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("fromTimestamp(DateTime) : {0}", " "));
                        }
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("toTimestamp(TimeStamp) : {0}", str_toTimestamp));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("toTimestamp(DateTime) : {0}", AJParkLib.AJCommon.Time.MilliSec_To_DateTime(Convert.ToDouble(str_toTimestamp))));
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 끝 ------ ");
                    }
                    //파라미터 로그 기록 끝.

                    string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드

                    if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                    {
                        temp.code = "181818";
                        //서버에서 응답 자체를 못받았을 경우
                        AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                    }
                    else
                    {
                        JObject obj = JObject.Parse(str_response);

                        //파싱 시작
                        temp.code = (string)obj["code"];                        //ini에 code 정보 붙임
                        temp.errmsgvariable = (string)obj["errmsgvariable"];    //ini에 errmsgvariable 정보 붙임
                        temp.errmsg = (string)obj["errmsg"];                    //ini에 errmsg 정보 붙임
                        temp.key = (string)obj["key"];                          //ini에 key 정보 붙임

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("code : {0}", temp.code));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsgvariable : {0}", temp.errmsgvariable));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsg : {0}", temp.errmsg));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("key : {0}", temp.key));
                        }
                        //로그 기록



                        //클로즈 인포(단일)
                        closeinfo_list_RESPONSE closeinfo_list_temp = new closeinfo_list_RESPONSE();
                        closeinfo_list_temp.parkingLotName = (string)obj["closeInfo"]["parkingLotName"];
                        closeinfo_list_temp.companyName = (string)obj["closeInfo"]["companyName"];
                        closeinfo_list_temp.companyNumber = (string)obj["closeInfo"]["companyNumber"];
                        closeinfo_list_temp.parkingLotAddr = (string)obj["closeInfo"]["parkingLotAddr"];
                        closeinfo_list_temp.fromTimestamp = (string)obj["closeInfo"]["fromTimestamp"];
                        closeinfo_list_temp.toTimestamp = (string)obj["closeInfo"]["toTimestamp"];
                        closeinfo_list_temp.manualOpenCount = (string)obj["closeInfo"]["manualOpenCount"];
                        closeinfo_list_temp.printDate = (string)obj["closeInfo"]["printDate"];
                        closeinfo_list_temp.equipmentNo = (string)obj["closeInfo"]["equipmentNo"];



                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포의 단일 키 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("parkingLotName : {0}", closeinfo_list_temp.parkingLotName));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("companyName : {0}", closeinfo_list_temp.companyName));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("companyNumber : {0}", closeinfo_list_temp.companyNumber));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("fromTimestamp : {0}", closeinfo_list_temp.fromTimestamp));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format(" - fromTimestamp(DateTime) : {0}", AJParkLib.AJCommon.Time.MilliSec_To_DateTime(Convert.ToDouble(closeinfo_list_temp.fromTimestamp))));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("toTimestamp : {0}", closeinfo_list_temp.toTimestamp));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format(" - toTimestamp(DateTime) : {0}", AJParkLib.AJCommon.Time.MilliSec_To_DateTime(Convert.ToDouble(closeinfo_list_temp.toTimestamp))));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("manualOpenCount : {0}", closeinfo_list_temp.manualOpenCount));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("printDate : {0}", closeinfo_list_temp.printDate));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format(" - printDate(DateTime) : {0}", AJParkLib.AJCommon.Time.MilliSec_To_DateTime(Convert.ToDouble(closeinfo_list_temp.printDate))));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("equipmentNo : {0}", closeinfo_list_temp.equipmentNo));
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포의 단일 키 끝 ------ ");
                        }
                        //로그 기록

                        //클로즈 인포 - 인컴캐쉬
                        int incomeCash_count = obj["closeInfo"]["incomeCash"].Count();
                        closeinfo_list_RESPONSE_list_incomeCash[] incomeCash_list_temp = new closeinfo_list_RESPONSE_list_incomeCash[incomeCash_count];


                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 인컴캐쉬 리스트------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("인컴캐쉬 리스트의 개수 : {0}", incomeCash_count));
                        }
                        //로그 기록
                        

                        for (int i = 0; i < incomeCash_count; i++)
                        {
                            incomeCash_list_temp[i].releaseCount = (string)obj["closeInfo"]["incomeCash"][i]["releaseCount"];
                            incomeCash_list_temp[i].releasePrice = (string)obj["closeInfo"]["incomeCash"][i]["releasePrice"];
                            incomeCash_list_temp[i].insertCount = (string)obj["closeInfo"]["incomeCash"][i]["insertCount"];
                            incomeCash_list_temp[i].insertPrice = (string)obj["closeInfo"]["incomeCash"][i]["insertPrice"];
                            
                            //로그 기록
                            if (use_Log_Write == true)
                            {
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 인컴캐쉬 리스트 -> releaseCount : {1}", i, incomeCash_list_temp[i].releaseCount));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 인컴캐쉬 리스트 -> releasePrice : {1}", i, incomeCash_list_temp[i].releasePrice));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 인컴캐쉬 리스트 -> insertCount : {1}", i, incomeCash_list_temp[i].insertCount));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 인컴캐쉬 리스트 -> insertPrice : {1}", i, incomeCash_list_temp[i].insertPrice));
                                AJParkLib.AJCommon.CommonClass.SendLog("");
                            }
                            //로그 기록
                        }
                        closeinfo_list_temp.incomeCash_list = incomeCash_list_temp;
                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 인컴캐쉬 리스트 끝 ------ ");
                        }
                        //로그 기록
                        


                        //클로즈 인포 - 인컴낫캐쉬
                        int incomeNotCash_count = obj["closeInfo"]["incomeNotCash"].Count();
                        closeinfo_list_RESPONSE_list_incomeNotCash[] incomeNotCash_list_temp = new closeinfo_list_RESPONSE_list_incomeNotCash[incomeNotCash_count];


                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 인컴낫캐쉬 리스트------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("인컴낫캐쉬 리스트의 개수 : {0}", incomeNotCash_count));
                        }
                        //로그 기록

                        for (int i = 0; i < incomeNotCash_count; i++)
                        {
                            incomeNotCash_list_temp[i].price = (string)obj["closeInfo"]["incomeNotCash"][i]["price"];
                            incomeNotCash_list_temp[i].count = (string)obj["closeInfo"]["incomeNotCash"][i]["count"];
                            incomeNotCash_list_temp[i].paymentMethod = (string)obj["closeInfo"]["incomeNotCash"][i]["paymentMethod"];

                            //로그 기록
                            if (use_Log_Write == true)
                            {
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 인컴낫캐쉬 리스트 -> price : {1}", i, incomeNotCash_list_temp[i].price));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 인컴캐쉬 리스트 -> count : {1}", i, incomeNotCash_list_temp[i].count));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 인컴캐쉬 리스트 -> paymentMethod : {1}", i, incomeNotCash_list_temp[i].paymentMethod));
                                AJParkLib.AJCommon.CommonClass.SendLog("");
                            }
                            //로그 기록
                        }
                        closeinfo_list_temp.incomeNotCash_list = incomeNotCash_list_temp;
                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 인컴낫캐쉬 리스트 끝 ------ ");
                        }
                        //로그 기록
                        

                        //클로즈 인포 - 인컴토탈
                        int incomeTotal_count = obj["closeInfo"]["incomeTotal"].Count();
                        closeinfo_list_RESPONSE_list_incomeTotal[] incomeTotal_list_temp = new closeinfo_list_RESPONSE_list_incomeTotal[incomeTotal_count];

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 인컴토탈 리스트------ ");       //로그 기록
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("인컴토탈 리스트의 개수 : {0}", incomeTotal_count));
                        }
                        //로그 기록

                        for (int i = 0; i < incomeTotal_count; i++)
                        {
                            incomeTotal_list_temp[i].total = (string)obj["closeInfo"]["incomeTotal"][i]["total"];
                            incomeTotal_list_temp[i].count = (string)obj["closeInfo"]["incomeTotal"][i]["count"];
                            incomeTotal_list_temp[i].price = (string)obj["closeInfo"]["incomeTotal"][i]["price"];

                            //로그 기록
                            if (use_Log_Write == true)
                            {
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 인컴토탈 리스트 -> total : {1}", i, incomeTotal_list_temp[i].total));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 인컴토탈 리스트 -> count : {1}", i, incomeTotal_list_temp[i].count));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 인컴토탈 리스트 -> price : {1}", i, incomeTotal_list_temp[i].price));
                                AJParkLib.AJCommon.CommonClass.SendLog("");
                            }
                            //로그 기록
                        }
                        closeinfo_list_temp.incomeTotal_list = incomeTotal_list_temp;
                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 인컴토탈 리스트 끝 ------ ");
                        }
                        //로그 기록

                        //클로즈 인포 - 디스카운트 인포 디테일
                        int discountInfoDetail_count = obj["closeInfo"]["discountInfoDetail"].Count();
                        closeinfo_list_RESPONSE_list_discountInfoDetail[] discountInfoDetail_list_temp = new closeinfo_list_RESPONSE_list_discountInfoDetail[discountInfoDetail_count];

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 디스카운트 인포 디테일 리스트------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("디스카운트 인포 디테일 리스트의 개수 : {0}",discountInfoDetail_count));
                        }
                        //로그 기록

                        for (int i = 0; i < discountInfoDetail_count; i++)
                        {
                            discountInfoDetail_list_temp[i].name = (string)obj["closeInfo"]["discountInfoDetail"][i]["name"];
                            discountInfoDetail_list_temp[i].count = (string)obj["closeInfo"]["discountInfoDetail"][i]["count"];
                            discountInfoDetail_list_temp[i].discountPrice = (string)obj["closeInfo"]["discountInfoDetail"][i]["discountPrice"];

                            //로그 기록
                            if (use_Log_Write == true)
                            {
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 디스카운트 인포 디테일 리스트 -> total : {1}", i, discountInfoDetail_list_temp[i].name));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 디스카운트 인포 디테일 리스트 -> count : {1}", i, discountInfoDetail_list_temp[i].count));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 디스카운트 인포 디테일 리스트 -> discountPrice : {1}", i, discountInfoDetail_list_temp[i].discountPrice));
                                AJParkLib.AJCommon.CommonClass.SendLog("");
                            }
                            //로그 기록
                        }
                        closeinfo_list_temp.discountInfoDetail_list = discountInfoDetail_list_temp;
                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 디스카운트 인포 디테일 리스트 끝 ------ ");
                        }
                        //로그 기록
                        

                        //클로즈 인포 - 디스카운트 인포 토탈
                        int discountInfoTotal_count = obj["closeInfo"]["discountInfoTotal"].Count();
                        closeinfo_list_RESPONSE_list_discountInfoTotal[] discountInfoTotal_list_temp = new closeinfo_list_RESPONSE_list_discountInfoTotal[discountInfoTotal_count];

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 디스카운트 인포 토탈 리스트------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("디스카운트 인포 토탈 리스트의 개수 : {0}", discountInfoTotal_count));
                        }
                        //로그 기록
                        

                        for (int i = 0; i < discountInfoTotal_count; i++)
                        {
                            discountInfoTotal_list_temp[i].total = (string)obj["closeInfo"]["discountInfoTotal"][i]["total"];
                            discountInfoTotal_list_temp[i].count = (string)obj["closeInfo"]["discountInfoTotal"][i]["count"];
                            discountInfoTotal_list_temp[i].discountPrice = (string)obj["closeInfo"]["discountInfoTotal"][i]["discountPrice"];


                            //로그 기록
                            if (use_Log_Write == true)
                            {
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 디스카운트 인포 토탈 리스트 -> total : {1}", i, discountInfoTotal_list_temp[i].total));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 디스카운트 인포 토탈 리스트 -> count : {1}", i, discountInfoTotal_list_temp[i].count));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 디스카운트 인포 토탈 리스트 -> discountPrice : {1}", i, discountInfoTotal_list_temp[i].discountPrice));
                                AJParkLib.AJCommon.CommonClass.SendLog("");
                            }
                            //로그 기록
                        }
                        closeinfo_list_temp.discountInfoTotal_list = discountInfoTotal_list_temp;
                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 디스카운트 인포 토탈 리스트 끝 ------ ");
                        }
                        //로그 기록


                        //클로즈 인포 - 리텐션 캐쉬 인포
                        int retentionCashInfo_count = obj["closeInfo"]["retentionCashInfo"].Count();
                        closeinfo_list_RESPONSE_list_retentionCashInfo[] retentionCashInfo_list_temp = new closeinfo_list_RESPONSE_list_retentionCashInfo[retentionCashInfo_count];

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");       //로그 기록
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 리텐션 캐쉬 인포 리스트------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("리텐션 캐쉬 인포 리스트의 개수 : {0}", retentionCashInfo_count));
                        }
                        //로그 기록

                        for (int i = 0; i < retentionCashInfo_count; i++)
                        {
                            retentionCashInfo_list_temp[i].localEquipmentId = (string)obj["closeInfo"]["retentionCashInfo"][i]["localEquipmentId"];
                            retentionCashInfo_list_temp[i].init5000 = (string)obj["closeInfo"]["retentionCashInfo"][i]["init5000"];
                            retentionCashInfo_list_temp[i].init1000 = (string)obj["closeInfo"]["retentionCashInfo"][i]["init1000"];
                            retentionCashInfo_list_temp[i].init500 = (string)obj["closeInfo"]["retentionCashInfo"][i]["init500"];
                            retentionCashInfo_list_temp[i].init100 = (string)obj["closeInfo"]["retentionCashInfo"][i]["init100"];
                            retentionCashInfo_list_temp[i].totalInit = (string)obj["closeInfo"]["retentionCashInfo"][i]["totalInit"];
                            retentionCashInfo_list_temp[i].current5000 = (string)obj["closeInfo"]["retentionCashInfo"][i]["current5000"];
                            retentionCashInfo_list_temp[i].current1000 = (string)obj["closeInfo"]["retentionCashInfo"][i]["current1000"];
                            retentionCashInfo_list_temp[i].current500 = (string)obj["closeInfo"]["retentionCashInfo"][i]["current500"];
                            retentionCashInfo_list_temp[i].current100 = (string)obj["closeInfo"]["retentionCashInfo"][i]["current100"];
                            retentionCashInfo_list_temp[i].totalCurrent = (string)obj["closeInfo"]["retentionCashInfo"][i]["totalCurrent"];


                            //로그 기록
                            if (use_Log_Write == true)
                            {
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리텐션 캐쉬 인포 리스트 -> localEquipmentId : {1}", i, retentionCashInfo_list_temp[i].localEquipmentId));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리텐션 캐쉬 인포 리스트 -> init5000 : {1}", i, retentionCashInfo_list_temp[i].init5000));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리텐션 캐쉬 인포 리스트 -> init1000 : {1}", i, retentionCashInfo_list_temp[i].init1000));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리텐션 캐쉬 인포 리스트 -> init500 : {1}", i, retentionCashInfo_list_temp[i].init500));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리텐션 캐쉬 인포 리스트 -> init100 : {1}", i, retentionCashInfo_list_temp[i].init100));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리텐션 캐쉬 인포 리스트 -> totalInit : {1}", i, retentionCashInfo_list_temp[i].totalInit));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리텐션 캐쉬 인포 리스트 -> current5000 : {1}", i, retentionCashInfo_list_temp[i].current5000));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리텐션 캐쉬 인포 리스트 -> current1000 : {1}", i, retentionCashInfo_list_temp[i].current1000));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리텐션 캐쉬 인포 리스트 -> current500 : {1}", i, retentionCashInfo_list_temp[i].current500));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리텐션 캐쉬 인포 리스트 -> current100 : {1}", i, retentionCashInfo_list_temp[i].current100));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리텐션 캐쉬 인포 리스트 -> totalCurrent : {1}", i, retentionCashInfo_list_temp[i].totalCurrent));
                                AJParkLib.AJCommon.CommonClass.SendLog("");
                            }
                            //로그 기록

                        }
                        closeinfo_list_temp.retentionCashInfo_list = retentionCashInfo_list_temp;
                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 리텐션 캐쉬 인포 리스트 끝------ ");
                        }
                        //로그 기록
                        


                        //클로즈 인포 - 인풋 캐쉬 인포(단일)
                        closeinfo_list_RESPONSE_list_inputCashInfo inputCashInfo_list_temp = new closeinfo_list_RESPONSE_list_inputCashInfo();
                        inputCashInfo_list_temp.count50000 = (string)obj["closeInfo"]["inputCashInfo"]["count50000"];
                        inputCashInfo_list_temp.count10000 = (string)obj["closeInfo"]["inputCashInfo"]["count10000"];
                        inputCashInfo_list_temp.count5000 = (string)obj["closeInfo"]["inputCashInfo"]["count5000"];
                        inputCashInfo_list_temp.count1000 = (string)obj["closeInfo"]["inputCashInfo"]["count1000"];
                        inputCashInfo_list_temp.count500 = (string)obj["closeInfo"]["inputCashInfo"]["count500"];
                        inputCashInfo_list_temp.count100 = (string)obj["closeInfo"]["inputCashInfo"]["count100"];
                        inputCashInfo_list_temp.count50 = (string)obj["closeInfo"]["inputCashInfo"]["count50"];
                        inputCashInfo_list_temp.count10 = (string)obj["closeInfo"]["inputCashInfo"]["count10"];
                        inputCashInfo_list_temp.totalCount = (string)obj["closeInfo"]["inputCashInfo"]["totalCount"];
                        inputCashInfo_list_temp.sum50000 = (string)obj["closeInfo"]["inputCashInfo"]["sum50000"];
                        inputCashInfo_list_temp.sum10000 = (string)obj["closeInfo"]["inputCashInfo"]["sum10000"];
                        inputCashInfo_list_temp.sum5000 = (string)obj["closeInfo"]["inputCashInfo"]["sum5000"];
                        inputCashInfo_list_temp.sum1000 = (string)obj["closeInfo"]["inputCashInfo"]["sum1000"];
                        inputCashInfo_list_temp.sum500 = (string)obj["closeInfo"]["inputCashInfo"]["sum500"];
                        inputCashInfo_list_temp.sum100 = (string)obj["closeInfo"]["inputCashInfo"]["sum100"];
                        inputCashInfo_list_temp.sum50 = (string)obj["closeInfo"]["inputCashInfo"]["sum50"];
                        inputCashInfo_list_temp.sum10 = (string)obj["closeInfo"]["inputCashInfo"]["sum10"];
                        inputCashInfo_list_temp.totalSum = (string)obj["closeInfo"]["inputCashInfo"]["totalSum"];

                        closeinfo_list_temp.inputCashInfo_list = inputCashInfo_list_temp;

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 인풋 캐쉬 인포(단일) ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("count50000 : {0}", inputCashInfo_list_temp.count50000));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("count10000 : {0}", inputCashInfo_list_temp.count10000));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("count5000 : {0}", inputCashInfo_list_temp.count5000));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("count1000 : {0}", inputCashInfo_list_temp.count1000));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("count500 : {0}", inputCashInfo_list_temp.count500));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("count100 : {0}", inputCashInfo_list_temp.count100));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("count50 : {0}", inputCashInfo_list_temp.count50));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("count10 : {0}", inputCashInfo_list_temp.count10));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("totalCount : {0}", inputCashInfo_list_temp.totalCount));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("sum50000 : {0}", inputCashInfo_list_temp.sum50000));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("sum10000 : {0}", inputCashInfo_list_temp.sum10000));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("sum5000 : {0}", inputCashInfo_list_temp.sum5000));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("sum1000 : {0}", inputCashInfo_list_temp.sum1000));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("sum500 : {0}", inputCashInfo_list_temp.sum500));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("sum100 : {0}", inputCashInfo_list_temp.sum100));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("sum50 : {0}", inputCashInfo_list_temp.sum50));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("sum10 : {0}", inputCashInfo_list_temp.sum10));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("totalSum : {0}", inputCashInfo_list_temp.totalSum));
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 인풋 캐쉬 인포(단일) 끝 ------ ");
                        }
                        //로그 기록

                        //클로즈 인포 - 아웃풋 캐쉬 인포(단일)
                        closeinfo_list_RESPONSE_list_outputCashInfo outputCashInfo_list_temp = new closeinfo_list_RESPONSE_list_outputCashInfo();
                        outputCashInfo_list_temp.count5000 = (string)obj["closeInfo"]["outputCashInfo"]["count5000"];
                        outputCashInfo_list_temp.count1000 = (string)obj["closeInfo"]["outputCashInfo"]["count1000"];
                        outputCashInfo_list_temp.count500 = (string)obj["closeInfo"]["outputCashInfo"]["count500"];
                        outputCashInfo_list_temp.count100 = (string)obj["closeInfo"]["outputCashInfo"]["count100"];
                        outputCashInfo_list_temp.totalCount = (string)obj["closeInfo"]["outputCashInfo"]["totalCount"];
                        outputCashInfo_list_temp.sum5000 = (string)obj["closeInfo"]["outputCashInfo"]["sum5000"];
                        outputCashInfo_list_temp.sum1000 = (string)obj["closeInfo"]["outputCashInfo"]["sum1000"];
                        outputCashInfo_list_temp.sum500 = (string)obj["closeInfo"]["outputCashInfo"]["sum500"];
                        outputCashInfo_list_temp.sum100 = (string)obj["closeInfo"]["outputCashInfo"]["sum100"];
                        outputCashInfo_list_temp.totalSum = (string)obj["closeInfo"]["outputCashInfo"]["totalSum"];

                        closeinfo_list_temp.outputCashInfo_list = outputCashInfo_list_temp;

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 아웃풋 캐쉬 인포(단일) ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("count5000 : {0}", outputCashInfo_list_temp.count5000));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("count1000 : {0}", outputCashInfo_list_temp.count1000));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("count500 : {0}", outputCashInfo_list_temp.count500));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("count100 : {0}", outputCashInfo_list_temp.count100));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("totalCount : {0}", outputCashInfo_list_temp.totalCount));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("sum5000 : {0}", outputCashInfo_list_temp.sum5000));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("sum1000 : {0}", outputCashInfo_list_temp.sum1000));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("sum500 : {0}", outputCashInfo_list_temp.sum500));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("sum100 : {0}", outputCashInfo_list_temp.sum100));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("totalSum : {0}", outputCashInfo_list_temp.totalSum));
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 아웃풋 캐쉬 인포(단일) 끝 ------ ");
                        }
                        //로그 기록

                        //클로즈 인포 - 파킹 카 인포(단일)
                        closeinfo_list_RESPONSE_list_parkingCarInfo parkingCarInfo_list_temp = new closeinfo_list_RESPONSE_list_parkingCarInfo();
                        parkingCarInfo_list_temp.monthlyCarCount = (string)obj["closeInfo"]["parkingCarInfo"]["monthlyCarCount"];
                        parkingCarInfo_list_temp.priceCarCount = (string)obj["closeInfo"]["parkingCarInfo"]["priceCarCount"];
                        parkingCarInfo_list_temp.recurrenceCarCount = (string)obj["closeInfo"]["parkingCarInfo"]["recurrenceCarCount"];

                        closeinfo_list_temp.parkingCarInfo_list = parkingCarInfo_list_temp;

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 파킹 카 인포(단일) ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("monthlyCarCount : {0}", parkingCarInfo_list_temp.monthlyCarCount));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("priceCarCount : {0}", parkingCarInfo_list_temp.priceCarCount));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("recurrenceCarCount : {0}", parkingCarInfo_list_temp.recurrenceCarCount));
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 파킹 카 인포(단일) 끝 ------ ");
                        }
                        //로그 기록

                        //클로즈 인포 - 노 페이 파킹(단일)
                        closeinfo_list_RESPONSE_list_noPayParking noPayParking_list_temp = new closeinfo_list_RESPONSE_list_noPayParking();
                        noPayParking_list_temp.name = (string)obj["closeInfo"]["noPayParking"]["name"];
                        noPayParking_list_temp.sumCount = (string)obj["closeInfo"]["noPayParking"]["sumCount"];
                        noPayParking_list_temp.sumPrice = (string)obj["closeInfo"]["noPayParking"]["sumPrice"];

                        closeinfo_list_temp.noPayParking_list = noPayParking_list_temp;

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");       //로그 기록
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 노 페이 파킹(단일) ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("name : {0}", noPayParking_list_temp.name));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("sumCount : {0}", noPayParking_list_temp.sumCount));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("sumPrice : {0}", noPayParking_list_temp.sumPrice));
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 노 페이 파킹(단일) 끝 ------ ");
                        }
                        //로그 기록

                        //리스트 붙이기 끝.


                        //전부 다 붙였으니까 이제 응답 클래스에 넣기
                        temp.CloseInfo_list = closeinfo_list_temp;

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트  끝. ------ ");       //로그 기록
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 끝 ------ ");       //로그 기록
                            AJParkLib.AJCommon.CommonClass.SendLog("전체 동작 성공");       //로그 기록
                        }
                        
                    }
                    //return temp;


                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(e.StackTrace.ToString());

                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("동작 실패");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format(" - e : {0}", e));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format(" - e.StackTrace. : {0}", e.StackTrace));
                    }
                    

                    temp.code = "1818";
                    //return temp;
                }
                finally
                {
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("모든 파싱 종료");       //로그 기록
                    }
                    
                }
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 02. 마감 정보 끝 === ");       //로그 기록
                }
                return temp;

            }
            #endregion

            #region 2_1번 진남 테스트!!!!
            public static AJ_RESPONSE_CloseInfo temp_name_2_1(int localEquipmentId, DateTime fromTimestamp_date, DateTime toTimestamp_date)
            {
                //var client = new RestClient("http://112.216.153.186:2080//api/equipment/get/closeInfo");
                string Server_path = "http://" + AJ_IP + ":" + AJ_PORT;
                Server_path += "//api/equipment/get/closeInfo";

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 02. 마감 정보 시작 === ");
                    AJParkLib.AJCommon.CommonClass.SendLog("접속 주소 : " + Server_path);
                }
                //로그 기록

                AJ_RESPONSE_CloseInfo temp = new AJ_RESPONSE_CloseInfo();             //리스폰스 변수 생성
                try
                {
                    var client = new RestClient(Server_path);
                    var request = new RestRequest(Method.POST);

                    //파라미터 추가
                    request.AddParameter("localEquipmentId", localEquipmentId);
                    double fromTimestamp;
                    string str_fromTimestamp = "";
                    
                    fromTimestamp = AJParkLib.AJCommon.Time.DateTime_To_MilliSec(fromTimestamp_date);
                    str_fromTimestamp = Math.Floor(fromTimestamp).ToString();
                    
                    //str_fromTimestamp = str_fromTimestamp.Replace(".", "");
                    //request.AddParameter("fromTimestamp", str_fromTimestamp);

                    double toTimestamp = AJParkLib.AJCommon.Time.DateTime_To_MilliSec(toTimestamp_date);
                    string str_toTimestamp = Math.Floor(toTimestamp).ToString();
                    //string str_toTimestamp = toTimestamp.ToString();
                    //str_toTimestamp = str_toTimestamp.Replace(".", "");
                    request.AddParameter("toTimestamp", str_toTimestamp);
                    //파라미터 추가 끝.

                    //파라미터 로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("");
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 ------ ");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("localEquipmentId : {0}", localEquipmentId));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("fromTimestamp(TimeStamp) : {0}", str_fromTimestamp));
                        if (str_fromTimestamp != "")
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("fromTimestamp(DateTime) : {0}", AJParkLib.AJCommon.Time.MilliSec_To_DateTime(Convert.ToDouble(str_fromTimestamp))));
                        }
                        else
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("fromTimestamp(DateTime) : {0}", " "));
                        }
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("toTimestamp(TimeStamp) : {0}", str_toTimestamp));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("toTimestamp(DateTime) : {0}", AJParkLib.AJCommon.Time.MilliSec_To_DateTime(Convert.ToDouble(str_toTimestamp))));
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 끝 ------ ");
                    }
                    //파라미터 로그 기록 끝.

                    string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드

                    if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                    {
                        temp.code = "181818";
                        //서버에서 응답 자체를 못받았을 경우
                        AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                    }
                    else
                    {
                        JObject obj = JObject.Parse(str_response);

                        //파싱 시작
                        temp.code = (string)obj["code"];                        //ini에 code 정보 붙임
                        temp.errmsgvariable = (string)obj["errmsgvariable"];    //ini에 errmsgvariable 정보 붙임
                        temp.errmsg = (string)obj["errmsg"];                    //ini에 errmsg 정보 붙임
                        temp.key = (string)obj["key"];                          //ini에 key 정보 붙임

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("code : {0}", temp.code));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsgvariable : {0}", temp.errmsgvariable));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsg : {0}", temp.errmsg));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("key : {0}", temp.key));
                        }
                        //로그 기록



                        //클로즈 인포(단일)
                        closeinfo_list_RESPONSE closeinfo_list_temp = new closeinfo_list_RESPONSE();
                        closeinfo_list_temp.parkingLotName = (string)obj["closeInfo"]["parkingLotName"];
                        closeinfo_list_temp.companyName = (string)obj["closeInfo"]["companyName"];
                        closeinfo_list_temp.companyNumber = (string)obj["closeInfo"]["companyNumber"];
                        closeinfo_list_temp.parkingLotAddr = (string)obj["closeInfo"]["parkingLotAddr"];
                        closeinfo_list_temp.fromTimestamp = (string)obj["closeInfo"]["fromTimestamp"];
                        closeinfo_list_temp.toTimestamp = (string)obj["closeInfo"]["toTimestamp"];
                        closeinfo_list_temp.manualOpenCount = (string)obj["closeInfo"]["manualOpenCount"];
                        closeinfo_list_temp.printDate = (string)obj["closeInfo"]["printDate"];
                        closeinfo_list_temp.equipmentNo = (string)obj["closeInfo"]["equipmentNo"];



                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포의 단일 키 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("parkingLotName : {0}", closeinfo_list_temp.parkingLotName));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("companyName : {0}", closeinfo_list_temp.companyName));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("companyNumber : {0}", closeinfo_list_temp.companyNumber));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("fromTimestamp : {0}", closeinfo_list_temp.fromTimestamp));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format(" - fromTimestamp(DateTime) : {0}", AJParkLib.AJCommon.Time.MilliSec_To_DateTime(Convert.ToDouble(closeinfo_list_temp.fromTimestamp))));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("toTimestamp : {0}", closeinfo_list_temp.toTimestamp));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format(" - toTimestamp(DateTime) : {0}", AJParkLib.AJCommon.Time.MilliSec_To_DateTime(Convert.ToDouble(closeinfo_list_temp.toTimestamp))));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("manualOpenCount : {0}", closeinfo_list_temp.manualOpenCount));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("printDate : {0}", closeinfo_list_temp.printDate));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format(" - printDate(DateTime) : {0}", AJParkLib.AJCommon.Time.MilliSec_To_DateTime(Convert.ToDouble(closeinfo_list_temp.printDate))));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("equipmentNo : {0}", closeinfo_list_temp.equipmentNo));
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포의 단일 키 끝 ------ ");
                        }
                        //로그 기록

                        //클로즈 인포 - 인컴캐쉬
                        int incomeCash_count = obj["closeInfo"]["incomeCash"].Count();
                        closeinfo_list_RESPONSE_list_incomeCash[] incomeCash_list_temp = new closeinfo_list_RESPONSE_list_incomeCash[incomeCash_count];


                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 인컴캐쉬 리스트------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("인컴캐쉬 리스트의 개수 : {0}", incomeCash_count));
                        }
                        //로그 기록


                        for (int i = 0; i < incomeCash_count; i++)
                        {
                            incomeCash_list_temp[i].releaseCount = (string)obj["closeInfo"]["incomeCash"][i]["releaseCount"];
                            incomeCash_list_temp[i].releasePrice = (string)obj["closeInfo"]["incomeCash"][i]["releasePrice"];
                            incomeCash_list_temp[i].insertCount = (string)obj["closeInfo"]["incomeCash"][i]["insertCount"];
                            incomeCash_list_temp[i].insertPrice = (string)obj["closeInfo"]["incomeCash"][i]["insertPrice"];

                            //로그 기록
                            if (use_Log_Write == true)
                            {
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 인컴캐쉬 리스트 -> releaseCount : {1}", i, incomeCash_list_temp[i].releaseCount));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 인컴캐쉬 리스트 -> releasePrice : {1}", i, incomeCash_list_temp[i].releasePrice));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 인컴캐쉬 리스트 -> insertCount : {1}", i, incomeCash_list_temp[i].insertCount));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 인컴캐쉬 리스트 -> insertPrice : {1}", i, incomeCash_list_temp[i].insertPrice));
                                AJParkLib.AJCommon.CommonClass.SendLog("");
                            }
                            //로그 기록
                        }
                        closeinfo_list_temp.incomeCash_list = incomeCash_list_temp;
                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 인컴캐쉬 리스트 끝 ------ ");
                        }
                        //로그 기록



                        //클로즈 인포 - 인컴낫캐쉬
                        int incomeNotCash_count = obj["closeInfo"]["incomeNotCash"].Count();
                        closeinfo_list_RESPONSE_list_incomeNotCash[] incomeNotCash_list_temp = new closeinfo_list_RESPONSE_list_incomeNotCash[incomeNotCash_count];


                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 인컴낫캐쉬 리스트------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("인컴낫캐쉬 리스트의 개수 : {0}", incomeNotCash_count));
                        }
                        //로그 기록

                        for (int i = 0; i < incomeNotCash_count; i++)
                        {
                            incomeNotCash_list_temp[i].price = (string)obj["closeInfo"]["incomeNotCash"][i]["price"];
                            incomeNotCash_list_temp[i].count = (string)obj["closeInfo"]["incomeNotCash"][i]["count"];
                            incomeNotCash_list_temp[i].paymentMethod = (string)obj["closeInfo"]["incomeNotCash"][i]["paymentMethod"];

                            //로그 기록
                            if (use_Log_Write == true)
                            {
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 인컴낫캐쉬 리스트 -> price : {1}", i, incomeNotCash_list_temp[i].price));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 인컴캐쉬 리스트 -> count : {1}", i, incomeNotCash_list_temp[i].count));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 인컴캐쉬 리스트 -> paymentMethod : {1}", i, incomeNotCash_list_temp[i].paymentMethod));
                                AJParkLib.AJCommon.CommonClass.SendLog("");
                            }
                            //로그 기록
                        }
                        closeinfo_list_temp.incomeNotCash_list = incomeNotCash_list_temp;
                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 인컴낫캐쉬 리스트 끝 ------ ");
                        }
                        //로그 기록


                        //클로즈 인포 - 인컴토탈
                        int incomeTotal_count = obj["closeInfo"]["incomeTotal"].Count();
                        closeinfo_list_RESPONSE_list_incomeTotal[] incomeTotal_list_temp = new closeinfo_list_RESPONSE_list_incomeTotal[incomeTotal_count];

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 인컴토탈 리스트------ ");       //로그 기록
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("인컴토탈 리스트의 개수 : {0}", incomeTotal_count));
                        }
                        //로그 기록

                        for (int i = 0; i < incomeTotal_count; i++)
                        {
                            incomeTotal_list_temp[i].total = (string)obj["closeInfo"]["incomeTotal"][i]["total"];
                            incomeTotal_list_temp[i].count = (string)obj["closeInfo"]["incomeTotal"][i]["count"];
                            incomeTotal_list_temp[i].price = (string)obj["closeInfo"]["incomeTotal"][i]["price"];

                            //로그 기록
                            if (use_Log_Write == true)
                            {
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 인컴토탈 리스트 -> total : {1}", i, incomeTotal_list_temp[i].total));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 인컴토탈 리스트 -> count : {1}", i, incomeTotal_list_temp[i].count));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 인컴토탈 리스트 -> price : {1}", i, incomeTotal_list_temp[i].price));
                                AJParkLib.AJCommon.CommonClass.SendLog("");
                            }
                            //로그 기록
                        }
                        closeinfo_list_temp.incomeTotal_list = incomeTotal_list_temp;
                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 인컴토탈 리스트 끝 ------ ");
                        }
                        //로그 기록

                        //클로즈 인포 - 디스카운트 인포 디테일
                        int discountInfoDetail_count = obj["closeInfo"]["discountInfoDetail"].Count();
                        closeinfo_list_RESPONSE_list_discountInfoDetail[] discountInfoDetail_list_temp = new closeinfo_list_RESPONSE_list_discountInfoDetail[discountInfoDetail_count];

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 디스카운트 인포 디테일 리스트------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("디스카운트 인포 디테일 리스트의 개수 : {0}", discountInfoDetail_count));
                        }
                        //로그 기록

                        for (int i = 0; i < discountInfoDetail_count; i++)
                        {
                            discountInfoDetail_list_temp[i].name = (string)obj["closeInfo"]["discountInfoDetail"][i]["name"];
                            discountInfoDetail_list_temp[i].count = (string)obj["closeInfo"]["discountInfoDetail"][i]["count"];
                            discountInfoDetail_list_temp[i].discountPrice = (string)obj["closeInfo"]["discountInfoDetail"][i]["discountPrice"];

                            //로그 기록
                            if (use_Log_Write == true)
                            {
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 디스카운트 인포 디테일 리스트 -> total : {1}", i, discountInfoDetail_list_temp[i].name));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 디스카운트 인포 디테일 리스트 -> count : {1}", i, discountInfoDetail_list_temp[i].count));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 디스카운트 인포 디테일 리스트 -> discountPrice : {1}", i, discountInfoDetail_list_temp[i].discountPrice));
                                AJParkLib.AJCommon.CommonClass.SendLog("");
                            }
                            //로그 기록
                        }
                        closeinfo_list_temp.discountInfoDetail_list = discountInfoDetail_list_temp;
                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 디스카운트 인포 디테일 리스트 끝 ------ ");
                        }
                        //로그 기록


                        //클로즈 인포 - 디스카운트 인포 토탈
                        int discountInfoTotal_count = obj["closeInfo"]["discountInfoTotal"].Count();
                        closeinfo_list_RESPONSE_list_discountInfoTotal[] discountInfoTotal_list_temp = new closeinfo_list_RESPONSE_list_discountInfoTotal[discountInfoTotal_count];

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 디스카운트 인포 토탈 리스트------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("디스카운트 인포 토탈 리스트의 개수 : {0}", discountInfoTotal_count));
                        }
                        //로그 기록


                        for (int i = 0; i < discountInfoTotal_count; i++)
                        {
                            discountInfoTotal_list_temp[i].total = (string)obj["closeInfo"]["discountInfoTotal"][i]["total"];
                            discountInfoTotal_list_temp[i].count = (string)obj["closeInfo"]["discountInfoTotal"][i]["count"];
                            discountInfoTotal_list_temp[i].discountPrice = (string)obj["closeInfo"]["discountInfoTotal"][i]["discountPrice"];


                            //로그 기록
                            if (use_Log_Write == true)
                            {
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 디스카운트 인포 토탈 리스트 -> total : {1}", i, discountInfoTotal_list_temp[i].total));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 디스카운트 인포 토탈 리스트 -> count : {1}", i, discountInfoTotal_list_temp[i].count));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 디스카운트 인포 토탈 리스트 -> discountPrice : {1}", i, discountInfoTotal_list_temp[i].discountPrice));
                                AJParkLib.AJCommon.CommonClass.SendLog("");
                            }
                            //로그 기록
                        }
                        closeinfo_list_temp.discountInfoTotal_list = discountInfoTotal_list_temp;
                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 디스카운트 인포 토탈 리스트 끝 ------ ");
                        }
                        //로그 기록


                        //클로즈 인포 - 리텐션 캐쉬 인포
                        int retentionCashInfo_count = obj["closeInfo"]["retentionCashInfo"].Count();
                        closeinfo_list_RESPONSE_list_retentionCashInfo[] retentionCashInfo_list_temp = new closeinfo_list_RESPONSE_list_retentionCashInfo[retentionCashInfo_count];

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");       //로그 기록
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 리텐션 캐쉬 인포 리스트------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("리텐션 캐쉬 인포 리스트의 개수 : {0}", retentionCashInfo_count));
                        }
                        //로그 기록

                        for (int i = 0; i < retentionCashInfo_count; i++)
                        {
                            retentionCashInfo_list_temp[i].localEquipmentId = (string)obj["closeInfo"]["retentionCashInfo"][i]["localEquipmentId"];
                            retentionCashInfo_list_temp[i].init5000 = (string)obj["closeInfo"]["retentionCashInfo"][i]["init5000"];
                            retentionCashInfo_list_temp[i].init1000 = (string)obj["closeInfo"]["retentionCashInfo"][i]["init1000"];
                            retentionCashInfo_list_temp[i].init500 = (string)obj["closeInfo"]["retentionCashInfo"][i]["init500"];
                            retentionCashInfo_list_temp[i].init100 = (string)obj["closeInfo"]["retentionCashInfo"][i]["init100"];
                            retentionCashInfo_list_temp[i].totalInit = (string)obj["closeInfo"]["retentionCashInfo"][i]["totalInit"];
                            retentionCashInfo_list_temp[i].current5000 = (string)obj["closeInfo"]["retentionCashInfo"][i]["current5000"];
                            retentionCashInfo_list_temp[i].current1000 = (string)obj["closeInfo"]["retentionCashInfo"][i]["current1000"];
                            retentionCashInfo_list_temp[i].current500 = (string)obj["closeInfo"]["retentionCashInfo"][i]["current500"];
                            retentionCashInfo_list_temp[i].current100 = (string)obj["closeInfo"]["retentionCashInfo"][i]["current100"];
                            retentionCashInfo_list_temp[i].totalCurrent = (string)obj["closeInfo"]["retentionCashInfo"][i]["totalCurrent"];


                            //로그 기록
                            if (use_Log_Write == true)
                            {
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리텐션 캐쉬 인포 리스트 -> localEquipmentId : {1}", i, retentionCashInfo_list_temp[i].localEquipmentId));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리텐션 캐쉬 인포 리스트 -> init5000 : {1}", i, retentionCashInfo_list_temp[i].init5000));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리텐션 캐쉬 인포 리스트 -> init1000 : {1}", i, retentionCashInfo_list_temp[i].init1000));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리텐션 캐쉬 인포 리스트 -> init500 : {1}", i, retentionCashInfo_list_temp[i].init500));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리텐션 캐쉬 인포 리스트 -> init100 : {1}", i, retentionCashInfo_list_temp[i].init100));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리텐션 캐쉬 인포 리스트 -> totalInit : {1}", i, retentionCashInfo_list_temp[i].totalInit));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리텐션 캐쉬 인포 리스트 -> current5000 : {1}", i, retentionCashInfo_list_temp[i].current5000));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리텐션 캐쉬 인포 리스트 -> current1000 : {1}", i, retentionCashInfo_list_temp[i].current1000));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리텐션 캐쉬 인포 리스트 -> current500 : {1}", i, retentionCashInfo_list_temp[i].current500));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리텐션 캐쉬 인포 리스트 -> current100 : {1}", i, retentionCashInfo_list_temp[i].current100));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리텐션 캐쉬 인포 리스트 -> totalCurrent : {1}", i, retentionCashInfo_list_temp[i].totalCurrent));
                                AJParkLib.AJCommon.CommonClass.SendLog("");
                            }
                            //로그 기록

                        }
                        closeinfo_list_temp.retentionCashInfo_list = retentionCashInfo_list_temp;
                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 리텐션 캐쉬 인포 리스트 끝------ ");
                        }
                        //로그 기록



                        //클로즈 인포 - 인풋 캐쉬 인포(단일)
                        closeinfo_list_RESPONSE_list_inputCashInfo inputCashInfo_list_temp = new closeinfo_list_RESPONSE_list_inputCashInfo();
                        inputCashInfo_list_temp.count50000 = (string)obj["closeInfo"]["inputCashInfo"]["count50000"];
                        inputCashInfo_list_temp.count10000 = (string)obj["closeInfo"]["inputCashInfo"]["count10000"];
                        inputCashInfo_list_temp.count5000 = (string)obj["closeInfo"]["inputCashInfo"]["count5000"];
                        inputCashInfo_list_temp.count1000 = (string)obj["closeInfo"]["inputCashInfo"]["count1000"];
                        inputCashInfo_list_temp.count500 = (string)obj["closeInfo"]["inputCashInfo"]["count500"];
                        inputCashInfo_list_temp.count100 = (string)obj["closeInfo"]["inputCashInfo"]["count100"];
                        inputCashInfo_list_temp.count50 = (string)obj["closeInfo"]["inputCashInfo"]["count50"];
                        inputCashInfo_list_temp.count10 = (string)obj["closeInfo"]["inputCashInfo"]["count10"];
                        inputCashInfo_list_temp.totalCount = (string)obj["closeInfo"]["inputCashInfo"]["totalCount"];
                        inputCashInfo_list_temp.sum50000 = (string)obj["closeInfo"]["inputCashInfo"]["sum50000"];
                        inputCashInfo_list_temp.sum10000 = (string)obj["closeInfo"]["inputCashInfo"]["sum10000"];
                        inputCashInfo_list_temp.sum5000 = (string)obj["closeInfo"]["inputCashInfo"]["sum5000"];
                        inputCashInfo_list_temp.sum1000 = (string)obj["closeInfo"]["inputCashInfo"]["sum1000"];
                        inputCashInfo_list_temp.sum500 = (string)obj["closeInfo"]["inputCashInfo"]["sum500"];
                        inputCashInfo_list_temp.sum100 = (string)obj["closeInfo"]["inputCashInfo"]["sum100"];
                        inputCashInfo_list_temp.sum50 = (string)obj["closeInfo"]["inputCashInfo"]["sum50"];
                        inputCashInfo_list_temp.sum10 = (string)obj["closeInfo"]["inputCashInfo"]["sum10"];
                        inputCashInfo_list_temp.totalSum = (string)obj["closeInfo"]["inputCashInfo"]["totalSum"];

                        closeinfo_list_temp.inputCashInfo_list = inputCashInfo_list_temp;

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 인풋 캐쉬 인포(단일) ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("count50000 : {0}", inputCashInfo_list_temp.count50000));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("count10000 : {0}", inputCashInfo_list_temp.count10000));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("count5000 : {0}", inputCashInfo_list_temp.count5000));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("count1000 : {0}", inputCashInfo_list_temp.count1000));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("count500 : {0}", inputCashInfo_list_temp.count500));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("count100 : {0}", inputCashInfo_list_temp.count100));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("count50 : {0}", inputCashInfo_list_temp.count50));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("count10 : {0}", inputCashInfo_list_temp.count10));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("totalCount : {0}", inputCashInfo_list_temp.totalCount));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("sum50000 : {0}", inputCashInfo_list_temp.sum50000));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("sum10000 : {0}", inputCashInfo_list_temp.sum10000));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("sum5000 : {0}", inputCashInfo_list_temp.sum5000));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("sum1000 : {0}", inputCashInfo_list_temp.sum1000));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("sum500 : {0}", inputCashInfo_list_temp.sum500));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("sum100 : {0}", inputCashInfo_list_temp.sum100));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("sum50 : {0}", inputCashInfo_list_temp.sum50));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("sum10 : {0}", inputCashInfo_list_temp.sum10));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("totalSum : {0}", inputCashInfo_list_temp.totalSum));
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 인풋 캐쉬 인포(단일) 끝 ------ ");
                        }
                        //로그 기록

                        //클로즈 인포 - 아웃풋 캐쉬 인포(단일)
                        closeinfo_list_RESPONSE_list_outputCashInfo outputCashInfo_list_temp = new closeinfo_list_RESPONSE_list_outputCashInfo();
                        outputCashInfo_list_temp.count5000 = (string)obj["closeInfo"]["outputCashInfo"]["count5000"];
                        outputCashInfo_list_temp.count1000 = (string)obj["closeInfo"]["outputCashInfo"]["count1000"];
                        outputCashInfo_list_temp.count500 = (string)obj["closeInfo"]["outputCashInfo"]["count500"];
                        outputCashInfo_list_temp.count100 = (string)obj["closeInfo"]["outputCashInfo"]["count100"];
                        outputCashInfo_list_temp.totalCount = (string)obj["closeInfo"]["outputCashInfo"]["totalCount"];
                        outputCashInfo_list_temp.sum5000 = (string)obj["closeInfo"]["outputCashInfo"]["sum5000"];
                        outputCashInfo_list_temp.sum1000 = (string)obj["closeInfo"]["outputCashInfo"]["sum1000"];
                        outputCashInfo_list_temp.sum500 = (string)obj["closeInfo"]["outputCashInfo"]["sum500"];
                        outputCashInfo_list_temp.sum100 = (string)obj["closeInfo"]["outputCashInfo"]["sum100"];
                        outputCashInfo_list_temp.totalSum = (string)obj["closeInfo"]["outputCashInfo"]["totalSum"];

                        closeinfo_list_temp.outputCashInfo_list = outputCashInfo_list_temp;

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 아웃풋 캐쉬 인포(단일) ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("count5000 : {0}", outputCashInfo_list_temp.count5000));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("count1000 : {0}", outputCashInfo_list_temp.count1000));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("count500 : {0}", outputCashInfo_list_temp.count500));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("count100 : {0}", outputCashInfo_list_temp.count100));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("totalCount : {0}", outputCashInfo_list_temp.totalCount));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("sum5000 : {0}", outputCashInfo_list_temp.sum5000));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("sum1000 : {0}", outputCashInfo_list_temp.sum1000));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("sum500 : {0}", outputCashInfo_list_temp.sum500));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("sum100 : {0}", outputCashInfo_list_temp.sum100));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("totalSum : {0}", outputCashInfo_list_temp.totalSum));
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 아웃풋 캐쉬 인포(단일) 끝 ------ ");
                        }
                        //로그 기록

                        //클로즈 인포 - 파킹 카 인포(단일)
                        closeinfo_list_RESPONSE_list_parkingCarInfo parkingCarInfo_list_temp = new closeinfo_list_RESPONSE_list_parkingCarInfo();
                        parkingCarInfo_list_temp.monthlyCarCount = (string)obj["closeInfo"]["parkingCarInfo"]["monthlyCarCount"];
                        parkingCarInfo_list_temp.priceCarCount = (string)obj["closeInfo"]["parkingCarInfo"]["priceCarCount"];
                        parkingCarInfo_list_temp.recurrenceCarCount = (string)obj["closeInfo"]["parkingCarInfo"]["recurrenceCarCount"];

                        closeinfo_list_temp.parkingCarInfo_list = parkingCarInfo_list_temp;

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 파킹 카 인포(단일) ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("monthlyCarCount : {0}", parkingCarInfo_list_temp.monthlyCarCount));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("priceCarCount : {0}", parkingCarInfo_list_temp.priceCarCount));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("recurrenceCarCount : {0}", parkingCarInfo_list_temp.recurrenceCarCount));
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 파킹 카 인포(단일) 끝 ------ ");
                        }
                        //로그 기록

                        //클로즈 인포 - 노 페이 파킹(단일)
                        closeinfo_list_RESPONSE_list_noPayParking noPayParking_list_temp = new closeinfo_list_RESPONSE_list_noPayParking();
                        noPayParking_list_temp.name = (string)obj["closeInfo"]["noPayParking"]["name"];
                        noPayParking_list_temp.sumCount = (string)obj["closeInfo"]["noPayParking"]["sumCount"];
                        noPayParking_list_temp.sumPrice = (string)obj["closeInfo"]["noPayParking"]["sumPrice"];

                        closeinfo_list_temp.noPayParking_list = noPayParking_list_temp;

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");       //로그 기록
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 노 페이 파킹(단일) ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("name : {0}", noPayParking_list_temp.name));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("sumCount : {0}", noPayParking_list_temp.sumCount));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("sumPrice : {0}", noPayParking_list_temp.sumPrice));
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트의 노 페이 파킹(단일) 끝 ------ ");
                        }
                        //로그 기록

                        //리스트 붙이기 끝.


                        //전부 다 붙였으니까 이제 응답 클래스에 넣기
                        temp.CloseInfo_list = closeinfo_list_temp;

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 클로즈 인포 리스트  끝. ------ ");       //로그 기록
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 끝 ------ ");       //로그 기록
                            AJParkLib.AJCommon.CommonClass.SendLog("전체 동작 성공");       //로그 기록
                        }

                    }
                    //return temp;


                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(e.StackTrace.ToString());

                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("동작 실패");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format(" - e : {0}", e));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format(" - e.StackTrace. : {0}", e.StackTrace));
                    }


                    temp.code = "1818";
                    //return temp;
                }
                finally
                {
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("모든 파싱 종료");       //로그 기록
                    }

                }
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 02. 마감 정보 끝 === ");       //로그 기록
                }
                return temp;

            }
            #endregion

            #region 3번
            public static AJ_RESPONSE_query temp_name_3(string query)
            {
                //var client = new RestClient("http://112.216.153.186:2080//api/admin/dynamicQuery/select");
                string Server_path = "http://" + AJ_IP + ":" + AJ_PORT;
                Server_path += "//api/admin/dynamicQuery/select";

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 03. 쿼리 전송 결과 시작 === ");
                    AJParkLib.AJCommon.CommonClass.SendLog("접속 주소 : " + Server_path);
                }
                //로그 기록

                AJ_RESPONSE_query temp = new AJ_RESPONSE_query();

                try
                {
                    var client = new RestClient(Server_path);
                    var request = new RestRequest(Method.POST);

                    //파라미터 추가
                    request.AddParameter("query", query);
                    //여기까지

                    //파라미터 로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("");       //로그 기록
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 ------ ");       //로그 기록
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("query : {0}", query));       //로그 기록
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 끝 ------ ");       //로그 기록
                    }
                    //파라미터 로그 기록 끝.

                    string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드

                    if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                    {
                        temp.code = "181818";
                        //서버에서 응답 자체를 못받았을 경우
                        AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                    }
                    else
                    {
                        try
                        {
                            JObject obj = JObject.Parse(str_response);
                            temp.code = (string)obj["code"];                        //ini에 code 정보 붙임
                            temp.errmsgvariable = (string)obj["errmsgvariable"];    //ini에 errmsgvariable 정보 붙임
                            temp.errmsg = (string)obj["errmsg"];                    //ini에 errmsg 정보 붙임
                            temp.key = (string)obj["key"];                          //ini에 key 정보 붙임

                            //로그 기록
                            if (use_Log_Write == true)
                            {
                                AJParkLib.AJCommon.CommonClass.SendLog("");
                                AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 ------ ");
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("code : {0}", temp.code));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsgvariable : {0}", temp.errmsgvariable));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsg : {0}", temp.errmsg));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("key : {0}", temp.key));
                            }
                            //로그 기록


                            //리절트 - 리스트
                            int result_list_count = obj["result"].Count();
                            result_list_RESPONSE[] result_list_temp = new result_list_RESPONSE[result_list_count];

                            //로그 기록
                            if (use_Log_Write == true)
                            {
                                AJParkLib.AJCommon.CommonClass.SendLog("");       //로그 기록
                                AJParkLib.AJCommon.CommonClass.SendLog(" ------ 리절트 리스트 ------ ");       //로그 기록
                            }
                            //로그 기록
                            
                            for (int i = 0; i < result_list_count; i++)
                            {
                                result_list_temp[i].isUse = (string)obj["result"][i]["isUse"];
                                result_list_temp[i].password = (string)obj["result"][i]["password"];
                                result_list_temp[i].loginId = (string)obj["result"][i]["loginId"];
                                result_list_temp[i].phone = (string)obj["result"][i]["phone"];
                                result_list_temp[i].adminId = (string)obj["result"][i]["adminId"];
                                result_list_temp[i].name = (string)obj["result"][i]["name"];
                                result_list_temp[i].regDate = (string)obj["result"][i]["regDate"];
                                result_list_temp[i].isSuperAdmin = (string)obj["result"][i]["isSuperAdmin"];
                                result_list_temp[i].email = (string)obj["result"][i]["email"];
                                result_list_temp[i].status = (string)obj["result"][i]["status"];

                                //로그 기록
                                if (use_Log_Write == true)
                                {
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리절트 리스트 -> isUse : {1}", i, result_list_temp[i].isUse));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리절트 리스트 -> password : {1}", i, result_list_temp[i].password));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리절트 리스트 -> loginId : {1}", i, result_list_temp[i].loginId));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리절트 리스트 -> phone : {1}", i, result_list_temp[i].phone));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리절트 리스트 -> adminId : {1}", i, result_list_temp[i].adminId));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리절트 리스트 -> name : {1}", i, result_list_temp[i].name));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리절트 리스트 -> regDate : {1}", i, result_list_temp[i].regDate));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리절트 리스트 -> isSuperAdmin : {1}", i, result_list_temp[i].isSuperAdmin));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리절트 리스트 -> email : {1}", i, result_list_temp[i].email));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리절트 리스트 -> status : {1}", i, result_list_temp[i].status));
                                }
                                //로그 기록

                                //로그 기록
                            }
                            temp.result_list = result_list_temp;
                            //return temp;
                            //로그 기록
                            if (use_Log_Write == true)
                            {
                                AJParkLib.AJCommon.CommonClass.SendLog(" ------ 리절트 리스트 끝 ------ ");
                                AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 끝 ------ ");
                                AJParkLib.AJCommon.CommonClass.SendLog("전체 동작 성공");
                            }
                            //로그 기록

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                            Console.WriteLine(e.StackTrace.ToString());
                        }

                    }

                }

                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(e.StackTrace.ToString());

                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("동작 실패");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e : {0}", e));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e.StackTrace : {0}", e.StackTrace));
                    }
                    //로그 기록
                    //return temp;
                    temp.code = "1818";
                }

                finally
                {
                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("모든 파싱 종료");
                    }
                    //로그 기록
                }
                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 03. 쿼리 전송 결과 끝 === ");
                }
                //로그 기록
                return temp;
                
            }
            #endregion


            #region 4-1번
            public static AJ_RESPONSE_vaultCashInfo_POST temp_name_4_1(int localEquipmentId, int won5000, int won1000, int won500, int won100, string isForceChange)
            {
                
                //var client = new RestClient("http://112.216.153.186:2080//api/equipment/post/vaultCashInfo");
                string Server_path = "http://" + AJ_IP + ":" + AJ_PORT;
                Server_path += "//api/equipment/post/vaultCashInfo";

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 04-1. 시재금 DB 정보를 전달(장비 -> LMS) 시작 === ");
                    AJParkLib.AJCommon.CommonClass.SendLog("접속 주소 : " + Server_path);
                }
                //로그 기록
                
                AJ_RESPONSE_vaultCashInfo_POST temp = new AJ_RESPONSE_vaultCashInfo_POST();             //리스폰스 변수 생성

                try
                {
                    var client = new RestClient(Server_path);
                    var request = new RestRequest(Method.POST);

                    //파라미터 추가
                    request.AddParameter("localEquipmentId", localEquipmentId);
                    request.AddParameter("won5000", won5000);
                    request.AddParameter("won1000", won1000);
                    request.AddParameter("won500", won500);
                    request.AddParameter("won100", won100);
                    request.AddParameter("isForceChange", isForceChange);           //0.7버전 추가
                    //여기까지

                    //파라미터 로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("");
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 ------ ");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("localEquipmentId : {0}", localEquipmentId));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("won5000 : {0}", won5000));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("won1000 : {0}", won1000));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("won500 : {0}", won500));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("won100 : {0}", won100));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("isForceChange : {0}", isForceChange));
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 끝 ------ ");
                    }
                    //파라미터 로그 기록 끝.

                    string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드

                    if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                    {
                        temp.code = "181818";
                        //서버에서 응답 자체를 못받았을 경우
                        AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                    }
                    else
                    {
                        try
                        {
                            JObject obj = JObject.Parse(str_response);
                            temp.code = (string)obj["code"];                        //ini에 code 정보 붙임
                            temp.errmsgvariable = (string)obj["errmsgvariable"];    //ini에 errmsgvariable 정보 붙임
                            temp.errmsg = (string)obj["errmsg"];                    //ini에 errmsg 정보 붙임
                            temp.key = (string)obj["key"];                          //ini에 key 정보 붙임

                            //로그 기록
                            if (use_Log_Write == true)
                            {
                                AJParkLib.AJCommon.CommonClass.SendLog("");
                                AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 ------ ");
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("code : {0}", temp.code));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsgvariable : {0}", temp.errmsgvariable));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsg : {0}", temp.errmsg));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("key : {0}", temp.key));
                                AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 끝 ------ ");
                                AJParkLib.AJCommon.CommonClass.SendLog("전체 동작 성공");
                            }
                            //로그 기록
                            
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                            Console.WriteLine(e.StackTrace.ToString());
                        }

                    }
                    //return temp;
                }

                catch (Exception e)
                {
                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("동작 실패");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e : {0}", e));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e.StackTrace : {0}", e.StackTrace));
                    }
                    //로그 기록
                    temp.code = "1818";
                    //return temp;
                }

                finally
                {
                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("모든 파싱 종료");       //로그 기록
                    }
                    //로그 기록
                }

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 04-1. 시재금 DB 정보를 전달(장비 -> LMS) 끝 === ");
                }
                //로그 기록
                
                return temp;
            }
            #endregion

            #region 4-2번
            public static AJ_RESPONSE_vaultCashInfo_GET temp_name_4_2(int localEquipmentId)
            {
                //var client = new RestClient("http://112.216.153.186:2080//api/equipment/get/vaultCashInfo");
                string Server_path = "http://" + AJ_IP + ":" + AJ_PORT;
                Server_path += "//api/equipment/get/vaultCashInfo";

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 04-2. 시재금 DB 정보를 전달(LMS -> 장비) 시작 === ");       //로그 기록
                    AJParkLib.AJCommon.CommonClass.SendLog("접속 주소 : " + Server_path);
                }
                //로그 기록

                AJ_RESPONSE_vaultCashInfo_GET temp = new AJ_RESPONSE_vaultCashInfo_GET();
                try
                {
                    var client = new RestClient(Server_path);
                    var request = new RestRequest(Method.POST);

                    //파라미터 추가
                    request.AddParameter("localEquipmentId", localEquipmentId);
                    //여기까지

                    //파라미터 로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("");
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 ------ ");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("localEquipmentId(장비 UID) : {0}", localEquipmentId));
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 끝 ------ ");
                    }
                    //파라미터 로그 기록 끝.

                    string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드

                    if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                    {
                        temp.code = "181818";
                        //서버에서 응답 자체를 못받았을 경우
                        AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                    }
                    else
                    {
                        JObject obj = JObject.Parse(str_response);

                        //파싱 시작
                        temp.code = (string)obj["code"];                        //ini에 code 정보 붙임
                        temp.errmsgvariable = (string)obj["errmsgvariable"];    //ini에 errmsgvariable 정보 붙임
                        temp.errmsg = (string)obj["errmsg"];                    //ini에 errmsg 정보 붙임
                        temp.key = (string)obj["key"];                          //ini에 key 정보 붙임

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("code : {0}", temp.code));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsgvariable : {0}", temp.errmsgvariable));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsg : {0}", temp.errmsg));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("key : {0}", temp.key));
                        }
                        //로그 기록


                        vaultCashInfo_list_RESPONSE temp_cash_list = new vaultCashInfo_list_RESPONSE();
                        temp_cash_list.won5000 = (string)obj["vaultCashInfo"]["won5000"];
                        temp_cash_list.won1000 = (string)obj["vaultCashInfo"]["won1000"];
                        temp_cash_list.won500 = (string)obj["vaultCashInfo"]["won500"];
                        temp_cash_list.won100 = (string)obj["vaultCashInfo"]["won100"];
                        temp_cash_list.set5000 = (string)obj["vaultCashInfo"]["set5000"];           //0.7버전에 추가
                        temp_cash_list.set1000 = (string)obj["vaultCashInfo"]["set1000"];           //0.7버전에 추가
                        temp_cash_list.set500 = (string)obj["vaultCashInfo"]["set500"];           //0.7버전에 추가
                        temp_cash_list.set100 = (string)obj["vaultCashInfo"]["set100"];           //0.7버전에 추가

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 템프 캐쉬 리스트(단일) ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("won5000 : {0}", temp_cash_list.won5000));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("won1000 : {0}", temp_cash_list.won1000));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("won500 : {0}", temp_cash_list.won500));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("won100 : {0}", temp_cash_list.won100));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("set5000 : {0}", temp_cash_list.set5000));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("set1000 : {0}", temp_cash_list.set1000));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("set500 : {0}", temp_cash_list.set500));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("set100 : {0}", temp_cash_list.set100));
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 템프 캐쉬 리스트(단일) 끝 ------ ");
                        }
                        //로그 기록

                        temp.vaultCashInfo_list = temp_cash_list;
                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 끝 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog("전체 동작 성공");
                        }
                        
                    }

                    //return temp;
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(e.StackTrace.ToString());

                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("동작 실패");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e : {0}", e));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e.StackTrace : {0}", e.StackTrace));
                    }
                    //로그 기록
                    //return temp;
                    temp.code = "1818";
                }
                finally
                {
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("모든 파싱 종료");
                    }
                }
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 04-2. 시재금 DB 정보를 전달(LMS -> 장비) 끝 === ");
                }
                
                return temp;
            }
            #endregion

            #region 5번
            public static AJ_RESPONSE_monthlyTicketInfo temp_name_5()
            {
                //var client = new RestClient("http://112.216.153.186:2080//api/equipment/get/monthlyTicketInfo");
                string Server_path = "http://" + AJ_IP + ":" + AJ_PORT;
                Server_path += "//api/equipment/get/monthlyTicketInfo";

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 05. 정기권 정보 전달 시작 === ");
                    AJParkLib.AJCommon.CommonClass.SendLog("접속 주소 : " + Server_path);
                }
                //로그 기록

                AJ_RESPONSE_monthlyTicketInfo temp = new AJ_RESPONSE_monthlyTicketInfo();
                try
                {
                    var client = new RestClient(Server_path);
                    var request = new RestRequest(Method.POST);

                    string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드

                    if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                    {
                        temp.code = "181818";
                        //서버에서 응답 자체를 못받았을 경우
                        AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                    }

                    else                                //정보를 받아왔을 때
                    {
                        JObject obj = JObject.Parse(str_response);

                        //파싱 시작

                        temp.code = (string)obj["code"];                        //ini에 code 정보 붙임
                        temp.errmsgvariable = (string)obj["errmsgvariable"];    //ini에 errmsgvariable 정보 붙임
                        temp.errmsg = (string)obj["errmsg"];                    //ini에 errmsg 정보 붙임
                        temp.key = (string)obj["key"];                          //ini에 key 정보 붙임

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("code : {0}", temp.code));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsgvariable : {0}", temp.errmsgvariable));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsg : {0}", temp.errmsg));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("key : {0}", temp.key));
                        }
                        //로그 기록


                        int temp_list_count = obj["monthlyTicketInfo"].Count();
                        monthlyTicketInfo_list_RESPONSE[] temp_list = new monthlyTicketInfo_list_RESPONSE[temp_list_count];
                        
                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");       //로그 기록
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 먼쓸리 티켓 인포 리스트 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("먼쓸리 티켓 인포 리스트 개수 : {0}", obj["monthlyTicketInfo"].Count() ));
                        }
                        //로그 기록
                        for (int i = 0; i < temp_list_count; i++)
                        {
                            temp_list[i].localMonthlyTicketId = (string)obj["monthlyTicketInfo"][i]["localMonthlyTicketId"];
                            temp_list[i].fromDate = (string)obj["monthlyTicketInfo"][i]["fromDate"];
                            temp_list[i].toDate = (string)obj["monthlyTicketInfo"][i]["toDate"];
                            temp_list[i].carNo = (string)obj["monthlyTicketInfo"][i]["carNo"];
                            temp_list[i].price = (string)obj["monthlyTicketInfo"][i]["price"];
                            temp_list[i].discountPrice = (string)obj["monthlyTicketInfo"][i]["discountPrice"];
                            temp_list[i].point = (string)obj["monthlyTicketInfo"][i]["point"];
                            temp_list[i].paymentMethod = (string)obj["monthlyTicketInfo"][i]["paymentMethod"];
                            temp_list[i].status = (string)obj["monthlyTicketInfo"][i]["status"];
                            temp_list[i].isUse = (string)obj["monthlyTicketInfo"][i]["isUse"];

                            /*
                            //로그 기록
                            //디버그 모드에서 일단 막음
                            AJParkLib.AJCommon.CommonClass.SendLog(i + "번 리스트의 " + "localMonthlyTicketId : " + temp_list[i].localMonthlyTicketId);       //로그 기록
                            AJParkLib.AJCommon.CommonClass.SendLog(i + "번 리스트의 " + "fromDate : " + temp_list[i].fromDate);       //로그 기록
                            AJParkLib.AJCommon.CommonClass.SendLog(i + "번 리스트의 " + "toDate : " + temp_list[i].toDate);       //로그 기록
                            AJParkLib.AJCommon.CommonClass.SendLog(i + "번 리스트의 " + "carNo : " + temp_list[i].carNo);       //로그 기록
                            AJParkLib.AJCommon.CommonClass.SendLog(i + "번 리스트의 " + "price : " + temp_list[i].price);       //로그 기록
                            AJParkLib.AJCommon.CommonClass.SendLog(i + "번 리스트의 " + "discountPrice : " + temp_list[i].discountPrice);       //로그 기록
                            AJParkLib.AJCommon.CommonClass.SendLog(i + "번 리스트의 " + "point : " + temp_list[i].point);       //로그 기록
                            AJParkLib.AJCommon.CommonClass.SendLog(i + "번 리스트의 " + "paymentMethod : " + temp_list[i].paymentMethod);       //로그 기록
                            AJParkLib.AJCommon.CommonClass.SendLog(i + "번 리스트의 " + "status : " + temp_list[i].status);       //로그 기록
                            AJParkLib.AJCommon.CommonClass.SendLog(i + "번 리스트의 " + "isUse : " + temp_list[i].isUse);       //로그 기록
                            AJParkLib.AJCommon.CommonClass.SendLog("");       //로그 기록
                            //로그 기록
                            */

                            //로그 기록
                            //if (use_Log_Write == true)
                            if (false)
                            {
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리스트 -> localMonthlyTicketId : {1}", i, temp_list[i].localMonthlyTicketId));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리스트 -> fromDate : {1}", i, temp_list[i].fromDate));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리스트 -> toDate : {1}", i, temp_list[i].toDate));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리스트 -> carNo : {1}", i, temp_list[i].carNo));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리스트 -> price : {1}", i, temp_list[i].price));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리스트 -> discountPrice : {1}", i, temp_list[i].discountPrice));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리스트 -> point : {1}", i, temp_list[i].point));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리스트 -> paymentMethod : {1}", i, temp_list[i].paymentMethod));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리스트 -> status : {1}", i, temp_list[i].status));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 리스트 -> isUse : {1}", i, temp_list[i].isUse));
                            }


                        }
                        temp.monthlyTicketInfo_list = temp_list;

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 먼쓸리 티켓 인포 리스트 끝 ------ ");       //로그 기록
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 끝 ------ ");       //로그 기록
                            AJParkLib.AJCommon.CommonClass.SendLog("전체 동작 성공");       //로그 기록
                        }
                        //로그 기록


                        //lprInfo_RESPONSE[] lprInfo_list = new lprInfo_RESPONSE[lpr_count];    //lprinfo 구조체 선언
                    }
                    //return temp;
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(e.StackTrace.ToString());
                    
                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("동작 실패");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e : {0}", e));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e.StackTrace : {0}", e.StackTrace));
                    }
                    //로그 기록
                    //return temp;
                    temp.code = "1818";
                }

                finally
                {
                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("모든 파싱 종료");
                    }
                    //로그 기록
                }
                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 05. 정기권 정보 전달 끝 === ");       //로그 기록
                }
                //로그 기록
                return temp;
                
            }
            #endregion

            #region 6번
            public static AJ_RESPONSE_error temp_name_6(int localEquipmentId, string equipmentCode, string errorCode)
            {
                
                //var client = new RestClient("http://112.216.153.186:2080//api/equipment/post/error");
                string Server_path = "http://" + AJ_IP + ":" + AJ_PORT;
                Server_path += "//api/equipment/post/error";

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 06. 정산기 오류 내역을 받음 시작 === ");
                    AJParkLib.AJCommon.CommonClass.SendLog("접속 주소 : " + Server_path);
                }
                //로그 기록


                AJ_RESPONSE_error temp = new AJ_RESPONSE_error();
                try
                {
                    var client = new RestClient(Server_path);
                    var request = new RestRequest(Method.POST);

                    //파라미터 추가
                    request.AddParameter("localEquipmentId", localEquipmentId);
                    request.AddParameter("equipmentCode", equipmentCode);
                    request.AddParameter("errorCode", errorCode);
                    //파라미터 추가 끝.

                    //파라미터 로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("");
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 ------ ");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("localEquipmentId(장비 UID) : {0}", localEquipmentId));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("equipmentCode : {0}", equipmentCode));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errorCode : {0}", errorCode));
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 끝 ------ ");
                    }
                    //파라미터 로그 기록 끝.

                    string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드

                    if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                    {
                        temp.code = "181818";
                        //서버에서 응답 자체를 못받았을 경우
                        AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                    }

                    else                                //정보를 받아왔을 때
                    {
                        JObject obj = JObject.Parse(str_response);
                        temp.code = (string)obj["code"];                        //ini에 code 정보 붙임
                        temp.errmsgvariable = (string)obj["errmsgvariable"];    //ini에 errmsgvariable 정보 붙임
                        temp.errmsg = (string)obj["errmsg"];                    //ini에 errmsg 정보 붙임
                        temp.key = (string)obj["key"];                          //ini에 key 정보 붙임

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("code : {0}", temp.code));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsgvariable : {0}", temp.errmsgvariable));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsg : {0}", temp.errmsg));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("key : {0}", temp.key));
                        }
                        //로그 기록

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 끝 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog("전체 동작 성공");
                        }
                        //로그 기록

                    }
                    //return temp;
                    
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(e.StackTrace.ToString());

                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("동작 실패");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e : {0}", e));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e.StackTrace : {0}", e.StackTrace));
                    }
                    //로그 기록
                    //return temp;
                    temp.code = "1818";
                }

                finally
                {
                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("모든 파싱 종료");
                    }
                    //로그 기록
                }
                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 06. 정산기 오류 내역을 받음 끝 === ");
                }
                //로그 기록
                return temp;
                
            }
            #endregion

            #region 7번
            public static AJ_RESPONSE_nopay temp_name_7(string carNo, int price, int won50000, int won10000, int won5000, int won1000, int won500, int won100, int won50, int won10)
            {

                //var client = new RestClient("http://112.216.153.186:2080//api/equipment/post/nopay");
                string Server_path = "http://" + AJ_IP + ":" + AJ_PORT;
                Server_path += "//api/equipment/post/nopay";

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 07. 미방출 금액 전달 시작 === ");
                    AJParkLib.AJCommon.CommonClass.SendLog("접속 주소 : " + Server_path);
                }
                //로그 기록
                

                AJ_RESPONSE_nopay temp = new AJ_RESPONSE_nopay();
                try
                {
                    var client = new RestClient(Server_path);
                    var request = new RestRequest(Method.POST);

                    //파라미터 추가
                    request.AddParameter("carNo", carNo);
                    request.AddParameter("price", price);
                    request.AddParameter("won50000", won50000);
                    request.AddParameter("won10000", won10000);
                    request.AddParameter("won5000", won5000);
                    request.AddParameter("won1000", won1000);
                    request.AddParameter("won500", won500);
                    request.AddParameter("won100", won100);
                    request.AddParameter("won50", won50);
                    request.AddParameter("won10", won10);
                    //파라미터 추가 끝.

                    //파라미터 로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("");
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 ------ ");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("carNo : {0}", carNo));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("price : {0}", price));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("won50000 : {0}", won50000));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("won10000 : {0}", won10000));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("won5000 : {0}", won5000));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("won1000 : {0}", won1000));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("won500 : {0}", won500));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("won100 : {0}", won100));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("won50 : {0}", won50));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("won10 : {0}", won10));
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 끝 ------ ");
                    }
                    //파라미터 로그 기록 끝.

                    string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드

                    if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                    {
                        temp.code = "181818";
                        //서버에서 응답 자체를 못받았을 경우
                        AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                    }

                    else                                //정보를 받아왔을 때
                    {
                        JObject obj = JObject.Parse(str_response);
                        temp.code = (string)obj["code"];                        //ini에 code 정보 붙임
                        temp.errmsgvariable = (string)obj["errmsgvariable"];    //ini에 errmsgvariable 정보 붙임
                        temp.errmsg = (string)obj["errmsg"];                    //ini에 errmsg 정보 붙임
                        temp.key = (string)obj["key"];                          //ini에 key 정보 붙임

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("code : {0}", temp.code));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsgvariable : {0}", temp.errmsgvariable));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsg : {0}", temp.errmsg));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("key : {0}", temp.key));
                        }
                        //로그 기록

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 끝 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog("전체 동작 성공");
                        }
                        //로그 기록
                    }
                    //return temp;
                    
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(e.StackTrace.ToString());

                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("동작 실패");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e : {0}", e));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e.StackTrace : {0}", e.StackTrace));
                    }
                    //로그 기록
                    //return temp;
                    temp.code = "1818";
                }

                finally
                {
                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("모든 파싱 종료");
                    }
                    //로그 기록
                }
                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 07. 미방출 금액 전달  끝 === ");
                }
                //로그 기록
                return temp;
                
            }
            #endregion

            #region 8번
            public static AJ_RESPONSE_result temp_name_8(int localLogId, string isSuccess)
            {
                //var client = new RestClient("http://112.216.153.186:2080//api/equipment/post/log/result");
                string Server_path = "http://" + AJ_IP + ":" + AJ_PORT;
                Server_path += "//api/equipment/post/log/result";

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 08. Polling Data 성공 여부 시작 === ");
                    AJParkLib.AJCommon.CommonClass.SendLog("접속 주소 : " + Server_path);
                }
                //로그 기록

                AJ_RESPONSE_result temp = new AJ_RESPONSE_result();
                try
                {
                    var client = new RestClient(Server_path);
                    var request = new RestRequest(Method.POST);

                    //파라미터 추가
                    request.AddParameter("localLogId", localLogId);
                    request.AddParameter("isSuccess", isSuccess);
                    //파라미터 추가 끝.

                    //파라미터 로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("");
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 ------ ");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("localLogId : {0}", localLogId));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("isSuccess : {0}", isSuccess));
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 끝 ------ ");
                    }
                    //파라미터 로그 기록 끝.

                    string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드

                    if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                    {
                        temp.code = "181818";
                        //서버에서 응답 자체를 못받았을 경우
                        AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                    }

                    else                                //정보를 받아왔을 때
                    {
                        JObject obj = JObject.Parse(str_response);
                        temp.code = (string)obj["code"];                        //ini에 code 정보 붙임
                        temp.errmsgvariable = (string)obj["errmsgvariable"];    //ini에 errmsgvariable 정보 붙임
                        temp.errmsg = (string)obj["errmsg"];                    //ini에 errmsg 정보 붙임
                        temp.key = (string)obj["key"];                          //ini에 key 정보 붙임

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("code : {0}", temp.code));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsgvariable : {0}", temp.errmsgvariable));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsg : {0}", temp.errmsg));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("key : {0}", temp.key));
                        }
                        //로그 기록

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 끝 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog("전체 동작 성공");
                        }
                        //로그 기록

                    }
                    //return temp;
                    
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(e.StackTrace.ToString());

                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("동작 실패");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e : {0}", e));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e.StackTrace : {0}", e.StackTrace));
                    }
                    //로그 기록
                    //return temp;
                    temp.code = "1818";
                }

                finally
                {
                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("모든 파싱 종료");
                    }
                    //로그 기록
                }
                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 08. Polling Data 성공 여부 끝 === ");
                }//로그 기록
                return temp;
            }
            #endregion

            #region 9번
            //public static AJ_RESPONSE_log temp_name_9(int localEquipmentId) //파라미터 스트링으로 변경
            public static AJ_RESPONSE_log temp_name_9(string localEquipmentId) //파라미터 스트링으로 변경
            {
                //var client = new RestClient("http://112.216.153.186:2080//api/equipment/get/log");
                string Server_path = "http://" + AJ_IP + ":" + AJ_PORT;
                Server_path += "//api/equipment/get/log";

                //로그 기록
                if (use_Log_Write == true)
                {
//                     AJParkLib.AJCommon.CommonClass.SendLog("");
//                     AJParkLib.AJCommon.CommonClass.SendLog(" === 09. Polling Data 시작 === ");       //로그 기록
//                     AJParkLib.AJCommon.CommonClass.SendLog("접속 주소 : " + Server_path);
                }
                //로그 기록

                AJ_RESPONSE_log temp = new AJ_RESPONSE_log();

                try
                {
                    var client = new RestClient(Server_path);
                    var request = new RestRequest(Method.POST);

                    //파라미터 추가
                    request.AddParameter("localEquipmentId", localEquipmentId);
                    //파라미터 추가 끝.

                    //파라미터 로그 기록
                    if (use_Log_Write == true)
                    {
//                         AJParkLib.AJCommon.CommonClass.SendLog("");
//                         AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 ------ ");
//                         AJParkLib.AJCommon.CommonClass.SendLog(string.Format("localEquipmentId(장비 UID) : {0}", localEquipmentId));
//                         AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 끝 ------ ");
                    }
                    //파라미터 로그 기록 끝.

                    string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드

                    if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                    {
                        temp.code = "181818";
                        //서버에서 응답 자체를 못받았을 경우
                        AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                    }

                    else                                //정보를 받아왔을 때
                    {
                        JObject obj = JObject.Parse(str_response);
                        temp.code = (string)obj["code"];                        //ini에 code 정보 붙임
                        temp.errmsgvariable = (string)obj["errmsgvariable"];    //ini에 errmsgvariable 정보 붙임
                        temp.errmsg = (string)obj["errmsg"];                    //ini에 errmsg 정보 붙임
                        temp.key = (string)obj["key"];                          //ini에 key 정보 붙임

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("code : {0}", temp.code));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsgvariable : {0}", temp.errmsgvariable));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsg : {0}", temp.errmsg));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("key : {0}", temp.key));
                        }
                        //로그 기록


                        int temp_list_count = obj["list"].Count();              //리스트 카운트
                        log_list_RESPONSE[] temp_list = new log_list_RESPONSE[temp_list_count];     //카운트 만큼 리스트 생성

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");       //로그 기록
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 폴링정보 리스트 ------ ");       //로그 기록
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("폴링 정보 리스트의 개수 : {0}", temp_list_count));       //로그 기록
                        }
                        //로그 기록
                        
                        for (int i = 0; i < temp_list_count; i++)
                        {
                            temp_list[i].localLogId = (string)obj["list"][i]["localLogId"];
                            temp_list[i].localEquipmentId = (string)obj["list"][i]["localEquipmentId"];
                            temp_list[i].type = (string)obj["list"][i]["type"];
                            temp_list[i].content = (string)obj["list"][i]["content"];
                            temp_list[i].regDate = (string)obj["list"][i]["regDate"];
                            temp_list[i].isSuccess = (string)obj["list"][i]["isSuccess"];

                            temp_list[i].time = (string)obj["list"][i]["time"];     //0.9버전 추가
                            temp_list[i].carNo = (string)obj["list"][i]["carNo"];     //0.9버전 추가
                            temp_list[i].localReceiptInfoId = (string)obj["list"][i]["localReceiptInfoId"];     //0.9버전 추가
                            temp_list[i].localEquipmentKioskId = (string)obj["list"][i]["localEquipmentKioskId"];     //0.9버전 추가
                            temp_list[i].localDiscountInfoId = (string)obj["list"][i]["localDiscountInfoId"];     //1월 26일 추가
                            temp_list[i].SendPrice = (string)obj["list"][i]["sendPrice"];     //1월 26일 추가
                            
                            //로그 기록
                            if (use_Log_Write == true)
                            {
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> localLogId : {1}", i, temp_list[i].localLogId));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> localEquipmentId : {1}", i, temp_list[i].localEquipmentId));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> type : {1}", i, temp_list[i].type));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> content : {1}", i, temp_list[i].content));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> regDate : {1}", i, temp_list[i].regDate));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> isSuccess : {1}", i, temp_list[i].isSuccess));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> time : {1}", i, temp_list[i].time));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> carNo : {1}", i, temp_list[i].carNo));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> localReceiptInfoId : {1}", i, temp_list[i].localReceiptInfoId));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> localEquipmentKioskId : {1}", i, temp_list[i].localEquipmentKioskId));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> localDiscountInfoId : {1}", i, temp_list[i].localDiscountInfoId));
                            }
                            //로그 기록

                            //이큅먼트(단일)
                            log_list_RESPONSE_list_equipment equipment_list_temp = new log_list_RESPONSE_list_equipment();

                            equipment_list_temp.localEquipmentId = (string)obj["list"][i]["equipment"]["localEquipmentId"];
                            equipment_list_temp.equipmentId = (string)obj["list"][i]["equipment"]["equipmentId"];
                            equipment_list_temp.parkingLotId = (string)obj["list"][i]["equipment"]["parkingLotId"];
                            equipment_list_temp.type = (string)obj["list"][i]["equipment"]["type"];
                            equipment_list_temp.name = (string)obj["list"][i]["equipment"]["name"];
                            equipment_list_temp.equipmentStatus = (string)obj["list"][i]["equipment"]["equipmentStatus"];
                            equipment_list_temp.equipmentStatusUpdDate = (string)obj["list"][i]["equipment"]["equipmentStatusUpdDate"];
                            equipment_list_temp.status = (string)obj["list"][i]["equipment"]["status"];
                            equipment_list_temp.regDate = (string)obj["list"][i]["equipment"]["regDate"];
                            equipment_list_temp.equipmentNo = (string)obj["list"][i]["equipment"]["equipmentNo"];

                            equipment_list_temp.equipmentIp = (string)obj["list"][i]["equipment"]["equipmentIp"];
                            equipment_list_temp.equipmentPort = (string)obj["list"][i]["equipment"]["equipmentPort"];
                            equipment_list_temp.displayIp = (string)obj["list"][i]["equipment"]["displayIp"];
                            equipment_list_temp.displayPort = (string)obj["list"][i]["equipment"]["displayPort"];
                            equipment_list_temp.barrierPort = (string)obj["list"][i]["equipment"]["barrierPort"];
                            equipment_list_temp.location = (string)obj["list"][i]["equipment"]["location"];
                            equipment_list_temp.barrierControlOption = (string)obj["list"][i]["equipment"]["barrierControlOption"];
                            equipment_list_temp.carInfoSave = (string)obj["list"][i]["equipment"]["carInfoSave"];
                            equipment_list_temp.dvrIp = (string)obj["list"][i]["equipment"]["dvrIp"];
                            equipment_list_temp.dvrPort = (string)obj["list"][i]["equipment"]["dvrPort"];

                            equipment_list_temp.won500Price = (string)obj["list"][i]["equipment"]["won500Price"];
                            equipment_list_temp.won100Price = (string)obj["list"][i]["equipment"]["won100Price"];
                            equipment_list_temp.won5000 = (string)obj["list"][i]["equipment"]["won5000"];
                            equipment_list_temp.won1000 = (string)obj["list"][i]["equipment"]["won1000"];
                            equipment_list_temp.won500 = (string)obj["list"][i]["equipment"]["won500"];
                            equipment_list_temp.won100 = (string)obj["list"][i]["equipment"]["won100"];
                            equipment_list_temp.updateDate = (string)obj["list"][i]["equipment"]["updateDate"];
                            equipment_list_temp.set5000 = (string)obj["list"][i]["equipment"]["set5000"];
                            equipment_list_temp.set1000 = (string)obj["list"][i]["equipment"]["set1000"];
                            equipment_list_temp.set500 = (string)obj["list"][i]["equipment"]["set500"];

                            equipment_list_temp.set100 = (string)obj["list"][i]["equipment"]["set100"];
                            equipment_list_temp.ckId = (string)obj["list"][i]["equipment"]["ckId"];
                            equipment_list_temp.localCkId = (string)obj["list"][i]["equipment"]["localCkId"];

                            temp_list[i].equipment_list = equipment_list_temp;      //붙이기
                            //이큅먼트(단일) 끝.

                            //로그 기록
                            if (use_Log_Write == true)
                            {
                                AJParkLib.AJCommon.CommonClass.SendLog("");
                                AJParkLib.AJCommon.CommonClass.SendLog(" ------ 폴링정보 리스트의 이큅먼트 리스트 ------ ");
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipment ->  localEquipmentId : {1}", i, equipment_list_temp.localEquipmentId));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipment ->  equipmentId : {1}", i, equipment_list_temp.equipmentId));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipment ->  parkingLotId : {1}", i, equipment_list_temp.parkingLotId));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipment ->  type : {1}", i, equipment_list_temp.type));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipment ->  name : {1}", i, equipment_list_temp.name));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipment ->  equipmentStatus : {1}", i, equipment_list_temp.equipmentStatus));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipment ->  equipmentStatusUpdDate : {1}", i, equipment_list_temp.equipmentStatusUpdDate));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipment ->  status : {1}", i, equipment_list_temp.status));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipment ->  regDate : {1}", i, equipment_list_temp.regDate));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipment ->  equipmentNo : {1}", i, equipment_list_temp.equipmentNo));

                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipment ->  equipmentIp : {1}", i, equipment_list_temp.equipmentIp));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipment ->  equipmentPort : {1}", i, equipment_list_temp.equipmentPort));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipment ->  displayIp : {1}", i, equipment_list_temp.displayIp));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipment ->  displayPort : {1}", i, equipment_list_temp.displayPort));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipment ->  barrierPort : {1}", i, equipment_list_temp.barrierPort));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipment ->  location : {1}", i, equipment_list_temp.location));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipment ->  barrierControlOption : {1}", i, equipment_list_temp.barrierControlOption));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipment ->  carInfoSave : {1}", i, equipment_list_temp.carInfoSave));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipment ->  dvrIp : {1}", i, equipment_list_temp.dvrIp));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipment ->  dvrPort : {1}", i, equipment_list_temp.dvrPort));

                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipment ->  won500Price : {1}", i, equipment_list_temp.won500Price));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipment ->  won100Price : {1}", i, equipment_list_temp.won100Price));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipment ->  won5000 : {1}", i, equipment_list_temp.won5000));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipment ->  won1000 : {1}", i, equipment_list_temp.won1000));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipment ->  won500 : {1}", i, equipment_list_temp.won500));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipment ->  won100 : {1}", i, equipment_list_temp.won100));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipment ->  updateDate : {1}", i, equipment_list_temp.updateDate));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipment ->  set5000 : {1}", i, equipment_list_temp.set5000));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipment ->  set1000 : {1}", i, equipment_list_temp.set1000));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipment ->  set500 : {1}", i, equipment_list_temp.set500));

                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipment ->  set100 : {1}", i, equipment_list_temp.set100));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipment ->  ckId : {1}", i, equipment_list_temp.ckId));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipment ->  localCkId : {1}", i, equipment_list_temp.localCkId));

                                AJParkLib.AJCommon.CommonClass.SendLog(" ------ 폴링정보 리스트의 이큅먼트 리스트 끝 ------ ");
                                AJParkLib.AJCommon.CommonClass.SendLog("");
                            }
                            //로그 기록

                            //이큅먼트 키오스크(단일)
                            //log_list_RESPONSE_list_equipmentKiosk equipmentKiosk_list_temp = new log_list_RESPONSE_list_equipmentKiosk();

                            if (temp_list[i].type == "POWER" && temp_list[i].content == "RESET")
                            {
                                //이큅먼트 키오스크(단일)
                                log_list_RESPONSE_list_equipmentKiosk equipmentKiosk_list_temp = new log_list_RESPONSE_list_equipmentKiosk();

                                equipmentKiosk_list_temp.localEquipmentKioskId = (string)obj["list"][i]["equipmentKiosk"]["localEquipmentKioskId"];
                                equipmentKiosk_list_temp.equipmentKioskId = (string)obj["list"][i]["equipmentKiosk"]["equipmentKioskId"];
                                equipmentKiosk_list_temp.name = (string)obj["list"][i]["equipmentKiosk"]["name"];
                                equipmentKiosk_list_temp.port = (string)obj["list"][i]["equipmentKiosk"]["port"];
                                equipmentKiosk_list_temp.status = (string)obj["list"][i]["equipmentKiosk"]["status"];
                                equipmentKiosk_list_temp.equipmentId = (string)obj["list"][i]["equipmentKiosk"]["equipmentId"];
                                equipmentKiosk_list_temp.localEquipmentId = (string)obj["list"][i]["equipmentKiosk"]["localEquipmentId"];
                                equipmentKiosk_list_temp.equipmentKioskStatus = (string)obj["list"][i]["equipmentKiosk"]["equipmentKioskStatus"];
                                equipmentKiosk_list_temp.updateDate = (string)obj["list"][i]["equipmentKiosk"]["updateDate"];
                                equipmentKiosk_list_temp.isUse = (string)obj["list"][i]["equipmentKiosk"]["isUse"];

                                temp_list[i].equipmentKiosk_list = equipmentKiosk_list_temp;      //붙이기

                                //로그 기록
                                if (use_Log_Write == true)
                                {
                                    AJParkLib.AJCommon.CommonClass.SendLog("");
                                    AJParkLib.AJCommon.CommonClass.SendLog(" ------ 폴링정보 리스트의 이큅먼트 리스트 ------ ");
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipmentKiosk ->  localEquipmentKioskId : {1}", i, equipmentKiosk_list_temp.localEquipmentKioskId));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipmentKiosk ->  equipmentKioskId : {1}", i, equipmentKiosk_list_temp.equipmentKioskId));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipmentKiosk ->  name : {1}", i, equipmentKiosk_list_temp.name));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipmentKiosk ->  port : {1}", i, equipmentKiosk_list_temp.port));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipmentKiosk ->  status : {1}", i, equipmentKiosk_list_temp.status));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipmentKiosk ->  equipmentId : {1}", i, equipmentKiosk_list_temp.equipmentId));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipmentKiosk ->  localEquipmentId : {1}", i, equipmentKiosk_list_temp.localEquipmentId));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipmentKiosk ->  equipmentKioskStatus : {1}", i, equipmentKiosk_list_temp.equipmentKioskStatus));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipmentKiosk ->  updateDate : {1}", i, equipmentKiosk_list_temp.updateDate));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> equipmentKiosk ->  isUse : {1}", i, equipmentKiosk_list_temp.isUse));
                                    AJParkLib.AJCommon.CommonClass.SendLog(" ------ 폴링정보 리스트의 이큅먼트 키오스크 끝 ------ ");
                                    AJParkLib.AJCommon.CommonClass.SendLog("");
                                }
                                //로그 기록


                            }
                            //이큅먼트 키오스크(단일) 끝.




                            //파킹뢋(단일)
                            if (temp_list[i].type == "RECEIPT" && temp_list[i].content == "REISSUE")
                            {
                                log_list_RESPONSE_list_parkingLot parkingLot_list_temp = new log_list_RESPONSE_list_parkingLot();

                                parkingLot_list_temp.address = (string)obj["list"][i]["parkingLot"]["address"];
                                parkingLot_list_temp.companyNumber = (string)obj["list"][i]["parkingLot"]["companyNumber"];
                                parkingLot_list_temp.ownerName = (string)obj["list"][i]["parkingLot"]["ownerName"];
                                parkingLot_list_temp.companyName = (string)obj["list"][i]["parkingLot"]["companyName"];
                                parkingLot_list_temp.name = (string)obj["list"][i]["parkingLot"]["name"];
                                parkingLot_list_temp.tel = (string)obj["list"][i]["parkingLot"]["tel"];



                                temp_list[i].parkingLot_list = parkingLot_list_temp;      //붙이기

                                //로그 기록
                                if (use_Log_Write == true)
                                {
                                    AJParkLib.AJCommon.CommonClass.SendLog("");
                                    AJParkLib.AJCommon.CommonClass.SendLog(" ------ 폴링정보 리스트의 파킹뢋 ------ ");
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> parkingLot ->  address : {1}", i, parkingLot_list_temp.address));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> parkingLot ->  companyNumber : {1}", i, parkingLot_list_temp.companyNumber));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> parkingLot ->  ownerName : {1}", i, parkingLot_list_temp.ownerName));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> parkingLot ->  companyName : {1}", i, parkingLot_list_temp.companyName));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> parkingLot ->  name : {1}", i, parkingLot_list_temp.name));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> parkingLot ->  tel : {1}", i, parkingLot_list_temp.tel));
                                    AJParkLib.AJCommon.CommonClass.SendLog(" ------ 폴링정보 리스트의 파킹뢋 끝 ------ ");
                                    AJParkLib.AJCommon.CommonClass.SendLog("");
                                }
                                //로그 기록

                            }
                            //파킹뢋(단일) 끝.


                            //리쉡트(단일)
                            if (temp_list[i].type == "RECEIPT" && temp_list[i].content == "REISSUE")  //위와 조건문 동일, 가독성위해서
                            {
                                log_list_RESPONSE_list_receipt receipt_list_temp = new log_list_RESPONSE_list_receipt();

                                receipt_list_temp.approvalNo = (string)obj["list"][i]["receipt"]["approvalNo"];
                                receipt_list_temp.orderNumber = (string)obj["list"][i]["receipt"]["orderNumber"];
                                receipt_list_temp.releasePrice = (string)obj["list"][i]["receipt"]["releasePrice"];
                                receipt_list_temp.minutes = (string)obj["list"][i]["receipt"]["minutes"];
                                receipt_list_temp.parkingPrice = (string)obj["list"][i]["receipt"]["parkingPrice"];
                                receipt_list_temp.discountPrice = (string)obj["list"][i]["receipt"]["discountPrice"];
                                receipt_list_temp.acquirer = (string)obj["list"][i]["receipt"]["acquirer"];
                                receipt_list_temp.cardNo = (string)obj["list"][i]["receipt"]["cardNo"];
                                receipt_list_temp.issuer = (string)obj["list"][i]["receipt"]["issuer"];
                                receipt_list_temp.enterDate = (string)obj["list"][i]["receipt"]["enterDate"];

                                receipt_list_temp.carNo = (string)obj["list"][i]["receipt"]["carNo"];
                                receipt_list_temp.insertPrice = (string)obj["list"][i]["receipt"]["insertPrice"];
                                receipt_list_temp.localReceiptInfoId = (string)obj["list"][i]["receipt"]["localReceiptInfoId"];
                                receipt_list_temp.price = (string)obj["list"][i]["receipt"]["price"];
                                receipt_list_temp.leaveDate = (string)obj["list"][i]["receipt"]["leaveDate"];
                                receipt_list_temp.paymentMethod = (string)obj["list"][i]["receipt"]["paymentMethod"];
                                receipt_list_temp.equipmentName = (string)obj["list"][i]["receipt"]["equipmentName"];
                                receipt_list_temp.paymentDate = (string)obj["list"][i]["receipt"]["paymentDate"];
                                receipt_list_temp.approvedPrice = (string)obj["list"][i]["receipt"]["approvedPrice"];

                                temp_list[i].receipt_list = receipt_list_temp;      //붙이기

                                //로그 기록
                                if (use_Log_Write == true)
                                {
                                    AJParkLib.AJCommon.CommonClass.SendLog("");
                                    AJParkLib.AJCommon.CommonClass.SendLog(" ------ 폴링정보 리스트의 리쉡트 ------ ");
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> receipt ->  approvalNo : {1}", i, receipt_list_temp.approvalNo));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> receipt ->  orderNumber : {1}", i, receipt_list_temp.orderNumber));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> receipt ->  releasePrice : {1}", i, receipt_list_temp.releasePrice));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> receipt ->  minutes : {1}", i, receipt_list_temp.minutes));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> receipt ->  parkingPrice : {1}", i, receipt_list_temp.parkingPrice));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> receipt ->  discountPrice : {1}", i, receipt_list_temp.discountPrice));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> receipt ->  acquirer : {1}", i, receipt_list_temp.acquirer));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> receipt ->  cardNo : {1}", i, receipt_list_temp.cardNo));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> receipt ->  issuer : {1}", i, receipt_list_temp.issuer));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> receipt ->  enterDate : {1}", i, receipt_list_temp.enterDate));

                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> receipt ->  carNo : {1}", i, receipt_list_temp.carNo));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> receipt ->  insertPrice : {1}", i, receipt_list_temp.insertPrice));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> receipt ->  localReceiptInfoId : {1}", i, receipt_list_temp.localReceiptInfoId));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> receipt ->  price : {1}", i, receipt_list_temp.price));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> receipt ->  leaveDate : {1}", i, receipt_list_temp.leaveDate));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> receipt ->  paymentMethod : {1}", i, receipt_list_temp.paymentMethod));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> receipt ->  equipmentName : {1}", i, receipt_list_temp.equipmentName));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> receipt ->  paymentDate : {1}", i, receipt_list_temp.paymentDate));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> receipt ->  approvedPrice : {1}", i, receipt_list_temp.approvedPrice));
                                    AJParkLib.AJCommon.CommonClass.SendLog(" ------ 폴링정보 리스트의 리쉡트 끝 ------ ");
                                    AJParkLib.AJCommon.CommonClass.SendLog("");
                                }
                                //로그 기록

                            }
                            //리쉡트(단일) 끝.



                            //temp_list[i].equipment = (string)obj["list"][i]["equipment"];     //0.9버전 추가
                            //temp_list[i].equipmentKiosk = (string)obj["list"][i]["equipmentKiosk"];     //0.9버전 추가
                            //temp_list[i].parkingLot = (string)obj["list"][i]["parkingLot"];     //0.9버전 추가
                            //temp_list[i].receipt = (string)obj["list"][i]["receipt"];     //0.9버전 추가
                        }
                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            //AJParkLib.AJCommon.CommonClass.SendLog(" ------ 폴링정보 리스트 끝 ------ ");
                        }
                        //로그 기록
                        temp.log_list = temp_list;

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            //AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 끝 ------ ");
                            //AJParkLib.AJCommon.CommonClass.SendLog("전체 동작 성공");
                        }
                        //로그 기록
                    }
                    //return temp;
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(e.StackTrace.ToString());

                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("동작 실패");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e : {0}", e));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e.StackTrace : {0}", e.StackTrace));
                    }
                    //로그 기록
                    //return temp;
                    temp.code = "1818";
                }

                finally
                {
                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        //AJParkLib.AJCommon.CommonClass.SendLog("모든 파싱 종료");
                    }
                    //로그 기록
                }

                //로그 기록
                if (use_Log_Write == true)
                {
//                     AJParkLib.AJCommon.CommonClass.SendLog("");
//                     AJParkLib.AJCommon.CommonClass.SendLog(" === 09. Polling Data 끝 === ");       //로그 기록
                }
                //로그 기록
                return temp;
                

            }
            #endregion

            #region 10번(VIP 사용X)
            public static AJ_RESPONSE_vipInfo temp_name_10()
            {
                
                //var client = new RestClient("http://112.216.153.186:2080//api/equipment/get/vipInfo");
                string Server_path = "http://" + AJ_IP + ":" + AJ_PORT;
                Server_path += "//api/equipment/get/vipInfo";


                AJ_RESPONSE_vipInfo temp = new AJ_RESPONSE_vipInfo();

                try
                {
                    var client = new RestClient(Server_path);
                    var request = new RestRequest(Method.POST);

                    //파라미터 없음.


                    string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드

                    if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                    {
                        temp.code = "181818";
                        //서버에서 응답 자체를 못받았을 경우
                        AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                    }

                    else                                //정보를 받아왔을 때
                    {
                        JObject obj = JObject.Parse(str_response);
                        temp.code = (string)obj["code"];                        //ini에 code 정보 붙임
                        temp.errmsgvariable = (string)obj["errmsgvariable"];    //ini에 errmsgvariable 정보 붙임
                        temp.errmsg = (string)obj["errmsg"];                    //ini에 errmsg 정보 붙임
                        temp.key = (string)obj["key"];                          //ini에 key 정보 붙임



                        int temp_list_count = obj["vipInfo"].Count();
                        vipinfo_list_RESPONSE[] temp_list = new vipinfo_list_RESPONSE[temp_list_count];     //카운트 만큼 리스트 생성

                        for (int i = 0; i < temp_list_count; i++)
                        {
                            temp_list[i].carNo = (string)obj["vipInfo"][i]["carNo"];
                            temp_list[i].localVipId = (string)obj["vipInfo"][i]["localVipId"];
                        }
                        temp.vipinfo_list = temp_list;
                    }

                    return temp;
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(e.StackTrace.ToString());

                    AJParkLib.AJCommon.CommonClass.SendLog("동작 실패");
                    AJParkLib.AJCommon.CommonClass.SendLog(" - e.ToString() : " + e.ToString());       //로그 기록
                    AJParkLib.AJCommon.CommonClass.SendLog(" - e.StackTrace.ToString() : " + (e.StackTrace.ToString()));       //로그 기록
                    temp.code = "1818";
                    return temp;
                }

                
            }
            #endregion

            #region 11번(프로토콜상 10. 입차 미인식일 경우 입차내역 선택할 수 있는 시나리오 API)
            //public static AJ_RESPONSE_unkown_price temp_name_11(int localParkingId, string carNo, DateTime leaveDate)
            public static AJ_RESPONSE_unkown_price temp_name_11(string carNo)     //0.7에서 변경
            {
                //var client = new RestClient("http://112.216.153.186:2080//api/parking/unkown/leave/price");
                string Server_path = "http://" + AJ_IP + ":" + AJ_PORT;
                Server_path += "//api/parking/unkown/leave/price";

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 10. 출차 미인식 차량 목록 시작 === ");
                    AJParkLib.AJCommon.CommonClass.SendLog("접속 주소 : " + Server_path);
                }
                //로그 기록

                AJ_RESPONSE_unkown_price temp = new AJ_RESPONSE_unkown_price();

                try
                {
                    var client = new RestClient(Server_path);
                    var request = new RestRequest(Method.POST);

                    //파라미터 추가
                    //request.AddParameter("localParkingId", localParkingId);       //0.7에서 제외
                    request.AddParameter("carNo", carNo);

                    /*
                    //Datetime 형식을 포스트맨에 있는것 처럼 변경
                    string str_leaveDate;
                    double time_stamp = AJParkLib.AJCommon.Time.DateTime_To_MilliSec(leaveDate);
                    str_leaveDate = time_stamp.ToString();
                    str_leaveDate = str_leaveDate.Replace(".", "");         //13자리 밀리세컨드
                    request.AddParameter("leaveDate", str_leaveDate);       
                     * */
                    //0.7에서 제외
                    //파라미터 추가 끝.

                    //파라미터 로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("");
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 ------ ");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("carNo : {0}", carNo));
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 끝 ------ ");
                    }
                    //파라미터 로그 기록 끝.



                    string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드

                    if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                    {
                        temp.code = "181818";
                        //서버에서 응답 자체를 못받았을 경우
                        AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                    }
                    else                                //정보를 받아왔을 때
                    {
                        JObject obj = JObject.Parse(str_response);
                        temp.code = (string)obj["code"];                        //ini에 code 정보 붙임
                        temp.errmsgvariable = (string)obj["errmsgvariable"];    //ini에 errmsgvariable 정보 붙임
                        temp.errmsg = (string)obj["errmsg"];                    //ini에 errmsg 정보 붙임
                        temp.key = (string)obj["key"];                          //ini에 key 정보 붙임

                        //temp.parkingType = (string)obj["parkingType"];        //0.7에서 변경
                        //temp.realPrice = (string)obj["realPrice"];        //0.7에서 변경
                        //temp.price = (string)obj["price"];        //0.7에서 변경
                        //temp.discountPrice = (string)obj["discountPrice"];        //0.7에서 변경

                        //temp.localParkingId = (string)obj["localParkingId"];        //주차권 아이디, 0.7에서 변경
                        //temp.url = (string)obj["url"];        //입차 이미지 url, 0.7에서 변경
                        //temp.width = (string)obj["width"];        //이미지 가로 크기, 0.7에서 변경
                        //temp.height = (string)obj["height"];        //이미지 세로 크기, 0.7에서 변경
                        //temp.enterDate = (string)obj["enterDate"];        //이미지 세로 크기, 0.7에서 변경
                        //temp.carNo = (string)obj["carNo"];        //이미지 세로 크기, 0.7에서 변경

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("code : {0}", temp.code));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsgvariable : {0}", temp.errmsgvariable));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsg : {0}", temp.errmsg));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("key : {0}", temp.key));
                        }
                        //로그 기록



                        int unknownParkingList_count = obj["unknownParkingList"].Count();
                        unknownParkingList_RESPONSE[] unknownParkingList_count_list_temp = new unknownParkingList_RESPONSE[unknownParkingList_count];    //kioskinfo 구조체 선언

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 리스트 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("리스트의 개수 : {0}", unknownParkingList_count));
                        }
                        //로그 기록

                        


                        for (int i = 0; i < unknownParkingList_count; i++)               //키오스크 인포 탐색
                        {
                            unknownParkingList_count_list_temp[i].localParkingId = (string)obj["unknownParkingList"][i]["localParkingId"];        //주차권 아이디, 0.7에서 변경
                            //unknownParkingList_count_list_temp[i].url = Uri.UnescapeDataString((string)obj["unknownParkingList"][i]["url"]);        //주차권 아이디, 0.7에서 변경
                            unknownParkingList_count_list_temp[i].url = (string)obj["unknownParkingList"][i]["url"];        //주차권 아이디, 0.7에서 변경
                            unknownParkingList_count_list_temp[i].width = (string)obj["unknownParkingList"][i]["width"];        //주차권 아이디, 0.7에서 변경
                            unknownParkingList_count_list_temp[i].height = (string)obj["unknownParkingList"][i]["height"];        //주차권 아이디, 0.7에서 변경
                            unknownParkingList_count_list_temp[i].enterDate = (string)obj["unknownParkingList"][i]["enterDate"];        //주차권 아이디, 0.7에서 변경
                            unknownParkingList_count_list_temp[i].carNo = (string)obj["unknownParkingList"][i]["carNo"];        //주차권 아이디, 0.7에서 변경

                            //로그 기록
                            if (use_Log_Write == true)
                            {
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 unknownParkingList -> localParkingId : {1}", i, unknownParkingList_count_list_temp[i].localParkingId));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 unknownParkingList -> url : {1}", i, unknownParkingList_count_list_temp[i].url));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 unknownParkingList -> width : {1}", i, unknownParkingList_count_list_temp[i].width));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 unknownParkingList -> height : {1}", i, unknownParkingList_count_list_temp[i].height));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 unknownParkingList -> enterDate : {1}", i, unknownParkingList_count_list_temp[i].enterDate));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 unknownParkingList -> carNo : {1}", i, unknownParkingList_count_list_temp[i].carNo));
                            }
                            //로그 기록
                        }
                        temp.unknownParkingList = unknownParkingList_count_list_temp;
                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 리스트 끝 ------ ");       //로그 기록
                        }
                        //로그 기록

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 끝 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog("전체 동작 성공");
                        }
                        //로그 기록
                        
                    }
                    //return temp;

                    
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(e.StackTrace.ToString());

                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("동작 실패");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e : {0}", e));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e.StackTrace : {0}", e.StackTrace));
                    }
                    //로그 기록
                    //return temp;
                    temp.code = "1818";
                }

                finally
                {
                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("모든 파싱 종료");
                    }
                    //로그 기록
                }
                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 10. 출차 미인식 차량 목록 끝 === ");
                }
                //로그 기록
                return temp;
            }
            #endregion

            #region 11번_1(프로토콜상 10. 입차 미인식일 경우 입차내역 선택할 수 있는 시나리오 API) 테스트!!진남
            //public static AJ_RESPONSE_unkown_price temp_name_11(int localParkingId, string carNo, DateTime leaveDate)
            public static AJ_RESPONSE_unkown_price temp_name_11_1(string carNo, string in_car_date)     //0.7에서 변경
            {
                //var client = new RestClient("http://112.216.153.186:2080//api/parking/unkown/leave/price");
                string Server_path = "http://" + AJ_IP + ":" + AJ_PORT;
                Server_path += "//api/parking/unkown/leave/price";

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 10. 출차 미인식 차량 목록 시작 === ");
                    AJParkLib.AJCommon.CommonClass.SendLog("접속 주소 : " + Server_path);
                }
                //로그 기록

                AJ_RESPONSE_unkown_price temp = new AJ_RESPONSE_unkown_price();

                try
                {
                    var client = new RestClient(Server_path);
                    var request = new RestRequest(Method.POST);

                    //파라미터 추가
                    //request.AddParameter("localParkingId", localParkingId);       //0.7에서 제외
                    request.AddParameter("carNo", carNo);

                    /*
                    //Datetime 형식을 포스트맨에 있는것 처럼 변경
                    string str_leaveDate;
                    double time_stamp = AJParkLib.AJCommon.Time.DateTime_To_MilliSec(leaveDate);
                    str_leaveDate = time_stamp.ToString();
                    str_leaveDate = str_leaveDate.Replace(".", "");         //13자리 밀리세컨드
                    request.AddParameter("leaveDate", str_leaveDate);       
                     * */
                    //0.7에서 제외
                    //파라미터 추가 끝.

                    //파라미터 로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("");
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 ------ ");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("carNo : {0}", carNo));
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 끝 ------ ");
                    }
                    //파라미터 로그 기록 끝.



                    string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드

                    if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                    {
                        temp.code = "181818";
                        //서버에서 응답 자체를 못받았을 경우
                        AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                    }
                    else                                //정보를 받아왔을 때
                    {
                        JObject obj = JObject.Parse(str_response);
                        temp.code = (string)obj["code"];                        //ini에 code 정보 붙임
                        temp.errmsgvariable = (string)obj["errmsgvariable"];    //ini에 errmsgvariable 정보 붙임
                        temp.errmsg = (string)obj["errmsg"];                    //ini에 errmsg 정보 붙임
                        temp.key = (string)obj["key"];                          //ini에 key 정보 붙임

                        //temp.parkingType = (string)obj["parkingType"];        //0.7에서 변경
                        //temp.realPrice = (string)obj["realPrice"];        //0.7에서 변경
                        //temp.price = (string)obj["price"];        //0.7에서 변경
                        //temp.discountPrice = (string)obj["discountPrice"];        //0.7에서 변경

                        //temp.localParkingId = (string)obj["localParkingId"];        //주차권 아이디, 0.7에서 변경
                        //temp.url = (string)obj["url"];        //입차 이미지 url, 0.7에서 변경
                        //temp.width = (string)obj["width"];        //이미지 가로 크기, 0.7에서 변경
                        //temp.height = (string)obj["height"];        //이미지 세로 크기, 0.7에서 변경
                        //temp.enterDate = (string)obj["enterDate"];        //이미지 세로 크기, 0.7에서 변경
                        //temp.carNo = (string)obj["carNo"];        //이미지 세로 크기, 0.7에서 변경

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("code : {0}", temp.code));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsgvariable : {0}", temp.errmsgvariable));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsg : {0}", temp.errmsg));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("key : {0}", temp.key));
                        }
                        //로그 기록



                        int unknownParkingList_count = obj["unknownParkingList"].Count();
                        unknownParkingList_RESPONSE[] unknownParkingList_count_list_temp = new unknownParkingList_RESPONSE[unknownParkingList_count];    //kioskinfo 구조체 선언

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 리스트 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("리스트의 개수 : {0}", unknownParkingList_count));
                        }
                        //로그 기록




                        for (int i = 0; i < unknownParkingList_count; i++)               //키오스크 인포 탐색
                        {
                            string date_temp = (string)obj["unknownParkingList"][i]["enterDate"];

                            DateTime list_date = AJParkLib.AJCommon.Time.MilliSec_To_DateTime(Convert.ToDouble(date_temp));

                            int in_car_year = Convert.ToInt32(in_car_date.Substring(0, 4));
                            int in_car_month = Convert.ToInt32(in_car_date.Substring(4, 2));
                            int in_car_day = Convert.ToInt32(in_car_date.Substring(6, 2));
                            int in_car_hour = Convert.ToInt32(in_car_date.Substring(8, 2));
                            int in_car_minute = Convert.ToInt32(in_car_date.Substring(10, 2));
                            int in_car_second = Convert.ToInt32(in_car_date.Substring(12, 2));

                            DateTime date_in_car_date = new DateTime(in_car_year, in_car_month, in_car_day, in_car_hour, in_car_minute, in_car_second);

                            //DateTime date_in_car_date = new DateTime(in_car_date.Substring(0, 4), in_car_date.Substring(4, 2), in_car_date.Substring(6, 2), in_car_date.Substring(8, 2), in_car_date.Substring(10, 2), in_car_date.Substring(12,2));

                            //CurrentVehicleData.ParkingTime = CurrentVehicleData.ExitPassTime.Subtract(AJParkLib.AJCommon.Time.MilliSec_To_DateTime(Convert.ToDouble(returnData.enterDate)));

                            TimeSpan time_interval = date_in_car_date - list_date;


                            if (time_interval.Days > 15)
                            {
                                //break;
                                continue;
                            }

                            if (time_interval.TotalMilliseconds != 0)
                            {
                                continue;
                            }





                            //string str_date = string.Format("{0:yyyyMMddHHmmss}", list_date);


                            /*
                            if (Convert.ToDouble(in_car_date) > Convert.ToDouble(str_date))
                            {
                                int a = 10;
                            }
                            */

                            unknownParkingList_count_list_temp[i].localParkingId = (string)obj["unknownParkingList"][i]["localParkingId"];        //주차권 아이디, 0.7에서 변경
                            unknownParkingList_count_list_temp[i].url = Uri.UnescapeDataString((string)obj["unknownParkingList"][i]["url"]);        //주차권 아이디, 0.7에서 변경, 대리님이 변경 한글 깨지는거 때문에
                            //unknownParkingList_count_list_temp[i].url = (string)obj["unknownParkingList"][i]["url"];        //주차권 아이디, 0.7에서 변경
                            unknownParkingList_count_list_temp[i].width = (string)obj["unknownParkingList"][i]["width"];        //주차권 아이디, 0.7에서 변경
                            unknownParkingList_count_list_temp[i].height = (string)obj["unknownParkingList"][i]["height"];        //주차권 아이디, 0.7에서 변경
                            unknownParkingList_count_list_temp[i].enterDate = (string)obj["unknownParkingList"][i]["enterDate"];        //주차권 아이디, 0.7에서 변경
                            unknownParkingList_count_list_temp[i].carNo = (string)obj["unknownParkingList"][i]["carNo"];        //주차권 아이디, 0.7에서 변경

                            //로그 기록
                            if (use_Log_Write == true)
                            {
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 unknownParkingList -> localParkingId : {1}", i, unknownParkingList_count_list_temp[i].localParkingId));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 unknownParkingList -> url : {1}", i, unknownParkingList_count_list_temp[i].url));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 unknownParkingList -> width : {1}", i, unknownParkingList_count_list_temp[i].width));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 unknownParkingList -> height : {1}", i, unknownParkingList_count_list_temp[i].height));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 unknownParkingList -> enterDate : {1}", i, unknownParkingList_count_list_temp[i].enterDate));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 unknownParkingList -> carNo : {1}", i, unknownParkingList_count_list_temp[i].carNo));
                            }
                            //로그 기록

                            break;  //★
                        }
                        temp.unknownParkingList = unknownParkingList_count_list_temp;
                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 리스트 끝 ------ ");       //로그 기록
                        }
                        //로그 기록

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 끝 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog("전체 동작 성공");
                        }
                        //로그 기록

                    }
                    //return temp;


                }

                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(e.StackTrace.ToString());

                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("동작 실패");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e : {0}", e));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e.StackTrace : {0}", e.StackTrace));
                    }
                    //로그 기록
                    //return temp;
                    temp.code = "1818";
                }

                finally
                {
                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("모든 파싱 종료");
                    }
                    //로그 기록
                }
                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 10. 출차 미인식 차량 목록 끝 === ");
                }
                //로그 기록
                return temp;
            }
            #endregion

            #region 12번(프로토콜상 11. 입차 API)
            //public static AJ_RESPONSE_car_enter temp_name_12(string carNo, string image, DateTime enterDate, int localEquipmentId)
            public static AJ_RESPONSE_car_enter temp_name_12(string carNo, string image, DateTime enterDate, int localEquipmentId, string isSmallCar)      //0.7버전 변경
            {
                //var client = new RestClient("http://112.216.153.186:2080//api/parking/enter/insert");
                string Server_path = "http://" + AJ_IP + ":" + AJ_PORT;
                Server_path += "//api/parking/enter/insert";

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 11. 입차 API 시작 === ");
                    AJParkLib.AJCommon.CommonClass.SendLog("접속 주소 : " + Server_path);
                }
                //로그 기록

                AJ_RESPONSE_car_enter temp = new AJ_RESPONSE_car_enter();

                try
                {
                    Console.WriteLine(carNo);
                    Console.WriteLine(image);
                    Console.WriteLine(localEquipmentId);
                    Console.WriteLine(isSmallCar);
                    var client = new RestClient(Server_path);
                    var request = new RestRequest(Method.POST);

                    //파라미터 추가
                    request.AddParameter("carNo", carNo);
                    request.AddFile("image", image);
                    
                    string str_enterDate;
                    double time_stamp = AJParkLib.AJCommon.Time.DateTime_To_MilliSec(enterDate);

                    str_enterDate = Math.Floor(time_stamp).ToString();
                    str_enterDate = str_enterDate.Replace(".", "");
                    str_enterDate = time_stamp.ToString();
                    str_enterDate = str_enterDate.Replace(".", "");
                    str_enterDate = Math.Floor(time_stamp).ToString();

                    //str_enterDate = time_stamp.ToString();
                    //str_enterDate = str_enterDate.Replace(".", "");
                    request.AddParameter("enterDate", str_enterDate);

                    request.AddParameter("localEquipmentId", localEquipmentId);
                    request.AddParameter("isSmallCar", isSmallCar);                 //0.7버전 변경
                    //파라미터 추가 끝.

                    //파라미터 로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("");
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 ------ ");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("carNo : {0}", carNo));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("image : {0}", image));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("enterDate(DateTime) : {0}", enterDate));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("enterDate(TimeStamp) : {0}", str_enterDate));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("localEquipmentId : {0}", localEquipmentId));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("isSmallCar : {0}", isSmallCar));
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 끝 ------ ");
                    }
                    //파라미터 로그 기록 끝.

                    string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드

                    if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                    {
                        temp.code = "181818";
                        //서버에서 응답 자체를 못받았을 경우
                        AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                    }

                    else                                //정보를 받아왔을 때
                    {
                        JObject obj = JObject.Parse(str_response);
                        temp.code = (string)obj["code"];                        //ini에 code 정보 붙임
                        temp.errmsgvariable = (string)obj["errmsgvariable"];    //ini에 errmsgvariable 정보 붙임
                        temp.errmsg = (string)obj["errmsg"];                    //ini에 errmsg 정보 붙임
                        temp.key = (string)obj["key"];                          //ini에 key 정보 붙임

                        Console.WriteLine(temp.code);
                        Console.WriteLine(temp.errmsgvariable);
                        Console.WriteLine(temp.errmsg);
                        Console.WriteLine(temp.key);

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("code : {0}", temp.code));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsgvariable : {0}", temp.errmsgvariable));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsg : {0}", temp.errmsg));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("key : {0}", temp.key));
                        }
                        //로그 기록

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 끝 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog("전체 동작 성공");
                        }
                        //로그 기록

                    }
                    //return temp;

                    
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(e.StackTrace.ToString());

                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("동작 실패");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e : {0}", e));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e.StackTrace : {0}", e.StackTrace));
                    }
                    //로그 기록

                    //return temp;
                    temp.code = "1818";
                }
                finally
                {
                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("모든 파싱 종료");
                    }
                    //로그 기록
                }
                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 11. 입차 API 끝 ===");
                }
                //로그 기록
                
                return temp;
                
            }
            #endregion

            #region 13번(프로토콜상 12. 출차API)
            //public static AJ_RESPONSE_car_out temp_name_13(int localEquipmentId, string carNo, DateTime leaveDate, string paymentMethod, int price, int insertPrice, int releasePrice, string cardNo, string approvalNo, string issuer, string acquirer, int approvedPrice, DateTime approvedDate)
            //public static AJ_RESPONSE_car_out temp_name_13(int localEquipmentId, string carNo, DateTime leaveDate, string paymentMethod, int price, string isRecurrence, string isPrepay, int insertPrice, int releasePrice, string cardNo, string approvalNo, string issuer, string acquirer, int approvedPrice, DateTime approvedDate, int in50000, int in10000, int in5000, int in1000, int in500, int in100, int in50, int in10, int out5000, int out1000, int out500, int out100)
            //public static AJ_RESPONSE_car_out temp_name_13(int localEquipmentId, string carNo, DateTime leaveDate, string paymentMethod, int price, string isRecurrence, string isPrepay, string isNotCalcul, int insertPrice, int notReleasePrice, int releasePrice, string cardNo, string approvalNo, string issuer, string acquirer, int approvedPrice, DateTime approvedDate, int in50000, int in10000, int in5000, int in1000, int in500, int in100, int in50, int in10, int out5000, int out1000, int out500, int out100)      //0.7버전 변경
            //public static AJ_RESPONSE_car_out temp_name_13(int localEquipmentId, string image, string carNo, DateTime leaveDate, string paymentMethod, int price, string isRecurrence, string isPrepay, string isNotCalcul, int insertPrice, int notReleasePrice, int releasePrice, string cardNo, string approvalNo, string issuer, string acquirer, int approvedPrice, DateTime approvedDate, int in50000, int in10000, int in5000, int in1000, int in500, int in100, int in50, int in10, int out5000, int out1000, int out500, int out100)      //0.8버전 변경, image 리퀘스트 추가(2번째 파라미터)
            public static AJ_RESPONSE_car_out temp_name_13(int localEquipmentId, string carNo, DateTime leaveDate, string paymentMethod, int price, string isRecurrence, string isPrepay, string isNotCalcul, int insertPrice, int notReleasePrice, int releasePrice, string cardNo, string approvalNo, string issuer, string acquirer, int approvedPrice, DateTime approvedDate, int in50000, int in10000, int in5000, int in1000, int in500, int in100, int in50, int in10, int out5000, int out1000, int out500, int out100)      //1.2버전 변경, image 리퀘스트 삭제( (구)2번째 파라미터 )
            {
                //var client = new RestClient("http://112.216.153.186:2080//api/parking/receiptInfo/insert");
                string Server_path = "http://" + AJ_IP + ":" + AJ_PORT;
                Server_path += "//api/parking/receiptInfo/insert";

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 12. 출차 API 시작 === ");
                    AJParkLib.AJCommon.CommonClass.SendLog("접속 주소 : " + Server_path);
                }
                //로그 기록

                AJ_RESPONSE_car_out temp = new AJ_RESPONSE_car_out();

                try
                {
                    var client = new RestClient(Server_path);
                    var request = new RestRequest(Method.POST);

                    //파라미터 추가
                    request.AddParameter("localEquipmentId", localEquipmentId);

                    //                     System.IO.FileStream fs = new System.IO.FileStream(image, System.IO.FileMode.Open);
                    //                     byte[] ImageData = new byte [fs.Length];
                    //                     fs.Read(ImageData, 0, (int)fs.Length);

                    //1.3버전에서 출차 이미지 파라미터 삭제
                    //if (System.IO.File.Exists(@"D:\temp.jpg"))
                    //System.IO.File.Delete(@"D:\temp.jpg");

                    //System.IO.File.Copy(image, @"D:\temp.jpg");
                    //request.AddFile("image", @"D:\temp.jpg");            //0.8버전 추가
                    //1.3버전에서 출차 이미지 파라미터 삭제 끝.

                    request.AddParameter("carNo", carNo);

                    string str_leaveDate;
                    double time_stamp = AJParkLib.AJCommon.Time.DateTime_To_MilliSec(leaveDate);

                    str_leaveDate = Math.Floor(time_stamp).ToString();      //0117 변경
                    //str_leaveDate = time_stamp.ToString();
                    //str_leaveDate = str_leaveDate.Replace(".", "");
                    request.AddParameter("leaveDate", str_leaveDate);

                    request.AddParameter("paymentMethod", paymentMethod);
                    request.AddParameter("price", price);
                    request.AddParameter("isRecurrence", isRecurrence);
                    request.AddParameter("isPrepay", isPrepay);
                    request.AddParameter("isNotCalcul", isNotCalcul);
                    request.AddParameter("insertPrice", insertPrice);
                    request.AddParameter("notReleasePrice", notReleasePrice);
                    request.AddParameter("releasePrice", releasePrice);
                    request.AddParameter("cardNo", cardNo);
                    request.AddParameter("approvalNo", approvalNo);
                    request.AddParameter("issuer", issuer);
                    request.AddParameter("acquirer", acquirer);
                    request.AddParameter("approvedPrice", approvedPrice);

                    string str_approvedDate;
                    double time_stamp_2 = AJParkLib.AJCommon.Time.DateTime_To_MilliSec(approvedDate);
                    str_approvedDate = Math.Floor(time_stamp_2).ToString();
                    //str_approvedDate = time_stamp_2.ToString();
                    //str_approvedDate = str_approvedDate.Replace(".", "");
                    request.AddParameter("approvedDate", str_approvedDate);

                    request.AddParameter("in50000", in50000);
                    request.AddParameter("in10000", in10000);
                    request.AddParameter("in5000", in5000);
                    request.AddParameter("in1000", in1000);
                    request.AddParameter("in500", in500);
                    request.AddParameter("in100", in100);
                    request.AddParameter("in50", in50);
                    request.AddParameter("in10", in10);
                    request.AddParameter("out5000", out5000);
                    request.AddParameter("out1000", out1000);
                    request.AddParameter("out500", out500);
                    request.AddParameter("out100", out100);
                    //파라미터 추가 끝.

                    //파라미터 로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("");
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 ------ ");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("localEquipmentId : {0}", localEquipmentId));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("carNo : {0}", carNo));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("leaveDate(DateTime) : {0}", leaveDate));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("leaveDate(TimeStamp) : {0}", str_leaveDate));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("paymentMethod : {0}", paymentMethod));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("price : {0}", price));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("isRecurrence : {0}", isRecurrence));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("isPrepay : {0}", isPrepay));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("isNotCalcul : {0}", isNotCalcul));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("insertPrice : {0}", insertPrice));

                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("notReleasePrice : {0}", notReleasePrice));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("releasePrice : {0}", releasePrice));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("cardNo : {0}", cardNo));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("approvalNo : {0}", approvalNo));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("issuer : {0}", issuer));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("acquirer : {0}", acquirer));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("approvedPrice : {0}", approvedPrice));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("approvedDate(DateTime) : {0}", approvedDate));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("approvedDate(TimeStamp) : {0}", str_approvedDate));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("in50000 : {0}", in50000));

                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("in10000 : {0}", in10000));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("in5000 : {0}", in5000));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("in1000 : {0}", in1000));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("in500 : {0}", in500));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("in100 : {0}", in100));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("in50 : {0}", in50));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("in10 : {0}", in10));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("out5000 : {0}", out5000));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("out1000 : {0}", out1000));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("out500 : {0}", out500));

                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("out100 : {0}", out100));

                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 끝 ------ ");
                    }
                    //파라미터 로그 기록 끝.

                    string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드

                    if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                    {
                        temp.code = "181818";
                        //서버에서 응답 자체를 못받았을 경우
                        AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                    }

                    else                                //정보를 받아왔을 때
                    {
                        JObject obj = JObject.Parse(str_response);
                        temp.code = (string)obj["code"];                        //ini에 code 정보 붙임
                        temp.errmsgvariable = (string)obj["errmsgvariable"];    //ini에 errmsgvariable 정보 붙임
                        temp.errmsg = (string)obj["errmsg"];                    //ini에 errmsg 정보 붙임
                        temp.key = (string)obj["key"];                          //ini에 key 정보 붙임

                        temp.orderNumber = (string)obj["orderNumber"];          //0.9버전 추가
                        temp.localEquipmentId = (string)obj["localEquipmentId"];          //0.9버전 추가???

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("code : {0}", temp.code));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsgvariable : {0}", temp.errmsgvariable));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsg : {0}", temp.errmsg));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("key : {0}", temp.key));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("orderNumber : {0}", temp.orderNumber));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("localEquipmentId : {0}", temp.localEquipmentId));
                        }
                        //로그 기록

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 끝 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog("전체 동작 성공");
                        }
                        //로그 기록
                    }
                    //return temp;
                    
                    
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(e.StackTrace.ToString());

                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("동작 실패");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e : {0}", e));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e.StackTrace : {0}", e.StackTrace));
                    }
                    //로그 기록
                    //return temp;
                    temp.code = "1818";
                }

                finally
                {
                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("모든 파싱 종료");
                    }
                    //로그 기록
                }
                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 12. 출차 API 끝 === ");
                }
                //로그 기록
                
                return temp;
                

            }
            #endregion

            #region 14번(프로토콜 상 13. 요금 정산 API)
            public static AJ_RESPONSE_payment temp_name_14(string carNo, DateTime leaveDate, int localEquipmentId, string image, string isSmallCar, string localParkingId = "")  //0.8버전 변경
            {
                //var client = new RestClient("http://112.216.153.186:2080//api/parking/leave/price");
                string Server_path = "http://" + AJ_IP + ":" + AJ_PORT;
                Server_path += "//api/parking/leave/price";

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 13. 요금 정산 API 시작 === ");
                    AJParkLib.AJCommon.CommonClass.SendLog("접속 주소 : " + Server_path);
                }
                //로그 기록

                AJ_RESPONSE_payment temp = new AJ_RESPONSE_payment();
                FileStream fs = null;
                try
                {
                    var client = new RestClient(Server_path);
                    ////client.Proxy = SimpleWebProxy.Default;      //180529 추가
                    var request = new RestRequest(Method.POST);


                    //180530 추가 및 테스트
                    /*180530 테스트 usenagle
                     * https://code-examples.net/ko/q/119c2e0
                     * https://stackoverflow.com/questions/29914318/how-to-disable-nagles-algorithm-in-servicestack
                     * https://blogs.msdn.microsoft.com/windowsazurestorage/2010/06/25/nagles-algorithm-is-not-friendly-towards-small-requests/
                     * https://msdn.microsoft.com/ko-kr/library/system.net.servicepointmanager_methods(v=vs.110).aspx
                     * */

                    ////ServicePoint sp = ServicePointManager.FindServicePoint(Server_path, client.Proxy);
                    ////sp.UseNagleAlgorithm = false;
                    //180530 추가 및 테스트

                    //파라미터 추가
                    if (localParkingId != "")
                        request.AddParameter("localParkingId", localParkingId); //1.3버전 추가. 출차미인식차량의 입차권 아이디
                    request.AddParameter("carNo", carNo);
                    //request.AddFile("image", image);        //0.7버전 변경   -> 0.8버전에서 제거
                    string str_leaveDate;

                    //진남 테스트


                    double time_stamp = AJParkLib.AJCommon.Time.DateTime_To_MilliSec(leaveDate);

                    //한강테스트 위해서 시간 임의 조정
                    //DateTime aaa = new DateTime(2017, 6, 27, 10, 00, 00);         
                    //double time_stamp = AJParkLib.AJCommon.Time.DateTime_To_MilliSec(aaa);
                    //한강테스트 위해서 시간 임의 조정 끝

                    //진남 테스트 끝

                    str_leaveDate = Math.Floor(time_stamp).ToString();
                    //str_leaveDate = time_stamp.ToString();
                    //str_leaveDate = str_leaveDate.Replace(".", "");



                    //request.AddHeader("cache-control", "no-cache");
                    //request.AddHeader("content-type", "multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW");

                    request.AddParameter("leaveDate", str_leaveDate);
                    request.AddParameter("localEquipmentId", localEquipmentId);   //0.7버전 변경

                    //180905 진남 추가, 이미지 없을 때 강제로 다른 이미지 넣는다
                    try
                    {
                        if (image != "")
                        {
                            if (File.Exists(@"C:\a.jpg"))
                                File.Delete(@"C:\a.jpg");
                            //image = @"C:\123123.jpg";    //0221테스트, 지워야한다
                            File.Copy(image, @"C:\a.jpg");
                            fs = new FileStream(@"C:\a.jpg", FileMode.Open);
                            byte[] bData = new byte[fs.Length];
                            fs.Position = 0;
                            fs.Read(bData, 0, bData.Length);
                            request.AddFile("image", bData, image);            //1.3버전 추가 image : 이미지 파일 경로
                        }
                    }
                    //180905 진남 추가, 이미지 없을 때 강제로 다른 이미지 넣는다

                    catch (Exception ex)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog(" 요금 정산 로직 중 CATCH문으로 빠짐 !! 이미지 공유 폴더 확인 요망");
                        image = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Image\ImageLoadFail.jpg";
                        File.Copy(image, @"C:\a.jpg");
                        fs = new FileStream(@"C:\a.jpg", FileMode.Open);
                        byte[] bData = new byte[fs.Length];
                        fs.Position = 0;
                        fs.Read(bData, 0, bData.Length);
                        request.AddFile("image", bData, image);            //1.3버전 추가 image : 이미지 파일 경로
                    }


                    request.AddParameter("isSmallCar", isSmallCar);   //0.7버전 변경
                    //파라미터 추가 끝.

                    //파라미터 로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("");
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 ------ ");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("carNo : {0}", carNo));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("leaveDate(DateTime) : {0}", leaveDate));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("leaveDate(TimeStamp) : {0}", str_leaveDate));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("localEquipmentId : {0}", localEquipmentId));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("image : {0}", image));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("isSmallCar : {0}", isSmallCar));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("localParkingId : {0}", localParkingId));
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 끝 ------ ");
                    }
                    //파라미터 로그 기록 끝.
                    AJParkLib.AJCommon.CommonClass.SendLog("요청 시작!!");
                    string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드
                    AJParkLib.AJCommon.CommonClass.SendLog("요청 완료!!");
                    if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                    {
                        temp.code = "181818";
                        //서버에서 응답 자체를 못받았을 경우
                        AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                    }

                    else                                //정보를 받아왔을 때
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("파싱 시작!!");
                        JObject obj = JObject.Parse(str_response);
                        temp.code = (string)obj["code"];                        //ini에 code 정보 붙임
                        temp.errmsgvariable = (string)obj["errmsgvariable"];    //ini에 errmsgvariable 정보 붙임
                        temp.errmsg = (string)obj["errmsg"];                    //ini에 errmsg 정보 붙임
                        temp.key = (string)obj["key"];                          //ini에 key 정보 붙임

                        temp.parkingType = (string)obj["parkingType"];
                        temp.realPrice = (string)obj["realPrice"];
                        temp.price = (string)obj["price"];
                        temp.discountPrice = (string)obj["discountPrice"];
                        temp.CouponId = (string)obj["usercouponId"] == null ? 0 : Convert.ToInt32((string)obj["usercouponId"]);
                        temp.CouponPrice = (string)obj["usercouponPrice"] == null ? 0 : Convert.ToInt32((string)obj["usercouponPrice"]);

                        temp.enterDate = (string)obj["enterDate"];      //0.7버전 변경
                        temp.durationSecond = (string)obj["durationSecond"];      //0.7버전 변경
                        temp.prePayDiscountInfoId = (string)obj["prePayDiscountInfoId"];      //0.7버전 변경
                        temp.insertPrice = (string)obj["insertPrice"];      //0.7버전 변경
                        temp.recurrence = (string)obj["recurrence"];      //???????
                        temp.localParkingId = (string)obj["localParkingId"];      //1.3버전 추가, 주차권 UID
                        temp.lackPrice = (string)obj["lackPrice"];      //1.3버전 추가, 법인차량 정산시 부족금액
                        temp.prepayPrice = (string)obj["prepayPrice"];      //1.4버전 추가, 선결제 금액
                        AJParkLib.AJCommon.CommonClass.SendLog("파싱 완료!!");
                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("code : {0}", temp.code));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsgvariable : {0}", temp.errmsgvariable));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsg : {0}", temp.errmsg));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("key : {0}", temp.key));

                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("parkingType : {0}", temp.parkingType));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("realPrice : {0}", temp.realPrice));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("price : {0}", temp.price));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("discountPrice : {0}", temp.discountPrice));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("enterDate(TimeStamp) : {0}", temp.enterDate));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("enterDate(DateTime) : {0}", AJCommon.Time.MilliSec_To_DateTime(Convert.ToDouble(temp.enterDate))));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("durationSecond : {0}", temp.durationSecond));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("prePayDiscountInfoId : {0}", temp.prePayDiscountInfoId));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("insertPrice : {0}", temp.insertPrice));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("recurrence : {0}", temp.recurrence));

                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("localParkingId : {0}", temp.localParkingId));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("lackPrice : {0}", temp.lackPrice));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("prepayPrice : {0}", temp.prepayPrice));
                        }

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 끝 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog("전체 동작 성공");
                        }

                        //client.Delete(request);     //180611 TEST

                    }
                    //return temp;

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(e.StackTrace.ToString());

                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("동작 실패");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e : {0}", e));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e.StackTrace : {0}", e.StackTrace));
                    }
                    //로그 기록
                    //return temp;
                    temp.code = "1818";
                }

                finally
                {
                    if (fs != null)
                        fs.Close();
                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("모든 파싱 종료");
                    }
                    //로그 기록
                }
                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 13. 요금 정산 API 끝 === ");       //로그 기록
                }

                //로그 기록
                return temp;
            }
            #endregion

            #region 14_1번(프로토콜 상 13. 요금 정산 API) 입구 미인식 차량 전용 조회!!!!!!!
            //public static AJ_RESPONSE_payment temp_name_14(string carNo, DateTime leaveDate)
            //public static AJ_RESPONSE_payment temp_name_14(string carNo, string image, DateTime leaveDate, int localEquipmentId, string isSmallCar)  //0.7버전 변경
            public static AJ_RESPONSE_payment temp_name_14_1(string compare_Enterdate, string carNo, DateTime leaveDate, int localEquipmentId, string image, string isSmallCar, string localParkingId = "")  //0.8버전 변경
            {
                //var client = new RestClient("http://112.216.153.186:2080//api/parking/leave/price");
                string Server_path = "http://" + AJ_IP + ":" + AJ_PORT;
                Server_path += "//api/parking/leave/price";

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 13. 요금 정산 API 시작 === ");
                    AJParkLib.AJCommon.CommonClass.SendLog("접속 주소 : " + Server_path);
                }
                //로그 기록

                AJ_RESPONSE_payment temp = new AJ_RESPONSE_payment();

                try
                {
                    var client = new RestClient(Server_path);
                    var request = new RestRequest(Method.POST);

                    //파라미터 추가
                    if (localParkingId != "")
                        request.AddParameter("localParkingId", localParkingId); //1.3버전 추가. 출차미인식차량의 입차권 아이디
                    request.AddParameter("carNo", carNo);
                    //request.AddFile("image", image);        //0.7버전 변경   -> 0.8버전에서 제거
                    string str_leaveDate;
                    double time_stamp = AJParkLib.AJCommon.Time.DateTime_To_MilliSec(leaveDate);
                    str_leaveDate = Math.Floor(time_stamp).ToString();
                    //str_leaveDate = time_stamp.ToString();
                    //str_leaveDate = str_leaveDate.Replace(".", "");
                    request.AddParameter("leaveDate", str_leaveDate);
                    request.AddParameter("localEquipmentId", localEquipmentId);   //0.7버전 변경
                    request.AddFile("image", image);            //1.3버전 추가 image : 이미지 파일 경로
                    request.AddParameter("isSmallCar", isSmallCar);   //0.7버전 변경

                    //파라미터 추가 끝.

                    //파라미터 로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("");
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 ------ ");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("carNo : {0}", carNo));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("leaveDate(DateTime) : {0}", leaveDate));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("leaveDate(TimeStamp) : {0}", str_leaveDate));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("localEquipmentId : {0}", localEquipmentId));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("image : {0}", image));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("isSmallCar : {0}", isSmallCar));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("localParkingId : {0}", localParkingId));
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 끝 ------ ");
                    }
                    //파라미터 로그 기록 끝.

                    string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드

                    if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                    {
                        temp.code = "181818";
                        //서버에서 응답 자체를 못받았을 경우
                        AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                    }

                    else                                //정보를 받아왔을 때
                    {
                        JObject obj = JObject.Parse(str_response);
                        temp.code = (string)obj["code"];                        //ini에 code 정보 붙임
                        temp.errmsgvariable = (string)obj["errmsgvariable"];    //ini에 errmsgvariable 정보 붙임
                        temp.errmsg = (string)obj["errmsg"];                    //ini에 errmsg 정보 붙임
                        temp.key = (string)obj["key"];                          //ini에 key 정보 붙임

                        temp.parkingType = (string)obj["parkingType"];
                        temp.realPrice = (string)obj["realPrice"];
                        temp.price = (string)obj["price"];
                        temp.discountPrice = (string)obj["discountPrice"];

                        temp.enterDate = (string)obj["enterDate"];      //0.7버전 변경
                        temp.durationSecond = (string)obj["durationSecond"];      //0.7버전 변경
                        temp.prePayDiscountInfoId = (string)obj["prePayDiscountInfoId"];      //0.7버전 변경
                        temp.insertPrice = (string)obj["insertPrice"];      //0.7버전 변경
                        temp.recurrence = (string)obj["recurrence"];      //???????
                        temp.localParkingId = (string)obj["localParkingId"];      //1.3버전 추가, 주차권 UID
                        temp.localParkingId = (string)obj["lackPrice"];      //1.3버전 추가, 법인차량 정산시 부족금액
                        temp.prepayPrice = (string)obj["prepayPrice"];      //1.4버전 추가, 선결제 금액

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("code : {0}", temp.code));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsgvariable : {0}", temp.errmsgvariable));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsg : {0}", temp.errmsg));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("key : {0}", temp.key));

                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("parkingType : {0}", temp.parkingType));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("realPrice : {0}", temp.realPrice));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("price : {0}", temp.price));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("discountPrice : {0}", temp.discountPrice));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("enterDate(TimeStamp) : {0}", temp.enterDate));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("enterDate(DateTime) : {0}", AJCommon.Time.MilliSec_To_DateTime(Convert.ToDouble(temp.enterDate))));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("durationSecond : {0}", temp.durationSecond));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("prePayDiscountInfoId : {0}", temp.prePayDiscountInfoId));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("insertPrice : {0}", temp.insertPrice));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("recurrence : {0}", temp.recurrence));

                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("localParkingId : {0}", temp.localParkingId));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("lackPrice : {0}", temp.lackPrice));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("prepayPrice : {0}", temp.prepayPrice));
                        }
                        //로그 기록

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 끝 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog("전체 동작 성공");
                        }
                        //로그 기록
                    }
                    //return temp;

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(e.StackTrace.ToString());

                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("동작 실패");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e : {0}", e));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e.StackTrace : {0}", e.StackTrace));
                    }
                    //로그 기록
                    //return temp;
                    temp.code = "1818";
                }

                finally
                {
                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("모든 파싱 종료");
                    }
                    //로그 기록
                }
                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 13. 요금 정산 API 끝 === ");       //로그 기록
                }
                //로그 기록
                return temp;
            }
            #endregion

            #region 15번(프로토콜상 14. 할인권 투입 API)
            public static AJ_RESPONSE_discount_insert temp_name_15(int localDiscountInfoId, int barcodeDiscountId, string barcode, string isPrepayDiscount, string type, string carNo, DateTime leaveDate)
            {
                //var client = new RestClient("http://112.216.153.186:2080//api/parking/discount/insert");
                string Server_path = "http://" + AJ_IP + ":" + AJ_PORT;
                Server_path += "//api/parking/discount/insert";

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 14. 할인권 투입 API 시작 === ");
                    AJParkLib.AJCommon.CommonClass.SendLog("접속 주소 : " + Server_path);
                }
                //로그 기록

                AJ_RESPONSE_discount_insert temp = new AJ_RESPONSE_discount_insert();

                try
                {
                    var client = new RestClient(Server_path);
                    var request = new RestRequest(Method.POST);

                    //파라미터 추가
                    request.AddParameter("localDiscountInfoId", localDiscountInfoId);
                    //                     if(barcodeDiscountId == -1)
                    //                         request.AddParameter("barcodeDiscountId",  null);
                    //                     else
                    request.AddParameter("barcodeDiscountId", barcodeDiscountId);
                    //                     if(barcode == "")
                    //                         request.AddParameter("barcode", null);
                    //                     else
                    request.AddParameter("barcode", barcode);

                    request.AddParameter("isPrepayDiscount", isPrepayDiscount);
                    request.AddParameter("type", type);
                    request.AddParameter("carNo", carNo);

                    string str_leaveDate;
                    double time_stamp = AJParkLib.AJCommon.Time.DateTime_To_MilliSec(leaveDate);
                    str_leaveDate = Math.Floor(time_stamp).ToString();
                    //str_leaveDate = time_stamp.ToString();
                    //str_leaveDate = str_leaveDate.Replace(".", "");
                    request.AddParameter("leaveDate", str_leaveDate);
                    //파라미터 추가 끝.

                    //파라미터 로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("");
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 ------ ");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("localDiscountInfoId : {0}", localDiscountInfoId));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("barcodeDiscountId : {0}", barcodeDiscountId));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("barcode : {0}", barcode));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("isPrepayDiscount : {0}", isPrepayDiscount));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("type : {0}", type));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("carNo : {0}", carNo));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("leaveDate(Datetime) : {0}", leaveDate));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("leaveDate(TimeStamp) : {0}", str_leaveDate));
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 끝 ------ ");
                    }
                    //파라미터 로그 기록 끝.

                    string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드

                    if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                    {
                        temp.code = "181818";
                        //서버에서 응답 자체를 못받았을 경우
                        AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                    }

                    else                                //정보를 받아왔을 때
                    {
                        JObject obj = JObject.Parse(str_response);
                        temp.code = (string)obj["code"];                        //ini에 code 정보 붙임
                        temp.errmsgvariable = (string)obj["errmsgvariable"];    //ini에 errmsgvariable 정보 붙임
                        temp.errmsg = (string)obj["errmsg"];                    //ini에 errmsg 정보 붙임
                        temp.key = (string)obj["key"];                          //ini에 key 정보 붙임

                        temp.parkingType = (string)obj["parkingType"];
                        temp.realPrice = (string)obj["realPrice"];
                        temp.price = (string)obj["price"];
                        temp.discountPrice = (string)obj["discountPrice"];

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("code : {0}", temp.code));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsgvariable : {0}", temp.errmsgvariable));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsg : {0}", temp.errmsg));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("key : {0}", temp.key));

                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("parkingType : {0}", temp.parkingType));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("realPrice : {0}", temp.realPrice));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("price : {0}", temp.price));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("discountPrice : {0}", temp.discountPrice));
                        }
                        //로그 기록

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 끝 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog("전체 동작 성공");
                        }
                        //로그 기록
                    }

                    //return temp;
                    
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(e.StackTrace.ToString());

                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("동작 실패");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e : {0}", e));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e.StackTrace : {0}", e.StackTrace));
                    }
                    //로그 기록

                    //return temp;
                    temp.code = "1818";
                }

                finally
                {
                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("모든 파싱 종료");
                    }
                    //로그 기록
                }

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 14. 할인권 투입 API 끝 === ");
                }
                //로그 기록

                return temp;



            }
            #endregion

            #region 16번(프로토콜상 15. 요금 정산에 필요한 데이터를 내려주는 API)
            public static AJ_RESPONSE_paymentinfo temp_name_16()
            {
                //var client = new RestClient("http://112.216.153.186:2080//api/equipment/get/paymentInfo");
                string Server_path = "http://" + AJ_IP + ":" + AJ_PORT;
                Server_path += "//api/equipment/get/paymentInfo";

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 15. 요금 정산에 필요한 데이터를 내려주는 API 시작 === ");
                    AJParkLib.AJCommon.CommonClass.SendLog("접속 주소 : " + Server_path);
                }
                //로그 기록

                AJ_RESPONSE_paymentinfo temp = new AJ_RESPONSE_paymentinfo();

                try
                {
                    var client = new RestClient(Server_path);
                    var request = new RestRequest(Method.POST);

                    string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드

                    if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                    {
                        temp.code = "181818";
                        //서버에서 응답 자체를 못받았을 경우
                        AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                    }

                    else                                //정보를 받아왔을 때
                    {
                        JObject obj = JObject.Parse(str_response);
                        temp.code = (string)obj["code"];                        //ini에 code 정보 붙임
                        temp.errmsgvariable = (string)obj["errmsgvariable"];    //ini에 errmsgvariable 정보 붙임
                        temp.errmsg = (string)obj["errmsg"];                    //ini에 errmsg 정보 붙임
                        temp.key = (string)obj["key"];                          //ini에 key 정보 붙임

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("code : {0}", temp.code));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsgvariable : {0}", temp.errmsgvariable));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsg : {0}", temp.errmsg));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("key : {0}", temp.key));
                        }
                        //로그 기록

                        //밑의 로그는 일단 안해놨엉

                        //아이템(단일)
                        item_list_RESPONSE item_list_temp = new item_list_RESPONSE();

                        //아이템 - 코포레이트 카
                        int corporateCar_count = obj["item"]["corporateCar"].Count();
                        item_list_RESPONSE_list_corporateCar[] corporateCar_list_temp = new item_list_RESPONSE_list_corporateCar[corporateCar_count];
                        for (int i = 0; i < corporateCar_count; i++)
                        {
                            corporateCar_list_temp[i].carNo = (string)obj["item"]["corporateCar"][i]["carNo"];
                            corporateCar_list_temp[i].localCorporateCarId = (string)obj["item"]["corporateCar"][i]["localCorporateCarId"];
                            corporateCar_list_temp[i].price2 = (string)obj["item"]["corporateCar"][i]["price2"];
                        }

                        item_list_temp.corporateCar_list = corporateCar_list_temp;

                        //아이템 - 디스카운트 인포
                        int discountInfo_count = obj["item"]["discountInfo"].Count();
                        item_list_RESPONSE_list_discountInfo[] discountInfo_list_temp = new item_list_RESPONSE_list_discountInfo[discountInfo_count];
                        for (int i = 0; i < discountInfo_count; i++)
                        {
                            discountInfo_list_temp[i].groupName = (string)obj["item"]["discountInfo"][i]["groupName"];
                            discountInfo_list_temp[i].localDiscountInfoId = (string)obj["item"]["discountInfo"][i]["localDiscountInfoId"];
                            discountInfo_list_temp[i].groupCount = (string)obj["item"]["discountInfo"][i]["groupCount"];
                            discountInfo_list_temp[i].num = (string)obj["item"]["discountInfo"][i]["num"];
                            discountInfo_list_temp[i].name = (string)obj["item"]["discountInfo"][i]["name"];
                            discountInfo_list_temp[i].count = (string)obj["item"]["discountInfo"][i]["count"];
                            discountInfo_list_temp[i].isDup = (string)obj["item"]["discountInfo"][i]["isDup"];
                            discountInfo_list_temp[i].type = (string)obj["item"]["discountInfo"][i]["type"];
                            discountInfo_list_temp[i].status = (string)obj["item"]["discountInfo"][i]["status"];
                        }

                        item_list_temp.discountInfo_list = discountInfo_list_temp;

                        //아이템 - 바코드 디스카운트
                        int barcodeDiscount_count = obj["item"]["barcodeDiscount"].Count();
                        item_list_RESPONSE_list_barcodeDiscount[] barcodeDiscount_list_temp = new item_list_RESPONSE_list_barcodeDiscount[barcodeDiscount_count];
                        for (int i = 0; i < barcodeDiscount_count; i++)
                        {
                            barcodeDiscount_list_temp[i].barcodeDiscountId = (string)obj["item"]["barcodeDiscount"][i]["barcodeDiscountId"];
                            barcodeDiscount_list_temp[i].name = (string)obj["item"]["barcodeDiscount"][i]["name"];
                            barcodeDiscount_list_temp[i].localDiscountInfoId = (string)obj["item"]["barcodeDiscount"][i]["localDiscountInfoId"];
                            barcodeDiscount_list_temp[i].isDup = (string)obj["item"]["barcodeDiscount"][i]["isDup"];
                            barcodeDiscount_list_temp[i].status = (string)obj["item"]["barcodeDiscount"][i]["status"];
                            barcodeDiscount_list_temp[i].regDate = (string)obj["item"]["barcodeDiscount"][i]["regDate"];
                        }
                        item_list_temp.barcodeDiscount_list = barcodeDiscount_list_temp;

                        //아이템 - 디스카운트 파킹
                        int discountParking_count = obj["item"]["discountParking"].Count();
                        item_list_RESPONSE_list_discountParking[] discountParking_list_temp = new item_list_RESPONSE_list_discountParking[discountParking_count];
                        for (int i = 0; i < discountParking_count; i++)
                        {
                            discountParking_list_temp[i].localDiscountParkingId = (string)obj["item"]["discountParking"][i]["localDiscountParkingId"];
                            discountParking_list_temp[i].discountPrice = (string)obj["item"]["discountParking"][i]["discountPrice"];
                            discountParking_list_temp[i].localDiscountInfoId = (string)obj["item"]["discountParking"][i]["localDiscountInfoId"];
                            discountParking_list_temp[i].localParkingId = (string)obj["item"]["discountParking"][i]["localParkingId"];
                            discountParking_list_temp[i].type = (string)obj["item"]["discountParking"][i]["type"];
                            discountParking_list_temp[i].status = (string)obj["item"]["discountParking"][i]["status"];

                        }
                        item_list_temp.discountParking_list = discountParking_list_temp;

                        //아이템 - 먼쓸리 티켓
                        int monthlyTicket_count = obj["item"]["monthlyTicket"].Count();
                        item_list_RESPONSE_list_monthlyTicket[] monthlyTicket_list_temp = new item_list_RESPONSE_list_monthlyTicket[monthlyTicket_count];
                        for (int i = 0; i < monthlyTicket_count; i++)
                        {
                            monthlyTicket_list_temp[i].localMonthlyTicketId = (string)obj["item"]["monthlyTicket"][i]["localMonthlyTicketId"];
                            monthlyTicket_list_temp[i].fromDate = (string)obj["item"]["monthlyTicket"][i]["fromDate"];
                            monthlyTicket_list_temp[i].toDate = (string)obj["item"]["monthlyTicket"][i]["toDate"];
                            monthlyTicket_list_temp[i].carNo = (string)obj["item"]["monthlyTicket"][i]["carNo"];
                            monthlyTicket_list_temp[i].price = (string)obj["item"]["monthlyTicket"][i]["price"];
                            monthlyTicket_list_temp[i].discountPrice = (string)obj["item"]["monthlyTicket"][i]["discountPrice"];
                            monthlyTicket_list_temp[i].point = (string)obj["item"]["monthlyTicket"][i]["point"];
                            monthlyTicket_list_temp[i].paymentMethod = (string)obj["item"]["monthlyTicket"][i]["paymentMethod"];
                            monthlyTicket_list_temp[i].status = (string)obj["item"]["monthlyTicket"][i]["status"];
                            monthlyTicket_list_temp[i].isUse = (string)obj["item"]["monthlyTicket"][i]["isUse"];
                        }
                        item_list_temp.monthlyTicket_list = monthlyTicket_list_temp;

                        //아이템 - 파킹
                        int parking_count = obj["item"]["parking"].Count();
                        item_list_RESPONSE_list_parking[] parking_list_temp = new item_list_RESPONSE_list_parking[parking_count];
                        for (int i = 0; i < parking_count; i++)
                        {
                            parking_list_temp[i].localParkingId = (string)obj["item"]["parking"][i]["localParkingId"];
                            parking_list_temp[i].localCorporateCarId = (string)obj["item"]["parking"][i]["localCorporateCarId"];
                            parking_list_temp[i].carNo = (string)obj["item"]["parking"][i]["carNo"];
                            parking_list_temp[i].carNoBack = (string)obj["item"]["parking"][i]["carNoBack"];
                            parking_list_temp[i].type = (string)obj["item"]["parking"][i]["type"];
                            parking_list_temp[i].priceType = (string)obj["item"]["parking"][i]["priceType"];
                            parking_list_temp[i].prepayPrice = (string)obj["item"]["parking"][i]["prepayPrice"];
                            parking_list_temp[i].price = (string)obj["item"]["parking"][i]["price"];
                            parking_list_temp[i].discountPrice = (string)obj["item"]["parking"][i]["discountPrice"];
                            parking_list_temp[i].point = (string)obj["item"]["parking"][i]["point"];
                            parking_list_temp[i].priceCalculator = (string)obj["item"]["parking"][i]["priceCalculator"];
                            parking_list_temp[i].insertPrice = (string)obj["item"]["parking"][i]["insertPrice"];
                            parking_list_temp[i].notReleasePrice = (string)obj["item"]["parking"][i]["notReleasePrice"];
                            parking_list_temp[i].paymentDate = (string)obj["item"]["parking"][i]["paymentDate"];
                            parking_list_temp[i].enterDate = (string)obj["item"]["parking"][i]["enterDate"];
                            parking_list_temp[i].leaveDate = (string)obj["item"]["parking"][i]["leaveDate"];
                            parking_list_temp[i].leaveScheduleDate = (string)obj["item"]["parking"][i]["leaveScheduleDate"];
                            parking_list_temp[i].corporateCarRemainPrice = (string)obj["item"]["parking"][i]["corporateCarRemainPrice"];
                            parking_list_temp[i].orderNumber = (string)obj["item"]["parking"][i]["orderNumber"];
                            parking_list_temp[i].isApp = (string)obj["item"]["parking"][i]["isApp"];
                            parking_list_temp[i].isAjpass = (string)obj["item"]["parking"][i]["isAjpass"];
                            parking_list_temp[i].status = (string)obj["item"]["parking"][i]["status"];
                        }
                        item_list_temp.parking_list = parking_list_temp;

                        //아이템 - 파킹 뢋(단일)
                        item_list_RESPONSE_list_parkingLot parkingLot_list_temp = new item_list_RESPONSE_list_parkingLot();
                        parkingLot_list_temp.parkingLotId = (string)obj["item"]["parkingLot"]["parkingLotId"];
                        parkingLot_list_temp.name = (string)obj["item"]["parkingLot"]["name"];
                        parkingLot_list_temp.tel = (string)obj["item"]["parkingLot"]["tel"];
                        parkingLot_list_temp.latitude = (string)obj["item"]["parkingLot"]["latitude"];
                        parkingLot_list_temp.longitude = (string)obj["item"]["parkingLot"]["longitude"];
                        parkingLot_list_temp.addr1 = (string)obj["item"]["parkingLot"]["addr1"];
                        parkingLot_list_temp.addr2 = (string)obj["item"]["parkingLot"]["addr2"];
                        parkingLot_list_temp.isAjpass = (string)obj["item"]["parkingLot"]["isAjpass"];
                        parkingLot_list_temp.isSuvRv = (string)obj["item"]["parkingLot"]["isSuvRv"];
                        parkingLot_list_temp.spaceType = (string)obj["item"]["parkingLot"]["spaceType"];

                        parkingLot_list_temp.spaceNum = (string)obj["item"]["parkingLot"]["spaceNum"];
                        parkingLot_list_temp.freeOpenInfo = (string)obj["item"]["parkingLot"]["freeOpenInfo"];
                        parkingLot_list_temp.dailyPrice = (string)obj["item"]["parkingLot"]["dailyPrice"];
                        parkingLot_list_temp.monthlyTicketPrice = (string)obj["item"]["parkingLot"]["monthlyTicketPrice"];
                        parkingLot_list_temp.monthlyTicketType = (string)obj["item"]["parkingLot"]["monthlyTicketType"];
                        parkingLot_list_temp.monthlyTicketComment = (string)obj["item"]["parkingLot"]["monthlyTicketComment"];
                        parkingLot_list_temp.monthlyTicketNum = (string)obj["item"]["parkingLot"]["monthlyTicketNum"];
                        parkingLot_list_temp.tagPrice = (string)obj["item"]["parkingLot"]["tagPrice"];
                        parkingLot_list_temp.additionalPriceInfoTitle = (string)obj["item"]["parkingLot"]["additionalPriceInfoTitle"];
                        parkingLot_list_temp.additionalPriceInfoPrice = (string)obj["item"]["parkingLot"]["additionalPriceInfoPrice"];

                        parkingLot_list_temp.discountType = (string)obj["item"]["parkingLot"]["discountType"];
                        parkingLot_list_temp.spaceHorizontal = (string)obj["item"]["parkingLot"]["spaceHorizontal"];
                        parkingLot_list_temp.spaceVertical = (string)obj["item"]["parkingLot"]["spaceVertical"];
                        parkingLot_list_temp.garageNum = (string)obj["item"]["parkingLot"]["garageNum"];
                        parkingLot_list_temp.availableCarType = (string)obj["item"]["parkingLot"]["availableCarType"];
                        parkingLot_list_temp.availableParkingType = (string)obj["item"]["parkingLot"]["availableParkingType"];
                        parkingLot_list_temp.isWidewidth = (string)obj["item"]["parkingLot"]["isWidewidth"];
                        parkingLot_list_temp.priceCalculator = (string)obj["item"]["parkingLot"]["priceCalculator"];
                        parkingLot_list_temp.comment = (string)obj["item"]["parkingLot"]["comment"];
                        parkingLot_list_temp.holidayInfo = (string)obj["item"]["parkingLot"]["holidayInfo"];

                        parkingLot_list_temp.smallCarDiscount = (string)obj["item"]["parkingLot"]["smallCarDiscount"];
                        parkingLot_list_temp.disableCarDiscount = (string)obj["item"]["parkingLot"]["disableCarDiscount"];
                        parkingLot_list_temp.manOfMeritDiscount = (string)obj["item"]["parkingLot"]["manOfMeritDiscount"];
                        parkingLot_list_temp.operationType = (string)obj["item"]["parkingLot"]["operationType"];
                        parkingLot_list_temp.location = (string)obj["item"]["parkingLot"]["location"];
                        parkingLot_list_temp.holOpenTime = (string)obj["item"]["parkingLot"]["holOpenTime"];
                        parkingLot_list_temp.horizontal = (string)obj["item"]["parkingLot"]["horizontal"];
                        parkingLot_list_temp.vertical = (string)obj["item"]["parkingLot"]["vertical"];
                        parkingLot_list_temp.femaleSpaceNum = (string)obj["item"]["parkingLot"]["femaleSpaceNum"];
                        parkingLot_list_temp.disableSpaceNum = (string)obj["item"]["parkingLot"]["disableSpaceNum"];

                        parkingLot_list_temp.dailyOpenTime = (string)obj["item"]["parkingLot"]["dailyOpenTime"];
                        parkingLot_list_temp.satOpenTime = (string)obj["item"]["parkingLot"]["satOpenTime"];
                        parkingLot_list_temp.etcOpenTime = (string)obj["item"]["parkingLot"]["etcOpenTime"];
                        parkingLot_list_temp.oneHourPrice = (string)obj["item"]["parkingLot"]["oneHourPrice"];
                        parkingLot_list_temp.rateExceptionInfo = (string)obj["item"]["parkingLot"]["rateExceptionInfo"];
                        parkingLot_list_temp.price = (string)obj["item"]["parkingLot"]["price"];
                        parkingLot_list_temp.addPrice = (string)obj["item"]["parkingLot"]["addPrice"];
                        parkingLot_list_temp.type = (string)obj["item"]["parkingLot"]["type"];
                        parkingLot_list_temp.homepageMsgView = (string)obj["item"]["parkingLot"]["homepageMsgView"];
                        parkingLot_list_temp.status = (string)obj["item"]["parkingLot"]["status"];

                        parkingLot_list_temp.garageVertical = (string)obj["item"]["parkingLot"]["garageVertical"];
                        parkingLot_list_temp.garageHorizontal = (string)obj["item"]["parkingLot"]["garageHorizontal"];
                        parkingLot_list_temp.regDate = (string)obj["item"]["parkingLot"]["regDate"];
                        parkingLot_list_temp.priceRound = (string)obj["item"]["parkingLot"]["priceRound"];
                        parkingLot_list_temp.isTag = (string)obj["item"]["parkingLot"]["isTag"];
                        parkingLot_list_temp.discountPriority = (string)obj["item"]["parkingLot"]["discountPriority"];
                        parkingLot_list_temp.paperDiscountReUse = (string)obj["item"]["parkingLot"]["paperDiscountReUse"];
                        parkingLot_list_temp.ownerName = (string)obj["item"]["parkingLot"]["ownerName"];
                        parkingLot_list_temp.companyNumber = (string)obj["item"]["parkingLot"]["companyNumber"];
                        parkingLot_list_temp.discountMaxCount = (string)obj["item"]["parkingLot"]["discountMaxCount"];

                        parkingLot_list_temp.monthlyLength = (string)obj["item"]["parkingLot"]["monthlyLength"];
                        parkingLot_list_temp.isWebDiscount = (string)obj["item"]["parkingLot"]["isWebDiscount"];
                        parkingLot_list_temp.receipt = (string)obj["item"]["parkingLot"]["receipt"];
                        parkingLot_list_temp.cashReceipt = (string)obj["item"]["parkingLot"]["cashReceipt"];
                        parkingLot_list_temp.homepageMonthlyPrice = (string)obj["item"]["parkingLot"]["homepageMonthlyPrice"];
                        parkingLot_list_temp.homepageComment = (string)obj["item"]["parkingLot"]["homepageComment"];
                        parkingLot_list_temp.timePriceDiscount = (string)obj["item"]["parkingLot"]["timePriceDiscount"];
                        parkingLot_list_temp.addDiscountPriceTypeId = (string)obj["item"]["parkingLot"]["addDiscountPriceTypeId"];
                        parkingLot_list_temp.monthlySmallCarDiscount = (string)obj["item"]["parkingLot"]["monthlySmallCarDiscount"];
                        parkingLot_list_temp.monthlyDisableCarDiscount = (string)obj["item"]["parkingLot"]["monthlyDisableCarDiscount"];

                        parkingLot_list_temp.monthlyManOfMeritDiscount = (string)obj["item"]["parkingLot"]["monthlyManOfMeritDiscount"];
                        parkingLot_list_temp.companyName = (string)obj["item"]["parkingLot"]["companyName"];
                        parkingLot_list_temp.updateDate = (string)obj["item"]["parkingLot"]["updateDate"];
                        parkingLot_list_temp.isCarDisWithOtherDis = (string)obj["item"]["parkingLot"]["isCarDisWithOtherDis"];
                        parkingLot_list_temp.isSync = (string)obj["item"]["parkingLot"]["isSync"];
                        parkingLot_list_temp.extinctPeriod = (string)obj["item"]["parkingLot"]["extinctPeriod"];
                        parkingLot_list_temp.localSmallCarDiscount = (string)obj["item"]["parkingLot"]["localSmallCarDiscount"];
                        parkingLot_list_temp.localDisableCarDiscount = (string)obj["item"]["parkingLot"]["localDisableCarDiscount"];
                        parkingLot_list_temp.localManOfMeritDiscount = (string)obj["item"]["parkingLot"]["localManOfMeritDiscount"];
                        parkingLot_list_temp.localAddDiscountPriceTypeId = (string)obj["item"]["parkingLot"]["localAddDiscountPriceTypeId"];

                        parkingLot_list_temp.rotationSystem = (string)obj["item"]["parkingLot"]["rotationSystem"];      //부제 기능 170613 추가
                        parkingLot_list_temp.pcmDiscount = (string)obj["item"]["parkingLot"]["pcmDiscount"];
                        item_list_temp.parkingLot_list = parkingLot_list_temp;


                        //아이템 - 파킹 뢋 홀리데이
                        int parkingLotHoliday_count = obj["item"]["parkingLotHoliday"].Count();
                        item_list_RESPONSE_list_parkingLotHoliday[] parkingLotHoliday_list_temp = new item_list_RESPONSE_list_parkingLotHoliday[parkingLotHoliday_count];
                        for (int i = 0; i < parkingLotHoliday_count; i++)
                        {
                            parkingLotHoliday_list_temp[i].localParkingLotHolidayId = (string)obj["item"]["parkingLotHoliday"][i]["localParkingLotHolidayId"];
                            parkingLotHoliday_list_temp[i].holiday = (string)obj["item"]["parkingLotHoliday"][i]["holiday"];
                            parkingLotHoliday_list_temp[i].content = (string)obj["item"]["parkingLotHoliday"][i]["content"];
                            parkingLotHoliday_list_temp[i].status = (string)obj["item"]["parkingLotHoliday"][i]["status"];
                        }
                        item_list_temp.parkingLotHoliday_list = parkingLotHoliday_list_temp;

                        //아이템 - 프라이스 인포
                        int priceInfo_count = obj["item"]["priceInfo"].Count();
                        item_list_RESPONSE_list_priceInfo[] priceInfo_list_temp = new item_list_RESPONSE_list_priceInfo[priceInfo_count];
                        for (int i = 0; i < priceInfo_count; i++)
                        {
                            priceInfo_list_temp[i].localPriceInfoId = (string)obj["item"]["priceInfo"][i]["localPriceInfoId"];
                            priceInfo_list_temp[i].basicTimeMinute = (string)obj["item"]["priceInfo"][i]["basicTimeMinute"];
                            priceInfo_list_temp[i].addTimeMinute = (string)obj["item"]["priceInfo"][i]["addTimeMinute"];
                            priceInfo_list_temp[i].dailyMaxPrice = (string)obj["item"]["priceInfo"][i]["dailyMaxPrice"];
                            priceInfo_list_temp[i].standardTime = (string)obj["item"]["priceInfo"][i]["standardTime"];
                            priceInfo_list_temp[i].dayOfWeek = (string)obj["item"]["priceInfo"][i]["dayOfWeek"];
                            priceInfo_list_temp[i].name = (string)obj["item"]["priceInfo"][i]["name"];
                            priceInfo_list_temp[i].startTime = (string)obj["item"]["priceInfo"][i]["startTime"];
                            priceInfo_list_temp[i].endTime = (string)obj["item"]["priceInfo"][i]["endTime"];
                            priceInfo_list_temp[i].maxPrice = (string)obj["item"]["priceInfo"][i]["maxPrice"];
                            priceInfo_list_temp[i].recurrenceTime = (string)obj["item"]["priceInfo"][i]["recurrenceTime"];
                            priceInfo_list_temp[i].respiteTime = (string)obj["item"]["priceInfo"][i]["respiteTime"];
                            priceInfo_list_temp[i].status = (string)obj["item"]["priceInfo"][i]["status"];
                        }
                        item_list_temp.priceInfo_list = priceInfo_list_temp;

                        //아이템 - 프라이스 타입
                        int priceType_count = obj["item"]["priceType"].Count();
                        item_list_RESPONSE_list_priceType[] priceType_list_temp = new item_list_RESPONSE_list_priceType[priceType_count];
                        for (int i = 0; i < priceType_count; i++)
                        {
                            priceType_list_temp[i].localPriceTypeId = (string)obj["item"]["priceType"][i]["localPriceTypeId"];
                            priceType_list_temp[i].localPriceInfoId = (string)obj["item"]["priceType"][i]["localPriceInfoId"];
                            priceType_list_temp[i].basicPrice = (string)obj["item"]["priceType"][i]["basicPrice"];
                            priceType_list_temp[i].addPrice = (string)obj["item"]["priceType"][i]["addPrice"];
                            priceType_list_temp[i].typeName = (string)obj["item"]["priceType"][i]["typeName"];

                        }
                        item_list_temp.priceType_list = priceType_list_temp;

                        string aaaa = (string)obj["item"]["parkingLot"]["name"];
                        aaaa = (string)obj["item"]["parkingLot"]["tel"];
                        aaaa = (string)obj["item"]["parkingLot"]["comment"];

                        //전부 다 붙였으니까 이제 응답 클래스에 넣기
                        temp.item_list = item_list_temp;

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 끝 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog("전체 동작 성공");
                        }
                        //로그 기록

                    }
                    
                    //return temp;
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(e.StackTrace.ToString());

                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("동작 실패");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e : {0}", e));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e.StackTrace : {0}", e.StackTrace));
                    }
                    //로그 기록
                    //return temp;
                    temp.code = "1818";
                }

                finally
                {
                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("모든 파싱 종료");
                    }
                    //로그 기록
                }

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 15. 요금 정산에 필요한 데이터를 내려주는 API 끝 === ");       //로그 기록
                }
                //로그 기록

                return temp;

            }
            #endregion

            #region 17번(프로토콜상 16. 장비 온라인 여부 확인 API)
            public static AJ_RESPONSE_equipment_update temp_name_17(int localEquipmentId)
            {
                //POST //api/equipment/post/equipment/update HTTP/1.1
                //Host: 112.216.153.186:2080
                string Server_path = "http://" + AJ_IP + ":" + AJ_PORT;
                Server_path += "//api/equipment/post/equipment/update";

                //로그 기록
                if (use_Log_Write == true)
                {
//                     AJParkLib.AJCommon.CommonClass.SendLog("");
//                     AJParkLib.AJCommon.CommonClass.SendLog(" === 16. 장비 온라인 여부 확인 API 시작 === ");       //로그 기록
//                     AJParkLib.AJCommon.CommonClass.SendLog("접속 주소 : " + Server_path);
                }
                //로그 기록

                AJ_RESPONSE_equipment_update temp = new AJ_RESPONSE_equipment_update();

                try
                {
                    var client = new RestClient(Server_path);
                    var request = new RestRequest(Method.POST);

                    //파라미터 추가
                    request.AddParameter("localEquipmentId", localEquipmentId);
                    //파라미터 추가 끝.

                    //파라미터 로그 기록
                    if (use_Log_Write == true)
                    {
//                         AJParkLib.AJCommon.CommonClass.SendLog("");
//                         AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 ------ ");
//                         AJParkLib.AJCommon.CommonClass.SendLog(string.Format("localEquipmentId(장비 UID) : {0}", localEquipmentId));
//                         AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 끝 ------ ");
                    }
                    //파라미터 로그 기록 끝.

                    

                    string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드

                    if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                    {
                        temp.code = "181818";
                        //서버에서 응답 자체를 못받았을 경우
                        AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                    }

                    else                                //정보를 받아왔을 때
                    {
                        JObject obj = JObject.Parse(str_response);
                        temp.code = (string)obj["code"];                        //ini에 code 정보 붙임
                        temp.errmsgvariable = (string)obj["errmsgvariable"];    //ini에 errmsgvariable 정보 붙임
                        temp.errmsg = (string)obj["errmsg"];                    //ini에 errmsg 정보 붙임
                        temp.key = (string)obj["key"];                          //ini에 key 정보 붙임

                        //로그 기록
                        if (use_Log_Write == true)
                        {
//                             AJParkLib.AJCommon.CommonClass.SendLog("");
//                             AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 ------ ");
//                             AJParkLib.AJCommon.CommonClass.SendLog(string.Format("code : {0}", temp.code));
//                             AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsgvariable : {0}", temp.errmsgvariable));
//                             AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsg : {0}", temp.errmsg));
//                             AJParkLib.AJCommon.CommonClass.SendLog(string.Format("key : {0}", temp.key));
                        }
                        //로그 기록

                        //로그 기록
                        if (use_Log_Write == true)
                        {
//                             AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 끝 ------ ");       //로그 기록
//                             AJParkLib.AJCommon.CommonClass.SendLog("전체 동작 성공");       //로그 기록
                        }
                        //로그 기록
                    }

                    //return temp;

                    

                }

                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(e.StackTrace.ToString());

                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("동작 실패");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e : {0}", e));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e.StackTrace : {0}", e.StackTrace));
                    }
                    //로그 기록
                    //return temp;
                    temp.code = "1818";
                }

                finally
                {
                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        //AJParkLib.AJCommon.CommonClass.SendLog("모든 파싱 종료");
                    }
                    //로그 기록
                }

                //로그 기록
                if (use_Log_Write == true)
                {
//                     AJParkLib.AJCommon.CommonClass.SendLog("");
//                     AJParkLib.AJCommon.CommonClass.SendLog(" === 16. 장비 온라인 여부 확인 API 끝 === ");
                }
                //로그 기록
                
                return temp;
            }
            #endregion

            #region 18번(프로토콜상 17. 요금 현금 납입 정보 저장 API)
            public static AJ_RESPONSE_cash_price temp_name_18(string carNo, int insertPrice)
            {
                //POST //api/parking/insert/cash/price HTTP/1.1
                //Host: 112.216.153.186:2080
                string Server_path = "http://" + AJ_IP + ":" + AJ_PORT;
                Server_path += "//api/parking/insert/cash/price";

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 17. 요금 현금 납입 정보 저장 API 시작 === ");
                    AJParkLib.AJCommon.CommonClass.SendLog("접속 주소 : " + Server_path);
                }
                //로그 기록

                AJ_RESPONSE_cash_price temp = new AJ_RESPONSE_cash_price();

                try
                {
                    var client = new RestClient(Server_path);
                    var request = new RestRequest(Method.POST);

                    //파라미터 추가
                    request.AddParameter("carNo", carNo);
                    request.AddParameter("insertPrice", insertPrice);
                    //파라미터 추가 끝.

                    //파라미터 로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("");
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 ------ ");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("carNo : {0}", carNo));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("insertPrice : {0}", insertPrice));
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 끝 ------ ");
                    }
                    //파라미터 로그 기록 끝.

                    string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드

                    if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                    {
                        temp.code = "181818";
                        //서버에서 응답 자체를 못받았을 경우
                        AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                    }

                    else                                //정보를 받아왔을 때
                    {
                        JObject obj = JObject.Parse(str_response);
                        temp.code = (string)obj["code"];                        //ini에 code 정보 붙임
                        temp.errmsgvariable = (string)obj["errmsgvariable"];    //ini에 errmsgvariable 정보 붙임
                        temp.errmsg = (string)obj["errmsg"];                    //ini에 errmsg 정보 붙임
                        temp.key = (string)obj["key"];                          //ini에 key 정보 붙임

                        temp.totalInsertPrice = (string)obj["totalInsertPrice"];                          //0.9버전 변경
                        temp.localParkingId = (string)obj["localParkingId"];                          //????

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("code : {0}", temp.code));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsgvariable : {0}", temp.errmsgvariable));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsg : {0}", temp.errmsg));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("key : {0}", temp.key));

                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("totalInsertPrice : {0}", temp.totalInsertPrice));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("localParkingId(문서상 기재X) : {0}", temp.localParkingId));
                        }
                        //로그 기록

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 끝 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog("전체 동작 성공");
                        }
                        //로그 기록

                    }
                    //return temp;
                    
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(e.StackTrace.ToString());

                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("동작 실패");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e : {0}", e));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e.StackTrace : {0}", e.StackTrace));
                    }
                    //로그 기록
                    //return temp;
                    temp.code = "1818";
                }

                finally
                {
                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("모든 파싱 종료");
                    }
                    //로그 기록
                    
                }
                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 17. 요금 현금 납입 정보 저장 API 끝 === ");
                }
                //로그 기록
                
                return temp;
            }
            #endregion

            #region 19번(프로토콜에서는 18. 사전 요금 정산 API)
            //사전 요금 정산
            public static AJ_RESPONSE_prepay_price temp_name_19(int localParkingId, string carNo, DateTime leaveDate, int localEquipmentId)
            {
                
                string Server_path = "http://" + AJ_IP + ":" + AJ_PORT;
                Server_path += "//api/parking/prepay/price";

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 18. 사전 요금 정산 API 시작 === ");
                    AJParkLib.AJCommon.CommonClass.SendLog("접속 주소 : " + Server_path);
                }
                //로그 기록

                AJ_RESPONSE_prepay_price temp = new AJ_RESPONSE_prepay_price();

                try
                {
                    var client = new RestClient(Server_path);
                    var request = new RestRequest(Method.POST);

                    //파라미터 추가
                    request.AddParameter("localParkingId", localParkingId);
                    request.AddParameter("carNo", carNo);

                    //데이트타임 -> 타임스탬프로 변환해서 파라미터에 넣어주기
                    string str_leaveDate;
                    double time_stamp = AJParkLib.AJCommon.Time.DateTime_To_MilliSec(leaveDate);
                    str_leaveDate = Math.Floor(time_stamp).ToString();
                    request.AddParameter("leaveDate", str_leaveDate);
                    //데이트타임 -> 타임스탬프로 변환해서 파라미터에 넣어주기 끝.

                    request.AddParameter("localEquipmentId", localEquipmentId);

                    //파라미터 추가 끝.

                    //파라미터 로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("");
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 ------ ");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("localParkingId : {0}", localParkingId));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("carNo : {0}", carNo));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("leaveDate(DateTime) : {0}", leaveDate));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("leaveDate(TimeStamp) : {0}", str_leaveDate));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("localEquipmentId : {0}", localEquipmentId));
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 끝 ------ ");
                    }

                    string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드

                    if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                    {
                        temp.code = "181818";
                        //서버에서 응답 자체를 못받았을 경우
                        AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                    }

                    else                                //정보를 받아왔을 때
                    {
                        JObject obj = JObject.Parse(str_response);
                        temp.code = (string)obj["code"];                        //code 정보 붙임
                        temp.errmsgvariable = (string)obj["errmsgvariable"];    //errmsgvariable 정보 붙임
                        temp.errmsg = (string)obj["errmsg"];                    //errmsg 정보 붙임
                        temp.key = (string)obj["key"];                          //key 정보 붙임


                        temp.localParkingId = (string)obj["localParkingId"];                //1
                        temp.enterDate = (string)obj["enterDate"];                          //2
                        temp.parkingType = (string)obj["parkingType"];                      //3
                        temp.price = (string)obj["price"];                                  //4
                        temp.discountPrice = (string)obj["discountPrice"];                  //5
                        temp.realPrice = (string)obj["realPrice"];                          //6
                        temp.localCorporateCarId = (string)obj["localCorporateCarId"];      //7
                        temp.prepayPrice = (string)obj["prepayPrice"];                      //8
                        temp.durationSecond = (string)obj["durationSecond"];                //9


                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("code : {0}", temp.code));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsgvariable : {0}", temp.errmsgvariable));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsg : {0}", temp.errmsg));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("key : {0}", temp.key));

                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("localParkingId : {0}", temp.localParkingId));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("enterDate(TimeStamp) : {0}", temp.enterDate));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("enterDate(보기쉽게 DateTime으로 변환) : {0}", AJCommon.Time.MilliSec_To_DateTime(Convert.ToDouble(temp.enterDate))));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("parkingType : {0}", temp.parkingType));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("price : {0}", temp.price));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("discountPrice : {0}", temp.discountPrice));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("realPrice : {0}", temp.realPrice));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("localCorporateCarId : {0}", temp.localCorporateCarId));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("prepayPrice : {0}", temp.prepayPrice));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("durationSecond : {0}", temp.durationSecond));
                        }
                        //로그 기록

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 끝 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog("전체 동작 성공");
                        }
                        //로그 기록

                    }
                    //return temp;

                }

                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(e.StackTrace.ToString());

                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("동작 실패");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e : {0}", e));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e.StackTrace : {0}", e.StackTrace));
                    }
                    //로그 기록
                    //return temp;
                    temp.code = "1818";
                }

                finally
                {
                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("모든 파싱 종료");
                    }
                    //로그 기록
                }
                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 18. 사전 요금 정산 API 끝 === ");
                }
                //로그 기록
                return temp;
            }

            #endregion


            #region 20번(프로토콜에서는 19번)
            //사전 정산 요금 결제
            public static AJ_RESPONSE_prepay_insert temp_name_20(int localEquipmentId, string carNo, DateTime leaveDate, string paymentMethod, int price, int discountPrice, int insertPrice, int notReleasePrice, int releasePrice, string cardNo, string approvalNo, string issuer, string acquirer, string approvedPrice, DateTime approvedDate, int in50000, int in10000, int in5000, int in1000, int in500, int in100, int in50, int in10, int out5000, int out1000, int out500, int out100)
            {
                string Server_path = "http://" + AJ_IP + ":" + AJ_PORT;
                Server_path += "//api/parking/prepay/receipt/insert";

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 19. 사전 정산 요금 결제 API 시작 === ");
                    AJParkLib.AJCommon.CommonClass.SendLog("접속 주소 : " + Server_path);
                }
                //로그 기록

                AJ_RESPONSE_prepay_insert temp = new AJ_RESPONSE_prepay_insert();

                try
                {
                    var client = new RestClient(Server_path);
                    var request = new RestRequest(Method.POST);

                    //파라미터 추가
                    request.AddParameter("localEquipmentId", localEquipmentId);                                     //1
                    request.AddParameter("carNo", carNo);                                                           //2

                    string str_leaveDate;
                    double time_stamp = AJParkLib.AJCommon.Time.DateTime_To_MilliSec(leaveDate);
                    str_leaveDate = Math.Floor(time_stamp).ToString();
                    request.AddParameter("leaveDate", str_leaveDate);                                               //3

                    request.AddParameter("paymentMethod", paymentMethod);                                           //4
                    request.AddParameter("price", price);                                                           //5
                    request.AddParameter("discountPrice", discountPrice);                                           //6
                    request.AddParameter("insertPrice", insertPrice);                                               //7

                    request.AddParameter("notReleasePrice", notReleasePrice);                                       //8
                    request.AddParameter("releasePrice", releasePrice);                                             //9
                    request.AddParameter("cardNo", cardNo);                                                         //10
                    request.AddParameter("approvalNo", approvalNo);                                                 //11
                    request.AddParameter("issuer", issuer);                                                         //12
                    request.AddParameter("acquirer", acquirer);                                                     //13
                    request.AddParameter("approvedPrice", approvedPrice);                                           //14

                    string str_approvedDate;
                    double time_stamp_2 = AJParkLib.AJCommon.Time.DateTime_To_MilliSec(approvedDate);
                    str_approvedDate = Math.Floor(time_stamp_2).ToString();
                    request.AddParameter("approvedDate", str_approvedDate);                                         //15

                    request.AddParameter("in50000", in50000);                                                       //16
                    request.AddParameter("in10000", in10000);                                                       //17
                    request.AddParameter("in5000", in5000);                                                         //18
                    request.AddParameter("in1000", in1000);                                                         //19
                    request.AddParameter("in500", in500);                                                           //20
                    request.AddParameter("in100", in100);                                                           //21
                    request.AddParameter("in50", in50);                                                             //22
                    request.AddParameter("in10", in10);                                                             //23
                    request.AddParameter("out5000", out5000);                                                       //24
                    request.AddParameter("out1000", out1000);                                                       //25
                    request.AddParameter("out500", out500);                                                         //26
                    request.AddParameter("out100", out100);                                                         //27
                    //파라미터 추가 끝.

                    //파라미터 로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("");
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 ------ ");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("localEquipmentId : {0}", localEquipmentId));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("carNo : {0}", carNo));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("leaveDate(DateTime) : {0}", leaveDate));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("leaveDate(TimeStamp) : {0}", str_leaveDate));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("paymentMethod : {0}", paymentMethod));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("price : {0}", price));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("discountPrice : {0}", discountPrice));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("insertPrice : {0}", insertPrice));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("notReleasePrice : {0}", notReleasePrice));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("releasePrice : {0}", releasePrice));

                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("cardNo : {0}", cardNo));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("approvalNo : {0}", approvalNo));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("issuer : {0}", issuer));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("acquirer : {0}", acquirer));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("approvedPrice : {0}", approvedPrice));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("approvedDate(DateTime) : {0}", approvedDate));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("approvedDate(TimeStamp) : {0}", str_approvedDate));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("in50000 : {0}", in50000));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("in10000 : {0}", in10000));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("in5000 : {0}", in5000));

                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("in1000 : {0}", in1000));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("in500 : {0}", in500));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("in100 : {0}", in100));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("in50 : {0}", in50));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("in10 : {0}", in10));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("out5000 : {0}", out5000));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("out1000 : {0}", out1000));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("out500 : {0}", out500));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("out100 : {0}", out100));
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 끝 ------ ");
                    }
                    //파라미터 로그 기록 끝.

                    string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드

                    if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                    {
                        temp.code = "181818";
                        //서버에서 응답 자체를 못받았을 경우
                        AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                    }

                    else                                //정보를 받아왔을 때
                    {
                        JObject obj = JObject.Parse(str_response);
                        temp.code = (string)obj["code"];                        //code 정보 붙임
                        temp.errmsgvariable = (string)obj["errmsgvariable"];    //errmsgvariable 정보 붙임
                        temp.errmsg = (string)obj["errmsg"];                    //errmsg 정보 붙임
                        temp.key = (string)obj["key"];                          //key 정보 붙임

                        temp.orderNumber = (string)obj["orderNumber"];

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("code : {0}", temp.code));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsgvariable : {0}", temp.errmsgvariable));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsg : {0}", temp.errmsg));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("key : {0}", temp.key));

                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("orderNumber : {0}", temp.orderNumber));
                        }
                        //로그 기록

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 끝 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog("전체 동작 성공");
                        }
                        //로그 기록
                    }
                    //return temp;
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(e.StackTrace.ToString());

                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("동작 실패");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e : {0}", e));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e.StackTrace : {0}", e.StackTrace));
                    }
                    //로그 기록

                    //return temp;
                    temp.code = "1818";
                }

                finally
                {
                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("모든 파싱 종료");
                    }
                    //로그 기록
                }

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 19. 사전 정산 요금 결제 API 끝 === ");
                }
                //로그 기록
                
                return temp;

            }
            #endregion


            #region 21번(프로토콜에서는 20번)
            //법인 차량 사전 정산 결제
            public static AJ_RESPONSE_prepay_corporateCar temp_name_21(int localEquipmentId, int localCorporateCarId, string localParkingId, int price, int discountPrice, DateTime leaveDate)
            {
                string Server_path = "http://" + AJ_IP + ":" + AJ_PORT;
                Server_path += "//api/parking/prepay/corporateCar";

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 20. 법인 차량 사전 정산 결제 API 시작 === ");
                    AJParkLib.AJCommon.CommonClass.SendLog("접속 주소 : " + Server_path);
                }
                //로그 기록

                AJ_RESPONSE_prepay_corporateCar temp = new AJ_RESPONSE_prepay_corporateCar();

                try
                {
                    var client = new RestClient(Server_path);
                    var request = new RestRequest(Method.POST);

                    //파라미터 추가
                    request.AddParameter("localEquipmentId", localEquipmentId);                                       // 1. 정산기 UID
                    request.AddParameter("localCorporateCarId", localCorporateCarId);                                 // 2. 법인차량 UID
                    request.AddParameter("localParkingId", localParkingId);                                           // 3. 주차권 종류
                    request.AddParameter("price", price);                                                             // 4. 결제금액
                    request.AddParameter("discountPrice", discountPrice);                                             // 5. 할인금액

                    string str_leaveDate;
                    double time_stamp = AJParkLib.AJCommon.Time.DateTime_To_MilliSec(leaveDate);
                    str_leaveDate = Math.Floor(time_stamp).ToString();
                    request.AddParameter("leaveDate", str_leaveDate);                                                 //6
                    //파라미터 추가 끝.

                    //파라미터 로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("");
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 ------ ");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("localEquipmentId : {0}", localEquipmentId));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("localCorporateCarId : {0}", localCorporateCarId));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("localParkingId : {0}", localParkingId));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("price : {0}", price));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("discountPrice : {0}", discountPrice));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("leaveDate(DateTime) : {0}", leaveDate));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("leaveDate(TimeStamp) : {0}", str_leaveDate));
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 끝 ------ ");
                    }
                    //파라미터 로그 기록 끝.

                    string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드

                    if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                    {
                        temp.code = "181818";
                        //서버에서 응답 자체를 못받았을 경우
                        AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                    }

                    else                                //정보를 받아왔을 때
                    {
                        JObject obj = JObject.Parse(str_response);
                        temp.code = (string)obj["code"];                        //code 정보 붙임
                        temp.errmsgvariable = (string)obj["errmsgvariable"];    //errmsgvariable 정보 붙임
                        temp.errmsg = (string)obj["errmsg"];                    //errmsg 정보 붙임
                        temp.key = (string)obj["key"];                          //key 정보 붙임

                        temp.lackPrice = (string)obj["lackPrice"];              //1
                        temp.price = (string)obj["price"];                      //2
                        temp.discountPrice = (string)obj["discountPrice"];      //3
                        temp.parkingType = (string)obj["parkingType"];          //4

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("code : {0}", temp.code));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsgvariable : {0}", temp.errmsgvariable));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsg : {0}", temp.errmsg));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("key : {0}", temp.key));

                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("lackPrice : {0}", temp.lackPrice));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("price : {0}", temp.price));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("discountPrice : {0}", temp.discountPrice));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("parkingType : {0}", temp.parkingType));
                        }
                        //로그 기록

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 끝 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog("전체 동작 성공");
                        }
                        //로그 기록

                    }
                    //return temp;
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(e.StackTrace.ToString());

                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("동작 실패");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e : {0}", e));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e.StackTrace : {0}", e.StackTrace));
                    }
                    //로그 기록
                    //return temp;
                    temp.code = "1818";
                }

                finally
                {

                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("모든 파싱 종료");
                    }
                    //로그 기록

                }

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 20. 법인 차량 사전 정산 결제 API 끝 === ");
                }
                //로그 기록
                
                return temp;
            }

            #endregion

            #region 22번(프로토콜에서는 21번)
            //사전 정산기 정기권 가격 계산
            public static AJ_RESPONSE_monthlyticket_calcul temp_name_22(string carNo, DateTime fromDate, DateTime toDate)
            {
                string Server_path = "http://" + AJ_IP + ":" + AJ_PORT;
                Server_path += "//api/parking/kiosk/monthlyTicket/calcul";

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 21. 사전 정산기 정기권 가격 계산 API 시작 === ");
                    AJParkLib.AJCommon.CommonClass.SendLog("접속 주소 : " + Server_path);
                }
                //로그 기록

                AJ_RESPONSE_monthlyticket_calcul temp = new AJ_RESPONSE_monthlyticket_calcul();

                try
                {
                    var client = new RestClient(Server_path);
                    var request = new RestRequest(Method.POST);

                    //파라미터 추가
                    request.AddParameter("carNo", carNo);                                               // 1. 차량 번호

                    string str_fromDate = "";
                    str_fromDate = str_fromDate + string.Format("{0}", fromDate.ToString("yyyy"));
                    str_fromDate = str_fromDate + string.Format("{0}", fromDate.ToString("MM"));
                    str_fromDate = str_fromDate + string.Format("{0}", fromDate.ToString("dd"));
                    request.AddParameter("fromDate", str_fromDate);                                     // 2. “yyyyMMdd” 형식의 텍스트 정기권 시작날짜

                    string str_toDate = "";
                    str_toDate = str_toDate + string.Format("{0}", toDate.ToString("yyyy"));
                    str_toDate = str_toDate + string.Format("{0}", toDate.ToString("MM"));
                    str_toDate = str_toDate + string.Format("{0}", toDate.ToString("dd"));
                    request.AddParameter("toDate", str_toDate);                                         // 3. “yyyyMMdd” 형식의 텍스트 정기권 끝 날짜
                    //파라미터 추가 끝.

                    //파라미터 로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("");
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 ------ ");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("carNo : {0}", carNo));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("fromDate(DateTime) : {0}", fromDate));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("fromDate(변환(yyyyMMdd형식) : {0}", str_fromDate));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("toDate(DateTime) : {0}", toDate));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("toDate(변환(yyyyMMdd형식) : {0}", str_toDate));
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 끝 ------ ");
                    }
                    //파라미터 로그 기록 끝.
                    
                    string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드

                    if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                    {
                        temp.code = "181818";
                        //서버에서 응답 자체를 못받았을 경우
                        AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                    }

                    else                                //정보를 받아왔을 때
                    {
                        JObject obj = JObject.Parse(str_response);
                        temp.code = (string)obj["code"];                        //code 정보 붙임
                        temp.errmsgvariable = (string)obj["errmsgvariable"];    //errmsgvariable 정보 붙임
                        temp.errmsg = (string)obj["errmsg"];                    //errmsg 정보 붙임
                        temp.key = (string)obj["key"];                          //key 정보 붙임

                        temp.price = (string)obj["price"];              //1
                        temp.type = (string)obj["type"];                //2
                        temp.fromDate = (string)obj["fromDate"];        //3
                        temp.toDate = (string)obj["toDate"];            //4

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("code : {0}", temp.code));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsgvariable : {0}", temp.errmsgvariable));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsg : {0}", temp.errmsg));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("key : {0}", temp.key));

                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("price : {0}", temp.price));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("type : {0}", temp.type));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("fromDate : {0}", temp.fromDate));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("toDate : {0}", temp.toDate));
                        }
                        //로그 기록

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 끝 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog("전체 동작 성공");
                        }
                        //로그 기록

                    }
                    //return temp;
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(e.StackTrace.ToString());

                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("동작 실패");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e : {0}", e));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e.StackTrace : {0}", e.StackTrace));
                    }
                    //로그 기록
                    //return temp;
                    temp.code = "1818";
                }

                finally
                {

                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("모든 파싱 종료");
                    }
                    //로그 기록

                }

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 21. 사전 정산기 정기권 가격 계산 API 끝 === ");
                }
                //로그 기록
                
                return temp;
            }
            #endregion

            #region 23번(프로토콜에서는 22번)
            //사전 정산기 정기권 그룹 목록
            public static AJ_RESPONSE_monthlyticket_group temp_name_23()
            {
                string Server_path = "http://" + AJ_IP + ":" + AJ_PORT;
                Server_path += "//api/parking/kiosk/monthlyTicketGroup/list";

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 22. 사전 정산기 정기권 그룹 목록 API 시작 === ");
                    AJParkLib.AJCommon.CommonClass.SendLog("접속 주소 : " + Server_path);
                }
                //로그 기록

                AJ_RESPONSE_monthlyticket_group temp = new AJ_RESPONSE_monthlyticket_group();

                try
                {
                    var client = new RestClient(Server_path);
                    var request = new RestRequest(Method.POST);

                    //파라미터 없음..

                    string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드

                    if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                    {
                        temp.code = "181818";
                        //서버에서 응답 자체를 못받았을 경우
                        AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                    }

                    else                                //정보를 받아왔을 때
                    {
                        JObject obj = JObject.Parse(str_response);
                        temp.code = (string)obj["code"];                        //code 정보 붙임
                        temp.errmsgvariable = (string)obj["errmsgvariable"];    //errmsgvariable 정보 붙임
                        temp.errmsg = (string)obj["errmsg"];                    //errmsg 정보 붙임
                        temp.key = (string)obj["key"];                          //key 정보 붙임

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("code : {0}", temp.code));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsgvariable : {0}", temp.errmsgvariable));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsg : {0}", temp.errmsg));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("key : {0}", temp.key));
                        }
                        //로그 기록

                        int monthly_group_list_count = obj["monthlyTicketGroup"].Count();
                        monthlyTicketGroup_list_RESPONSE[] monthlyTicketGroup_list_temp = new monthlyTicketGroup_list_RESPONSE[monthly_group_list_count];


                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 먼쓸리 티켓그룹 리스트 관련 응답 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("monthly_group_list의 개수 : {0}", monthly_group_list_count));
                        }
                        //로그 기록
                       
                        for (int i = 0; i < monthly_group_list_count; i++)
                        {
                            monthlyTicketGroup_list_temp[i].localMonthlyTicketGroupId = (string)obj["monthlyTicketGroup"][i]["localMonthlyTicketGroupId"];
                            monthlyTicketGroup_list_temp[i].name = (string)obj["monthlyTicketGroup"][i]["name"];

                            //로그 기록
                            if (use_Log_Write == true)
                            {
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 monthly_group_list -> localMonthlyTicketGroupId : {1}", i, monthlyTicketGroup_list_temp[i].localMonthlyTicketGroupId));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 monthly_group_list -> name : {1}", i, monthlyTicketGroup_list_temp[i].name));
                                AJParkLib.AJCommon.CommonClass.SendLog("");
                            }
                            //로그 기록

                        }

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 먼쓸리 티켓그룹 리스트 관련 응답 끝 ------ ");
                        }
                        //로그 기록

                        temp.monthlyTicketGroup_list = monthlyTicketGroup_list_temp;

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 끝 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog("전체 동작 성공");
                        }
                        //로그 기록

                    }
                    //return temp;
                    
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(e.StackTrace.ToString());

                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("동작 실패");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e : {0}", e));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e.StackTrace : {0}", e.StackTrace));
                    }
                    //로그 기록
                    //return temp;
                    temp.code = "1818";
                }

                finally
                {

                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("모든 파싱 종료");
                    }
                    //로그 기록

                }

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 22. 사전 정산기 정기권 그룹 목록 API 끝 === ");
                }
                //로그 기록
                
                return temp;

            }
            #endregion

            #region 24번(프로토콜에서는 23번)
            public static AJ_RESPONSE_monthlyticket_insert temp_name_24(string isNew, int localEquipmentId, int localMonthlyTicketGroupId, int localMonthlyTicketInfoId, string carNo, string carName, string userName, string tel, string dong, string ho, string etc, string paymentMethod, int price, DateTime fromDate, DateTime toDate, string rfid, string isUse, int insertPrice, int notReleasePrice, int releasePrice, string cardNo, string approvalNo, string issuer, string acquirer, string approvedPrice, DateTime approvedDate, int in50000, int in10000, int in5000, int in1000, int in500, int in100, int in50, int in10, int out5000, int out1000, int out500, int out100)
            {
                //사전 정산기 정기권 결제
                string Server_path = "http://" + AJ_IP + ":" + AJ_PORT;
                Server_path += "//api/parking/kiosk/monthlyTicket/insert";

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 23. 사전 정산기 정기권 결제 API 시작 === ");
                    AJParkLib.AJCommon.CommonClass.SendLog("접속 주소 : " + Server_path);
                }
                //로그 기록

                AJ_RESPONSE_monthlyticket_insert temp = new AJ_RESPONSE_monthlyticket_insert();

                try
                {
                    var client = new RestClient(Server_path);
                    var request = new RestRequest(Method.POST);

                    //파라미터 추가
                    request.AddParameter("isNew", isNew);                                               // 1. y : 신규 정기권, n : 연장 정기권
                    request.AddParameter("localEquipmentId", localEquipmentId);                         // 2. 사전 정산기 UID
                    request.AddParameter("localMonthlyTicketGroupId", localMonthlyTicketGroupId);       // 3. 정기권 그룹 UID
                    request.AddParameter("localMonthlyTicketInfoId", localMonthlyTicketInfoId);       // 1.5버전 추가!
                    request.AddParameter("carNo", carNo);                                               // 4. 차량 번호
                    request.AddParameter("carName", carName);                                           // 5. 차량 이름
                    request.AddParameter("userName", userName);                                         // 6. 정기권 사용자 이름
                    request.AddParameter("tel", tel);                                                   // 7. 정기권 사용자 전화번호
                    request.AddParameter("dong", dong);                                                 // 8. 동
                    request.AddParameter("ho", ho);                                                     // 9. 호
                    request.AddParameter("etc", etc);                                                   // 10. 비고

                    request.AddParameter("paymentMethod", paymentMethod);                               // 11. 결제방법
                    request.AddParameter("price", price);                                               // 12. 결제금액

                    string str_fromDate = "";
                    str_fromDate = str_fromDate + string.Format("{0}", fromDate.ToString("yyyy"));
                    str_fromDate = str_fromDate + string.Format("{0}", fromDate.ToString("MM"));
                    str_fromDate = str_fromDate + string.Format("{0}", fromDate.ToString("dd"));
                    request.AddParameter("fromDate", str_fromDate);                                     // 13. “yyyyMMdd” 형식의 텍스트 정기권 시작날짜

                    string str_toDate = "";
                    str_toDate = str_toDate + string.Format("{0}", toDate.ToString("yyyy"));
                    str_toDate = str_toDate + string.Format("{0}", toDate.ToString("MM"));
                    str_toDate = str_toDate + string.Format("{0}", toDate.ToString("dd"));
                    request.AddParameter("toDate", str_toDate);                                         // 14. “yyyyMMdd” 형식의 텍스트 정기권 끝 날짜

                    request.AddParameter("rfid", rfid);                                                 // 15. rfid 코드
                    request.AddParameter("isUse", isUse);                                               // 16. y : 사용정기권, n : 사용하지 않는 정기권
                    request.AddParameter("insertPrice", insertPrice);                                   // 17. 현금 입금 금액
                    request.AddParameter("notReleasePrice", notReleasePrice);                           // 18. 미방출 금액
                    request.AddParameter("releasePrice", releasePrice);                                 // 19. 방출 금액
                    request.AddParameter("cardNo", cardNo);                                             // 20. 카드번호

                    request.AddParameter("approvalNo", approvalNo);                                     // 21. 승인번호
                    request.AddParameter("issuer", issuer);                                             // 22. 발급사
                    request.AddParameter("acquirer", acquirer);                                         // 23. 매입사
                    request.AddParameter("approvedPrice", approvedPrice);                               // 24. 승인금액

                    string str_approvedDate;
                    double time_stamp_2 = AJParkLib.AJCommon.Time.DateTime_To_MilliSec(approvedDate);
                    str_approvedDate = Math.Floor(time_stamp_2).ToString();
                    request.AddParameter("approvedDate", str_approvedDate);                             // 25.승인일자

                    request.AddParameter("in50000", in50000);                                           // 26. 입금 오만원권 수
                    request.AddParameter("in10000", in10000);                                           // 27. 입금 일만원권 수
                    request.AddParameter("in5000", in5000);                                             // 28. 입금 오천원권 수
                    request.AddParameter("in1000", in1000);                                             // 29. 입금 일천원권 수
                    request.AddParameter("in500", in500);                                               // 30. 입금 오백원권 수

                    request.AddParameter("in100", in100);                                               // 31. 입금 일백원권 수
                    request.AddParameter("in50", in50);                                                 // 32. 입금 오십원권 수
                    request.AddParameter("in10", in10);                                                 // 33. 입금 일십원권 수
                    request.AddParameter("out5000", out5000);                                           // 34. 출금 오천원권 수
                    request.AddParameter("out1000", out1000);                                           // 35. 출금 일천원권 수
                    request.AddParameter("out500", out500);                                             // 36. 출금 오백원권 수
                    request.AddParameter("out100", out100);                                             // 37. 출금 일백원권 수
                    //파라미터 추가 끝.

                    //파라미터 로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("");
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 ------ ");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("isNew : {0}", isNew));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("localEquipmentId : {0}", localEquipmentId));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("localMonthlyTicketGroupId : {0}", localMonthlyTicketGroupId));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("carNo : {0}", carNo));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("carName : {0}", carName));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("userName : {0}", userName));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("tel : {0}", tel));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("dong : {0}", dong));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("ho : {0}", ho));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("etc : {0}", etc));

                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("paymentMethod : {0}", paymentMethod));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("price : {0}", price));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("fromDate(DateTime) : {0}", fromDate));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("fromDate(형식변환(yyyMMdd) : {0}", str_fromDate));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("toDate(DateTime) : {0}", toDate));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("toDate(형식변환(yyyyMMdd) : {0}", str_toDate));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("rfid : {0}", rfid));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("isUse : {0}", isUse));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("insertPrice : {0}", insertPrice));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("notReleasePrice : {0}", notReleasePrice));

                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("releasePrice : {0}", releasePrice));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("cardNo : {0}", cardNo));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("approvalNo : {0}", approvalNo));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("issuer : {0}", issuer));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("acquirer : {0}", acquirer));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("approvedPrice : {0}", approvedPrice));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("approvedDate(DateTime) : {0}", approvedDate));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("approvedDate(TimeStamp) : {0}", str_approvedDate));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("in50000 : {0}", in50000));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("in10000 : {0}", in10000));

                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("in5000 : {0}", in5000));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("in1000 : {0}", in1000));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("in500 : {0}", in500));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("in100 : {0}", in100));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("in50 : {0}", in50));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("in10 : {0}", in10));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("out5000 : {0}", out5000));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("out1000 : {0}", out1000));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("out500 : {0}", out500));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("out100 : {0}", out100));
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 끝 ------ ");
                    }
                    //파라미터 로그 기록 끝.

                    string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드

                    if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                    {
                        temp.code = "181818";
                        //서버에서 응답 자체를 못받았을 경우
                        AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                    }

                    else                                //정보를 받아왔을 때
                    {
                        JObject obj = JObject.Parse(str_response);
                        temp.code = (string)obj["code"];                        //code 정보 붙임
                        temp.errmsgvariable = (string)obj["errmsgvariable"];    //errmsgvariable 정보 붙임
                        temp.errmsg = (string)obj["errmsg"];                    //errmsg 정보 붙임
                        temp.key = (string)obj["key"];                          //key 정보 붙임

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("code : {0}", temp.code));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsgvariable : {0}", temp.errmsgvariable));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsg : {0}", temp.errmsg));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("key : {0}", temp.key));
                        }
                        //로그 기록

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 끝 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog("전체 동작 성공");
                        }
                        //로그 기록

                    }
                    //return temp;

                }

                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(e.StackTrace.ToString());

                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("동작 실패");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e : {0}", e));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e.StackTrace : {0}", e.StackTrace));
                    }
                    //로그 기록

                    //return temp;
                    temp.code = "1818";
                }

                finally
                {

                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("모든 파싱 종료");
                    }
                    //로그 기록

                }

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 23. 사전 정산기 정기권 결제 API 끝 === ");       //로그 기록
                }
                //로그 기록

                return temp;


            }
            #endregion

            #region 25번 무료구간 LPR 입출차(프로토콜에서는 24번)
            public static AJ_RESPONSE_enter_subLPR temp_name_25(string carNo, DateTime date, int localEquipmentId,string UseNow = "y")
            {
                string Server_path = "http://" + AJ_IP + ":" + AJ_PORT;
                Server_path += "//api/parking/enter/subLpr";

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 24. 무료구간 LPR 입출차 API 시작 === ");
                    AJParkLib.AJCommon.CommonClass.SendLog("접속 주소 : " + Server_path);
                }
                //로그 기록

                AJ_RESPONSE_enter_subLPR temp = new AJ_RESPONSE_enter_subLPR();

                try
                {
                    var client = new RestClient(Server_path);
                    var request = new RestRequest(Method.POST);

                    //파라미터 추가
                    request.AddParameter("carNo", carNo);

                    double double_timestamp_date = AJCommon.Time.DateTime_To_MilliSec(date);
                    string str_timestamp_date = Math.Floor(double_timestamp_date).ToString();
                    request.AddParameter("date", str_timestamp_date);
                    request.AddParameter("localEquipmentId", localEquipmentId);
                    request.AddParameter("UseNow", UseNow);
                    //여기까지

                    //파라미터 로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("");
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 ------ ");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("carNo : {0}", carNo));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("date(datetime) : {0}", date));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("date(timestamp로변환) : {0}", str_timestamp_date));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("localEquipmentId : {0}", localEquipmentId));
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 끝 ------ ");
                    }
                    //파라미터 로그 기록 끝.

                    string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드

                    if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                    {
                        temp.code = "181818";
                        //서버에서 응답 자체를 못받았을 경우
                        AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                    }
                    else
                    {
                        try
                        {
                            JObject obj = JObject.Parse(str_response);
                            temp.code = (string)obj["code"];                        //ini에 code 정보 붙임
                            temp.errmsgvariable = (string)obj["errmsgvariable"];    //ini에 errmsgvariable 정보 붙임
                            temp.errmsg = (string)obj["errmsg"];                    //ini에 errmsg 정보 붙임
                            temp.key = (string)obj["key"];                          //ini에 key 정보 붙임

                            //로그 기록
                            if (use_Log_Write == true)
                            {
                                AJParkLib.AJCommon.CommonClass.SendLog("");
                                AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 ------ ");
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("code : {0}", temp.code));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsgvariable : {0}", temp.errmsgvariable));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsg : {0}", temp.errmsg));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("key : {0}", temp.key));
                                AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 끝 ------ ");
                                AJParkLib.AJCommon.CommonClass.SendLog("전체 동작 성공");
                            }
                            //로그 기록

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                            Console.WriteLine(e.StackTrace.ToString());
                        }

                    }
                    //return temp;
                }

                catch (Exception e)
                {
                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("동작 실패");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e : {0}", e));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e.StackTrace : {0}", e.StackTrace));
                    }
                    //로그 기록
                    temp.code = "1818";
                    //return temp;
                }

                finally
                {
                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("모든 파싱 종료");       //로그 기록
                    }
                    //로그 기록
                }

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 24. 무료구간 LPR 입출차 API 끝 === ");
                }
                //로그 기록

                return temp;

            }
            #endregion

            #region 26번 요금 변경권 적용(프로토콜에서는 25번)
            public static AJ_RESPONSE_priceinfo_change temp_name_26(int localPriceInfoChangeListId, string carNo)
            {
                string Server_path = "http://" + AJ_IP + ":" + AJ_PORT;
                Server_path += "//api/parking/priceInfo/change";

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 25. 요금 변경권 적용 API 시작 === ");
                    AJParkLib.AJCommon.CommonClass.SendLog("접속 주소 : " + Server_path);
                }
                //로그 기록

                AJ_RESPONSE_priceinfo_change temp = new AJ_RESPONSE_priceinfo_change();

                try
                {
                    var client = new RestClient(Server_path);
                    var request = new RestRequest(Method.POST);

                    //파라미터 추가
                    request.AddParameter("localPriceInfoChangeListId", localPriceInfoChangeListId);
                    request.AddParameter("carNo", carNo);
                    //여기까지

                    //파라미터 로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("");
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 ------ ");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("localPriceInfoChangeListId : {0}", localPriceInfoChangeListId));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("carNo : {0}", carNo));
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 끝 ------ ");
                    }
                    //파라미터 로그 기록 끝.

                    string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드

                    if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                    {
                        temp.code = "181818";
                        //서버에서 응답 자체를 못받았을 경우
                        AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                    }
                    else
                    {
                        try
                        {
                            JObject obj = JObject.Parse(str_response);
                            temp.code = (string)obj["code"];                        //ini에 code 정보 붙임
                            temp.errmsgvariable = (string)obj["errmsgvariable"];    //ini에 errmsgvariable 정보 붙임
                            temp.errmsg = (string)obj["errmsg"];                    //ini에 errmsg 정보 붙임
                            temp.key = (string)obj["key"];                          //ini에 key 정보 붙임

                            //로그 기록
                            if (use_Log_Write == true)
                            {
                                AJParkLib.AJCommon.CommonClass.SendLog("");
                                AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 ------ ");
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("code : {0}", temp.code));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsgvariable : {0}", temp.errmsgvariable));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsg : {0}", temp.errmsg));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("key : {0}", temp.key));
                                AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 끝 ------ ");
                                AJParkLib.AJCommon.CommonClass.SendLog("전체 동작 성공");
                            }
                            //로그 기록

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                            Console.WriteLine(e.StackTrace.ToString());
                        }

                    }
                    //return temp;
                }

                catch (Exception e)
                {
                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("동작 실패");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e : {0}", e));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e.StackTrace : {0}", e.StackTrace));
                    }
                    //로그 기록
                    temp.code = "1818";
                    //return temp;
                }

                finally
                {
                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("모든 파싱 종료");       //로그 기록
                    }
                    //로그 기록
                }

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 25. 요금 변경권 적용 API 끝 === ");
                }
                //로그 기록

                return temp;
            }
            #endregion

            #region 27번 사전 정산기 정기권 정보 목록(프로토콜에서는 26번)
            public static AJ_RESPONSE_monthlyTicketInfo_list temp_name_27()
            {
                string Server_path = "http://" + AJ_IP + ":" + AJ_PORT;
                Server_path += "//api/parking/kiosk/monthlyTicketInfo/list";

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 26. 사전정산기 정기권 정보 목록 API 시작 === ");
                    AJParkLib.AJCommon.CommonClass.SendLog("접속 주소 : " + Server_path);
                }
                //로그 기록

                AJ_RESPONSE_monthlyTicketInfo_list temp = new AJ_RESPONSE_monthlyTicketInfo_list();

                try
                {
                    var client = new RestClient(Server_path);
                    var request = new RestRequest(Method.POST);

                    //파라미터 추가
                    
                    //여기까지

                    //파라미터 로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("");
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 ------ ");
                        
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 끝 ------ ");
                    }
                    //파라미터 로그 기록 끝.

                    string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드

                    if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                    {
                        temp.code = "181818";
                        //서버에서 응답 자체를 못받았을 경우
                        AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                    }
                    else
                    {
                        try
                        {
                            JObject obj = JObject.Parse(str_response);
                            temp.code = (string)obj["code"];                        //ini에 code 정보 붙임
                            temp.errmsgvariable = (string)obj["errmsgvariable"];    //ini에 errmsgvariable 정보 붙임
                            temp.errmsg = (string)obj["errmsg"];                    //ini에 errmsg 정보 붙임
                            temp.key = (string)obj["key"];                          //ini에 key 정보 붙임

                            //로그 기록
                            if (use_Log_Write == true)
                            {
                                AJParkLib.AJCommon.CommonClass.SendLog("");
                                AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 ------ ");
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("code : {0}", temp.code));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsgvariable : {0}", temp.errmsgvariable));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsg : {0}", temp.errmsg));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("key : {0}", temp.key));
                                AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 끝 ------ ");
                                AJParkLib.AJCommon.CommonClass.SendLog("전체 동작 성공");
                            }
                            //로그 기록

                            //클로즈 인포 - 인컴낫캐쉬
                            int pre_monthlyTicketInfoList_count = obj["monthlyTicketInfoList"].Count();
                            pre_monthlyTicketInfo_list_RESPONSE[] monthlyTicketInfo_list_temp = new pre_monthlyTicketInfo_list_RESPONSE[pre_monthlyTicketInfoList_count];


                            //로그 기록
                            if (use_Log_Write == true)
                            {
                                AJParkLib.AJCommon.CommonClass.SendLog("");
                                AJParkLib.AJCommon.CommonClass.SendLog(" ------사전정산기 정기권 정보 리스트 시작------ ");
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("사전정산기 정기권 정보 리스트의 개수 : {0}", pre_monthlyTicketInfoList_count));
                            }
                            //로그 기록

                            for (int i = 0; i < pre_monthlyTicketInfoList_count; i++)
                            {
                                monthlyTicketInfo_list_temp[i].localMonthlyTicketInfoId = (string)obj["monthlyTicketInfoList"][i]["localMonthlyTicketInfoId"];
                                monthlyTicketInfo_list_temp[i].monthlyTicketInfoId = (string)obj["monthlyTicketInfoList"][i]["monthlyTicketInfoId"];
                                monthlyTicketInfo_list_temp[i].name = (string)obj["monthlyTicketInfoList"][i]["name"];
                                monthlyTicketInfo_list_temp[i].price = (string)obj["monthlyTicketInfoList"][i]["price"];
                                monthlyTicketInfo_list_temp[i].status = (string)obj["monthlyTicketInfoList"][i]["status"];
                                monthlyTicketInfo_list_temp[i].regDate = (string)obj["monthlyTicketInfoList"][i]["regDate"];
                                monthlyTicketInfo_list_temp[i].updateDate = (string)obj["monthlyTicketInfoList"][i]["updateDate"];
                                monthlyTicketInfo_list_temp[i].parkingLotId = (string)obj["monthlyTicketInfoList"][i]["parkingLotId"];
                                

                                //로그 기록
                                if (use_Log_Write == true)
                                {
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 먼쓸리티켓인포리스트 -> localMonthlyTicketInfoId : {1}", i, monthlyTicketInfo_list_temp[i].localMonthlyTicketInfoId));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 먼쓸리티켓인포리스트 -> monthlyTicketInfoId : {1}", i, monthlyTicketInfo_list_temp[i].monthlyTicketInfoId));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 먼쓸리티켓인포리스트 -> name : {1}", i, monthlyTicketInfo_list_temp[i].name));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 먼쓸리티켓인포리스트 -> price : {1}", i, monthlyTicketInfo_list_temp[i].price));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 먼쓸리티켓인포리스트 -> status : {1}", i, monthlyTicketInfo_list_temp[i].status));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 먼쓸리티켓인포리스트 -> regDate : {1}", i, monthlyTicketInfo_list_temp[i].regDate));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 먼쓸리티켓인포리스트 -> updateDate : {1}", i, monthlyTicketInfo_list_temp[i].updateDate));
                                    AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 먼쓸리티켓인포리스트 -> parkingLotId : {1}", i, monthlyTicketInfo_list_temp[i].parkingLotId));
                                    AJParkLib.AJCommon.CommonClass.SendLog("");
                                }
                                //로그 기록
                            }
                            temp.pre_monthlyTicketInfo_list = monthlyTicketInfo_list_temp;
                            //로그 기록
                            if (use_Log_Write == true)
                            {
                                AJParkLib.AJCommon.CommonClass.SendLog(" ------ 사전정산기 정기권 정보 리스트 끝 ------ ");
                            }
                            //로그 기록


                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                            Console.WriteLine(e.StackTrace.ToString());
                        }

                    }
                    //return temp;
                }

                catch (Exception e)
                {
                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("동작 실패");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e : {0}", e));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e.StackTrace : {0}", e.StackTrace));
                    }
                    //로그 기록
                    temp.code = "1818";
                    //return temp;
                }

                finally
                {
                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("모든 파싱 종료");       //로그 기록
                    }
                    //로그 기록
                }

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 26. 사전정산기 정기권 정보 목록 API 끝 === ");
                }
                //로그 기록

                return temp;

            }
            #endregion

            #region Test
            public static void Test()      //0.7버전 변경
            {
                //var client = new RestClient("http://112.216.153.186:2080//api/parking/enter/insert");
                //string Server_path = "http://" + AJ_IP + ":" + AJ_PORT;
                string Server_path = "http://" + "127.0.0.1" + ":" + "8080";
                Server_path += "//api/parking/AppliedDiscountList";

                AJ_RESPONSE_car_enter temp = new AJ_RESPONSE_car_enter();

                try
                {
                    var client = new RestClient(Server_path);
                    var request = new RestRequest(Method.POST);
                    request.AddParameter("localParkingId", 106);
                    string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드

                    if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                    {
                        temp.code = "181818";
                        //서버에서 응답 자체를 못받았을 경우
                        AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                    }

                    else                                //정보를 받아왔을 때
                    {
                        JObject obj = JObject.Parse(str_response);
                        temp.code = (string)obj["code"];                        //ini에 code 정보 붙임
                        temp.errmsgvariable = (string)obj["errmsgvariable"];    //ini에 errmsgvariable 정보 붙임
                        temp.errmsg = (string)obj["errmsg"];                    //ini에 errmsg 정보 붙임
                        temp.key = (string)obj["key"];                          //ini에 key 정보 붙임

                        Console.WriteLine(temp.code);
                        Console.WriteLine(temp.errmsgvariable);
                        Console.WriteLine(temp.errmsg);
                        Console.WriteLine(temp.key);

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("code : {0}", temp.code));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsgvariable : {0}", temp.errmsgvariable));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("errmsg : {0}", temp.errmsg));
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("key : {0}", temp.key));
                        }
                        //로그 기록

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 응답 목록 끝 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog("전체 동작 성공");
                        }
                        //로그 기록

                    }
                    //return temp;


                }

                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(e.StackTrace.ToString());

                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("동작 실패");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e : {0}", e));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e.StackTrace : {0}", e.StackTrace));
                    }
                    //로그 기록

                    //return temp;
                    temp.code = "1818";
                }
                finally
                {
                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("모든 파싱 종료");
                    }
                    //로그 기록
                }
                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 11. 입차 API 끝 ===");
                }
                //로그 기록

                

            }
            #endregion

            #region 28번 영수증 재발행 정보 조회
            public static AJ_RESPONSE_ReceiptReIssue_list temp_name_28(string ReceiptNumber)
            {
                string Server_path = "http://" + AJ_IP + ":" + AJ_PORT;
                Server_path += "//api/admin/get/receipt/print";

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 28. 영수증 재발행 정보 조회 === ");
                    AJParkLib.AJCommon.CommonClass.SendLog("접속 주소 : " + Server_path);
                }


                AJ_RESPONSE_ReceiptReIssue_list data = new AJ_RESPONSE_ReceiptReIssue_list();


                try
                {
                    var client = new RestClient(Server_path);
                    var request = new RestRequest(Method.POST);

                    //파라미터 추가
                    request.AddParameter("localReceiptInfoId", ReceiptNumber);                                       // 1. 정산기 UID
                    //여기까지

                    //파라미터 로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("");
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 ------ ");

                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 끝 ------ ");
                    }
                    //파라미터 로그 기록 끝.

                    string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드

                    if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                    {
                        data.code = "181818";
                        //서버에서 응답 자체를 못받았을 경우
                        AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                    }
                    else
                    {
                        JObject obj = JObject.Parse(str_response);
                        data.code = (string)obj["code"];                        //ini에 code 정보 붙임
                        data.errmsgvariable = (string)obj["errmsgvariable"];    //ini에 errmsgvariable 정보 붙임
                        data.errmsg = (string)obj["errmsg"];                    //ini에 errmsg 정보 붙임
                        data.key = (string)obj["key"];                          //ini에 key 정보 붙임


                        data.ReceiptData.paymentMethod = string.Format("{0}",obj["item"]["paymentMethod"]);   //결재 방식
                        data.ReceiptData.equipmentName = string.Format("{0}",obj["item"]["equipmentName"]);   //장비명
                        data.ReceiptData.orderNumber = string.Format("{0}",obj["item"]["orderNumber"]);     //영수증 번호

                        data.ReceiptData.carNo = string.Format("{0}",obj["item"]["carNo"]);           //차량번호
                        data.ReceiptData.enterDate = string.Format("{0}",AJCommon.Time.MilliSec_To_DateTime(Convert.ToDouble(obj["item"]["enterDate"])));       //입차시간
                        data.ReceiptData.leaveDate = string.Format("{0}",AJCommon.Time.MilliSec_To_DateTime(Convert.ToDouble(obj["item"]["leaveDate"])));       //출차시간
                        data.ReceiptData.minutes = string.Format("{0}",obj["item"]["minutes"]);         //주차시간
                        data.ReceiptData.parkingPrice = string.Format("{0}",obj["item"]["parkingPrice"]);    //주차요금
                        data.ReceiptData.discountPrice = string.Format("{0}",obj["item"]["discountPrice"]);   //할인요금
                        data.ReceiptData.price = string.Format("{0}",obj["item"]["price"]);           //발생요금
                        data.ReceiptData.insertPrice = string.Format("{0}",obj["item"]["insertPrice"]);     //투입금
                        data.ReceiptData.releasePrice = string.Format("{0}",obj["item"]["releasePrice"]);    //반환금
                        data.ReceiptData.paymentDate = string.Format("{0}",AJCommon.Time.MilliSec_To_DateTime(Convert.ToDouble(obj["item"]["paymentDate"])));     //결재시간

                        data.ReceiptData.cardNo = string.Format("{0}",obj["item"]["cardNo"]);          //카드번호
                        data.ReceiptData.approvalNo = string.Format("{0}",obj["item"]["approvalNo"]);      //승인번호
                        data.ReceiptData.issuer = string.Format("{0}",obj["item"]["issuer"]);          //카드발행사
                        data.ReceiptData.acquirer = string.Format("{0}",obj["item"]["acquirer"]);        //카드매입사
                        data.ReceiptData.approvedPrice = string.Format("{0}",obj["item"]["approvedPrice"]);   //승인금액
                        if (Convert.ToDouble(obj["item"]["approvedDate"]) < 0)
                            data.ReceiptData.approvedDate = "";
                        else
                            data.ReceiptData.approvedDate = string.Format("{0}", AJCommon.Time.MilliSec_To_DateTime(Convert.ToDouble(obj["item"]["approvedDate"])));    //승인날짜
                        
                        #region 임시
                        /*
                        obj["item"]["monthlyTicketId"];
                        obj["item"]["type"]; 
                        obj["item"]["itemName"];
                        obj["item"]["in50000"];
                        obj["item"]["localReceiptInfoId"];
                        obj["item"]["tel"];
                        obj["item"]["coporateName"];
                        obj["item"]["parkingId"];
                        obj["item"]["localCorporateCarId"];
                        obj["item"]["fromDate"];  
                        obj["item"]["in50"];
                        obj["item"]["in10"];
                        obj["item"]["cardId"];
                        obj["item"]["receiptNo"];
                        obj["item"]["in10000"];
                        obj["item"]["updateDate"];
                        obj["item"]["cancelDate"];
                        obj["item"]["cardName"];
                        obj["item"]["corporateCarId"];
                        obj["item"]["in500"];
                        obj["item"]["localParkingId"];
                        obj["item"]["point"];
                        obj["item"]["receiptInfoId"];
                        obj["item"]["in5000"];
                        obj["item"]["in1000"];
                        obj["item"]["in100"];
                        obj["item"]["out1000"];
                        obj["item"]["addr"];
                        obj["item"]["paymentStatus"];
                        obj["item"]["out5000"];
                        obj["item"]["isHp"];
                        obj["item"]["registrationNum"];
                        obj["item"]["toDate"];
                        obj["item"]["parkingLotId"];
                        obj["item"]["isApp"];
                        obj["item"]["userId"];
                        obj["item"]["parkingLotName"];
                        
                        obj["item"]["out100"]; 
                        obj["item"]["userItemId"];
                        obj["item"]["localMonthlyTicketId"];
                        obj["item"]["out500"];
                        
                        obj["item"]["userCardId"];
                        */
                        #endregion
                    }
                }
                catch(Exception ex)
                {
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("동작 실패");
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e : {0}", ex));
                        AJParkLib.AJCommon.CommonClass.SendLog(string.Format("-e.StackTrace : {0}", ex.StackTrace));
                    }
                    //로그 기록
                    data.code = "1818";
                }
                finally
                {
                    //로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("모든 파싱 종료");       //로그 기록
                    }
                }
                return data;
            }
            #endregion

            public static void temp_name_29(int localParkingId, string Carno, DateTime approveDate, int pcmBinNo, int ParkingPrice)
            {
                string Server_path = "http://" + AJ_IP + ":" + AJ_PORT;
                Server_path += "//api/parking/PCMInsert";

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 28. 영수증 재발행 정보 조회 === ");
                    AJParkLib.AJCommon.CommonClass.SendLog("접속 주소 : " + Server_path);
                }


                AJ_RESPONSE_ReceiptReIssue_list data = new AJ_RESPONSE_ReceiptReIssue_list();


                try
                {
                    var client = new RestClient(Server_path);
                    var request = new RestRequest(Method.POST);

                    //파라미터 추가
                    request.AddParameter("localParkingId", localParkingId);
                    request.AddParameter("carNo", Carno);
                    request.AddParameter("approveDate", AJCommon.Time.DateTime_To_MilliSec(approveDate));
                    request.AddParameter("pcmBinNo", pcmBinNo);
                    request.AddParameter("parkingPrice", ParkingPrice);
                    //여기까지

                    //파라미터 로그 기록
                    if (use_Log_Write == true)
                    {
                        AJParkLib.AJCommon.CommonClass.SendLog("");
                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 ------ ");

                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 끝 ------ ");
                    }
                    //파라미터 로그 기록 끝.

                    string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드

                    if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                    {
                        data.code = "181818";
                        //서버에서 응답 자체를 못받았을 경우
                        AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }



            //18년 1월 17일 발렛 파킹 관련 작성
            #region 1. 발렛리스트 조회 API
            public static AJ_Valet_List_Response_List Call_Valet_List_Search(string pos_token, DateTime date, string car_number="")
            {
                //string Server_path = "http://" + AJ_IP + ":" + AJ_PORT;
                //Server_path += "//api/parking/PCMInsert";
                /*
                 * 개발 - http://valet-dev.skoopmedia.co.kr
                   운영 - https://valet.skoopmedia.co.kr
                 * */
                string Server_path = "http://valet-dev.skoopmedia.co.kr/api/v2/valet_pos";

                string client_path = "";
                string param_pos_token = "";        //POS 구분 토큰
                string param_date = "";             //조회할 날짜(입국일) 2017-12-13
                string param_car_number = "";       //차량번호(4자리)1234

                client_path = Server_path + "?";
                param_pos_token = string.Format("pos_token={0}", pos_token);
                param_date = string.Format("&date={0:yyyy}-{1:MM}-{2:dd}", date, date, date);
                param_car_number = string.Format("&car_number={0}", car_number);

                if (car_number.Length > 1)
                {
                    client_path += param_pos_token + param_date + param_car_number;
                }
                else
                {
                    client_path += param_pos_token + param_date;
                }


                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 1. 발렛 리스트 조회 API === ");
                    AJParkLib.AJCommon.CommonClass.SendLog("접속 주소 : " + Server_path);
                }

                //파라미터 로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 ------ ");
                    AJParkLib.AJCommon.CommonClass.SendLog(" : pos_token : " + param_pos_token);
                    AJParkLib.AJCommon.CommonClass.SendLog(" : date : " + param_date);
                    AJParkLib.AJCommon.CommonClass.SendLog(" : car_number : " + param_car_number);
                    AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 끝 ------ ");
                }
                //파라미터 로그 기록 끝.

                AJ_Valet_List_Response_List temp = new AJ_Valet_List_Response_List();

                try
                {
                    var client = new RestClient(client_path);
                    var request = new RestRequest(Method.GET);
                    

                    string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드
                    if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                    {
                        //data.code = "181818";
                        //서버에서 응답 자체를 못받았을 경우
                        AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                    }
                    else
                    {
                        JObject obj = JObject.Parse(str_response);
                        temp.return_code = (string)obj["return_code"];
                        temp.return_message = (string)obj["return_message"];

                        int temp_list_count = obj["result"].Count();              //리스트 카운트
                        temp.aj_valet_list_response_list = new AJ_Valet_List_Response[temp_list_count];
                        AJ_Valet_List_Response[] temp_list = new AJ_Valet_List_Response[temp_list_count];     //카운트 만큼 리스트 생성

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog("");       //로그 기록
                            AJParkLib.AJCommon.CommonClass.SendLog(" === 1. 발렛 리스트 응답 목록 === ");
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("return_code : {0}", temp.return_code));       //로그 기록
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("return_message : {0}", temp.return_message));       //로그 기록
                            AJParkLib.AJCommon.CommonClass.SendLog(" --- 발렛 리스트 조회 결과 리스트--- ");       //로그 기록
                            AJParkLib.AJCommon.CommonClass.SendLog(string.Format("결과 리스트의 개수 : {0}", temp_list_count));       //로그 기록
                        }
                        //로그 기록

                        for (int i = 0; i < temp_list_count; i++)
                        {
                            temp_list[i].seq = (string)obj["result"][i]["seq"];
                            temp_list[i].car_number = (string)obj["result"][i]["car_number"];
                            temp_list[i].phone_number = (string)obj["result"][i]["phone_number"];
                            temp_list[i].enter_date = (string)obj["result"][i]["enter_date"];
                            temp_list[i].name = (string)obj["result"][i]["name"];
                            temp_list[i].discount_type = (string)obj["result"][i]["discount_type"];
                            temp_list[i].parking_line = (string)obj["result"][i]["parking_line"];
                            temp_list[i].payment_status = (string)obj["result"][i]["payment_status"];
                            temp_list[i].price = (string)obj["result"][i]["price"];
                            

                            //로그 기록
                            if (use_Log_Write == true)
                            {
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> seq : {1}", i, temp_list[i].seq));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> car_number : {1}", i, temp_list[i].car_number));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> phone_number : {1}", i, temp_list[i].phone_number));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> enter_date : {1}", i, temp_list[i].enter_date));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> name : {1}", i, temp_list[i].name));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> discount_type : {1}", i, temp_list[i].discount_type));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> parking_line : {1}", i, temp_list[i].parking_line));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> payment_status : {1}", i, temp_list[i].payment_status));
                                AJParkLib.AJCommon.CommonClass.SendLog(string.Format("{0}번 List -> price : {1}", i, temp_list[i].price));
                            }
                            //로그 기록

                            temp.aj_valet_list_response_list[i] = temp_list[i];
                        }

                        //로그 기록
                        if (use_Log_Write == true)
                        {
                            AJParkLib.AJCommon.CommonClass.SendLog(" --- 발렛 리스트 조회 결과 리스트 끝--- ");       //로그 기록
                            AJParkLib.AJCommon.CommonClass.SendLog(" === 1. 발렛 리스트 응답 목록 끝=== ");
                        }



                    }
                    return temp;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return temp;
                }
            }
            #endregion

            #region 2. 발렛 정보 조회 API
            public static AJ_VALET_Information_Response Call_Valet_Informaton(string seq, string pos_token)
            {
                /*
                 * 개발 - http://valet-dev.skoopmedia.co.kr
                   운영 - https://valet.skoopmedia.co.kr
                 * */
                string Server_path = "http://valet-dev.skoopmedia.co.kr/api/v2/valet_pos";

                string client_path = "";
                string param_seq = "";             //발렛 시퀀스
                string param_pos_token = "";        //POS 구분 토큰
                string param_car_number = "";       //차량번호(4자리)1234

                client_path = Server_path + "/";
                param_seq = string.Format("{0}", seq);
                param_pos_token = string.Format("?pos_token={0}", pos_token);

                client_path += param_seq + param_pos_token;


                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 2. 발렛 정보 조회 API === ");
                    AJParkLib.AJCommon.CommonClass.SendLog("접속 주소 : " + Server_path);
                }

                //파라미터 로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 ------ ");
                    AJParkLib.AJCommon.CommonClass.SendLog("seq : " + param_seq);
                    AJParkLib.AJCommon.CommonClass.SendLog("pos_token : " + param_pos_token);
                    AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 끝 ------ ");
                }
                //파라미터 로그 기록 끝.

                AJ_VALET_Information_Response temp = new AJ_VALET_Information_Response();

                try
                {
                    if (seq.Length < 1 || pos_token.Length < 1)
                    {
                        //ERROR
                        temp.return_code = "0";   //에러인지 확인 필요
                        temp.return_message = "PARAM IS EMPTY";   //에러인지 확인 필요
                        return temp;
                    }

                    else
                    {
                        var client = new RestClient(client_path);
                        var request = new RestRequest(Method.GET);


                        string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드
                        if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                        {
                            //data.code = "181818";
                            //서버에서 응답 자체를 못받았을 경우
                            AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                        }
                        else
                        {
                            JObject obj = JObject.Parse(str_response);
                            temp.return_code = (string)obj["return_code"];
                            temp.return_message = (string)obj["return_message"];

                            temp.seq = (string)obj["result"]["seq"];
                            temp.car_number = (string)obj["result"]["car_number"];
                            temp.phone_number = (string)obj["result"]["phone_number"];
                            temp.enter_date = (string)obj["result"]["enter_date"];
                            temp.name = (string)obj["result"]["name"];
                            temp.discount_type = (string)obj["result"]["discount_type"];
                            temp.parking_line = (string)obj["result"]["parking_line"];
                            temp.payment_status = (string)obj["result"]["payment_status"];
                            temp.price = (string)obj["result"]["price"];

                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 2. 발렛 정보 조회 API 응답 목록 ------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog("return_code : " + temp.return_code);
                            AJParkLib.AJCommon.CommonClass.SendLog("return_message : " + temp.return_message);
                            AJParkLib.AJCommon.CommonClass.SendLog("seq : " + temp.seq);
                            AJParkLib.AJCommon.CommonClass.SendLog("car_number : " + temp.car_number);
                            AJParkLib.AJCommon.CommonClass.SendLog("phone_number : " + temp.phone_number);
                            AJParkLib.AJCommon.CommonClass.SendLog("enter_date : " + temp.enter_date);
                            AJParkLib.AJCommon.CommonClass.SendLog("name : " + temp.name);
                            AJParkLib.AJCommon.CommonClass.SendLog("discount_type : " + temp.discount_type);
                            AJParkLib.AJCommon.CommonClass.SendLog("parking_line : " + temp.parking_line);
                            AJParkLib.AJCommon.CommonClass.SendLog("payment_status : " + temp.payment_status);
                            AJParkLib.AJCommon.CommonClass.SendLog("price : " + temp.price);

                            AJParkLib.AJCommon.CommonClass.SendLog(" ------ 2. 발렛 정보 조회 API 응답 목록 끝------ ");
                            AJParkLib.AJCommon.CommonClass.SendLog(" === 2. 발렛 정보 조회 API 끝=== ");
                            
                            
                        }
                        return temp;
                    }

                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return temp;
                }
            }
            #endregion

            #region 3. 발렛 결제/취소 결과 업데이트 API
            public static AJ_VALET_Result_Response Call_Valet_result(string seq, string pos_token, string payment_type, string request_type, string price)
            {
                /*
                 * 개발 - http://valet-dev.skoopmedia.co.kr
                   운영 - https://valet.skoopmedia.co.kr
                 * */
                string Server_path = "http://valet-dev.skoopmedia.co.kr/api/v2/valet_pos";

                string client_path = "";
                string param_seq = "";             //발렛 시퀀스
                string param_pos_token = "";        //POS 구분 토큰

                client_path = Server_path + "/";

                param_seq = string.Format("{0}", seq);
                //param_pos_token = string.Format("?pos_token={0}", pos_token);

                //client_path += param_seq + param_pos_token;
                client_path += param_seq;

                var client = new RestClient(client_path);
                var request = new RestRequest(Method.PUT);

                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddParameter("pos_token", pos_token);
                request.AddParameter("payment_type", payment_type);
                request.AddParameter("request_type", request_type);

                //요금에서 숫자만 추출 시작
                string str_price = price.Replace("원", "");
                str_price = str_price.Replace(",", "");
                str_price = str_price.Replace(" ", "");
                //요금에서 숫자만 추출 끝
                request.AddParameter("price", str_price);

                //로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" === 3. 발렛 결제/취소 결과 업데이트 API === ");
                    AJParkLib.AJCommon.CommonClass.SendLog("접속 주소 : " + Server_path);
                }

                //파라미터 로그 기록
                if (use_Log_Write == true)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("");
                    AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 ------ ");
                    AJParkLib.AJCommon.CommonClass.SendLog("seq : " + seq);
                    AJParkLib.AJCommon.CommonClass.SendLog("pos_token : " + pos_token);
                    AJParkLib.AJCommon.CommonClass.SendLog("payment_type : " + payment_type);
                    AJParkLib.AJCommon.CommonClass.SendLog("request_type : " + request_type);
                    AJParkLib.AJCommon.CommonClass.SendLog("price : " + str_price);
                    AJParkLib.AJCommon.CommonClass.SendLog(" ------ 파라미터 목록 끝 ------ ");
                }
                //파라미터 로그 기록 끝.

                AJ_VALET_Result_Response temp = new AJ_VALET_Result_Response();

                try
                {
                    string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드
                    if (str_response.Length < 1)        //타임아웃으로 서버 접속 자체가 안되었을 때
                    {
                        //data.code = "181818";
                        //서버에서 응답 자체를 못받았을 경우
                        AJParkLib.AJCommon.CommonClass.SendLog("응답 에러 !! 응답 없음. 접속 주소 확인요망");       //로그 기록
                    }
                    else
                    {
                        JObject obj = JObject.Parse(str_response);
                        temp.return_code = (string)obj["return_code"];
                        temp.return_message = (string)obj["return_message"];

                        temp.seq = (string)obj["result"]["seq"];
                        temp.car_number = (string)obj["result"]["car_number"];
                        temp.phone_number = (string)obj["result"]["phone_number"];
                        temp.enter_date = (string)obj["result"]["enter_date"];
                        temp.name = (string)obj["result"]["name"];
                        temp.discount_type = (string)obj["result"]["discount_type"];
                        temp.parking_line = (string)obj["result"]["parking_line"];
                        temp.payment_status = (string)obj["result"]["payment_status"];
                        temp.price = (string)obj["result"]["price"];

                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 3. 발렛 결제/취소 결과 업데이트 API 응답 목록 ------ ");
                        AJParkLib.AJCommon.CommonClass.SendLog("return_code : " + temp.return_code);
                        AJParkLib.AJCommon.CommonClass.SendLog("return_message : " + temp.return_message);
                        AJParkLib.AJCommon.CommonClass.SendLog("seq : " + temp.seq);
                        AJParkLib.AJCommon.CommonClass.SendLog("car_number : " + temp.car_number);
                        AJParkLib.AJCommon.CommonClass.SendLog("phone_number : " + temp.phone_number);
                        AJParkLib.AJCommon.CommonClass.SendLog("enter_date : " + temp.enter_date);
                        AJParkLib.AJCommon.CommonClass.SendLog("name : " + temp.name);
                        AJParkLib.AJCommon.CommonClass.SendLog("discount_type : " + temp.discount_type);
                        AJParkLib.AJCommon.CommonClass.SendLog("parking_line : " + temp.parking_line);
                        AJParkLib.AJCommon.CommonClass.SendLog("payment_status : " + temp.payment_status);
                        AJParkLib.AJCommon.CommonClass.SendLog("price : " + temp.price);

                        AJParkLib.AJCommon.CommonClass.SendLog(" ------ 3. 발렛 결제/취소 결과 업데이트 API 응답 목록 끝------ ");
                        AJParkLib.AJCommon.CommonClass.SendLog(" === 3. 발렛 결제/취소 결과 업데이트 API 끝=== ");
                    }
                    return temp;
                }

                catch (Exception ex)
                {
                    AJParkLib.AJCommon.CommonClass.SendLog("[결제 API ERROR] " + ex.Message);
                    AJParkLib.AJCommon.CommonClass.SendLog("[결제 API ERROR] " + ex.StackTrace);
                    AJParkLib.AJCommon.CommonClass.SendLog("[결제 API ERROR] " + ex.Source);
                    return temp;
                }





                //request.AddParameter("application/x-www-form-urlencoded");
                //request.AddParameter(string.Format("pos_token={0}", pos_token));
                //request.AddParameter(string.Format("payment_type={0}", payment_type));
                //request.AddParameter(string.Format("request_type={0}", request_type));
                //request.AddParameter(string.Format("price={0}", price));

            }
            #endregion


            #region 웹할인관련 테스트 진남 0309
            public static string my_test(string carnumber)
            {
                string serverpath = "http://192.10.60.96:8080/advanced/api/discount/carSearch";

                var client = new RestClient(serverpath);
                var request = new RestRequest(Method.POST);

                request.AddParameter("carNumber", carnumber);


                string str_response = AJ_Web_Access(client, request);       //서버 접속 및 데이터 센드
                return str_response;
            }
            #endregion

        }
    }

}