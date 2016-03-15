namespace TestSite.DAL.Contract
{
    using System.Collections.Generic;
    using Entites;

    public interface IUserStore
    {
        bool AddUser(User user);

        bool InsertUser(User user);

        bool RemoveUser(int userId);

        ICollection<User> ListAllUsers();

        ICollection<User> ListUsersByRoleName(string roleName);

        User GetUserById(int userId);

        User GetUserByUserName(string userName);
    }
}
