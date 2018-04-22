using UnityEngine;
using UnityEngine.UI;

namespace OA.Tes.UI
{
    [RequireComponent(typeof(Image))]
    public class UICrosshair : MonoBehaviour
    {
        Image _crosshair = null;

        public bool Enabled
        {
            get { return _crosshair.enabled; }
            set { _crosshair.enabled = value; }
        }

        private void Awake()
        {
            _crosshair = GetComponent<Image>();
        }

        private void Start()
        {
            var crosshairTexture = TesEngine.instance._asset.LoadTexture("target", true);
            _crosshair.sprite = GUIUtils.CreateSprite(crosshairTexture);
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}
