using DotNet4.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XunleiHomeCloud
{
    public class HomeCloud
    {
        /// <summary>
        /// Http time out
        /// </summary>
        private static int Timeout = 30000;

        /// <summary>
        /// http://homecloud.yuancheng.xunlei.com/
        /// </summary>
        private static string XunleiBaseURL = "http://homecloud.yuancheng.xunlei.com/";

        /// <summary>
        /// Xunlei home cloud device info struct
        /// </summary>
        public struct DeviceInfo
        {
            public long lastLoginTime;
            public string localIP;
            public string name;
            public int online;
            public string path_list;
            public string pid;
            public int status;
            public int type;
            public int vodport;
        }

        /// <summary>
        /// Get xunlei home cloud devices use cookies
        /// </summary>
        /// <returns>DeviceInfo[]</returns>
        public static DeviceInfo[] DeviceList()
        {
            if (!Cookie.CheckCookie())
            {
                throw new NoCookieException("HomeCloud.DeviceList:Cookie not found.");
            }
            return DeviceList(Cookie.Cookies);
        }

        /// <summary>
        /// Get xunlei home cloud devices use cookies
        /// </summary>
        /// <returns>Task<DeviceInfo[]></returns>
        public static Task<DeviceInfo[]> DeviceListAsync()
        {
            return Task.Factory.StartNew(()=> {
                return DeviceList();
            });
        }

        /// <summary>
        /// Get xunlei home cloud devices use cookies
        /// </summary>
        /// <param name="cookie">Xunlei cookies</param>
        /// <returns>DeviceInfo[]</returns>
        public static DeviceInfo[] DeviceList(string cookie)
        {
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = string.Format("{0}listPeer?type=0&v=2&ct=0", XunleiBaseURL),
                Encoding = Encoding.UTF8,
                Timeout = Timeout,
                Referer = "http://yuancheng.xunlei.com/",
                Host = "homecloud.yuancheng.xunlei.com",
                Cookie = cookie
            };
            string result = http.GetHtml(item).Html;
            var json = (JObject)JsonConvert.DeserializeObject(result);
            switch (Convert.ToInt32(json["rtn"]))
            {
                case 0:
                    DeviceInfo[] device = new DeviceInfo[json["peerList"].Count()];
                    for (int i = 0; i < json["peerList"].Count(); i++)
                    {
                        device[i] = new DeviceInfo
                        {
                            lastLoginTime = Convert.ToInt64(json["peerList"][i]["lastLoginTime"]),
                            localIP = json["peerList"][i]["localIP"].ToString(),
                            name = json["peerList"][i]["name"].ToString(),
                            online = Convert.ToInt32(json["peerList"][i]["online"]),
                            path_list = json["peerList"][i]["path_list"].ToString(),
                            pid = json["peerList"][i]["pid"].ToString(),
                            status = Convert.ToInt32(json["peerList"][i]["status"]),
                            type = Convert.ToInt32(json["peerList"][i]["type"]),
                            vodport = Convert.ToInt32(json["peerList"][i]["vodport"])
                        };
                    }
                    return device;
                case 403:
                    throw new UserSessionNoneException("HomeCloud.DeviceList:User id or Session id is none.");
            }

            throw new XunleiNoDeviceException("HomeCloud.DeviceList:Device not found.");
        }

        /// <summary>
        /// Get xunlei home cloud devices use cookies
        /// </summary>
        /// <param name="cookie">Xunlei cookies</param>
        /// <returns>Task<DeviceInfo[]></returns>
        public static Task<DeviceInfo[]> DeviceListAsync(string cookie)
        {
            return Task.Factory.StartNew(()=> {
                return DeviceList(cookie);
            });
        }

        /// <summary>
        /// Add new download task into device
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <param name="url">Resource url which need download</param>
        /// <param name="fileName">File name</param>
        /// <returns>True:succeed, false:failed</returns>
        public static bool AddNewTask(DeviceInfo device, string url, string fileName)
        {
            if (!Cookie.CheckCookie())
            {
                throw new NoCookieException("HomeCloud.AddNewTask:Cookie not found.");
            }
            return AddNewTask(device, url, fileName, Cookie.Cookies);
        }

        /// <summary>
        /// Add new download task into device
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <param name="url">Resource url which need download</param>
        /// <param name="fileName">File name</param>
        /// <returns>Task<bool></returns>
        public static Task<bool> AddNewTaskAsync(DeviceInfo device, string url, string fileName)
        {
            return Task.Factory.StartNew(()=> {
                return AddNewTask(device, url, fileName);
            });
        }

        /// <summary>
        /// Add new download task into device
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <param name="url">Resource url which need download</param>
        /// <param name="fileName">File name</param>
        /// <param name="cookie">Xunlei cookies</param>
        /// <returns>True:succeed, false:failed</returns>
        public static bool AddNewTask(DeviceInfo device, string url, string fileName, string cookie)
        {
            StringBuilder SB = new StringBuilder("{\"path\":\"");
            SB.Append(GetSetting_DefaultPath(device, cookie));
            SB.Append("\",\"tasks\":[{\"url\":\"");
            SB.Append(url);
            SB.Append("\",\"name\":\"");
            SB.Append(fileName);
            SB.Append("\",\"gcid\":\"\",\"cid\":\"\",\"filesize\":0,\"ext_json\":{\"autoname\":1}}]}");
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                Method = "POST",
                URL = string.Format("{0}createTask?pid={1}&v=2&ct=0", XunleiBaseURL, device.pid),
                Encoding = Encoding.UTF8,
                Timeout = Timeout,
                Referer = "http://yuancheng.xunlei.com/",
                Host = "homecloud.yuancheng.xunlei.com",
                Postdata = "json=" + Tools.URLEncoding(SB.ToString(), Encoding.UTF8),
                PostEncoding = Encoding.UTF8,
                Cookie = cookie,
                ContentType = "application/x-www-form-urlencoded",
                Accept = "text/html, application/xhtml+xml, image/jxr, */*",
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko",
                KeepAlive = true,
            };
            string result = http.GetHtml(item).Html;
            var json = (JObject)JsonConvert.DeserializeObject(result);
            if (Convert.ToInt32(json["rtn"]) == 0)
            {
                int code = Convert.ToInt32(json["tasks"][0]["result"]);
                switch (code)
                {
                    case 0:
                        return true;
                    case 202:
                        throw new XunleiRepeatTaskException("Xunlei.AddNewTask:Repeat task, skip.");
                }
            }
            return false;
        }

        /// <summary>
        /// Add new download task into device
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <param name="url">Resource url which need download</param>
        /// <param name="fileName">File name</param>
        /// <param name="cookie">Xunlei cookies</param>
        /// <returns>Task<bool></returns>
        public static Task<bool> AddNewTaskAsync(DeviceInfo device, string url, string fileName, string cookie)
        {
            return Task.Factory.StartNew(()=> {
                return AddNewTask(device, url, fileName, cookie);
            });
        }

        /// <summary>
        /// Get the home cloud default download path
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <param name="cookie">Xunlei cookies</param>
        /// <returns>Default path</returns>
        public static string GetSetting_DefaultPath(DeviceInfo device, string cookie)
        {
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = string.Format("{0}settings?pid={1}&v=2&ct=0", XunleiBaseURL, device.pid),
                Encoding = Encoding.UTF8,
                Timeout = Timeout,
                Referer = "http://yuancheng.xunlei.com/",
                Host = "homecloud.yuancheng.xunlei.com",
                Cookie = cookie
            };
            string result = http.GetHtml(item).Html;
            var json = (JObject)JsonConvert.DeserializeObject(result);
            switch (Convert.ToInt32(json["rtn"]))
            {
                case 0:
                    return json["defaultPath"].ToString();
                case 1004:
                    throw new UserNotLoginException("Xunlei.GetSetting_DefaultPath:User not login.");
            }
            throw new XunleiDevicePathException("Xunlei.GetSetting_DefaultPath:Default path not found.");
        }

        /// <summary>
        /// Get the home cloud default download path
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <param name="cookie">Xunlei cookies</param>
        /// <returns>Task<string></returns>
        public static Task<string> GetSetting_DefaultPathAsync(DeviceInfo device, string cookie)
        {
            return Task.Factory.StartNew(()=> {
                return GetSetting_DefaultPath(device, cookie);
            });
        }

        /// <summary>
        /// Get the home cloud default download path
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <returns>Default path</returns>
        public static string GetSetting_DefaultPath(DeviceInfo device)
        {
            if (!Cookie.CheckCookie())
            {
                throw new NoCookieException("HomeCloud.GetSetting_DefaultPath:Cookie not found.");
            }
            return GetSetting_DefaultPath(device, Cookie.Cookies);
        }

        /// <summary>
        /// Get the home cloud default download path
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <returns>Task<string></returns>
        public static Task<string> GetSetting_DefaultPathAsync(DeviceInfo device)
        {
            return Task.Factory.StartNew(()=> {
                return GetSetting_DefaultPath(device);
            });
        }
    }
}
