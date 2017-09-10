using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.SqlServer.Types;

namespace Demos
{
    public class User
    {
        public int Id;
        public string FirstName;
        public string LastName;
        public string EmailAddress;
        public JArray Tags;
        public Roles Roles = new Roles();
        public Company Company;
        public Preferences Preferences;

        public override string ToString()
        {
            return String.Format($"'({Id}) {FirstName},{LastName},{EmailAddress}");
        }
    }

    public class Company
    {
        public string CompanyName;
        public Address Address;

        public override string ToString()
        {
            return String.Format($"{CompanyName}");
        }
    }

    public class Address
    {
        public string Street;
        public string City;
        public string State;
        public string Country;
        public SqlGeography Location;

        public override string ToString()
        {
            string[] a = new string[] { Street, City, State, Country };

            var r = Array.FindAll<string>(a, s => !string.IsNullOrEmpty(s));
           
            return String.Format(string.Join(",", r));
        }
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
            return string.Join("/", this);
        }
    }

    public class Preferences
    {
        public string Theme;
        public string Style;
        public string Resolution;
    }
}
