using Advantech.Entity;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Advantech.Repository
{
    public class UserRepository : BaseRepository<Users>,IBaseRepository<Users>
    {
        public UserRepository(SqlSugarClient sqlSugarClient) : base(sqlSugarClient)
        {

        }
    }
}
