using OA.Core;
using OA.Tes.FilePacks.Records;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace OA.Tes.UI
{
    public class UIBook : MonoBehaviour
    {
        int _numberOfPages;
        int _cursor;
        string[] _pages;
        BOOKRecord _bookRecord;

        [SerializeField]
        int _numCharPerPage = 565;

        [SerializeField]
        GameObject _container = null;
        [SerializeField]
        Image _background = null;
        [SerializeField]
        Text _page1 = null;
        [SerializeField]
        Text _page2 = null;
        [SerializeField]
        Text _numPage1 = null;
        [SerializeField]
        Text _numPage2 = null;
        [SerializeField]
        Button _nextButton = null;
        [SerializeField]
        Button _previousButton = null;

        public event Action<BOOKRecord> OnTake = null;
        public event Action<BOOKRecord> OnClosed = null;

        void Start()
        {
            var texture = TesEngine.Instance.Asset.LoadTexture("tx_menubook", true);
            _background.sprite = GUIUtils.CreateSprite(texture);
            // If the book is already opened, don't change its transform.
            if (_bookRecord == null)
                Close();
        }

        void Update()
        {
            if (!_container.activeSelf)
                return;
            if (InputManager.GetButtonDown("Use")) Take();
            else if (InputManager.GetButtonDown("Menu")) Close();
        }

        public void Show(BOOKRecord book)
        {
            _bookRecord = book;
            var words = _bookRecord.TEXT.value;
            words = words.Replace("<BR>", "\n");
            words = words.Replace("<BR><BR>", "\n");
            words = System.Text.RegularExpressions.Regex.Replace(words, @"<[^>]*>", string.Empty);
            var countChar = 0;
            var j = 0;
            for (var i = 0; i < words.Length; i++)
                if (words[i] != '\n')
                    countChar++;
            // Ceil returns the bad value... 16.6 returns 16..
            _numberOfPages = Mathf.CeilToInt(countChar / _numCharPerPage) + 1;
            _pages = new string[_numberOfPages];
            for (var i = 0; i < countChar; i++)
            {
                if (i % _numCharPerPage == 0 && i > 0)
                {
                    _pages[j] = _pages[j].TrimEnd('\n');
                    j++;
                }
                if (_pages[j] == null)
                    _pages[j] = String.Empty;
                _pages[j] += words[i];
            }
            _cursor = 0;
            UpdateBook();
            StartCoroutine(SetBookActive(true));
        }

        private void UpdateBook()
        {
            if (_numberOfPages > 1)
            {
                _page1.text = _pages[_cursor];
                _page2.text = _cursor + 1 >= _numberOfPages ? "" : _pages[_cursor + 1];
            }
            else
            {
                _page1.text = _pages[0];
                _page2.text = string.Empty;
            }
            _nextButton.interactable = _cursor + 2 < _numberOfPages;
            _previousButton.interactable = _cursor - 2 >= 0;
            if (_cursor + 2 < _numberOfPages && _pages[_cursor + 2] == string.Empty)
                _nextButton.interactable = false;
            _numPage1.text = (_cursor + 1).ToString();
            _numPage2.text = (_cursor + 2).ToString();
        }

        public void Take()
        {
            if (OnTake != null)
                OnTake(_bookRecord);
            Close();
        }

        public void Next()
        {
            if (_cursor + 2 >= _numberOfPages)
                return;
            if (_pages[_cursor + 2] == string.Empty)
                return;
            _cursor += 2;
            UpdateBook();
        }

        public void Previous()
        {
            if (_cursor - 2 < 0)
                return;
            _cursor -= 2;
            UpdateBook();
        }

        public void Close()
        {
            _container.SetActive(false);
            if (OnClosed != null)
                OnClosed(_bookRecord);
            _bookRecord = null;
        }

        private IEnumerator SetBookActive(bool active)
        {
            yield return new WaitForEndOfFrame();
            _container.SetActive(active);
        }
    }
}
