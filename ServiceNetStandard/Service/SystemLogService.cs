using Advantech.Entity.SysLog;
using Advantech.Repository;
using Advantech.UtilsStandard.Interface;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advantech.Service
{
    public class SystemLogService : SystemLogRepository, ISystemLogService
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        public SystemLogService(ISqlSugarClient sqlSugarClient):base(sqlSugarClient)
        {
            this._sqlSugarClient = sqlSugarClient;
        }
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="content"></param>
        /// <param name="ex"></param>
        public void WriteLog(string content, Exception ex)
        {
            SystemLog systemLog = new SystemLog();
            systemLog.content = ex.Message;
            systemLog.source = content;
            systemLog.create_time = DateTime.Now;
            this.Insert(systemLog);
        }
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="content"></param>
        public void WriteLog(string content)
        {
            SystemLog systemLog = new SystemLog();
            systemLog.content = content;
            systemLog.create_time = DateTime.Now;
            this.Insert(systemLog);
        }

        public void WriteLog(string source, string subject, string content)
        {
            SystemLog systemLog = new SystemLog();
            systemLog.source = source;
            systemLog.subject = subject;
            systemLog.content = content;
            systemLog.create_time = DateTime.Now;
            this.Insert(systemLog);
        }
    }
}
