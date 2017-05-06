﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MLFactorGraph;
using ResearchUtils;
using System.Collections;

namespace MobileMLFactorGraph
{
    public class MobileDataset : Dataset
    {
        public MobileDataset(DatabaseConnection dataSource)
            : base(dataSource, true)
        {
            base.RegisterMethod(GetCellInfo);
            base.RegisterMethod(GetCellAdjacency);
            base.RegisterMethod(GetPoiInfo);
            base.RegisterMethod(GetFamilyInfo);
            base.RegisterMethod(GetFamilyInfoList);
            base.RegisterMethod(GetColleagueInfo);
            base.RegisterMethod(GetColleagueInfoList);
            base.RegisterMethod(GetFriendInfo);
            base.RegisterMethod(GetFriendInfoList);
            base.RegisterMethod(GetCallInfoList);
            base.RegisterMethod(GetMessageInfoList);
            base.RegisterMethod(GetUserInfo);
            base.RegisterMethod(GetUserEvent);
            base.RegisterMethod(GetUserInteraction);
            base.RegisterMethod(GetUserOD);
        }

        #region Basic Get Methods - Registered
        public CellInfo GetCellInfo(DatabaseConnection dataSource, ArrayList info)
        {
            try
            {
                int cellId = (int)info[0];
                string query = "SELECT * FROM " + MobileTableName.CellInfo +
                    " WHERE CELL_ID = " + cellId;
                return dataSource.PollingExecuteQuery_ObjectResult<CellInfo>(query)[0];
            }
            catch (InvalidCastException)
            {
            }
            return null;
        }
        public CellAdjacency GetCellAdjacency(DatabaseConnection dataSource, ArrayList info)
        {
            try
            {
                int cellAId = (int)info[0];
                int cellBId = (int)info[1];
                string query = "SELECT * FROM " + MobileTableName.CellAdjacency +
                    " WHERE CELL_A = " + cellAId + "AND CELL_B = " + cellBId;
                return dataSource.PollingExecuteQuery_ObjectResult<CellAdjacency>(query)[0];
            }
            catch (InvalidCastException)
            {
            }
            return null;
        }
        public PoiInfo GetPoiInfo(DatabaseConnection dataSource, ArrayList info)
        {
            try
            {
                int cellId = (int)info[0];
                string query = "SELECT * FROM " + MobileTableName.PoiInfo +
                    " WHERE CELL_ID = " + cellId;
                return dataSource.PollingExecuteQuery_ObjectResult<PoiInfo>(query)[0];
            }
            catch (InvalidCastException)
            {
            }
            return null;
        }
        public FamilyInfo GetFamilyInfo(DatabaseConnection dataSource, ArrayList info)
        {
            try
            {
                string Id = (string)info[0];
                Int64 memberId = (Int64)info[1];
                string query = "SELECT * FROM " + MobileTableName.FamilyInfo +
                    " WHERE ID = '" + Id + "' AND MEMBER_ID = " + memberId;
                return dataSource.PollingExecuteQuery_ObjectResult<FamilyInfo>(query)[0];
            }
            catch (InvalidCastException)
            {
            }
            return null;
        }
        public List<FamilyInfo> GetFamilyInfoList(DatabaseConnection dataSource, ArrayList info)
        {
            try
            {
                string Id = (string)info[0];
                string query = "SELECT * FROM " + MobileTableName.FamilyInfo +
                    " WHERE ID = '" + Id + "'";
                return dataSource.PollingExecuteQuery_ObjectResult<FamilyInfo>(query);
            }
            catch (InvalidCastException)
            {
            }
            return null;
        }
        public ColleagueInfo GetColleagueInfo(DatabaseConnection dataSource, ArrayList info)
        {
            try
            {
                string Id = (string)info[0];
                Int64 memberId = (Int64)info[1];
                string query = "SELECT * FROM " + MobileTableName.ColleagueInfo +
                    " WHERE ID = '" + Id + "' AND MEMBER_ID = " + memberId;
                return dataSource.PollingExecuteQuery_ObjectResult<ColleagueInfo>(query)[0];
            }
            catch (InvalidCastException)
            {
            }
            return null;
        }
        public List<ColleagueInfo> GetColleagueInfoList(DatabaseConnection dataSource, ArrayList info)
        {
            try
            {
                string Id = (string)info[0];
                string query = "SELECT * FROM " + MobileTableName.ColleagueInfo +
                    " WHERE ID = '" + Id + "'";
                return dataSource.PollingExecuteQuery_ObjectResult<ColleagueInfo>(query);
            }
            catch (InvalidCastException)
            {
            }
            return null;
        }
        public FriendInfo GetFriendInfo(DatabaseConnection dataSource, ArrayList info)
        {
            try
            {
                Int64 IdA = (Int64)info[0];
                Int64 IdB = (Int64)info[1];
                string query = "SELECT * FROM " + MobileTableName.FriendInfo +
                    " WHERE ID_A = " + IdA + " AND ID_B = " + IdB;
                return dataSource.PollingExecuteQuery_ObjectResult<FriendInfo>(query)[0];
            }
            catch (InvalidCastException)
            {
            }
            return null;
        }
        public List<FriendInfo> GetFriendInfoList(DatabaseConnection dataSource, ArrayList info)
        {
            try
            {
                Int64 IdA = (Int64)info[0];
                string query = "SELECT * FROM " + MobileTableName.FriendInfo +
                    " WHERE ID_A = " + IdA;
                return dataSource.PollingExecuteQuery_ObjectResult<FriendInfo>(query);
            }
            catch (InvalidCastException)
            {
            }
            return null;
        }
        public List<CallInfo> GetCallInfoList(DatabaseConnection dataSource, ArrayList info)
        {
            try
            {
                Int64 callFrom = (Int64)info[0];
                Int64 callTo = (Int64)info[1];
                string query = "SELECT * FROM " + MobileTableName.CallInfo +
                    " WHERE CALL_FROM = " + callFrom + " AND CALL_TO = " + callTo;
                return dataSource.PollingExecuteQuery_ObjectResult<CallInfo>(query);
            }
            catch (InvalidCastException)
            {
            }
            return null;
        }
        public List<MessageInfo> GetMessageInfoList(DatabaseConnection dataSource, ArrayList info)
        {
            try
            {
                Int64 msgFrom = (Int64)info[0];
                Int64 msgTo = (Int64)info[1];
                string query = "SELECT * FROM " + MobileTableName.MessageInfo +
                    " WHERE MSG_FROM = " + msgFrom + " AND MSG_TO = " + msgTo;
                return dataSource.PollingExecuteQuery_ObjectResult<MessageInfo>(query);
            }
            catch (InvalidCastException)
            {
            }
            return null;
        }
        public UserInfo GetUserInfo(DatabaseConnection dataSource, ArrayList info)
        {
            try
            {
                Int64 Id = (Int64)info[0];
                string query = "SELECT * FROM " + MobileTableName.UserInfo +
                    " WHERE ID = " + Id;
                return dataSource.PollingExecuteQuery_ObjectResult<UserInfo>(query)[0];
            }
            catch (InvalidCastException)
            {
            }
            return null;
        }
        public UserEvent GetUserEvent(DatabaseConnection dataSource, ArrayList info)
        {
            try
            {
                Int64 Id = (Int64)info[0];
                string query = "SELECT * FROM " + MobileTableName.UserEvent +
                    " WHERE ID = " + Id;
                return dataSource.PollingExecuteQuery_ObjectResult<UserEvent>(query)[0];
            }
            catch (InvalidCastException)
            {
            }
            return null;
        }
        public UserInteraction GetUserInteraction(DatabaseConnection dataSource, ArrayList info)
        {
            try
            {
                Int64 Id = (Int64)info[0];
                string query = "SELECT * FROM " + MobileTableName.UserInteraction +
                    " WHERE ID = " + Id;
                return dataSource.PollingExecuteQuery_ObjectResult<UserInteraction>(query)[0];
            }
            catch (InvalidCastException)
            {
            }
            return null;
        }
        public UserOD GetUserOD(DatabaseConnection dataSource, ArrayList info)
        {
            try
            {
                Int64 Id = (Int64)info[0];
                string query = "SELECT * FROM " + MobileTableName.UserOD +
                    " WHERE ID = " + Id;
                return dataSource.PollingExecuteQuery_ObjectResult<UserOD>(query)[0];
            }
            catch (InvalidCastException)
            {
            }
            return null;
        }
        #endregion

        #region Specified Fetch Methods - Assign when use
        public List<CallInfo> FetchCallInfoList_From(DatabaseConnection dataSource, ArrayList info)
        {
            try
            {
                Int64 callFrom = (Int64)info[0];
                string query = "SELECT * FROM " + MobileTableName.CallInfo +
                    " WHERE CALL_FROM = " + callFrom;
                return dataSource.PollingExecuteQuery_ObjectResult<CallInfo>(query);
            }
            catch (InvalidCastException)
            {
            }
            return null;
        }
        public List<CallInfo> FetchCallInfoList_To(DatabaseConnection dataSource, ArrayList info)
        {
            try
            {
                Int64 callTo = (Int64)info[0];
                string query = "SELECT * FROM " + MobileTableName.CallInfo +
                    " WHERE CALL_TO = " + callTo;
                return dataSource.PollingExecuteQuery_ObjectResult<CallInfo>(query);
            }
            catch (InvalidCastException)
            {
            }
            return null;
        }
        public List<MessageInfo> FetchMessageInfoList_From(DatabaseConnection dataSource, ArrayList info)
        {
            try
            {
                Int64 msgFrom = (Int64)info[0];
                string query = "SELECT * FROM " + MobileTableName.MessageInfo +
                    " WHERE MSG_FROM = " + msgFrom;
                return dataSource.PollingExecuteQuery_ObjectResult<MessageInfo>(query);
            }
            catch (InvalidCastException)
            {
            }
            return null;
        }
        public List<MessageInfo> FetchMessageInfoList_To(DatabaseConnection dataSource, ArrayList info)
        {
            try
            {
                Int64 msgTo = (Int64)info[0];
                string query = "SELECT * FROM " + MobileTableName.MessageInfo +
                    " WHERE MSG_TO = " + msgTo;
                return dataSource.PollingExecuteQuery_ObjectResult<MessageInfo>(query);
            }
            catch (InvalidCastException)
            {
            }
            return null;
        }
        public List<Int64> FetchAllUserId(DatabaseConnection dataSource, ArrayList info)
        {
            string query = "SELECT * FROM " + MobileTableName.UserInfo;
            return dataSource.PollingExecuteQuery_ObjectResult<UserInfo>(query)
                .Select(x => x.ID)
                .ToList();
        }
        public List<FamilyInfo> FetchAllFamilyInfo(DatabaseConnection dataSource, ArrayList info)
        {
            string query = "SELECT * FROM " + MobileTableName.FamilyInfo;
            return dataSource.PollingExecuteQuery_ObjectResult<FamilyInfo>(query);
        }
        public List<ColleagueInfo> FetchAllColleagueInfo(DatabaseConnection dataSource, ArrayList info)
        {
            string query = "SELECT * FROM " + MobileTableName.ColleagueInfo;
            return dataSource.PollingExecuteQuery_ObjectResult<ColleagueInfo>(query);
        }
        public List<FriendInfo> FetchAllFriendInfo(DatabaseConnection dataSource, ArrayList info)
        {
            string query = "SELECT * FROM " + MobileTableName.FriendInfo;
            return dataSource.PollingExecuteQuery_ObjectResult<FriendInfo>(query);
        }
        #endregion
    }
}
