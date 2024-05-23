using System;
using System.Collections;
using System.Collections.Generic;
using CRA.DataMigration.DAL.Entities.BigQuery;
using CRA.DataMigration.DAL.Extensions;
using Google.Cloud.BigQuery.V2;

namespace CRA.DataMigration.DAL.Helpers
{
    public static class BigQueryHelper
    {
        public static BigQueryInsertRow ConvertToInsertRow<T>(T model) where T : IBigQueryEntity
        {
            var insertRow = new BigQueryInsertRow(model.GetId());

            var properties = model.GetType().GetPublicFilteredProperties();
            foreach (var propertyInfo in properties)
            {
                if (IsSimpleType(propertyInfo.PropertyType))
                {
                    var key = propertyInfo.Name.ToCamelCase();
                    var value = propertyInfo.GetValue(model, null);

                    insertRow.Add(key, value);
                    continue;
                }

                if (!IsEnumerableType(propertyInfo.PropertyType))
                {
                    continue;
                }

                var collection = propertyInfo.GetValue(model, null) as IEnumerable;
                if (collection == null)
                {
                    continue;
                }

                var nestedRows = new List<BigQueryInsertRow>();

                foreach (var element in collection)
                {
                    var nestedRow = new BigQueryInsertRow();

                    var elementProperties = element.GetType().GetPublicFilteredProperties();

                    foreach (var elementProperty in elementProperties)
                    {
                        var key = elementProperty.Name.ToCamelCase();
                        var value = elementProperty.GetValue(element, null);

                        nestedRow.Add(key, value);
                    }

                    nestedRows.Add(nestedRow);
                }

                insertRow.Add(propertyInfo.Name.ToCamelCase(), nestedRows);
            }

            return insertRow;
        }

        private static bool IsSimpleType(Type type)
        {
            return type.IsPrimitive
                   || type.IsEnum
                   || type == typeof(DateTime)
                   || type == typeof(string)
                   || type == typeof(decimal);
        }

        private static bool IsEnumerableType(Type type)
        {
            return type.GetInterface(nameof(IEnumerable)) != null;
        }
    }
}