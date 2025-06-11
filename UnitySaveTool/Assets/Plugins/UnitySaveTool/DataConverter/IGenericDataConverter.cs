namespace UnitySaveTool
{
    public interface IGenericDataConverter : IDataConverter
    {
        T ConvertToObject<T>(string objectSrting) where T : class;
    }
}
