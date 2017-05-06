using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

using MLFactorGraph;
using ResearchUtils;

namespace MobileMLFactorGraph
{
    public class MobileMLFGraph : MLFGraph
    {
        public MobileMLFGraph(DatabaseConnection database, bool bidirectionEdge = false)
            : base(bidirectionEdge)
        {
            dataset = new MobileDataset(database);
            base.SetDataSource(dataset);

            BuildGraph();
            AssignFactor();
        }

        void BuildGraph()
        {
            List<Int64> UserIds = dataset.InquiryDataWithMethod(new ArrayList(), dataset.FetchAllUserId);
            Dictionary<Int64, uint> nodeIdMapping = new Dictionary<Int64, uint>();
            foreach (Int64 id in UserIds)
            {
                Node v = base.AddNode();
                v.Attribute[MobileAttribute.UserId] = id;
                nodeIdMapping[id] = v.Id;
            }

            foreach (Int64 fromId in UserIds)
            {
                Node fromNode = base.FindNode(nodeIdMapping[fromId]);
                List<CallInfo> callInfo_from = dataset.InquiryDataWithMethod(new ArrayList { fromId }, dataset.FetchCallInfoList_From);
                List<CallInfo> callInfo_to = dataset.InquiryDataWithMethod(new ArrayList { fromId }, dataset.FetchCallInfoList_To);
                List<MessageInfo> msgInfo_from = dataset.InquiryDataWithMethod(new ArrayList { fromId }, dataset.FetchMessageInfoList_From);
                List<MessageInfo> msgInfo_to = dataset.InquiryDataWithMethod(new ArrayList { fromId }, dataset.FetchMessageInfoList_To);
                
                // Get Relation
                List<Int64> hasRelation = new List<Int64>();
                foreach (CallInfo info in callInfo_from)
                {
                    hasRelation.Add(info.CALL_TO);
                }
                foreach (CallInfo info in callInfo_to)
                {
                    hasRelation.Add(info.CALL_FROM);
                }
                foreach (MessageInfo info in msgInfo_from)
                {
                    hasRelation.Add(info.MSG_TO);
                }
                foreach (MessageInfo info in msgInfo_to)
                {
                    hasRelation.Add(info.MSG_FROM);
                }
                hasRelation = hasRelation.Distinct().ToList();

                foreach (Int64 toId in hasRelation)
                {
                    Edge e = base.AddEdge(nodeIdMapping[fromId], nodeIdMapping[toId], MobileLabel.UNKNOWN);
                }
            }

            List<FamilyInfo> familyInfo = dataset.InquiryDataWithMethod(new ArrayList(), dataset.FetchAllFamilyInfo);
            List<ColleagueInfo> colleagueInfo = dataset.InquiryDataWithMethod(new ArrayList(), dataset.FetchAllColleagueInfo);

            Dictionary<string, List<Int64>> rawFamilies = new Dictionary<string, List<Int64>>();
            Dictionary<string, List<Int64>> rawColleagues = new Dictionary<string, List<Int64>>();

            foreach (FamilyInfo info in familyInfo)
            {
                if (!rawFamilies.ContainsKey(info.ID))
                {
                    rawFamilies[info.ID] = new List<Int64>();
                }
                rawFamilies[info.ID].Add(info.MEMBER_ID);
            }
            foreach (KeyValuePair<string, List<Int64>> pair in rawFamilies)
            {
                if (pair.Value.Count != 1)
                {
                    Group g = base.AddGroup(MobileLabel.FAMILY, pair.Value.Select(x => nodeIdMapping[x]).ToList());
                    g.Attribute[MobileAttribute.GroupId] = pair.Key;
                }
            }

            foreach (ColleagueInfo info in colleagueInfo)
            {
                if (!rawColleagues.ContainsKey(info.ID))
                {
                    rawColleagues[info.ID] = new List<Int64>();
                }
                rawColleagues[info.ID].Add(info.MEMBER_ID);
            }
            foreach (KeyValuePair<string, List<Int64>> pair in rawColleagues)
            {
                if (pair.Value.Count != 1)
                {
                    Group g = base.AddGroup(MobileLabel.COLLEAGUE, pair.Value.Select(x => nodeIdMapping[x]).ToList());
                    g.Attribute[MobileAttribute.GroupId] = pair.Key;
                }
            }

            return;
        }
        void AssignFactor()
        {
            base.AddFactor(MobileFactor.EDGE_CALLMSGFREQUENCY, MobileFactorFunction.Edge_CallMsgFrequency, Layer.EdgeLayer);
        }

        MobileDataset dataset;
    }
}
