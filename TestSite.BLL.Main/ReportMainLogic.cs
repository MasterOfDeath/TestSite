namespace TestSite.BLL.Main
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Contract;
    using Entites;

    public class ReportMainLogic : IReportLogic
    {
        public int InsertReport(Report report)
        {
            int result = -1;

            try
            {
                this.IsValidReport(report);

                result = Stores.ReportStore.InsertReport(report);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public ICollection<Report> ListReportsByDep(int depId, DateTime start, DateTime end)
        {
            ICollection<Report> result = null;

            if (depId < -1)
            {
                throw new ArgumentException($"{nameof(depId)} не может быть отрицательным");
            }

            if (start == null && end == null)
            {
                throw new ArgumentException($"{nameof(start)} и {nameof(end)} не могут быть отрицательными");
            }

            try
            {
                result = Stores.ReportStore.ListReportsByDep(depId, start, end);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public ICollection<Report> ListReportsByEmployee(int employeeId, DateTime start, DateTime end)
        {
            ICollection<Report> result = null;

            if (employeeId < -1)
            {
                throw new ArgumentException($"{nameof(employeeId)} не может быть отрицательным");
            }

            if (start == null && end == null)
            {
                throw new ArgumentException($"{nameof(start)} и {nameof(end)} не могут быть отрицательными");
            }

            try
            {
                result = Stores.ReportStore.ListReportsByEmployee(employeeId, start, end);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public bool RemoveReport(int reportId)
        {
            if (reportId < -1)
            {
                throw new ArgumentException($"{nameof(reportId)} не может быть отрицательным");
            }

            try
            {
                Stores.ReportStore.RemoveReport(reportId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        private bool IsValidReport(Report report)
        {
            if (report == null)
            {
                throw new ArgumentException($"{nameof(report)} не может быть null");
            }

            if (report.Id < -1)
            {
                throw new ArgumentException($"{nameof(report.Id)} не может быть отрицательным");
            }

            if (report.TestId < 1)
            {
                throw new ArgumentException($"{nameof(report.TestId)} не может быть отрицательным");
            }

            if (report.EmployeeId < 1)
            {
                throw new ArgumentException($"{nameof(report.EmployeeId)} не может быть отрицательным");
            }

            if (report.Date > DateTime.Now)
            {
                throw new ArgumentException($"Дата не может быть в будущем");
            }

            if (report.ErrCount < 0)
            {
                throw new ArgumentException($"{nameof(report.ErrCount)} не может быть отрицательным");
            }

            if (report.ErrPercent < 0)
            {
                throw new ArgumentException($"{nameof(report.ErrPercent)} не может быть отрицательным");
            }

            return true;
        }
    }
}
