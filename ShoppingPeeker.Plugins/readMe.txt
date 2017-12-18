插件定义信息类库
1 接口的定义
2 插件的开发：
 2-1 必须继承实现IPlugin接口；
 2-2 必须有插件的自描述文件 MetaManifest.txt;
 2-3 插件的dll 文件名格式： Plugin.*.Extension.dll;
 2-4 插件必须有静态工厂自身实例的方法：InstanceFactory ,一个静态方法，返回当前插件的实例对象