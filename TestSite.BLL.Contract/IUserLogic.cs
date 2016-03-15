namespace TestSite.BLL.Contract
{
    using System.Collections.Generic;
    using Entites;

    public interface IUserLogic
    {
        bool AddUser(User user, string password);

        bool InsertUser(User user);

        bool RemoveUser(int userId);

        ICollection<User> ListAllUsers();

        User GetUserById(int userId);

        User GetUserByUserName(string userName);

        bool CanLogin(User user, string password);

        bool ChangePassword(int userId, string oldPassword, string newPassword);

        bool SetUserState(int userId, bool enabled);
    }
}
