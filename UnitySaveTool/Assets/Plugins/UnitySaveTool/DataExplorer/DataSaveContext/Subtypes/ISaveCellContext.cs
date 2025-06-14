using Cysharp.Threading.Tasks;

namespace UnitySaveTool
{
    public interface ISaveCellContext : ISceneDataContext
    {
        ISceneDataContext OpenScene(string sceneName);
        UniTask<ISceneDataContext> OpenSceneAsync(string sceneName);

        ISceneDataContext OpenedSceneContext { get; }
    }
}
