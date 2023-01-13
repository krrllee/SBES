using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Audit : IDisposable
    {
        private static EventLog customLog = null;
        const string SourceName = "CityGovernment.Audit";
        const string LogName = "GovernmentAudit";

        static Audit()
        {
            try
            {
                if (!EventLog.SourceExists(SourceName))
                {
                    EventLog.CreateEventSource(SourceName, LogName);
                }
                customLog = new EventLog(LogName, Environment.MachineName, SourceName);
            }
            catch (Exception e)
            {
                customLog = null;
                Console.WriteLine("Error while trying to create log handle. Error = {0}", e.Message);
            }
        }

        public static void UserForgiven(string username)
        {
            if (customLog == null)
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.UserForgiven));
            }

            string userForgiven = AuditEvents.UserForgiven;
            string message = String.Format(userForgiven, username);
            customLog.WriteEntry(message);
        }

        public static void UserBanned(string username)
        {
            if (customLog == null)
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.UserBanned));
            }

            string userBanned = AuditEvents.UserBanned;
            string message = String.Format(userBanned, username);
            customLog.WriteEntry(message);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
