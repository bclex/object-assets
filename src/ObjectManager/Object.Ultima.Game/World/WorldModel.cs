using OA.Core;
using OA.Ultima.World.Managers;
using OA.Ultima.World.Maps;
using System;
using System.Collections.Generic;

namespace OA.Ultima.World
{
    class WorldModel : AModel
    {
        // ============================================================================================================
        // Private variables
        // ============================================================================================================
        Map _map;
        WorldCursor _cursor;
        readonly INetworkClient _network;
        readonly UserInterfaceService _userInterface;
        readonly UltimaGame _engine;

        // ============================================================================================================
        // Public Static Properties
        // ============================================================================================================
        public static Serial PlayerSerial { get; set; }

        public static EntityManager Entities { get; private set; }

        public static EffectManager Effects { get; private set; }

        public static StaticManager Statics { get; private set; }

        // ============================================================================================================
        // Public Properties
        // ============================================================================================================
        public WorldClient Client { get; private set; }

        public WorldInput Input { get; private set; }

        public WorldInteraction Interaction { get; private set; }

        public WorldCursor Cursor
        {
            get { return _cursor; }
            set
            {
                if (_cursor != null)
                    _cursor.Dispose();
                _cursor = value;
            }
        }

        public Map Map
        {
            get { return _map; }
        }

        public uint MapIndex
        {
            get { return _map == null ? 0xFFFFFFFF : _map.Index; }
            set
            {
                if (value != MapIndex)
                {
                    // clear all entities
                    Entities.Reset(false);
                    if (_map != null)
                    {
                        var player = Entities.GetPlayerEntity();
                        // save current player position
                        int x = player.X, y = player.Y, z = player.Z;
                        // place the player in null space (allows the map to be reloaded when we return to the same location in a different map).
                        player.SetMap(null);
                        // dispose of map
                        _map.Dispose();
                        _map = null;
                        // add new map!
                        _map = new Map(value);
                        player.SetMap(_map);
                        // restore previous player position
                        player.Position.Set(x, y, z);
                    }
                    else
                    {
                        var player = Entities.GetPlayerEntity();
                        _map = new Map(value);
                        player.SetMap(_map);
                    }
                }
            }
        }

        public static bool IsInWorld { get; set; }  // InWorld allows us to tell when our character object has been loaded in the world.

        // ============================================================================================================
        // Ctor, Initialization, Dispose, Update
        // ============================================================================================================
        public WorldModel()
        {
            Service.Add<WorldModel>(this);
            _engine = Service.Get<UltimaGame>();
            _network = Service.Get<INetworkClient>();
            _userInterface = Service.Get<UserInterfaceService>();
            Entities = new EntityManager(this);
            Entities.Reset(true);
            Effects = new EffectManager(this);
            Statics = new StaticManager();
            Input = new WorldInput(this);
            Interaction = new WorldInteraction(this);
            Client = new WorldClient(this);
        }

        protected override void OnInitialize()
        {
            _engine.SetupWindowForWorld();
            _userInterface.Cursor = Cursor = new WorldCursor(this);
            Client.Initialize();
            Player.PlayerState.Journaling.AddEntry("Welcome to Ultima Online!", 9, 0x3B4, string.Empty, false);
        }

        protected override void OnDispose()
        {
            SaveOpenGumps();
            _engine.SaveResolution();
            Service.Remove<WorldModel>();
            _userInterface.Reset();
            Entities.Reset();
            Entities = null;
            Effects = null;
            Input.Dispose();
            Input = null;
            Interaction = null;
            Client.Dispose();
            Client = null;
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (!_network.IsConnected)
            {
                if (_userInterface.IsModalControlOpen == false)
                {
                    var g = MsgBoxGump.Show("You have lost your connection with the server.", MsgBoxTypes.OkOnly);
                    g.OnClose = OnCloseLostConnectionMsgBox;
                }
            }
            else
            {
                Input.Update(frameMS);
                Entities.Update(frameMS);
                Effects.Update(frameMS);
                Statics.Update(frameMS);
            }
        }

        // ============================================================================================================
        // Public Methods
        // ============================================================================================================
        public void LoginToWorld()
        {
            _userInterface.AddControl(new WorldViewGump(), 0, 0); // world gump will restore its position on load.
            if (!Settings.UserInterface.MenuBarDisabled)
                _userInterface.AddControl(new TopMenuGump(), 0, 0); // by default at the top of the screen.
            Client.SendWorldLoginPackets();
            IsInWorld = true;
            Client.StartKeepAlivePackets();
            // wait until we've received information about the entities around us before restoring saved gumps.
            DelayedAction.Start(RestoreSavedGumps, 1000);
        }

        public void Disconnect()
        {
            _network.Disconnect(); // stops keep alive packets.
            IsInWorld = false;
            _engine.Models.Current = new LoginModel();
        }

        // ============================================================================================================
        // Private/Protected Methods
        // ============================================================================================================
        protected override AView CreateView()
        {
            return new WorldView(this);
        }

        void OnCloseLostConnectionMsgBox()
        {
            Disconnect();
        }

        void SaveOpenGumps()
        {
            Settings.Gumps.SavedGumps.Clear();
            foreach (var gump in _userInterface.OpenControls)
                if (gump is Gump)
                    if ((gump as Gump).SaveOnWorldStop)
                    {
                        Dictionary<string, object> data;
                        if ((gump as Gump).SaveGump(out data))
                            Settings.Gumps.SavedGumps.Add(new SavedGumpProperty(gump.GetType(), data));
                    }
        }

        void RestoreSavedGumps()
        {
            foreach (var savedGump in Settings.Gumps.SavedGumps)
            {
                try
                {
                    var type = Type.GetType(savedGump.GumpType);
                    var gump = System.Activator.CreateInstance(type);
                    if (gump is Gump)
                    {
                        if ((gump as Gump).RestoreGump(savedGump.GumpData))
                            _userInterface.AddControl(gump as Gump, 0, 0);
                        else Utils.Error($"Unable to restore saved gump with type {savedGump.GumpType}: Failed to restore gump.");
                    }
                    else Utils.Error($"Unable to restore saved gump with type {savedGump.GumpType}: Type does not derive from Gump.");
                }
                catch { Utils.Error($"Unable to restore saved gump with type {savedGump.GumpType}: Type cannot be Instanced."); }
            }
            Settings.Gumps.SavedGumps.Clear();
        }
    }
}
