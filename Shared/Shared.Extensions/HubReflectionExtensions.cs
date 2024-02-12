using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using System.Reflection;
using Microsoft.AspNetCore.Routing;

namespace Shared.Extensions;

public static class HubReflectionExtensions
{
    public static IEnumerable<Type> GetAllHubs(string namespacePrefix)
    {
        var hubType = typeof(Hub);
        return Assembly.GetExecutingAssembly().GetTypes()
            .Where(type => type.Namespace != null &&
                           hubType.IsAssignableFrom(type) &&
                           type.Namespace.StartsWith(namespacePrefix))
            .ToList();
    }
}