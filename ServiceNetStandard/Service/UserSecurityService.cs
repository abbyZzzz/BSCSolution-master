using Advantech.Repository;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.Service
{
    public class UserSecurityService : UserSecurityRepository, IUserSecurityService
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        public UserSecurityService(ISqlSugarClient sqlSugarClient):base(sqlSugarClient)
        {
            this._sqlSugarClient = sqlSugarClient;
        }
    }
}
