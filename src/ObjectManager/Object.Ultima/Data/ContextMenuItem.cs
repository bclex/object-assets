using OA.Core;
using OA.Ultima.Resources;

namespace OA.Ultima.Data
{
    public class ContextMenuItem
    {
        readonly string _caption;
        readonly int _responseCode;

        public ContextMenuItem(int responseCode, int stringID, int flags, int hue)
        {
            // get the resource provider
            var provider = Service.Get<IResourceProvider>();
            _caption = provider.GetString(stringID);
            _responseCode = responseCode;
        }

        public int ResponseCode => _responseCode;

        public string Caption => _caption;

        public override string ToString()
        {
            return string.Format("{0} [{1}]", _caption, _responseCode);
        }
    }
}
