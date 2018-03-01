using RPCWinService.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace RPCWinService
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            HproseTools.Start();
            Console.ReadKey();
            HproseTools.Stop();
            return;

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new RPCService() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
