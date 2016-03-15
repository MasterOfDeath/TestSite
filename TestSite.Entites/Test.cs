namespace TestSite.Entites
{
    public class Test
    {
        public Test(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public Test(string name)
            : this(-1, name)
        {
        }

        public int Id { get; set; }

        public string Name { get; set; }
    }
}
