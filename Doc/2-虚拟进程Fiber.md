Unity.Loader是非热更新项目。

在Init中有初始化的操作，比如this.Startasync(),Corontine().

设置不销毁物体DontDestoryOnload。

设置app作用域AppDomain.currentdomain.Unhandledexception +=xx.

设置命令行参数（在option中），如果是游戏客户端，则命令行是空字符串， 如果是游戏服务器端，则是转换为Options当中的参数，即可以在服务器命令行中使用的参数，有apptype，启服务器配置是startConfig，如果使用了不对的参数，则会报错。

有一个world静态单例类，有Addsingleton添加单例的方法，增加timInfo与FiberManager的单例，使用
await World.Instance.AddSingleton<ResourcesComponent>().CreatePackageAsync("DefaultPackage", true)异步加载默认资源.

codeLoader加载热更新代码，如果不是编辑器，则会加载热更新代码，然后执行它的start（）函数，通过反射调用到ET.Entry的Start函数。这就是共同的启动入口，

DotNet.App是服务端启动的位置，程序入口是program文件，从main函数开始，start()函数也如果刚才的start（）函数一样，获取与加载程序集，然后以反射的方式调用ET.Entry，在dotnet.model/share/Entry中，在该函数中，先设置windons平台的精度设计，然后进行mongoRegister初始化，通过world.instance并添加各种单例类，并使用Codetypes创建一系列的单例类,主要是通过对应标签来创建单例类，需要以逻辑热重载的方式，

客户端服务端不热更不共享的组件可以写到Loader中，比如表现层需要一个组件不需要热更，可以写在Loader中，这样性能更高。如果客户端跟服务端共享的并且不需要热更的的组件可以写在Core中

Fiber图，一个Fiber负责一个场景的运行。

![image](https://github.com/kof123w/MyGame/assets/40864999/77fbca58-11ed-43f4-8482-12d680c7bc5c)

![image](https://github.com/kof123w/MyGame/assets/40864999/fa9880c8-2523-434d-98f1-100299f86ea3)

FIber类中，有一个EntitySystem的变量，创建实体或组件的时候，会将其注册进EntitySystem中，作用是为了调用生命周期函数。

调度类型ScheulerType有三种，Main（主线程） Thread（固定线程） ThreadPool（线程池）
