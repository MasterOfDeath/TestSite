namespace Photos.PL.WebPages.Models
{
    using System;
    using System.Linq;
    using System.Web.Security;
    using Logger;

    public class PhotosRoleProvider : RoleProvider
    {
        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override string[] GetRolesForUser(string username)
        {
            string[] result = null;

            try
            {
                result = LogicProvider.RoleLogic.ListRolesForUser(username).ToArray();
            }
            catch (Exception ex)
            {   // TODO Maybe FATAL?
                Logger.Log.Error(nameof(LogicProvider.RoleLogic.ListRolesForUser), ex);
            }

            return result;
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            var result = false;

            try
            {
                result = this.GetRolesForUser(username).Contains(roleName);
            }
            catch (Exception ex)
            {   // TODO Maybe FATAL?
                Logger.Log.Error(nameof(this.IsUserInRole), ex);
            }
            return result;
        }

        #region NotImplemented
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}