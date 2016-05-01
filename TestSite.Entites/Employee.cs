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
            bool enabled,
            int role_id)
        {
            this.Id = id;
            this.Dep_Id = dep_id;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Hash = hash;
            this.Enabled = enabled;
            this.Role_Id = role_id;
        }

        public Employee(int dep_id, string firstName, string lastName, byte[] hash, bool enabled, int role_id)
            : this(0, dep_id, firstName, lastName, hash, enabled, role_id)
        {
        }

        public int Id { get; set; }

        public int Dep_Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public byte[] Hash { get; set; }

        public bool Enabled { get; set; }

        public int Role_Id { get; set; }
    }
}
