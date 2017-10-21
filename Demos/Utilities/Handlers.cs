using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Dapper;
using Dapper.FluentMap.Mapping;
using Demos.Entities;

namespace Demos.Handlers
{
    public class JArrayTypeHandler : SqlMapper.TypeHandler<JArray>
    {
        public override JArray Parse(object value)
        {
            string json = value.ToString();
            json.Replace("\"", "'");
            return JArray.Parse(value.ToString());
        }

        public override void SetValue(IDbDataParameter parameter, JArray value)
        {
            parameter.Value = value.ToString();
        }
    }

    public class RolesTypeHandler : SqlMapper.TypeHandler<Roles>
    {
        public override Roles Parse(object value)
        {
            Roles result = new Roles();

            string[] roles = value.ToString().Split('|');
            foreach (var r in roles)
            {
                result.Add(new Role() { RoleName = r });
            }

            return result;
        }

        public override void SetValue(IDbDataParameter parameter, Roles value)
        {
            throw new NotImplementedException();
        }
    }

    public class UserTypeHandler : SqlMapper.TypeHandler<User>
    {
        public override User Parse(object value)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(IDbDataParameter parameter, User value)
        {
            parameter.Value = JsonConvert.SerializeObject(value);
        }
    }
}
