# 简介
为方便调用[迅雷远程下载](http://yuancheng.xunlei.com/)而编写。已实现基本功能（获取下载列表，获取设备，获取及设置设备参数，添加新下载任务等），将逐步完善各项功能。
# 环境
需要.net4.5.2环境支持。
# 使用方法
将编译后的XunleiHomeCloud.dll及依赖dll引用到工程内即可。
需要先获取[迅雷远程下载](http://yuancheng.xunlei.com/)的Cookie，使用
`
Cookie.SetCookie(string cookie);
`
或
`
Cookie.LoadCookie(string path, bool overwrite = false);
`
设置Cookie后即可使用其他的功能。
# License
Under the GPL license.See the LICENSE file for more info. 
