using Advantech.Entity;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Advantech.Repository
{
    public class ProgramRegionRepository : BaseRepository<ProgramRegion>, IProgramRegionRepository
    {
        public ProgramRegionRepository(ISqlSugarClient sqlSugarClient) : base(sqlSugarClient)
        {

        }
    }
}
