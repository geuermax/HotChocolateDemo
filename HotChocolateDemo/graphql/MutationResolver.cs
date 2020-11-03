using GraphQLDemo.data;
using HotChocolate;
using HotChocolate.Subscriptions;
using HotChocolateDemo.graphql.types;
using System;
using System.Threading.Tasks;

namespace HotChocolateDemo.graphql
{
    public class MutationResolver
    {


        public Player CreatePlayer(PlayerInputTest input)
        {
            int playerId = DemoData.GetNewPlayerId(); 

            Player player = new Player(playerId, input.FirstName, input.LastName);

            DemoData.AddPlayer(player);
            return player;
        }


        public async Task<Player> AssignPenaltyToPlayer(int playerId, int penaltyId, [Service] ITopicEventSender eventSender)
        {
            Player p = DemoData.AssignPenalty(penaltyId, playerId);
            if (p == null)
            {
                throw new Exception("PlayerNotFound");
            }
            await eventSender.SendAsync("PenaltyAssigned", p);
            return p;
        }
    }
}
