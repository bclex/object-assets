//using OA.Core;
//using OA.Core.Input;
//using OA.Core.UI;
//using OA.Ultima.Audio;
//using OA.Ultima.Configuration.Properties;
//using OA.Ultima.Core;
//using OA.Ultima.Core.Graphics;
//using OA.Ultima.Core.Network;
//using OA.Ultima.Core.Patterns.MVC;
//using OA.Ultima.IO;
//using OA.Ultima.Login;
//using OA.Ultima.Resources;
//using OA.Ultima.World;
//using System.ComponentModel;

//namespace OA.Ultima
//{
//    public class UltimaGame
//    {
//        UserInterfaceService _userInterface;
//        PluginManager _plugins;
//        ModelManager _models;
//        bool _isRunning;
//        double _totalMS;

//        public ModelManager Models => _models;
//        public double TotalMS => _totalMS;

//        public UltimaGame()
//        {
//            GameForm.FormClosing += OnFormClosing;
//            SetupWindowForLogin();
//        }

//        protected override void Initialize()
//        {
//            Content.RootDirectory = "Content";
//            Service.Add(this);
//            Service.Add(new SpriteBatch3D(this));
//            Service.Add(new SpriteBatchUI(this));
//            Service.Add<INetworkClient>(new NetworkClient());
//            Service.Add<IInputService>(new InputService(Window.Handle));
//            Service.Add(new AudioService());
//            _userInterface = Service.Add(new UserInterfaceService());
//            _plugins = new PluginManager(AppDomain.CurrentDomain.BaseDirectory);
//            _models = new ModelManager();
//            // Make sure we have a UO installation before loading IO.
//            if (FileManager.IsDataPresent)
//            {
//                // Initialize and load data
//                var provider = new ResourceProvider(this);
//                provider.RegisterResource(new EffectDataResource());
//                Service.Add(provider);
//                HueData.Initialize(GraphicsDevice);
//                SkillsData.Initialize();
//                //GraphicsDevice.Textures[1] = HueData.HueTexture0;
//                //GraphicsDevice.Textures[2] = HueData.HueTexture1;
//                _isRunning = true;
//                WorldModel.IsInWorld = false;
//                Models.Current = new LoginModel();
//            }
//            else Utils.Critical("Did not find a compatible UO Installation. UltimaXNA is compatible with any version of UO through Mondian's Legacy.");
//        }

//        protected override void OnUpdate(double totalMS, double frameMS)
//        {
//            if (!_isRunning)
//            {
//                UltimaGameSettings.Save();
//                Exit();
//            }
//            else
//            {
//                IsFixedTimeStep = UltimaGameSettings.Engine.IsFixedTimeStep;
//                _totalMS = totalMS;
//                Service.Get<AudioService>().Update();
//                Service.Get<IInputService>().Update(totalMS, frameMS);
//                _userInterface.Update(totalMS, frameMS);
//                Service.Get<INetworkClient>().Slice();
//                Models.Current.Update(totalMS, frameMS);
//            }
//        }

//        protected override void OnDraw(double frameMS)
//        {
//            if (!IsMinimized)
//            {
//                if (Models.Current is WorldModel)
//                {
//                    ResolutionProperty resolution = UltimaGameSettings.UserInterface.PlayWindowGumpResolution;
//                    CheckWindowSize(resolution.Width, resolution.Height);
//                }
//                else CheckWindowSize(800, 600);
//                Models.Current.GetView().Draw(frameMS);
//                _userInterface.Draw(frameMS);
//            }
//        }

//        public void Quit()
//        {
//            _isRunning = false;
//        }

//        public void SetupWindowForLogin()
//        {
//            RestoreWindow();
//            Window.AllowUserResizing = false;
//            SetGraphicsDeviceWidthHeight(new ResolutionProperty(800, 600)); // a wee bit bigger than legacy. Looks nicer.
//        }

//        public void SetupWindowForWorld()
//        {
//            Window.AllowUserResizing = true;
//            SetGraphicsDeviceWidthHeight(Settings.UserInterface.WindowResolution);
//            if (Settings.UserInterface.IsMaximized)
//                MaximizeWindow();
//        }

//        public void SaveResolution()
//        {
//            if (IsMaximized)
//                UltimaGameSettings.UserInterface.IsMaximized = true;
//            else
//            {
//                var res = new ResolutionProperty(DeviceManager.PreferredBackBufferWidth, DeviceManager.PreferredBackBufferHeight);
//                UltimaGameSettings.UserInterface.WindowResolution = res;
//            }
//        }

//        void OnFormClosing(object sender, CancelEventArgs e)
//        {
//            // we must dispose of the active model BEFORE we dispose of the window.
//            if (Models.Current != null)
//                Models.Current = null;
//        }
//    }
//}