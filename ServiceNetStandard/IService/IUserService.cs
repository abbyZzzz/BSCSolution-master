﻿using Advantech.Entity.UserAndGroup;
using Advantech.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.Service
{
    public interface IUserService : IBaseRepository<UserInfo>
    {
    }
}
