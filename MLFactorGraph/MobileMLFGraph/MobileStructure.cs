using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileMLFactorGraph
{
    public class MobileTableName
    {
        public static string CellInfo = "cell_info";
        public static string CellAdjacency = "cell_adj";
        public static string PoiInfo = "poi_info";
        public static string FamilyInfo = "family_info";
        public static string ColleagueInfo = "colleague_info";
        public static string FriendInfo = "friend_info";
        public static string CallInfo = "call_info";
        public static string MessageInfo = "msg_info";
        public static string UserInfo = "user_info";
        public static string UserEvent = "user_event";
        public static string UserInteraction = "user_interaction";
        public static string UserOD = "user_od";
    }

    public class CellInfo
    {
        public int CELL_ID { get; set; }
        public double LONGITUDE { get; set; }
        public double LATITUDE { get; set; }
    }

    public class CellAdjacency
    {
        public Int32 CELL_A { get; set; }
        public double LONGITUDE_A { get; set; }
        public double LATITUDE_A { get; set; }
        public Int32 CELL_B { get; set; }
        public double LONGITUDE_B { get; set; }
        public double LATITUDE_B { get; set; }
    }

    public class PoiInfo
    {
        public int CELL_ID { get; set; }
        public int POI0 { get; set; }
        public int POI1 { get; set; }
        public int POI2 { get; set; }
        public int POI3 { get; set; }
        public int POI4 { get; set; }
        public int POI5 { get; set; }
        public int POI6 { get; set; }
        public int POI7 { get; set; }
        public int POI8 { get; set; }
        public int POI9 { get; set; }
        public int POI10 { get; set; }
        public int POI11 { get; set; }
        public int POI12 { get; set; }
        public int POI13 { get; set; }
        public int POI14 { get; set; }
        public int POI15 { get; set; }
        public int POI16 { get; set; }
    }

    public class FamilyInfo
    {
        public string ID { get; set; }
        public Int64 MEMBER_ID { get; set; }
    }

    public class ColleagueInfo
    {
        public string ID { get; set; }
        public Int64 MEMBER_ID { get; set; }
    }

    public class FriendInfo
    {
        public Int64 ID_A { get; set; }
        public Int64 ID_B { get; set; }
    }

    public class CallInfo
    {
        public Int64 CALL_FROM { get; set; }
        public Int64 CALL_TO { get; set; }
        public DateTime CALL_TIME { get; set; }
        public Int32 CELL_FROM { get; set; }
        public Int32 CELL_TO { get; set; }
    }

    public class MessageInfo
    {
        public Int64 MSG_FROM { get; set; }
        public Int64 MSG_TO { get; set; }
        public DateTime MSG_TIME { get; set; }
        public Int32 CELL_FROM { get; set; }
        public Int32 CELL_TO { get; set; }
    }

    public class UserInfo
    {
        public Int64 ID { get; set; }
        public string ONLINE_ID { get; set; }
        public string TOTAL_FEE { get; set; }
        public string CALL_DURATION { get; set; }
        public string TOTAL_DUR { get; set; }
        public string TOTAL_CNT { get; set; }
        public string OUT_DUR { get; set; }
        public string OUT_CNT { get; set; }
        public string INT_DUR { get; set; }
        public string INT_CNT { get; set; }
        public string P2P_SMS_UP_CNT { get; set; }
        public string P2P_SMS_INNER_DOWN_CNT { get; set; }
        public string P2P_SMS_OUTER_DOWN_CNT { get; set; }
        public string P2P_SMS_INNER_UP_CNT { get; set; }
        public string P2P_SMS_OUTER_UP_CNT { get; set; }
        public string SMS_BILL_CNT { get; set; }
        public string B_LAC_CALL_1 { get; set; }
        public string B_LAC_CNT_1 { get; set; }
        public string B_LAC_DURATION_1 { get; set; }
        public string B_LAC_CALL_2 { get; set; }
        public string B_LAC_CNT_2 { get; set; }
        public string B_LAC_DURATION_2 { get; set; }
        public string B_LAC_CALL_3 { get; set; }
        public string B_LAC_CNT_3 { get; set; }
        public string B_LAC_DURATION_3 { get; set; }
        public string A_LAC_CALL_1 { get; set; }
        public string A_LAC_CNT_1 { get; set; }
        public string A_LAC_DURATION_1 { get; set; }
        public string A_LAC_CALL_2 { get; set; }
        public string A_LAC_CNT_2 { get; set; }
        public string A_LAC_DURATION_2 { get; set; }
        public string A_LAC_CALL_3 { get; set; }
        public string A_LAC_CNT_3 { get; set; }
        public string A_LAC_DURATION_3 { get; set; }
        public string W_LAC_CALL_1 { get; set; }
        public string W_LAC_CNT_1 { get; set; }
        public string W_LAC_DURATION_1 { get; set; }
        public string W_LAC_CALL_2 { get; set; }
        public string W_LAC_CNT_2 { get; set; }
        public string W_LAC_DURATION_2 { get; set; }
        public string W_LAC_CALL_3 { get; set; }
        public string W_LAC_CNT_3 { get; set; }
        public string W_LAC_DURATION_3 { get; set; }
    }

    public class UserEvent
    {
        public Int64 ID { get; set; }
        public DateTime EVENT_TIME { get; set; }
        public Int16 EVENT_TYPE { get; set; }
        public Int32 START_LAC { get; set; }
        public Int32 START_CELL_ID { get; set; }
        public Int32 END_LAC { get; set; }
        public Int32 END_CELL_ID { get; set; }
        public Int16 AREA_CODE { get; set; }
    }

    public class UserInteraction
    {
        public Int64 ID { get; set; }
        public Int64 OPP_ID { get; set; }
        public Int16 OPP_TYPE { get; set; }
        public Int32 CALL_ALL_CNT { get; set; }
        public Int32 CALL_ALL_DURATION { get; set; }
        public Int32 CALL_BUSY_CNT { get; set; }
        public Int32 CALL_BUSY_DURATION { get; set; }
        public Int32 CALL_FREE_CNT { get; set; }
        public Int32 CALL_FREE_DURATION { get; set; }
        public Int32 CALL_WEEKEND_CNT { get; set; }
        public Int32 CALL_WEEKEND_DURATION { get; set; }
        public Int32 CALL_BUSY_OUT_CNT { get; set; }
        public Int32 CALL_BUSY_OUT_DURATION { get; set; }
        public Int32 CALL_FREE_OUT_CNT { get; set; }
        public Int32 CALL_FREE_OUT_DURATION { get; set; }
        public Int32 CALL_WEEKEND_OUT_CNT { get; set; }
        public Int32 CALL_WEEKEND_OUT_DURATION { get; set; }
        public Int32 CALL_BUSY_IN_CNT { get; set; }
        public Int32 CALL_BUSY_IN_DURATION { get; set; }
        public Int32 CALL_FREE_IN_CNT { get; set; }
        public Int32 CALL_FREE_IN_DURATION { get; set; }
        public Int32 CALL_WEEKEND_IN_CNT { get; set; }
        public Int32 CALL_WEEKEND_IN_DURATION { get; set; }
        public Int32 SMS_ALL_CNT { get; set; }
        public Int32 SMS_BUSY_CNT { get; set; }
        public Int32 SMS_FREE_CNT { get; set; }
        public Int32 SMS_WEEKEND_CNT { get; set; }
        public Int32 SMS_BUSY_OUT_CNT { get; set; }
        public Int32 SMS_FREE_OUT_CNT { get; set; }
        public Int32 SMS_WEEKEND_OUT_CNT { get; set; }
        public Int32 SMS_BUSY_IN_CNT { get; set; }
        public Int32 SMS_FREE_IN_CNT { get; set; }
        public Int32 SMS_WEEKEND_IN_CNT { get; set; }
    }

    public class UserOD
    {
        public Int64 ID { get; set; }
        public Int32 OCELL_ID { get; set; }
        public float OCELL_LONGITUDE { get; set; }
        public float OCELL_LATITUDE { get; set; }
        public Int32 DCELL_ID { get; set; }
        public float DCELL_LONGITUDE { get; set; }
        public float DCELL_LATITUDE { get; set; }
    }
}
