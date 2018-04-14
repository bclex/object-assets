using OA.Ultima.Data;
using OA.Ultima.Resources;
using OA.Ultima.World.Entities.Items;
using OA.Ultima.World.Maps;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Ultima.World.Entities.Mobiles
{
    static class MobileMovementCheck
    {
        static List<Item>[] _pools = { new List<Item>(), new List<Item>(), new List<Item>(), new List<Item>() };
        static List<MapTile> _tiles = new List<MapTile>();
        const TileFlag ImpassableSurface = TileFlag.Impassable | TileFlag.Surface;
        const int PersonHeight = 16;
        const int StepHeight = 2;

        public static int GetNextZ(Mobile m, Position3D loc, Direction d)
        {
            if (CheckMovement(m, loc, d, out int newZ, true))
                return newZ;
            return loc.Z;
        }

        public static bool CheckMovement(Mobile m, Position3D loc, Direction d, out int newZ, bool forceOK = false)
        {
            var map = m.Map;
            if (map == null)
            {
                newZ = 0;
                return true;
            }

            var xStart = loc.X;
            var yStart = loc.Y;
            int xForward = xStart, yForward = yStart;
            int xRight = xStart, yRight = yStart;
            int xLeft = xStart, yLeft = yStart;

            var checkDiagonals = ((int)d & 0x1) == 0x1;

            offsetXY(d, ref xForward, ref yForward);
            offsetXY((Direction)(((int)d - 1) & 0x7), ref xLeft, ref yLeft);
            offsetXY((Direction)(((int)d + 1) & 0x7), ref xRight, ref yRight);

            if (xForward < 0 || yForward < 0 || xForward >= map.TileWidth || yForward >= map.TileHeight)
            {
                newZ = 0;
                return false;
            }

            int startZ, startTop;

            var itemsStart = _pools[0];
            var itemsForward = _pools[1];
            var itemsLeft = _pools[2];
            var itemsRight = _pools[3];

            var reqFlags = ImpassableSurface;

            // if (m.CanSwim)
            //     reqFlags |= TileFlag.Wet;

            if (checkDiagonals)
            {
                var tileStart = map.GetMapTile(xStart, yStart);
                var tileForward = map.GetMapTile(xForward, yForward);
                var tileLeft = map.GetMapTile(xLeft, yLeft);
                var tileRight = map.GetMapTile(xRight, yRight);
                if ((tileForward == null) || (tileStart == null) || (tileLeft == null) || (tileRight == null))
                {
                    newZ = loc.Z;
                    return false;
                }

                var tiles = _tiles;

                tiles.Add(tileStart);
                tiles.Add(tileForward);
                tiles.Add(tileLeft);
                tiles.Add(tileRight);

                for (var i = 0; i < tiles.Count; ++i)
                {
                    var tile = tiles[i];
                    for (var j = 0; j < tile.Entities.Count; ++j)
                    {
                        var entity = tile.Entities[j];
                        // if (ignoreMovableImpassables && item.Movable && item.ItemData.Impassable)
                        //     continue;
                        if (entity is Item)
                        {
                            var item = (Item)entity;
                            if ((item.ItemData.Flags & reqFlags) == 0)
                                continue;
                            if (tile == tileStart && item.AtWorldPoint(xStart, yStart) && item.ItemID < 0x4000)
                                itemsStart.Add(item);
                            else if (tile == tileForward && item.AtWorldPoint(xForward, yForward) && item.ItemID < 0x4000)
                                itemsForward.Add(item);
                            else if (tile == tileLeft && item.AtWorldPoint(xLeft, yLeft) && item.ItemID < 0x4000)
                                itemsLeft.Add(item);
                            else if (tile == tileRight && item.AtWorldPoint(xRight, yRight) && item.ItemID < 0x4000)
                                itemsRight.Add(item);
                        }
                    }
                }
            }
            else
            {
                var tileStart = map.GetMapTile(xStart, yStart);
                var tileForward = map.GetMapTile(xForward, yForward);
                if (tileForward == null || tileStart == null)
                {
                    newZ = loc.Z;
                    return false;
                }
                if (tileStart == tileForward)
                {
                    for (var i = 0; i < tileStart.Entities.Count; ++i)
                    {
                        var entity = tileStart.Entities[i];
                        if (entity is Item)
                        {
                            var item = (Item)entity;
                            // if (ignoreMovableImpassables && item.Movable && item.ItemData.Impassable)
                            //     continue;
                            if ((item.ItemData.Flags & reqFlags) == 0)
                                continue;
                            if (item.AtWorldPoint(xStart, yStart) && item.ItemID < 0x4000)
                                itemsStart.Add(item);
                            else if (item.AtWorldPoint(xForward, yForward) && item.ItemID < 0x4000)
                                itemsForward.Add(item);
                        }
                    }
                }
                else
                {
                    for (var i = 0; i < tileForward.Entities.Count; ++i)
                    {
                        var entity = tileForward.Entities[i];
                        if (entity is Item)
                        {
                            var item = (Item)entity;
                            // if (ignoreMovableImpassables && item.Movable && item.ItemData.Impassable)
                            //     continue;
                            if ((item.ItemData.Flags & reqFlags) == 0)
                                continue;
                            if (item.AtWorldPoint(xForward, yForward) && item.ItemID < 0x4000)
                                itemsForward.Add(item);
                        }
                    }

                    for (var i = 0; i < tileStart.Entities.Count; ++i)
                    {
                        AEntity entity = tileStart.Entities[i];
                        if (entity is Item)
                        {
                            var item = (Item)entity;
                            // if (ignoreMovableImpassables && item.Movable && item.ItemData.Impassable)
                            //     continue;
                            if ((item.ItemData.Flags & reqFlags) == 0)
                                continue;
                            if (item.AtWorldPoint(xStart, yStart) && item.ItemID < 0x4000)
                                itemsStart.Add(item);
                        }
                    }
                }
            }

            getStartZ(m, map, loc, itemsStart, out startZ, out startTop);

            var moveIsOk = check(map, m, itemsForward, xForward, yForward, startTop, startZ, out newZ) || forceOK;

            if (moveIsOk && checkDiagonals)
            {
                int hold;
                if (UltimaGameSettings.UltimaOnline.AllowCornerMovement)
                {
                    if (!check(map, m, itemsLeft, xLeft, yLeft, startTop, startZ, out hold) && !check(map, m, itemsRight, xRight, yRight, startTop, startZ, out hold))
                        moveIsOk = false;
                }
                else
                {
                    if (!check(map, m, itemsLeft, xLeft, yLeft, startTop, startZ, out hold) || !check(map, m, itemsRight, xRight, yRight, startTop, startZ, out hold))
                        moveIsOk = false;
                }
            }

            for (var i = 0; i < (checkDiagonals ? 4 : 2); ++i)
                if (_pools[i].Count > 0)
                    _pools[i].Clear();

            if (!moveIsOk)
                newZ = startZ;

            return moveIsOk;
        }

        static bool check(Map map, Mobile m, List<Item> items, int x, int y, int startTop, int startZ, out int newZ)
        {
            newZ = 0;

            var mapTile = map.GetMapTile(x, y);
            if (mapTile == null)
                return false;

            var tiles = mapTile.GetStatics().ToArray();

            var landBlocks = (mapTile.Ground.LandData.Flags & TileFlag.Impassable) != 0;
            var considerLand = !mapTile.Ground.IsIgnored;

            //if (landBlocks && canSwim && (TileData.LandTable[landTile.ID & 0x3FFF].Flags & TileFlag.Wet) != 0)	//Impassable, Can Swim, and Is water.  Don't block it.
            //    landBlocks = false;
            // else 
            // if (cantWalk && (TileData.LandTable[landTile.ID & 0x3FFF].Flags & TileFlag.Wet) == 0)	//Can't walk and it's not water
            //     landBlocks = true;

            int landLow = 0, landCenter = 0, landTop = 0;
            landCenter = map.GetAverageZ(x, y, ref landLow, ref landTop);

            var moveIsOk = false;
            var stepTop = startTop + StepHeight;
            var checkTop = startZ + PersonHeight;
            var ignoreDoors = (!m.IsAlive || m.Body == 0x3DB);

            #region Tiles
            for (var i = 0; i < tiles.Length; ++i)
            {
                var tile = tiles[i];
                if ((tile.ItemData.Flags & ImpassableSurface) == TileFlag.Surface) //  || (canSwim && (flags & TileFlag.Wet) != 0) Surface && !Impassable
                {
                    // if (cantWalk && (flags & TileFlag.Wet) == 0)
                    //     continue;
                    var itemZ = tile.Z;
                    var itemTop = itemZ;
                    var ourZ = itemZ + tile.ItemData.CalcHeight;
                    var ourTop = ourZ + PersonHeight;
                    var testTop = checkTop;
                    if (moveIsOk)
                    {
                        var cmp = Math.Abs(ourZ - m.Z) - Math.Abs(newZ - m.Z);
                        if (cmp > 0 || (cmp == 0 && ourZ > newZ))
                            continue;
                    }
                    if (ourZ + PersonHeight > testTop)
                        testTop = ourZ + PersonHeight;
                    if (!tile.ItemData.IsBridge)
                        itemTop += tile.ItemData.Height;
                    if (stepTop >= itemTop)
                    {
                        var landCheck = itemZ;
                        if (tile.ItemData.Height >= StepHeight)
                            landCheck += StepHeight;
                        else
                            landCheck += tile.ItemData.Height;
                        if (considerLand && landCheck < landCenter && landCenter > ourZ && testTop > landLow)
                            continue;
                        if (IsOk(ignoreDoors, ourZ, testTop, tiles, items))
                        {
                            newZ = ourZ;
                            moveIsOk = true;
                        }
                    }
                }
            }
            #endregion

            #region Items
            for (var i = 0; i < items.Count; ++i)
            {
                var item = items[i];
                var itemData = item.ItemData;
                var flags = itemData.Flags;
                if ((flags & ImpassableSurface) == TileFlag.Surface) // Surface && !Impassable && !Movable
                {
                    //  || (m.CanSwim && (flags & TileFlag.Wet) != 0))
                    // !item.Movable && 
                    // if (cantWalk && (flags & TileFlag.Wet) == 0)
                    //     continue;
                    var itemZ = item.Z;
                    var itemTop = itemZ;
                    var ourZ = itemZ + itemData.CalcHeight;
                    var ourTop = ourZ + PersonHeight;
                    var testTop = checkTop;
                    if (moveIsOk)
                    {
                        var cmp = Math.Abs(ourZ - m.Z) - Math.Abs(newZ - m.Z);
                        if (cmp > 0 || (cmp == 0 && ourZ > newZ))
                            continue;
                    }
                    if (ourZ + PersonHeight > testTop)
                        testTop = ourZ + PersonHeight;
                    if (!itemData.IsBridge)
                        itemTop += itemData.Height;
                    if (stepTop >= itemTop)
                    {
                        var landCheck = itemZ;
                        if (itemData.Height >= StepHeight)
                            landCheck += StepHeight;
                        else
                            landCheck += itemData.Height;
                        if (considerLand && landCheck < landCenter && landCenter > ourZ && testTop > landLow)
                            continue;
                        if (IsOk(ignoreDoors, ourZ, testTop, tiles, items))
                        {
                            newZ = ourZ;
                            moveIsOk = true;
                        }
                    }
                }
            }

            #endregion

            if (considerLand && !landBlocks && (stepTop) >= landLow)
            {
                var ourZ = landCenter;
                var ourTop = ourZ + PersonHeight;
                var testTop = checkTop;
                if (ourZ + PersonHeight > testTop)
                    testTop = ourZ + PersonHeight;
                var shouldCheck = true;
                if (moveIsOk)
                {
                    var cmp = Math.Abs(ourZ - m.Z) - Math.Abs(newZ - m.Z);
                    if (cmp > 0 || (cmp == 0 && ourZ > newZ))
                        shouldCheck = false;
                }
                if (shouldCheck && IsOk(ignoreDoors, ourZ, testTop, tiles, items))
                {
                    newZ = ourZ;
                    moveIsOk = true;
                }
            }
            return moveIsOk;
        }

        static bool IsOk(bool ignoreDoors, int ourZ, int ourTop, StaticItem[] tiles, List<Item> items)
        {
            for (var i = 0; i < tiles.Length; ++i)
            {
                var item = tiles[i];
                if ((item.ItemData.Flags & ImpassableSurface) != 0) // Impassable || Surface
                {
                    var checkZ = item.Z;
                    var checkTop = checkZ + item.ItemData.CalcHeight;
                    if (checkTop > ourZ && ourTop > checkZ)
                        return false;
                }
            }
            for (var i = 0; i < items.Count; ++i)
            {
                var item = items[i];
                var itemID = item.ItemID & 0x3FFF;
                var itemData = TileData.ItemData[itemID];
                var flags = itemData.Flags;
                if ((flags & ImpassableSurface) != 0) // Impassable || Surface
                {
                    if (ignoreDoors && ((flags & TileFlag.Door) != 0 || itemID == 0x692 || itemID == 0x846 || itemID == 0x873 || (itemID >= 0x6F5 && itemID <= 0x6F6)))
                        continue;
                    var checkZ = item.Z;
                    var checkTop = checkZ + itemData.CalcHeight;
                    if (checkTop > ourZ && ourTop > checkZ)
                        return false;
                }
            }
            return true;
        }

        public static Vector2Int OffsetTile(Position3D currentTile, Direction facing)
        {
            var nextTile = currentTile.Tile;
            switch (facing & Direction.FacingMask)
            {
                case Direction.North:
                    nextTile.y--;
                    break;
                case Direction.South:
                    nextTile.y++;
                    break;
                case Direction.West:
                    nextTile.x--;
                    break;
                case Direction.East:
                    nextTile.x++;
                    break;
                case Direction.Right:
                    nextTile.x++;
                    nextTile.y--;
                    break;
                case Direction.Left:
                    nextTile.x--;
                    nextTile.y++;
                    break;
                case Direction.Down:
                    nextTile.x++;
                    nextTile.y++;
                    break;
                case Direction.Up:
                    nextTile.x--;
                    nextTile.y--;
                    break;
            }
            return nextTile;
        }

        static void offsetXY(Direction d, ref int x, ref int y)
        {
            switch (d & Direction.FacingMask)
            {
                case Direction.North:
                    --y;
                    break;
                case Direction.South:
                    ++y;
                    break;
                case Direction.West:
                    --x;
                    break;
                case Direction.East:
                    ++x;
                    break;
                case Direction.Right:
                    ++x;
                    --y;
                    break;
                case Direction.Left:
                    --x;
                    ++y;
                    break;
                case Direction.Down:
                    ++x;
                    ++y;
                    break;
                case Direction.Up:
                    --x;
                    --y;
                    break;
            }
        }

        static void getStartZ(AEntity m, Map map, Position3D loc, List<Item> itemList, out int zLow, out int zTop)
        {
            int xCheck = loc.X, yCheck = loc.Y;
            var mapTile = map.GetMapTile(xCheck, yCheck);
            if (mapTile == null)
            {
                zLow = int.MinValue;
                zTop = int.MinValue;
            }
            var landBlocks = mapTile.Ground.LandData.IsImpassible; //(TileData.LandTable[landTile.ID & 0x3FFF].Flags & TileFlag.Impassable) != 0;
            // if (landBlocks && m.CanSwim && (TileData.LandTable[landTile.ID & 0x3FFF].Flags & TileFlag.Wet) != 0)
            //     landBlocks = false;
            // else if (m.CantWalk && (TileData.LandTable[landTile.ID & 0x3FFF].Flags & TileFlag.Wet) == 0)
            //     landBlocks = true;
            int landLow = 0, landCenter = 0, landTop = 0;
            landCenter = map.GetAverageZ(xCheck, yCheck, ref landLow, ref landTop);
            var considerLand = !mapTile.Ground.IsIgnored;
            var zCenter = zLow = zTop = 0;
            var isSet = false;
            if (considerLand && !landBlocks && loc.Z >= landCenter)
            {
                zLow = landLow;
                zCenter = landCenter;
                if (!isSet || landTop > zTop)
                    zTop = landTop;
                isSet = true;
            }
            var staticTiles = mapTile.GetStatics().ToArray();
            for (var i = 0; i < staticTiles.Length; ++i)
            {
                var tile = staticTiles[i];
                var calcTop = tile.Z + tile.ItemData.CalcHeight;
                if ((!isSet || calcTop >= zCenter) && ((tile.ItemData.Flags & TileFlag.Surface) != 0) && loc.Z >= calcTop)
                {
                    //  || (m.CanSwim && (id.Flags & TileFlag.Wet) != 0)
                    // if (m.CantWalk && (id.Flags & TileFlag.Wet) == 0)
                    //     continue;
                    zLow = tile.Z;
                    zCenter = calcTop;
                    var top = tile.Z + tile.ItemData.Height;
                    if (!isSet || top > zTop)
                        zTop = top;
                    isSet = true;
                }
            }
            for (var i = 0; i < itemList.Count; ++i)
            {
                var item = itemList[i];
                var id = item.ItemData;
                var calcTop = item.Z + id.CalcHeight;
                if ((!isSet || calcTop >= zCenter) && ((id.Flags & TileFlag.Surface) != 0) && loc.Z >= calcTop)
                {
                    //  || (m.CanSwim && (id.Flags & TileFlag.Wet) != 0)
                    // if (m.CantWalk && (id.Flags & TileFlag.Wet) == 0)
                    //     continue;
                    zLow = item.Z;
                    zCenter = calcTop;
                    var top = item.Z + id.Height;
                    if (!isSet || top > zTop)
                        zTop = top;
                    isSet = true;
                }
            }
            if (!isSet)
                zLow = zTop = loc.Z;
            else if (loc.Z > zTop)
                zTop = loc.Z;
        }
    }
}
