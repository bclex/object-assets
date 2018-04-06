using OA.Ultima.World.Maps;

namespace OA.Ultima.World.Entities.Items
{
    public class BaseBook : Item
    {
        string _title;
        string _author;
        BookPageInfo[] _pages;
        bool _isEditable;

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public string Author
        {
            get { return _author; }
            set { _author = value; }
        }

        public bool IsEditable
        {
            get { return _isEditable; }
            set { _isEditable = value; }
        }

        public int PageCount => _pages.Length;

        public BookPageInfo[] Pages
        {
            get { return _pages; }
            set
            {
                _pages = value;
                _onUpdated?.Invoke(this);
            }
        }

        public BaseBook(Serial serial, Map map) 
            : this(serial, map, true) { }
        public BaseBook(Serial serial, Map map, bool writable)
            : this(serial, map, writable, null, null) { }
        public BaseBook(Serial serial, Map map, bool writable, string title, string author)
            : base(serial, map)
        {
            _title = title;
            _author = author;
            _isEditable = writable;
            _pages = new BookPageInfo[0];
        }

        public class BookPageInfo
        {
            string[] _lines;
            public string[] Lines
            {
                get { return _lines; }
                set { _lines = value; }
            }

            public BookPageInfo()
            {
                _lines = new string[0];
            }

            public BookPageInfo(string[] lines)
            {
                _lines = lines;
            }

            public string GetAllLines() => string.Join("\n", _lines);
        }
    }
}