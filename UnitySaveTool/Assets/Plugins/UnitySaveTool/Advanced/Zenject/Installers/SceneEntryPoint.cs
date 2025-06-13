using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace UnitySaveTool.Advanced
{
    [RequireComponent(typeof(SceneContext))]
    public class SceneEntryPoint : MonoBehaviour
    {
        private async void Awake()
        {
            SceneContext context = GetComponent<SceneContext>();
            DiContainer container = context.Container;

            string sceneName = SceneManager.GetActiveScene().name;
            ZenjectDIContainer containerInterface = new ZenjectDIContainer(container);

            ISaveToolBindInstaller bindInstaller = ProjectContext.Instance.Container.Resolve<ISaveToolBindInstaller>();
            await bindInstaller.InstallDataProviderInSceneContextAsync(sceneName, containerInterface);

            context.Run();
        }
    }
}