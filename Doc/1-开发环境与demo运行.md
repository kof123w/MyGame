![image](https://github.com/kof123w/MyGame/assets/40864999/b557d892-267e-44bb-8db2-28b13ca3fdf2)

安装Unity模块时，记得安装Widnow Build Support(IL2CPP)，否则打包会出问题。
安装Visul Studio，因为需要安装开发组件，需要选择

![image](https://github.com/kof123w/MyGame/assets/40864999/84475aa2-30ae-41c8-8a2d-50e764498ab6)

1-拉去第三方库，unity引擎中-windows-PackageManager-
2-Rider设置。Editor-Preferences-ExternalTools，externalScriptEditor选择Rider，钩上

![image](https://github.com/kof123w/MyGame/assets/40864999/4a8e17a8-1389-49bb-8f54-43c15aba24e5)

然后点击Regenerator project files
3-编译Unity.sln项目，即可编译成功，启动游戏可以看到登录面板,这里启动的客户端与服务端合在一起的模式。

客户端与服务端分开。
1-修改config配置
