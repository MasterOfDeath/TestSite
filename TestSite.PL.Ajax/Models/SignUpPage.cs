namespace Photos.PL.WebPages.Models
{
    using System;
    using Entites;
    using Logger;

    public static class SignUpPage
    {
        public static bool AddUser(string firstname, string lastname, string username, string password)
        {
            var result = false;

            if (firstname == null || lastname == null || username == null || password == null)
            {
                Logger.Log.Error(nameof(AddUser), new Exception("Arguments mustn't be null"));
                throw new ArgumentException("Arguments mustn't be null");
            }

            var user = new User(firstname, lastname, username, hash: null, tariff_id: 0, enabled: true);

            try
            {
                result = LogicProvider.UserLogic.AddUser(user, password);
            }
            catch (Exception ex)
            {
                // TODO Fix this catch. Maybe FATAL?
                Logger.Log.Error("AddUser", ex);
                throw ex;
            }

            return result;
        }
    }
}