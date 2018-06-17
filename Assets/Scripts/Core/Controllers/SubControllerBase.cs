using Core;
using Core.Interfaces;

namespace Core.Controllers
{
    public abstract class SubControllerBase : MonoBehaviorWraper, IInitializable, IReleaseble
    {
        public abstract void Init();

        public virtual void Release() { }
    }
}
