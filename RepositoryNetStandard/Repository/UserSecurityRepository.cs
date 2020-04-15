using Advantech.Entity;
using Advantech.Entity.UserAndGroup;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Advantech.Repository
{
    public class UserSecurityRepository : BaseRepository<UserSecurity>, IUserSecurityRepository
    {
        public UserSecurityRepository(ISqlSugarClient sqlSugarClient) : base(sqlSugarClient)
        {

        }
    }
}
