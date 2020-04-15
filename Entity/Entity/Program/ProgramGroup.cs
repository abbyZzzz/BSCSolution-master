using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.Entity
{
    [SugarTable("program_group")]
    public class ProgramGroup
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }

        public int parent_id { get; set; }

        public string pgroup_name { get; set; }

        public int group_id { get; set; }

        public DateTime create_time { get; set; } = DateTime.Now;
    }
}
