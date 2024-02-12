using SqlSugar;
using SugarRedis;

namespace Shared.Data;

public class SqlSugarRedisCache : ICacheService
{
    //NUGET安装 SugarRedis  （也可以自个实现）   
    //注意:SugarRedis 不要扔到构造函数里面， 一定要单例模式  
    private static readonly SugarRedisClient Service = new("127.0.0.1:6379,password=,connectTimeout=3000,connectRetry=1,syncTimeout=10000,DefaultDatabase=0");
       
       
    //+1重载 new SugarRedisClient(字符串)
    //默认:127.0.0.1:6379,password=,connectTimeout=3000,connectRetry=1,syncTimeout=10000,DefaultDatabase=0
    public void Add<TV>(string key, TV value)
    {
        if (value != null) Service.Set(key, value);
    }
 
    public void Add<TV>(string key, TV value, int cacheDurationInSeconds)
    {
        //SugarRedis单位是分钟
        //其他Redis可以不需要转换
        cacheDurationInSeconds=Convert.ToInt32(cacheDurationInSeconds/60);
        if (value != null) Service.Set(key, value, cacheDurationInSeconds);
    }
 
    public bool ContainsKey<TV>(string key)
    {
        return Service.Exists(key);
    }
 
    public TV Get<TV>(string key)
    {
        return Service.Get<TV>(key);
    }
 
    public IEnumerable<string> GetAllKey<TV>()
    {
 
        return Service.SearchCacheRegex("SqlSugarDataCache.*");
    }
 
    public TV GetOrCreate<TV>(string cacheKey, Func<TV> create, int cacheDurationInSeconds = int.MaxValue)
    {
        //SugarRedis单位是分钟
        //其他Redis可以不需要转换
        cacheDurationInSeconds=Convert.ToInt32(cacheDurationInSeconds/60);
        if (this.ContainsKey<TV>(cacheKey))
        {
            var result=this.Get<TV>(cacheKey);
            if(result==null)
            {
                return create();
            }
            else
            {
                return result;
            }
        }
        else
        {
            var result = create();
            this.Add(cacheKey, result, cacheDurationInSeconds);
            return result;
        }
    }
 
    public void Remove<TV>(string key)
    {
        Service.Remove(key);
    }
}