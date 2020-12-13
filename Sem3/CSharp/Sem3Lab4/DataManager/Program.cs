using System.ServiceProcess;

namespace Sem3Lab4.DataManager
{
	static class Program
	{
		static void Main ()
		{
			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[]
			{
				new Service1()
			};
			ServiceBase.Run (ServicesToRun);
		}
	}
}
