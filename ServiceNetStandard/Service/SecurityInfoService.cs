using Advantech.Repository;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.Service
{
    public class SecurityInfoService : SecurityInfoRepository, ISecurityInfoService
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        public SecurityInfoService(ISqlSugarClient sqlSugarClient):base(sqlSugarClient)
        {
            this._sqlSugarClient = sqlSugarClient;
        }
    }
}
