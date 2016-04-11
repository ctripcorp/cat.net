# CAT.net客户端
为.net应用提供接入CAT的API。

CAT.net客户端的API设计、客户端配置方式，与[CAT Java客户端](https://github.com/dianping/cat)基本一致。

### 编译工程
CAT.net客户端要求**.NET Framework 4.0或更高版本**。

用Visual Studio 2010或更高版本，打开Cat\Cat.sln。可以看到Solution中包括两个工程：

1. `Cat`：CAT.net客户端实现代码
2. `CatClientTest`：示例程序和测试用例。

单击Rebuild Solution编译这两个工程：
![Rebuild Solution](doc/rebuild-solution.png)

编译的输出是`Cat.dll`，如下图。在业务应用的工程中，通过引用这个dll，调用其中的API，来接入CAT。
![Cat DLL](doc/cat-dll-location.png)

### 配置
1. 创建以下目录，确保执行CAT客户端的帐户有权限读写它们：
  - `d:\data\appdatas\cat\`  (CAT客户端使用的临时数据目录)
  - `d:\data\applogs\cat\`  (CAT客户端的日志输出目录)
2. 创建`d:\data\appdatas\cat\client.xml`。在其中配置Domain ID和CAT服务器地址。推荐client.xml用**UTF-8**编码。client.xml内容如下：
```
<?xml version="1.0" encoding="utf-8"?>
<config mode="client" enabled="true" queue-size="123">
	<!--logEnabled enabled="true"></logEnabled-->
	
	<!-- 配置Domain ID-->
	<domain id="1237" enabled="true" max-message-size="1000"/>
	
	<servers>
		<!-- 配置CAT服务器地址-->
		<server ip="10.2.6.98" port="2280" http-port="8080"></server>
	</servers>
</config>
```

### 执行工程自带的测试用例
1. 设置CatClientTest工程为默认启动工程：
![Set CatClientTest to be Startup Project](doc/set-startup-project.png)

2. 单击执行，就会运行CatClientTest中的Program.cs的Main()方法。
![Run CatClientTest](doc/run-cat-client-test.png)

程序输出：
![CatClientTest output](doc/catclienttest-output.png)

3. 在CAT中可以看到测试程序的CAT埋点，如下图。其中的CAT服务器地址、Domain ID应该与client.xml中的配置一致。

### 在其他应用中引用`Cat.dll`，调用CAT API
1. 假设我们有一个console应用。
![Create New Console Project](doc/new-console-project.png)

确保工程使用了.NET Framework 4.0或更高版本的服务端Profile，而**不是Client Profile**。
![Use Server Profile](doc/use-server-profile.png)

2. 添加对`Cat.dll`的引用
![Add Reference](doc/add-reference.png)

3. 调用CAT API埋点。示例代码：
```
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.Unidal.Cat.Message;
using Org.Unidal.Cat;
using System.Threading;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            ITransaction transaction = null; ;
            try
            {
                transaction = Cat.NewTransaction("Order", "Cash");

                // Do your business...

                Cat.LogEvent("City", "Shanghai");
                transaction.Status = CatConstants.SUCCESS;
            }
            catch (Exception ex)
            {
            	Cat.LogError(ex);
                transaction.SetStatus(ex);
                
                // You may need to re-throw exception ex out.
            }
            finally
            {
                transaction.Complete();

                // 程序退出前睡一会儿。使得CAT客户端有时间发出最后一批消息到网络。
                Console.Read();
            }
        }
    }
}
```

4. 执行以上Main()方法。
5. 在CAT中可以看到埋点效果：
![Application Output](doc/console-app-output.png)

![Application Output Logview](doc/console-app-output-logview.png)

### 日志输出
1. 在client.xml中，启用`<logEnabled enabled="true"></logEnabled>` XML元素，以开启日志输出。
2. 日志输出位于`D:\data\applogs\cat`目录中：
![Application Log File Path](doc/console-app-log-file-path.png)
