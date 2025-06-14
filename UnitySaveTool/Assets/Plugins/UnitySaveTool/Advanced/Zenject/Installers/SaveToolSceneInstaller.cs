using UnityEngine.SceneManagement;
using Zenject;

namespace UnitySaveTool.Advanced
{
    public class SaveToolSceneInstaller : MonoInstaller
    {
        [Inject] ISaveToolBindInstaller _bindInstaller;
        [Inject] IDataExplorer _explorer;

        public override void InstallBindings()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            ZenjectDIContainer containerInterface = new ZenjectDIContainer(Container);

            _bindInstaller.InstallDataProviderInSceneContext(sceneName, containerInterface);
        }

        private void OnDestroy()
        {
            _explorer.SceneDataSet.SaveAll();
        }
    }
}