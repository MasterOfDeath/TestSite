namespace TestSite.BLL.Main
{
    using System;
    using System.Collections.Generic;
    using Contract;
    using Entites;

    public class DepMainLogic : IDepLogic
    {
        public Dep GetDepById(int depId)
        {
            Dep result = null;

            try
            {
                result = Stores.DepStore.GetDepById(depId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public bool InsertDep(Dep dep)
        {
            var result = false;

            try
            {
                this.IsValidDep(dep);

                result = Stores.DepStore.InsertDep(dep);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public ICollection<Dep> ListAllDeps()
        {
            ICollection<Dep> result = null;

            try
            {
                result = Stores.DepStore.ListAllDeps();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public bool RemoveDep(int depId)
        {
            try
            {
                Stores.DepStore.RemoveDep(depId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        private bool IsValidDep(Dep dep)
        {
            if (dep == null)
            {
                throw new ArgumentException($"{nameof(dep)} не может быть null");
            }

            if (string.IsNullOrWhiteSpace(dep?.Name))
            {
                throw new ArgumentException("Название отдела не может быть пустым или состоять из пробелов");
            }

            if (dep.Id < -1)
            {
                throw new ArgumentException($"Не легальный номер отдела");
            }

            return true;
        }
    }
}
