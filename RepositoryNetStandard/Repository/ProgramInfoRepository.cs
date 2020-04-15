using Advantech.Entity;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Advantech.Repository
{
    public class ProgramInfoRepository : BaseRepository<ProgramInfo>, IProgramInfoRepository
    {
        public ProgramInfoRepository(ISqlSugarClient sqlSugarClient) : base(sqlSugarClient)
        {

        }
    }
}
