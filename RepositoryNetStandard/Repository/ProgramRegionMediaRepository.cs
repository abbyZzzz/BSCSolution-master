using Advantech.Entity;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Advantech.Repository
{
    public class ProgramRegionMediaRepository : BaseRepository<ProgramRegionMedia>, IProgramRegionMediaRepository
    {
        public ProgramRegionMediaRepository(ISqlSugarClient sqlSugarClient) : base(sqlSugarClient)
        {

        }
    }
}
