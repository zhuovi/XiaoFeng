# XiaoFeng

![fayelf](https://user-images.githubusercontent.com/16105174/197918392-29d40971-a8a2-4be4-ac17-323f1d0bed82.png)

![GitHub top language](https://img.shields.io/github/languages/top/zhuovi/xiaofeng?logo=github)
![GitHub License](https://img.shields.io/github/license/zhuovi/xiaofeng?logo=github)
![Nuget Downloads](https://img.shields.io/nuget/dt/xiaofeng?logo=nuget)
![Nuget](https://img.shields.io/nuget/v/xiaofeng?logo=nuget)
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/xiaofeng?label=dev%20nuget&logo=nuget)

Nuget：xiaofeng

QQ群号：748408911 

QQ群二维码： 

![QQ 群](https://user-images.githubusercontent.com/16105174/198058269-0ea5928c-a2fc-4049-86da-cca2249229ae.png)

公众号： 

![畅聊了个科技](https://user-images.githubusercontent.com/16105174/198059698-adbf29c3-60c2-4c76-b894-21793b40cf34.jpg)

源码： https://github.com/zhuovi/xiaofeng

教程： https://www.yuque.com/fayelf/xiaofeng

其中包含了Redis,Memcached,Socket,Json,Xml,ADO.NET数据库操作兼容以下数据库（SQLSERVER,MYSQL,ORACLE,达梦,SQLITE,ACCESS,OLEDB,ODBC等数十种数据库）,正则表达式,QueryableX(ORM)和EF无缝对接,FTP,网络日志,调度,IO操作,加密算法(AES,DES,DES3,MD5,RSA,RC4,SHA等常用加密算法),超级好用的配置管理器,应用池,类型转换等功能。

## XiaoFeng

XiaoFeng generator with [XiaoFeng](https://github.com/zhuovi/XiaoFeng).

## Install

.NET CLI

```
$ dotnet add package XiaoFeng --version 3.3.0
```

Package Manager

```
PM> Install-Package XiaoFeng -Version 3.3.0
```

PackageReference

```
<PackageReference Include="XiaoFeng" Version="3.3.0" />
```

Paket CLI

```
> paket add XiaoFeng --version 3.3.0
```

Script & Interactive

```
> #r "nuget: XiaoFeng, 3.3.0"
```

Cake

```
// Install XiaoFeng as a Cake Addin
#addin nuget:?package=XiaoFeng&version=3.3.0

// Install XiaoFeng as a Cake Tool
#tool nuget:?package=XiaoFeng&version=3.3.0
```


# XiaoFeng.Redis

Redis提供了友好的访问API。Redis中间件,支持.NET框架、.NET内核和.NET标准库,一种非常方便操作的客户端工具。实现了Hash,Key,String,ZSet,Stream,Log,订阅发布,线程池功能。

## 基本使用方法

Redis连接串 

```csharp
redis://7092734@127.0.0.1:6379/0?ConnectionTimeout=3000&ReadTimeout=3000&SendTimeout=3000&pool=3
```

7092734	    密码

127.0.0.1	主机

6379		端口

0			0库

ConnectionTimeout	连接超时时长

ReadTimeout		    读取数据超时时长

SendTimeout		    发送数据超时时长

pool			    连接池中连接数量

最小的连接串是：redis://127.0.0.1

实例化一个redis对象

```csharp
var redis = new XiaoFeng.Redis.RedisClient("redis://7092734@127.0.0.1:6379/0");
```

## KEY的基本操作

```csharp

//删除连接串中默认库中 key为 a,b的key
var r1 = await redis.DelKeyAsync("a", "b");
//删除1库中key为 a,b的key
var r2 = await redis.DelKeyAsync(1, "a", "b");
//删除0库中key为a,并返回a的值
var r3 = await redis.GetDelKeyAsync("a", 0);
//序列化key
var r4 = await redis.DumpKeyAsync("a");
//是否存在key
var r5 = await redis.ExistsKeyAsync("a");
//设置过期时间 秒
var r6 = await redis.SetKeyExpireSecondsAsync("a", 100);
//设置过期时间 毫秒
var r7 = await redis.SetKeyExpireMillisecondsAsync("a", 100000);
//设置过期时长    时间戳     秒
var r8 = await redis.SetKeyExpireSecondsTimestampAsync("a", 123456);
//设置过期时长    时间戳     毫秒
var r9 = await redis.SetKeyExpireMillisecondsTimestampAsync("a", 123456);
//重命名key a 到 b
var r10 = await redis.RenameKeyAsync("a", "b");
//重命名key a 到 b 并且b不存在
var r11 = await redis.RenameKeyNoExistsAsync("a", "b");
//移动key 从库 0 至 库 1
var r12 = await redis.MoveKeyAsync("a", 1, 0);
//移除过期时间
var r13 = await redis.RemoveKeyExpireAsync("a");
//获取剩余时间    秒
var r14 = await redis.GetKeyExpireSecondsAsync("a");
//获取剩余时间    毫秒
var r15 = await redis.GetKeyExpireMillisecondsAsync("a");
//从当前数据库随机获取一个key
var r16 = await redis.GetKeyRandomAsync(0);
//获取key值 类型
var r17 = await redis.GetKeyTypeAsync("a");
//复制一个key
var r18 = await redis.CopyKeyAsync("a", "b");
//查找 key 支持 * 和? * 代表好多个字符  ?代表一个字符
var r19 = await redis.GetKeysAsync("*a");
//获取当前库 key 的数量
var r20 = await redis.GetDbKeySizeAsync();
//查询当前库中符合模式的key
var r21 = await redis.GetKeysAsync("*a");
//获取key为a的值类型
var r22 = await redis.GetKeyTypeAsync("a");
//删除当前库key为a b的数据
var r23 = await redis.DelKeyAsync("a", "b");
//删除当前库所有key
var r24 = await redis.DelDbKeysAsync();
//删除所有库的所有key
var r25 = await redis.DelAllKeysAsync();

```

## 字符串类型操作

```csharp

//设置key a 的值 为 "abc"
var s1 = await redis.SetStringAsync("a", "abc");
//批量设置 key 和 值
var s2 = await redis.SetStringAsync(new Dictionary<string, object>
{
    { "a","abc" },
    { "b",DateTime.Now },
    { "c",11110 },
    { "d",123.23 },
    { "e",Guid.NewGuid() },
    { "f",new List<string>{"a","b","c","d"} }
    });
//设置key a 的 值 为 b 并且 a不存在时才成功
var s3 = await redis.SetStringNoExistsAsync("a", "b");
//获取key a ,b 的值
var s4 = await redis.GetStringAsync("a", "b");
//获取key a 的值 的长度
var s5 = await redis.GetStringLengthAsync("a");
//设置 key a 的值 为 111 并返回 原来的值
var s6 = await redis.GetSetStringAsync("a", 111);
//自动加1
var s7 = await redis.StringIncrementAsync("a");
//自动减1
var s8 = await redis.StringDecrementAsync("a");
```

## Hash操作

```csharp

//设置hash key为a 字段名为 b 值为 c
var h1 = await redis.SetHashAsync("a", "b", "c");
//批量设置has key为a
var h2 = await redis.SetHashAsync("a", new Dictionary<string, object>
{
    { "a","abc" },
    { "b",DateTime.Now },
    { "c",11110 },
    { "d",123.23 },
    { "e",Guid.NewGuid() },
    { "f",new List<string>{"a","b","c","d"} }
});
//设置hash key 为a 字段名为b 值为 c 只有字段名b不存在时才设置成功
var h3 = await redis.SetHashNoExistsAsync("a", "b", "c");
//删除hash key 为 a 中的字段名为 "b","c"的键值
var h4 = await redis.DelHashAsync("a", "b", "c");
//查询hash key 为 a 中的字段为 "b" 是否存在
var h5 = await redis.ExistsHashAsync("a", "b");
//设置hash key 为 a 中的字段为 "b" 的值增加2 如果减则输入负数即可
var h6 = await redis.HashIncrementAsync("a", "b", 2);
```

## List操作

```csharp

//设置List key 为 a  值为 "b","c","d"
var l1 = await redis.SetListItemAsync("a", "b", "c", "d");
//设置List key 为 a 索引为2 的值 为 111
var l2 = await redis.SetListItemAsync("a", 2, 111);
//在List Key 为 a 的最前边加入 "b","c","d"值
var l3 = await redis.SetListItemBeforeAsync("a", "b", "c", "d");
//将111插入到已存在key 为a的List中最前边 不存在则不插入
var l4 = await redis.SetListItemBeforeExistsAsync("a", 111);
//将"c"插入到已存在key 为a的List中最后边 不存在则不插入
var l5 = await redis.SetListItemExistsAsync("a", "c");
//从 List key为a中从头到尾搜索3个值为"c"的数据 然后移除 如果想从尾到头搜索则变成负数即可 如 -3
var l6 = await redis.DelListItemAsync("a", "c", 3);
//从 List key为a中移除索引从2到5的数据
var l7 = await redis.DelListItemAsync("a", 2, 5);
//获取List中key为a的值数量
var l8 = await redis.GetListCountAsync("a");
//移出并获取列表的第一个元素
var l9 = await redis.GetListFirstItemAsync("a");
//通过索引获取列表中的元素
var l10 = await redis.GetListItemAsync("a", 2);
//获取List中key为a的 索引从2到5的值
var l11 = await redis.GetListItemsAsync("a", 2, 5);
//List 从key为a的尾部取出数据并插入到key为b的头部
var l12 = await redis.GetListLastItemToOtherListFirstAsync("a", "b");
```

## Set操作

```csharp

//向Set中key为"a"的插入"b","c"成员
var set1 = await redis.SetSetMemberAsync("a", "b", "c");
//获取Set中key为a的值数量
var set2 = await redis.GetSetCountAsync("a");
//获取List中key 为a和b 值的差集
var set3 = await redis.GetSetDiffAsync("a", "b");
//获取List中key 为a和b 值的差集 并把差集存到key为c中
var set4 = await redis.GetSetDiffStoreAsync("a", "c", "b");
//获取List中key 为a和b 值的交集
var set5 = await redis.GetSetInterAsync("a", "b");
//获取List中key 为a和b 值的交集 并把差集存到key为c中
var set6 = await redis.GetSetInterStoreAsync("a", "c", "b");
//获取Set中 key为a的所有成员
var set7 = await redis.GetSetMemberListAsync("a");
//移除并返回集合中的一个或多个随机元素
var set8 =await redis.GetSetPopAsync("a");
//获取集合中一个或多个随机数
var set9 = await redis.GetSetRandomMemberAsync("a");
//删除集合中的b c 成员
var set10 = await redis.DelSetMemberAsync("a", "b", "c");
//判断成员b是否是a集合的成员
var set11 = await redis.ExistsSetMemberAsync("a", "b");
//获取集合 a b的并集
var set12 = await redis.GetSetUnionAsync("a", "b");
//获取集合 a b的并集 并存到集合c中
var set13 = await redis.GetSetUnionStoreAsync("a", "c", "b");
```

## Sorted set 操作 和Set 操作基本相似

```csharp

//在这里就不再举例

```

# XiaoFeng.Memcached

XiaoFeng.Memcached提供了友好的访问API。Memcached中间件,支持.NET框架、.NET内核和.NET标准库,一种非常方便操作的客户端工具。实现了Set,Add,Replace,PrePend,Append,Cas,Get,Gets,Gat,Gats,Delete,Touch,Stats,Stats Items,Stats Slabs,Stats Sizes,Flush_All,线程池功能。

Memcached缓存数据库连接驱动

Memcached连接串 

```csharp
memcached://memcached:123456@127.0.0.1:11211?ConnectionTimeout=3000&ReadTimeout=3000&SendTimeout=3000&pool=3
```

memcached   帐号

123456      密码

127.0.0.1	主机

11211		端口

ConnectionTimeout	连接超时时长

ReadTimeout		    读取数据超时时长

SendTimeout		    发送数据超时时长

pool			    连接池中连接数量

最小的连接串是：memcached://127.0.0.1

实例化一个memcached对象

```csharp
var memcached = new XiaoFeng.Memcached.MemcachedClient("memcached://memcached:123456@127.0.0.1:11211");
```

#使用方法

```csharp
//实例化
var memcached = new XiaoFeng.Memcached.MemcachedClient("memcached://memcached:123456@127.0.0.1:11211");
//最大压缩比
memcached.CompressLength = 1024;
//协议
memcached.MemcachedProtocol = MemcachedProtocol.Text;
//Hash算法
memcached.Transform = new XiaoFeng.Memcached.Transform.FNV1_64();

//给key设置一个值
var set = memcached.Set("abc", "abcda");
//如果key不存在的话，就添加
var add1 = memcached.Add("abc", "abcde");
//如果key不存在的话，就添加
var add2 = memcached.Add("a1", "abcde");
//如果key不存在的话，就添加 异步
var add3 = await memcached.AddAsync("a2", "abcde");
//用来替换已知key的value
var replace1 = memcached.Replace("a3", "abc3");
//用来替换已知key的value
var replace2 = memcached.Replace("a2", "abc3");
//表示将提供的值附加到现有key的value之后，是一个附加操作
var append1 = memcached.Append("a3", "a4f");
//表示将提供的值附加到现有key的value之后，是一个附加操作
var append2 = memcached.Append("a2", "a2f");
//将此数据添加到现有数据之前的现有键中
var prepend1 = memcached.Prepend("a3", "a3d");
//将此数据添加到现有数据之前的现有键中
var prepend2 = memcached.Prepend("a2", "a3d");
//一个原子操作，只有当casunique匹配的时候，才会设置对应的值
var cas = memcached.Cas("a1", "aaa", 113);
//获取key的value值，若key不存在，返回空。
var get1 = memcached.Get("a1");
//获取key的value值，若key不存在，返回空。
var get2 = memcached.Get("a1", "a2");
//用于获取key的带有CAS令牌值的value值，若key不存在，返回空。
var gets1 = memcached.Gets("a1");
//用于获取key的带有CAS令牌值的value值，若key不存在，返回空。
var gets2 = memcached.Gets("a1", "a2");
//获取key的value值，若key不存在，返回空。更新缓存时间
var gat = memcached.Gat(5*24 * 60, "a1");
//获取key的value值，若key不存在，返回空。更新缓存时间
var gat1 = memcached.Gat(6*24 * 60, "a1","a2");
//用于获取key的带有CAS令牌值的value值，若key不存在，返回空。支持多个key 更新缓存时间
var gats = memcached.Gats(5 * 24 * 60, "a1");
//用于获取key的带有CAS令牌值的value值，若key不存在，返回空。支持多个key 更新缓存时间
var gats1 = memcached.Gats(6 * 24 * 60, "a1", "a2");
//删除已存在的 key(键)
var delete = memcached.Delete("a10");
//给key设置一个值
var set3 = memcached.Set("a10", 100);
//递增
var incr1 = memcached.Increment("a10", 10);
//递减
var decr1 = memcached.Decrement("a10", 10);
//修改key过期时间
var touch = memcached.Touch("a10", 24 * 60);
//统计信息
var stats = memcached.Stats();
//显示各个 slab 中 item 的数目和存储时长(最后一次访问距离现在的秒数)
var items = memcached.StatsItems();
//显示各个slab的信息，包括chunk的大小、数目、使用情况等
var slabs = memcached.StatsSlabs();
//显示所有item的大小和个数
var sizes = memcached.StatsSizes();
//用于清理缓存中的所有 key=>value(键=>值) 对
var flushall = memcached.FulshAll();
```

# XiaoFeng.HttpHelper

HttpHelper 是Http模拟请求库。

## 使用操作

* GET 请求

``` csharp
var result = await XiaoFeng.Http.HttpHelper.GetHtmlAsync(new XiaoFeng.Http.HttpRequest
{
    Method = HttpMethod.Get,//不设置默认为Get请求
    Address = "http://www.fayelf.com"
});
if (result.StatusCode == System.Net.HttpStatusCode.OK)
{
    /*请求成功*/
    //响应内容
    var value = result.Html;
    //响应内容字节集
    var bytes = result.Data;
}
else
{
    /*请求失败*/
}

```

* POST 表单请求

``` csharp
var result = await XiaoFeng.Http.HttpHelper.GetHtmlAsync(new XiaoFeng.Http.HttpRequest
{
    Method = HttpMethod.Post,
    Address = "http://www.fayelf.com",
    Data=new Dictionary<string, string>
    {
        {"account","jacky" },
        {"password","123456" }
    }
});
if (result.StatusCode == System.Net.HttpStatusCode.OK)
{
    /*请求成功*/
    //响应内容
    var value = result.Html;
    //响应内容字节集
    var bytes = result.Data;
}
else
{
    /*请求失败*/
}
```

* POST BODY请求

``` csharp

var result = await XiaoFeng.Http.HttpHelper.GetHtmlAsync(new XiaoFeng.Http.HttpRequest
{
    Method = HttpMethod.Post,
    ContentType="application/json",
    Address = "http://www.fayelf.com",
    BodyData=@"{""account"":""jacky"",""password"":""123456""}"
});
if (result.StatusCode == System.Net.HttpStatusCode.OK)
{
    /*请求成功*/
    //响应内容
    var value = result.Html;
    //响应内容字节集
    var bytes = result.Data;
}
else
{
    /*请求失败*/
}

```

* POST FORMDATA 请求，就是有表单输入数据也有文件流数据

``` csharp
var result = await XiaoFeng.Http.HttpHelper.GetHtmlAsync(new XiaoFeng.Http.HttpRequest
{
    Method = HttpMethod.Post,
    ContentType = "application/x-www-form-urlencoded",
    Address = "http://www.fayelf.com",
    FormData = new List<XiaoFeng.Http.FormData>
    {
        new XiaoFeng.Http.FormData
        {
             Name="account",Value="jacky", FormType= XiaoFeng.Http.FormType.Text
        },
        new XiaoFeng.Http.FormData
        {
            Name="password",Value="123456",FormType= XiaoFeng.Http.FormType.Text
        },
        new XiaoFeng.Http.FormData
        {
            Name="headimage",Value=@"E://Work/headimage.png", FormType= XiaoFeng.Http.FormType.File
        }
    }
});
if (result.StatusCode == System.Net.HttpStatusCode.OK)
{
    /*请求成功*/
    //响应内容
    var value = result.Html;
    //响应内容字节集
    var bytes = result.Data;
}
else
{
    /*请求失败*/
}

```

* 下载文件

``` csharp

await XiaoFeng.Http.HttpHelper.Instance.DownFileAsync(new XiaoFeng.Http.HttpRequest
{
    Method = HttpMethod.Get,
    Address = "http://www.fayelf.com/test.rar"
}, @"E:/Work/test.rar");

```

# 万能的类型转换扩展方法 ToCast<T>()

当前方法 可转任何值 类型 包括 对象类型 数组类型.
在转换方法前 首选会验证当前值 类型和要转换的类型是否相同，接着就是验证 它是否符合目标类型的格式 如果不符合会转换成目标类型的默认值 也可以设置默认值。



## 使用方法

```csharp
using XiaoFeng;

int a = "10".ToCast<int>();
Int64 b = "10".ToCast<Int64>();
double c = "10".ToCast<double>();
DateTime d = "2022-01-19".ToCast<DateTime>();
float e = "".ToCast<float>(1.0);
int f = (int)"".GetValue(typeof(int));
Guid g = "58AFBEB5791311ECBF49FA163E542B11".ToCast<Guid>();
Guid h = "58AFBEB5-7913-11EC-BF49-FA163E542B11".ToCast<Guid>();
long j = "a".ToCast<long>(100);

```

* 也可以用下边的方法

```csharp
Int16 a = "1".ToInt16();
int b = "2".ToInt32();
Int64 c = "3".ToInt64();
UInt16 d = "4".ToUInt16();
UInt32 e = "5".ToUInt32();
UInt64 f ="6".ToUInt64();
float e = "7.2".ToFloat();
DateTime g = "2022-01-19 12:32".ToDateTime();
double h = "6.3".ToDouble();
byte i = "2".ToByte();
Boolean j = "1".ToBoolean();
Boolean k = "true".ToBoolean();
Boolean l = "False".ToBoolean();
Decimal m = "3.658".ToDecimal();
long n = "2584512".ToLong();
Guid o = "58AFBEB5791311ECBF49FA163E542B11".ToGuid();
Guid p = "58AFBEB5-7913-11EC-BF49-FA163E542B11".ToGuid();

```
# 数据库操作 DataHelper

* XiaoFeng.Data.DataHelper，当前类库支持SQLSERVER,MYSQL,ORACLE,达梦,SQLITE,ACCESS,OLEDB,ODBC等数十种数据库。

## 使用说明

简单实例

```csharp
var data = new XiaoFeng.Data.DataHelper(new XiaoFeng.Data.ConnectionConfig
{
    ProviderType= XiaoFeng.Data.DbProviderType.SqlServer,
    ConnectionString= "server=.;uid=testuser;pwd=123;database=Fay_TestDb;"
});
var dt = data.ExecuteDataTable("select * from F_Tb_Account;");
```
1. 直接执行SQL语句

```csharp
var non1 = data.ExecuteNonQuery("insert into F_Tb_Account(Account,Password) values('jacky','admin');");
```
non1值，如果non1是-1则表示 执行出错，可以通过data.ErrorMessage拿到最后一次执行出错的错误信息
如果non1是大于等于0则表示执行SQL语句后所执行的行数。

2. 返回DataTable

```csharp
var dt = data.ExecuteDataTable("select * from F_Tb_Account;");
```
dt就是一个datatable 。

3. 直接返回首行首列

```csharp
var val1 = data.ExecuteScalar("select Acount from F_Tb_Account;");
```
val1类型是object对象，根据数据库的值不同我们可以自定义转换如：var val2 = (int)val1;也可以用XiaoFeng自带的扩展方法,var val2 = val1.ToCast<int>();

4. 直接返回DataReader

```csharp
var dataReader = data.ExecuteReader("select * from F_Tb_Account;");
```
dataReader就是DataReader对象。

5. 直接返回DataSet

```csharp
var dataSet = data.ExecuteDataSet("select * from F_Tb_Account;select * from F_Tb_Account;");
```
dataSet就是DataSet对象。

6. 执行存储过程

```csharp
var data = data.ExecuteDataTable("proc_name", System.Data.CommandType.StoredProcedure, new System.Data.Common.DbParameter[]
{
    data.MakeParam(@"Account","jacky")
});
```

7. SQL语句带存储参数

```csharp
var data2 = data.ExecuteDataTable("select * from F_Tb_Account where Account=@Account;", new System.Data.Common.DbParameter[]
{
    data.MakeParam(@"@Account","jacky")
});
```

8. 直接转换成对象

```csharp
var models = data.QueryList<Account>("select * from F_Tb_Account");
var model = data.Query<Account>("select * from F_Tb_Account");
```

# 正则表达式 扩展方法

字符串匹配，提取，是否符合规则，替换，移除等都可用是正则表达式来实现的。

## 使用说明

* IsMatch 扩展方法 主要是当前字符串是否匹配上正则表达式，比如，匹配当前字符串是否是QQ号码，代码如下：

```csharp
if("7092734".IsMatch(@"^\d{5-11}$"))
    Console.WriteLine("是QQ号码格式.");
else
    Console.WriteLine("非QQ号码格式.");
```

输出结果为：是QQ号码格式。

因为字符串 "7092734"确实是QQ号码。

IsNotMatch 扩展方法其实就是 !IsMatch,用法和IsMatch用法一样。

Match 扩展方法返回的是Match,使用指定的匹配选项在输入字符串中搜索指定的正则表达式的第一个匹配项。

Matches 当前扩展方法返回的是使用指定的匹配选项在指定的输入字符串中搜索指定的正则表达式的所有匹配项。

这三个方法是最原始最底层的方法，其它扩展都基于当前三个方法中的一个或两个来实现的。

GetMatch 扩展方法返回结果是：提取符合模式的数据所匹配的第一个匹配项所匹配的第一项或a组的数据。

GetPatterns 扩展方法返回结果是：提取符合模式的数据所有匹配的第一项数据或a组数据。

GetMatchs 扩展方法返回结果是：提取符合模式的数据所匹配的第一项中所有组数据。

GetMatches 扩展方法返回结果是：提取符合模式的数据所有匹配项或所有组数据。

提取的数据量对比：GetMatch<GetMatchs<GetPatterns<GetMatches 。

ReplacePattern 扩展方法用途是使用正则达式来替换数据。

下边通过实例来讲解这几个方法的使用及返回结果的区别：

```csharp
var a = "abc4d5e6hh5654".GetMatch(@"\d+");
a的值为："4";
var b = "abc4d5e6hh5654".GetPatterns(@"\d+");
b的值为：["4","5","6","5654"];
var c = "abc4d5e6hh5654".GetMatchs(@"(?<a>[a-z]+)(?<b>\d+)");
c的值为：{{"a","abc"},{"b","4"}};
var d = "abc4d5e6hh5654".GetMatches(@"(?<a>[a-z]+)(?<b>\d+)");
d的值为：[{{"a","abc"},{"b","4"}},{{"a","d"},{"b","5"}},{{"a","e"},{"b","6"}},{{"a","hh"},{"b","5654"}}]
var g = "a6b9c53".ReplacePattern(@"\d+","g");
g的值为："agbgcg";
var h = "a6b7c56".RemovePattern(@"\d+");
h的值为："abc";
var i = "a1b2c3".ReplacePattern(@"\d+",m=>{
   var a = a.Groups["a"].Value;
    if(a == "1")return "a1";
    else return "a2";
});
i的值为："aa1ba2ca2";
```

# 作业调度

作业调度其实就是一个定时器，定时完成某件事，比如：每分钟执行一次，每小时执行一次，每天执行一次，第二周几执行，每月几号几点执行，间隔多少个小时执行一次等。

作业类：XiaoFeng.Threading.Job

主调度类：XiaoFeng.Threading.JobScheduler

## 使用说明

1. 定时只执行一次也就是多久后执行

```csharp
var job = new XiaoFeng.Threading.Job
{
     Async = true,
     Name="作业名称",
      TimerType= XiaoFeng.Threading.TimerType.Once,
        StartTime= DateTime.Now.AddMinutes(5),
    SuccessCallBack = job =>
    {
        /*到时间执行任务*/
    }
};
job.Start();
```
当前作业为5 分钟后执行一次，然后就是销毁，作业从调度中移除。

2. 间隔执行

```csharp
var job = new XiaoFeng.Threading.Job
{
    Async = true,
    Name = "作业名称",
    TimerType = XiaoFeng.Threading.TimerType.Interval,
    Period = 5000,
    StartTime = DateTime.Now.AddMinutes(5),
    SuccessCallBack = job =>
    {
        /*到时间执行任务*/
    }
};
job.Start();
```
当前作业为，5分钟后运行，然后每隔5分钟会再执行一次。

3. 每天定时执行一次

```csharp
var job = new XiaoFeng.Threading.Job
{
    Async = true,
    Name = "作业名称",
    TimerType = XiaoFeng.Threading.TimerType.Day,
    Time = new XiaoFeng.Threading.Time(2, 0, 0),
    StartTime = DateTime.Now.AddMinutes(5),
    SuccessCallBack = job =>
    {
        /*到时间执行任务*/
    }
};
```
当前作业为，5分钟后运行，然后每天2点执行一次。

4. 每周几几点执行,每月几号几点执行

```csharp
var job = new XiaoFeng.Threading.Job
{
    Async = true,
    Name = "作业名称",
    TimerType = XiaoFeng.Threading.TimerType.Week,
    DayOrWeekOrHour = new int[] { 1, 4 },
    Time = new XiaoFeng.Threading.Time(2, 0, 0),
    StartTime = DateTime.Now.AddMinutes(5),
    SuccessCallBack = job =>
    {
        /*到时间执行任务*/
    }
};
job.Start();
```

当前作业为，5分钟后运行，然后每周的周一，周四的两点执行一次。

# XiaoFeng.Ftp

FTP客户端

```csharp
var ftp = new XiaoFeng.Ftp.FtpClient(new XiaoFeng.Ftp.FtpClientConfig
{
     Host= "127.0.0.1",
     Port= 21
});
//上传一个文件
var a = await ftp.PutAsync(@"E://a/a.txt");
//上传一个文件到指定目录
var b = await ftp.UploadFileAsync(@"E://a/a.txt", "/a/a.txt");
//上传一个文件夹到指定目录
await ftp.UploadFolderAsync(@"E://a", "/a");
//下载一个文件到指定目录
var c = await ftp.DownFileAsync(@"/a/a.txt", @"E://a", "a.txt");
//获取文件夹列表
var d = await ftp.GetDirAsync("a");
//获取文件夹列表-详细
var e = await ftp.GetDirListAsync("a");
//获取文件详细信息
var f = await ftp.GetFileInfoAsync("a/a.txt");
//获取文件大小
var g = await ftp.GetFileSizeAsync(@"a/a/txt");
//改变文件目录
var h = await ftp.ChangeDirectoryAsync("/b");
//删除文件夹
var i = await ftp.DeleteAsync("a");
//删除文件
var j = await ftp.DeleteAsync("a/a.txt");
//当前目录
var k = ftp.RemoteDirectory;
```

# XiaoFeng.Xml Xml序列化

XML序列化操作就是把一个数据对象序列化成XML格式的数据，反序列化操作就是把一个XML格式的数据反序列化成一个数据对象。
命名空间:XiaoFeng.Xml
先看序列化配置

```csharp
/// <summary>
/// 序列化设置
/// </summary>
public class XmlSerializerSetting
{
    #region 构造器
    /// <summary>
    /// 无参构造器
    /// </summary>
    public XmlSerializerSetting()
    {

    }
    #endregion

    #region 属性
    /// <summary>
    /// Guid格式
    /// </summary>
    public string GuidFormat { get; set; } = "D";
    /// <summary>
    /// 日期格式
    /// </summary>
    public string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss.fff";
    /// <summary>
    /// 是否格式化
    /// </summary>
    public bool Indented { get; set; } = true;
    /// <summary>
    /// 枚举值
    /// </summary>
    public EnumValueType EnumValueType { get; set; } = 0;
    /// <summary>
    /// 解析最大深度
    /// </summary>
    public int MaxDepth { get; set; } = 28;
    /// <summary>
    /// 是否写注释
    /// </summary>
    public bool OmitComment { get; set; } = true;
    /// <summary>
    /// 忽略大小写 key值统一变为小写
    /// </summary>
    public bool IgnoreCase { get; set; } = false;
    /// <summary>
    /// 默认根目录节点名称
    /// </summary>
    public string DefaultRootName { get; set; } = "Root";
    /// <summary>
    /// 默认编码
    /// </summary>
    public Encoding DefaultEncoding { get; set; } = Encoding.UTF8;
    /// <summary>
    /// 获取或设置一个值，该值指示是否 System.Xml.XmlWriter 编写 XML 内容时应移除重复的命名空间声明。 写入器的默认行为是输出写入器的命名空间解析程序中存在的所有命名空间声明。
    /// </summary>
    public NamespaceHandling NamespaceHandling { get; set; }
    /// <summary>
    /// 是否忽略输出XML声明
    /// </summary>
    public Boolean OmitXmlDeclaration { get; set; } = false;
    /// <summary>
    /// 获取或设置要用于换行符的字符串。要用于换行符的字符串。 它可以设置为任何字符串值。 但是，为了确保 XML 有效，应该只指定有效的空格字符，例如空格、制表符、回车符或换行符。 默认值是\r\n （回车符、 换行符）。
    /// </summary>
    public string NewLineChars { get; set; } = Environment.NewLine;
    /// <summary>
    /// 是否忽略数组项未指定KEY的项用节点名称代替
    /// </summary>
    public Boolean OmitArrayItemName { get; set; } = true;
    /// <summary>
    /// 是否忽略空节点
    /// </summary>
    public Boolean OmitEmptyNode { get; set; } = true;
    /// <summary>
    /// 是否忽略命名空间
    /// </summary>
    public Boolean OmitNamespace { get; set; } = true;
    #endregion
}
```

简单使用，扩展了两个方法 EntityToXml(),XmlToEntity();
先看 XMl模型对象

``` csharp
/// <summary>
/// XmlModel 类说明
/// </summary>
[XmlRoot("Root")]
public class XmlModel
{
    #region 构造器
    /// <summary>
    /// 无参构造器
    /// </summary>
    public XmlModel()
    {

    }
    #endregion

    #region 属性
    /// <summary>
    /// 属性1
    /// </summary>
    [XmlCData]
    [XmlElement("NameA")]
    public string FieldName1 { get; set; }
    /// <summary>
    /// 属性2
    /// </summary>
    [XmlConverter(typeof(XiaoFeng.Xml.StringEnumConverter))]
    [XmlElement("NameB")]
    public EnumValueType FieldName2 { get; set;}
    /// <summary>
    /// 属性3
    /// </summary>
    [XmlConverter(typeof(XiaoFeng.Xml.DescriptionConverter))]
    [XmlElement("Namec")]
    public EnumValueType FieldName3 { get; set; }
    /// <summary>
    /// 属性4
    /// </summary>
    public string FieldName4 { get; set; }
    #endregion

    #region 方法

    #endregion
}
```
简单使用

```csharp
var a = new XmlModel
    {
        FieldName1 = "Value1",
        FieldName2 = EnumValueType.Name,
        FieldName3 = EnumValueType.Value,
        FieldName4 = "Value4"
    }.EntityToXml();
//XmlSerializer.Serializer(a) 和a.EntityToXml()是一样的
```
//输出结果
```xml
<?xml version="1.0" encoding="utf-8"?>
<Root>
  <FieldName1><![CDATA[Value1]]></FieldName1>
  <NameB>Name</NameB>
  <Namec>值</Namec>
  <FieldName4>Value4</FieldName4>
</Root>
var b = a.XmlToEntity<XmlModel>();
//XmlSerializer.Deserialize<XmlModel>(a) 和XmlToEntity<XmlModel>()是一样的
```
接下来讲一下序列化时的几个特性

```csharp
//忽略属性值
XmlIgnoreAttribute
//指定节点名称
XmlElementPath
//转换类型
XmlConverterAttribute
//枚举转换器
StringEnumConverter
//说明转换器
DescriptionConverter
```
下边举例讲一下XmlElementPath的使用,当前属性仅支持反序列化时使用，序列化时暂时还不支持当前属性。假设下边有一个 这样的xml

```xml
<?xml version="1.0" encoding="utf-8"?>
<Root>
  <NameA>
    <NameD><![CDATA[Value1]]><NameD>
    <NameC>bbb</NameC>
  </NameA>
  <NameB>Name</NameB>
  <Namec>值</Namec>
  <FieldName4>Value4</FieldName4>
</Root>
```
按正常定义模型时 NameA 子节点 A  B 要定义到一个类中 
实际在这里可以这样定义

```csharp
/// <summary>
  /// XmlModel 类说明
  /// </summary>
  [XmlRoot("Root")]
  public class XmlModel
  {
      #region 构造器
      /// <summary>
      /// 无参构造器
      /// </summary>
      public XmlModel()
      {

      }
      #endregion

      #region 属性
      /// <summary>
      /// 属性1
      /// </summary>
      [XmlCData]
      [XmlElementPath("NameA/NameC")]
      public string A { get; set; }
      /// <summary>
      /// 属性1
      /// </summary>
      [XmlCData]
      [XmlElementPath("NameA/NameD")]
      public string B { get; set; }
      /// <summary>
      /// 属性2
      /// </summary>
      [XmlConverter(typeof(XiaoFeng.Xml.StringEnumConverter))]
      [XmlElement("NameB")]
      public EnumValueType FieldName2 { get; set;}
      /// <summary>
      /// 属性3
      /// </summary>
      [XmlConverter(typeof(XiaoFeng.Xml.DescriptionConverter))]
      [XmlElement("Namec")]
      public EnumValueType FieldName3 { get; set; }
      /// <summary>
      /// 属性4
      /// </summary>
      public string FieldName4 { get; set; }
      #endregion

      #region 方法

      #endregion
  }
```
反序列化结果为:

就是可以直接从子节点取数据反序列化到对象上，不用再单独去定义子模型了。
如果不想定义模型，则XiaoFeng.Xml中提供了一个万能的对象模型就是XmlValue对象。
Xml序列化，反序列化就讲到这里，具体操作还需要自己去实践操作。

# XiaoFeng.Json Json序列号

JSON序列化操作，就是把数据对象序列化成JSON数据，也可以把JSON数据反序列化成数据对象。
命名空间是：XiaoFeng.Json
序列化方法 JsonParser.SerializeObject 也可以用扩展方法 ToJson()
反序列化方法 JsonParser.JsonParser.DeserializeObject<T>() 也可以使用JsonToObject()
简单使用，看代码

```csharp
//序列化
var a = new {
        key1="value1",
        key2 ="value2"
    }.ToJson();
//a的值就是：{"key1":"value1","key2":"value2"}
//反序列化
var b = @"{""key1"":""value1"",""key2"":""value2""}.JsonToObject();
//b的值就是：一个字典形式

```
上边用的是一个匿名对象，反序列化回来的时候因为没有设置对应的类型，所以自动转换成JsonValue类型的值；
下边详细介绍一下 序列配置,在使用Tojson,JsonToObject扩展方法时可以设置配置参数的。配置参数如下：

```csharp
/// <summary>
/// Json格式设置
/// </summary>
public class JsonSerializerSetting
{
    #region 构造器
    /// <summary>
    /// 无参构造器
    /// </summary>
    public JsonSerializerSetting() { }
    #endregion

    #region 属性
    /// <summary>
    /// Guid格式
    /// </summary>
    public string GuidFormat { get; set; } = "D";
    /// <summary>
    /// 日期格式
    /// </summary>
    public string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss.fff";
    /// <summary>
    /// 是否格式化
    /// </summary>
    public bool Indented { get; set; } = false;
    /// <summary>
    /// 枚举值
    /// </summary>
    public EnumValueType EnumValueType { get; set; } = 0;
    /// <summary>
    /// 解析最大深度
    /// </summary>
    public int MaxDepth { get; set; } = 28;
    /// <summary>
    /// 是否写注释
    /// </summary>
    public bool IsComment { get; set; } = false;
    /// <summary>
    /// 忽略大小写 key值统一变为小写
    /// </summary>
    public bool IgnoreCase { get; set; } = false;
    /// <summary>
    /// 忽略空节点
    /// </summary>
    public bool OmitEmptyNode { get; set; } = false;
    #endregion
}
```
接下来讲一下序列化时的几个特性

```csharp
//忽略属性值
JsonIgnoreAttribute
//指定节点名称
JsonElement
//转换类型
JsonConverterAttribute
//枚举转换器
StringEnumConverter
//说明转换器
DescriptionConverter
```

下边通过实例讲解一下;下面是一个定义好的JSON模型

```csharp
/// <summary>
/// JsonModel 类说明
/// </summary>
public class JsonModel
{
    #region 构造器
    /// <summary>
    /// 无参构造器
    /// </summary>
    public JsonModel()
    {

    }
    #endregion

    #region 属性
    /// <summary>
    /// 属性1
    /// </summary>
    [JsonElement("NameA")]
    public string FieldName1 { get; set; }
    /// <summary>
    /// 属性2
    /// </summary>
    [JsonConverter(typeof(XiaoFeng.Json.DescriptionConverter))]
    [JsonElement("NameB")]
    public EnumValueType FieldName2 { get; set; }
    /// <summary>
    /// 属性3
    /// </summary>
    [JsonConverter(typeof(XiaoFeng.Json.StringEnumConverter))]
    [JsonElement("NameC")]
    public EnumValueType FieldName3 { get; set; }
    /// <summary>
    /// 属性4
    /// </summary>
    [JsonElement("NameD")]
    public EnumValueType FieldName4 { get; set; }
    #endregion

    #region 方法

    #endregion
}
//使用时
var a = new JsonModel
{
    FieldName1 = "aaaa",
    FieldName2 = EnumValueType.Name,
    FieldName3 = EnumValueType.Value,
	FieldName4 = EnumValueType.Value
}.ToJson();
//当前转换成JSON是：{"NameA":"aaaa","NameB":"名称","NameC":"Value","NameD":0}
//因为 FieldName1 被设置成了NameA FieldName2被设置成了NameB FieldName3被设置成了NameC FieldName4无设置
//两个枚举值不一样，是因为第二个设置的是读取Description内容 就是EnumValueType.Name的属性值DescriptionAttribute中设置的值
//第三个取的是Value是因为设置取的是StringEnumConverter 所以直接就转换成了名称，如果不设置则直接输出对应的数字
//反序列化时也是这样对应到实体模型中去
```

Json序列化，反序列化就讲到这里，具体操作还需要自己去实践操作。


# XiaoFeng.Excel 操作

```csharp

var excel = new XiaoFeng.Excel.Workbook();
//打开excel
excel.Open(@"E://a.xlsx");
///excel中所有的表
var sheets = excel.Sheets;
//第一张表中所有行
var rows = excel.Sheets.FirstOrDefault().Rows;
//第一张表中第一行所有列
var cells = rows.FirstOrDefault().Cells;
//单元格值
var cellValue = cell.Value;
//单元格位置
var location = cell.Location;
//单元格格式
var format = cell.Format;
//单元格所在行
var cellRow = cell.Row;
//单元格所在列
var cellColumn = cell.Column;

```

# XiaoFeng.Net Socket操作

## 服务端实例

```csharp
//新建一个服务端,同时支持websocket,socket客户端连接
var SocketServer = new XiaoFeng.Net.NetServer<XiaoFeng.Net.ServerSession>(8888)
{
    //是否启用ping
    IsPing = true,
    //是否启用新行
    IsNewLine = true,
    //传输编码
    Encoding = System.Text.Encoding.UTF8
};
SocketServer.SocketAuth = session =>
{
    //判断 客户端是否符合认证,不符合则直接返回false即可
    return true;
};
SocketServer.OnClientError += (session,e)=>
{
    //客户端出错事件
};
SocketServer.OnDisconnected += (session,e)=>
{
    //断开连接事件
};
SocketServer.OnError += (session, e) =>
{
    //服务端出错事件
};
SocketServer.OnNewConnection += (session, e) =>
{
    //有新的连接事件
};
SocketServer.OnMessageByte += (session,message,e)=>
{
    //接收消息事件

    session.Send("回复客户端消息");
};
SocketServer.OnStart += (socket,e)=>
{
    //服务端启动事件
};
SocketServer.OnStop += (socket,e)=>
{
    //服务端停止事件
};
//启动监听
SocketServer.Start();
//发送消息给所有客户端
SocketServer.Send("发送消息");
//添加黑名单
SocketServer.AddIpBlack("124.2.2.2");
```

## 客户端实例

```csharp
var SocketClient = new XiaoFeng.Net.NetClient<XiaoFeng.Net.ClientSession>("127.0.0.1", 8888);
SocketClient.OnStart += (socket, e) =>
{
    //启动消息
};
SocketClient.OnClose += (socket,e)=>
{
    //关闭消息
};
SocketClient.OnDisconnected += (session,e)=>{
    //断开连接消息
};
SocketClient.OnError += (socket,e)=>
{
    //出错消息
};

SocketClient.OnMessageByte += (session,message,e)=>
{
//接收信息
};

//启动客户端
SocketClient.Start();

SocketClient.Send("发送消息");
SocketClient.Subscribe("订阅频道");

SocketClient.UnSubscribe("取消订阅频道");
```

# 作者介绍



* 网址 : http://www.fayelf.com
* QQ : 7092734
* EMail : jacky@fayelf.com

