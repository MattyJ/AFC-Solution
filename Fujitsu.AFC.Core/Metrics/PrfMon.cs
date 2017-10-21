using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujitsu.AFC.Core.Metrics
{
    [ExcludeFromCodeCoverage]
    public class PrfMon
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public PrfMon()
        {
            _stopwatch.Start();
        }

        public double Stop()
        {
            _stopwatch.Stop();
            return _stopwatch.Elapsed.TotalSeconds;
        }

        public string StopString()
        {
            _stopwatch.Stop();
            return _stopwatch.Elapsed.TotalSeconds.ToString("0.000");
        }
    }
}
