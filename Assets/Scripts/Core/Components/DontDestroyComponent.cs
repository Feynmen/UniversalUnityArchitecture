using UnityEngine;

namespace Core.Components
{
    public class DontDestroyComponent : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
