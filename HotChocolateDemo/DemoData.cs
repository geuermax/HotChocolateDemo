using GraphQLDemo.data;
using System.Collections.Generic;
using System.Linq;

namespace HotChocolateDemo
{
    public static class DemoData
    {
        private static List<Player> players = new List<Player>
            {
                new Player(1, "Max", "Geuer") {
                    Penalties = { 1, 2}
                },
                new Player(2, "Dirk", "Nowitski")
                {
                    Penalties = { 2 }
                }
            };


        public static List<Player> GetPlayers()
        {
            return players;
        }


        public static List<Penalty> GetPenalties()
        {
            return new List<Penalty>
            {
                new Penalty(1, "Red card", 50),
                new Penalty(2, "Stupidity", 60)
            };
        }


        public static Penalty GetPenaltyById(int id)
        {
            foreach (var penalty in GetPenalties())
            {
                if (penalty.Id == id)
                {
                    return penalty;
                }
            }
            return null;
        }


        public static int GetNewPlayerId()
        {
            return players.Last().Id + 1;
        }

        public static void AddPlayer(Player player)
        {
            players.Add(player);
        }

        public static Player AssignPenalty(int penaltyId, int playerId)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].Id == playerId)
                {
                    players[i].AddPenalty(penaltyId);
                    return players[i];
                }
            }
            return null;
        }
    }
}
