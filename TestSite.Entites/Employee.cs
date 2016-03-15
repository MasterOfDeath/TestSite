namespace TestSite.Entites
{
    public class Employee
    {
        public Employee(
            int id, 
            int dep_id,
            string firstName, 
            string lastName,
            byte[] hash,
            bool enabled)
        {
            this.Id = id;
            this.Dep_Id = dep_id;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Hash = hash;
            this.Enabled = enabled;
        }

        public Employee(int dep_id, string firstName, string lastName, byte[] hash, bool enabled)
            : this(0, dep_id, firstName, lastName, hash, enabled)
        {
        }

        public int Id { get; set; }

        public int Dep_Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public byte[] Hash { get; set; }

        public bool Enabled { get; set; }
    }
}
