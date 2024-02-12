using System.Net;

namespace Shared.Models.Common;

public class Response
{
    public string? Message { get; set; }
    public int Status { get; set; }
}

public class Response<T>
{
    public T? Data { get; set; }
    public int? Status { get; set; }
    public string? Message { get; set; }
}