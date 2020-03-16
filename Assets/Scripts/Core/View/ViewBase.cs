using Core.Interfaces;

namespace Core.View
{
    public abstract class ViewBase : MonoBehaviorWrapper, IInitializable, IReleaseble
    {
        public abstract void Init();

        public virtual void Release() { }

        private void OnDestroy()
        {
            Release();
        }
    }
}
