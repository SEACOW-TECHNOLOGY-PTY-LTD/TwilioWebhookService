namespace Shared.Helpers;

public class ObjectHelper
{
    public static T CopyPropertiesFromDictionary<T>(Dictionary<string, string> from) where T : new()
    {
        T to = new T();
        var properties = to.GetType().GetProperties();
        if (properties.Any())
        {
            foreach (var item in properties)
            {
                if (item.CanWrite && item.Name != "Id")
                {
                    if (from.TryGetValue(item.Name, out var value))
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            item.SetValue(to, Convert.ChangeType(value, item.PropertyType));
                        }
                    }
                }
            }
        }
        return to;
    }
}