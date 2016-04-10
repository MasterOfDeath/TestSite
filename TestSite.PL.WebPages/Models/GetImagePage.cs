namespace TestSite.PL.WebPages.Models
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Helpers;
    using Entites;
    using Logger;

    public class GetImagePage
    {
        private static readonly IDictionary<string, Func<HttpRequestBase, byte[]>> _Queries
            = new Dictionary<string, Func<HttpRequestBase, byte[]>>();

        static GetImagePage()
        {
            _Queries.Add("getSchema", GetSchema);
        }

        public static IDictionary<string, Func<HttpRequestBase, byte[]>> Queries
        {
            get { return _Queries; }
        }

        private static byte[] GetSchema(HttpRequestBase request)
        {
            var methodName = nameof(GetSchema);
            var questionIdStr = request["questionid"];
            byte[] image = null;

            if (string.IsNullOrEmpty(questionIdStr))
            {
                Logger.Log.Error(
                    methodName,
                    new Exception($"Invalid request: null values of {nameof(questionIdStr)}"));
                return null;
            }

            int questionId = 0;

            try
            {
                questionId = Convert.ToInt32(questionIdStr);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(methodName, ex);
                return null;
            }

            try
            {
                image = LogicProvider.QuestionLogic.GetImage(questionId);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(methodName, ex);
                return null;
            }

            return image;
        }
    }
}