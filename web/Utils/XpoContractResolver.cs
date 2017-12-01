using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo;

namespace web.Utils
{
    // Custom resolver - skips properties found on the XPO base class types, since
    // these are unnecessary and created issues with JSON serialization
    public class XpoContractResolver : DefaultContractResolver
    {
        static List<Type> incompatibleTypes = new List<Type>{
            typeof(XPCustomObject),
            typeof(XPBaseObject),
            typeof(PersistentBase)
        };

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            if (incompatibleTypes.Contains(member.DeclaringType))
                return null;
            else return base.CreateProperty(member, memberSerialization);
        }
    }
}