using System;

namespace Sem3Lab4.Models
{
	public class InsertReportParams
	{
		public string ExceptionType { get; set; }
		public string Message { get; set; }
		public string TargetSite { get; set; }
		public string StackTrace { get; set; }
		public int? InnerException { get; set; }
		public DateTime? Date { get; set; }
	}
}
