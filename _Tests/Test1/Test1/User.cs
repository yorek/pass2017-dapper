using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Test1
{
    public class User
    {
        public int Id;
        public string FirstName;
        public string LastName;
        public string EmailAddress;
        public JArray Tags;
        public Roles Roles;
        public Preferences Preferences;
    }

    public class Role
    {
        public string RoleName;

        public override string ToString() 
        {
            return RoleName;
        }
    }

    public class Roles: List<Role>
    {
        public override string ToString()
        {
            return string.Join(",", this);
        }
    }

    public class Preferences
    {
        public string Theme;
        public string Style;
        public string Resolution;
    }
}
