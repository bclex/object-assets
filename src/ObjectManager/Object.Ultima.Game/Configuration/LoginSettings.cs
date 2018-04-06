using OA.Configuration;

namespace OA.Ultima.Configuration
{
    public sealed class LoginSettings : ASettingsSection
    {
        string _serverAddress;
        int _serverPort;
        string _userName;
        bool _autoSelectLastCharacter;
        string _lastCharacterName;

        public LoginSettings()
        {
            ServerAddress = "127.0.0.1";
            ServerPort = 2593;
            LastCharacterName = string.Empty;
            AutoSelectLastCharacter = false;
        }

        public string UserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }

        public int ServerPort
        {
            get { return _serverPort; }
            set { SetProperty(ref _serverPort, value); }
        }

        public string ServerAddress
        {
            get { return _serverAddress; }
            set { SetProperty(ref _serverAddress, value); }
        }

        public string LastCharacterName
        {
            get { return _lastCharacterName; }
            set { SetProperty(ref _lastCharacterName, value); }
        }

        public bool AutoSelectLastCharacter
        {
            get { return _autoSelectLastCharacter; }
            set { SetProperty(ref _autoSelectLastCharacter, value); }
        }
    }
}