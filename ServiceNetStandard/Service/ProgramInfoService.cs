using Advantech.Entity;
using Advantech.Repository;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.Service
{
    public class ProgramInfoService : ProgramInfoRepository, IProgramInfoService
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        public ProgramInfoService(ISqlSugarClient sqlSugarClient):base(sqlSugarClient)
        {
            this._sqlSugarClient = sqlSugarClient;
        }
        /// <summary>
        /// 通过id获取一个复合对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ProgramInfo GetCompositeById(int id)
        {
            ProgramInfo programInfo = this.QueryableToEntity(x=>x.id==id);
            if(programInfo!=null)
            {
                programInfo.RegionList = _sqlSugarClient.Queryable<ProgramRegion>().Where(x => x.program_id == id).ToList();
                if(programInfo.RegionList !=null)
                {
                    foreach(var region in programInfo.RegionList)
                    {
                        region.MediaList= _sqlSugarClient.Queryable<ProgramRegionMedia>().Where(x => x.region_id == region.id).ToList();
                    }
                }
            }
            return programInfo;
        }
    }
}
