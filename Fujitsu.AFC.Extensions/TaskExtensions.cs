using System;
using AutoMapper;
using Fujitsu.AFC.Model;

namespace Fujitsu.AFC.Extensions
{
    public static class TaskExtensions
    {
        public static HistoryLog CreateHistoryLog(this Task value, string identity)
        {
            var dateTime = DateTime.Now;
            var historyLog = Mapper.Map<HistoryLog>(value);
            historyLog.CompletedDate = dateTime;
            historyLog.InsertedDate = dateTime;
            historyLog.InsertedBy = identity;
            historyLog.UpdatedDate = dateTime;
            historyLog.UpdatedBy = identity;

            return historyLog;
        }
    }
}
