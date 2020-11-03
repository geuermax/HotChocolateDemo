using HotChocolate.Types;

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
