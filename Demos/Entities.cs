using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.SqlServer.Types;
using Dapper;
using Dapper.FluentMap.Mapping;

namespace Demos.Entities
{ 
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public JArray Tags { get; set; }
        public Roles Roles { get; set; }
        public Company Company { get; set; }
        public Preferences Preferences { get; set; }

        public override string ToString()
        {
            return String.Format($"({Id}) {FirstName},{LastName},{EmailAddress}");
        }
    }

    public class Company
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public Address Address { get; set; }

        public override string ToString()
        {
            return String.Format($"({Id}) {CompanyName}");
        }
    }

    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public SqlGeography Location { get; set; }

        public override string ToString()
        {
            string[] a = new string[] { Street, City, State, Country };

            var r = Array.FindAll<string>(a, s => !string.IsNullOrEmpty(s));
           
            return String.Format(string.Join(",", r));
        }
    }

    public class Role
    {
        public string RoleName { get; set; }

        public Role()
        {
        }

        public Role(string name)
        {
            RoleName = name;
        }

        public override string ToString() 
        {
            return RoleName;
        }
    }

    public class Roles: List<Role>
    {
        public override string ToString()
        {
            return string.Join("|", this);
        }
    }

    public class Preferences
    {
        public string Theme;
        public string Style;
        public string Resolution;
    }
}

