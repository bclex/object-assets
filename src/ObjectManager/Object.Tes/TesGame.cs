using OA.Core;
using OA.Tes.FilePacks;
using OA.Tes.IO;
using OA.Tes.UI;
using System.IO;
using UnityEngine;

namespace OA.Tes
{
    public class TesGame : MonoBehaviour
    {
        public static TesGame instance;

        bool _playMusic;
        GameId _gameId;
        IAssetPack _asset;
        IDataPack _data;
        TesEngine _engine;
        MusicPlayer _musicPlayer;

        [Header("UI")]
        public TesUIManager UIManager;
        public Sprite UIBackgroundImg;
        public Sprite UICheckmarkImg;
        public Sprite UIDropdownArrowImg;
        public Sprite UIInputFieldBackgroundImg;
        public Sprite UIKnobImg;
        public Sprite UIMaskImg;
        public Sprite UISpriteImg;

        [Header("Prefabs")]
        public GameObject PlayerPrefab;
        public GameObject WaterPrefab;

        void Awake()
        {
            var game = TesSettings.Game;
            Debug.unityLogger.logEnabled = game.EnableLog;
            _playMusic = game.PlayMusic;
            _gameId = GameId.Morrowind;
            instance = this;
        }

        void Start()
        {
            var dataPath = FileManager.GetFilePath(".", _gameId);
            _asset = new TesAssetPack(dataPath, null);
            _data = new TesDataPack(dataPath, null, _gameId);
            _engine = new TesEngine(AssetManager.GetAssetManager(EngineId.Tes), _asset, _data, UIManager);
            if (_playMusic)
            {
                // Start the music.
                _musicPlayer = new MusicPlayer();
                if (_gameId == GameId.Morrowind)
                    foreach (var songFilePath in Directory.GetFiles(dataPath + "/Music/Explore"))
                        if (!songFilePath.Contains("Morrowind Title"))
                            _musicPlayer.AddSong(songFilePath);
                _musicPlayer.Play();
            }

            // Spawn the player.
            _engine.SpawnPlayerOutside(PlayerPrefab, new Vector2i(-2, -9), new Vector3(-137.94f, 2.30f, -1037.6f));
        }

        void OnDestroy()
        {
            if (_asset != null)
            {
                _asset.Dispose();
                _asset = null;
            }
            if (_data != null)
            {
                _data.Dispose();
                _data = null;
            }
        }

        private void Update()
        {
            _engine.Update();
            if (_playMusic)
                _musicPlayer.Update();
        }
    }
}
