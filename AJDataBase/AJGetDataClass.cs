using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AJParkLib.AJDataBase;
using System.Data;
using AJParkLib.AJDataClass;
namespace AJParkLib
{
    namespace AJDataBase
    {
        public static class AJGetDataClass
        {
            public static AJ_MSSQL db = new AJ_MSSQL();
            /// <summary>
            /// 정기권 데이터 조회
            /// </summary>
            /// <param name="VehicleInfo"></param>
            public static void GetSeasonVehicleInfo(ref VehicleData VehicleInfo)
            {
                string strQuery = string.Format("select StartDateForSeason, ExpireDateForSeason, LedMessage, UseOrNot, DigitInVehicleNumber,userInfo.UserCode, vehicleInfo.SeasonCode,GroupCode, " +
                                                "list.ParkingFeeLPR, led.EnteranceOrExit, led.UpMessage, led.DownMessage, led.UpTextProperty, led.DownTextProperty, parkingFare.SetCode " +
                                                "from SeasonVehicleInformation as vehicleInfo  " +
                                                "left join SeasonUserInformation as userInfo on vehicleInfo.UserCode = userInfo.UserCode " +
                                                "left join SeasonList as list on vehicleinfo.SeasonCode = list.SeasonCode " +
                                                "left join VehicleSplitCode as splitCode on list.VehicleSplitCode = splitCode.VehicleSplitCode " +
                                                "left join LedListPerVehicle as led on led.VehicleSplitCode = splitCode.VehicleSplitCode " +
                                                "left join ASetOfParkingFare as parkingFare on parkingFare.SeasonCode = vehicleInfo.SeasonCode " +
                                                "where VehicleNumber = '{0}'", VehicleInfo.VehicleNumber);

                DataTable dt = db.GetDataTable(strQuery);

                if (dt != null && dt.Rows.Count > 0)
                {
                    int i;
                    for (i = 0; i < (int)(AJEnums.DataLength.SeasonVehicleInfo); i++)
                    {
                        VehicleInfo.SeasonVehicleInfo[i] = dt.Rows[0][i] == null ? "" : dt.Rows[0][i].ToString();
                    }

                    //요금 부과 정기권인지 판단.
                    VehicleInfo.type = VehicleInfo.SeasonVehicleInfo[(int)AJEnums.SeasonCheckData.SetCode] == "" ? AJEnums.VehicleType.정기권 : AJEnums.VehicleType.일반;

                    //요금 부과 LPR로 입차한 경우 일반차량으로 처리
                    VehicleInfo.type = VehicleInfo.SeasonVehicleInfo[(int)AJEnums.SeasonCheckData.ParkingFeeLPR] == VehicleInfo.EnteranceData[(int)AJEnums.EnteranceData.LocalDeviceCode] ? AJEnums.VehicleType.일반 : AJEnums.VehicleType.정기권;
                }
            }

            /// <summary>
            /// 입차 정보 조회
            /// </summary>
            public static void GetEnteranceData(ref VehicleData VehicleInfo)
            {
                string strQuery = string.Format(" SELECT TOP 100 PassIndex, LocalDeviceCode, PassTime, VehicleNumber, " +
                                                 "Case WHEN KindOfVehicle is null Then '2' else KindOfVehicle end KindOfVehicle, " +
                                                 "VehicleImagePath FROM LocalPassHistory WHERE " +
                                                 "Convert(date, PassTime) >=  Convert(date , Dateadd(d, -7, '{0} 00:00:00' ) ) " +
                                                 "AND Convert(date ,PassTime) <=  Convert(date, '{0} 23:59:59') AND DirectionOfPass = '1' " +
                                                 "AND ExitPassIndex IS NULL AND ExitLocalDeviceCode IS NULL " +
                                                 "AND VehicleNumber = '{1}' ORDER BY PassTime DESC", VehicleInfo.ExitPassTime.ToString("yyyy-MM-dd"), VehicleInfo.VehicleNumber);

                DataTable dt = db.GetDataTable(strQuery);


                if (dt != null && dt.Rows.Count > 0)
                {
                    int i;
                    for (i = 0; i < (int)(AJEnums.DataLength.EnteranceData); i++)
                    {
                        VehicleInfo.EnteranceData[i] = dt.Rows[0][i] == null ? "" : dt.Rows[0][i].ToString();
                    }
                    VehicleInfo.ExistEnteranceData = true;
                }
                else
                {
                    VehicleInfo.ExistEnteranceData = false;
                }
            }

            /// <summary>
            /// 최근 결제 데이터 조회
            /// </summary>
            /// <param name="VehicleInfo"></param>
            public static void GetlastPaymentData(ref VehicleData VehicleInfo)
            {
                string strQuery = string.Format("SELECT TOP 1  B.PaymentDate FROM LocalPassHistory A " +
                                                "LEFT JOIN  PaymentList B ON A.PassIndex = B.PassIndex AND A.LocalDeviceCode = B.LocalDeviceCode   WHERE A.VehicleNumber = '{0}' " +
                                                "AND A.ExitPassIndex IS NOT NULL AND A.DirectionOfPass = '1' " +
                                                "ORDER BY B.PaymentDate DESC  ", VehicleInfo.VehicleNumber);

                DataTable dt = db.GetDataTable(strQuery);

                if (dt != null && dt.Rows.Count > 0)
                {
                    VehicleInfo.lastPayementDate = Convert.ToDateTime(dt.Rows[0][0]);
                }
            }

            /// <summary>
            /// 예외 차량 정보 조회
            /// </summary>
            /// <param name="VehicleInfo"></param>
            public static void GetExceptionVehicle(ref VehicleData VehicleInfo)
            {
                string strQuery = string.Format("select ServiceTime, SDate, EDate, DisCountCode, Company, PubDate from OperatingVehicles where OperatingVehicleNumber = '{0}'", VehicleInfo.VehicleNumber);

                DataTable dt = db.GetDataTable(strQuery);

                if (dt != null && dt.Rows.Count > 0)
                {
                    int i;
                    for (i = 0; i < (int)(AJEnums.DataLength.ExceptionData); i++)
                    {
                        VehicleInfo.ExceptionInfo[i] = dt.Rows[0][i] == null ? "" : dt.Rows[0][i].ToString();
                    }
                    VehicleInfo.type = AJEnums.VehicleType.예외;
                    try
                    {
                        //DB에 저장된 내용이 Int가 아닐 경우, 0으로 처리
                        VehicleInfo.ExceptionServiceTime = Convert.ToInt32(VehicleInfo.ExceptionInfo[(int)(AJEnums.ExceptionData.ServiceTime)]);
                    }
                    catch (System.Exception ex)
                    {
                        VehicleInfo.ExceptionServiceTime = 0;
                    }
                }
                else //예외차량이 아니기 때문에 예외차량 서비스 시간을 적용하지 않음.
                {
                    VehicleInfo.ServiceTime = 0;
                }
            }


            /// <summary>
            /// 방문 차량 정보 조회
            /// </summary>
            /// <param name="VehicleInfo"></param>
            public static void GetVisitVehicle(ref VehicleData VehicleInfo)
            {
                string strQuery = string.Format("select VisitDay, DiscountCode from VisitVehicle where VisitVehicle = '{0}'", VehicleInfo.VehicleNumber);

                DataTable dt = db.GetDataTable(strQuery);

                if (dt != null && dt.Rows.Count > 0)
                {
                    int i;
                    for (i = 0; i < (int)(AJEnums.DataLength.VisitData); i++)
                    {
                        VehicleInfo.VisitInfo[i] = dt.Rows[0][i] == null ? "" : dt.Rows[0][i].ToString();
                    }
                    VehicleInfo.type = AJEnums.VehicleType.방문;
                }
                
            }

            /// <summary>
            /// 사전정산 정보 조회
            /// </summary>
            /// <param name="VehicleInfo"></param>
            public static void GetPrePaymentVehicle(ref VehicleData VehicleInfo)
            {
                string strQuery = string.Format("select ParkingFare, PaymentAmount, PaymentCompleteOrNot,PaymentDate from PaymentList where passIndex = '{0}' and LocalDeviceCode = '{1}'",
                    VehicleInfo.EnteranceData[(int)AJEnums.EnteranceData.PassIndex], VehicleInfo.EnteranceData[(int)AJEnums.EnteranceData.LocalDeviceCode]);

                DataTable dt = db.GetDataTable(strQuery);

                if (dt != null && dt.Rows.Count > 0)
                {
                    int i;
                    for (i = 0; i < (int)(AJEnums.DataLength.Prepayment); i++)
                    {
                        VehicleInfo.PrepaymentInfo[i] = dt.Rows[0][i] == null ? "" : dt.Rows[0][i].ToString();
                    }

                    if(Convert.ToInt32(VehicleInfo.PrepaymentInfo[(int)(AJEnums.PrepaymentInfo.PaymentCompleteOrNot)]) == 2)
                        VehicleInfo.type = AJEnums.VehicleType.사전정산;
                }

            }

            /// <summary>
            /// 사전 정산 후, 서비스 시간 조회
            /// </summary>
            /// <returns>서비스 시간</returns>
            public static int GetPrePaymentServiceTime()
            {
                string strQuery = string.Format("select AppliedTimeOfPrecalculation from ParkManagement");

                return db.GetScala(strQuery);
            }
            /// <summary>
            /// 블랙리스트 차량 정보 조회
            /// </summary>
            /// <param name="VehicleInfo"></param>
            public static void GetBlackListVehicle(ref VehicleData VehicleInfo)
            {
                string strQuery = string.Format("select VehicleNumber from BlackList where VehicleNumber = '{0}' and '{1}' between BeginDate and EndDate", VehicleInfo.VehicleNumber, VehicleInfo.ExitPassTime.ToString("yyyy-MM-dd HH:mm:ss"));

                DataTable dt = db.GetDataTable(strQuery);

                if (dt != null && dt.Rows.Count > 0)
                {
                    int i;
                    for (i = 0; i < (int)(AJEnums.DataLength.BlackList); i++)
                    {
                        VehicleInfo.VisitInfo[i] = dt.Rows[0][i] == null ? "" : dt.Rows[0][i].ToString();
                    }
                    VehicleInfo.type = AJEnums.VehicleType.블랙리스트;
                }

            }

            /// <summary>
            /// 회차 시간 조회
            /// </summary>
            /// <param name="VehicleInfo"></param>
            public static void GetServiceTime(ref VehicleData VehicleInfo)
            {
                string strQuery = string.Format("select case when DATEPART(dw,'{0}') between  2 and 6 then TimeOfFreeInDayOfWeek else TimeOfFreeinWeekend end as ServiceTime from ParkManagement", VehicleInfo.ExitPassTime.ToString("yyyy-MM-dd 00:00:00"));

                DataTable dt = db.GetDataTable(strQuery);

                if (dt != null && dt.Rows.Count > 0)
                {
                    VehicleInfo.ServiceTime += Convert.ToInt32(dt.Rows[0][0]);
                }
            }

            /// <summary>
            /// 기존 할인 사용 내역 조회
            /// </summary>
            /// <param name="VehicleInfo"></param>
            public static void GetDiscountHistory(ref VehicleData VehicleInfo)
            {
                string strQuery = string.Format("select DiscountCode from from DiscountAppliedHistory where passIndex = '{0}' and LocalDeviceCode = '{1}'",
                    VehicleInfo.EnteranceData[(int)AJEnums.EnteranceData.PassIndex], VehicleInfo.EnteranceData[(int)AJEnums.EnteranceData.LocalDeviceCode]);

                DataTable dt = db.GetDataTable(strQuery);

                if (dt != null && dt.Rows.Count > 0)
                {
                    int i;
                    for (i = 0; i < dt.Rows.Count; i++)
                    {
                        if(dt.Rows[i][(int)AJEnums.DiscountHistoryInfo.DiscountCode] != null)
                        {
                            VehicleInfo.arUsedDiscountData.Add(dt.Rows[i][(int)AJEnums.DiscountHistoryInfo.DiscountCode].ToString());
                        }
                    }
                }
            }


            /// <summary>
            /// 할인권 리스트를 가져온다
            /// </summary>
            /// <returns></returns>
            public static DataTable GetDiscountList()
            {
                string strQuery = string.Format("select DiscountCode,DiscountName from DiscountCodeList order by DiscountCode");
                return db.GetDataTable(strQuery);
            }
        }
    }
}