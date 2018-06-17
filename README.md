# UniversalUnityArchitecture
Core logic for Unity projects. 
## How to start
1) Import into your Unity projects:

1.1) Resources (prefab Debug_ApplicationSystem need for work in editor)

1.2) Scripts

2) Create Scene Controllers for your scenes. For example:
```C#
namespace Controllers.TestScene
{
	public class TestSceneController : SceneControllerBase 
	{
		public override IEnumerator Init()
		{
      //Put here your Init scene logic
			throw new System.NotImplementedException();
		}
	}
}
```
3) Add a gameobject into your target scene and add to the gameobject the script from point 2
4) If you need create Controller. For example:
```C#
namespace Controllers.TestScene
{
	public class TestController :  ControllerBase
	{
		protected override void Init()
		{
			//Put here your controller logic
			throw new System.NotImplementedException();
		}
	}
}
```
5) Put controller on the scene 
6) If you need, create View. For example:
```C#
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
```
7) Add your View to the scene and conect it to the controller, and init. For Example:
```C#
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
				Debug.Log("Click");
			});
		}
	}
}
```
8) All classes which inherit ControllerBase, have access to curent SceneController. Using this link, you can get another Controller. For Example:
```C#
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
				SceneController.Get<AnotherTestController>().Enable();
			});   
      
		}
	}
}
```
9) You can cathc moment when your controller Enabling or Disabling. For example:
```C#
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
```
## Another features(like Managers) and logic of workflow
You can read this in Wiki
## Summary
I applied this logic to several projects of different directions and it successfully demonstrated itself as a universal and enduring approach to the work of a large team on a project

If you have suggestions for improving this logic then please contact me, I'm open for discussions
