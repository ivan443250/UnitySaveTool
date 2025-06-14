using UnityEngine.SceneManagement;
using Zenject;

namespace UnitySaveTool.Advanced
{
    public class SaveToolSceneInstaller : MonoInstaller
    {
        [Inject] ISaveToolBindInstaller _bindInstaller;

        public override void InstallBindings()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            ZenjectDIContainer containerInterface = new ZenjectDIContainer(Container);

            _bindInstaller.InstallDataProviderInSceneContext(sceneName, containerInterface);
        }
    }
}