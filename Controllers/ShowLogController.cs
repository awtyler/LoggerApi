using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using log_webapi.Model;

namespace log_webapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShowLogController : ControllerBase
    {
        private readonly ILogger<LogController> _logger;

        public ShowLogController(ILogger<LogController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{log}")]
        public String Get(String log)
        {

            var pft = "";

            if(LogController.history.Keys.Contains(log)) {
                var entries = LogController.history[log];

                if(entries.Count == 0) {
                    pft = "No log entries";
                } else {

                    entries.Sort();

                    foreach(LogEntry entry in entries) {
                        pft += entry.ToString(log) + "\n";
                    }
                }
            } else {
                pft = "No log";
            }

            return pft;
        }
    }
}

