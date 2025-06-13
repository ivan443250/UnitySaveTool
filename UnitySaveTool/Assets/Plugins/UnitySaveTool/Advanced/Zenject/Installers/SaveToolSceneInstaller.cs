using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using Zenject;

namespace UnitySaveTool.Advanced
{
    public class SaveToolSceneInstaller : MonoInstaller
    {
        [Inject] ISaveToolBindInstaller _bindInstaller;

        private bool _completed = false;

        public override void InstallBindings()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            ZenjectDIContainer containerInterface = new ZenjectDIContainer(Container);

            InstallProviderAsync(sceneName, containerInterface).Forget();

            while (!_completed) { } //temporary solution
        }

        private async UniTask InstallProviderAsync(string sceneName, ZenjectDIContainer containerInterface)
        {
            await _bindInstaller.InstallDataProviderInSceneContextAsync(sceneName, containerInterface);
            _completed = true;
        }
    }
}