using Advantech.Entity;
using Advantech.Entity.UserAndGroup;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Advantech.Repository
{
    public class SecurityInfoRepository : BaseRepository<SecurityInfo>, ISecurityInfoRepository
    {
        public SecurityInfoRepository(ISqlSugarClient sqlSugarClient) : base(sqlSugarClient)
        {

        }
    }
}
