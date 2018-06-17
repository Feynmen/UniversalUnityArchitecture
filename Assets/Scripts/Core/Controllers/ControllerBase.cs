using Core.Atributes;
using Core.Interfaces;
using UnityEngine;

namespace Core.Controllers
{
    public abstract class ControllerBase : MonoBehaviorWraper, IInitializable, IReleaseble
    {
        [Inject(typeof(SceneControllerBase),false)]
        protected SceneControllerBase SceneController { private set; get; }

        protected T GetSceneControler<T>() where T : SceneControllerBase
        {
            return SceneController as T;
        }

        protected abstract void Init();

        protected virtual void Release() { }

        void IInitializable.Init()
        {
            Init();
        }

        void IReleaseble.Release()
        {
            Release();
        }
    }
}
