using Microsoft.Extensions.Configuration;

namespace CRA.DataMigration.Core.Extensions
{
    public static class ConfigurationExtensions
    {
        public static TModel GetConfiguration<TModel>(this IConfiguration configuration, string section = null)
            where TModel : new()
        {
            var model = new TModel();

            var sectionName = section ?? model.GetType().Name;

            configuration.GetSection(sectionName).Bind(model);

            return model;
        }
    }
}