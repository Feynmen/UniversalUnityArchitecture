using System;
using System.Collections;

namespace Core.Controllers
{
	public abstract class StateMachineSceneControllerBase : SceneControllerBase 
	{
		protected StateControllerBase CurrentState { private set; get; }
		private IEnumerator _changeStateRoutine;
		
		public void ActiveState<T>() where T : StateControllerBase
		{
			if (_changeStateRoutine != null)
			{
				throw new Exception("Can't change state because change operation in progress " + CurrentState.ToString());
			}
			var state = Get<T>();

			ActiveState(state);
		}

		public void ActiveState(StateControllerBase state)
		{
			_changeStateRoutine = ChangeState(state);
			StartCoroutine(_changeStateRoutine);
		}
		
		private IEnumerator ChangeState(StateControllerBase state)
		{
			if (CurrentState != null)
			{
				yield return CurrentState.ExitState();
			}
			CurrentState = state;
			_changeStateRoutine = null;
			yield return state.EnterState();
			state.StartProcessing();
			yield return null;
		}
	}
}
