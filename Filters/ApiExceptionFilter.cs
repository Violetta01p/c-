
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace C_.Filters;

public class ApiExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        // 1. Некоректний параметр (наприклад, ID < 0)
        if (context.Exception is ArgumentException argEx)
        {
            context.Result = new BadRequestObjectResult(new 
            { 
                Error = "Некоректний параметр запиту", 
                Message = argEx.Message 
            });
            context.ExceptionHandled = true; // Кажемо системі, що ми обробили помилку
        }
        // 2. Порушення бізнес-правила
        else if (context.Exception is InvalidOperationException invEx)
        {
            context.Result = new BadRequestObjectResult(new 
            { 
                Error = "Порушення бізнес-правила", 
                Message = invEx.Message 
            });
            context.ExceptionHandled = true;
        }
    }
}
