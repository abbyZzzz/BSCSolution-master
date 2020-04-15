using Advantech.Repository;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.Service
{
    public class ProgramRegionMediaService : ProgramRegionMediaRepository, IProgramRegionMediaService
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        public ProgramRegionMediaService(ISqlSugarClient sqlSugarClient):base(sqlSugarClient)
        {
            this._sqlSugarClient = sqlSugarClient;
        }
    }
}
