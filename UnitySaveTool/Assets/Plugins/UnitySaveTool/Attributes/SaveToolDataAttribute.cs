using System;

namespace UnitySaveTool
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class SaveToolDataAttribute : Attribute 
    {
        public SaveContext Context => _saveContext;
        private SaveContext _saveContext;

        public SaveToolDataAttribute(SaveContext saveContext) 
        {
            _saveContext = saveContext;
        }
    }

    public enum SaveContext
    {
        Scene,
        SaveCell,
        Global
    }
}

