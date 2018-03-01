using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Text.RegularExpressions;
using System.Text;
using Newtonsoft.Json;

namespace RPC.Web.Tools
{
    public class PubMethod
    {
        public static bool RegexViladatePhone(string phone)
        {
            return Regex.IsMatch(phone, @"^1[34578]\d{9}$");
        }
        public static bool RegexViladateEmail(string email)
        {
            return Regex.IsMatch(email, @"^[a-z0-9]+([._\\-]*[a-z0-9])*@([a-z0-9]+[-a-z0-9]*[a-z0-9]+.){1,63}[a-z0-9]+$");
        }
        public static string GetRequestIP(HttpRequestBase request)
        {
            string ip = string.Empty;
            if (!string.IsNullOrEmpty(request.ServerVariables["HTTP_X_FORWARDED_FOR"]))
            {
                //获取非代理
                ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            }
            else if (request.UserHostAddress.Length != 0)
            {
                ip = request.UserHostAddress;
            }
            else
            {
                ip = request.ServerVariables["REMOTE_ADDR"] ?? string.Empty;
            }
            return ip;
        }
        /// <summary>
        /// 生成混合字母数字随机数2
        /// </summary>
        /// <param name="n">位数[0-Z]</param>
        /// <returns></returns>
        public static string GenerateRandCode(int type, int count)
        {
            char[] arrChar;
            if (type == 1)
            {
                arrChar = new char[]
                {
                    '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
                };
            }
            else if (type == 2)
            {
                arrChar = new char[]
                {
                     'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'j', 'k','l','m', 'n', 'p', 'r', 's', 't', 'w', 'x', 'y'
                };
            }
            else
            {
                arrChar = new char[]
                {
                    '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'j', 'k',
                    'l',
                    'm', 'n', 'p', 'r', 's', 't', 'w', 'x', 'y', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L',
                    'M', 'N', 'P', 'R', 'S', 'T', 'W', 'X', 'Y'
                };
            }
            StringBuilder num = new StringBuilder();
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < count; i++)
            {
                num.Append(arrChar[rnd.Next(0, arrChar.Length)].ToString());
            }
            return num.ToString();
        }

        private static string MD5(string str, string charset)
        {
            byte[] buffer = System.Text.Encoding.GetEncoding(charset).GetBytes(str);
            try
            {
                System.Security.Cryptography.MD5CryptoServiceProvider check;
                check = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] somme = check.ComputeHash(buffer);
                string ret = "";
                foreach (byte a in somme)
                {
                    if (a < 16)
                        ret += "0" + a.ToString("X");
                    else
                        ret += a.ToString("X");
                }
                return ret.ToLower();
            }
            catch
            {
                throw;
            }
        }

        private static string Base64(string str, string charset)
        {
            return Convert.ToBase64String(System.Text.Encoding.GetEncoding(charset).GetBytes(str));
        }

        public static string Encrypt(string content, string keyValue, string charset)
        {
            if (keyValue != null)
            {
                return Base64(MD5(content + keyValue, charset), charset);
            }
            return Base64(MD5(content, charset), charset);
        }

        public static DateTime ConvertStringToDateTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        } 


        public static T WebClientGet<T>(string url)
        {
            using (var web = new WebClient())
            {
                string html = web.DownloadString(url);
                return JsonConvert.DeserializeObject<T>(html);
            }
        }

        public static T BaiduWebClientGet<T>(string url)
        {
            using (var web = new WebClient())
            {
                web.Headers.Add("Cookie", "BAIDUID=8CACF6F06A4F85DB69A91980CA3D7F4C:FG=1;");
                web.Headers.Add("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_3) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.84 Safari/537.36Accept:text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
                string html = web.DownloadString(url);
                return JsonConvert.DeserializeObject<T>(html);
            }
        }

        public static T WebClientPost<T>(string url, string data)
        {
            string responsejson;
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);
            using (WebClient web = new WebClient())
            {
                web.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                web.Headers.Add("ContentLength", bytes.Length.ToString());

                byte[] response = web.UploadData(url, "POST", bytes);
                responsejson = System.Text.Encoding.UTF8.GetString(response);
            }
            return JsonConvert.DeserializeObject<T>(responsejson);
        }
    }
}