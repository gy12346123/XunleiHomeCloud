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
获取远程设备列表，在使用“获取下载列表”，“获取及设置设备参数”，“添加新下载任务”功能时需先要获取远程设备列表，对应相关远程设备使用其他功能。
```
    HomeCloud.DeviceInfo[] list = HomeCloud.DeviceList();
```
获取下载列表，使用此功能即可获取对应设备正在下载的任务信息。
```
    HomeCloud.ListInfo task = HomeCloud.TaskList(list[0]);
```
获取设备参数，获取对应设备的设置参数。
```
    HomeCloud.SettingInfo setting = HomeCloud.GetSetting(list[0]);
```
设置设备参数，设置对应设备的设置参数，主要是设置最大同时任务量，下载限速，上传限速，自动离线，自动高速等。
```
    bool result = HomeCloud.PostSetting(list[0], 3, 0, 1440, 2048, 0, 0, 1, 1);
```
添加新下载任务，为对应设备添加新下载任务，需提供下载链接及存储文件名。
```
    bool taskResult = HomeCloud.AddNewTask(list[0], "http://test.com/test.torrent", "1.torrent");
```
# License
Under the GPL license.See the LICENSE file for more info. 
