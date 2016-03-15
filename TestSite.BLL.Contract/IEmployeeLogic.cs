namespace TestSite.BLL.Contract
{
    using System.Collections.Generic;
    using Entites;

    public interface IEmployeeLogic
    {
        int AddEmployee(Employee employee, string password);

        int InsertEmployee(Employee employee);

        bool RemoveEmployee(int employeeId);

        ICollection<Employee> ListAllEmployees();

        ICollection<Employee> ListEmployeesByDepId(int depId);

        Employee CanLogin(int employeeId, string password);

        bool ChangePassword(Employee employee, string oldPassword, string newPassword);

        Employee GetEmployeeById(int employeeId);
    }
}
