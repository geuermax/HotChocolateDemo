using GraphQLDemo.data;
using HotChocolate.AspNetCore.Authorization;
using System.Collections.Generic;

namespace HotChocolateDemo.graphql
{
    [Authorize(Policy = "Administrator")]
    public class QueryResolver
    {

        public IReadOnlyList<Player> GetPlayers()
        {
            return DemoData.GetPlayers();
        }
        
        public Player GetPlayer(int id)
        {
            foreach (var player in DemoData.GetPlayers())
            {
                if (player.Id == id)
                {
                    return player;
                }
            }
            return null;
        }

        public IReadOnlyList<Penalty> GetPenalties()
        {
            return DemoData.GetPenalties();
        }


        public Penalty GetPenalty(int id)
        {
            foreach (var penalty in DemoData.GetPenalties())
            {
                if (penalty.Id == id)
                {
                    return penalty;
                }
            }
            return null;
        }

    }
}
