using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotChocolateDemo.graphql.types
{
    public class PlayerInputType : InputObjectType<PlayerInputTest>
    {

        protected override void Configure(IInputObjectTypeDescriptor<PlayerInputTest> descriptor)
        {
            base.Configure(descriptor);

            descriptor.Field(p => p.FirstName)
                .Type<NonNullType<StringType>>();

            descriptor.Field(p => p.LastName)
                .Type<NonNullType<StringType>>();
        }

    }
}
