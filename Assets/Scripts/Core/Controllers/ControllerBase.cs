using Core.Atributes;
using Core.Interfaces;
using UnityEngine;

namespace Core.Controllers
{
    public abstract class ControllerBase : MonoBehaviorWrapper, IInitializable, IReleaseble
    {
        [Inject(typeof(SceneControllerBase),false)]
        protected SceneControllerBase SceneController { set; get; }

        protected T GetSceneController<T>() where T : SceneControllerBase
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
