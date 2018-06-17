using UnityEngine;

namespace Core.Components
{
	public class LookAtCamera : MonoBehaviour
	{
		private Transform _transform;
		private Transform _cameraTransform;

		private void Awake()
		{
			var curentCamera = Camera.current;
			if (curentCamera == null)
			{
				curentCamera = Camera.main;
			}
			_cameraTransform = curentCamera.transform;
			_transform = transform;
		}

		private void LateUpdate()
		{
			_transform.LookAt(_cameraTransform.position);
		}
	}
}
