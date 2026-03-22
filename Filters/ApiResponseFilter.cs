using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GlobalFlameMinistry.API.Filters
{
    public class ApiResponseFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {

        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null) return;

            if (context.Result is ObjectResult { Value: ApiResponse<object> }) return;

            if (context.Result is ObjectResult objectResult)
            {
                if (objectResult.Value is string message)
                {
                    context.Result = new ObjectResult(new ApiResponse<object>
                    {
                        isSuccess = objectResult.StatusCode >= 200 && objectResult.StatusCode < 300,
                        Message = message,
                        Data = null
                    })
                    { StatusCode = objectResult.StatusCode };
                    return;
                }

                context.Result = new ObjectResult(new ApiResponse<object>
                {
                    isSuccess = objectResult.StatusCode >= 200 && objectResult.StatusCode < 300,
                    Message = "Request successful",
                    Data = objectResult.Value
                })
                { StatusCode = objectResult.StatusCode };
            }

            else if (context.Result is StatusCodeResult statusResult)
            {
                var isSuccess = statusResult.StatusCode >= 200 && statusResult.StatusCode < 300;
                context.Result = new ObjectResult(new ApiResponse<object>
                {
                    isSuccess = isSuccess,
                    Message = isSuccess ? "Request successful" : GetDefaultMessage(statusResult.StatusCode),
                    Data = null
                })
                { StatusCode = statusResult.StatusCode };
            }
        }

        private static string GetDefaultMessage(int statusCode) => statusCode switch
        {
            400 => "Bad request",
            401 => "Unauthorized",
            403 => "Forbidden",
            404 => "Resource not found",
            409 => "Conflict",
            500 => "An unexpected error occurred",
            _ => "Request failed"
        };
    }
}