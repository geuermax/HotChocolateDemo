
namespace GraphQLDemo.data
{
    public class Penalty
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }


        public Penalty(int id, string name, int cost)
        {
            this.Id = id;
            this.Name = name;
            this.Cost = cost;
        }
    }
}
