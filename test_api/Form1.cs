using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AJParkLib.AJWebdataClass;
using AJParkLib.AJDataBase;
using Newtonsoft.Json.Linq;
using RestSharp;




namespace test_api
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            AJParkLib.AJCommon.CommonClass.init();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //AJ_RESPONSE_initializedInfo aaa = new AJ_RESPONSE_initializedInfo();
            AJ_RESPONSE_initializedInfo bbb = new AJ_RESPONSE_initializedInfo();
            //TEST
            //TEST 2 FROM MSI
            bbb = AJWebDatabase.AJ_Get_Initial_Info(3);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            AJ_RESPONSE_CloseInfo test_2 = new AJ_RESPONSE_CloseInfo();
            //test_2 = AJWebDatabase.temp_name_2(1, DateTime.Now, DateTime.Now);
            //test_2 = AJWebDatabase.temp_name_2(1, "", new DateTime(2017, 04, 24, 16, 30, 50));
            test_2 = AJWebDatabase.temp_name_2(29, "", DateTime.Now);
        }

        private void button19_Click(object sender, EventArgs e)
        {
            AJ_RESPONSE_query test_3 = new AJ_RESPONSE_query();
            test_3 = AJWebDatabase.temp_name_3("select* from ADMIN;");
        }



        private void button2_Click(object sender, EventArgs e)
        {
            AJ_RESPONSE_vaultCashInfo_POST test_4_1 = new AJ_RESPONSE_vaultCashInfo_POST();
            test_4_1 = AJWebDatabase.temp_name_4_1(3, 10, 250, 0, 0, "y");     //0.7버전 파라미터 추가
        }

        private void button10_Click(object sender, EventArgs e)
        {
            AJ_RESPONSE_vaultCashInfo_GET test_4_2 = new AJ_RESPONSE_vaultCashInfo_GET();
            test_4_2 = AJWebDatabase.temp_name_4_2(3);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            AJ_RESPONSE_monthlyTicketInfo test_5 = new AJ_RESPONSE_monthlyTicketInfo();
            test_5 = AJWebDatabase.temp_name_5();

        }

        private void button6_Click(object sender, EventArgs e)
        {
            AJ_RESPONSE_error test_6 = new AJ_RESPONSE_error();
            test_6 = AJWebDatabase.temp_name_6(1, "1", "1");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            AJ_RESPONSE_nopay test_7 = new AJ_RESPONSE_nopay();
            test_7 = AJWebDatabase.temp_name_7("1", 10000, 0, 1, 0, 0, 0, 0, 0, 0);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            AJ_RESPONSE_result test_8 = new AJ_RESPONSE_result();
            test_8 = AJWebDatabase.temp_name_8(3, "n");
            test_8 = AJWebDatabase.temp_name_8(5, "n");
            test_8 = AJWebDatabase.temp_name_8(29, "n");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            AJ_RESPONSE_log test_9 = new AJ_RESPONSE_log();
            test_9 = AJWebDatabase.temp_name_9("");

        }

        private void button8_Click(object sender, EventArgs e)
        {
            AJ_RESPONSE_vipInfo test_10 = new AJ_RESPONSE_vipInfo();
            test_10 = AJWebDatabase.temp_name_10();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //출차 미인식 차량 목록
            AJ_RESPONSE_unkown_price test_11 = new AJ_RESPONSE_unkown_price();
            //test_11 = AJWebDatabase.temp_name_11(1, "12힘1234", DateTime.Now);     //0.7에서 변경
            test_11 = AJWebDatabase.temp_name_11("2018");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            AJ_RESPONSE_car_enter test_12 = new AJ_RESPONSE_car_enter();
            //test_12 = AJWebDatabase.temp_name_12("12헤1234", "c:\\123123.jpg", DateTime.Now, 1, "n");
            //test_12 = AJWebDatabase.temp_name_12("12헤1234", "c:\\123123.jpg", DateTime.Now, 1, "n");
            //test_12 = AJWebDatabase.temp_name_12("12가4568", "\\\\192.10.60.123\\NPImage\\2017\\0126\\CH1_20170126000036_0000000000.jpg", DateTime.Now, 1, "n");
            //test_12 = AJWebDatabase.temp_name_12("12가1007", "c:\\123123.jpg", DateTime.Now, 1, "n");      //나우
            //test_12 = AJWebDatabase.temp_name_12("22가2222", "c:\\123123.jpg", new DateTime(2017,3,29,5,5,5), 1, "n");        //일부러 시간 설정
            //test_12 = AJWebDatabase.temp_name_12("87테1515", "c:\\123123.jpg", new DateTime(2018, 4, 17, 08, 15, 5), 47, "n");        //오토허브
            //test_12 = AJWebDatabase.temp_name_12("21모5292", "c:\\a.jpg", new DateTime(2018, 9, 21, 02, 30, 0), 47, "n");        //사무실 서버
            test_12 = AJWebDatabase.temp_name_12("56호1349", "c:\\a.jpg", new DateTime(2019, 2, 20, 09, 00, 0), 3, "n");        //부천먹자골

        }

        private void button3_Click(object sender, EventArgs e)
        {
            AJ_RESPONSE_car_out test_13 = new AJ_RESPONSE_car_out();
            //test_13 = AJWebDatabase.temp_name_13(2, "12바1234", DateTime.Now, "cash", 30000, 50000, 20000, "", "", "", "", 0, DateTime.Now);
            //test_13 = AJWebDatabase.temp_name_13(2, "12가1238", DateTime.Now, "cash", 30000, "n", "", 50000, 20000, "", "", "", "", 0, DateTime.Now, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            //test_13 = AJWebDatabase.temp_name_13(2, "c:\\123123.jpg", "12헤1234", DateTime.Now, "cash", 30000, "n", "", "n", 50000, 0, 20000, "", "", "", "", 0, DateTime.Now, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);    //0.7버전 변경
            //test_13 = AJWebDatabase.temp_name_13(2, "12헤1234", DateTime.Now, "cash", 30000, "n", "", "n", 50000, 0, 20000, "", "", "", "", 0, DateTime.Now, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);    //1.3버전 변경(이미지 파라미터 삭제)
            //test_13 = AJWebDatabase.temp_name_13(3, "44테4445", new DateTime(2018, 6, 5, 13, 01, 00), "cash", 6000, "n", "", "n", 6000, 0, 0, "", "", "", "", 0, DateTime.Now, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0);    //테스트
            test_13 = AJWebDatabase.temp_name_13(3, "44테4445", new DateTime(2018, 6, 5, 13, 01, 00), "card", 2000, "n", "", "n", 2000, 0, 0, "4330-28**-****-****", "00695187", "진남비자", "진남카드", 6000, DateTime.Now, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0);    //테스트
        }

        private void button13_Click(object sender, EventArgs e)
        {
            AJ_RESPONSE_payment test_14 = new AJ_RESPONSE_payment();
            //test_14 = AJWebDatabase.temp_name_14("12가1239", DateTime.Now);
            //test_14 = AJWebDatabase.temp_name_14("12가1239", new DateTime(2017, 1, 2, 7, 30, 1));
            //test_14 = AJWebDatabase.temp_name_14("12가1239", "c:\\123123.jpg", new DateTime(2017, 1, 2, 7, 30, 1), 1, "n");
            //test_14 = AJWebDatabase.temp_name_14("62호7239", /*new DateTime(2017, 1, 2, 7, 30, 1)*/DateTime.Now, 1, "n");    //0.8버전 변경(이미지 리퀘스트 삭제)
            //test_14 = AJWebDatabase.temp_name_14(1, "62호7239", /*new DateTime(2017, 1, 2, 7, 30, 1)*/DateTime.Now, 1, "n");    //1.1버전 변경(localParkingId 파라미터 추가)
            //test_14 = AJWebDatabase.temp_name_14(1, "62호7239", /*new DateTime(2017, 1, 2, 7, 30, 1)*/DateTime.Now, 1, "c:\\123123.jpg", "n");    //1.3버전 변경(이미지 파라미터 추가)
            //test_14 = AJWebDatabase.temp_name_14("12가5479", /*new DateTime(2017, 1, 2, 7, 30, 1)*/DateTime.Now, 1, "c:\\123123.jpg", "n");    //대리님이 파라미터 위치 변경
            test_14 = AJWebDatabase.temp_name_14("22테2223", /*new DateTime(2017, 4, 26, 11, 00, 00)*/new DateTime(2020, 6, 20, 17, 00, 1), 3, "c:\\123123.jpg", "n");    //테스트


            AJ_RESPONSE_payment asdad = test_14;


        }

        private void button14_Click(object sender, EventArgs e)

        {
            AJ_RESPONSE_discount_insert test_15 = new AJ_RESPONSE_discount_insert();
            //test_15 = AJWebDatabase.temp_name_15(5, 0, "", "n", "paper", "99구9996", DateTime.Now);
            //test_15 = AJWebDatabase.temp_name_15(5, 0, "", "n", "paper", "12헤1234", (new DateTime(2017, 1, 17, 18, 42, 13)));
            //test_15 = AJWebDatabase.temp_name_15(5, 0, "", "n", "paper", "11가1717", (new DateTime(2017, 1, 17, 18, 42, 13)));
            test_15 = AJWebDatabase.temp_name_15(5, 0, "", "n", "paper", "11가1717", DateTime.Now);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            AJ_RESPONSE_paymentinfo test_16 = new AJ_RESPONSE_paymentinfo();
            test_16 = AJWebDatabase.temp_name_16();
        }

        private void button18_Click(object sender, EventArgs e)
        {
            AJ_RESPONSE_equipment_update test_17 = new AJ_RESPONSE_equipment_update();
            test_17 = AJWebDatabase.temp_name_17(1);
        }

        private void button20_Click(object sender, EventArgs e)
        {
            AJ_RESPONSE_cash_price test_18 = new AJ_RESPONSE_cash_price();
            test_18 = AJWebDatabase.temp_name_18("12헤1234", 5000);
        }

        private void button21_Click(object sender, EventArgs e)
        {
            //사전 요금 정산
            AJ_RESPONSE_prepay_price test_19 = new AJ_RESPONSE_prepay_price();
            //DateTime tt = new DateTime(2018,2,5,17,20,00);
            //test_19 = AJWebDatabase.temp_name_19(1, "88가8888", tt,"88가8888", , 0);
        }

        private void button22_Click(object sender, EventArgs e)
        {
            //사전 정산 요금 결제
            AJ_RESPONSE_prepay_insert temp_20 = new AJ_RESPONSE_prepay_insert();
            temp_20 = AJWebDatabase.temp_name_20(99, "11타1111", DateTime.Now, "cash", 50000, 40000, 10000, 0, 0, "", "", "", "", "", DateTime.Now, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            
        }


        private void button23_Click(object sender, EventArgs e)
        {
            //법인 차량 사전 정산 결제
            AJ_RESPONSE_prepay_corporateCar temp_21 = new AJ_RESPONSE_prepay_corporateCar();
              temp_21 = AJWebDatabase.temp_name_21(1, 1, "296", 30000, 10000, DateTime.Now);
        }

        

        private void button26_Click(object sender, EventArgs e)
        {
            //사전 정산기 정기권 가격 계산
            AJ_RESPONSE_monthlyticket_calcul temp_22 = new AJ_RESPONSE_monthlyticket_calcul();
            temp_22 = AJWebDatabase.temp_name_22("12헤1234", DateTime.Now, new DateTime(2017, 09, 10));
        }

        private void button25_Click(object sender, EventArgs e)
        {
            //사전 정산기 정기권 그룹 목록
            AJ_RESPONSE_monthlyticket_group temp_23 = new AJ_RESPONSE_monthlyticket_group();
            temp_23 = AJWebDatabase.temp_name_23();
        }

        private void button24_Click(object sender, EventArgs e)
        {
            //사전 정산기 정기권 결제
            AJ_RESPONSE_monthlyticket_insert temp_24 = new AJ_RESPONSE_monthlyticket_insert();
            temp_24 = AJWebDatabase.temp_name_24("y", 1, 2, 1, "12김5555", "", "김진남", "7865", "2동", "3호", "단골", "cash", 100000, DateTime.Now, new DateTime(2017, 09, 30), "119", "y", 100000, 0, 0, "", "", "", "", "", DateTime.Now, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            //AJWebDatabase.AJ_Set_IP("192.10.60.92");       //사무실
            //AJWebDatabase.AJ_Set_IP("192.168.1.100");       //사무실
            //AJWebDatabase.AJ_Set_IP("112.216.153.186");         //테스트 서버
            //AJWebDatabase.AJ_Set_IP("uiwang1.iptime.org");    //의왕
            //AJWebDatabase.AJ_Set_IP("ajkd.asuscomm.com");    //검단 오류
            
            //AJWebDatabase.AJ_Set_IP("192.10.60.100");       //

            //AJWebDatabase.AJ_Set_IP("192.10.60.96");         //사무실
            //AJWebDatabase.AJ_Set_IP("ajautohub.asuscomm.com");         //오토허브
            //AJWebDatabase.AJ_Set_IP("ajpuravida.asuscomm.com");         //문정 프라비다

            AJWebDatabase.AJ_Set_IP(tb_IP.Text);       //테스트

            AJWebDatabase.AJ_Set_PORT(tb_PORT.Text);
            //AJWebDatabase.AJ_Set_PORT("8080");

            //AJParkLib.AJCommon.CommonClass.SendLog("test");
        }

        
        private void button28_Click(object sender, EventArgs e)
        {
            //25번 무료구간 LPR 입출차(프로토콜상 24번)
            AJ_RESPONSE_enter_subLPR temp_25 = new AJ_RESPONSE_enter_subLPR();
            //temp_25 = AJWebDatabase.temp_name_25("99테9990", DateTime.Now, 998);
            //DateTime tt = new DateTime(2018,2,1,00,00,10);
            //temp_25 = AJWebDatabase.temp_name_25("44테4444", tt, 998, "n");
            DateTime tt = new DateTime(2018,2,5,19,20,00);
            temp_25 = AJWebDatabase.temp_name_25("44테4444",tt, 999);
        }

        private void button29_Click(object sender, EventArgs e)
        {
            //26번 요금 변경권 적용(프로토콜상 25번)
            AJ_RESPONSE_priceinfo_change temp_26 = new AJ_RESPONSE_priceinfo_change();
            temp_26 = AJWebDatabase.temp_name_26(1, "1234");
        }

        private void button30_Click(object sender, EventArgs e)
        {
            //27번 사전정산기 정기권 정보 목록(프로토콜상 26번)
            AJ_RESPONSE_monthlyTicketInfo_list temp_27 = new AJ_RESPONSE_monthlyTicketInfo_list();
            temp_27 = AJWebDatabase.temp_name_27();
        }

        private void button27_Click(object sender, EventArgs e)
        {
            double temp_millisec = 1243546212312;
            DateTime temp_date_time = AJParkLib.AJCommon.Time.MilliSec_To_DateTime(temp_millisec);
        }

        private void button31_Click(object sender, EventArgs e)
        {
            string aaaa = AJWebDatabase.my_test("7720");
        }

        private void button32_Click(object sender, EventArgs e)
        {
            AJWebDatabase.AJ_Set_IP(tb_IP.Text);       //테스트
            AJWebDatabase.AJ_Set_PORT("2080");
        }

        private void button33_Click(object sender, EventArgs e)
        {

            AJWebDatabase.AJ_Set_IP(tb_IP.Text);       //테스트
            AJWebDatabase.AJ_Set_PORT("2080");


            string[] my_time = tb_시간.Text.Split(':');

            AJ_RESPONSE_payment test_14 = new AJ_RESPONSE_payment();
            //test_14 = AJWebDatabase.temp_name_14(tb_차번.Text, /*new DateTime(2017, 4, 26, 11, 00, 00)*/DateTime.Now, Convert.ToInt32(tb_정산번호.Text), "c:\\enter_lpr_error.jpg", "n");    //테스트
            test_14 = AJWebDatabase.temp_name_14(tb_차번.Text, new DateTime(2019, 1, 3, Convert.ToInt32(my_time[0]), Convert.ToInt32(my_time[1]), Convert.ToInt32(my_time[2])), Convert.ToInt32(tb_정산번호.Text), "c:\\enter_lpr_error.jpg", "n");    //테스트

            //AJParkLib.AJWebdataClass.AJ_RESPONSE_payment fee_data = AJWebDatabase.temp_name_14(CurrentVehicleData.VehicleNumber, CurrentVehicleData.ExitPassTime, Convert.ToInt32(CommonVariable.DeviceNumber), CurrentVehicleData.ImagePath, "n");

            if (test_14.code == "0")
            {
                lb_응답.Text = "응답 : 정상";
                lb_내용.Text = "내용 : ";
                tb_요금.Text = test_14.price;
                tb_할인요금.Text = test_14.discountPrice;

                tb_결과.Text = string.Format("{0:###,0}", Convert.ToInt32(test_14.price) - Convert.ToInt32(test_14.discountPrice));
            }
            else
            {
                lb_응답.Text = "응답 : 에러";
                lb_내용.Text = test_14.errmsg;
            }

            


        }

        

        

        

        

        

        

        

        

        

        
    }
}
