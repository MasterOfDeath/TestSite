namespace TestSite.DAL.Contract
{
    using System.Collections.Generic;

    public interface IRoleStore
    {
        ICollection<string> ListRolesForUserByUserId(int userId);

        ICollection<string> ListAllRoles();

        bool GiveRole(int userId, string roleName);

        bool PullOffRole(int userId, string roleName);
    }
}
