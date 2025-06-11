using System;
using System.Collections.Generic;

namespace UnitySaveTool.Test
{
    public class GlobalData
    {
        private Dictionary<Type, object> _objectContainer;

        private static GlobalData _instance;

        static GlobalData()
        {
            _instance = new GlobalData();
            AddDefaultBindings();
        }

        private GlobalData()
        {
            _objectContainer = new();
        }

        private static void AddDefaultBindings()
        {
            _instance._objectContainer.Add(typeof(IDataConverter), new JsonUtilityDataConverter());
            _instance._objectContainer.Add(typeof(IFileSystem), new FileSystem(Resolve<IDataConverter>()));
            _instance._objectContainer.Add(typeof(IDataExplorer), new DataExplorer(Resolve<IFileSystem>()));
        }

        public static T Resolve<T>() where T : class
        {
            if (_instance._objectContainer.ContainsKey(typeof(T)) == false)
                return null;

            return _instance._objectContainer[typeof(T)] as T;
        }
    }
}
