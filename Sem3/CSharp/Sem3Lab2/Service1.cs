using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sem3Lab2
{
	public partial class Service1 : ServiceBase
	{
		public Service1 ()
		{
			InitializeComponent ();
		}

		protected override void OnStart (string[] args)
		{
			Program.extractor = new DirectoryFileExtractor
			(
				new DirectoryFileExtractorSettings
				(
					new DirectoryInfo ("D:\\in"),
					new DirectoryInfo ("D:\\out"),
					".txt",
					new GZipFileSettings (CompressionLevel.Optimal, ".zip"),
					new AesFileSettings
					(
						"tmp.bin",
						260,
						new StreamAesCryptorSettings (Program.k, Program.v)
					)
				),
				Program.ExtractorLog
			);
			Program.extractor.StartContinue ();
		}

		protected override void OnContinue ()
		{
			base.OnContinue ();
			Program.extractor.StartContinue ();
		}

		protected override void OnPause ()
		{
			base.OnPause ();
			Program.extractor.Pause ();
		}

		protected override void OnStop ()
		{
			Program.extractor.Stop ();
		}
	}
}
