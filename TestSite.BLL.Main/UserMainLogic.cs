namespace TestSite.BLL.Main
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using Contract;
    using Entites;

    public class UserMainLogic : IUserLogic
    {
        private const int UsernameMinLength = 6;
        private const int PasswordMinLength = 6;
        private const int DefaultTariffId = 1;
        private const string DefaultRole = "users";
        private const string AdminsRole = "admins";

        public bool AddUser(User user, string password)
        {
            var result = false;

            try
            {
                if (string.IsNullOrWhiteSpace(password))
                {
                    throw new ArgumentException("Password mustn't be empty");
                }

                user.Tariff_Id = DefaultTariffId;
                user.Hash = this.GetHash(password);

                this.IsValidUser(user);

                result = Stores.UserStore.AddUser(user);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (result)
            {
                try
                {
                    result = Stores.RoleStore.GiveRole(user.Id, DefaultRole);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return result;
        }

        public bool CanLogin(User user, string password)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException($"{nameof(user)} and {nameof(password)} mustn't be null or contain only spaces");
            }

            var userName = user.UserName;
            User returnUser = null;

            try
            {
                returnUser = Stores.UserStore.GetUserByUserName(userName);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (returnUser == null)
            {
                throw new InvalidOperationException($"The user {userName} not found");
            }

            if (!returnUser.Enabled)
            {
                throw new InvalidOperationException($"The user {userName} was disabled by admin");
            }

            user.Id = returnUser.Id;

            return returnUser.Hash.SequenceEqual(this.GetHash(password));
        }

        public User GetUserById(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException($"{nameof(userId)} mustn't be negative");
            }

            User result = null;

            try
            {
                result = Stores.UserStore.GetUserById(userId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public User GetUserByUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException($"{nameof(userName)} mustn't be null or contains only spaces");
            }

            User result = null;

            try
            {
                result = Stores.UserStore.GetUserByUserName(userName);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public bool InsertUser(User user)
        {
            var result = false;

            try
            {
                this.IsValidUser(user);
                result = Stores.UserStore.InsertUser(user);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public ICollection<User> ListAllUsers()
        {
            ICollection<User> result = null;

            try
            {
                result = Stores.UserStore.ListAllUsers();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public bool RemoveUser(int userId)
        {
            throw new NotImplementedException();
        }

        public bool ChangePassword(int userId, string oldPassword, string newPassword)
        {
            if (userId < 0)
            {
                throw new ArgumentException($"Value {nameof(userId)} mustn't be negative");
            }

            if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword))
            {
                throw new ArgumentException($"Values {nameof(oldPassword)} or {nameof(newPassword)} mustn't be empty");
            }

            User user = this.GetUserById(userId);

            if (user == null)
            {
                throw new InvalidOperationException($"User: {userId} hasn't found");
            }

            var oldHash = this.GetHash(oldPassword);

            if (!oldHash.SequenceEqual(user.Hash))
            {
                throw new InvalidOperationException("Old and new passwords do not match");
            }

            user.Hash = this.GetHash(newPassword);

            return this.InsertUser(user);
        }

        public bool SetUserState(int userId, bool enabled)
        {
            if (userId <= 0)
            {
                throw new ArgumentException($"{nameof(userId)} mustn't be negative");
            }

            if (!enabled)
            {
                ICollection<string> usersRoles = null;

                try
                {
                    usersRoles = Stores.RoleStore.ListRolesForUserByUserId(userId);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (usersRoles.Contains(AdminsRole))
                {
                    ICollection<User> admins = null;

                    try
                    {
                        admins = Stores.UserStore.ListUsersByRoleName(AdminsRole);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    if (admins.Where(u => u.Enabled == true).Count() <= 1)
                    {
                        throw new InvalidOperationException($"At least one user in role {AdminsRole} has to be active");
                    }
                }
            }

            User user = this.GetUserById(userId);

            user.Enabled = enabled;

            return this.InsertUser(user);
        }

        private bool IsValidUser(User user)
        {
            if (string.IsNullOrWhiteSpace(user.FirstName) ||
                string.IsNullOrWhiteSpace(user.LastName) ||
                string.IsNullOrWhiteSpace(user.UserName))
            {
                throw new ArgumentException("Arguments mustn't be null or contain only spaces");
            }

            if (user.UserName.Length < UsernameMinLength || user.Hash.Length < PasswordMinLength)
            {
                throw new ArgumentException($"Username is shorter than {UsernameMinLength} " +
                    $"or password is shorter than {PasswordMinLength}");
            }

            return true;
        }

        private byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = SHA1.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }
    }
}
