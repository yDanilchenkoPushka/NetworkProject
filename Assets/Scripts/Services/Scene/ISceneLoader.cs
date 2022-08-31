namespace Services.Scene
{
    public interface ISceneLoader : IService
    {
        void Load(string sceneName);
        void LoadAdditive(string sceneName);
    }
}