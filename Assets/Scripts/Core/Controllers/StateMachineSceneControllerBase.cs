using System;
using System.Collections;

namespace Core.Controllers
{
	public abstract class StateMachineSceneControllerBase : SceneControllerBase 
	{
		protected StateControllerBase CurentState { private set; get; }
		private IEnumerator _changeStateRoutine;
		
		public void ActiveState<T>() where T : StateControllerBase
		{
			if (_changeStateRoutine != null)
			{
				throw new Exception("Can't change state because change operation in progress " + CurentState.ToString());
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
			if (CurentState != null)
			{
				yield return CurentState.ExitState();
			}
			CurentState = state;
			_changeStateRoutine = null;
			yield return state.EnterState();
			state.StartProcessing();
			yield return null;
		}
	}
}
