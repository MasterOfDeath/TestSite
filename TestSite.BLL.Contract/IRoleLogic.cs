namespace TestSite.BLL.Contract
{
    using System.Collections.Generic;

    public interface IRoleLogic
    {
        ICollection<string> ListRolesForUserByUserId(int userId);

        ICollection<string> ListAllRoles();

        bool GiveRole(int userId, string roleName);

        bool PullOffRole(int userId, string roleName);
    }
}
