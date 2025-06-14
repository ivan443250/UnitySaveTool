using Cysharp.Threading.Tasks;
using System;

namespace UnitySaveTool
{
    public interface ISaveToolBindInstaller
    {
        void InstallDataProviderInSceneContext(string sceneName, IDIContainer sceneContext);
        UniTask InstallDataProviderInSceneContextAsync(string sceneName, IDIContainer sceneContext);
    }
}
