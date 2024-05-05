Fiber里有个scene是只读属性，能从fiber中获取它属于哪个场景，scene实体中又声明了fiber，fiber与scene是互相引用的关系。

scene层级关系

![image](https://github.com/kof123w/MyGame/assets/40864999/d89227ba-bc42-4a40-b59e-c9be7c5e6d6d)

举例

![image](https://github.com/kof123w/MyGame/assets/40864999/0ce3c7b7-9055-4a3c-a31e-21d55158c7b4)

在unity.core中的EntityHelper中，可以通过entity获取对应的scene root fiber等属性。

获得实体的scene场景：Scene scene = computer.root();
