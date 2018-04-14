//using UnityEngine;
//using UnityEngine.Rendering.PostProcessing;
//using UnityEngine.XR;

//namespace OA.XR
//{
//    public sealed class XRVignetteOverlay : MonoBehaviour
//    {
//        Vignette _vignette = null;
//        Rigidbody _rigidbody = null;

//        [SerializeField]
//        float _threshold = 0.5f;

//        private void Start()
//        {
//            if (!XRSettings.enabled)
//                Destroy(this);
//            var volume = FindObjectOfType<PostProcessVolume>();
//            var profile = volume.profile;
//            profile.TryGetSettings(out _vignette);
//            if (_vignette == null)
//                Destroy(this);
//            _vignette.enabled.value = false;
//            _vignette.intensity.value = 1.5f;
//            _rigidbody = GetComponent<Rigidbody>();
//        }

//        private void Update()
//        {
//            var show = _rigidbody.velocity.sqrMagnitude > _threshold || _rigidbody.angularVelocity.sqrMagnitude > _threshold;
//            if (show && !_vignette.enabled.value)
//                _vignette.enabled.value = true;
//            else if (!show && _vignette.enabled.value)
//                _vignette.enabled.value = false;
//        }
//    }
//}