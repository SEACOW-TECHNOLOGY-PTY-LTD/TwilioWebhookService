using System.Text;
using Newtonsoft.Json;

namespace Shared.RabbitMQ;

public class MessageSerializer : IMessageSerializer
{
    /// <summary>
    /// 将指定的对象实例序列化为 JSON 字符串。
    /// </summary>
    /// <param name="instance">要序列化的对象实例。</param>
    /// <returns>表示对象的 JSON 字符串。</returns>
    public string Serialize(object? instance)
    {
        return JsonConvert.SerializeObject(instance);
    }

    /// <summary>
    /// 将指定的 JSON 字符串反序列化为指定类型的对象。
    /// </summary>
    /// <param name="str">要反序列化的 JSON 字符串。</param>
    /// <param name="type">目标对象的类型。</param>
    /// <returns>反序列化后的对象。</returns>
    public object? Deserialize(string str, Type type)
    {
        return JsonConvert.DeserializeObject(str, type);
    }
    
    /// <summary>
    /// 将指定类型的对象实例序列化为字节数组。
    /// </summary>
    /// <typeparam name="T">对象的类型。</typeparam>
    /// <param name="instance">要序列化的对象实例。</param>
    /// <returns>表示对象的字节数组。</returns>
    public byte[] SerializeToBytes<T>(T instance)
    {
        var jsonString = Serialize(instance);
        return Encoding.UTF8.GetBytes(jsonString);
    }

    /// <summary>
    /// 将指定的 JSON 字符串反序列化为指定类型的对象。
    /// </summary>
    /// <typeparam name="T">目标对象的类型。</typeparam>
    /// <param name="str">要反序列化的 JSON 字符串。</param>
    /// <returns>反序列化后的对象实例，或者如果 JSON 字符串不表示有效对象，则为 null。</returns>
    public T? Deserialize<T>(string str)
    {
        return (T?)Deserialize(str, typeof(T));
    }
    
    /// <summary>
    /// 将指定的字节数组反序列化为指定类型的对象。
    /// </summary>
    /// <typeparam name="T">目标对象的类型。</typeparam>
    /// <param name="bytes">要反序列化的字节数组。</param>
    /// <returns>反序列化后的对象实例，或者如果字节数组不表示有效对象，则为 null。</returns>
    public T? Deserialize<T>(byte[] bytes)
    {
        var jsonString = Encoding.UTF8.GetString(bytes);
        return (T?)Deserialize(jsonString, typeof(T));
    }
}