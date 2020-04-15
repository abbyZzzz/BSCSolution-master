using Advantech.Entity;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Advantech.Repository
{
    public class MediaInfoRepository : BaseRepository<MediaInfo>,IBaseRepository<MediaInfo>
    {
        public MediaInfoRepository(SqlSugarClient sqlSugarClient) : base(sqlSugarClient)
        {

        }
    }
}
