namespace TestSite.DAL.Contract
{
    using System.Collections.Generic;
    using TestSite.Entites;

    public interface IDepStore
    {
        bool InsertDep(Dep dep);

        bool RemoveDep(int depId);

        ICollection<Dep> ListAllDeps();
    }
}
