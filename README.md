# OxoBrowser
  
已更换为cef的嵌入的chrome核心，目前支持舰娘二期的显示。
提供数据接口方便其他游戏改造接入。 
已实现部分游戏提取、顶置、代理、关闭声音、缩放比例等基本功能。

## 开发指南

### 添加游戏类型
* 在AppConfig.cs中定义新游戏类型GameTypeEnum，并指定SizeWithGameType函数中游戏的宽高。
* 在FlyoutConfigDialog中添加新游戏的选项及初始化、保存时调用的方法。
* ChromeMain_LoadingStateChanged函数中实现对应游戏的提取代码，可参考Touken及KanColle的代码根据Chrome调试修改。

### 实现数据包拦截
在HttpPacketHookManager.cs中可以设置需要过滤的网站地址及请求类型，以及处理函数
![image](https://user-images.githubusercontent.com/4475018/199701287-05a272e8-9a1d-4acc-8ff8-7ebeef7ff3f2.png)

### 界面预览
![image](https://github.com/user-attachments/assets/dd52e3cd-fe99-484b-9349-8b1796db2749)



License (MIT) 
