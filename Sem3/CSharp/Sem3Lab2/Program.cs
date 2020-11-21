using System.ServiceProcess;

namespace Sem3Lab2
{
	public class Program
	{
		static void Main (string[] args)
		{
			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[]
			{
				new Service1 ()
			};
			ServiceBase.Run (ServicesToRun);
		}
	}
}
