using OA.Ultima.Data;
using OA.Ultima.World.EntityViews;

namespace OA.Ultima.World.Entities
{
    public class Overhead : AEntity
    {
        public AEntity Parent { get; private set; }

        public MessageTypes MessageType { get; private set; }

        public string Text { get; private set; }

        int _timePersist;

        public Overhead(AEntity parent, MessageTypes msgType, string text)
            : base(parent.Serial, parent.Map)
        {
            Parent = parent;
            MessageType = msgType;
            Text = text;
            var plainText = text.Substring(text.IndexOf('>') + 1);
            // Every speech message lasts at least 2.5s, and increases by 100ms for every char, to a max of 10s
            _timePersist = 2500 + (plainText.Length * 100);
            if (_timePersist > 10000)
                _timePersist = 10000;
        }

        public void ResetTimer()
        {
            _timePersist = 5000;
        }

        public override void Update(double frameMS)
        {
            base.Update(frameMS);
            _timePersist -= (int)frameMS;
            if (_timePersist <= 0)
                Dispose();
        }

        // ============================================================================================================
        // View management
        // ============================================================================================================

        protected override AEntityView CreateView()
        {
            return new OverheadView(this);
        }
    }
}
