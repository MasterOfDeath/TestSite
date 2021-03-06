﻿namespace TestSite.PL.WebPages.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Entites;

    public class RegularAjaxPage
    {
        public static IDictionary<string, Func<HttpRequestBase, AjaxResponse>> Queries { get; } =
            new Dictionary<string, Func<HttpRequestBase, AjaxResponse>>()
            {
                ["listEmployeesByDep"] = ListEmployeesByDep,
                ["listDeps"] = ListDeps,
            };

        private static AjaxResponse ListEmployeesByDep(HttpRequestBase request)
        {
            var methodName = nameof(ListEmployeesByDep);
            int depId = -1;
            int requestOwnerId = -1;
            ICollection<Employee> employees;

            try
            {
                depId = Convert.ToInt32(request["depId"]);
                requestOwnerId = Convert.ToInt32(request["requestowner"]);
            }
            catch (Exception ex)
            {
                return Common.SendError(ex, methodName);
            }

            if (depId < 0)
            {
                try
                {
                    depId = LogicProvider.EmployeeLogic.GetEmployeeById(requestOwnerId).Dep_Id;
                }
                catch (Exception ex)
                {
                    return Common.SendError(ex, methodName);
                }
            }

            try
            {
                employees = LogicProvider.EmployeeLogic.ListEmployeesByDepId(depId);
            }
            catch (Exception ex)
            {
                return Common.SendError(ex, methodName);
            }

            if (employees != null)
            {
                employees = employees.Select(e => { e.Hash = null; return e; }).ToList();
            }

            return new AjaxResponse(null, employees);
        }

        private static AjaxResponse ListDeps(HttpRequestBase request)
        {
            var methodName = nameof(ListDeps);

            ICollection<Dep> deps = null;
            try
            {
                deps = LogicProvider.DepLogic.ListAllDeps();
            }
            catch (Exception ex)
            {
                return Common.SendError(ex, methodName);
            }

            return new AjaxResponse(null, deps);
        }
    }
}