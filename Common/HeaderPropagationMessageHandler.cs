﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Common
{
    public class HeaderPropagationMessageHandler : DelegatingHandler
    {
        private readonly HeaderPropagationOptions _options;
        private readonly IHttpContextAccessor _contextAccessor;

        public HeaderPropagationMessageHandler(HeaderPropagationOptions options, IHttpContextAccessor contextAccessor)
        {
            _options = options;
            _contextAccessor = contextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            if (_contextAccessor.HttpContext != null)
            {
                foreach (var headerName in _options.HeaderNames)
                {
                    // Get the incoming header value
                    var headerValue = _contextAccessor.HttpContext.Request.Headers[headerName];
                    if (StringValues.IsNullOrEmpty(headerValue))
                    {
                        continue;
                    }

                    request.Headers.TryAddWithoutValidation(headerName, (string[])headerValue);
                }
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}