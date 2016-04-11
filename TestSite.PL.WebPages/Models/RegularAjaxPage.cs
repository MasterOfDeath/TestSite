﻿namespace TestSite.PL.WebPages.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Entites;
    using Logger;
    using System.Text;
    using System.Web.Security;
    using System.Web.WebPages;
    public class RegularAjaxPage
    {
        private static readonly IDictionary<string, Func<HttpRequestBase, AjaxResponse>> _Queries
            = new Dictionary<string, Func<HttpRequestBase, AjaxResponse>>();

        static RegularAjaxPage()
        {
            _Queries.Add("changeDepSelector", ChangeDepSelector);
        }

        public static IDictionary<string, Func<HttpRequestBase, AjaxResponse>> Queries
        {
            get { return _Queries; }
        }

        private static AjaxResponse ChangeDepSelector(HttpRequestBase request)
        {
            string depIdStr = null;
            var methodName = nameof(ChangeDepSelector);

            try
            {
                depIdStr = request["depid"];
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            int depId;

            try
            {
                depId = Convert.ToInt32(depIdStr);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            ICollection<Employee> employees = null;
            try
            {
                employees = LogicProvider.EmployeeLogic.ListEmployeesByDepId(depId);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            string result = null;

            if (employees != null)
            {
                var sb = new StringBuilder(employees.Count);
                foreach (var employee in employees)
                {
                    sb.Append($"<option value='{employee.Id}'>{employee.LastName} {employee.FirstName}</option>");
                }

                result = sb.ToString();
            }

            return new AjaxResponse(null, result);
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