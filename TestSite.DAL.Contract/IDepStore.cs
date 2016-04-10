namespace TestSite.DAL.Contract
{
    using System.Collections.Generic;
    using TestSite.Entites;

    public interface IDepStore
    {
        Dep GetDepById(int depId);

        bool InsertDep(Dep dep);

        bool RemoveDep(int depId);

        ICollection<Dep> ListAllDeps();
    }
}
