namespace TestSite.Entites
{
    public class Dep
    {
        public Dep(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public Dep(string name)
            : this(-1, name)
        {
        }

        public int Id { get; set; }

        public string Name { get; set; }
    }
}
