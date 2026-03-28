using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BarbeariaRocha.Configurations
{
    public class EnumSchemaFilterConfig : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                var enumValues = schema.Enum.ToArray();
                var i = 0;
                schema.Enum.Clear();
                foreach (var value in Enum.GetNames(context.Type))
                {
                    schema.Enum.Add(new OpenApiString(value + $" = {((OpenApiPrimitive<int>)enumValues[i]).Value}"));
                    i++;
                }
            }
        }
    }
}
