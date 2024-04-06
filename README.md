# MyGame
个人独立开发的始点

# 初定目标
一个剧情向的角色扮演类型，战斗采用即时动作，剧情可以有多个分支

# 里程碑
## Beta1实现功能
1.完善UI框架  程序:   策划:

2.处理完善加载场景界面 程序: 

3.开发人物Joy控制 程序:

4.开发场景昼夜功能(可选) 程序:

5.开发任务框架脚本，结合场景触发 程序:  策划：

6.确定战斗形式 讨论结果:

7.战斗数值初版（确认战斗形式确认后）策划:

未定部分 

1.战斗系统 

2.AI框架 

3.名声系统{未定，刻画世界必备} 

4.财富系统{必备}）

5.传闻系统{未定，刻画世界必备，包含教学指导}

# 游戏框架规则
游戏框架使用了ET框架，可以支持全栈式开发

设计原则：树状领域，组合模式，事件驱动，逻辑分发
## 编码原则
I.实体即组件，组件即实体

II.如要编写一个新的实体或组件，绝不继承除Enity之外的任何父类

III.绝不使用任何的虚函数，使用逻辑分发替代

IV.model和modelView，只存放实体和组件的数据字段声明，如非不要绝不放任何逻辑函数。

V.Hotifix和HotfixView中只保存纯逻辑函数，也就是使用静态类和扩展方法编写的System，且绝不允许声明任何数据字段

VI.ModelView、HotfixView与Model、Hotfix区别是表现层和逻辑层，Model和Hotfix作为逻辑层项目中绝不允许出现跟Unity3D引擎相关的游戏对象类和调用相关API函数

VII.如实体或组件有数据字段声明必须编写相关生命周期函数，以防止实体对象池回收再利用导致逻辑错误


# 游戏框架结构
## 客户端树形结构
World -> FiberManager -> Fiber -> Scene -> XXXCompoent -> Entity
![image](https://github.com/kof123w/MyGame/assets/40864999/c0589e55-313d-4e77-add5-2267658ae5b1)

## 服务器端树形结构
World -> FiberManager->Fiber->Scene->ProcessOuterSender(业务组件)
![image](https://github.com/kof123w/MyGame/assets/40864999/47996eb6-51ff-472e-9c56-82f76cd5200e)

# Git代码
[控制台下克隆、拉新、提交、发布](https://blog.csdn.net/m0_45234510/article/details/120181503)

# 打包流程
[PC包开发版打包流程](https://blog.csdn.net/qq_41094072/article/details/137381372?spm=1001.2014.3001.5501)

# 配置表使用流程

按照Unity工程下Aseet/Config/Excel/StartConfig下的xlsx文件下的格式，上空两行，左空两列即可。
1.配置excel规则：第二行第二列的#代表忽略，c代表客户端生成，s代表服务端生成，默认代表两端都生成，同一个excel的sheet分页可以组合数据，sheet的名字前#则忽略

![image](https://github.com/kof123w/MyGame/assets/40864999/96539f80-82ab-4573-8622-d70ccce4abd3)

获取配置方法添加一个TestConfig配置表

![image](https://github.com/kof123w/MyGame/assets/40864999/ae886c5b-6b4f-44e4-806a-1673fb7c1679)

选择ET>BuildTool，BuildEditor面板点击ExcelExporter

![image](https://github.com/kof123w/MyGame/assets/40864999/42c77181-80c6-486a-b5da-db3067a89839)
使用生成的单例代码即可

![image](https://github.com/kof123w/MyGame/assets/40864999/4072c72c-535f-4c1f-a97d-a87f4d91e0af)




