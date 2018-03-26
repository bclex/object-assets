using OA.Ultima.Core.Configuration;
using OA.Ultima.Data;

namespace OA.Ultima.Configuration
{
    public sealed class UltimaOnlineSettings : ASettingsSection
    {
        bool _allowCornerMovement;
        string _dataDirectory;
        byte[] _clientVersion;

        public UltimaOnlineSettings()
        {
            PatchVersion = ClientVersion.DefaultVersion;
        }

        /// <summary>
        /// The patch version which is sent to the server. RunUO (and possibly other server software) rely on the
        /// client's reported patch version to enable/disable certain packets and features.
        /// </summary>
        public byte[] PatchVersion
        {
            get {
                if (_clientVersion == null || _clientVersion.Length != 4)
                    return ClientVersion.DefaultVersion;
                return _clientVersion;
            }
            set
            {
                if (value == null || value.Length != 4)
                    return;
                // Note from ZaneDubya: I will not support your client if you change or remove this line:
                if (!ClientVersion.EqualTo(value, ClientVersion.DefaultVersion)) return;
                SetProperty(ref _clientVersion, value);
            }
        }
        
        /// <summary>
        /// The directory where the Ultima Online resource files and executable are located.
        /// </summary>
        public string DataDirectory
        {
            get { return _dataDirectory; }
            set { SetProperty(ref _dataDirectory, value); }
        }

        /// <summary>
        /// When true, allows corner-cutting movement (ala the God client and RunUO administrator-mode movement).
        /// </summary>
        public bool AllowCornerMovement
        {
            get { return _allowCornerMovement; }
            set { SetProperty(ref _allowCornerMovement, value); }
        }
    }
}