using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;

namespace ManagedApplicationScheduler.Services.Utilities;
public class ExceptionHandlerAttribute : ExceptionFilterAttribute
{
    private readonly ILogger<ExceptionHandlerAttribute> _logger;
    private readonly IModelMetadataProvider _modelMetadataProvider;
    public ExceptionHandlerAttribute(IModelMetadataProvider modelMetadataProvider, ILogger<ExceptionHandlerAttribute> logger)
    {
        _modelMetadataProvider = modelMetadataProvider;
        _logger = logger;
    }

    public override void OnException(ExceptionContext context)
    {
        base.OnException(context);
        _logger.LogError(context.Exception, $"Exception: {context.Exception.Message} - {context.Exception.InnerException?.Message ?? ""}");


        var result = new ViewResult
        {
            ViewName = "Error",
            ViewData = new ViewDataDictionary(_modelMetadataProvider, context.ModelState)
            {
                Model = context.Exception
            }
        };

        context.ExceptionHandled = true; // mark exception as handled
        context.Result = result;
    }
}
