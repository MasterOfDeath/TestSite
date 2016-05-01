namespace TestSite.BLL.Main
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using Contract;
    using Entites;
    using Logger;

    public class EmployeeMainLogic : IEmployeeLogic
    {
        private readonly string SuperAdminPass = ConfigurationManager.AppSettings["superAdminPass"];

        private const int PasswordMinLength = 6;
        private const int AdminRoleId = 1;
        private const int InspectorRoleId = 4;
        private const int InspectorsDepId = 2;
        private const int SuperAdminId = 1;
        private readonly Regex passwordEx = new Regex($"^(?=.{{{PasswordMinLength},}}$)[^\\s]+$"); //^(?=.{6,}$)[^\s]+$

        public int AddEmployee(Employee employee, string password)
        {
            if (password == null || !passwordEx.IsMatch(password))
            {
                throw new ArgumentException($"Пароль не соответсвует требованиям безопасности");
            }

            this.IsValidEmployee(employee);

            employee.Hash = this.GetHash(password);

            int result = this.InsertEmployee(employee);

            if (result > 0)
            {
                Logger.Log.Info($"Создан пользователь: {employee.Id} {employee.FirstName} {employee.LastName}");
            }

            return result;
        }

        public Employee CanLogin(int employeeId, string password)
        {
            if (employeeId < 1)
            {
                throw new ArgumentException($"{nameof(employeeId)} не может быть 0 или меньше");
            }

            Employee employee = null;

            try
            {
                employee = Stores.EmployeeStore.GetEmployeeById(employeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (employee == null)
            {
                throw new InvalidOperationException($"Пользователь {employeeId} не найден");
            }

            // Пароль Супер админа из Web.config
            if (employeeId == SuperAdminId)
            {
                if (SuperAdminPass != null && passwordEx.IsMatch(SuperAdminPass) && password == SuperAdminPass)
                {
                    return employee;
                }
            }

            return employee.Hash.SequenceEqual(this.GetHash(password)) ? employee : null;
        }

        public int InsertEmployee(Employee employee)
        {
            int result = -1;

            this.IsValidEmployee(employee);

            if (employee.Id == SuperAdminId)
            {
                throw new ArgumentException("Данный пользователь является системным, изменение запрещено");
            }

            // Инспекторы лежат только в отделе Инспекторы
            if (employee.Role_Id == InspectorRoleId)
            {
                employee.Dep_Id = InspectorsDepId;
            }

            // Проверка на последнего администратора
            try
            {
                var oldEmployee = Stores.EmployeeStore.GetEmployeeById(employee.Id);
                if (oldEmployee != null && oldEmployee.Role_Id == AdminRoleId && employee.Role_Id != AdminRoleId)
                {
                    var employees = Stores.EmployeeStore.ListEmployeesByDepId(employee.Dep_Id);
                    if (employees.Count(e => e.Role_Id == AdminRoleId) == 1)
                    {
                        throw new InvalidOperationException("Нельзя удалить последнего администратора в отделе");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            try
            {
                result = Stores.EmployeeStore.InsertEmployee(employee);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public ICollection<Employee> ListAllEmployees()
        {
            ICollection<Employee> result = null;

            try
            {
                result = Stores.EmployeeStore.ListAllEmployees();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public bool RemoveEmployee(int employeeId)
        {
            if (employeeId < 1)
            {
                throw new ArgumentException($"{nameof(employeeId)} не может быть 0 или меньше");
            }

            if (employeeId == SuperAdminId)
            {
                throw new ArgumentException("Данный пользователь является системным, удаление запрещено");
            }

            // Проверка на последнего администратора
            try
            {
                var employee = Stores.EmployeeStore.GetEmployeeById(employeeId);
                if (employee != null && employee.Role_Id == AdminRoleId)
                {
                    var employees = Stores.EmployeeStore.ListEmployeesByDepId(employee.Dep_Id);
                    if (employees.Count(e => e.Role_Id == AdminRoleId) == 1)
                    {
                        throw new InvalidOperationException("Нельзя удалить последнего администратора в отделе");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            try
            {
                Stores.EmployeeStore.RemoveEmployee(employeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        public bool ChangePassword(int employeeId, string oldPassword, string newPassword, bool godMode)
        {
            if (employeeId < 1)
            {
                throw new ArgumentException($"{nameof(employeeId)} не может быть 0 или меньше");
            }

            if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(oldPassword))
            {
                throw new ArgumentException("Новый или старый пароль не может быть пуст");
            }

            if (!passwordEx.IsMatch(newPassword))
            {
                throw new ArgumentException($"Пароль не соответсвует требованиям безопасности");
            }

            Employee employee = null;

            if (godMode)
            {
                employee = this.GetEmployeeById(employeeId);
            }
            else
            {
                employee = this.CanLogin(employeeId, oldPassword);
            }

            if (employee != null)
            {
                employee.Hash = this.GetHash(newPassword);
            }
            else
            {
                Logger.Log.Info($"При попытке смены пароля, пользовател: {employeeId} ввел не верный текущий пароль");
                throw new InvalidOperationException("Не верный текущий пароль");
            }

            int result = -1;
            try
            {
                result = Stores.EmployeeStore.InsertEmployee(employee);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (result > 0)
            {
                Logger.Log.Info($"Пользователь {employee.Id} {employee.FirstName} {employee.LastName} " +
                                "сменил пароль");
            }

            return result > 0;
        }

        public Employee GetEmployeeById(int employeeId)
        {
            if (employeeId < 1)
            {
                throw new ArgumentException($"{nameof(employeeId)} не может быть меньше 1");
            }

            Employee result;

            try
            {
                result = Stores.EmployeeStore.GetEmployeeById(employeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public ICollection<Employee> ListEmployeesByDepId(int depId)
        {
            if (depId < 1)
            {
                throw new ArgumentException($"{nameof(depId)} не может быть меньше 1");
            }

            ICollection<Employee> result = null;

            try
            {
                result = Stores.EmployeeStore.ListEmployeesByDepId(depId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public ICollection<string> ListRolesForUserByUserId(int employeeId)
        {
            if (employeeId < 0)
            {
                throw new ArgumentException($"{nameof(employeeId)} не может быть отрицательным");
            }

            return Stores.EmployeeStore.ListRolesForUserByUserId(employeeId);
        }

        private bool IsValidEmployee(Employee employee)
        {
            if (employee == null)
            {
                throw new ArgumentException($"{nameof(employee)} не может быть null");
            }

            //if (admin.UserName.Length < UsernameMinLength)
            //{
            //    throw new ArgumentException($"Имя пользователя короче чем {UsernameMinLength}");
            //}

            return true;
        }

        private byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }
    }
}
