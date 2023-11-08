using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.Mvc.ModelBinding.Binders
{
    /// <summary>
    /// Represents model binder for CustomProperties
    /// </summary>
    public class CustomPropertiesModelBinder : IModelBinder
    {
        async Task IModelBinder.BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var modelName = bindingContext.ModelName;

            var result = new Dictionary<string, string>();
            if (bindingContext.HttpContext.Request.Method == "POST")
                await bindingContext.HttpContext.Request.FormForeachAsync(
                    x => x.IndexOf(modelName, StringComparison.Ordinal) == 0,
                    (key, value) =>
                    {
                        var dicKey = key.Replace(modelName + "[", "").Replace("]", "");
                        result.Add(dicKey, value.ToString());
                    });

            if (bindingContext.HttpContext.Request.Method == "GET")
            {
                var queryStringValue = bindingContext.HttpContext.Request.QueryString.Value;
                if (!string.IsNullOrEmpty(queryStringValue))
                {
                    var keys = queryStringValue.TrimStart('?').Split('&').Where(x => x.StartsWith(modelName)).ToList();

                    foreach (var key in keys)
                    {
                        var dicKey = key[(key.IndexOf("[", StringComparison.Ordinal) + 1)..key.IndexOf("]", StringComparison.Ordinal)];
                        var value = key[(key.IndexOf("=", StringComparison.Ordinal) + 1)..];

                        result.Add(dicKey, value);
                    }
                }
            }

            bindingContext.Result = ModelBindingResult.Success(result);
        }
    }
}
