using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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

        [HttpGet]
        public String Get(String log)
        {

            var pft = "";

            if(LogController.history.Keys.Contains(log)) {
                var history = LogController.history[log];

                if(history.Count == 0) {
                    pft = "No log entries";
                } else {
                    for(int i = 0; i < history.Count; i++) {
                        pft += history[i] + "\n";
                    }
                }
            } else {
                pft = "No log";
            }

            return pft;
        }
    }
}

