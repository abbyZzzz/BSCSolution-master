using Advantech.Repository;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.Service
{
    public class MediaGroupService : MediaGroupRepository, IMediaGroupService
    {
        private readonly SqlSugarClient _sqlSugarClient;

        public MediaGroupService(SqlSugarClient sqlSugarClient):base(sqlSugarClient)
        {
            this._sqlSugarClient = sqlSugarClient;
        }
    }
}
