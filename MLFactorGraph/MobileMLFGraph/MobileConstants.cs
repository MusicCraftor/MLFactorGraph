using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

using MLFactorGraph;

namespace MobileMLFactorGraph
{
    public static class MobileFactor
    {
        public static int DEFAULT = 0;

        public static int EDGE_CALLMSGCOUNT = 10000;
        public static int EDGE_CALLMSGFREQUENCY_HOUR_BASE = 10100;
    }

    public static class MobileFactorFunctionGenerator
    {
        public static byte Edge_CallMsgCount(Factorable obj, Dataset dataset)
        {
            Edge e = obj as Edge;
            if (e == null)
            {
                return 0;
            }

            List<CallInfo> callInfo = dataset.InquiryData<List<CallInfo>>(new ArrayList {
                e.From.Attribute[MobileAttribute.UserId],
                e.To.Attribute[MobileAttribute.UserId]
            });
            List<MessageInfo> msgInfo = dataset.InquiryData<List<MessageInfo>>(new ArrayList {
                e.From.Attribute[MobileAttribute.UserId],
                e.To.Attribute[MobileAttribute.UserId]
            });

            Int64 callCount = dataset.InquiryDataWithMethod<List<Int64>>(new ArrayList(),
                MobileDataset.FetchAllCallInfoCount_List)[0];
            Int64 msgCount = dataset.InquiryDataWithMethod<List<Int64>>(new ArrayList(),
                MobileDataset.FetchAllMessageInfoCount_List)[0];

            double result = (callInfo == null ? 0 : callInfo.Count)+
                (msgInfo == null ? 0 : msgInfo.Count);
            return Bytelize(result, (callCount + msgCount) / 10);
        }
        public static Dictionary<int, Factorable.UnitaryFactor> Gen_Edge_CallMsgFrequency_Hour()
        {
            Dictionary<int, Factorable.UnitaryFactor> functionDict = new Dictionary<int, Factorable.UnitaryFactor>();
            for (int i = 0; i < 24; i++)
            {
                int hr = i;
                functionDict.Add(MobileFactor.EDGE_CALLMSGFREQUENCY_HOUR_BASE + i,
                    delegate (Factorable obj, Dataset dataset)
                    {
                        return Edge_CallMsgFrequency_Hour(obj, dataset, hr);
                    });
            }
            return functionDict;
        }
        public static byte Edge_CallMsgFrequency_Hour(Factorable obj, Dataset dataset, int hour)
        {
            Edge e = obj as Edge;
            if (e == null)
            {
                return 0;
            }

            List<CallInfo> callInfo = dataset.InquiryData<List<CallInfo>>(new ArrayList {
                e.From.Attribute[MobileAttribute.UserId],
                e.To.Attribute[MobileAttribute.UserId]
            });
            List<MessageInfo> msgInfo = dataset.InquiryData<List<MessageInfo>>(new ArrayList {
                e.From.Attribute[MobileAttribute.UserId],
                e.To.Attribute[MobileAttribute.UserId]
            });

            int callCount = callInfo.Count;
            int msgCount = msgInfo.Count;
            int callHourCount = callInfo.Count(x => x.CALL_TIME.Hour == hour);
            int msgHourCount = msgInfo.Count(x => x.MSG_TIME.Hour == hour);

            double result = callHourCount + msgHourCount;
            return Bytelize(result, callCount + msgCount);
        }

        static byte Bytelize(double result, double maxValue, double minValue = 0)
        {
            return (byte)(result * 254 / (maxValue - minValue));
        }
    }

    public static class MobileAttribute
    {
        public static int DEFAULT = 0;

        public static int UserId = 1001;
        public static int GroupId = 2001;

        public static Type UserId_Type = typeof(Int64);
        public static Type GroupId_Type = typeof(string);
    }

    public static class MobileLabel
    {
        public const short NORELATION = 0;
        public const short FAMILY = 1;
        public const short COLLEAGUE = 2;
        public const short FRIEND = 3;
        public const short UNKNOWN = -1;

        public static List<short> ToList()
        {
            return new List<short>
            {
                NORELATION,
                FAMILY,
                COLLEAGUE,
                FRIEND
            };
        }
    }
}
