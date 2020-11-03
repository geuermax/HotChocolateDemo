using HotChocolate.Types;

namespace HotChocolateDemo.graphql.types
{
    public class QueryResolverType : ObjectType<QueryResolver>
    {

        protected override void Configure(IObjectTypeDescriptor<QueryResolver> descriptor)
        {
            base.Configure(descriptor);

            descriptor.Field(q => q.GetPlayers())
                .Type<NonNullType<ListType<NonNullType<PlayerType>>>>();

            descriptor.Field(q => q.GetPlayer(default))
                .Argument("id", a => a.Type<NonNullType<IdType>>());

            descriptor.Field(q => q.GetPenalties())
                .Type<NonNullType<ListType<NonNullType<PenaltyType>>>>();

            descriptor.Field(q => q.GetPenalty(default))
                .Argument("id", a => a.Type<NonNullType<IdType>>());
        }

    }
}
