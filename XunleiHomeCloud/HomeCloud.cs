using DotNet4.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XunleiHomeCloud
{
    public class HomeCloud
    {

        #region Private
        /// <summary>
        /// Http time out
        /// </summary>
        private static int Timeout = 30000;

        /// <summary>
        /// http://homecloud.yuancheng.xunlei.com/
        /// </summary>
        private static string XunleiBaseURL = "http://homecloud.yuancheng.xunlei.com/";
        #endregion

        #region PublicStruct
        /// <summary>
        /// Xunlei home cloud device info struct
        /// </summary>
        public struct DeviceInfo
        {
            public string accesscode;
            public string company;
            public int deviceVersion;
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
        /// tasks.lixianChannel
        /// </summary>
        public struct LixianChannelInfo
        {
            public int dlBytes;
            public int failCode;
            public int serverProgress;
            public int serverSpeed;
            public int speed;
            public int state;
        }

        /// <summary>
        /// tasks.vipChannel
        /// </summary>
        public struct VipChannelInfo
        {
            public int available;
            public int dlBytes;
            public int failCode;
            public int opened;
            public int speed;
            public int type;
        }

        /// <summary>
        /// Task list item
        /// </summary>
        public struct TaskInfo
        {
            public long completeTime;
            public long createTime;
            public int downTime;
            public int failCode;
            public long id;
            public LixianChannelInfo lixianChannel;
            public string name;
            public string path;
            public int progress;
            public int remainTime;
            public long size;
            public int speed;
            public int state;
            public string subList;
            public int type;
            public string url;
            public VipChannelInfo vipChannel;
        }

        /// <summary>
        /// TaskList result struct
        /// </summary>
        public struct ListInfo
        {
            public int completeNum;
            public int dlNum;
            public int recycleNum;
            public int rtn;
            public int serverFailNum;
            public int sync;
            public TaskInfo[] task;
        }

        public struct SettingInfo
        {
            public int autoDlSubtitle;
            public int autoOpenLixian;
            public int autoOpenVip;
            public string defaultPath;
            public int downloadSpeedLimit;
            public int maxRunTaskNumber;
            public string msg;
            public int rtn;
            public int slEndTime;
            public int slStartTime;
            public int syncRange;
            public int uploadSpeedLimit;
        }
        #endregion

        /// <summary>
        /// Get xunlei home cloud devices use cookies
        /// </summary>
        /// <returns>DeviceInfo[]</returns>
        public static DeviceInfo[] DeviceList()
        {
            // Use default cookie
            if (!Cookie.CheckCookie())
            {
                // Throw exception if the cookie not found
                throw new XunleiNoCookieException("HomeCloud.DeviceList:Cookie not found.");
            }
            return DeviceList(Cookie.Cookies);
        }

        /// <summary>
        /// Get xunlei home cloud devices use cookies
        /// </summary>
        /// <returns>Task<DeviceInfo[]></returns>
        public static Task<DeviceInfo[]> DeviceListAsync()
        {
            // Use new task to get the device list
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
            // Convert string to json
            var json = (JObject)JsonConvert.DeserializeObject(result);
            // Get the status code
            int code = Convert.ToInt32(json["rtn"]);
            // Normal query if return code = 0
            if (code == 0)
            {
                // New some Deviceinfo use "peerList" count
                DeviceInfo[] device = new DeviceInfo[json["peerList"].Count()];
                // Get each "peerList" item
                for (int i = 0; i < json["peerList"].Count(); i++)
                {
                    device[i] = new DeviceInfo
                    {
                        // Get the info we need, not all items
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
            }else
            {
                // Return error code, Match the error code map
                CommonException.ErrorCode(code, "HomeCloud.DeviceList");
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
        /// Rename device name
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <param name="newName">New device name</param>
        /// <param name="cookie">Xunlei cookies</param>
        /// <returns>Rename succeed or not</returns>
        public static bool RenameDevice(DeviceInfo device, string newName, string cookie)
        {
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = string.Format("{0}rename?pid={1}&boxName={2}&v=2&ct=0", XunleiBaseURL, device.pid, Tools.URLEncoding(newName, Encoding.UTF8)),
                Encoding = Encoding.UTF8,
                Timeout = Timeout,
                Referer = "http://yuancheng.xunlei.com/",
                Host = "homecloud.yuancheng.xunlei.com",
                Cookie = cookie,
                Accept = "application/javascript, */*;q=0.8"
            };
            string result = http.GetHtml(item).Html;
            var json = (JObject)JsonConvert.DeserializeObject(result);
            int code = Convert.ToInt32(json["rtn"]);
            if (code == 0)
            {
                return true;
            }else
            {
                CommonException.ErrorCode(code, "Xunlei.RenameDevice");
            }
            return false;
        }

        /// <summary>
        /// Rename device name
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <param name="newName">New device name</param>
        /// <returns>Rename succeed or not</returns>
        public static bool RenameDevice(DeviceInfo device, string newName)
        {
            // Use default cookie
            if (!Cookie.CheckCookie())
            {
                // Throw exception if the cookie not found
                throw new XunleiNoCookieException("HomeCloud.RenameDevice:Cookie not found.");
            }
            return RenameDevice(device, newName, Cookie.Cookies);
        }

        /// <summary>
        /// Rename device name
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <param name="newName">New device name</param>
        /// <param name="cookie">Xunlei cookies</param>
        /// <returns>Task<bool></returns>
        public static Task<bool> RenameDeviceAsync(DeviceInfo device, string newName, string cookie)
        {
            return Task.Factory.StartNew(()=> {
                return RenameDevice(device, newName, cookie);
            });
        }

        /// <summary>
        /// Rename device name
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <param name="newName">New device name</param>
        /// <returns>Task<bool></returns>
        public static Task<bool> RenameDeviceAsync(DeviceInfo device, string newName)
        {
            return Task.Factory.StartNew(() => {
                return RenameDevice(device, newName);
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
                throw new XunleiNoCookieException("HomeCloud.AddNewTask:Cookie not found.");
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
            // Use StringBuilder to generate the Post data
            StringBuilder SB = new StringBuilder("{\"path\":\"");
            // Get the device default save file path use GetSetting_DefaultPath();
            SB.Append(GetSetting_DefaultPath(device, cookie));
            // Add download task with a url
            SB.Append("\",\"tasks\":[{\"url\":\"");
            SB.Append(url);
            // Add a file name
            SB.Append("\",\"name\":\"");
            SB.Append(fileName);
            // Default setting
            SB.Append("\",\"gcid\":\"\",\"cid\":\"\",\"filesize\":0,\"ext_json\":{\"autoname\":1}}]}");
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                // Use POST
                Method = "POST",
                URL = string.Format("{0}createTask?pid={1}&v=2&ct=0", XunleiBaseURL, device.pid),
                Encoding = Encoding.UTF8,
                Timeout = Timeout,
                Referer = "http://yuancheng.xunlei.com/",
                Host = "homecloud.yuancheng.xunlei.com",
                // Encoding the Postdata to a URL format
                Postdata = "json=" + Tools.URLEncoding(SB.ToString(), Encoding.UTF8),
                PostEncoding = Encoding.UTF8,
                Cookie = cookie,
                // Need use this ContentType and Accept maybe
                ContentType = "application/x-www-form-urlencoded",
                Accept = "text/html, application/xhtml+xml, image/jxr, */*",
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko",
                // Need KeepAlive maybe
                KeepAlive = true,
            };
            string result = http.GetHtml(item).Html;
            var json = (JObject)JsonConvert.DeserializeObject(result);
            int code = Convert.ToInt32(json["rtn"]);
            // Normal query if code = 0
            if (code == 0)
            {
                int resultCode = Convert.ToInt32(json["tasks"][0]["result"]);
                // Normal query if "tasks" "result" return 0
                if (resultCode == 0)
                {
                    return true;
                }else
                {
                    // Return error code, match the error code map
                    CommonException.ErrorCode(resultCode, "Xunlei.AddNewTask");
                }
            }else
            {
                CommonException.ErrorCode(code, "Xunlei.AddNewTask");
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
            int code = Convert.ToInt32(json["rtn"]);
            if (code == 0)
            {
                // Normal query, return the default path
                return json["defaultPath"].ToString();
            }
            else
            {
                CommonException.ErrorCode(code, "Xunlei.GetSetting_DefaultPath");
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
                throw new XunleiNoCookieException("HomeCloud.GetSetting_DefaultPath:Cookie not found.");
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

        /// <summary>
        /// Get the home cloud task list
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <param name="cookie">Xunlei cookies</param>
        /// <param name="number">Hit list number</param>
        /// <returns>ListInfo</returns>
        public static ListInfo TaskList(DeviceInfo device, string cookie, int number = 10)
        {
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = string.Format("{0}list?pid={1}&type=0&pos=0&number={2}&needUrl=1&v=2&ct=0", XunleiBaseURL, device.pid, number),
                Encoding = Encoding.UTF8,
                Timeout = Timeout,
                Referer = "http://yuancheng.xunlei.com/",
                Host = "homecloud.yuancheng.xunlei.com",
                Cookie = cookie
            };
            string result = http.GetHtml(item).Html;
            var json = (JObject)JsonConvert.DeserializeObject(result);
            int code = Convert.ToInt32(json["rtn"]);
            if (code == 0)
            {
                // Normal query, create a ListInfo
                ListInfo list = new ListInfo
                {
                    completeNum = Convert.ToInt32(json["completeNum"]),
                    dlNum = Convert.ToInt32(json["dlNum"]),
                    recycleNum = Convert.ToInt32(json["recycleNum"]),
                    rtn = Convert.ToInt32(json["rtn"]),
                    serverFailNum = Convert.ToInt32(json["serverFailNum"]),
                    sync = Convert.ToInt32(json["sync"])
                };
                // Check the "tasks" key
                if (json["tasks"].HasValues)
                {
                    // Create some TaskInfo use "tasks" count
                    TaskInfo[] task = new TaskInfo[json["tasks"].Count()];
                    // Get each tasks item
                    for (int i = 0; i < json["tasks"].Count(); i++)
                    {
                        // Get the value we need
                        task[i].completeTime = Convert.ToInt64(json["tasks"][i]["completeTime"]);
                        task[i].createTime = Convert.ToInt64(json["tasks"][i]["createTime"]);
                        task[i].downTime = Convert.ToInt32(json["tasks"][i]["downTime"]);
                        task[i].failCode = Convert.ToInt32(json["tasks"][i]["failCode"]);
                        task[i].id = Convert.ToInt64(json["tasks"][i]["id"]);
                        // If "tasks" "i" "lixianChannel" has values
                        if (json["tasks"][i]["lixianChannel"].HasValues)
                        {
                            // Get the "lixianChannel" values
                            task[i].lixianChannel = new LixianChannelInfo
                            {
                                dlBytes = Convert.ToInt32(json["tasks"][i]["lixianChannel"]["dlBytes"]),
                                failCode = Convert.ToInt32(json["tasks"][i]["lixianChannel"]["failCode"]),
                                serverProgress = Convert.ToInt32(json["tasks"][i]["lixianChannel"]["serverProgress"]),
                                serverSpeed = Convert.ToInt32(json["tasks"][i]["lixianChannel"]["serverSpeed"]),
                                speed = Convert.ToInt32(json["tasks"][i]["lixianChannel"]["speed"]),
                                state = Convert.ToInt32(json["tasks"][i]["lixianChannel"]["state"])
                            };
                        }
                        task[i].name = json["tasks"][i]["name"].ToString();
                        task[i].path = json["tasks"][i]["path"].ToString();
                        task[i].progress = Convert.ToInt32(json["tasks"][i]["progress"]);
                        task[i].remainTime = Convert.ToInt32(json["tasks"][i]["remainTime"]);
                        task[i].size = Convert.ToInt64(json["tasks"][i]["size"]);
                        task[i].speed = Convert.ToInt32(json["tasks"][i]["speed"]);
                        task[i].state = Convert.ToInt32(json["tasks"][i]["state"]);
                        task[i].subList = json["tasks"][i]["subList"].ToString();
                        task[i].type = Convert.ToInt32(json["tasks"][i]["type"]);
                        task[i].url = json["tasks"][i]["url"].ToString();
                        // If "tasks" "i" "lixianChannel" has values
                        if (json["tasks"][i]["vipChannel"].HasValues)
                        {
                            // Get the "lixianChannel" values
                            task[i].vipChannel = new VipChannelInfo
                            {
                                available = Convert.ToInt32(json["tasks"][i]["vipChannel"]["available"]),
                                dlBytes = Convert.ToInt32(json["tasks"][i]["vipChannel"]["dlBytes"]),
                                failCode = Convert.ToInt32(json["tasks"][i]["vipChannel"]["failCode"]),
                                opened = Convert.ToInt32(json["tasks"][i]["vipChannel"]["opened"]),
                                speed = Convert.ToInt32(json["tasks"][i]["vipChannel"]["speed"]),
                                type = Convert.ToInt32(json["tasks"][i]["vipChannel"]["type"]),
                            };
                        }
                    }
                    list.task = task;
                }
                return list;
            }
            else
            {
                CommonException.ErrorCode(code, "HomeCloud.TaskList");
            }
            throw new XunleiTaskListException("HomeCloud.TaskList:Return error code.");
        }

        /// <summary>
        /// Get the home cloud task list use cookies
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <param name="number">Hit list number</param>
        /// <returns>ListInfo</returns>
        public static ListInfo TaskList(DeviceInfo device, int number = 10)
        {
            if (!Cookie.CheckCookie())
            {
                throw new XunleiNoCookieException("HomeCloud.TaskList:Cookie not found.");
            }
            return TaskList(device, Cookie.Cookies, number);
        }

        /// <summary>
        /// Get the home cloud task list
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <param name="cookie">Xunlei cookies</param>
        /// <param name="number">Hit list number</param>
        /// <returns>Task<ListInfo></returns>
        public static Task<ListInfo> TaskListAsync(DeviceInfo device, string cookie, int number = 10)
        {
            return Task.Factory.StartNew(()=> {
                return TaskList(device, cookie, number);
            });
        }

        /// <summary>
        /// Get the home cloud task list use cookies
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <param name="number">Hit list number</param>
        /// <returns>Task<ListInfo></returns>
        public static Task<ListInfo> TaskListAsync(DeviceInfo device, int number = 10)
        {
            return Task.Factory.StartNew(()=> {
                return TaskList(device, number);
            });
        }

        /// <summary>
        /// Get xunlei home cloud setting
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <param name="cookie">Xunlei cookies</param>
        /// <returns>SettingInfo</returns>
        public static SettingInfo GetSetting(DeviceInfo device, string cookie)
        {
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = string.Format("{0}settings?pid={1}&v=2&ct=0", XunleiBaseURL, device.pid),
                Encoding = Encoding.UTF8,
                Timeout = Timeout,
                Referer = "http://yuancheng.xunlei.com/",
                Host = "homecloud.yuancheng.xunlei.com",
                Cookie = cookie,
                Accept = "application/javascript, */*;q=0.8"
            };
            string result = http.GetHtml(item).Html;
            var json = (JObject)JsonConvert.DeserializeObject(result);
            int code = Convert.ToInt32(json["rtn"]);
            if (code == 0)
            {
                // Not all values maybe
                return new SettingInfo {
                    autoDlSubtitle = Convert.ToInt32(json["autoDlSubtitle"]),
                    autoOpenLixian = Convert.ToInt32(json["autoOpenLixian"]),
                    autoOpenVip = Convert.ToInt32(json["autoOpenVip"]),
                    defaultPath = json["defaultPath"].ToString(),
                    downloadSpeedLimit = Convert.ToInt32(json["downloadSpeedLimit"]),
                    maxRunTaskNumber = Convert.ToInt32(json["maxRunTaskNumber"]),
                    msg = json["msg"].ToString(),
                    rtn = Convert.ToInt32(json["rtn"]),
                    slEndTime = Convert.ToInt32(json["slEndTime"]),
                    slStartTime = Convert.ToInt32(json["slStartTime"]),
                    syncRange = Convert.ToInt32(json["syncRange"]),
                    uploadSpeedLimit = Convert.ToInt32(json["uploadSpeedLimit"])
                };
            }else
            {
                CommonException.ErrorCode(code, "HomeCloud.GetSetting");
            }
            throw new XunleiSettingException("HomeCloud.GetSetting:Return error code.");
        }

        /// <summary>
        /// Get xunlei home cloud setting use cookies
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <returns>SettingInfo</returns>
        public static SettingInfo GetSetting(DeviceInfo device)
        {
            if (!Cookie.CheckCookie())
            {
                throw new XunleiNoCookieException("HomeCloud.GetSetting:Cookie not found.");
            }
            return GetSetting(device, Cookie.Cookies);
        }

        /// <summary>
        /// Get xunlei home cloud setting
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <param name="cookie">Xunlei cookies</param>
        /// <returns>Task<SettingInfo></returns>
        public static Task<SettingInfo> GetSettingAsync(DeviceInfo device, string cookie)
        {
            return Task.Factory.StartNew(()=> {
                return GetSetting(device, cookie);
            });
        }

        /// <summary>
        /// Get xunlei home cloud setting
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <returns>Task<SettingInfo></returns>
        public static Task<SettingInfo> GetSettingAsync(DeviceInfo device)
        {
            return Task.Factory.StartNew(()=> {
                return GetSetting(device);
            });
        }

        /// <summary>
        /// Set Setting
        /// TODO: need test
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <param name="cookie">Xunlei cookies</param>
        /// <param name="maxRunTaskNumber">Max running task number</param>
        /// <param name="slStartTime"></param>
        /// <param name="slEndTime"></param>
        /// <param name="downloadSpeedLimit">Max download speed</param>
        /// <param name="uploadSpeedLimit">Max upload speed</param>
        /// <param name="autoDlSubtitle"></param>
        /// <param name="autoOpenLixian">1:Auto open, 0:not</param>
        /// <param name="autoOpenVip">1:Auto open, 0:not</param>
        /// <returns>True:succeed, false:failed</returns>
        public static bool PostSetting(DeviceInfo device, string cookie, int maxRunTaskNumber, int slStartTime, int slEndTime, int downloadSpeedLimit, int uploadSpeedLimit, int autoDlSubtitle, int autoOpenLixian, int autoOpenVip)
        {
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = string.Format("{0}settings?pid={1}&maxRunTaskNumber={2}&slStartTime={3}&slEndTime={4}&downloadSpeedLimit={5}&uploadSpeedLimit={6}&autoDlSubtitle={7}&autoOpenLixian={8}&autoOpenVip={9}&v=2&ct=0",
                XunleiBaseURL, device.pid, maxRunTaskNumber, slStartTime, slEndTime, downloadSpeedLimit, uploadSpeedLimit, autoDlSubtitle, autoOpenLixian, autoOpenVip),
                Encoding = Encoding.UTF8,
                Timeout = Timeout,
                Referer = "http://yuancheng.xunlei.com/",
                Host = "homecloud.yuancheng.xunlei.com",
                Cookie = cookie
            };
            string result = http.GetHtml(item).Html;
            var json = (JObject)JsonConvert.DeserializeObject(result);
            int code = Convert.ToInt32(json["rtn"]);
            if (code == 0)
            {
                return true;
            }else
            {
                CommonException.ErrorCode(code, "HomeCloud.PostSetting");
            }
            throw new XunleiSettingException("HomeCloud.PostSetting:Return error code.");
        }


        /// <summary>
        /// Set Setting use cookies
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <param name="maxRunTaskNumber">Max running task number</param>
        /// <param name="slStartTime"></param>
        /// <param name="slEndTime"></param>
        /// <param name="downloadSpeedLimit">Max download speed</param>
        /// <param name="uploadSpeedLimit">Max upload speed</param>
        /// <param name="autoDlSubtitle"></param>
        /// <param name="autoOpenLixian">1:Auto open, 0:not</param>
        /// <param name="autoOpenVip">1:Auto open, 0:not</param>
        /// <returns>True:succeed, false:failed</returns>
        public static bool PostSetting(DeviceInfo device, int maxRunTaskNumber, int slStartTime, int slEndTime, int downloadSpeedLimit, int uploadSpeedLimit, int autoDlSubtitle, int autoOpenLixian, int autoOpenVip)
        {
            if (!Cookie.CheckCookie())
            {
                throw new XunleiNoCookieException("HomeCloud.PostSetting:Cookie not found.");
            }
            return PostSetting(device, Cookie.Cookies, maxRunTaskNumber, slStartTime, slEndTime, downloadSpeedLimit, uploadSpeedLimit, autoDlSubtitle, autoOpenLixian, autoOpenVip);
        }

        /// <summary>
        /// Set Setting
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <param name="cookie">Xunlei cookies</param>
        /// <param name="maxRunTaskNumber">Max running task number</param>
        /// <param name="slStartTime"></param>
        /// <param name="slEndTime"></param>
        /// <param name="downloadSpeedLimit">Max download speed</param>
        /// <param name="uploadSpeedLimit">Max upload speed</param>
        /// <param name="autoDlSubtitle"></param>
        /// <param name="autoOpenLixian">1:Auto open, 0:not</param>
        /// <param name="autoOpenVip">1:Auto open, 0:not</param>
        /// <returns>Task<bool></returns>
        public static Task<bool> PostSettingAsync(DeviceInfo device, string cookie, int maxRunTaskNumber, int slStartTime, int slEndTime, int downloadSpeedLimit, int uploadSpeedLimit, int autoDlSubtitle, int autoOpenLixian, int autoOpenVip)
        {
            return Task.Factory.StartNew(()=> {
                return PostSetting(device, cookie, maxRunTaskNumber, slStartTime, slEndTime, downloadSpeedLimit, uploadSpeedLimit, autoDlSubtitle, autoOpenLixian, autoOpenVip);
            });
        }

        /// <summary>
        /// Set Setting use cookies
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <param name="maxRunTaskNumber">Max running task number</param>
        /// <param name="slStartTime"></param>
        /// <param name="slEndTime"></param>
        /// <param name="downloadSpeedLimit">Max download speed</param>
        /// <param name="uploadSpeedLimit">Max upload speed</param>
        /// <param name="autoDlSubtitle"></param>
        /// <param name="autoOpenLixian">1:Auto open, 0:not</param>
        /// <param name="autoOpenVip">1:Auto open, 0:not</param>
        /// <returns>Task<bool></returns>
        public static Task<bool> PostSettingAsync(DeviceInfo device, int maxRunTaskNumber, int slStartTime, int slEndTime, int downloadSpeedLimit, int uploadSpeedLimit, int autoDlSubtitle, int autoOpenLixian, int autoOpenVip)
        {
            return Task.Factory.StartNew(()=> {
                return PostSetting(device, maxRunTaskNumber, slStartTime, slEndTime, downloadSpeedLimit, uploadSpeedLimit, autoDlSubtitle, autoOpenLixian, autoOpenVip);
            });
        }
    }
}
