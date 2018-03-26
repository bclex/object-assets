using System;

namespace OA.Ultima.Core.Patterns.MVC
{
    /// <summary>
    /// Abstract Model. Maintains the state, core data, and update logic of a model.
    /// </summary>
    public abstract class AModel
    {
        bool _isInitialized;
        AView _view;
        AController _controller;

        public AView GetView()
        {
            if (_view == null)
                _view = CreateView();
            return _view;
        }
        
        public AController GetController()
        {
            if (_controller == null)
                _controller = CreateController();
            return _controller;
        }

        public void Initialize()
        {
            if (_isInitialized)
                return;
            _isInitialized = true;
            OnInitialize();
        }

        public void Dispose()
        {
            OnDispose();
        }

        public abstract void Update(double totalTime, double frameTime);

        protected abstract AView CreateView();
        protected abstract void OnInitialize();
        protected abstract void OnDispose();

        protected virtual AController CreateController()
        {
            throw new NotImplementedException();
        }
    }
}
