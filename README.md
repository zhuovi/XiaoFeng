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

其中包含了Redis,Socket,Json,Xml,ADO.NET数据库操作兼容以下数据库（SQLSERVER,MYSQL,ORACLE,达梦,SQLITE,ACCESS,OLEDB,ODBC等数十种数据库）,正则表达式,QueryableX(ORM)和EF无缝对接,FTP,网络日志,调度,IO操作,加密算法(AES,DES,DES3,MD5,RSA,RC4,SHA等常用加密算法),超级好用的配置管理器,应用池,类型转换等功能。

## XiaoFeng

XiaoFeng generator with [XiaoFeng](https://github.com/zhuovi/XiaoFeng).

## Install

.NET CLI

```
$ dotnet add package XiaoFeng
```

Package Manager

```
PM> Install-Package XiaoFeng
```

PackageReference

```
<PackageReference Include="XiaoFeng" Version="3.1.5" />
```


# XiaoFeng.Redis

Redis提供了友好的访问API。

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

# XiaoFeng.FTP

# XiaoFeng.Xml Xml序列化

# XiaoFeng.Json Json序列号

# XiaoFeng.Socket Socket操作

# 作者介绍



* 网址 : http://www.fayelf.com
* QQ : 7092734
* EMail : jacky@fayelf.com

