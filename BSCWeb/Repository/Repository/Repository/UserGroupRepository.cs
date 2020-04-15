using Advantech.Entity;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Advantech.Repository
{
    public class UserGroupRepository : BaseRepository<UserGroup>,IBaseRepository<UserGroup>
    {
        public UserGroupRepository(SqlSugarClient sqlSugarClient) : base(sqlSugarClient)
        {

        }
    }
}
