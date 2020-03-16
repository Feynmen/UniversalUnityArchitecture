using System.Collections;

namespace Core.Controllers
{
	public abstract class StateControllerBase : ControllerBase
	{
		protected override void Init()
		{
			base.Disable();
		}

		public virtual IEnumerator EnterState()
		{
			base.Enable();
			yield return null;
		}

		public virtual IEnumerator ExitState()
		{
			base.Disable();
			yield return null;
		}
		
		public void StartProcessing()
		{
			OnStartProcessing();
		}

		public sealed override void Enable() { }

		public sealed override void Disable() { }

		protected abstract void OnStartProcessing();

		protected void ActiveState<T>() where T : StateControllerBase
		{
			GetSceneController<StateMachineSceneControllerBase>().ActiveState<T>();
		}
	}
}
