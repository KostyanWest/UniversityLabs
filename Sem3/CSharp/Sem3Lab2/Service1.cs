using System;
using System.IO;
using System.ServiceProcess;
using System.Text;
using Sem3Lab3;

namespace Sem3Lab2
{
	public partial class Service1 : ServiceBase
	{
		private string logPath;
		private DirectoryFileExtractor extractor;

		public Service1 ()
		{
			InitializeComponent ();
		}

		public void Start () => OnStart (null);
		public new void Stop () => OnPause ();

		protected override void OnStart (string[] args)
		{
			logPath = Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "log.txt");
			extractor = new DirectoryFileExtractor
			(
				ConfigReader.GetOptions<DirectoryFileExtractorSettings>
				(
					AppDomain.CurrentDomain.BaseDirectory, "config*", ConfigLog
				),
				ExtractorLog
			);
			extractor.StartContinue ();
		}

		protected override void OnContinue ()
		{
			base.OnContinue ();
			extractor.StartContinue ();
		}

		protected override void OnPause ()
		{
			base.OnPause ();
			extractor.Pause ();
		}

		protected override void OnStop ()
		{
			extractor.Stop ();
		}

		private void ExtractorLog (string s)
		{
			try
			{
				using (StreamWriter writer = new StreamWriter (logPath, true, Encoding.UTF8))
				{
					writer.WriteLine ($"{DateTime.Now}\nExtractor:\n{s}\n");
				}
			}
			catch { }
		}

		private void ConfigLog (string s)
		{
			try
			{
				using (StreamWriter writer = new StreamWriter (logPath, true, Encoding.UTF8))
				{
					writer.WriteLine ($"{DateTime.Now}\nConfigReader:\n{s}\n");
				}
			}
			catch { }
		}
	}
}
