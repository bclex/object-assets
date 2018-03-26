using OA.Core;
using OA.Ultima.Core;
using OA.Ultima.Core.Patterns.MVC;
using OA.Ultima.Login.Accounts;
using OA.Ultima.Login.Data;
using OA.Ultima.Network.Client;
using OA.Ultima.Network.Server;
using OA.Ultima.Resources;

namespace OA.Ultima.Login
{
    class LoginModel : AModel
    {
        UserInterfaceService _userInterface;

        public LoginClient Client { get; private set; }

        public LoginModel()
        {
            Service.Add(this);
            Client = new LoginClient();
        }

        protected override AView CreateView()
        {
            return new LoginView(this);
        }

        protected override void OnInitialize()
        {
            Service.Get<UltimaGame>().SetupWindowForLogin();
            _userInterface = Service.Get<UserInterfaceService>();
            _userInterface.Cursor = new UltimaCursor();
            Service.Get<AudioService>().PlayMusic(0);
            ResetToLogin();
        }

        protected override void OnDispose()
        {
            Service.Get<AudioService>().StopMusic();
            Service.Remove<LoginModel>();
            Client.Dispose();
            Client = null;
            _userInterface.Reset();
        }

        public override void Update(double totalTime, double frameTime)
        {
            // nothing needs to be updated.
        }

        // ============================================================================================================
        // Login Screen
        // ============================================================================================================

        public Gump CurrentGump { get; private set; }
        ServerListEntry[] m_Servers;

        void ResetToLogin()
        {
            Client.Disconnect();
            _userInterface.Reset();
            CurrentGump = _userInterface.AddControl(new LoginGump(OnLogin), 0, 0) as Gump;
        }

        void OnLogin(string server, int port, string account, SecureString password)
        {
            CurrentGump.Dispose();
            CurrentGump = _userInterface.AddControl(new LoginStatusGump(OnCancelLogin), 0, 0) as Gump;
            if (Client.Connect(Settings.Login.ServerAddress, Settings.Login.ServerPort, account, password))
                (CurrentGump as LoginStatusGump).Page = LoginStatusGump.PageCouldntConnect;
            else
                (CurrentGump as LoginStatusGump).Page = LoginStatusGump.PageVerifyingAccount;
        }

        void OnCancelLogin()
        {
            ResetToLogin();
        }

        // ============================================================================================================
        // Logging in Screen
        // ============================================================================================================

        public void ShowLoginRejection(LoginRejectionReasons rejection)
        {
            switch (rejection)
            {
                case LoginRejectionReasons.InvalidAccountPassword:
                    (CurrentGump as LoginStatusGump).ActivePage = LoginStatusGump.PageIncorrectUsernamePassword;
                    break;
                case LoginRejectionReasons.AccountInUse:
                    (CurrentGump as LoginStatusGump).ActivePage = LoginStatusGump.PageAccountInUse;
                    break;
                case LoginRejectionReasons.AccountBlocked:
                    (CurrentGump as LoginStatusGump).ActivePage = LoginStatusGump.PageAccountBlocked;
                    break;
                case LoginRejectionReasons.BadPassword:
                    (CurrentGump as LoginStatusGump).ActivePage = LoginStatusGump.PageCredentialsInvalid;
                    break;
                case LoginRejectionReasons.IdleExceeded:
                    (CurrentGump as LoginStatusGump).ActivePage = LoginStatusGump.PageConnectionLost;
                    break;
                case LoginRejectionReasons.BadCommuncation:
                    (CurrentGump as LoginStatusGump).ActivePage = LoginStatusGump.PageBadCommunication;
                    break;
            }
        }

        // ============================================================================================================
        // Server list
        // ============================================================================================================

        public void ShowServerList(ServerListEntry[] servers)
        {
            m_Servers = servers;
            CurrentGump.Dispose();
            CurrentGump = _userInterface.AddControl(new SelectServerGump(servers, OnBackToLoginScreen, OnSelectLastServer, OnSelectServer), 0, 0) as Gump;
            // Auto select first server if only one exists.
            if (m_Servers.Length == 1)
                OnSelectServer(m_Servers[0].Index);
        }

        void OnBackToLoginScreen()
        {
            ResetToLogin();
        }

        void OnSelectLastServer()
        {
            // select the last server.
        }

        void OnSelectServer(int index)
        {
            (CurrentGump as SelectServerGump).ActivePage = 2;
            Client.SelectShard(index);
        }

        // ============================================================================================================
        // Character list
        // ============================================================================================================

        public void ShowCharacterList()
        {
            CurrentGump.Dispose();
            CurrentGump = _userInterface.AddControl(new CharacterListGump(
                OnBackToSelectServer, OnLoginWithCharacter, OnDeleteCharacter, OnNewCharacter), 0, 0) as Gump;
            if (Settings.Login.AutoSelectLastCharacter && !string.IsNullOrWhiteSpace(Settings.Login.LastCharacterName))
                for (var i = 0; i < Characters.List.Length; i++)
                    if (Characters.List[i].Name == Settings.Login.LastCharacterName)
                        OnLoginWithCharacter(i);
        }

        void OnBackToSelectServer()
        {
            // !!! This SHOULD take us back to the 'logging in' screen,
            // which automatically logs in again. But we can't do that,
            // since I have UltimaClient clear your account/password data
            // once connected (is this really neccesary?) Have to fix ..
            ResetToLogin();
        }

        void OnLoginWithCharacter(int index)
        {
            if (index < 0 || index >= Characters.List.Length)
                return;
            (CurrentGump as CharacterListGump).ActivePage = 2;
            Client.LoginWithCharacter(index);
        }

        void OnDeleteCharacter(int index)
        {
            Client.DeleteCharacter(index);
        }

        void OnNewCharacter()
        {
            ShowCreateCharacter();
        }

        // ============================================================================================================
        // Create Character
        // ============================================================================================================

        CreateCharacterData _data;

        public void ShowCreateCharacter()
        {
            _data = new CreateCharacterData();
            _data.ShirtColor = Utility.RandomValue(600, 900);
            _data.PantsColor = Utility.RandomValue(600, 900);
            openSkillsGump();
        }

        void openSkillsGump()
        {
            CurrentGump.Dispose();
            CurrentGump = _userInterface.AddControl(new CreateCharSkillsGump(OnForwardSkills, OnBackwardSkills), 0, 0) as Gump;
            if (_data.HasSkillData)
                (CurrentGump as CreateCharSkillsGump).RestoreData(_data);
        }

        void openAppearanceGump()
        {
            CurrentGump.Dispose();
            CurrentGump = _userInterface.AddControl(new CreateCharAppearanceGump(OnForwardAppearance, OnBackwardAppearance), 0, 0) as Gump;
            if (_data.HasAppearanceData)
                (CurrentGump as CreateCharAppearanceGump).RestoreData(_data);
        }

        bool validateSkills()
        {
            var gump = CurrentGump as CreateCharSkillsGump;
            // we need to make sure that the stats add up to 80, skills add up to 100, and 3 unique skills are selected.
            // if not, pop up an appropriate error message.
            if (gump.Strength + gump.Dexterity + gump.Intelligence != 80)
            {
                MsgBoxGump.Show("Error: your stat values did not add up to 80. Please logout and try to make another character.", MsgBoxTypes.OkOnly);
                return false;
            }
            if (gump.SkillPoints0 + gump.SkillPoints1 + gump.SkillPoints2 != 100)
            {
                MsgBoxGump.Show("Error: your skill values did not add up to 100. Please logout and try to make another character.", MsgBoxTypes.OkOnly);
                return false;
            }
            if (gump.SkillIndex0 == -1 || gump.SkillIndex1 == -1 || gump.SkillIndex2 == -1 ||
                (gump.SkillIndex0 == gump.SkillIndex1) ||
                (gump.SkillIndex1 == gump.SkillIndex2) ||
                (gump.SkillIndex0 == gump.SkillIndex2))
            {
                MsgBoxGump.Show("You must have three unique skills chosen!", MsgBoxTypes.OkOnly);
                return false;
            }
            (CurrentGump as CreateCharSkillsGump).SaveData(_data);
            return true;
        }

        bool validateAppearance()
        {
            // get the resource provider
            var provider = Service.Get<IResourceProvider>();
            // save the values
            (CurrentGump as CreateCharAppearanceGump).SaveData(_data);
            if (_data.Name.Length < 2)
            {
                MsgBoxGump.Show(provider.GetString(1075458), MsgBoxTypes.OkOnly); // 1075458: Your character name is too short.
                return false;
            }
            if (_data.Name[_data.Name.Length - 1] == '.')
            {
                MsgBoxGump.Show(provider.GetString(1075457), MsgBoxTypes.OkOnly); // 1075457: Your character name cannot end with a period('.').
                return false;
            }
            return true;
        }

        void OnBackwardSkills()
        {
            ShowCharacterList();
        }

        void OnForwardSkills()
        {
            if (validateSkills())
                openAppearanceGump();
        }

        void OnBackwardAppearance()
        {
            openSkillsGump();
        }

        void OnForwardAppearance()
        {
            if (validateAppearance())
            {
                Client.CreateCharacter(new CreateCharacterPacket(_data, 0, (short)Characters.FirstEmptySlot, Utility.IPAddress));
            }
        }
    }
}