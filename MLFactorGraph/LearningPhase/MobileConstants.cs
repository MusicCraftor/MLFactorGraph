using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

using MLFactorGraph;

namespace MobileMLFactorGraph
{
    /*
     * NODE_X = 0XXX;
     * EDGE_X = 1XXX;
     * GROUP_X = 2XXX;
     * ALL_X = 3XXX;
     */
    public static class MobileFactor
    {
        public static int DEFAULT = 0;

        public static int EDGE_CALLMSGFREQUENCY = 1001;
    }

    public static class MobileFactorFunction
    {
        public static double Edge_CallMsgFrequency(Factorable obj, Dataset dataset)
        {
            Edge e = obj as Edge;
            if (e == null)
            {
                return 0.0;
            }

            List<CallInfo> callInfo = dataset.InquiryData<List<CallInfo>>(new ArrayList {
                e.From.Attribute[MobileAttribute.UserId],
                e.To.Attribute[MobileAttribute.UserId]
            });
            List<MessageInfo> msgInfo = dataset.InquiryData<List<MessageInfo>>(new ArrayList {
                e.From.Attribute[MobileAttribute.UserId],
                e.To.Attribute[MobileAttribute.UserId]
            });

            return (callInfo == null ? 0 : callInfo.Count) +
                (msgInfo == null ? 0 : msgInfo.Count);
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
        public static short UNKNOWN = 0;
        public static short NORELATION = 1;
        public static short FAMILY = 2;
        public static short COLLEAGUE = 3;
        public static short FRIEND = 4;
    }
}
