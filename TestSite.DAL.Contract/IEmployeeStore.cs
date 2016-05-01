namespace TestSite.DAL.Contract
{
    using System.Collections.Generic;
    using Entites;

    public interface IEmployeeStore
    {
        int InsertEmployee(Employee employee);

        bool RemoveEmployee(int employeeId);

        ICollection<Employee> ListAllEmployees();

        ICollection<Employee> ListEmployeesByRoleName(string roleName);

        ICollection<Employee> ListEmployeesByDepId(int depId);

        Employee GetEmployeeById(int employeeId);

        ICollection<string> ListRolesForUserByUserId(int employeeId);
    }
}
