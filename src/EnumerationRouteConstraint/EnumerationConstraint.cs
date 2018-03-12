using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Routing;

namespace DeLoachAero.WebApi
{
    /// <summary>
    /// Custom Web API route constraint to validate a string value is in the list
    /// of an enumeration's members.
    /// </summary>
    /// <remarks>
    /// You will need to register the constraint in your WebApiConfig.cs file:
    /// <code>
    ///   var constraintResolver = new DefaultInlineConstraintResolver();
    ///   constraintResolver.ConstraintMap.Add("enum", typeof(EnumerationConstraint));
    ///   config.MapHttpAttributeRoutes(constraintResolver);
    /// </code>
    /// Then in your Route attribute you can use it like so; note the Enum type name must be fully
    /// namespace-qualified, and if the Enum is embedded in another class use the namespace-qualified
    /// class name of the parent object "+" the Enum name (ex. My.NameSpace.MyClass+ColorsEnum):
    /// <code>
    ///   [HttpGet, Route("colors/{color:enum(My.Namespace.ColorsEnum)}")]
    ///   public string GetColor(string color)
    ///   {
    ///      return color;
    ///   }
    /// </code>
    /// 
    /// If you want the method parameter to get the value as an enumeration, you can safely do so
    /// since the Web API engine will have already validated it; do note that you
    /// should enable string enums in the Swagger config file, or on the enum itself using
    /// [JsonConverter(typeof(StringEnumConverter))], if you are using Swagger to test your service.
    /// <code>
    ///   [HttpGet, Route("colors/{color:enum(My.Namespace.ColorsEnum)}")]
    ///   public string GetColor(ColorsEnum color)
    ///   {
    ///      return color.ToString();
    ///   }
    /// </code>
    /// </remarks>
    public class EnumerationConstraint : IHttpRouteConstraint

    {
        /// <summary>
        /// Holds the type of the Enum class to validate against
        /// </summary>
        public readonly Type EnumType;

        /// <summary>
        /// Constructor taking a namespace-qualified type name of the Enum type to use
        /// </summary>
        public EnumerationConstraint(string type)
        {
            var t = GetType(type);

            if (t == null || !t.IsEnum)
                throw new ArgumentException("Argument is not an Enum type", "type");
            EnumType = t;
        }

        /// <summary>
        /// Internal method to convert the string enum type name into a Type instance
        /// by checking all of the currently loaded assemblies
        /// </summary>
        private static Type GetType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null) return type;
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null)
                    return type;
            }
            return null;
        }

        /// <summary>
        /// IHttpRouteConstraint.Match implementation to validate a parameter against
        /// the Enum members.  String comparison is NOT case-sensitive.
        /// </summary>
        public bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName,
            IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            object value;

            if (values.TryGetValue(parameterName, out value) && value != null)
            {
                var stringVal = value as string;
                if (!String.IsNullOrEmpty(stringVal))
                {
                    // see if we can find the string in the enumeration members
                    stringVal = stringVal.ToLower();
                    if (null != EnumType.GetEnumNames().FirstOrDefault(a => a.ToLower().Equals(stringVal)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
