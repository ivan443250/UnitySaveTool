using System;

namespace UnitySaveTool
{
    public interface IDataConverter
    {
        string ConvertFromObject(object obj);

        object ConvertToObject(string objectSrting, Type objectType);
    }
}
