using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace UnitySaveTool
{
    public class SaveCellDataContext : DataContext, ISaveCellContext
    {
        public ISceneDataContext OpenedSceneContext => _cachedSceneContext;

        protected override string ContextName => nameof(SaveCellDataContext);

        protected override SaveContext ContextType => SaveContext.SaveCell;

        private int _cellIndex;

        private string _cachedSceneName = string.Empty;
        private DataContext _cachedSceneContext;

        public SaveCellDataContext(Dictionary<Type, object> contextData,
            IFileSystem fileSystem,
            IAsyncFileSystem asyncFileSystem,
            int cellIndex)
            : base(contextData, fileSystem, asyncFileSystem, cellIndex.ToString())
        {
            _cellIndex = cellIndex;
        }

        public ISceneDataContext OpenScene(string sceneName)
        {
            return OpenSceneInternal(sceneName, false).GetAwaiter().GetResult();
        }

        public async UniTask<ISceneDataContext> OpenSceneAsync(string sceneName)
        {
            return await OpenSceneInternal(sceneName, true);
        }

        private async UniTask<ISceneDataContext> OpenSceneInternal(string sceneName, bool doAsync)
        {
            if (sceneName == _cachedSceneName)
                return _cachedSceneContext;

            if (_cachedSceneContext != null)
            {
                if (doAsync)
                    await _cachedSceneContext.SaveAllAsync();
                else
                    _cachedSceneContext.SaveAll();
            }

            Dictionary<Type, object> sceneData = doAsync ? 
                await AsyncFileSystem.LoadAllAsync(_cellIndex.ToString(), sceneName) 
                : FileSystem.LoadAll(_cellIndex.ToString(), sceneName);

            _cachedSceneContext = new SceneDataContext(sceneData, FileSystem, AsyncFileSystem, _cellIndex.ToString(), sceneName);
            _cachedSceneName = sceneName;

            return _cachedSceneContext;
        }
    }
}
