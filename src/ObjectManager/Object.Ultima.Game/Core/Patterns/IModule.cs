using OA.Core;

namespace OA.Ultima.Core.Patterns
{
    public interface IModule
    {
        string Name { get; }
        void Load();
        void Unload();
    }

    public abstract class Module : IModule
    {
        public virtual string Name
        {
            get { return GetType().Name; }
        }

        public void Load()
        {
            Utils.Info("Loading Module {0}.", Name);
            OnLoad();
        }

        public void Unload()
        {
            Utils.Info("Unloading Module {0}.", Name);
            OnUnload();
        }

        protected abstract void OnLoad();

        protected abstract void OnUnload();
    }
}