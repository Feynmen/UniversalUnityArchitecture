using Core.Controllers;

namespace Core.Services
{
    public interface IUpdaterService : IService
    {
        void Update(SceneControllerBase sceneController);
    }
}
