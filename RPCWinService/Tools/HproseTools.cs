using Hprose.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using log4net;
using Shop.Public.Common;
using Hprose.Common;

namespace RPCWinService.Tools
{
    public class HproseTools
    {
        private static HproseTcpListenerServer tcpserver;
        private static string ipPort = ConfigurationManager.AppSettings["ipPort"];
        private static string startServicesFolder = ConfigurationManager.AppSettings["startServicesFolder"];
        private static int? traceMilliseconds;
        private static ILog hprose_logger = LogManager.GetLogger("Hprose");
        private static bool isConsole;
        public static void Start()
        {
            int parse_traceMilliseconds;
            traceMilliseconds = int.TryParse(ConfigurationManager.AppSettings["traceMilliseconds"], out parse_traceMilliseconds) ? parse_traceMilliseconds : default(int?);
            string[] allowSecretKeys = ConfigurationManager.AppSettings["allowSecretKeys"].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            isConsole = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle != IntPtr.Zero;
            tcpserver = new HproseTcpListenerServer(string.Format("tcp4://{0}/", ipPort));
            writeLog(string.Format("开始监听{0}", ipPort));
            //服务dll所在文件夹：/bin/ServiceCores/xxx
            string directoryPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, startServicesFolder);
            if (!Directory.Exists(directoryPath))
            {
                writeLog("服务dll文件夹不存在");
                return;
            }
            writeLog(string.Format("开始加载{0}目录下服务", startServicesFolder));
            var services = new List<string>();
            string[] files = Directory.GetFiles(directoryPath);
            try
            {
                ProxyDomain pd = new ProxyDomain();
                foreach (var fullName in files)
                {
                    // Path.GetExtension(fileName).Equals(".dll")
                    if (fullName.ToLower().EndsWith("core.dll"))
                    {
                        string fileName = Path.GetFileNameWithoutExtension(fullName);
                        //byte[] buffer = File.ReadAllBytes(fullName);
                        //var ass = Assembly.Load(buffer);
                        var ass = pd.GetAssembly(fullName);
                        foreach (Type t in ass.GetExportedTypes())
                        {
                            if (t.IsClass && t.Namespace.EndsWith("Services"))
                            {
                                if (services.Contains(t.Name.ToLower()))
                                {
                                    writeLog(string.Format("服务列表已存在{0}服务", t.Name));
                                    continue;
                                }
                                object serv = Activator.CreateInstance(t);
                                tcpserver.Add(serv, t, t.Name);
                                services.Add(t.Name.ToLower());
                                writeLog(string.Format("{0}服务已添加到服务器列表", t.Name));
                            }
                        }
                    }
                }
                tcpserver.AddFilter(new HproseTokenFilter(allowSecretKeys));
                tcpserver.OnSendError += tcpserver_OnSendError;
                tcpserver.IsDebugEnabled = true;
                tcpserver.Start();
                writeLog("服务已启动");
            }
            catch (Exception ex)
            {
                writeLog(string.Format("服务启动异常，信息：{0}", ex.Message));
            }
        }
        public static void Stop()
        {
            try
            {
                if (tcpserver != null && tcpserver.IsStarted)
                    tcpserver.Stop();
                writeLog("服务已停止");
            }
            catch (Exception ex)
            {
                writeLog(string.Format("服务停止异常，信息：{0}", ex.Message));
            }
        }

        static void tcpserver_OnSendError(Exception e, Hprose.Common.HproseContext context)
        {
            //写入txt
            writeLog(string.Format("服务异常\r\n\r\n异常信息：{0}", e));
        }
        static void writeLog(string logTxt)
        {
            if (isConsole)
            {
                Console.WriteLine(logTxt);
                return;
            }
            hprose_logger.Error(logTxt);
        }
    }

    class HproseTokenFilter : IHproseFilter
    {
        /// <summary>
        /// 构造访问token过滤器
        /// </summary>
        /// <param name="allowSecretKeys">allowSecretKeys集合,SecretKey为8字节</param>
        public HproseTokenFilter(params string[] allowSecretKeys)
        {
            this.allowSecretKeys = allowSecretKeys;
        }
        private string[] allowSecretKeys;
        public MemoryStream InputFilter(MemoryStream inStream, HproseContext context)
        {
            //server
            if (inStream.Length < 8)
                throw new SystemException("token verification failed..");
            var tokenbuf = new byte[8];
            inStream.Read(tokenbuf, 0, 8);
            string token = Encoding.UTF8.GetString(tokenbuf);
            if (!allowSecretKeys.Contains(token))
                throw new SystemException("token verification failed..");
            byte[] buf = new byte[inStream.Length - 8];
            inStream.Read(buf, 0, (int)inStream.Length - 8);
            return new MemoryStream(buf);
        }

        public MemoryStream OutputFilter(MemoryStream outStream, HproseContext context)
        {
            //server
            return outStream;
        }
    }

    class ProxyDomain : MarshalByRefObject
    {
        public Assembly GetAssembly(string assemblyPath)
        {
            try
            {
                return Assembly.LoadFrom(assemblyPath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}
