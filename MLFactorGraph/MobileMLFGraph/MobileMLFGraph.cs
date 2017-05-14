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
            : base(MobileLabel.ToList(), bidirectionEdge)
        {
            dataset = new MobileDataset(database);
            base.SetDataSource(dataset);

            BuildGraph();
            AssignFactor();
            FactorInitialization(Layer.AllLayer);

            List<FamilyInfo> familyInfo = dataset.InquiryDataWithMethod(new ArrayList(), MobileDataset.FetchAllFamilyInfo);
            List<ColleagueInfo> colleagueInfo = dataset.InquiryDataWithMethod(new ArrayList(), MobileDataset.FetchAllColleagueInfo);
            List<FriendInfo> friendInfo = dataset.InquiryDataWithMethod(new ArrayList(), MobileDataset.FetchAllFriendInfo);

            /*int correct = 0, error = 0;
            foreach (Edge e in this.EdgeLayer)
            {
                switch (e.Label)
                {
                    case MobileLabel.FAMILY:
                        FamilyInfo fA, fB;
                        fA = familyInfo.Find(x => x.MEMBER_ID == (Int64)e.From.Attribute[MobileAttribute.UserId]);
                        fB = familyInfo.Find(x => x.MEMBER_ID == (Int64)e.To.Attribute[MobileAttribute.UserId]);
                        try
                        {
                            if (fA.ID == fB.ID)
                            {
                                correct++;
                            }
                        }
                        catch (Exception)
                        {
                            error++;
                        }
                        break;
                    case MobileLabel.COLLEAGUE:
                        ColleagueInfo cA, cB;
                        cA = colleagueInfo.Find(x => x.MEMBER_ID == (Int64)e.From.Attribute[MobileAttribute.UserId]);
                        cB = colleagueInfo.Find(x => x.MEMBER_ID == (Int64)e.To.Attribute[MobileAttribute.UserId]);
                        try
                        {
                            if (cA.ID == cB.ID)
                            {
                                correct++;
                            }
                        }
                        catch (Exception)
                        {
                            error++;
                        }
                        break;
                    case MobileLabel.FRIEND:
                        FriendInfo info;
                        info = friendInfo.Find(x => ((x.ID_A == (Int64)e.From.Attribute[MobileAttribute.UserId]) && (x.ID_B == (Int64)e.To.Attribute[MobileAttribute.UserId])));
                        if (info != null)
                        {
                            correct++;
                        }
                        else
                        {
                            error++;
                        }
                        break;
                    default:
                        break;
                }
            }

            Console.WriteLine("Edge label check: {0} {1}", correct, error);*/
        }

        void BuildGraph()
        {
            List<Int64> UserIds = dataset.InquiryDataWithMethod(new ArrayList(), MobileDataset.FetchAllUserId);
            Dictionary<Int64, uint> nodeIdMapping = new Dictionary<Int64, uint>();
            foreach (Int64 id in UserIds)
            {
                Node v = base.AddNode();
                v.Attribute[MobileAttribute.UserId] = id;
                nodeIdMapping[id] = v.Id;
            }

            List<FamilyInfo> familyInfo = dataset.InquiryDataWithMethod(new ArrayList(), MobileDataset.FetchAllFamilyInfo);
            List<ColleagueInfo> colleagueInfo = dataset.InquiryDataWithMethod(new ArrayList(), MobileDataset.FetchAllColleagueInfo);
            List<FriendInfo> friendInfo = dataset.InquiryDataWithMethod(new ArrayList(), MobileDataset.FetchAllFriendInfo);

            foreach (Int64 fromId in UserIds)
            {
                Node fromNode = base.FindNode(nodeIdMapping[fromId]);
                List<CallInfo> callInfo_from = dataset.InquiryDataWithMethod(new ArrayList { fromId }, MobileDataset.FetchCallInfoList_From);
                List<CallInfo> callInfo_to = dataset.InquiryDataWithMethod(new ArrayList { fromId }, MobileDataset.FetchCallInfoList_To);
                List<MessageInfo> msgInfo_from = dataset.InquiryDataWithMethod(new ArrayList { fromId }, MobileDataset.FetchMessageInfoList_From);
                List<MessageInfo> msgInfo_to = dataset.InquiryDataWithMethod(new ArrayList { fromId }, MobileDataset.FetchMessageInfoList_To);
                
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
                    Edge e = base.AddEdge(nodeIdMapping[fromId], nodeIdMapping[toId], MobileLabel.NORELATION);
                    if (friendInfo.Exists(x => (Int64)e.From.Attribute[MobileAttribute.UserId] == x.ID_A && (Int64)e.To.Attribute[MobileAttribute.UserId] == x.ID_B))
                    {
                        e.Label = MobileLabel.FRIEND;
                    }
                }
            }

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
            base.AddUnitaryFactor(MobileFactor.EDGE_CALLMSGCOUNT, MobileFactorFunctionGenerator.Edge_CallMsgCount, Layer.EdgeLayer, false);
            foreach (var factor in MobileFactorFunctionGenerator.Gen_Edge_CallMsgFrequency_Hour())
            {
                base.AddUnitaryFactor(factor.Key, factor.Value, Layer.EdgeLayer, false);
            }
        }

        MobileDataset dataset;
    }
}
