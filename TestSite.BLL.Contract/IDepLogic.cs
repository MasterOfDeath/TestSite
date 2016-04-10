namespace TestSite.BLL.Contract
{
    using System.Collections.Generic;
    using Entites;

    public interface IDepLogic
    {
        Dep GetDepById(int depId);

        bool InsertDep(Dep dep);

        bool RemoveDep(int adminId);

        ICollection<Dep> ListAllDeps();
    }
}
