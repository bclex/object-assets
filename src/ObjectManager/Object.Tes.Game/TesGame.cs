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
        TesAssetPack _assetPack;
        TesDataPack _dataPack;
        TesEngine _engine;
        MusicPlayer _musicPlayer;

        [Header("UI")]
        public UIManager UIManager;
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

        private void Awake()
        {
            var tesRender = TesSettings.TesRender;
            Debug.unityLogger.logEnabled = tesRender.EnableLog;
            _playMusic = tesRender.PlayMusic;
            _gameId = GameId.Morrowind;
            instance = this;
        }

        private void Start()
        {
            var dataPath = FileManager.GetFilePath(".", _gameId);
            _assetPack = new TesAssetPack(dataPath, null);
            _dataPack = new TesDataPack(dataPath, null, _gameId);
            _engine = new TesEngine(_assetPack, _dataPack, UIManager);
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

        private void OnDestroy()
        {
            if (_assetPack != null)
            {
                _assetPack.Close();
                _assetPack = null;
            }
            if (_dataPack != null)
            {
                _dataPack.Close();
                _dataPack = null;
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
