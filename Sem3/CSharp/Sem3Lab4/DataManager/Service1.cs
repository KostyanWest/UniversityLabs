using System;
using System.ServiceProcess;
using Sem3Lab3;
using Sem3Lab4.ServiceLayer;

namespace Sem3Lab4.DataManager
{
	public partial class Service1 : ServiceBase
	{
		private DataTransfer transfer;

		public Service1 ()
		{
			InitializeComponent ();
		}

		protected override void OnStart (string[] args)
		{
			transfer = new DataTransfer
			(
				ConfigReader.GetOptions<DataTransferSettings>
				(
					AppDomain.CurrentDomain.BaseDirectory, "config*", null
				)
			);
			transfer.Transfer ();
		}

		protected override void OnStop ()
		{
		}

		protected override void OnCustomCommand (int command)
		{
			base.OnCustomCommand (command);
			transfer.Transfer ();
		}
	}
}
