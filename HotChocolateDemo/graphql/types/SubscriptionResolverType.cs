using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotChocolateDemo.graphql.types
{
    public class SubscriptionResolverType : ObjectType<SubscriptionResolver>
    {
        protected override void Configure(IObjectTypeDescriptor<SubscriptionResolver> descriptor)
        {
            base.Configure(descriptor);

            descriptor.Field(s => s.OnPenaltyAssigned(default, default))
                .Type<NonNullType<PlayerType>>();
        }
    }
}
