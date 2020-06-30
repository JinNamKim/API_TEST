using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AJParkLib
{
    namespace AJDataBase
    {
        public static class AJSetDataClass
        {
            public static AJ_MSSQL db = new AJ_MSSQL();

            public static bool InsertAutoDisocuntVehicle(string VehicleNumber, DateTime dtApplyStartDate, DateTime dtApplyEndDate, string ApplyDiscountCode, string Memo="", int UseCount=1)
            {
                bool bResult = false;
                string strQuery = string.Format("insert into AutoDiscountVehicleList(VehicleNumber,ApplyStartDate,ApplyEndDate,ApplyDiscountCode,UseCount,Memo) " + 
                                                "values ('{0}','{1} 00:00:00', '{2} 23:59:59','{3}',{4},'{5}')",
                                                VehicleNumber,dtApplyStartDate.ToString("yyyy-MM-dd"),dtApplyEndDate.ToString("yyyy-MM-dd"),ApplyDiscountCode,UseCount,Memo);

                int iResult = db.ExcuteNonQuery(strQuery);
                if (iResult >= 0)
                    bResult = true;

                return bResult;
            }

            public static bool DeleteAllAutoDisocuntVehicle()
            {
                bool bResult = false;
                string strQuery = string.Format("Delete AutoDiscountVehicleList");

                int iResult = db.ExcuteNonQuery(strQuery);
                if (iResult >= 0)
                    bResult = true;

                return bResult;
            }
        }
    }
}
