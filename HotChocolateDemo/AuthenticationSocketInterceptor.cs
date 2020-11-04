using HotChocolate.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HotChocolateDemo
{
    // Dient die initiale Nachricht abzufangen in dem das Token enthalten ist
    public class AuthenticationSocketInterceptor : ISocketConnectionInterceptor<HttpContext>
    {

        // This is the key to the auth token in the HTTP Context
        public static readonly string HTTP_CONTEXT_WEBSOCKET_AUTH_KEY = "websocket-auth-token";
        // This is the key that apollo uses in the connection init request
        public static readonly string WEBOCKET_PAYLOAD_AUTH_KEY = "Authorization"; // <-- So heißt der Bums auch in Apollo

        private readonly IAuthenticationSchemeProvider _schemes;
        
        public AuthenticationSocketInterceptor(IAuthenticationSchemeProvider schemes)
        {
            _schemes = schemes;
            
        }


        /// <summary>
        /// Intercepts the websocket connection, extracts the JWT from the onOpenMessage and authenticate the user with it. The connection will be rejected if the no JWT is given.
        /// </summary>        
        public async Task<ConnectionStatus> OnOpenAsync(
            HttpContext context,
            IReadOnlyDictionary<string, object> properties,
            CancellationToken cancellationToken            
            )
        {
            if (properties.TryGetValue(WEBOCKET_PAYLOAD_AUTH_KEY, out object token) &&
                token is string stringToken)
            {
                // Das Token dem HTTP Context hinzufügen, sodass dies später für über den TokenRetriever verwendet werden aknn
                context.Items[HTTP_CONTEXT_WEBSOCKET_AUTH_KEY] = stringToken;

                context.Features.Set<IAuthenticationFeature>(new AuthenticationFeature
                {
                    OriginalPath = context.Request.Path,
                    OriginalPathBase = context.Request.PathBase
                });

                // Give any IAuthenticationRequestHandler schemes a chance to handle the request
                var handlers = context.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
                foreach (var scheme in await _schemes.GetRequestHandlerSchemesAsync())
                {
                    var handler = handlers.GetHandlerAsync(context, scheme.Name) as IAuthenticationRequestHandler;
                    if (handler != null && await handler.HandleRequestAsync())
                    {
                        return ConnectionStatus.Reject();
                    }
                } 

                var defaultAuthenticate = await _schemes.GetDefaultAuthenticateSchemeAsync();
                if (defaultAuthenticate != null)
                {
                    // Benutzer mithilfe des Tokens authentifizieren
                    var result = await context.AuthenticateAsync(defaultAuthenticate.Name);
                    // war die Authentifizierung erfolgreich wird die Anfrage angenommen und der Websockt kann Nachrichten erhalten
                    if (result?.Principal != null)
                    {
                        context.User = result.Principal;
                        return ConnectionStatus.Accept();
                    }
                }
            }
            // Sollte kein Token vorhanden sein, wird die Anfrage abgelehnt
            return ConnectionStatus.Reject();
        }
    }
}
