using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using log_webapi.Model;

namespace log_webapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogController : ControllerBase
    {
        public static Dictionary<String, List<LogEntry>> history = new Dictionary<String, List<LogEntry>>();

        private readonly ILogger<LogController> _logger;
        private readonly IOptions<LoggerConfiguration> _config;

        public LogController(ILogger<LogController> logger, IOptions<LoggerConfiguration> config)
        {
            _logger = logger;
            _config = config;
	        if(!history.Keys.Contains("default")) history["default"] = new List<LogEntry>();
	    }

        [HttpPost("{log}")]
        public String Post([FromBody] LogEntry entry, String log = "default") {
            // entry = new LogEntry(entry);
            try {
                Console.WriteLine("Entry: " + entry != null ? entry.ToString(log) : "null");

                if(!history.Keys.Contains(log)) history.Add(log, new List<LogEntry>());
                history[log].Add(entry);
                return "{ \"success\": true, \"message\": \"Logged\" }";
            } catch(Exception ex) {
                return "{ \"success\": false, \"message\": \"" + ex.Message + "\" }";
            }
        }

        [HttpGet("{log}")]
        public List<String> GetAsString(String log = "default")
        {
            var items = history[log];

            items.Sort(delegate(LogEntry a, LogEntry b) {
                return (a.date).CompareTo(b.date);
            });

            var output = new List<String>();
            foreach(LogEntry entry in items) {
                output.Add(entry.ToString(log));
            }
            return output;
        }

        [HttpGet("{log}/json")]
        public List<LogEntry> GetAsJson(String log = "default")
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
                System.IO.File.AppendAllLines(Path.Combine(path, filename), history[log].Select(entry => entry.ToString(log)));

            } catch(Exception ex) { 
                System.Console.WriteLine("Error writing file: " + ex.Message);
            }  //No problems - the log just doesn't get saved.


            try {
                history[log] = new List<LogEntry>();
                return "{ \"success\": true, \"message\": \"Cleared\" }";
            } catch(Exception ex) {
                return "{ \"success\": false, \"message\": \"" + ex.Message + "\" }";
            }
        }
    }
}
