using Advantech.Repository;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.Service
{
    public class MediaInfoService : MediaInfoRepository, IMediaInfoService
    {
        private readonly SqlSugarClient _sqlSugarClient;

        public MediaInfoService(SqlSugarClient sqlSugarClient):base(sqlSugarClient)
        {
            this._sqlSugarClient = sqlSugarClient;
        }
    }
}
