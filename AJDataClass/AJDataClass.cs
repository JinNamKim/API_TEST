using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AJParkLib.AJEnums;
using System.Collections;
namespace AJParkLib
{
    namespace AJDataClass
    {
        #region 요금제 관련 클레스


        public class FeeBlock
        {
            public DateTime StartDateTime;
            public DateTime OriginalEndDateTime;
            public DateTime EndDateTime;
            public int Fee;
        }

        public class BaseFeeBlock
        {
            public int Fee;
            public string BlockCode;
            public int BlockIndex;
            public int time;
            public int iterations;
        }

        public class ParkingFeeSet
        {
            public string Setcode;
            public string BlockCode;
            public string SeasonCode;
            public DateTime StartApplyDateTime;
            public DateTime EndApplyDateTime;
            public int DailyMaxFee;
            public string ExceptDays;
        }
        #endregion

        #region 차량별 결재정보
        public class FeeData : IDisposable
        {
            /// <summary>
            /// 주차 요금
            /// </summary>
            public int Fee;
            /// <summary>
            /// 할인 요금
            /// </summary>
            public int DiscountFee;
            /// <summary>
            /// 투입금
            /// </summary>
            public int InsertFee;

            /// <summary>
            /// 신용카드 번호
            /// </summary>
            public string CardNumber;
            /// <summary>
            /// 신용카드 승인번호
            /// </summary>
            public string ApprovalNumber;
            /// <summary>
            /// 신용카드 발급사
            /// </summary>
            public string issueCompany;
            /// <summary>
            /// 신용카드 매입사
            /// </summary>
            public string purchaseCompany;

            /// <summary>
            /// 신용카드 승인금액
            /// </summary>
            public string ApplovalMoney;

            /// <summary>
            /// 승인일시
            /// </summary>
            public DateTime ApplovalDate;

            public int in50000 = 0;
            public int in10000 = 0;
            public int in5000 = 0;
            public int in1000 = 0;
            public int in500 = 0;
            public int in100 = 0;
            public int in50 = 0;
            public int in10 = 0;

            public int out5000 = 0;
            public int out1000 = 0;
            public int out500 = 0;
            public int out100 = 0;

            public int RemainMoney = 0;

            public int PrevFee;
            /// <summary>
            /// 데이터 메모리 릴리즈
            /// </summary>
            public void Dispose()
            {
                
            }
        }
        #endregion

        #region 잔돈 방출 정보
        public class EjectMoneyInformation
        {
            public int out5000 = 0;
            public int out1000 = 0;
            public int out500 = 0;
            public int out100 = 0;
            public int RemainMoney = 0;
        }
        #endregion
        #region 차량 정보 클래스
        /// <summary>
        /// 차량 정보 클래스
        /// </summary>
        public class VehicleData : IDisposable
        {
            public AutohubVehicleTypeInfo AutoHubinfo;
            //0713
            public string AUTOHUB_CAR_STATE;
            //0713

            //0712
            public string AUTOHUB_ID;
            //0712

            //0710
            public string RFID_TAG;
            //0710

            /// <summary>
            /// 차량번호
            /// </summary>
            public string VehicleNumber;
            /// <summary>
            /// 출차시간
            /// </summary>
            public DateTime ExitPassTime;
            /// <summary>
            /// 출차통행인덱스
            /// </summary>
            public string ExitPassIndex;

            /// <summary>
            /// 차량 처리 종류
            /// </summary>
            public VehicleType type;

            /// <summary>
            /// 차량 종류(경차,일반,대형)
            /// </summary>
            public KindofVehicle Kind;
            /// <summary>
            /// 최근 결재 시간
            /// </summary>
            public DateTime lastPayementDate;

            public ArrayList arUsedDiscountData = null;
            /// <summary>
            /// 입차 조회 정보
            /// </summary>
            public string[] EnteranceData = null;

            /// <summary>
            /// 입차정보 존재 여부
            /// </summary>
            public bool ExistEnteranceData = false;
            /// <summary>
            /// 정기권 차량 정보
            /// </summary>
            public string[] SeasonVehicleInfo = null;

            /// <summary>
            /// 예외차량 조회 정보
            /// </summary>
            public string[] ExceptionInfo = null;

            /// <summary>
            /// 방문차량 조회 정보
            /// </summary>
            public string[] VisitInfo = null;

            /// <summary>
            /// 블랙리스트 조회 정보
            /// </summary>
            public string[] BlackListInfo = null;

            /// <summary>
            /// 사전정산 데이터 저장
            /// </summary>
            public string[] PrepaymentInfo = null;

            /// <summary>
            /// 회차 시간
            /// </summary>
            public int ServiceTime = 0;

            /// <summary>
            /// 예외차량 서비스 시간
            /// </summary>
            public int ExceptionServiceTime = 0;

            /// <summary>
            /// 주차시간
            /// </summary>
            public TimeSpan ParkingTime;

            public FeeData Fee;

            
            /// <summary>
            /// 영수증 번호
            /// </summary>
            public string ReceiptNumber = "";

            public string ImagePath = "";
            public PaymentType payType;
            public string localParkingId;
            /// <summary>
            /// 생성자(데이터 메모리 할당)
            /// </summary>
            public VehicleData()
            {
                AUTOHUB_CAR_STATE = string.Format("");  //0713추가
                AUTOHUB_ID = string.Format("");     //0712 추가
                SeasonVehicleInfo = new string[(int)(AJEnums.DataLength.SeasonVehicleInfo)] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
                EnteranceData = new string[(int)(AJEnums.DataLength.EnteranceData)] { "", "", "", "", "", "" };
                ExceptionInfo = new string[(int)(AJEnums.DataLength.ExceptionData)] { "", "", "", "", "", "" };
                VisitInfo = new string[(int)(AJEnums.DataLength.VisitData)] { "", "" };
                BlackListInfo = new string[(int)(AJEnums.DataLength.BlackList)] { ""};
                PrepaymentInfo = new string[(int)(AJEnums.DataLength.Prepayment)] { "","","","" };
                Fee = new FeeData();
                arUsedDiscountData = new ArrayList();
            }

            /// <summary>
            /// 데이터 메모리 릴리즈
            /// </summary>
            public void Dispose()
            {
                AUTOHUB_CAR_STATE = null;   //0713 추가
                AUTOHUB_ID = null;          //0712 추가
                SeasonVehicleInfo = null;
                EnteranceData = null;
                ExceptionInfo = null;
                VisitInfo = null;
                BlackListInfo = null;
                PrepaymentInfo = null;
                Fee.Dispose();
                Fee = null;
                arUsedDiscountData.Clear();
                arUsedDiscountData = null;
            }
        }

        #endregion

        #region 시재금 출력 정보
        /// <summary>
        /// 시재금 출력 정보
        /// </summary>
        public class PrintVaultMoney
        {
            public int Before5000 = 0;
            public int Before1000 = 0;
            public int Before500 = 0;
            public int Before100 = 0;
            public int current5000 = 0;
            public int current1000 = 0;
            public int current500 = 0;
            public int current100 = 0;
            public int Added5000 = 0;
            public int Added1000 = 0;
            public int Added500 = 0;
            public int Added100 = 0;
        }
        #endregion
    }
}