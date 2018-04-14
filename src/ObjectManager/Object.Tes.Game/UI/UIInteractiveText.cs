using System;
using UnityEngine;
using UnityEngine.UI;

namespace OA.Tes.UI
{
    public class UIInteractiveText : MonoBehaviour
    {
        const int WidgetWidth = 200;
        const int WidgetHeight = 100;

        bool _opened;

        [SerializeField]
        GameObject _container = null;
        [SerializeField]
        Image _icon = null;
        [SerializeField]
        Text _title = null;

        [SerializeField]
        GameObject _inventoryInfos = null;
        [SerializeField]
        Text _weight = null;
        [SerializeField]
        Text _value = null;

        void Start()
        {
            _opened = false;
            _container.SetActive(false);
        }

        public void Show(Sprite icon, string prefixTitle, string title, string weight, string value)
        {
            _icon.enabled = icon != null;
            if (_icon.enabled)
                _icon.sprite = icon;
            _title.text = string.IsNullOrEmpty(prefixTitle) ? title : prefixTitle + title;
            var showInventoryInfos = !string.IsNullOrEmpty(weight) && !string.IsNullOrEmpty(value);
            _inventoryInfos.SetActive(showInventoryInfos);
            _container.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, showInventoryInfos ? WidgetHeight : WidgetHeight / 2.0f);
            if (showInventoryInfos)
            {
                _weight.text = "Weight: " + weight;
                _value.text = "Value: " + value;
            }
            _container.SetActive(true);
            _opened = true;
        }

        public void Close()
        {
            if (_opened)
            {
                _container.SetActive(false);
                _opened = false;
            }
        }
    }
}