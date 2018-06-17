using Core.Controllers;
using UnityEngine;

namespace Controllers.TestScene
{
	public class AnotherTestController : ControllerBase
	{
		protected override void Init()
		{
			Disable();
		}

		public override void Enable()
		{
			base.Enable();
			Debug.Log("AnotherTestController Enabled");
		}

		public override void Disable()
		{
			base.Disable();
			Debug.Log("AnotherTestController Disabled");
		}
	}
}
