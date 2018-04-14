using UnityEngine;

namespace OA.Tes.Components
{
    public class DayNightCycle : MonoBehaviour
    {
        Transform _transform = null;
        Quaternion _originalOrientation;

        [SerializeField]
        float _rotationTime = 0.5f;

        private void Start()
        {
            _transform = transform;
            _originalOrientation = _transform.rotation;
            RenderSettings.sun = GetComponent<Light>();
        }

        private void Update()
        {
            _transform.Rotate(_rotationTime * Time.deltaTime, 0.0f, 0.0f);
        }
    }
}
