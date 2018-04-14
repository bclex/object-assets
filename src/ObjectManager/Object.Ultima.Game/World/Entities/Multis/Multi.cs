using OA.Ultima.Resources;
using OA.Ultima.World.Data;
using OA.Ultima.World.Entities.Items;
using OA.Ultima.World.Maps;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Ultima.World.Entities.Multis
{
    class Multi : AEntity
    {
        private static List<Multi> _registeredMultis = new List<Multi>();

        private static void RegisterForMapBlockLoads(Multi multi)
        {
            if (!_registeredMultis.Contains(multi))
                _registeredMultis.Add(multi);
        }

        private static void UnregisterForMapBlockLoads(Multi multi)
        {
            if (_registeredMultis.Contains(multi))
                _registeredMultis.Remove(multi);
        }

        public static void AnnounceMapChunkLoaded(MapChunk chunk)
        {
            for (var i = 0; i < _registeredMultis.Count; i++)
                if (!_registeredMultis[i].IsDisposed)
                    _registeredMultis[i].PlaceTilesIntoNewlyLoadedChunk(chunk);
        }

        MultiComponentList _components;

        int _customHouseRevision = 0x7FFFFFFF;
        StaticTile[] _customHouseTiles;
        public int CustomHouseRevision { get { return _customHouseRevision; } }

        // bool _hasCustomTiles = false;
        CustomHouse _customHouse;
        public void AddCustomHousingTiles(CustomHouse house)
        {
            // _hasCustomTiles = true;
            _customHouse = house;
            _customHouseTiles = house.GetStatics(_components.Width, _components.Height);
        }

        int _MultiID = -1;
        public int MultiID
        {
            get { return _MultiID; }
            set
            {
                if (_MultiID != value)
                {
                    _MultiID = value;
                    _components = MultiData.GetComponents(_MultiID);
                    InitialLoadTiles();
                }
            }
        }

        public Multi(Serial serial, Map map)
            : base(serial, map)
        {
            RegisterForMapBlockLoads(this);
        }

        public override void Dispose()
        {
            UnregisterForMapBlockLoads(this);
            base.Dispose();
        }

        private void InitialLoadTiles()
        {
            var px = Position.X;
            var py = Position.Y;
            foreach (MultiComponentList.MultiItem item in _components.Items)
            {
                var x = px + item.OffsetX;
                var y = py + item.OffsetY;
                var tile = Map.GetMapTile((uint)x, (uint)y);
                if (tile != null)
                {
                    if (tile.ItemExists(item.ItemID, item.OffsetZ))
                        continue;
                    var staticItem = new StaticItem(item.ItemID, 0, 0, Map);
                    if (staticItem.ItemData.IsDoor)
                        continue;
                    staticItem.Position.Set(x, y, Z + item.OffsetZ);
                }
            }
        }

        private void PlaceTilesIntoNewlyLoadedChunk(MapChunk chunk)
        {
            var px = Position.X;
            var py = Position.Y;
            var bounds = new RectInt((int)chunk.ChunkX * 8, (int)chunk.ChunkY * 8, 8, 8);
            foreach (MultiComponentList.MultiItem item in _components.Items)
            {
                var x = px + item.OffsetX;
                var y = py + item.OffsetY;
                if (bounds.Contains(new Vector2Int(x, y)))
                {
                    // would it be faster to get the tile from the chunk?
                    var tile = Map.GetMapTile(x, y);
                    if (tile != null)
                        if (!tile.ItemExists(item.ItemID, item.OffsetZ))
                        {
                            var staticItem = new StaticItem(item.ItemID, 0, 0, Map);
                            staticItem.Position.Set(x, y, Z + item.OffsetZ);
                        }
                }
            }
        }

        public override int GetMaxUpdateRange()
        {
            if (_customHouse == null) return 22;
            else return 24;
        }
    }
}
