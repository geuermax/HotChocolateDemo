using GraphQLDemo.data;
using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace HotChocolateDemo.graphql.types
{

    // Ein Type definiert Welche Felder eines Objektes über GraphQL preisgegeben werden und welchen GrpahQL-Datentypen diese haben. Zudem dient er um Relationen aufzulösen (siehe ResolvePenalties)
    // Beschreibungen und ähnliches können hier definiert werden
    public class PlayerType : ObjectType<Player>
    {
        protected override void Configure(IObjectTypeDescriptor<Player> descriptor)
        {
            base.Configure(descriptor);

            descriptor.Field(p => p.Id)
                .Type<NonNullType<IdType>>()
                .Description("An unique id for the player");

            descriptor.Field(p => p.FirstName)
                .Type<NonNullType<StringType>>()
                .Description("The first name of the player");

            descriptor.Field(p => p.LastName)
                .Type<NonNullType<StringType>>()
                .Description("The last name of the player");

            descriptor.Field(p => p.Penalties)
                .Ignore();


            descriptor.Field(p => ResolvePenalties(p))
                .Name("penalties")
                .Type<NonNullType<ListType<NonNullType<PenaltyType>>>>();

            
        }
        private List<Penalty> ResolvePenalties(Player p)
        {
            List<Penalty> output = new List<Penalty>();
            foreach (int id in p.Penalties)
            {
                output.Add(DemoData.GetPenaltyById(id));
            }

            return output;
        }
    }
}
