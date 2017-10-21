using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Dapper;
using Dapper.FluentMap.Mapping;
using Demos.Entities;

namespace Demos.Mappers
{
    public class UserMap : EntityMap<User>
    {
        public UserMap()
        {
            Map(u => u.Id).ToColumn("UserId");
        }
    }

    public class CompanyMap : EntityMap<Company>
    {
        public CompanyMap()
        {
            Map(c => c.Id).ToColumn("CompanyId");
        }
    }
}
