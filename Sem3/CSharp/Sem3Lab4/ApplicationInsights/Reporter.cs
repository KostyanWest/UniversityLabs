using System;
using Sem3Lab4.DataAccess;
using Sem3Lab4.Models;

namespace Sem3Lab4.ApplicationInsights
{
	public class Reporter
	{
		private AccessorSettings accessorSettings;

		public Reporter (AccessorSettings settings)
		{
			accessorSettings = settings;
		}

		public void Report (Exception ex)
		{
			using (Accessor accessor = new Accessor(accessorSettings))
			{
				Report (ex, accessor);
			}
		}

		private int? Report (Exception ex, Accessor accessor)
		{
			int? inner = (ex.InnerException != null) ? Report (ex.InnerException, accessor) : null;
			InsertElementParams insertElementParams = new InsertElementParams {
				ProcedureName = "uspInsertReport",
				Data = new InsertReportParams {
					ExceptionType = ex.GetType ().ToString (),
					Message = (ex.Message?.Length > 100) ? ex.Message?.Substring (0, 100) : ex?.Message,
					TargetSite = (ex.TargetSite?.ToString ().Length > 100) ? ex.TargetSite?.ToString ().Substring (0, 100) : ex.TargetSite?.ToString (),
					StackTrace = (ex.StackTrace?.Length > 100) ? ex.StackTrace?.Substring (0, 100) : ex.StackTrace,
					InnerException = inner,
					Date = DateTime.Now,
				}
			};
			object result = accessor.InsertElement (insertElementParams);
			return (result.GetType () != typeof (DBNull)) ? (int?)(decimal)(result) : null;
		}
	}
}
