using System.Collections.Generic;
using System.Diagnostics;

namespace OA.Core.Diagnostics
{
    public static class Metrics
    {
        static readonly List<NameValuePair> _dataReadList = new List<NameValuePair>();
        public static int TotalDataRead
        {
            get
            {
                int total = 0;
                foreach (var p in _dataReadList)
                    total += p.Value;
                return total;
            }
        }

        static string _dataReadBreakdown;
        static bool _dataReadBreakdown_MustUpdate = true;
        public static string DataReadBreakdown
        {
            get
            {
                if (_dataReadBreakdown_MustUpdate)
                {
                    _dataReadBreakdown_MustUpdate = false;
                    _dataReadBreakdown = "Data Read from HDD:";
                    foreach (var p in _dataReadList)
                        _dataReadBreakdown += '\n' + p.Name + ": " + p.Value;
                }
                return _dataReadBreakdown;
            }
        }

        public static void ReportDataRead(int dataAmount)
        {
            string name;
#if DEBUG
            var stackTrace = new StackTrace();
            var stackFrame = stackTrace.GetFrame(1);
            var methodBase = stackFrame.GetMethod();
            name = methodBase.DeclaringType.FullName;
#else
            name = "Total";
#endif
            var mustAddPair = true;
            foreach (var p in _dataReadList)
            {
                if (p.Name == name)
                {
                    mustAddPair = false;
                    p.Value += dataAmount;
                }
            }
            if (mustAddPair)
                _dataReadList.Add(new NameValuePair(name, dataAmount));
            _dataReadBreakdown_MustUpdate = true;
        }
    }

    class NameValuePair
    {
        public string Name = string.Empty;
        public int Value;

        public NameValuePair(string name, int value)
        {
            Name = name;
            Value = value;
        }
    }
}
