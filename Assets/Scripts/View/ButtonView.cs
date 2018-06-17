using Core.View;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace View
{
	public class ButtonView : ViewBase
	{
		[SerializeField] private Button _button;
	
		public override void Init() { }

		public void AddButtonOnClickListener(UnityAction action)
		{
			_button.onClick.AddListener(action);
		}
	}
}
