using System;
using System.Collections.Generic;

namespace log_webapi.Model {

	public class LogEntry {
		public DateTime date { get; set; }
		public String text { get; set; }

		public String ToString(String logName) {
			return $"[{date.ToString("yyyy-MM-dd HH:mm:ss.fffff")}] [{logName}] {text}";
		}

		public LogEntry() { 
			date = DateTime.Now;
		}
	}
}