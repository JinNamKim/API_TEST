using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace AJParkLib
{
    namespace AJEnums
    {
        public enum CaptureType {양방향, 단방향 };
        public enum CaptureMethod { 전후방, 동시촬영 };

        public enum KindofDevice { 알수없음, LPR, 무인정산기, 유인정산기, 사전정산기, 관리, 무인정산기_LMS, 유인정산기_LMS, 사전정산기_LMS,DeviceControl,PrevChecker_LMS };
        /// <summary>
        /// AutoHub 연동
        /// </summary>
        public enum AutohubVehicleTypeInfo { 일반차량, 상품차량,출차불가 };

        /// <summary>
        /// 차량 처리 프로세스 코드(한글)
        /// </summary>;
        public enum ProcessStepKor { 대기, 차량입차, 정기권, 입차조회, 회차, 예외, 방문, 블랙리스트, 요금표출, 차량번호입력, 출차,출차완료 };

        /// <summary>
        /// 차량 처리 서브 프로세스 코드(한글)
        /// </summary>
        public enum SubProessStepKor {요금표출최초 };
        /// <summary>
        /// 차량 처리 종류
        /// </summary>
        public enum VehicleType { AJ_Pass,법인차량,정기권, 일반, 예외, 사전정산, 회차, 방문, 블랙리스트, 미정 }

        /// <summary>
        /// 차량 종류 (일반, 경차, 대형)
        /// </summary> 
        public enum KindofVehicle { General = 0x01, Compact = 0x02, Big = 0x03 };

        /// <summary>
        /// 차량 처리 프로세스 코드
        /// </summary>
        public enum ProcessStep { S0, S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14, S15, S16, S17 };

        /// <summary>
        /// 차량 처리 서브 프로세스 코드
        /// </summary>
        public enum SubProcessStep { SS0, SS1, SS2, SS3, SS4, SS5, SS6 };

        /// <summary>
        /// 프로그램 시작, 종료
        /// </summary>
        public enum ProgramStep { PS, PE };

        /// <summary>
        /// Database 상태 정보(에러)
        /// </summary>
        public enum DatabaseStatus { ConnectFail, QueryError };

        /// <summary>
        /// 정기권 정보 데이터 순서
        /// </summary>
        public enum SeasonCheckData { StartDateForSeason = 0x00, ExpireDateForSeason, LedMessage, UseOrNot, DigitInVehicleNumber, UserCode, SeasonCode, GroupCode, ParkingFeeLPR, EnteranceOrExit, UpMessage, DownMessage, UpTextProperty, DownTextProperty, SetCode };

        /// <summary>
        /// 입차 정보 데이터 순서
        /// </summary>
        public enum EnteranceData { PassIndex, LocalDeviceCode, PassTime, VehicleNumber, KindOfVehicle, VehicleImagePath };

        /// <summary>
        /// 예외 차량 데이터 순서
        /// </summary>
        public enum ExceptionData { ServiceTime, SDate, EDate, DisCountCode, Company, PubDate };

        /// <summary>
        /// 방문 차량 데이터 순서
        /// </summary>
        public enum VisitData { VisitDay,DiscountCode};

        /// <summary>
        /// 블랙리스트 차량 데이터 순서
        /// </summary>
        public enum BlackListData { VehicleNumber };

        /// <summary>
        /// 사전 정산 조회 데이터 순서
        /// </summary>
        public enum PrepaymentInfo { ParkingFare, PaymentAmount, PaymentCompleteOrNot,PaymentDate };

        public enum PaymentType { 카드, 현금, 할인무료};
        /// <summary>
        /// 할인권 사용 리스트
        /// </summary>
        public enum DiscountHistoryInfo { DiscountCode};

        /// <summary>
        /// 각종 데이터 길이
        /// </summary>
        public enum DataLength { SeasonVehicleInfo = 15, EnteranceData = 6, ExceptionData = 6, VisitData = 2,BlackList = 1, Prepayment = 4,DiscountHistory = 1 };

        public enum KindofFeeStationDetailDevice 
        {
            PapperMoneyRecognizer,
            CoinRecognizer,
            PapperHopper,
            CoinHopper,
            KeyPad,
            IoBoard,
            MagneticReader,
            print,
            BarCode,
            kis,
            kicc 
        };

        public enum WaveFileName {
            할인권투입,
            정기차량,
            회차차량,
            에이제이패스,
            법인차량,
            결제완료,
            영수증출력,
            입차내역없음,
            잔돈방출중,
            미방출증,
            카드를투입,
            카드를읽고,
            카드를뽑아,
            카드의방향,
            한도초과,
            다른카드,
            사용이정지,
            금액을확인,
            발렛_차량번호네자리를,
            발렛_차량내역이없습니다,
            발렛_화면의차량중,
            발렛_결제가실패했습니다
        };

        public enum Manned_WaveFileName
        {
            정기차량입니다,
            차량이들어왔습니다,
            결제처리되었습니다,
            회차차량입니다,
            할인무료처리되었습니다
        };

        public enum PageInfo
        {
            마감, 환경설정메인,환경설정,요금표출,초기화면,로그인,차량선택,차량조회
        }

        public enum BarrierCmd
        {
            오픈, 다운, 오픈락, 언락, 언락다운, 리셋, 상태정보
        }

        public enum VanType
        {
            KIS = 41,KICC = 42
        }
    }
}