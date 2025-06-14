using System;
using System.Collections.Generic;

namespace UnitySaveTool
{
    public class SceneDataContext : DataContext
    {
        public SceneDataContext(Dictionary<Type, object> contextData,
            IFileSystem fileSystem, 
            IAsyncFileSystem asyncFileSystem, 
            params string[] folders) 
            : base(contextData, fileSystem, asyncFileSystem, folders)
        {
        }

        protected override string ContextName => nameof(SceneDataContext);

        protected override SaveContext ContextType => SaveContext.Scene;
    }
}
