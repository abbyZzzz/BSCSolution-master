using Advantech.Repository;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.Service
{
    public class MediaGroupService : MediaGroupRepository, IMediaGroupService
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        public MediaGroupService(ISqlSugarClient sqlSugarClient):base(sqlSugarClient)
        {
            this._sqlSugarClient = sqlSugarClient;
        }
    }
}
