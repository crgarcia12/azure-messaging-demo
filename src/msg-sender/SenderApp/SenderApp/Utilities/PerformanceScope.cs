using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SenderApp
{
    public class PerformanceScope : IDisposable
    {
        private readonly string _operationName;
        private readonly ILogger _logger;
        private readonly Stopwatch _timer;
        private readonly IDisposable _scope;

        public PerformanceScope (ILogger logger, string scopeName)
        {
            _operationName = scopeName;
            _logger = logger;
            _timer = Stopwatch.StartNew();
            _scope = _logger.BeginScope(scopeName);
        }

        public void Dispose()
        {
            _logger.LogInformation("OperationName took ms " + _timer.ElapsedMilliseconds);
            _scope.Dispose();
        }
    }
}
