using GraphQLDemo.data;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HotChocolateDemo.graphql
{
    
    public class SubscriptionResolver
    {

        [Authorize]
        [SubscribeAndResolve]
        public async ValueTask<IAsyncEnumerable<Player>> OnPenaltyAssigned(
            [Service]ITopicEventReceiver eventReceiver,
            CancellationToken token
            )
        {     
            return await eventReceiver.SubscribeAsync<string, Player>("PenaltyAssigned", token);
        }


    }
}
