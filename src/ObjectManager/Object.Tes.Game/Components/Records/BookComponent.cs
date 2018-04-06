using OA.Bae.Esm;

namespace OA.Bae.Components.Records
{
    public class BookComponent : GenericObjectComponent
    {
        static PlayerComponent _player = null;
        static UIManager _uiManager = null;

        public static PlayerComponent Player
        {
            get
            {
                if (_player == null)
                    _player = FindObjectOfType<PlayerComponent>();
                return _player;
            }
        }

        public static UIManager UIManager
        {
            get
            {
                if (_uiManager == null)
                    _uiManager = FindObjectOfType<UIManager>();
                return _uiManager;
            }
        }

        void Start()
        {
            usable = true;
            pickable = false;
            var book = (BOOKRecord)record;
            objData.interactionPrefix = "Read ";
            objData.name = book.FNAM != null ? book.FNAM.value : book.NAME.value;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(BOOK.ITEX.value, "icons");
            objData.weight = book.BKDT.weight.ToString();
            objData.value = book.BKDT.value.ToString();
        }

        public override void Interact()
        {
            var book = (BOOKRecord)record;
            if (book.TEXT == null)
            {
                if (book.BKDT.scroll == 1) OnTakeScroll(book);
                else OnTakeBook(book);
            }
            if (book.BKDT.scroll == 1)
            {
                UIManager.Scroll.Show(book);
                UIManager.Scroll.OnClosed += OnCloseScroll;
                UIManager.Scroll.OnTake += OnTakeScroll;
            }
            else
            {
                UIManager.Book.Show(book);
                UIManager.Book.OnClosed += OnCloseBook;
                UIManager.Book.OnTake += OnTakeBook;
            }
            Player.Pause(true);
        }

        private void OnTakeScroll(BOOKRecord obj)
        {
            var inventory = FindObjectOfType<PlayerInventory>();
            inventory.Add(this);
        }

        private void OnCloseScroll(BOOKRecord obj)
        {
            UIManager.Scroll.OnClosed -= OnCloseScroll;
            UIManager.Scroll.OnTake -= OnTakeScroll;
            Player.Pause(false);
        }

        private void OnTakeBook(BOOKRecord obj)
        {
            var inventory = FindObjectOfType<PlayerInventory>();
            inventory.Add(this);
        }

        private void OnCloseBook(BOOKRecord obj)
        {
            UIManager.Book.OnClosed -= OnCloseBook;
            UIManager.Book.OnTake -= OnTakeBook;
            Player.Pause(false);
        }
    }
}