using Core.Controllers;

namespace Core.Services
{
    public interface IService
    {
        void Register(SceneControllerBase sceneController);
        void Unregister(SceneControllerBase sceneController);
    }
}