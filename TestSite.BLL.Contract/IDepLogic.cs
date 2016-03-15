namespace TestSite.BLL.Contract
{
    using System.Collections.Generic;
    using Entites;

    public interface IDepLogic
    {
        bool InsertDep(Dep dep);

        bool RemoveDep(int adminId);

        ICollection<Dep> ListAllDeps();
    }
}
