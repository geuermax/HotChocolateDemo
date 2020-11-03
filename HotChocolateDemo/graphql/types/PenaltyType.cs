using GraphQLDemo.data;
using HotChocolate.Types;

namespace HotChocolateDemo.graphql.types
{
    public class PenaltyType : ObjectType<Penalty>
    {

        protected override void Configure(IObjectTypeDescriptor<Penalty> descriptor)
        {
            base.Configure(descriptor);

            descriptor.Field(p => p.Id)
                .Type<NonNullType<IdType>>();

            descriptor.Field(p => p.Name)
                .Type<NonNullType<StringType>>();

            descriptor.Field(p => p.Description)
                .Type<StringType>();

            descriptor.Field(p => p.Cost)
                .Type<NonNullType<IntType>>();
        }

    }
}
