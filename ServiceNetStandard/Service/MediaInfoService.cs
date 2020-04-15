using Advantech.Repository;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.Service
{
    public class MediaInfoService : MediaInfoRepository, IMediaInfoService
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        public MediaInfoService(ISqlSugarClient sqlSugarClient):base(sqlSugarClient)
        {
            this._sqlSugarClient = sqlSugarClient;
        }
    }
}
