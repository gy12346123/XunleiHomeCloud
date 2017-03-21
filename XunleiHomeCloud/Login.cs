using DotNet4.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XunleiHomeCloud
{
    public class Login
    {
        #region Private
        /// <summary>
        /// Xunlei login base url
        /// </summary>
        private static string BaseURL = "https://login.xunlei.com/";

        /// <summary>
        /// Http time out
        /// </summary>
        private static int Timeout = 30000;
        #endregion

        #region PublicStruct
        /// <summary>
        /// For generate a device id
        /// </summary>
        public struct GenerateDeviceIdInfo
        {
            public string xl_fp_raw;
            public string xl_fp;
            public string xl_fp_sign;
            public long cachetime;
        }
        #endregion

        /// <summary>
        /// Execute JScript
        /// </summary>
        /// <param name="expression">Expression code</param>
        /// <param name="code">Source code</param>
        /// <returns>Result</returns>
        private static string ExecuteScript(string expression, string code)
        {
            try
            {
                MSScriptControl.ScriptControl scriptControl = new MSScriptControl.ScriptControl();
                scriptControl.UseSafeSubset = true;
                scriptControl.Language = "JScript";
                scriptControl.AddCode(code);
                return scriptControl.Eval(expression).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get xunlei encrypt function(js)
        /// </summary>
        /// <param name="longTimeStamp">Tools.GetLongTimeStamp()</param>
        /// <returns>Js function</returns>
        private static string XunleiEncryptFunction(long longTimeStamp)
        {
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = string.Format("{0}risk?cmd=algorithm&t={1}", BaseURL, longTimeStamp),
                Encoding = Encoding.UTF8,
                Timeout = Timeout,
                Referer = string.Format("http://i.xunlei.com/login/?r_d=1&use_cdn=0&timestamp={0}&refurl=http%3A%2F%2Fyuancheng.xunlei.com%2Flogin.html", longTimeStamp),
                Host = "login.xunlei.com"
            };
            return http.GetHtml(item).Html.Replace("\n", "");
        }

        /// <summary>
        /// Generate a "GenerateDeviceIdInfo" use a browser user agent
        /// </summary>
        /// <param name="userAgent">Browser user agent</param>
        /// <returns>GenerateDeviceIdInfo</returns>
        private static GenerateDeviceIdInfo GenerateDII(string userAgent)
        {
            StringBuilder SB = new StringBuilder(userAgent);
            SB.Append("###zh-cn###24###960x1440###-540###true###true###true###undefined###undefined###x86###Win32#########");
            SB.Append(MD5Helper.HashString(Tools.GetTimeStamp()));
            string raw = Convert.ToBase64String(Encoding.UTF8.GetBytes(SB.ToString()));
            return new GenerateDeviceIdInfo {
                xl_fp_raw = raw,
                xl_fp = MD5Helper.HashString(raw),
                xl_fp_sign = ExecuteScript(string.Format("xl_al(\"{0}\")", raw), XunleiEncryptFunction(Tools.GetLongTimeStamp(DateTime.UtcNow))),
                cachetime = Tools.GetLongTimeStamp(DateTime.UtcNow)
            };
        }

        /// <summary>
        /// Generate device id
        /// </summary>
        /// <param name="userAgent">Browser user agent, Default: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0), if = null</param>
        /// <returns>Cookies</returns>
        public static string GenerateDeviceId(string userAgent = null)
        {
            var generatorInfo = GenerateDII(userAgent == null ? "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)" : userAgent);
            StringBuilder SB = new StringBuilder("xl_fp_raw=");
            SB.Append(Tools.URLEncoding(generatorInfo.xl_fp_raw, Encoding.UTF8));
            SB.Append(string.Format("&xl_fp={0}&xl_fp_sign={1}&cachetime={2}", generatorInfo.xl_fp, generatorInfo.xl_fp_sign, generatorInfo.cachetime));
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = string.Format("{0}risk?cmd=report", BaseURL),
                Encoding = Encoding.UTF8,
                Timeout = Timeout,
                Referer = string.Format("http://i.xunlei.com/login/?r_d=1&use_cdn=0&timestamp={0}&refurl=http%3A%2F%2Fyuancheng.xunlei.com%2Flogin.html", Tools.GetLongTimeStamp(DateTime.UtcNow)),
                Host = "login.xunlei.com",
                ContentType = "application/x-www-form-urlencoded",
                Method = "Post",
                Postdata = SB.ToString()
            };
            return http.GetHtml(item).Cookie;
        }

        /// <summary>
        /// Generate device id
        /// </summary>
        /// <param name="userAgent">Browser user agent, Default: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0), if = null</param>
        /// <returns>Task<string></returns>
        public static Task<string> GenerateDeviceIdAsync(string userAgent = null)
        {
            return Task.Factory.StartNew(()=> {
                return GenerateDeviceId(userAgent);
            });
        }

        /// <summary>
        /// Use username, password and device id to login, and return cookies
        /// </summary>
        /// <param name="username">Xunlei username</param>
        /// <param name="password">Xunlei password</param>
        /// <param name="deviceIdCookie">GenerateDeviceId()</param>
        /// <returns>Cookies</returns>
        public static string Post(string username, string password, string deviceIdCookie)
        {
            var match = Regex.Match(deviceIdCookie, "(?<=deviceid=).{32}");
            if (!match.Success)
            {
                throw new XunleiLoginDeviceIdException("Device id cookie format error.");
            }

            StringBuilder SB = new StringBuilder("p=");
            SB.Append(Tools.URLEncoding(password, Encoding.UTF8));
            SB.Append("&u=");
            SB.Append(Tools.URLEncoding(username, Encoding.UTF8));
            SB.Append("&verifycode=&login_enable=0&business_type=113&v=101&cachetime=");
            SB.Append(Tools.GetLongTimeStamp(DateTime.UtcNow));
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = string.Format("{0}sec2login/?csrf_token={1}", BaseURL, MD5Helper.HashString(match.Value)),
                Encoding = Encoding.UTF8,
                Timeout = Timeout,
                Referer = string.Format("http://i.xunlei.com/login/?r_d=1&use_cdn=0&timestamp={0}&refurl=http%3A%2F%2Fyuancheng.xunlei.com%2Flogin.html", Tools.GetLongTimeStamp(DateTime.UtcNow)),
                Host = "login.xunlei.com",
                ContentType = "application/x-www-form-urlencoded",
                Method = "Post",
                Postdata = SB.ToString(),
                Cookie = deviceIdCookie
            };
            return http.GetHtml(item).Cookie.Replace(";,", ";");
        }

        /// <summary>
        /// Use username, password and device id to login, and return cookies
        /// </summary>
        /// <param name="username">Xunlei username</param>
        /// <param name="password">Xunlei password</param>
        /// <param name="deviceIdCookie">GenerateDeviceId()</param>
        /// <returns>Task<string></returns>
        public static Task<string> PostAsync(string username, string password, string deviceIdCookie)
        {
            return Task.Factory.StartNew(()=> {
                return Post(username, password, deviceIdCookie);
            });
        }

        /// <summary>
        /// Use username and password to login, and return cookies
        /// </summary>
        /// <param name="username">Xunlei username</param>
        /// <param name="password">Xunlei password</param>
        /// <returns>Cookies</returns>
        public static string Post(string username, string password)
        {
            string deviceIdCookie = GenerateDeviceId();
            return Post(username, password, deviceIdCookie);
        }

        /// <summary>
        /// Use username and password to login, and return cookies
        /// </summary>
        /// <param name="username">Xunlei username</param>
        /// <param name="password">Xunlei password</param>
        /// <returns>Task<string></returns>
        public static Task<string> PostAsync(string username, string password)
        {
            return Task.Factory.StartNew(()=> {
                return Post(username, password);
            });
        }
    }
}
