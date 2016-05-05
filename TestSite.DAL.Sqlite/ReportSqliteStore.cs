namespace TestSite.DAL.Sqlite
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SQLite;
    using Contract;
    using Entites;

    public class ReportSqliteStore : IReportStore
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["Sqlite"].ConnectionString;

        private readonly DateTime zeroUnixDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public int InsertReport(Report report)
        {
            int result = -1;

            using (var connection = new SQLiteConnection(this.connectionString))
            {
                connection.Open();
                var insert = "INSERT INTO report (employee_id, test_id, date, err_count, err_percent) " +
                             "VALUES (:employeeId, :testId, :date, :errCount, :errPercent); " +
                             "SELECT last_insert_rowid() AS id;";
                var update = "UPDATE report SET " +
                             "employee_id=:employeeId, test_id=:testId, date=:date, err_count=:errCount, err_percent=:errPercent " +
                             "WHERE id=:id";

                if (report.Id > 0)
                {
                    using (var command = new SQLiteCommand(update, connection))
                    {
                        command.Parameters.AddWithValue(":id", report.Id);
                        command.Parameters.AddWithValue(":employeeId", report.EmployeeId);
                        command.Parameters.AddWithValue(":testId", report.TestId);
                        command.Parameters.AddWithValue(":date", this.DateTimeToUnixTime(report.Date));
                        command.Parameters.AddWithValue(":errCount", report.ErrCount);
                        command.Parameters.AddWithValue(":errPercent", report.ErrPercent);
                        result = (command.ExecuteNonQuery() > 0) ? report.Id : -1;
                    }
                }
                else
                {
                    using (var command = new SQLiteCommand(insert, connection))
                    {
                        command.Parameters.AddWithValue(":employeeId", report.EmployeeId);
                        command.Parameters.AddWithValue(":testId", report.TestId);
                        command.Parameters.AddWithValue(":date", this.DateTimeToUnixTime(report.Date));
                        command.Parameters.AddWithValue(":errCount", report.ErrCount);
                        command.Parameters.AddWithValue(":errPercent", report.ErrPercent);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                result = reader.GetInt32(0);
                            }
                            else
                            {
                                result = -1;
                            }
                        }
                    }
                }
            }

            return result;
        }

        public ICollection<Report> ListAllReports()
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                using (var command = new SQLiteCommand("SELECT * FROM report", connection))
                {
                    List<Report> result = null;

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            result = new List<Report>(reader.StepCount);
                        }

                        while (reader.Read())
                        {
                            result.Add(this.RowToReport(reader));
                        }

                        return result;
                    }
                }
            }
        }

        public ICollection<Report> ListReportsByDep(int depId, DateTime start, DateTime end, bool emplOrder)
        {
            var select = "SELECT report.* , employee.dep_id " +
                         "FROM report " +
                         "JOIN employee ON report.employee_id = employee.id " +
                         "JOIN test ON report.test_id = test.id " +
                         "WHERE (\"date\" BETWEEN :dateStart AND :dateEnd) AND (employee.dep_id = :depId)";

            if (emplOrder)
            {
                select = select + " ORDER BY employee_id, test_id, id";
            }

            using (var connection = new SQLiteConnection(this.connectionString))
            {
                using (var command = new SQLiteCommand(select, connection))
                {
                    command.Parameters.AddWithValue(":dateStart", this.DateTimeToUnixTime(start));
                    command.Parameters.AddWithValue(":dateEnd", this.DateTimeToUnixTime(end));
                    command.Parameters.AddWithValue(":depId", depId);
                    List<Report> result = null;

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            result = new List<Report>(reader.StepCount);
                        }

                        while (reader.Read())
                        {
                            result.Add(this.RowToReport(reader));
                        }

                        return result;
                    }
                }
            }
        }

        public ICollection<Report> ListReportsByEmployee(int employeeId, DateTime start, DateTime end, bool emplOrder)
        {
            var select = "SELECT * " +
                         "FROM report " +
                         "WHERE (\"date\" BETWEEN :dateStart AND :dateEnd) AND (employee_id = :employeeId)";

            if (emplOrder)
            {
                select = select + " ORDER BY employee_id, test_id, id";
            }

            using (var connection = new SQLiteConnection(this.connectionString))
            {
                using (var command = new SQLiteCommand(select, connection))
                {
                    command.Parameters.AddWithValue(":dateStart", this.DateTimeToUnixTime(start));
                    command.Parameters.AddWithValue(":dateEnd", this.DateTimeToUnixTime(end));
                    command.Parameters.AddWithValue(":employeeId", employeeId);
                    List<Report> result = null;

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            result = new List<Report>(reader.StepCount);
                        }

                        while (reader.Read())
                        {
                            result.Add(this.RowToReport(reader));
                        }

                        return result;
                    }
                }
            }
        }

        public bool RemoveReport(int reportId)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                connection.Open();

                var delete = "PRAGMA foreign_keys = ON; DELETE FROM report WHERE id=:id;";

                using (var command = new SQLiteCommand(delete, connection))
                {
                    command.Parameters.AddWithValue(":id", reportId);
                    var result = command.ExecuteNonQuery();

                    return result > 0;
                }
            }
        }

        private Report RowToReport(SQLiteDataReader reader)
        {
            var id = reader.GetInt32(0);
            var employeeId = reader.GetInt32(1);
            var testId = reader.GetInt32(2);
            var dateEpoch = reader.GetInt64(3);
            var errCount = reader.GetInt32(4);
            var errPercent = reader.GetInt32(5);

            return new Report(id, employeeId, testId, this.UnixTimeToDateTime(dateEpoch), errCount, errPercent);
        }

        private DateTime UnixTimeToDateTime(long unixTimeStamp)
        {
            return this.zeroUnixDate.AddSeconds(unixTimeStamp).ToUniversalTime();
        }

        private long DateTimeToUnixTime(DateTime date)
        {
            TimeSpan span = date - this.zeroUnixDate;

            return (long)span.TotalSeconds;
        }
    }
}
