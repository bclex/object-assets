using OA.UI;
using UnityEngine;

namespace OA.Tes.UI
{
    //[RequireComponent(typeof(Canvas))]
    public class TesUIManager : UIManager
    {
        [Header("UI Elements")]
        [SerializeField]
        UIBook _book = null;
        public UIBook Book
        {
            get { return _book; }
        }
        [SerializeField]
        UIScroll _scroll = null;
        public UIScroll Scroll
        {
            get { return _scroll; }
        }
    }
}
