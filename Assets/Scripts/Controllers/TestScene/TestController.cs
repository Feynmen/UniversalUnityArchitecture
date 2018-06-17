using Core.Controllers;
using Helpers;
using UnityEngine;
using View;

namespace Controllers.TestScene
{
	public class TestController : ControllerBase
	{
		[SerializeField] private ButtonView _buttonView;
		
		protected override void Init()
		{
			_buttonView.Init();
			_buttonView.AddButtonOnClickListener(() =>
			{
				Disable();
				SceneController.Enable<AnotherTestController>();
			});
		}
	}
}
