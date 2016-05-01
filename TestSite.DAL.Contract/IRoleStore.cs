namespace TestSite.DAL.Contract
{
    using System.Collections.Generic;

    public interface IRoleStore
    {
        ICollection<string> ListAllRoles();

        //ICollection<string> ListRolesForUserByUserId(int userId);

        //bool GiveRole(int userId, string roleName);

        //bool PullOffRole(int userId, string roleName);
    }
}
