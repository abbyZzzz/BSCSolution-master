using Advantech.Repository;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.Service
{
    public class UserGroupService : UserGroupRepository, IUserGroupService
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        public UserGroupService(ISqlSugarClient sqlSugarClient):base(sqlSugarClient)
        {
            this._sqlSugarClient = sqlSugarClient;
        }
    }
}
