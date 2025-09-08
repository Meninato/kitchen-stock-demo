namespace KitchenStock.Api.Models;

public class ApiResponse<T>
{
    public T Data { get; set; }
    public string? Message { get; set; }

    public ApiResponse(T data, string? message = null)
    {
        Data = data;
        Message = message;
    }

    public static ApiResponse<T> Create(T data, string? message = null) => new(data, message);
}
