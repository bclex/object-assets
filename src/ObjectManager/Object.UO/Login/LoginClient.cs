using OA.Core;
using OA.Ultima.Core.Network;
using OA.Ultima.Data;
using OA.Ultima.Network.Server;
using OA.Ultima.World;
using OA.Ultima.World.Entities.Mobiles;
using System;
using System.Security;
using System.Timers;

namespace OA.Ultima.Login
{
    public class LoginClient : IDisposable
    {
        readonly INetworkClient _network;
        readonly UltimaGame _engine;
        readonly UserInterfaceService _userInterface;
        Timer _keepAliveTimer;
        string _userName;
        SecureString _password;

        public LoginClient()
        {
            _network = Service.Get<INetworkClient>();
            _engine = Service.Get<UltimaGame>();
            _userInterface = Service.Get<UserInterfaceService>();
            Initialize();
        }

        // ============================================================================================================
        // Packet registration and unregistration
        // ============================================================================================================
        void Initialize()
        {
            Register<LoginConfirmPacket>(0x1B, 37, ReceiveLoginConfirmPacket);
            Register<LoginCompletePacket>(0x55, 1, ReceiveLoginComplete);
            Register<ServerPingPacket>(0x73, 2, ReceivePingPacket);
            Register<LoginRejectionPacket>(0x82, 2, ReceiveLoginRejection);
            Register<DeleteResultPacket>(0x85, 2, ReceiveDeleteCharacterResponse);
            Register<CharacterListUpdatePacket>(0x86, -1, ReceiveCharacterListUpdate);
            Register<ServerRelayPacket>(0x8C, 11, ReceiveServerRelay);
            Register<ServerListPacket>(0xA8, -1, ReceiveServerList);
            Register<CharacterCityListPacket>(0xA9, -1, ReceiveCharacterList);
            Register<SupportedFeaturesPacket>(0xB9, 3, ReceiveEnableFeatures);
            Register<VersionRequestPacket>(0xBD, -1, ReceiveVersionRequest);
        }

        public void Dispose()
        {
            StopKeepAlivePackets();
            _network.Unregister(this);
        }

        public void Register<T>(int id, int length, Action<T> onReceive) where T : IRecvPacket
        {
            _network.Register(this, id, length, onReceive);
        }

        public void Unregister(int id)
        {
            _network.Unregister(this, id);
        }

        // ============================================================================================================
        // Connection and Disconnect
        // ============================================================================================================
        public bool Connect(string host, int port, string account, SecureString password)
        {
            Disconnect();
            if (_network.Connect(host, port))
            {
                _userName = account;
                _password = password;
                _network.Send(new SeedPacket(0x1337BEEF, Settings.UltimaOnline.PatchVersion));
                Login(_userName, _password);
                return true;
            }
            return false;
        }

        public void Disconnect()
        {
            StopKeepAlivePackets();
            if (_network.IsConnected)
            {
                _network.Disconnect();
            }
        }

        // ============================================================================================================
        // Login sequence routines
        // ============================================================================================================
        public void Login(string account, SecureString password)
        {
            _network.Send(new LoginPacket(account, password.ConvertToUnsecureString()));
        }

        public void SelectShard(int index)
        {
            _network.Send(new SelectServerPacket(index));
        }

        public void LoginWithCharacter(int index)
        {
            if (Characters.List[index].Name != string.Empty)
            {
                _engine.Models.Next = new WorldModel();
                _network.Send(new LoginCharacterPacket(Characters.List[index].Name, index, Utility.IPAddress));
                Macros.Player.Load(Characters.List[index].Name);
            }
        }

        public void CreateCharacter(CreateCharacterPacket packet)
        {
            _engine.Models.Next = new WorldModel();
            _network.Send(packet);
        }

        public void DeleteCharacter(int index)
        {
            if (index == -1)
                return;
            if (Characters.List[index].Name != string.Empty)
                _network.Send(new DeleteCharacterPacket(index, Utility.IPAddress));
        }

        public void SendClientVersion()
        {
            if (ClientVersion.HasExtendedFeatures(Settings.UltimaOnline.PatchVersion))
            {
                Utils.Info("Client version is greater than 6.0.14.2, enabling extended 0xB9 packet.");
                Unregister(0xB9);
                Register<SupportedFeaturesPacket>(0xB9, 5, ReceiveEnableFeatures);
            }
            _network.Send(new ClientVersionPacket(Settings.UltimaOnline.PatchVersion));
        }

        void ReceiveDeleteCharacterResponse(DeleteResultPacket packet)
        {
            MsgBoxGump.Show(packet.Result, MsgBoxTypes.OkOnly);
        }

        void ReceiveCharacterListUpdate(CharacterListUpdatePacket packet)
        {
            Characters.SetCharacterList(packet.Characters);
            (_engine.Models.Current as LoginModel).ShowCharacterList();
        }

        void ReceiveCharacterList(CharacterCityListPacket packet)
        {
            Characters.SetCharacterList(packet.Characters);
            Characters.SetStartingLocations(packet.Locations);
            StartKeepAlivePackets();
            (_engine.Models.Current as LoginModel).ShowCharacterList();
        }

        void ReceiveServerList(ServerListPacket packet)
        {
            (_engine.Models.Current as LoginModel).ShowServerList((packet).Servers);
        }

        void ReceiveLoginRejection(LoginRejectionPacket packet)
        {
            Disconnect();
            (_engine.Models.Current as LoginModel).ShowLoginRejection(packet.Reason);
        }

        void ReceiveServerRelay(ServerRelayPacket packet)
        {
            // On OSI, upon receiving this packet, the client would disconnect and
            // log in to the specified server. Since emulated servers use the same
            // server for both shard selection and world, we don't need to disconnect.
            _network.IsDecompressionEnabled = true;
            _network.Send(new GameLoginPacket(packet.AccountId, _userName, _password.ConvertToUnsecureString()));
        }

        void ReceiveEnableFeatures(SupportedFeaturesPacket packet)
        {
            PlayerState.ClientFeatures.SetFlags(packet.Flags);
        }

        void ReceiveVersionRequest(VersionRequestPacket packet)
        {
            SendClientVersion();
        }

        void ReceivePingPacket(ServerPingPacket packet)
        {
        }

        // ============================================================================================================
        // Login to World - Nominally, the server should send LoginConfirmPacket, followed by GeneralInfo0x08, and 
        // finally LoginCompletePacket. However, the legacy client finds it valid to receive the packets in any
        // order. The code below allows any of these possibilities.
        // ============================================================================================================
        LoginConfirmPacket _queuedLoginConfirmPacket;
        bool _loggingInToWorld;

        void ReceiveLoginConfirmPacket(LoginConfirmPacket packet)
        {
            _queuedLoginConfirmPacket = packet;
            // set the player serial and create the player entity. Don't need to do anything with it yet.
            WorldModel.PlayerSerial = packet.Serial;
            var player = WorldModel.Entities.GetObject<Mobile>(WorldModel.PlayerSerial, true);
            if (player == null)
                Utils.Critical("Could not create player object.");
            CheckIfOkayToLogin();
        }

        void ReceiveLoginComplete(LoginCompletePacket packet)
        {
            CheckIfOkayToLogin();
        }

        void CheckIfOkayToLogin()
        {
            // Before the client logs in, we need to know the player entity's serial, and the
            // map the player will be loading on login. If we don't have either of these, we
            // delay loading until we do.
            if (!_loggingInToWorld)
            {
                if ((_engine.Models.Next as WorldModel).MapIndex != 0xffffffff)
                { // will be 0xffffffff if no map
                    _loggingInToWorld = true; // stops double log in attempt caused by out of order packets.
                    _engine.Models.ActivateNext();
                    if (_engine.Models.Current is WorldModel)
                    {
                        (_engine.Models.Current as WorldModel).LoginToWorld();
                        Mobile player = WorldModel.Entities.GetObject<Mobile>(_queuedLoginConfirmPacket.Serial, true);
                        if (player == null)
                            Utils.Critical("No player object ready in CheckIfOkayToLogin().");
                        player.Move_Instant(
                            _queuedLoginConfirmPacket.X, _queuedLoginConfirmPacket.Y,
                            _queuedLoginConfirmPacket.Z, _queuedLoginConfirmPacket.Direction);
                    }
                    else Utils.Critical("Not in world model at login.");
                }
            }
        }

        // ============================================================================================================
        // Keep-alive packets - These are necessary because the client does not otherwise send packets during character
        // creation, and the server will disconnect after a given length of inactivity.
        // ============================================================================================================
        void StartKeepAlivePackets()
        {
            if (_keepAliveTimer == null)
                _keepAliveTimer = new Timer(e => SendKeepAlivePacket(), null, TimeSpan.Zero, TimeSpan.FromSeconds(60));
        }

        void StopKeepAlivePackets()
        {
            if (_keepAliveTimer != null)
                _keepAliveTimer.Dispose();
        }

        void SendKeepAlivePacket()
        {
            if (_network.IsConnected) _network.Send(new ClientPingPacket());
            else StopKeepAlivePackets();
        }
    }
}