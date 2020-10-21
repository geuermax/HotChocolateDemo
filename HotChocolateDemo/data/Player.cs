using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQLDemo.data
{
    public class Player
    {

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ICollection<int> Penalties { get; private set; }


        public Player(int id, string firstName, string lastName)
        {
            this.Id = id;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Penalties = new List<int>();
        }

        public void AddPenalty(int penaltyId)
        {
            this.Penalties.Add(penaltyId);
        }
    }
}
