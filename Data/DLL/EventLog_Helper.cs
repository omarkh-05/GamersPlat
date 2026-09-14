using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Data.DLL
{
    public class EventLog_Helper
    {
        public static void WriteEventLog(string title, Exception ex)
        {
            string error = ex.Message;
            if (ex.InnerException != null)
                error += "\nInner Exception: " + ex.InnerException.Message;

            EventLog.WriteEntry("Application", $"{title}: {error}", EventLogEntryType.Error);
        }
    }
}
