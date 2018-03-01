using Hprose.Client;
using Hprose.Common;
using System;
using System.Configuration;
using System.IO;
using System.Text;

namespace RPC.IServices
{
    class HproseTokenFilter : IHproseFilter
    {
        /// <summary>
        /// 构造访问token过滤器
        /// </summary>
        /// <param name="tokens">secretKey为8字节</param>
        public HproseTokenFilter(string secretKey)
        {
            this.secretKey = secretKey;
        }
        private string secretKey;
        public MemoryStream InputFilter(MemoryStream inStream, HproseContext context)
        {
            //client
            return inStream;
        }

        public MemoryStream OutputFilter(MemoryStream outStream, HproseContext context)
        {
            //client
            var buf = new byte[outStream.Length + 8];
            outStream.Read(buf, 8, (int)outStream.Length);
            var resultStream = new MemoryStream(buf);
            var tokenbuf = Encoding.UTF8.GetBytes(this.secretKey);
            resultStream.Write(tokenbuf, 0, 8);
            resultStream.Position = resultStream.Length;
            return resultStream;
        }
    }
    public static class ServiceProvider
    {
        private static HproseTcpClient hpClient;
        //private static HproseClient orderClient;
        static ServiceProvider()
        {
            hpClient = HproseClient.Create(string.Format("tcp://{0}", ConfigurationManager.AppSettings["hproseIpPort"])) as HproseTcpClient;
            hpClient.AddFilter(new HproseTokenFilter(ConfigurationManager.AppSettings["hproseSecretKey"]));

            //orderClient = HproseClient.Create(string.Format("tcp://{0}", ConfigurationManager.AppSettings["orderServicesIpPort"]));
        }

        public static void Close()
        {
            if (hpClient != null)
                hpClient.Close();
        }


        private static IUserServ _userServ;
        public static IUserServ UserServ
        {
            get
            {
                if (_userServ == null)
                    _userServ = hpClient.UseService<IUserServ>("UserServ");
                return _userServ;
            }
        }
    }
}
