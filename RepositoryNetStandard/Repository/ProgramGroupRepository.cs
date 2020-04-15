using Advantech.Entity;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Advantech.Repository
{
    public class ProgramGroupRepository : BaseRepository<ProgramGroup>, IProgramGroupRepository
    {
        public ProgramGroupRepository(ISqlSugarClient sqlSugarClient) : base(sqlSugarClient)
        {

        }
    }
}
