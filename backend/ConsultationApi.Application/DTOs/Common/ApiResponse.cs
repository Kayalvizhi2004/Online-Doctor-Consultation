namespace ConsultationApi.Application.DTOs.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }

    public int StatusCode { get; set; } = 200;

    public string Message { get; set; }
        = string.Empty;

    public T? Data { get; set; }

    public List<string> Errors { get; set; }
        = new();

    public static ApiResponse<T> Failure(string message, int statusCode = 500)
        => new ApiResponse<T>
        {
            Success = false,
            Message = message,
            StatusCode = statusCode
        };

    public static ApiResponse<T> SuccessResponse(T? data, string message = "", int statusCode = 200)
        => new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            StatusCode = statusCode
        };
}