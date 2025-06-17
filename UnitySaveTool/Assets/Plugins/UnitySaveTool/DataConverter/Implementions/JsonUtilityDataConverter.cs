using System;
using UnityEngine;

namespace UnitySaveTool
{
    public class JsonUtilityDataConverter : IGenericDataConverter
    {
        public string ConvertFromObject(object obj)
        {
            if (obj is IBeforeConvertationCallbackReciever reciever)
                reciever.OnBeforeConvertation();

            return JsonUtility.ToJson(obj, true);
        }

        public object ConvertToObject(string objectSrting, Type objectType)
        {
            object obj = JsonUtility.FromJson(objectSrting, objectType);

            if (obj is IAfterConvertationCallbackReciever reciever)
                reciever.OnAfterConvertation();

            return obj;
        }

        public T ConvertToObject<T>(string objectSrting) where T : class
        {
            return ConvertToObject(objectSrting, typeof(T)) as T;
        }
    }
}
