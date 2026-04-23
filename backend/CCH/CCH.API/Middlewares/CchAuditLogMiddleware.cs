using System.Diagnostics;
using System.Text;
using CCH.Core.Entities;
using CCH.Core.Entities.CSP;
using CCH.Core.Entities.ReSm;
using CCH.Core.Interfaces;
using CCH.Services.Repositories.Data;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace CCH.API.Middlewares;

/// <summary>
/// Middleware to log Request and Response data to CCHLog table.
/// (繁體中文) 將 Request 與 Response 資料記錄至 CCHLog 資料表的中介軟體。
/// </summary>
public class CchAuditLogMiddleware
{
    private readonly RequestDelegate _next;

    public CchAuditLogMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, CspDbContext dbContext, IUserContext userContext)
    {
        var endpoint = context.GetEndpoint();
        var controllerActionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();

        // Only log if it's a controller request (僅在為 Controller 請求時記錄)
        if (controllerActionDescriptor == null)
        {
            await _next(context);
            return;
        }

        var sw = Stopwatch.StartNew();
        var requestBody = string.Empty;

        // 1. Enable buffering and read Request Body (啟用緩衝並讀取 Request Body)
        context.Request.EnableBuffering();
        using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true))
        {
            requestBody = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
        }

        // 2. Intercept Response Body (攔截 Response Body)
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);

            sw.Stop();

            // 3. Read Response Body (讀取 Response Body)
            responseBody.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(responseBody).ReadToEndAsync();
            responseBody.Seek(0, SeekOrigin.Begin);

            // 4. Log to Database (紀錄至資料庫)
            var log = new CchLog
            {
                Request = requestBody,
                Response = responseText,
                HttpMethod = context.Request.Method,
                EndpointUrl = $"{context.Request.Path}{context.Request.QueryString}",
                FunctionName = $"{controllerActionDescriptor.ControllerName}/{controllerActionDescriptor.ActionName}",
                StatusCode = context.Response.StatusCode,
                ExecutionTime = (int)sw.ElapsedMilliseconds,
                CreatedBy = userContext.UserId ?? "System",
                CreatedDate = DateTime.Now
            };

            dbContext.CchLog.Add(log);
            await dbContext.SaveChangesAsync();

            // 5. Copy captured response back to original stream (將擷取的回應複製回原始串流)
            await responseBody.CopyToAsync(originalBodyStream);
        }
        catch (Exception ex)
        {
            // Fail-safe: ensure logging failure doesn't break the API (確保日誌失敗不影響 API)
            Console.WriteLine($"Logging Error: {ex.Message}");
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }
}
