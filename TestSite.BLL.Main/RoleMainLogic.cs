namespace TestSite.BLL.Main
{
    using System;
    using System.Collections.Generic;
    using Contract;
    using Entites;
    using Logger;

    public class RoleMainLogic : IRoleLogic
    {
        private const string adminRole = "admin";

        public bool GiveRole(int userId, string roleName)
        {
            if (userId < 0)
            {
                throw new ArgumentException($"{nameof(userId)} должно быть отрицательным");
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException($"{nameof(roleName)} не может быть пустым");
            }

            ICollection<string> usersRoles = null;

            try
            {
                usersRoles = Stores.RoleStore.ListRolesForUserByUserId(userId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (usersRoles.Contains(roleName))
            {
                throw new InvalidOperationException($"Пользователь: {userId} уже имеет роль: {roleName}");
            }

            var result = Stores.RoleStore.GiveRole(userId, roleName);

            if (result)
            {
                Logger.Log.Info($"Пользователю: {userId} присвоенна роль: {roleName}");
            }

            return result;
        }

        public ICollection<string> ListAllRoles()
        {
            return Stores.RoleStore.ListAllRoles();
        }

        public ICollection<string> ListRolesForUserByUserId(int userId)
        {
            if (userId < 0)
            {
                throw new ArgumentException($"{nameof(userId)} не может быть отрицательным");
            }

            return Stores.RoleStore.ListRolesForUserByUserId(userId);
        }

        public bool PullOffRole(int userId, string roleName)
        {
            if (userId < 0)
            {
                throw new ArgumentException($"{nameof(userId)} не может быть отрицательным");
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException($"{nameof(roleName)} не может быть пустым");
            }

            ICollection<string> usersRoles = null;

            try
            {
                usersRoles = Stores.RoleStore.ListRolesForUserByUserId(userId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (usersRoles == null || usersRoles?.Count <= 1)
            {
                throw new InvalidOperationException("Пользователь не может не иметь роль");
            }

            ICollection<Employee> admins = null;

            try
            {
                admins = Stores.EmployeeStore.ListEmployeesByRoleName(adminRole);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (admins.Count <= 1)
            {
                throw new InvalidOperationException($"В системе должен быть хотя бы один: {adminRole}");
            }

            return Stores.RoleStore.PullOffRole(userId, roleName);
        }
    }
}
