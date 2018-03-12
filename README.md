# enumeration-constraint
ASP.NET Web Api 2.x Route parameter constraint to limit a parameter to the named members of an enumeration class.

## Registering the Constraint
You will need to register the constraint in your WebApiConfig.cs file:
       
       var constraintResolver = new DefaultInlineConstraintResolver();
       constraintResolver.ConstraintMap.Add("enum", typeof(EnumerationConstraint));
       config.MapHttpAttributeRoutes(constraintResolver);

## Using the Constraint
In the Route attribute on your ApiController action method, use it like any other parameter constraint.
The constraint requires one string parameter, the fully namespace-qualified type of the Enum
you want to validate against.

If the Enum is embedded in another class use the namespace-qualified class name of the parent
object "+" the Enum name (ex. My.NameSpace.MyParentClass+ColorsEnum):
    
       [HttpGet, Route("colors/{color:enum(My.Namespace.ColorsEnum)}")]
       public string GetColor(string color)
       {
          return color;
       }

The constrained parameter is validated against the list of member names in the enumeration,
where the compare is NOT case-sensitive.
