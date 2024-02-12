using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Shared.Enums.RabbitMQ;
using SqlSugar;

namespace Shared.Helpers;

public class DatabaseHelper
{
    private readonly ISqlSugarClient _db;
    private readonly ILogger _logger;

    public DatabaseHelper(ISqlSugarClient db, ILogger logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<MessageReceiveResult> InsertObjectToDatabaseAsync<T>(T objectToInsert, CancellationToken cancellationToken) where T : class, new()
    {
        // 获取主键属性
        var primaryKeyPropertyName = _db.EntityMaintenance.GetEntityInfo<T>().Columns.Single(c => c.IsPrimarykey).PropertyName;

        if (!string.IsNullOrEmpty(primaryKeyPropertyName))
        {
            // 获取主键的值
            var primaryKeyProperty = typeof(T).GetProperty(primaryKeyPropertyName);
            var primaryKeyValue = primaryKeyProperty?.GetValue(objectToInsert);

            // 使用主键属性构建 Lambda 表达式
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, primaryKeyPropertyName);
            var constant = Expression.Constant(primaryKeyValue);
            var equalsExpression = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<T, bool>>(equalsExpression, parameter);

            // 检查数据是否已存在
            var existingObject = await _db.Queryable<T>().SingleAsync(lambda);
            if (existingObject != null)
            {
                _logger.LogWarning("Object with primary key {PrimaryKeyValue} of type {ObjectType} already exists; skipping insertion", primaryKeyValue, typeof(T).Name);
                return MessageReceiveResult.AlreadyExists; // 返回处理结果，已存在
            }
        }
        else
        {
            _logger.LogWarning("No primary key defined for type {ObjectType}. Unable to check for existing records", typeof(T).Name);
        }

        try
        {
            await _db.Insertable(objectToInsert).ExecuteCommandAsync(cancellationToken);
            return MessageReceiveResult.Success; // 返回处理结果，成功
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while processing object of type {ObjectType}", typeof(T).Name);
            return MessageReceiveResult.Failed; // 返回处理结果，失败
        }
    }
}
