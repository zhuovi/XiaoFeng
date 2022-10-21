# XiaoFeng

![GitHub top language](https://img.shields.io/github/languages/top/zhuovi/xiaofeng?logo=github)
![GitHub License](https://img.shields.io/github/license/zhuovi/xiaofeng?logo=github)
![Nuget Downloads](https://img.shields.io/nuget/dt/xiaofeng?logo=nuget)
![Nuget](https://img.shields.io/nuget/v/xiaofeng.redis?logo=nuget)
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/xiaofeng?label=dev%20nuget&logo=nuget)

Nuget：xiaofeng

源码： https://github.com/zhuovi/xiaofeng

教程：https://www.yuque.com/fayelf/xiaofeng

其中包含了Redis,Socket,Json,Xml,ADO.NET数据库操作兼容以下数据库（SQLSERVER,MYSQL,ORACLE,达梦,SQLITE,ACCESS,OLEDB,ODBC等数十种数据库）,正则表达式,QueryableX(ORM)和EF无缝对接,FTP,网络日志,调度,IO操作,加密算法(AES,DES,DES3,MD5,RSA,RC4,SHA等常用加密算法),超级好用的配置管理器,应用池等功能。

# XiaoFeng.Redis

Redis提供了友好的访问API。

## 基本使用方法

Redis连接串 redis://7092734@127.0.0.1:6379/0?ConnectionTimeout=3000&ReadTimeout=3000&SendTimeout=3000&pool=3

7092734	    密码

127.0.0.1	主机

6379		端口

0			0库

ConnectionTimeout	连接超时时长

ReadTimeout		读取数据超时时长

SendTimeout		发送数据超时时长

pool			连接池中连接数量

最小的连接串是：redis://127.0.0.1

实例化一个redis对象	var redis = new XiaoFeng.Redis.RedisClient("redis://7092734@127.0.0.1:6379/0");

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
