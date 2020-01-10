using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace log_webapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogController : ControllerBase
    {
        public static Dictionary<String, List<String>> history = new Dictionary<String, List<String>>();

        private readonly ILogger<LogController> _logger;
        private readonly IOptions<LoggerConfiguration> _config;

        public LogController(ILogger<LogController> logger, IOptions<LoggerConfiguration> config)
        {
            _logger = logger;
            _config = config;
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
            
                //Write to log file, if a logPath has been set and is accessible
                var path = _config.Value.LogPath;
                var filename = $"{_config.Value.FileNamePrefix}{log}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.log";

                //Remove invalid and special characters and replace with '-'
                var invalidCharacters = new List<char>() {'!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '{', '}', '?', '+', '/', '=', '\\', '"', '\'', ',', '.', '<', '>', '`', '~', ':', ';'};
                invalidCharacters.AddRange(Path.GetInvalidFileNameChars());
                foreach(var chr in invalidCharacters) {
                    filename = filename.Replace(chr, '-');
                }                

                System.IO.Directory.CreateDirectory(path);

                System.Console.WriteLine("Writing to log: " + Path.Combine(path, filename));
                System.IO.File.AppendAllLines(Path.Combine(path, filename), history[log]);

            } catch(Exception ex) { 
                System.Console.WriteLine("Error writing file: " + ex.Message);
            }  //No problems - the log just doesn't get saved.


            try {
                history[log] = new List<String>();
                return "{ \"success\": true, \"message\": \"Cleared\" }";
            } catch(Exception ex) {
                return "{ \"success\": false, \"message\": \"" + ex.Message + "\" }";
            }
        }
    }
}
