namespace CCH.Core.Shared;

/// <summary>
/// Standard API Response wrapper.
/// (繁體中文) 標準 API 回應封裝。
/// </summary>
/// <typeparam name="T">Data type. (繁體中文) 資料型別。</typeparam>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public static ApiResponse<T> SuccessResponse(T data, string message = "Success") => 
        new() { Success = true, Data = data, Message = message };

    public static ApiResponse<T> FailureResponse(string message) => 
        new() { Success = false, Message = message };
}
