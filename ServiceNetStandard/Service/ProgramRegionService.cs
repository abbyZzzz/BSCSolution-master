using Advantech.Repository;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.Service
{
    public class ProgramRegionService : ProgramRegionRepository, IProgramRegionService
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        public ProgramRegionService(ISqlSugarClient sqlSugarClient):base(sqlSugarClient)
        {
            this._sqlSugarClient = sqlSugarClient;
        }
    }
}
