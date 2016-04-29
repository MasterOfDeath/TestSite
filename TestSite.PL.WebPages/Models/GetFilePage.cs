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
    using SpreadsheetLight;

    public class GetFilePage
    {
        private static readonly IDictionary<string, Func<HttpRequestBase, Tuple<byte[], string>>> _Queries
            = new Dictionary<string, Func<HttpRequestBase, Tuple<byte[], string>>>();

        static GetFilePage()
        {
            _Queries.Add("getSchema", GetSchema);
            _Queries.Add("saveDataToFileByDep", SaveDataToFile);
            _Queries.Add("saveDataToFileByEmployee", SaveDataToFile);
        }

        public static IDictionary<string, Func<HttpRequestBase, Tuple<byte[], string>>> Queries
        {
            get { return _Queries; }
        }

        private static Tuple<byte[], string> GetSchema(HttpRequestBase request)
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

            return new Tuple<byte[], string>(image, "image/svg+xml");
        }

        private static Tuple<byte[], string> SaveDataToFile(HttpRequestBase request)
        {
            string queryName;
            int employeeId = -1;
            DateTime dateStart;
            DateTime dateEnd;

            try
            {
                queryName = request["queryName"];
                employeeId = Convert.ToInt32(request["employeeid"]);
                dateStart = Convert.ToDateTime(request["datestart"]);
                dateEnd = Convert.ToDateTime(request["dateend"]);
            }
            catch (Exception)
            {
                return null;
            }

            List<Dictionary<string, object>> results = null;
            MemoryStream destination = new MemoryStream();

            var template = AppDomain.CurrentDomain.GetData("DataDirectory") + @"\ReportTemplate\report.xlsx";

            if (!File.Exists(template))
            {
                return null;
            }

            try
            {
                var employee = LogicProvider.EmployeeLogic.GetEmployeeById(employeeId);
                dateEnd = dateEnd.AddHours(23 - dateEnd.Hour);
                ICollection<Report> reports = null;

                if (queryName == "saveDataToFileByEmployee")
                {
                    reports = LogicProvider.ReportLogic.ListReportsByEmployee(employee.Id, dateStart, dateEnd);
                }

                if (queryName == "saveDataToFileByDep")
                {
                    reports = LogicProvider.ReportLogic.ListReportsByDep(employee.Dep_Id, dateStart, dateEnd);
                }

                if (reports == null)
                {
                    return null;
                }

                results = UsersAjaxPage.GetReportData(reports);

                SLDocument sl = new SLDocument(template, "Protokol");

                var style = sl.GetCellStyle(2, 1);

                var line = 3;

                foreach (var result in results)
                {
                    sl.InsertRow(line, 1);
                    sl.SetCellValue(line, 1, (string)result["Date"]);
                    sl.SetCellStyle(line, 1, style);
                    sl.SetCellValue(line, 2, (string)result["Employee"]);
                    sl.SetCellStyle(line, 2, style);
                    sl.SetCellValue(line, 3, (string)result["Test"]);
                    sl.SetCellStyle(line, 3, style);
                    sl.SetCellValue(line, 4, (int)result["ErrCount"]);
                    sl.SetCellStyle(line, 4, style);
                    sl.SetCellValue(line, 5, (int)result["ErrPercent"]);
                    sl.SetCellStyle(line, 5, style);

                    line++;
                }

                sl.AutoFitRow(1, line, 40);

                sl.SaveAs(destination);
            }
            catch (Exception)
            {
                return null;
            }

            var filename = HttpUtility.UrlEncode(
                        "Протокол-" + dateStart.ToShortDateString() + "-" + dateEnd.ToShortDateString() + ".xlsx",
                        System.Text.Encoding.UTF8);
            HttpContext.Current.Response.AppendCookie(new HttpCookie("fileDownload", "true") { Path = "/" });
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename);

            return new Tuple<byte[], string>(destination.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }
}