using System;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Diagnostics;
using Sem3Lab3;

namespace Sem3Lab2
{
	public class Program
	{
		private static string logPath;

		public static DirectoryFileExtractor extractor;

		public static byte[] k = {
			81, 99, 239, 239, 141, 195, 172, 104, 209, 33, 88, 219, 189, 87, 173, 122,
			176, 200, 162, 224, 96, 162, 15, 61, 99, 69, 208, 211, 85, 35, 209, 206
		};
		public static byte[] v = { 196, 220, 130, 86, 174, 224, 70, 5, 129, 134, 233, 132, 29, 245, 24, 20 };

		public static void ExtractorLog (string s)
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

		static void Main (string[] args)
		{
			logPath = Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "log.txt");

			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[]
			{
				new Service1 ()
			};
			ServiceBase.Run (ServicesToRun);
		}
	}
}
