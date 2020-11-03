using System;
using System.Threading.Tasks;
using HotChocolate.AspNetCore.Subscriptions;
using HotChocolate.Execution;
using Microsoft.AspNetCore.Http;

namespace HotChocolateDemo.hcWebsockets
{
    public class MySubscriptionMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly IMessagePipeline _messagePipeline;
        private readonly SubscriptionMiddlewareOptions _options;

        private MyWebSocketManager socketManager;

        public MySubscriptionMiddleware(
            RequestDelegate next,
            IMessagePipeline messagePipeline,
            SubscriptionMiddlewareOptions options,
            MyWebSocketManager socketManager)
        {
            _next = next
                ?? throw new ArgumentNullException(nameof(next));
            _messagePipeline = messagePipeline
                ?? throw new ArgumentNullException(nameof(messagePipeline));
            _options = options
                ?? throw new ArgumentNullException(nameof(options));
            
            this.socketManager = socketManager;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            
            if (context.WebSockets.IsWebSocketRequest)
            {
                var wsSession = WebSocketSession.New(context, _messagePipeline);              
                socketManager.AddSocketSession(wsSession);
                await wsSession.HandleAsync(context.RequestAborted).ConfigureAwait(false);

            }
            else
            {
                await _next(context).ConfigureAwait(false);
            }
        }

        public static bool IsValidPath(HttpContext context, PathString path)
        {
            return context.Request.Path.Equals(path,
                StringComparison.OrdinalIgnoreCase);
        }

    }
}
