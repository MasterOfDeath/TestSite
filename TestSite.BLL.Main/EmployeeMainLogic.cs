namespace TestSite.BLL.Main
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using Contract;
    using Entites;
    using Logger;

    public class EmployeeMainLogic : IEmployeeLogic
    {
        private const int UsernameMinLength = 6;
        private const int PasswordMinLength = 6;
        private const string defaultRole = "user";
        private readonly Regex passwordEx = new Regex("^(?=.{" + PasswordMinLength + ",}$)[^\\s]+$"); //^(?=.{6,}$)[^\s]+$

        public int AddEmployee(Employee employee, string password)
        {
            if (!passwordEx.IsMatch(password))
            {
                throw new ArgumentException($"Пароль не соответсвует требованиям безопасности");
            }

            employee.Hash = this.GetHash(password);

            int result = this.InsertEmployee(employee);

            if (result > 0)
            {
                Logger.Log.Info($"Создан пользователь: {employee.Id} {employee.FirstName} {employee.LastName}");
                Stores.RoleStore.GiveRole(result, defaultRole);
            }

            return result;
        }

        public Employee CanLogin(int employeeId, string password)
        {
            if (employeeId < 1)
            {
                throw new ArgumentException($"{nameof(employeeId)} не может быть 0 или меньше");
            }

            var employee = new Employee(0, null, null, null, false);

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

            return employee.Hash.SequenceEqual(this.GetHash(password)) ? employee : null;
        }

        public int InsertEmployee(Employee employee)
        {
            int result = -1;

            try
            {
                this.IsValidEmployee(employee);

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

        public bool ChangePassword(int employeeId, string oldPassword, string newPassword)
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

            var employee = this.CanLogin(employeeId, oldPassword);

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

        private bool IsValidEmployee(Employee employee)
        {
            //if (string.IsNullOrWhiteSpace(employee.))
            //{
            //    throw new ArgumentException("Имя пользователя не может быть пустым или состоять из пробелов");
            //}

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
