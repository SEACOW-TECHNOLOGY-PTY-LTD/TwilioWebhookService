using System.Text;

namespace Shared.Helpers;

public static class Base64Helper
{
    public static string Base64Encode(string plainText) 
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        return System.Convert.ToBase64String(plainTextBytes);
    }
    
    public static string Base64Decode(string base64EncodedData) 
    {
        var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }
}