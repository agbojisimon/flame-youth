using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace g_flame_youth.Fillters
{
    public class ApiResponseFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
                return;

            if (context.Result is ObjectResult objectResult)
            {
                if (objectResult.Value is ApiResponse<object>)
                    return;

                var wrappedResponse = new ApiResponse<object>
                {
                    isSuccess = true,
                    Message = "Request successful",
                    Data = objectResult.Value
                };

                context.Result = new ObjectResult(wrappedResponse)
                {
                    StatusCode = objectResult.StatusCode
                };
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {

        }
    }
}