using DotNet4.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        /// Xunlei home cloud device free space info struct
        /// </summary>
        public struct SpaceInfo
        {
            public string path;
            public long remain;
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

        public struct TaskSubListInfo
        {
            public int failCode;
            public int id;
            public string name;
            public int progress;
            public int selected;
            public long size;
            public int status;
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
            public TaskSubListInfo[] subList;
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

        /// <summary>
        /// BT check result struct
        /// </summary>
        public struct BTCheckerInfo
        {
            public string infohash;
            public int rtn;
            public TaskInfo taskInfo;
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
        /// Get xunlei home cloud device of free space
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <param name="cookie">Xunlei cookies</param>
        /// <returns>SpaceInfo[]</returns>
        public static SpaceInfo[] FreeSpace(DeviceInfo device, string cookie)
        {
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = string.Format("{0}boxSpace?pid={1}&v=2&ct=0", XunleiBaseURL, device.pid),
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
                int pathCount = json["space"].Count();
                if (pathCount > 0)
                {
                    SpaceInfo[] info = new SpaceInfo[pathCount];
                    for (int i = 0; i < pathCount; i++)
                    {
                        info[i] = new SpaceInfo {
                            path = json["space"][i]["path"].ToString(),
                            remain = Convert.ToInt64(json["space"][i]["remain"])
                        };
                    }
                    return info;
                }else
                {
                    // No save path
                    throw new XunleiDeviceNoSavePathException("HomeCloud.FreeSpace: No save path exist, so no free space.");
                }
            }else
            {
                CommonException.ErrorCode(code, "HomeCloud.FreeSpace");
            }
            throw new XunleiDeviceSpaceException("HomeCloud.FreeSpace: Get device free space error.");
        }

        /// <summary>
        /// Get xunlei home cloud device of free space
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <returns>SpaceInfo[]</returns>
        public static SpaceInfo[] FreeSpace(DeviceInfo device)
        {
            if (!Cookie.CheckCookie())
            {
                throw new XunleiNoCookieException("HomeCloud.FreeSpace:Cookie not found.");
            }
            return FreeSpace(device, Cookie.Cookies);
        }

        /// <summary>
        /// Get xunlei home cloud device of free space
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <param name="cookie">Xunlei cookies</param>
        /// <returns>Task<SpaceInfo[]></returns>
        public static Task<SpaceInfo[]> FreeSpaceAsync(DeviceInfo device, string cookie)
        {
            return Task.Factory.StartNew(()=> {
                return FreeSpace(device, cookie);
            });
        }

        /// <summary>
        /// Get xunlei home cloud device of free space
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <returns>Task<SpaceInfo[]></returns>
        public static Task<SpaceInfo[]> FreeSpaceAsync(DeviceInfo device)
        {
            return Task.Factory.StartNew(() => {
                return FreeSpace(device);
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
        /// <param name="position">Get task items start with</param>
        /// <param name="type">Task type, 0:Downloading, 1:Finished, 2:Trash, 3:Failed</param>
        /// <returns>ListInfo</returns>
        public static ListInfo TaskList(DeviceInfo device, string cookie, int number = 10, int position = 0, int type = 0)
        {
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = string.Format("{0}list?pid={1}&type={2}&pos={3}&number={4}&needUrl=1&v=2&ct=0", XunleiBaseURL, device.pid, type, position, number),
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
                        if (json["tasks"][i]["subList"].HasValues)
                        {
                            task[i].subList = new TaskSubListInfo[json["tasks"][i]["subList"].Count()];
                            for (int j = 0; j < json["tasks"][i]["subList"].Count(); j++)
                            {
                                task[i].subList[j] = new TaskSubListInfo {
                                    failCode = Convert.ToInt32(json["tasks"][i]["subList"][j]["failCode"]),
                                    id = Convert.ToInt32(json["tasks"][i]["subList"][j]["id"]),
                                    name = json["tasks"][i]["subList"][j]["name"].ToString(),
                                    progress = Convert.ToInt32(json["tasks"][i]["subList"][j]["progress"]),
                                    selected = Convert.ToInt32(json["tasks"][i]["subList"][j]["selected"]),
                                    size = Convert.ToInt64(json["tasks"][i]["subList"][j]["size"]),
                                    status = Convert.ToInt32(json["tasks"][i]["subList"][j]["status"]),
                                };
                            }
                        }
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
        /// <param name="position">Get task items start with</param>
        /// <param name="type">Task type, 0:Downloading, 1:Finished, 2:Trash, 3:Failed</param>
        /// <returns>ListInfo</returns>
        public static ListInfo TaskList(DeviceInfo device, int number = 10, int position = 0, int type = 0)
        {
            if (!Cookie.CheckCookie())
            {
                throw new XunleiNoCookieException("HomeCloud.TaskList:Cookie not found.");
            }
            return TaskList(device, Cookie.Cookies, number, position, type);
        }

        /// <summary>
        /// Get the home cloud task list
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <param name="cookie">Xunlei cookies</param>
        /// <param name="number">Hit list number</param>
        /// <param name="position">Get task items start with</param>
        /// <param name="type">Task type, 0:Downloading, 1:Finished, 2:Trash, 3:Failed</param>
        /// <returns>Task<ListInfo></returns>
        public static Task<ListInfo> TaskListAsync(DeviceInfo device, string cookie, int number = 10, int position = 0, int type = 0)
        {
            return Task.Factory.StartNew(()=> {
                return TaskList(device, cookie, number, position, type);
            });
        }

        /// <summary>
        /// Get the home cloud task list use cookies
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <param name="number">Hit list number</param>
        /// <param name="position">Get task items start with</param>
        /// <param name="type">Task type, 0:Downloading, 1:Finished, 2:Trash, 3:Failed</param>
        /// <returns>Task<ListInfo></returns>
        public static Task<ListInfo> TaskListAsync(DeviceInfo device, int number = 10, int position = 0, int type = 0)
        {
            return Task.Factory.StartNew(()=> {
                return TaskList(device, number, position, type);
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
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <param name="cookie">Xunlei cookies</param>
        /// <param name="maxRunTaskNumber">Max running task number</param>
        /// <param name="slStartTime">Speed limit start time, example:0</param>
        /// <param name="slEndTime">Speed limit end time, example:24</param>
        /// <param name="downloadSpeedLimit">Max download speed</param>
        /// <param name="uploadSpeedLimit">Max upload speed, the Minimum value is 10</param>
        /// <param name="autoDlSubtitle">Auto download subtitle</param>
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
        /// <param name="slStartTime">Speed limit start time, example:0</param>
        /// <param name="slEndTime">Speed limit end time, example:24</param>
        /// <param name="downloadSpeedLimit">Max download speed</param>
        /// <param name="uploadSpeedLimit">Max upload speed, the Minimum value is 10</param>
        /// <param name="autoDlSubtitle">Auto download subtitle</param>
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
        /// <param name="info">SettingInfo, need fill all of them</param>
        /// <returns>True:succeed, false:failed</returns>
        public static bool PostSetting(DeviceInfo device, string cookie, SettingInfo info)
        {
            return PostSetting(device, cookie, info.maxRunTaskNumber, info.slStartTime, info.slEndTime, info.downloadSpeedLimit, info.uploadSpeedLimit, info.autoDlSubtitle, info.autoOpenLixian, info.autoOpenVip);
        }

        /// <summary>
        /// Set Setting
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <param name="cookie">Xunlei cookies</param>
        /// <param name="info">SettingInfo, need fill all of them</param>
        /// <returns>True:succeed, false:failed</returns>
        public static bool PostSetting(DeviceInfo device, SettingInfo info)
        {
            if (!Cookie.CheckCookie())
            {
                throw new XunleiNoCookieException("HomeCloud.PostSetting:Cookie not found.");
            }
            return PostSetting(device, Cookie.Cookies, info);
        }

        /// <summary>
        /// Set Setting
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <param name="cookie">Xunlei cookies</param>
        /// <param name="maxRunTaskNumber">Max running task number</param>
        /// <param name="slStartTime">Speed limit start time, example:0</param>
        /// <param name="slEndTime">Speed limit end time, example:24</param>
        /// <param name="downloadSpeedLimit">Max download speed</param>
        /// <param name="uploadSpeedLimit">Max upload speed, the Minimum value is 10</param>
        /// <param name="autoDlSubtitle">Auto download subtitle</param>
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
        /// <param name="slStartTime">Speed limit start time, example:0</param>
        /// <param name="slEndTime">Speed limit end time, example:24</param>
        /// <param name="downloadSpeedLimit">Max download speed</param>
        /// <param name="uploadSpeedLimit">Max upload speed, the Minimum value is 10</param>
        /// <param name="autoDlSubtitle">Auto download subtitle</param>
        /// <param name="autoOpenLixian">1:Auto open, 0:not</param>
        /// <param name="autoOpenVip">1:Auto open, 0:not</param>
        /// <returns>Task<bool></returns>
        public static Task<bool> PostSettingAsync(DeviceInfo device, int maxRunTaskNumber, int slStartTime, int slEndTime, int downloadSpeedLimit, int uploadSpeedLimit, int autoDlSubtitle, int autoOpenLixian, int autoOpenVip)
        {
            return Task.Factory.StartNew(()=> {
                return PostSetting(device, maxRunTaskNumber, slStartTime, slEndTime, downloadSpeedLimit, uploadSpeedLimit, autoDlSubtitle, autoOpenLixian, autoOpenVip);
            });
        }

        /// <summary>
        /// Set Setting
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <param name="cookie">Xunlei cookies</param>
        /// <param name="info">SettingInfo, need fill all of them</param>
        /// <returns>Task<bool></returns>
        public static Task<bool> PostSettingAsync(DeviceInfo device, string cookie, SettingInfo info)
        {
            return Task.Factory.StartNew(()=> {
                return PostSetting(device, cookie, info);
            });
        }

        /// <summary>
        /// Set Setting
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <param name="info">SettingInfo, need fill all of them</param>
        /// <returns>Task<bool></returns>
        public static Task<bool> PostSettingAsync(DeviceInfo device, SettingInfo info)
        {
            return Task.Factory.StartNew(() => {
                return PostSetting(device, info);
            });
        }

        /// <summary>
        /// Active xunlei home cloud device use a activity key
        /// </summary>
        /// <param name="key">Activity key</param>
        /// <param name="cookie">Xunlei cookies</param>
        /// <returns>Succeed or not</returns>
        public static bool ActiveDevice(string key, string cookie)
        {
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = string.Format("{0}bind?boxName=&key={1}&v=2&ct=0", XunleiBaseURL, key),
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
                CommonException.ErrorCode(code, "HomeCloud.ActiveDevice");
            }
            return false;
        }

        /// <summary>
        /// Active xunlei home cloud device use a activity key
        /// </summary>
        /// <param name="key">Activity key</param>
        /// <returns>Succeed or not</returns>
        public static bool ActiveDevice(string key)
        {
            if (!Cookie.CheckCookie())
            {
                throw new XunleiNoCookieException("HomeCloud.ActiveDevice:Cookie not found.");
            }
            return ActiveDevice(key, Cookie.Cookies);
        }

        /// <summary>
        /// Active xunlei home cloud device use a activity key
        /// </summary>
        /// <param name="key">Activity key</param>
        /// <param name="cookie">Xunlei cookies</param>
        /// <returns>Task<bool></returns>
        public static Task<bool> ActiveDeviceAsync(string key, string cookie)
        {
            return Task.Factory.StartNew(()=> {
                return ActiveDevice(key, cookie);
            });
        }

        /// <summary>
        /// Active xunlei home cloud device use a activity key
        /// </summary>
        /// <param name="key">Activity key</param>
        /// <returns>Task<bool></returns>
        public static Task<bool> ActiveDeviceAsync(string key)
        {
            return Task.Factory.StartNew(() => {
                return ActiveDevice(key);
            });
        }

        /// <summary>
        /// Check torrent download info
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <param name="cookie">Xunlei cookies</param>
        /// <param name="path">Torrent local save path</param>
        /// <returns>BTCheckerInfo</returns>
        public static BTCheckerInfo CheckTorrent(DeviceInfo device, string cookie, string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            if (!fileInfo.Exists)
            {
                throw new IOException("HomeCloud.CheckTorrent:Torrent file not found.");
            }
            string boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
            byte[] beginBoundary = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");
            byte[] endBoundary = Encoding.ASCII.GetBytes("--" + boundary + "--\r\n");
            string filePartHeader = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" + "Content-Type: application/octet-stream\r\n\r\n";
            byte[] headerbytes = Encoding.UTF8.GetBytes(string.Format(filePartHeader, "filepath", fileInfo.Name));
            MemoryStream memStream = new MemoryStream();
            memStream.Write(beginBoundary, 0, beginBoundary.Length);
            memStream.Write(headerbytes, 0, headerbytes.Length);
            using (FileStream FS = new FileStream(fileInfo.FullName, FileMode.Open))
            {
                byte[] buffer = new byte[2048];
                int bytesRead;
                while ((bytesRead = FS.Read(buffer, 0, buffer.Length)) != 0)
                {
                    memStream.Write(buffer, 0, bytesRead);
                }
            }
            memStream.Write(endBoundary, 0, endBoundary.Length);
            memStream.Position = 0;
            byte[] tempBuffer = new byte[memStream.Length];
            memStream.Read(tempBuffer, 0, tempBuffer.Length);
            memStream.Close();
            memStream = null;
            
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                // Use POST
                Method = "POST",
                URL = string.Format("{0}btCheck?pid={1}&v=2&ct=0&callback=window.parent._FILE_1_", XunleiBaseURL, device.pid),
                Timeout = Timeout,
                Encoding = Encoding.UTF8,
                Referer = "http://yuancheng.xunlei.com/",
                Host = "homecloud.yuancheng.xunlei.com",
                PostDataType = PostDataType.Byte,
                PostdataByte = tempBuffer,
                Cookie = cookie,
                ContentType = string.Format("multipart/form-data; boundary={0}", boundary),
                Accept = "text/html, application/xhtml+xml, image/jxr, */*",
                // Need KeepAlive maybe
                KeepAlive = true
            };
            string result = http.GetHtml(item).Html;
            tempBuffer = null;
            Match match = Regex.Match(result, "(?<=\\().*(?=\\)</script>)");
            if (match.Success)
            {
                var json = (JObject)JsonConvert.DeserializeObject(match.Value);
                int code = Convert.ToInt32(json["rtn"]);
                if (code == 0)
                {
                    BTCheckerInfo info = new BTCheckerInfo
                    {
                        infohash = json["infohash"].ToString(),
                        rtn = Convert.ToInt32(json["rtn"]),
                        taskInfo = new TaskInfo
                        {
                            id = 0,
                            name = json["taskInfo"]["name"].ToString(),
                            path = json["taskInfo"]["path"].ToString(),
                            size = Convert.ToInt64(json["taskInfo"]["size"]),
                            type = Convert.ToInt32(json["taskInfo"]["type"]),
                            url = json["taskInfo"]["url"].ToString()
                        }
                    };
                    if (json["taskInfo"]["subList"].HasValues)
                    {
                        TaskSubListInfo[] subList = new TaskSubListInfo[json["taskInfo"]["subList"].Count()];
                        for (int i = 0; i < json["taskInfo"]["subList"].Count(); i++)
                        {
                            subList[i] = new TaskSubListInfo
                            {
                                id = Convert.ToInt32(json["taskInfo"]["subList"][i]["id"]),
                                name = json["taskInfo"]["subList"][i]["name"].ToString(),
                                size = Convert.ToInt64(json["taskInfo"]["subList"][i]["size"])
                            };
                        }
                        info.taskInfo.subList = subList;
                    }
                    return info;
                }
                else
                {
                    CommonException.ErrorCode(code, "HomeCloud.CheckTorrent");
                }
            }

            throw new XunleiErrorCodeNotHandleException("HomeCloud.CheckTorrent");
        }

        /// <summary>
        /// Check torrent download info
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <param name="path">Torrent local save path</param>
        /// <returns>BTCheckerInfo</returns>
        public static BTCheckerInfo CheckTorrent(DeviceInfo device, string path)
        {
            if (!Cookie.CheckCookie())
            {
                throw new XunleiNoCookieException("HomeCloud.CheckTorrent:Cookie not found.");
            }
            return CheckTorrent(device, Cookie.Cookies, path);
        }

        /// <summary>
        /// Check torrent download info
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <param name="cookie">Xunlei cookies</param>
        /// <param name="path">Torrent local save path</param>
        /// <returns>Task<BTCheckerInfo></returns>
        public static Task<BTCheckerInfo> CheckTorrentAsync(DeviceInfo device, string cookie, string path)
        {
            return Task.Factory.StartNew(()=> {
                return CheckTorrent(device, cookie, path);
            });
        }

        /// <summary>
        /// Check torrent download info
        /// </summary>
        /// <param name="device">Xunlei home cloud device</param>
        /// <param name="path">Torrent local save path</param>
        /// <returns>Task<BTCheckerInfo></returns>
        public static Task<BTCheckerInfo> CheckTorrentAsync(DeviceInfo device, string path)
        {
            return Task.Factory.StartNew(() => {
                return CheckTorrent(device, path);
            });
        }
    }
}
