using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using RestSharp;
//using Newtonsoft.Json.Linq;

namespace AJParkLib
{
    namespace AJWebdataClass
    {
        public class REPONSE_Default
        {
            public string code;                     //0 : 성공 / !0 : 실패(에러)
            public string errmsgvariable;           //NULL
            public string errmsg;                   //에러메시지
            public string key;                      //에러메시지 키
        }

        #region 1. 초기 장비 세팅 정보
        /// <summary>
        /// 1. 초기 장비 세팅 정보 응답
        /// </summary>
        //public struct AJ_initializedInfo_RESPONSE
        //public struct AJ_RESPONSE_initializedInfo
        public class AJ_RESPONSE_initializedInfo : REPONSE_Default
        {
            //public string code;
            //public string errmsgvariable;
            //public string errmsg;
            //public string key;
            public lprInfo_RESPONSE[] lprInfo_List;
            public kioskInfo_RESPONSE[] kioskInfo_List;
        }

        public struct lprInfo_RESPONSE
        {
            public string localEquipmentId;             //장비 UID
            public string type;                         //장비 종류
            public string name;                         //장비 이름
            public string equipmentStatus;              //장비 상태
            public string equipmentNo;                  //장비 번호
            public string equipmentIp;                  //장비IP
            public string equipmentPort;                //장비 PORT
            public string displayIp;                    //전광판 IP
            public string displayPort;                  //전광판 PORT
            public string barrierPort;                  //차단기 PORT
            public string location;                     //위치
            public string barrierControlOption;         //차단기제어옵션
            public string carInfoSave;                  //차량정보저장옵션
            public string localCkId;                    //연결된Kiosk(type)UID   17년1월11일 추가VER 0.6
        }

        public struct kioskInfo_RESPONSE
        {
            public string localEquipmentId;             //장비 UID
            public string type;                         //장비 종류
            public string name;                         //장비 이름
            public string equipmentStatus;              //장비 상태
            public string equipmentNo;                  //장비번호
            public string powerIp;                      //1.0 버전 추가, 전원제어보드 IP
            public string powerPort;                    //1.0 버전 추가, 전원제어보드 PORT
            public string equipmentIp;                  //장비IP
            public string equipmentPort;                //장비PORT
            public string displayIp;                    //전광판IP
            public string displayPort;                  //전광판PORT
            public string barrierPort;                  //차단기PORT
            public string location;                     //위치
            public string dvrIp;                        //DVR정보 IP
            public string dvrPort;                      //DVR 정보 PORT
            public string won500Price;                  //동전방출기(500원)정보
            public string won100Price;                  //동전방출기(100원)정보
            public kioskChild_RESPONSE[] kioskchildList;//kiosk 연결된 장비 리스트
        }

        public struct kioskChild_RESPONSE
        {
            public string localEquipmentKioskId;        //장비 UID(Child 장비)
            public string name;                         //장비 이름
            public string port;                         //장비PORT
            public string localEquipmentId;             //부모장비 UID
            public string equipmentKioskStatus;         //장비상태
            public string isUse;                        //포트 사용 유무
        }

        public class AJ_PARAMS__initializedInfo
        {
            public int localEquipmentId;
        }

        /*
        public class initializedInfo
        {
            public initializedInfo_RESPONSE my_initializedInfo;
        }
        */

#endregion

        #region 2. 마감정보
        public class AJ_RESPONSE_CloseInfo : REPONSE_Default
        {
            //public string code;
            //public string errmsgvariable;
            //public string errmsg;
            //public string key;
            //public closeinfo_list_RESPONSE[] CloseInfo_list;      //리스트가 아닌것 같아서 주석
            public closeinfo_list_RESPONSE CloseInfo_list;
        }

        public struct closeinfo_list_RESPONSE    
        {
            public string parkingLotName;
            public string companyName;
            public string companyNumber;
            public string parkingLotAddr;
            public string fromTimestamp;
            public string toTimestamp;
            public string equipmentNo;          //0.7버전에서 삭제 ?
            public closeinfo_list_RESPONSE_list_incomeCash[] incomeCash_list;
            public closeinfo_list_RESPONSE_list_incomeNotCash[] incomeNotCash_list;
            public closeinfo_list_RESPONSE_list_incomeTotal[] incomeTotal_list;
            public closeinfo_list_RESPONSE_list_discountInfoDetail[] discountInfoDetail_list;
            public closeinfo_list_RESPONSE_list_discountInfoTotal[] discountInfoTotal_list;
            public closeinfo_list_RESPONSE_list_retentionCashInfo[] retentionCashInfo_list;
            public closeinfo_list_RESPONSE_list_inputCashInfo inputCashInfo_list;
            public closeinfo_list_RESPONSE_list_outputCashInfo outputCashInfo_list;
            public closeinfo_list_RESPONSE_list_parkingCarInfo parkingCarInfo_list;
            public closeinfo_list_RESPONSE_list_noPayParking noPayParking_list;
            public string manualOpenCount;
            public string printDate;

        }

        public struct closeinfo_list_RESPONSE_list_incomeCash
        {
            public string releaseCount;
            public string releasePrice;
            public string insertCount;
            public string insertPrice;
        }

        public struct closeinfo_list_RESPONSE_list_incomeNotCash
        {
            public string price;
            public string count;
            public string paymentMethod;
        }

        public struct closeinfo_list_RESPONSE_list_incomeTotal
        {
            public string total;
            public string count;
            public string price;
        }

        public struct closeinfo_list_RESPONSE_list_discountInfoDetail
        {
            public string name;
            public string count;
            public string discountPrice;
        }

        public struct closeinfo_list_RESPONSE_list_discountInfoTotal
        {
            public string total;
            public string count;
            public string discountPrice;
        }

        public struct closeinfo_list_RESPONSE_list_retentionCashInfo
        {
            public string localEquipmentId;
            public string init5000;
            public string init1000;
            public string init500;
            public string init100;
            public string totalInit;
            public string current5000;
            public string current1000;
            public string current500;
            public string current100;
            public string totalCurrent;
        }

        public struct closeinfo_list_RESPONSE_list_inputCashInfo
        {
            public string count50000;
            public string count10000;
            public string count5000;
            public string count1000;
            public string count500;
            public string count100;
            public string count50;
            public string count10;
            public string totalCount;
            public string sum50000;
            public string sum10000;
            public string sum5000;
            public string sum1000;
            public string sum500;
            public string sum100;
            public string sum50;
            public string sum10;
            public string totalSum;
        }

        public struct closeinfo_list_RESPONSE_list_outputCashInfo
        {
            public string count5000;
            public string count1000;
            public string count500;
            public string count100;
            public string totalCount;
            public string sum5000;
            public string sum1000;
            public string sum500;
            public string sum100;
            public string totalSum;
        }

        public struct closeinfo_list_RESPONSE_list_parkingCarInfo
        {
            public string monthlyCarCount;
            public string priceCarCount;
            public string recurrenceCarCount;
        }

        public struct closeinfo_list_RESPONSE_list_noPayParking
        {
            public string name;
            public string sumCount;
            public string sumPrice;
        }



        public class AJ_PARAMS_CloseInfo
        {
            public int localEquipmentId;
            public double fromTimestamp;
            public double toTimestamp;
        }


        #endregion

        #region 3. 쿼리 결과 전송
        public class AJ_RESPONSE_query : REPONSE_Default
        {
            //public string code;
            //public string errmsgvariable;
            //public string errmsg;
            //public string key;
            public result_list_RESPONSE[] result_list;
        }

        public struct result_list_RESPONSE
        {
            public string isUse;
            public string password;
            public string loginId;
            public string phone;
            public string adminId;
            public string name;
            public string regDate;
            public string isSuperAdmin;
            public string email;
            public string status;
        }
        #endregion

        #region 4-1. 시재금(장비 -> LMS)
        /// <summary>
        /// 4-1. 시재금(장비 -> LMS) 파라미터
        /// </summary>
        //public struct vaultCashInfo_POST_PARAMS
        public class AJ_PARAMS_vaultCashInfo_POST
        {
            public int localEquipmentId;
            public int won5000;
            public int won1000;
            public int won500;
            public int won100;
            public string isForceChange;
        }

        /// <summary>
        /// 4-1. 시재금(장비 -> LMS) 응답
        /// </summary>
        //public struct AJ_vaultCashInfo_POST_RESPONSE
        public class AJ_RESPONSE_vaultCashInfo_POST : REPONSE_Default
        {
            //public string code;
            //public string errmsgvariable;
            //public string errmsg;
            //public string key;
        }
        #endregion

        #region 4-2. 시재금(LMS -> 장비)
        /// <summary>
        /// 4-2. 시재금(LMS -> 장비) 파라미터
        /// </summary>
        //public struct vaultCashInfo_GET_PARAMS
        public class AJ_PARAMS_vaultCashInfo_GET
        {
            public int localEquipmentId;
        }

        /// <summary>
        /// 4-2. 시재금(LMS -> 장비) 응답
        /// </summary>
        //public struct AJ_vaultCashInfo_GET_RESPONSE
        public class AJ_RESPONSE_vaultCashInfo_GET : REPONSE_Default
        {
            //public string code;
            //public string errmsgvariable;
            //public string errmsg;
            //public string key;
            //public vaultCashInfo[] vaultCashInfo_list;
            public vaultCashInfo_list_RESPONSE vaultCashInfo_list;      //기기 하나에 돈통 하나니까 배열로 안가도 될듯 고민

        }

        public class vaultCashInfo_list_RESPONSE
        {
            public string won5000;                          //5,000원 갯수
            public string won1000;                          //1,000원 갯수
            public string won500;                           //500원 갯수
            public string won100;                           //100원 갯수
            public string set5000;                           //셋팅된 5,000원 갯수    0.7버전에 추가
            public string set1000;                           //셋팅된 1,000원 갯수    0.7버전에 추가
            public string set500;                           //셋팅된 500원 갯수    0.7버전에 추가
            public string set100;                           //셋팅된 100원 갯수    0.7버전에 추가

        }
        #endregion

        #region 5. 정기권 정보 전달
        /// <summary>
        /// 5. 정기권 정보 응답
        /// </summary>
        //public struct AJ_monthlyTicketInfo_RESPONSE
        public class AJ_RESPONSE_monthlyTicketInfo : REPONSE_Default
        {
            //public string code;
            //public string errmsgvariable;
            //public string errmsg;
            //public string key;
            public monthlyTicketInfo_list_RESPONSE[] monthlyTicketInfo_list;
        }

        public struct monthlyTicketInfo_list_RESPONSE
        {
            public string localMonthlyTicketId;             //정기권 UID
            public string fromDate;                         //정기권 시작일
            public string toDate;                           //정기권 종료일
            public string carNo;                            //정기권 사용자 차량 번호
            public string price;                            //정기권 가격
            public string discountPrice;                    //할인 금액
            public string point;                            //정기권 결제시 사용된 포인트
            public string paymentMethod;                    //정기권 결제 방법
            public string status;                           //정기권 상태
            public string isUse;                            //사용 여부
        }

        #endregion

        #region 6. 정산기 오류 내역
        /// <summary>
        /// 6. 정산기 오류 내역 파라미터
        /// </summary>
        //public struct error_PARAMS
        public class AJ_PARAMS_error
        {
            public int localEquipmentId;
            public string equipmentCode;
            public string errorCode;
        }

        /// <summary>
        /// 6. 정산기 오류 내역 응답
        /// </summary>
        //public struct AJ_error_RESPONSE
        public class AJ_RESPONSE_error : REPONSE_Default
        {
            //public string code;
            //public string errmsgvariable;
            //public string errmsg;
            //public string key;
        }
        #endregion

        #region 7. 미방출 금액 전달
        /// <summary>
        /// 7. 미방출 금액 전달 파라미터
        /// </summary>
        //public struct nopay_PARAMS
        public class AJ_PARAMS_nopay
        {
            public string carNo;
            public int price;
            public int won50000;
            public int won10000;
            public int won5000;
            public int won1000;
            public int won500;
            public int won100;
            public int won50;
            public int won10;
        }

        /// <summary>
        /// 7. 미방출 금액 전달 응답
        /// </summary>
        //public struct AJ_nopay_RESPONSE
        public class AJ_RESPONSE_nopay : REPONSE_Default
        {
            //public string code;
            //public string errmsgvariable;
            //public string errmsg;
            //public string key;
        }
        #endregion

        #region 8. POLLING DATA 성공 여부
        /// <summary>
        /// 8. POLLING DATA 성공 여부 파라미터
        /// </summary>
        //public struct result_PARAMS
        public class AJ_PARAMS_result
        {
            public int localLogId;
            public string isSuccess;
        }

        /// <summary>
        /// 8. POLLING DATA 성공 여부 응답
        /// </summary>
        //public struct AJ_result_RESPONSE
        public class AJ_RESPONSE_result : REPONSE_Default
        {
            //public string code;
            //public string errmsgvariable;
            //public string errmsg;
            //public string key;
        }
        #endregion

        #region 9. POLLING DATA
        /// <summary>
        /// 9. POLLING DATA 파라미터
        /// </summary>
        //public struct log_PARAMS
        public class AJ_PARAMS_log
        {
            public int localEquipmentId;
        }

        /// <summary>
        /// 9. POLLING DATA 응답
        /// </summary>
        //public struct AJ_log_RESPONSE
        public class AJ_RESPONSE_log : REPONSE_Default
        {
            //public string code;
            //public string errmsgvariable;
            //public string errmsg;
            //public string key;
            public log_list_RESPONSE[] log_list;
        }
        public struct log_list_RESPONSE
        {
            public string localLogId;               //폴링 로그 UID
            public string localEquipmentId;         //장비 UID
            public string type;                     //폴링 로그 타입
            public string content;                  //폴링 로그 내용
            public string regDate;                  //등록 일시
            public string isSuccess;                //폴링 성공 여부
            public string time;                     //수동 출차시 내려가는 출차 일시, 0.9V에 추가
            public string carNo;                    //수동 출차시 내려가는 차량 번호, 0.9V에 추가
            public string localReceiptInfoId;       //영수증 재발행시 내려가는 영수증 번호, 0.9V에 추가
            public string localEquipmentKioskId;    //정산기 결제 관련 장비 재시작시 내려가는 장비 UID, 0.9V에 추가
            public string localDiscountInfoId;       //1월26일 추가, 
            public string SendPrice;
            //public string equipment;                //장비 정보, 0.9V에 추가....리스트, 장비정보인데 1번 참조라기 보다 훨씬 많다;;헐
            public log_list_RESPONSE_list_equipment equipment_list;

            //public string equipmentKiosk;           //정산기 결제 관련 장비 정보, 0.9V에 추가
            public log_list_RESPONSE_list_equipmentKiosk equipmentKiosk_list;

            //public string parkingLot;               //영수증 재발행, 0.9V에 추가
            public log_list_RESPONSE_list_parkingLot parkingLot_list;

            //public string receipt;                  //영수증 재발행, 0.9V에 추가
            public log_list_RESPONSE_list_receipt receipt_list;

        }

        public struct log_list_RESPONSE_list_equipment
        {
            public string localEquipmentId;                       //1
            public string equipmentId;                            //2
            public string parkingLotId;                          //3
            public string type;                          //4
            public string name;     //5
            public string equipmentStatus;     //6
            public string equipmentStatusUpdDate;     //7
            public string status;     //8
            public string regDate;     //9
            public string equipmentNo;     //10

            public string equipmentIp;     //11
            public string equipmentPort;     //12
            public string displayIp;     //13
            public string displayPort;     //14
            public string barrierPort;     //15
            public string location;     //16
            public string barrierControlOption;     //17
            public string carInfoSave;     //18
            public string dvrIp;     //19
            public string dvrPort;     //20

            public string won500Price;     //21
            public string won100Price;     //22
            public string won5000;     //23
            public string won1000;     //24
            public string won500;     //25
            public string won100;     //26
            public string updateDate;     //27
            public string set5000;     //28
            public string set1000;     //29
            public string set500;     //30

            public string set100;     //31
            public string ckId;     //32
            public string localCkId;     //33
        }

        public struct log_list_RESPONSE_list_equipmentKiosk
        {
            public string localEquipmentKioskId;
            public string equipmentKioskId;
            public string name;
            public string port;
            public string status;
            public string equipmentId;
            public string localEquipmentId;
            public string equipmentKioskStatus;
            public string updateDate;
            public string isUse;        //10

        }

        public struct log_list_RESPONSE_list_parkingLot
        {
            public string name;
            public string companyName;
            public string companyNumber;
            public string ownerName;
            public string address;
            public string tel;
        }

        public struct log_list_RESPONSE_list_receipt
        {
            public string localReceiptInfoId;       //1
            public string carNo;                    //2
            public string enterDate;                //3
            public string leaveDate;                //4
            public string minutes;                  //5
            public string parkingPrice;             //6
            public string discountPrice;            //7
            public string price;                    //8
            public string insertPrice;              //9
            public string releasePrice;             //10

            public string orderNumber;              //11
            public string paymentDate;              //12
            public string equipmentName;            //13
            public string paymentMethod;            //14
            public string cardNo;                   //15
            public string approvalNo;               //16
            public string issuer;                   //17
            public string acquirer;                 //18
            public string approvedPrice;            //19
            public string approvedDate;             //20
            
        }

        #endregion

        #region 10. VIP 정보(0.8버전에서 빠짐)
        /// <summary>
        /// 10. VIP 정보 응답
        /// </summary>
        //public struct AJ_vipInfo_RESPONSE
        public class AJ_RESPONSE_vipInfo : REPONSE_Default
        {
            //public string code;
            //public string errmsgvariable;
            //public string errmsg;
            //public string key;
            public vipinfo_list_RESPONSE[] vipinfo_list;
        }

        public struct vipinfo_list_RESPONSE
        {
            public string carNo;                //VIP 차량 번호
            public string localVipId;           //VIP 정보 UID
        }
        #endregion

        #region 11. 미인식 차량 요금 정산
        /// <summary>
        /// 11. 미인식 차량 요금 정산 파라미터
        /// </summary>
        //public struct unkown_price_PARAMS
        public class AJ_PARAMS_unkown_price
        {
            //public int localParkingId;    //0.8버전에서 빠짐
            public string carNo;
            //public string leaveDate;      //0.8버전에서 빠짐
        }

        /// <summary>
        /// 11. 미인식 차량 요금 정산 응답
        /// </summary>
        //public struct AJ_unkown_price_RESPONSE
        public class AJ_RESPONSE_unkown_price : REPONSE_Default
        {
            //public string code;
            //public string errmsgvariable;
            //public string errmsg;
            //public string key;
            //public string parkingType;              //주차권 종류  0.7에서 변경
            //public string realPrice;                //결제 금액  0.7에서 변경
            //public string price;                    //정산 금액  0.7에서 변경
            //public string discountPrice;            //할인 금액  0.7에서 변경
            public unknownParkingList_RESPONSE[] unknownParkingList;
        }

        public struct unknownParkingList_RESPONSE
        {
            public string localParkingId;            //주차권 아이디  0.7에서 변경
            public string url;                       //입차 이미지 url  0.7에서 변경
            public string width;                     //이미지 가로 크기  0.7에서 변경
            public string height;                    //이미지 세로 크기  0.7에서 변경
            public string enterDate;                    //TIMESTAMP, 입차시간  0.8버전에서 추가
            public string carNo;                    //전체 차량번호  0.8버전에서 추가
        }

        #endregion

        #region 12. 차량 입차
        /// <summary>
        /// 12. 차량 입차 파라미터
        /// </summary>
        //public struct car_enter_PARAMS
        public class AJ_PARAMS_car_enter
        {
            public string carNo;
            public string image;                //이미지 파일 경로여야 한다. 호출하는 파라미터는 실제 string이 아니라 file!!!!!
            public DateTime enterDate;
            public int localEquipmentId;
            public string isSmallCar;           //경차 여부
        }

        /// <summary>
        /// 12. 차량 입차 응답
        /// </summary>
        //public struct AJ_car_enter_RESPONSE
        public class AJ_RESPONSE_car_enter : REPONSE_Default
        {
            //public string code;
            //public string errmsgvariable;
            //public string errmsg;
            //public string key;
        }
        #endregion

        #region 13. 차량 출차
        /// <summary>
        /// 13. 차량 출차 파라미터
        /// </summary>
        //public struct car_out_PARAMS
        public class AJ_PARAMS_car_out
        {
            public int localEquipmentId;
            public string image;            //출차 사진 정보, 0.8버전에서 추가
            public string carNo;
            public DateTime leaveDate;
            public string paymentMethod;
            public int price;
            public string isRecurrence;
            public string isPrepay;
            public int insertPrice;
            public int releasePrice;
            public string cardNo;
            public string approvalNo;
            public string issuer;
            public string acquirer;
            public int approvedPrice;
            public DateTime approvedDate;
            public int in50000;
            public int in10000;
            public int in5000;
            public int in1000;
            public int in500;
            public int in100;
            public int in50;
            public int in10;
            public int out5000;
            public int out1000;
            public int out500;
            public int out100;
        }

        /// <summary>
        /// 13. 차량 출차 응답
        /// </summary>
        //public struct AJ_car_out_RESPONSE
        public class AJ_RESPONSE_car_out : REPONSE_Default
        {
            //public string code;
            //public string errmsgvariable;
            //public string errmsg;
            //public string key;
            public string orderNumber;          //0.9버전 추가
            public string localEquipmentId;     //추가 ???
        }
        #endregion

        #region 14. 요금 정산
        /// <summary>
        /// 14. 요금 정산 파라미터
        /// </summary>
        //public struct payment_PARAMS
        public class AJ_PARAMS_payment
        {
            public string carNo;
            public DateTime leaveDate;
            public int localEquipmentId;
            public string isSmallCar;
        }

        /// <summary>
        /// 14. 요금 정산 응답
        /// </summary>
        //public struct AJ_payment_RESPONSE
        public class AJ_RESPONSE_payment : REPONSE_Default
        {
            //public string code;
            //public string errmsgvariable;
            //public string errmsg;
            //public string key;
            public string parkingType;              //주차권 종류
            public string realPrice;                //결제 금액
            public string price;                    //정산 금액
            public string discountPrice;            //할인 금액
            public string enterDate;                //입차시간(타임스탬프), 0.7버전 변경
            public string durationSecond;           //주차시간(초), 0.7버전 변경
            public string prePayDiscountInfoId;     //앱 선결제시 투입 예정 설정 지류 할인권의 할인정보 아이디, 0.7버전 변경
            public string insertPrice;              //입금한 현금 총액, 0.9버전 변경
            public string recurrence;               //???
            public string localParkingId;           //주차권 UID, 1.3버전 추가
            public string lackPrice;                //법인차량 정산시 부족금액, 1.3버전 추가
            public string prepayPrice;              //1.4버전 추가, 선결제 금액
            public int CouponId = 0;
            public int CouponPrice = 0;
        }
        #endregion

        #region 15. 할인권 투입
        public class AJ_RESPONSE_discount_insert : REPONSE_Default
        {
            //public string code;
            //public string errmsgvariable;
            //public string errmsg;
            //public string key;
            public string parkingType;
            public string realPrice;
            public string price;
            public string discountPrice;
        }

        public class AJ_PARAMS_discount_insert
        {
            public int localDiscountInfoId;
            public int barcodeDiscountId;
            public string barcode;
            public string isPrepayDiscount;
            public string type;
            public string carNo;
            public double leaveDate;
        }
        #endregion

        #region 16. 요금 정산에 필요한 데이터
        public class AJ_RESPONSE_paymentinfo : REPONSE_Default
        {
            //public string code;
            //public string errmsgvariable;
            //public string errmsg;
            //public string key;
            public item_list_RESPONSE item_list;
        }

        public struct item_list_RESPONSE
        {
            public item_list_RESPONSE_list_corporateCar[] corporateCar_list;
            public item_list_RESPONSE_list_discountInfo[] discountInfo_list;
            public item_list_RESPONSE_list_barcodeDiscount[] barcodeDiscount_list;
            public item_list_RESPONSE_list_discountParking[] discountParking_list;
            public item_list_RESPONSE_list_monthlyTicket[] monthlyTicket_list;
            public item_list_RESPONSE_list_parking[] parking_list;
            public item_list_RESPONSE_list_parkingLot parkingLot_list;      //단일

            public item_list_RESPONSE_list_parkingLotHoliday[] parkingLotHoliday_list;
            public item_list_RESPONSE_list_priceInfo[] priceInfo_list;
            public item_list_RESPONSE_list_priceType[] priceType_list;

        }

        public struct item_list_RESPONSE_list_corporateCar
        {
            public string carNo;
            public string localCorporateCarId;
            public string price2;
        }

        public struct item_list_RESPONSE_list_discountInfo
        {
            public string groupName;
            public string localDiscountInfoId;
            public string groupCount;
            public string num;
            public string name;
            public string count;
            public string isDup;
            public string type;
            public string status;
        }

        public struct item_list_RESPONSE_list_barcodeDiscount
        {
            public string barcodeDiscountId;
            public string name;
            public string localDiscountInfoId;
            public string isDup;
            public string status;
            public string regDate;
        }

        public struct item_list_RESPONSE_list_discountParking
        {
            public string localDiscountParkingId;
            public string discountPrice;
            public string localDiscountInfoId;
            public string localParkingId;
            public string type;
            public string status;
        }

        public struct item_list_RESPONSE_list_monthlyTicket
        {
            public string localMonthlyTicketId;
            public string fromDate;
            public string toDate;
            public string carNo;
            public string price;
            public string discountPrice;
            public string point;
            public string paymentMethod;
            public string status;
            public string isUse;
        }

        public struct item_list_RESPONSE_list_parking
        {
            public string localParkingId;
            public string localCorporateCarId;
            public string carNo;
            public string carNoBack;
            public string type;
            public string priceType;
            public string prepayPrice;
            public string price;
            public string discountPrice;
            public string point;
            public string priceCalculator;
            public string insertPrice;
            public string notReleasePrice;
            public string paymentDate;
            public string enterDate;
            public string leaveDate;
            public string leaveScheduleDate;
            public string corporateCarRemainPrice;
            public string orderNumber;  //0124 추가
            public string isApp;
            public string isAjpass;
            public string status;
        }

        public struct item_list_RESPONSE_list_parkingLot
        {
            public string parkingLotId;
            public string name;
            public string tel;
            public string latitude;
            public string longitude;
            public string addr1;
            public string addr2;
            public string isAjpass;
            public string isSuvRv;
            public string spaceType;

            public string spaceNum;
            public string freeOpenInfo;
            public string dailyPrice;
            public string monthlyTicketPrice;
            public string monthlyTicketType;
            public string monthlyTicketComment;
            public string monthlyTicketNum;
            public string tagPrice;
            public string additionalPriceInfoTitle;
            public string additionalPriceInfoPrice;

            public string discountType;
            public string spaceHorizontal;
            public string spaceVertical;
            public string garageNum;
            public string availableCarType;
            public string availableParkingType;
            public string isWidewidth;
            public string priceCalculator;
            public string comment;
            public string holidayInfo;

            public string smallCarDiscount;
            public string disableCarDiscount;
            public string manOfMeritDiscount;
            public string operationType;
            public string location;
            public string holOpenTime;
            public string horizontal;
            public string vertical;
            public string femaleSpaceNum;
            public string disableSpaceNum;

            public string dailyOpenTime;
            public string satOpenTime;
            public string etcOpenTime;
            public string oneHourPrice;
            public string rateExceptionInfo;
            public string price;
            public string addPrice;
            public string type;
            public string homepageMsgView;
            public string status;

            public string garageVertical;
            public string garageHorizontal;
            public string regDate;
            public string priceRound;
            public string isTag;
            public string discountPriority;
            public string paperDiscountReUse;
            public string ownerName;
            public string companyNumber;
            public string discountMaxCount;

            public string monthlyLength;
            public string isWebDiscount;
            public string receipt;
            public string cashReceipt;
            public string homepageMonthlyPrice;
            public string homepageComment;
            public string timePriceDiscount;
            public string addDiscountPriceTypeId;
            public string monthlySmallCarDiscount;
            public string monthlyDisableCarDiscount;

            public string monthlyManOfMeritDiscount;
            public string companyName;
            public string updateDate;
            public string isCarDisWithOtherDis;
            public string isSync;
            public string extinctPeriod;
            public string localSmallCarDiscount;
            public string localDisableCarDiscount;
            public string localManOfMeritDiscount;
            public string localAddDiscountPriceTypeId;
            public string pcmDiscount;
            public string rotationSystem;   //부제 기능 170613 추가 진남

        }

        public struct item_list_RESPONSE_list_parkingLotHoliday
        {
            public string localParkingLotHolidayId;
            public string holiday;
            public string content;
            public string status;
        }

        public struct item_list_RESPONSE_list_priceInfo
        {
            public string localPriceInfoId;
            public string basicTimeMinute;
            public string addTimeMinute;
            public string dailyMaxPrice;
            public string standardTime;
            public string dayOfWeek;
            public string name;
            public string startTime;
            public string endTime;
            public string maxPrice;
            public string recurrenceTime;
            public string respiteTime;
            public string status;
        }

        public struct item_list_RESPONSE_list_priceType
        {
            public string localPriceTypeId;
            public string localPriceInfoId;
            public string basicPrice;
            public string addPrice;
            public string typeName;
        }





        #endregion

        #region 17. 장비 온라인 여부 확인 API
        public class AJ_RESPONSE_equipment_update : REPONSE_Default
        {
            //public string code;
            //public string errmsgvariable;
            //public string errmsg;
            //public string key;
        }
        
        #endregion

        #region 18. 요금 현금 납입 정보 저장
        public class AJ_RESPONSE_cash_price : REPONSE_Default
        {
            //public string code;
            //public string errmsgvariable;
            //public string errmsg;
            //public string key;
            public string totalInsertPrice;       //0.9버전 변경
            public string localParkingId;       //?????
        }
        #endregion


        #region 19. 사전 요금 정산
        public class AJ_RESPONSE_prepay_price : REPONSE_Default
        {
            //public string code;
            //public string errmsgvariable;
            //public string errmsg;
            //public string key;

            public string localParkingId;       // 1. 주차권 UID
            public string enterDate;            // 2. TIMESTAMP(입차시간)
            public string parkingType;          // 3. 주차권 종류
            public string price;                // 4. 출차예정시간으로 계산한 금액
            public string discountPrice;        // 5. 할인 금액
            public string realPrice;            // 6. 실제 결제 해야할 금액
            public string localCorporateCarId;  // 7. 법인차량 UID(법인차량 선결제일 경우)
            public string prepayPrice;          // 8. 이미 선결제 되어진 금액
            public string durationSecond;       // 9. 주차 시간
        }
        #endregion

        #region 20. 사전 정산 요금 결제
        public class AJ_RESPONSE_prepay_insert : REPONSE_Default
        {
            //public string code;
            //public string errmsgvariable;
            //public string errmsg;
            //public string key;

            public string orderNumber;      // 1. 주문번호
        }
        #endregion

        #region 21. 법인 차량 사전 정산 결제
        public class AJ_RESPONSE_prepay_corporateCar : REPONSE_Default
        {
            //public string code;
            //public string errmsgvariable;
            //public string errmsg;
            //public string key;

            public string lackPrice;            // 1. 충전금액으로 결제시, 금액부족으로 미결제된 금액
            public string price;                // 2. 결제금액
            public string discountPrice;        // 3. 할인금액
            public string parkingType;          // 4. 법인 차량의 주차권 종류

        }
        #endregion

        #region 22. 사전 정산기 정기권 가격 계산
        public class AJ_RESPONSE_monthlyticket_calcul : REPONSE_Default
        {
            //public string code;
            //public string errmsgvariable;
            //public string errmsg;
            //public string key;

            public string price;        //1. 계산된 결제 금액
            public string type;         //2. 신규 : new, 연장 : plus
            public string fromDate;     //3. “yyyyMMdd” 형식의 텍스트 정기권 시작 날자
            public string toDate;       //4. “yyyyMMdd” 형식의 텍스트 정기권 끝 날자
        }
        #endregion

        #region 23. 사전 정산기 정기권 그룹 목록
        public class AJ_RESPONSE_monthlyticket_group : REPONSE_Default
        {
            //public string code;
            //public string errmsgvariable;
            //public string errmsg;
            //public string key;

            public monthlyTicketGroup_list_RESPONSE[] monthlyTicketGroup_list;

        }

        public struct monthlyTicketGroup_list_RESPONSE
        {
            public string localMonthlyTicketGroupId;
            public string name;
        }
        #endregion

        #region 24. 사전 정산기 정기권 결제
        public class AJ_RESPONSE_monthlyticket_insert : REPONSE_Default
        {
            //public string code;
            //public string errmsgvariable;
            //public string errmsg;
            //public string key;
        }
        #endregion

        #region 25. 무료구간 LPR 입출차(프로토콜상 24번)
        public class AJ_RESPONSE_enter_subLPR : REPONSE_Default
        {
            //public string code;
            //public string errmsgvariable;
            //public string errmsg;
            //public string key;
        }
        #endregion

        #region 26. 요금 변경권 적용(프로토콜상 25번)
        public class AJ_RESPONSE_priceinfo_change : REPONSE_Default
        {
            //public string code;
            //public string errmsgvariable;
            //public string errmsg;
            //public string key;
        }
        #endregion

        #region 27. 사전 정산기 정기권 정보 목록(프로토콜상 26번)
        public class AJ_RESPONSE_monthlyTicketInfo_list : REPONSE_Default
        {
            //public string code;
            //public string errmsgvariable;
            //public string errmsg;
            //public string key;
            public pre_monthlyTicketInfo_list_RESPONSE[] pre_monthlyTicketInfo_list;    //정기권 정보 목록
        }

        public struct pre_monthlyTicketInfo_list_RESPONSE
        {
            public string localMonthlyTicketInfoId;             //정기권 UID
            public string monthlyTicketInfoId;                  //클라우드 정기권 정보 UID
            public string name;                                 //정기권 정보 이름
            public string price;                                //정기권 가격
            public string status;                               //정기권 정보 상태값
            public string regDate;                              //정기권 정보 등록일
            public string updateDate;                           //정기권 정보 수정일
            public string parkingLotId;                         //주차장 UID
        }
        #endregion    

        #region 28.
        public class AJ_RESPONSE_ReceiptReIssue_list : REPONSE_Default
        {
            //public string code;
            //public string errmsgvariable;
            //public string errmsg;
            //public string key;
            public log_list_RESPONSE_list_receipt ReceiptData;    //정기권 정보 목록
        }
        #endregion


        //발렛 사전정산기 관련(AWS), 18년 1월 17일 진남 작성
        #region 1. 발렛 리스트 조회 API
        public class AJ_Valet_List_Response_List
        {
            public AJ_Valet_List_Response[] aj_valet_list_response_list;
            public string return_code;
            public string return_message;
        }

        public struct AJ_Valet_List_Response
        {
            public string seq;
            public string car_number;
            public string phone_number;
            public string enter_date;
            public string name;
            public string discount_type;
            public string parking_line;
            public string payment_status;
            public string price;
        }
        #endregion

        #region 2. 발렛 정보 조회 API
        public class AJ_VALET_Information_Response
        {
            public string seq;
            public string car_number;
            public string phone_number;
            public string enter_date;
            public string name;
            public string discount_type;
            public string parking_line;
            public string payment_status;
            public string price;

            public string return_code;
            public string return_message;
        }
        #endregion

        #region 3. 발렛 결제/취소 결과 업데이트 API
        public class AJ_VALET_Result_Response
        {
            public string seq;
            public string car_number;
            public string phone_number;
            public string enter_date;
            public string name;
            public string discount_type;
            public string parking_line;
            public string payment_status;
            public string price;

            public string return_code;
            public string return_message;
        }
        #endregion
    }
}

