using Advantech.Repository;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.Service
{
    public class ProgramGroupService : ProgramGroupRepository, IProgramGroupService
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        public ProgramGroupService(ISqlSugarClient sqlSugarClient):base(sqlSugarClient)
        {
            this._sqlSugarClient = sqlSugarClient;
        }
    }
}
