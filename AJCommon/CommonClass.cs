using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

namespace AJParkLib
{
    namespace AJCommon
    {
        public static class CommonClass
        {
            /// <summary>
            /// 로그 이벤트
            /// </summary>
            public static event EventHandler<LogEventArg> OnLog = delegate { };

            /// <summary>
            /// 로그 이벤트 초기화
            /// </summary>
            public static void init()
            {
                OnLog += CommonClass_OnLog;
            }

            /// <summary>
            /// 로그 기록
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            static void CommonClass_OnLog(object sender, LogEventArg e)
            {

                string ConfigFolderPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Log";

                if (e.FilePath != "")
                    ConfigFolderPath = e.FilePath;


                if (!Directory.Exists(ConfigFolderPath))
                    Directory.CreateDirectory(ConfigFolderPath);

                //AJParkLib.AJLog.LogMessage.LogFileBasePathSetting(ConfigFolderPath);

                //AJParkLib.AJLog.LogMessage.WriteLog(e.strLogMessage);
            }
            /// <summary>
            /// 로그 이벤트 기록
            /// </summary>
            /// <param name="msg">로그 메시지</param>
            public static void SendLog(string msg)
            {
                if (OnLog != null)
                    OnLog.Invoke(null, new LogEventArg(msg));
            }

            /// <summary>
            /// 로그 이벤트 기록
            /// </summary>
            /// <param name="msg">로그 메시지</param>
            public static void SendLog(string msg,string FilePath)
            {
                if (OnLog != null)
                    OnLog.Invoke(null, new LogEventArg(msg, FilePath));
            }
        }

        public static class AJProcess
        {
            /// <summary>
            /// 중복 실행 체크 및 실행
            /// </summary>
            /// <param name="ProcessName">체크할 프로세스명</param>
            /// <returns>true : 중복실행 , False : 실행가능</returns>
            public static bool chkProcess(string ProcessName, System.Windows.Forms.Form Frm)
            {
                bool createdNew = false;
                //System.Threading.Mutex dup = new System.Threading.Mutex(true, ProcessName, out createdNew);
                setUnhandledException();
                if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length >= 2)
                {
                    createdNew = true;
                    AJParkLib.AJCommon.CommonClass.SendLog("프로그램 중복 실행!!");
                }
                else
                {
                    Application.Run(Frm);
                }
                return createdNew;
            }

            private static void setUnhandledException()
            {
                AppDomain currentDomain = AppDomain.CurrentDomain;
                currentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);
            }


            static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
            {
                Exception e = (Exception)args.ExceptionObject;
                AJParkLib.AJCommon.CommonClass.SendLog("처리되지 않은 오류 발생!!");
                AJParkLib.AJCommon.CommonClass.SendLog("MyHandler caught : " + e.Message);
                AJParkLib.AJCommon.CommonClass.SendLog("StackTrace : " + e.StackTrace);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }

        #region Event Args
        /// <summary>
        /// 로그 이벤트 Args
        /// </summary>
        public class LogEventArg : EventArgs
        {
            public string strLogMessage;
            public string FilePath = "";
            /// <summary>
            /// 로그 이벤트 Args
            /// </summary>
            /// <param name="log">로그 내용</param>
            public LogEventArg(string log,string strFilePath = "")
            {
                strLogMessage = log;
                FilePath = strFilePath;
            }
        }


        /// <summary>
        /// 화면 표출 변경 Args
        /// </summary>
        public class DisplayChangeArg : EventArgs
        {
            public UserControl _uc;
            public string strDisplayName = "";
            public object EtcData;

            /// <summary>
            /// 로그 이벤트 Args
            /// </summary>
            /// <param name="log">로그 내용</param>
            public DisplayChangeArg(UserControl uc,string name = "")
            {
                _uc = uc;
                strDisplayName = name;
            }
        }
        #endregion

        #region Time
        public static class Time
        {
            /// <summary>
            /// DateTime의 TotalMilliseconds(13자리, double형)를 입력하면, DateTime으로 변환해서 리턴
            /// </summary>
            /// <param name="milliseconds"></param>
            /// <returns></returns>
            public static DateTime MilliSec_To_DateTime(double milliseconds)
            {
                DateTime during_time = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                during_time = during_time.AddMilliseconds(milliseconds).ToLocalTime();     //a는 파라미터
                return during_time;
            }

            /// <summary>
            /// 특정 DateTime을 TimeStamp(13자리, double)로 변환해서 리턴
            /// </summary>
            /// <param name="temp_date"></param>
            /// <returns></returns>
            public static double DateTime_To_MilliSec(DateTime temp_date)
            {

                double TimeStamp = TimeZoneInfo.ConvertTimeToUtc(temp_date).Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
                return TimeStamp;
            }


            /// <summary>
            /// 현재 시간의 TotalMilliseconds를 반환(double형)
            /// </summary>
            /// <returns></returns>
            public static double TimeStamp_now()
            {
                double TimeStamp_now = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
                return TimeStamp_now;
            }


        }


        #endregion
    }
}