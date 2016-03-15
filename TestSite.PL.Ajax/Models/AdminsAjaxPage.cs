namespace Photos.PL.WebPages.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Helpers;
    using Entites;
    using Logger;
    
    public static class AdminsAjaxPage
    {
        private static readonly IDictionary<string, Func<HttpRequestBase, AjaxResponse>> _Queries
            = new Dictionary<string, Func<HttpRequestBase, AjaxResponse>>();

        static AdminsAjaxPage()
        {
            _Queries.Add("clickSecurityPromptBtn", ClickSecurityPromptBtn);
            _Queries.Add("getHtmlForUsersSecurityTable", GetHtmlForUsersSecurityTable);
        }

        public static IDictionary<string, Func<HttpRequestBase, AjaxResponse>> Queries
        {
            get { return _Queries; }
        }

        private static AjaxResponse ClickSecurityPromptBtn(HttpRequestBase request)
        {
            string userIdStr = null;
            dynamic roleChanges = null;
            dynamic enableChange = null;
            var methodName = nameof(ClickSecurityPromptBtn);

            try
            {
                userIdStr = request["userid"];
                roleChanges = Json.Decode(request["rolechanges"]);
                enableChange = Json.Decode(request["enablechange"]);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            int userId = 0;

            try
            {
                userId = Convert.ToInt32(userIdStr);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            foreach (var item in roleChanges)
            {
                if (item.action)
                {
                    try
                    {
                        LogicProvider.RoleLogic.GiveRole(userId, item.role);
                    }
                    catch (Exception ex)
                    {
                        return SendError(ex, methodName);
                    }

                    Logger.Log.Info($"Role: {item.role} was given to user: {userIdStr}");
                }
                else
                {
                    try
                    {
                        LogicProvider.RoleLogic.PullOffRole(userId, item.role);
                    }
                    catch (Exception ex)
                    {
                        return SendError(ex, methodName);
                    }

                    Logger.Log.Info($"Role: {item.role} was pulled off from user: {userIdStr}");
                }
            }

            if (enableChange != null)
            {
                try
                {
                    LogicProvider.UserLogic.SetUserState(userId, enabled: enableChange);
                }
                catch (Exception ex)
                {
                    return SendError(ex, methodName);
                }

                Logger.Log.Info($"State of user: {userIdStr} was changed to: {enableChange}");
            }
            
            return new AjaxResponse(null, true);
        }

        private static AjaxResponse GetHtmlForUsersSecurityTable(HttpRequestBase request)
        {
            string userIdStr = null;
            var methodName = nameof(GetHtmlForUsersSecurityTable);

            try
            {
                userIdStr = request["userid"];
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            int userId = 0;

            try
            {
                userId = Convert.ToInt32(userIdStr);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            User user = null;

            try
            {
                user = LogicProvider.UserLogic.GetUserById(userId);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            if (user == null)
            {
                return SendError($"User: {userIdStr} hasn't found in data store", methodName);
            }

            ICollection<string> usersRoles = null;

            try
            {
                usersRoles = LogicProvider.RoleLogic.ListRolesForUser(user.UserName);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName); ;
            }

            if (usersRoles == null)
            {
                return SendError($"User: {userIdStr} hasn't any roles", methodName);
            }

            string returnStr = null;

            returnStr = 
                $"<tr data-user-id='{user.Id}'>" + 
                $"<td class='username'>{user.UserName}</td>" + 
                $"<td class='roles'>{string.Join(", ", usersRoles)}</td>" + 
                $"<td class='enabled'>{user.Enabled}</td>" +
                $"</tr>";

            return new AjaxResponse(null, returnStr);
        }

        private static AjaxResponse SendError(Exception ex, string sender = null)
        {
            if (sender == null)
            {
                Logger.Log.Error(ex.Message);
            }
            else
            {
                Logger.Log.Error(sender, ex);
            }

            return new AjaxResponse(ex.Message);
        }

        private static AjaxResponse SendError(string message, string logMessage, string sender = null)
        {
            if (sender == null)
            {
                Logger.Log.Error(logMessage);
            }
            else
            {
                Logger.Log.Error(sender, new Exception(logMessage));
            }

            return new AjaxResponse(message);
        }
    }
}