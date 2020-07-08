using LiteDB;
using System;

namespace RetroGameHandler.Entities
{
    public class ErrorEntity : EntityBase
    {
        public ErrorEntity()
        {

        }
        public ErrorEntity(string Message, ErrorLevel ErrorLevel, string Error = null, string StackTrace = null)
        {
            Id = new ObjectId();
            this.Message = Message;
            this.ErrorLevel = ErrorLevel;
            this.Error = Error;
            this.StackTrace = StackTrace;
        }

        [BsonId]
        public ObjectId Id { get; set; }

        public DateTime DateTime { get; set; } = DateTime.Now;
        public ErrorLevel ErrorLevel { get; set; }
        public string Message { get; set; }
        public string Error { get; set; } = null;
        public string StackTrace { get; set; } = null;

        public string ToString(string format = "yyyy-MM-dd HH:mm:ss", IFormatProvider formatProvider = null)
        {
            return $"{DateTime.ToString(format, formatProvider)},{Message},{ErrorLevel},{Error},{StackTrace}";
        }
    }
}