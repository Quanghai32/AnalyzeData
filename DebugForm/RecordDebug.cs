using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugForm
{
    public static class RecordDebug
    {
        static ucLog ucLogger;
        public static void SetLoggerForm(ucLog uc)
        {
            ucLogger = uc;
        }
        public static void Print(string s)
        {
            ucLogger.Print(s);
        }
    }
}
