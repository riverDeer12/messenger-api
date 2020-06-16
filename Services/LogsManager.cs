using MessengerAPI.Data;
using MessengerAPI.Data.Models;
using Microsoft.EntityFrameworkCore;
using Nancy.Json;
using System;

namespace MessengerAPI.Services
{
    public class LogsManager
    {
        private readonly Model _db;

        public LogsManager(Model context)
        {
            _db = context;
        }

        public void SaveLog(object data, string errorMessage)
        {
            var serializer = new JavaScriptSerializer();

            var receivedData = serializer.Serialize(data);

            var newLog = new ApplicationLog
            {
                ApplicationLogId = Guid.NewGuid(),
                IssuedAt = DateTime.Now,
                Data = receivedData,
                ErrorMessage = errorMessage
            };

            _db.Entry(newLog).State = EntityState.Added;
            _db.SaveChanges();
        }
    }
}
