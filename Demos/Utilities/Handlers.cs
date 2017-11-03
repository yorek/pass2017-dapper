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
            return Roles.FromString(value.ToString());
        }

        public override void SetValue(IDbDataParameter parameter, Roles value)
        {
            parameter.Value = value.ToString();
        }
    }

    public class UserTypeHandler : SqlMapper.TypeHandler<User>
    {
        public override User Parse(object value)
        {
            return JsonConvert.DeserializeObject<User>(value.ToString());
        }

        public override void SetValue(IDbDataParameter parameter, User value)
        {
            parameter.Value = JsonConvert.SerializeObject(value);
        }
    }
}
