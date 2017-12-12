using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class MappingHelper
{
    public static void MapObjects<TFromObjType, TToObjType>(TFromObjType fromObject, TToObjType toObject)
    {
        var fromObjectProperties = fromObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var toObjectProperties = toObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var fromObjectPropertyInfo in fromObjectProperties)
        {
            var toObjectPropertyInfo = toObjectProperties.FirstOrDefault(x => x.Name == fromObjectPropertyInfo.Name);

            if (toObjectPropertyInfo == null)
            {
                continue;
            }

            if (toObjectPropertyInfo.PropertyType != typeof(string) && toObjectPropertyInfo.PropertyType.GetInterfaces().Any(x => x == typeof(IEnumerable)))
            {
                var fromObjectCollection = fromObjectPropertyInfo.GetValue(fromObject) as IEnumerable;

                if (fromObjectCollection == null)
                {
                    continue;
                }

                dynamic toObjectCollection = toObjectPropertyInfo.GetValue(toObject) as ICollection;
                Type toObjectType = GetGenericType(toObjectCollection);

                var collection = toObjectPropertyInfo.GetValue(toObject) as IList;

                foreach (var fromCollObj in fromObjectCollection)
                {
                    var toCollObj = Activator.CreateInstance(toObjectType);
                    MapObjects(fromCollObj, toCollObj);
                    collection?.Add(toCollObj);
                }

                toObjectPropertyInfo.PropertyType.GetMethod("AddRange")?.Invoke(toObject, new object[] {collection});
            }
            else if (toObjectPropertyInfo.CanWrite)
            {
                toObjectPropertyInfo.SetValue(toObject, fromObjectPropertyInfo.GetValue(fromObject));
            }
        }
    }

    public static Type GetGenericType<T>(ICollection<T> collection)
    {
        return typeof(T);
    }
}
