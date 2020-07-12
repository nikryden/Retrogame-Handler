using LiteDB;
using Renci.SshNet.Messages;
using RetroGameHandler.Entities;
using RetroGameHandler.TimeOnline;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RetroGameHandler.Handlers
{
    public static class ErrorHandler
    {
        //LiteDBHelper.Load<DataModel>().FirstOrDefault() ?? new DataModel() { Id = 1, ScrapGuid = "", ScrapEmail = "" };

        public static ILiteQueryable<ErrorEntity> GetAllErrors() => LiteDBHelper.Load<ErrorEntity>();

        public static void ClearLog()
        {
            LiteDBHelper.Instance.ClearCollection<ErrorEntity>();
        }

        public static ILiteQueryable<ErrorEntity> GetErrors(ErrorLevel? ErrorLevel = null,
                                                    DateTime? StartDateTime = null,
                                                    DateTime? EndDateTime = null,
                                                    string ValueInMessage = null,
                                                    string ValueInError = null,
                                                    string ValueInStackTrace = null)
        {
            return LiteDBHelper.Load<ErrorEntity>().Where(e =>
                        (!ErrorLevel.HasValue || (ErrorLevel.HasValue && e.ErrorLevel == ErrorLevel)) &&
                        (!StartDateTime.HasValue || (StartDateTime.HasValue && e.DateTime >= StartDateTime)) &&
                        (!EndDateTime.HasValue || (EndDateTime.HasValue && e.DateTime <= EndDateTime)) &&
                        (
                            (ValueInMessage == null || (ValueInMessage != null && e.Message.Contains(ValueInMessage))) ||
                            (ValueInError == null || (ValueInError != null && e.Error.Contains(ValueInError))) ||
                            (ValueInStackTrace == null || (ValueInStackTrace != null && e.StackTrace.Contains(ValueInStackTrace)))
                        )
                        );
        }

        public static void Info(string Message)
        {
            Log(new ErrorEntity(Message, ErrorLevel.INFO));
        }

        public static void Debug(string Message)
        {
            Log(new ErrorEntity(Message, ErrorLevel.DEBUG));
        }

        public static void Warning(string Message)
        {
            Log(new ErrorEntity(Message, ErrorLevel.WARN));
        }

        public static void Trace(string Message)
        {
            StackTrace stackTrace = new StackTrace();
            var Caller = stackTrace.GetFrame(1);
            var method = Caller.GetMethod();
            string err = $"{Caller.GetFileLineNumber()}:{Caller.GetFileColumnNumber()},{method.DeclaringType.Name}:{method.Name}()";
            Log(new ErrorEntity(Message, ErrorLevel.WARN, err));
        }

        public static void Error(Exception exception)
        {
            System.Diagnostics.Debug.WriteLine(exception);
            Log(new ErrorEntity(exception.Message, ErrorLevel.ERROR, exception.ToString(), exception.StackTrace));
        }

        public static void Error(string Message, ErrorLevel ErrorLevel, string Error = null, string StackTrace = null)
        {
            Log(new ErrorEntity(Message, ErrorLevel, Error, StackTrace));
        }

        private static void Log(ErrorEntity errorEntity)
        {
            System.Diagnostics.Debug.WriteLine(errorEntity.ToString());
            if (RGHSettings.LogLevel >= errorEntity.ErrorLevel) LiteDBHelper.Save(errorEntity);
        }
    }
}