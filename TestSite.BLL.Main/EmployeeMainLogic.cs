namespace TestSite.BLL.Main
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using Contract;
    using Entites;

    public class EmployeeMainLogic : IEmployeeLogic
    {
        private const int UsernameMinLength = 6;
        private const int PasswordMinLength = 6;
        private const string defaultRole = "user";

        public int AddEmployee(Employee employee, string password)
        {
            employee.Hash = this.GetHash(password);

            int result = this.InsertEmployee(employee);

            if (result > 0)
            {
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

        public bool ChangePassword(Employee employee, string oldPassword, string newPassword)
        {
            try
            {
                this.IsValidEmployee(employee);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(oldPassword))
            {
                throw new ArgumentException("Новый или старый пароль не может быть пуст");
            }

            employee = this.CanLogin(employee.Id, oldPassword);

            if (employee != null)
            {
                employee.Hash = this.GetHash(newPassword);
            }
            else
            {
                throw new InvalidOperationException("Не верный старый пароль");
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

            return result > 0;
        }

        private bool IsValidEmployee(Employee employee)
        {
            //if (string.IsNullOrWhiteSpace(admin.UserName))
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
    }
}
