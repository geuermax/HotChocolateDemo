using GraphQLDemo.data;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using System.Collections.Generic;
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
