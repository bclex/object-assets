using OA.Tes.FilePacks;
using OA.Tes.Formats;
using OA.Core;
using System;
using System.IO;
using UnityEngine;
using OA.Tes.FilePacks.Tes3;

namespace OA.Tes
{
    public class MorrowindEngine
    {
        public static MorrowindEngine instance;

        //public const float maxInteractDistance = 3;

        //private const float playerHeight = 2;
        //private const float playerRadius = 0.4f;
        private const float desiredWorkTimePerFrame = 1.0f / 200;
        //private const int cellRadiusOnLoad = 2;
        private CELLRecord _currentCell;
        ////private GameObject sunObj;
        ////private GameObject waterObj;
        ////private Transform playerTransform;
        private GameObject playerCameraObj;
        //private Color32 defaultAmbientColor = new Color32(137, 140, 160, 255);
        //private RaycastHit[] interactRaycastHitBuffer = new RaycastHit[32];

        public MorrowindDataReader dataReader;
        public TextureManager textureManager;
        public MaterialManager materialManager;
        public NifManager nifManager;
        public CellManager cellManager;
        public TemporalLoadBalancer temporalLoadBalancer;

        public CELLRecord currentCell
        {
            get { return _currentCell; }
        }

        public MorrowindEngine(MorrowindDataReader mwDataReader)
        {
            Debug.Assert(instance == null);
            instance = this;
            dataReader = mwDataReader;
            textureManager = new TextureManager(dataReader);
            materialManager = new MaterialManager(textureManager);
            var markerLayer = 0; // LayerMask.NameToLayer("Marker")
            nifManager = new NifManager(dataReader, materialManager, markerLayer);
            temporalLoadBalancer = new TemporalLoadBalancer();
            cellManager = new CellManager(dataReader, textureManager, nifManager, temporalLoadBalancer);
            //Cursor.SetCursor(textureManager.LoadTexture("tx_cursor", true), Vector2.zero, CursorMode.Auto);
        }

        public void Update()
        {
            // The current cell can be null if the player is outside of the defined game world.
            if (_currentCell == null || !_currentCell.isInterior)
                cellManager.UpdateExteriorCells(playerCameraObj.transform.position);
            temporalLoadBalancer.RunTasks(desiredWorkTimePerFrame);
        }

        public void TestAllCells(string resultsFilePath)
        {
            using (StreamWriter w = new StreamWriter(resultsFilePath))
                foreach (var record in dataReader.ESMFile.GetRecordsOfType<CELLRecord>())
                {
                    var CELL = (CELLRecord)record;
                    try
                    {
                        var cellInfo = cellManager.StartInstantiatingCell(CELL);
                        temporalLoadBalancer.WaitForTask(cellInfo.objectsCreationCoroutine);
                        //DestroyImmediate(cellInfo.gameObject);
                        w.Write("Pass: ");
                    }
                    catch (Exception e) { w.Write($"Fail: {e.Message}"); }
                    if (!CELL.isInterior)
                        w.WriteLine(CELL.gridCoords.ToString());
                    else w.WriteLine(CELL.NAME.value);
                    w.Flush();
                }
        }
    }
}