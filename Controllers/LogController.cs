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
    public class LogController : ControllerBase
    {
        public static Dictionary<String, List<String>> history = new Dictionary<String, List<String>>();

        private readonly ILogger<LogController> _logger;

        public LogController(ILogger<LogController> logger)
        {
            _logger = logger;
	    if(!history.Keys.Contains("default")) history["default"] = new List<String>();
	}

        [HttpPost]
        public String Post([FromBody] String text, String log = "default") {
            try {
                var logText = $"[{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}] [{log}] {text}";
                Console.WriteLine(logText);

                if(!history.Keys.Contains(log)) history.Add(log, new List<String>());
                history[log].Add(logText);
                return "{ \"success\": true, \"message\": \"Logged\" }";
            } catch(Exception ex) {
                return "{ \"success\": false, \"message\": \"" + ex.Message + "\" }";
            }
        }

        [HttpGet]
        public List<String> Get(String log = "default")
        {
            var items = history[log];
            return items;
        }

	[HttpGet]
	[Route("LogNames")]
	public List<String> GetLogNames() {
		return history.Keys.ToList();
	}

	[HttpGet]
	[Route("Clear")]
	public String GetClear(String log = "default")
	{
	    try {
		history[log] = new List<String>();
		return "{ \"success\": true, \"message\": \"Cleared\" }";
	    } catch(Exception ex) {
		return "{ \"success\": false, \"message\": \"" + ex.Message + "\" }";
	    }
	}
    }
}

