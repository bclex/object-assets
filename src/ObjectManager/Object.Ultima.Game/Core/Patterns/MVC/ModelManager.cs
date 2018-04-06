namespace OA.Ultima.Core.Patterns.MVC
{
    class ModelManager
    {
        AModel _model;
        AModel _queuedModel;

        public AModel Next
        {
            get { return _queuedModel; }
            set
            {
                if (_queuedModel != null)
                {
                    _queuedModel.Dispose();
                    _queuedModel = null;
                }
                _queuedModel = value;
                if (_queuedModel != null)
                    _queuedModel.Initialize();
            }
        }

        public AModel Current
        {
            get { return _model; }
            set
            {
                if (_model != null)
                {
                    _model.Dispose();
                    _model = null;
                }
                _model = value;
                if (_model != null)
                    _model.Initialize();
            }
        }

        public void ActivateNext()
        {
            if (_queuedModel != null)
            {
                Current = Next;
                _queuedModel = null;
            }
        }
    }
}
