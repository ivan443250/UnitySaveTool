using Cysharp.Threading.Tasks;
using System;

namespace UnitySaveTool
{
    public interface ISaveToolBindInstaller
    {
        UniTask InstallDataProviderInSceneContextAsync(string sceneName, IDIContainer sceneContext);
    }
}
