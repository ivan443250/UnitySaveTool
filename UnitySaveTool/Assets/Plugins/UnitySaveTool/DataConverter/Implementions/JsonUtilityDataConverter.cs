using System;
using UnityEngine;

namespace UnitySaveTool
{
    public class JsonUtilityDataConverter : IGenericDataConverter
    {
        public string ConvertFromObject(object obj)
        {
            return JsonUtility.ToJson(obj);
        }

        public object ConvertToObject(string objectSrting, Type objectType)
        {
            return JsonUtility.FromJson(objectSrting, objectType);
        }

        public T ConvertToObject<T>(string objectSrting) where T : class
        {
            return ConvertToObject(objectSrting, typeof(T)) as T;
        }
    }
}
