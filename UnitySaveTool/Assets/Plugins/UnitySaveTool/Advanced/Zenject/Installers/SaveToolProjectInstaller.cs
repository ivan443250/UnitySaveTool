using UnityEngine;
using Zenject;

namespace UnitySaveTool.Advanced
{
    public class SaveToolProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .Bind<IDataConverter>()
                .To<JsonUtilityDataConverter>()
                .AsSingle();

            Container
                .Bind<IFileSystem>()
                .To<FileSystem>()
                .AsSingle();

            Container
                .Bind<IDataExplorer>()
                .To<DataExplorer>()
                .AsSingle();

            Container
                .Bind<IDIContainer>()
                .FromInstance(new ZenjectDIContainer(Container))
                .WhenInjectedInto<SaveToolBindInstaller>();

            Container
                .Bind<ISaveToolBindInstaller>()
                .To<SaveToolBindInstaller>()
                .AsSingle();

            Debug.Log("SaveToolProjectInstaller");
        }
    }
}