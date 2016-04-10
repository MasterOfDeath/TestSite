namespace TestSite.Entites
{
    public class Test
    {
        public Test(int id, string name, int depId)
        {
            this.Id = id;
            this.Name = name;
            this.DepId = depId;
        }

        public Test(string name, int depId)
            : this(-1, name, depId)
        {
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public int DepId { get; set; }
    }
}
