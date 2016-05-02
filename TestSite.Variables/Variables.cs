namespace TestSite.Variables
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Entites;

    public static class Variables
    {
        public static readonly Role AdminRole = new Role(1, "admin");
        public static readonly Role UserRole = new Role(2, "user");
        public static readonly Role SuperadminRole = new Role(3, "superadmin");
        public static readonly Role InspectorRole = new Role(4, "inspector");

        public static readonly List<Role> Roles = new List<Role>()
        {
            AdminRole,
            UserRole,
            SuperadminRole,
            InspectorRole,
        };

        public static readonly int SuperadminId = 1;

        public static readonly Dep SuperadminsDep = new Dep(1, "Супер Администратор");
        public static readonly Dep InspectorsDep = new Dep(2, "Инспекторы");
    }
}
