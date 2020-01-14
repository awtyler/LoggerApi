using System;
using System.Collections.Generic;

namespace log_webapi.Model {

	public class LogEntry: IComparable {
		public DateTime date { get; set; }
		public String text { get; set; }

		public String ToString(String logName) {
			return $"[{date.ToString("yyyy-MM-dd HH:mm:ss.fffff")}] [{logName}] {text}";
		}

		public LogEntry() { 
			date = DateTime.Now;
		}

		public int CompareTo(object o) {

			LogEntry otherEntry = o as LogEntry;
			if (otherEntry != null) {
				if(date.CompareTo(otherEntry.date) == 0) {
					return text.CompareTo(otherEntry.text);
				}
				return (date).CompareTo(otherEntry.date);
			}
			else {
				throw new ArgumentException("Object is not a LogEntry");
			}

		}

	}
}