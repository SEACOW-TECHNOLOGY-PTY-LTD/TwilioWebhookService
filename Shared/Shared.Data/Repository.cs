using SqlSugar;

namespace Shared.Data;

public class Repository<T> : SimpleClient<T> where T : class, new()
{
    public Repository()
    {
        Context = SqlSugarHelper.Db;
        //Furion:       base.Context=App.GetService<ISqlSugarClient>();
        //Furion脚手架:    base.Context=DbContext.Instance
        //SqlSugar.Ioc:    base.Context=DbScoped.SugarScope; 
        //手动去赋值:     base.Context=DbHelper.GetDbInstance()     
    }
    
    public Repository(ISqlSugarClient db)
    {          
        Context=db;
    }

    /// <summary>
    /// 扩展方法，自带方法不能满足的时候可以添加新方法
    /// </summary>
    /// <returns></returns>
    public List<T>? CommQuery(string json)
    {
        return Context.Queryable<T>().ToList(); // 可以拿到SqlSugarClient 做复杂操作
    }
     
}