using Microsoft.AspNetCore.Mvc.Filters;

namespace g_flame_youth.Fillters
{
    public class ValidationFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value.Errors.Select(e => e.ErrorMessage)
                    );

                var response = new ApiResponse<object>
                {
                    isSuccess = false,
                    Message = "Validation failed",
                    Data = null,
                    Errors = errors
                };
            }
        }
    }
}