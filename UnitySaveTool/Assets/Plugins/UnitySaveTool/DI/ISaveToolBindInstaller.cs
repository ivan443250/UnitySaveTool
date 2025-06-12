using Cysharp.Threading.Tasks;
using System;

namespace UnitySaveTool
{
    public interface ISaveToolBindInstaller
    {
        UniTask InstallDataProviderInSceneContext(string sceneName, IDIContainer sceneContext);
    }
}
