namespace Shared.RabbitMQ;

public interface IMessageSerializer
{
    /// <summary>
    /// 序列化
    /// </summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    string Serialize(object? instance);
    
    /// <summary>
    /// 将指定类型的对象实例序列化为字节数组。
    /// </summary>
    /// <typeparam name="T">对象的类型。</typeparam>
    /// <param name="instance">要序列化的对象实例。</param>
    /// <returns>表示对象的字节数组。</returns>
    byte[] SerializeToBytes<T>(T instance);

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <param name="str"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    object? Deserialize(string str, Type type);
    
    /// <summary>
    /// 将指定的 JSON 字符串反序列化为指定类型的对象。
    /// </summary>
    /// <typeparam name="T">目标对象的类型。</typeparam>
    /// <param name="str">要反序列化的 JSON 字符串。</param>
    /// <returns>反序列化后的对象实例。</returns>
    T? Deserialize<T>(string str);
    
    /// <summary>
    /// 将指定的字节数组反序列化为指定类型的对象。
    /// </summary>
    /// <typeparam name="T">目标对象的类型。</typeparam>
    /// <param name="bytes">要反序列化的字节数组。</param>
    /// <returns>反序列化后的对象实例。</returns>
    T? Deserialize<T>(byte[] bytes);
}