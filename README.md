# 简介
为方便调用[迅雷远程下载](http://yuancheng.xunlei.com/)而编写。已实现基本功能（登录账号，获取下载列表，获取设备，获取及设置设备参数，添加新下载任务等），将逐步完善各项功能。
# 环境
需要.net4.5.2环境支持。
# 使用方法
将编译后的XunleiHomeCloud.dll及依赖dll引用到工程内即可。

## 登录信息
需要先获取[迅雷远程下载](http://yuancheng.xunlei.com/)的Cookie，方法可使用本程序提供的方式。
```
    string Cookies = Login.Post("username", "password");
    Cookie.SetCookie(Cookies);
```

或从本地文档中载入Cookie。
```
    Cookie.LoadCookie("path", false);
```
设置好Cookie后即可使用其他的功能。

## 相关功能
绑定远程设备（下载器），需先在对应设备中安装由迅雷提供的客户端，使用激活码完成绑定。
```
    bool activity = HomeCloud.ActiveDevice("123123");
```
获取远程设备列表，在使用“获取下载列表”，“获取及设置设备参数”，“添加新下载任务”功能时需先要获取远程设备列表，对应相关远程设备使用其他功能。
```
    HomeCloud.DeviceInfo[] list = HomeCloud.DeviceList();
```
重命名远程设备。
```
    bool renameDevice = HomeCloud.RenameDevice(list[0], "New name");
```
解除绑定远程设备，获取远程设备列表后，对应删除设备即可。
```
    bool unbindDevice = HomeCloud.UnbindDevice(list[0]);
```
获取远程设备的剩余空间，可获取远程设备的各盘符剩余空间。
```
    HomeCloud.SpaceInfo[] spaceInfo = HomeCloud.FreeSpace(list[0]);
```
获取任务列表，支持“正在下载”，“已完成”，“垃圾箱”，“提交失败”列表。
```
    HomeCloud.ListInfo task = HomeCloud.TaskList(list[0]);
    HomeCloud.ListInfo task = HomeCloud.TaskList(list[0]， 10， 0， 0);
```
获取设备参数，获取对应设备的设置参数。
```
    HomeCloud.SettingInfo setting = HomeCloud.GetSetting(list[0]);
```
设置设备参数，设置对应设备的设置参数，主要是设置最大同时任务量，下载限速，上传限速，自动离线，自动高速等。
```
    bool result = HomeCloud.PostSetting(list[0], 3, 0, 1440, 2048, 0, 0, 1, 1);
    bool result = HomeCloud.PostSetting(list[1], SettingInfo);
```
添加新下载任务，为对应设备添加新下载任务，需提供下载链接及存储文件名。
```
    bool taskResult = HomeCloud.AddNewTask(list[0], "http://test.com/test.torrent", "1.torrent");
```
或提供本地种子文件路径。先获取种子信息，再选择需要下载的资源进行下载。
```
    HomeCloud.BTCheckerInfo torrentInfo = HomeCloud.CheckTorrent(list[0], @"C:\test.torrent");
    bool localTorrentDownload = HomeCloud.AddNewTask(list[0], remotePath, torrentInfo.infohash, torrentInfo.taskInfo.name, new Int32[2] { 0,1 });
```
亦或者直接使用本地种子文件路径，默认下载种子中的所有资源至对应设备中的默认路径中。
```
    bool localTorrentDownload = HomeCloud.AddNewTask(list[0], @"C:\test.torrent");
```
操作下载任务，支持开启，暂停，删除操作。
```
    // Get task list
    HomeCloud.ListInfo taskList = HomeCloud.TaskList(list[0]);
    // Start
    bool startTask = HomeCloud.StartTask(list[0], taskList.task[2].id, taskList.task[2].state);
    // Or
    bool startTask = HomeCloud.StartTask(list[0], taskList.task[2]);
    // Pause
    bool pauseTask = HomeCloud.PauseTask(list[0], taskList.task[2]);
    // Delete
    bool deleteTask = HomeCloud.DeleteTask(list[0], taskList.task[2], 0, true);
```
# License
Under the GPL license.See the LICENSE file for more info. 
