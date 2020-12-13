using System.ComponentModel;
using System.ServiceProcess;

namespace Sem3Lab4.DataManager
{
	[RunInstaller (true)]
	public partial class Installer1 : System.Configuration.Install.Installer
	{
		ServiceInstaller serviceInstaller;
		ServiceProcessInstaller processInstaller;

		public Installer1 ()
		{
			InitializeComponent ();
			serviceInstaller = new ServiceInstaller ();
			processInstaller = new ServiceProcessInstaller ();

			processInstaller.Account = ServiceAccount.LocalSystem;
			serviceInstaller.StartType = ServiceStartMode.Manual;
			serviceInstaller.ServiceName = "DataManager";
			Installers.Add (processInstaller);
			Installers.Add (serviceInstaller);
		}
	}
}
