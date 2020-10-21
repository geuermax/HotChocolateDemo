using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotChocolateDemo.graphql.types
{
    public class MutationResolverType : ObjectType<MutationResolver>
    {

        protected override void Configure(IObjectTypeDescriptor<MutationResolver> descriptor)
        {
            base.Configure(descriptor);

            descriptor.Field(m => m.CreatePlayer(default))
                .Type<NonNullType<PlayerType>>()
                .Argument("input", i => i.Type<NonNullType<PlayerInputType>>());

            descriptor.Field(m => m.AssignPenaltyToPlayer(default, default, default))
                .Type<NonNullType<PlayerType>>()
                .Argument("playerId", a => a.Type<NonNullType<IdType>>())
                .Argument("penaltyId", a => a.Type<NonNullType<IdType>>());

        }

    }
}
