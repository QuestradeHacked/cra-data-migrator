using System;
using System.Linq;
using System.Reflection;
using CRA.DataMigration.DAL.Attributes;

namespace CRA.DataMigration.DAL.Extensions
{
    public static class TypeExtensions
    {
        public static PropertyInfo[] GetPublicFilteredProperties(this Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(pi => !pi.GetCustomAttributes(typeof(SkipPropertyAttribute), true).Any()).ToArray();
        }
    }
}