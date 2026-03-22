using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GlobalFlameMinistry.API.Fillters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var response = new ApiResponse<string>
            {
                isSuccess = false,
                Message = "An unexpected error occurred.",
                Data = null,
                Errors = context.Exception.Message // Log full error internally in real apps
            };

            // Return 500 Internal Server Error
            context.Result = new ObjectResult(response)
            {
                StatusCode = 500
            };

            context.ExceptionHandled = true;
        }
    }
}