﻿using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Nop.Web.Framework.Extensions
{
    /// <summary>
    /// HttpRequest extensions
    /// </summary>
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Check if the request is the POST request
        /// </summary>
        /// <param name="request">Request to check</param>
        /// <returns>True if the request is POST request, false in all other cases</returns>
        public static bool IsPostRequest(this HttpRequest request)
        {
            return request.Method.Equals(WebRequestMethods.Http.Post, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Gets the form value
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="formKey">The form key</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the form value
        /// </returns>
        public static async Task<StringValues> GetFormValueAsync(this HttpRequest request, string formKey)
        {
            if (!IsPostRequest(request))
                return new StringValues();

            var form = await request.ReadFormAsync();

            return form[formKey];
        }

        /// <summary>
        /// Perform foreach loop on the form values
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="acquire">Action to apply to each element</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static async Task FormForeachAsync(this HttpRequest request, Action<string, StringValues> acquire)
        {
            await FormForeachAsync(request, null, acquire);
        }

        /// <summary>
        /// Perform foreach loop on the form values
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="predicate">Filter to foreach</param>
        /// <param name="acquire">Action to apply to each element</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static async Task FormForeachAsync(this HttpRequest request, Func<string, bool> predicate, Action<string, StringValues> acquire)
        {
            if (!IsPostRequest(request))
                return;

            var form = await request.ReadFormAsync();

            foreach (var item in predicate == null ? form : form.Where(item => predicate(item.Key)))
                acquire(item.Key, item.Value);
        }

        /// <summary>
        /// Checks if the provided key is exists on the form
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="formKey">Form key</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains True if the key is persists in the form, false in other case
        /// </returns>
        public static async Task<bool> IsFormKeyExistsAsync(this HttpRequest request, string formKey)
        {
            return await IsFormAnyAsync(request, key => key.Equals(formKey));
        }

        /// <summary>
        /// Checks if the items are exists on the form
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="predicate">Filter. Set null if filtering no need</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains True if the any item is persists in the form, false in other case
        /// </returns>
        public static async Task<bool> IsFormAnyAsync(this HttpRequest request, Func<string, bool> predicate = null)
        {
            if (!IsPostRequest(request))
                return false;

            var form = await request.ReadFormAsync();

            return  predicate == null ? form.Any() : form.Keys.Any(predicate);
        }

        /// <summary>
        /// Gets the value associated with the specified form key
        /// </summary>
        /// <param name="request"></param>
        /// <param name="formKey"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains True and the form value if the form contains an element with the specified key; otherwise, false and default value.
        /// </returns>
        public static async Task<(bool keyExists, StringValues formValue)> TryGetFormValueAsync(this HttpRequest request, string formKey)
        {
            if (!IsPostRequest(request))
                return (false, default);

            var form = await request.ReadFormAsync();

            var flag = form.TryGetValue(formKey, out var formValue);

            return (flag, formValue);
        }

        /// <summary>
        /// Returns the first element of the Form.Files, or a default value if the sequence contains no elements
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the <see cref="IFormFile"/> element or default value
        /// </returns>
        public static async Task<IFormFile> GetFirstOrDefaultFileAsync(this HttpRequest request)
        {
            if (!IsPostRequest(request))
                return default;

            var form = await request.ReadFormAsync();

            return form.Files.FirstOrDefault();
        }
    }
}
