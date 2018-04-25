using OA.Core;
using OA.Ultima.Core.Graphics;
using OA.Ultima.World.Maps;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Ultima.World.WorldViews
{
    public class MiniMapTexture
    {
        uint[] _textureData;
        uint[] _blockColors;
        MiniMapChunk[] _blockCache;

        public Texture2DInfo Texture { get; private set; }

        SpriteBatchUI _spriteBatch;

        bool _mustRedrawEntireTexture;
        uint _lastCenterCellX, _lastCenterCellY;

        List<uint> _queuedToDrawBlocks;

        const uint Stride = 256;
        const uint BlockCacheWidth = 48, BlockCacheHeight = 48;
        const uint TilesPerBlock = 64;

        public void Initialize()
        {
            _spriteBatch = Service.Get<SpriteBatchUI>();
            Texture = new Texture2DInfo((int)Stride, (int)Stride);
            _textureData = new uint[Stride * Stride];
            _blockColors = new uint[TilesPerBlock];
            _blockCache = new MiniMapChunk[BlockCacheWidth * BlockCacheHeight];
            _mustRedrawEntireTexture = true;
            _queuedToDrawBlocks = new List<uint>();
        }

        public void Update(Map map, Position3D center)
        {
            var centerCellX = (uint)center.X / 8;
            var centerCellY = (uint)center.Y / 8;
            var centerDiffX = (int)(centerCellX - _lastCenterCellX);
            var centerDiffY = (int)(centerCellY - _lastCenterCellY);
            _lastCenterCellX = centerCellX;
            _lastCenterCellY = centerCellY;
            if (centerDiffX < -1 || centerDiffX > 1 || centerDiffY < -1 || centerDiffY > 1)
                _mustRedrawEntireTexture = true;
            if (_mustRedrawEntireTexture)
            {
                var firstX = centerCellX - 15;
                var firstY = centerCellY;
                for (uint y = 0; y < 32; y++)
                    for (uint x = 0; x < 16; x++)
                        InternalQueueMapBlock(map, firstX + ((y + 1) / 2) + x, firstY + (y / 2) - x);
                _mustRedrawEntireTexture = false;
            }
            else if (centerDiffX != 0 || centerDiffY != 0)
            {
                // draw just enough of the minimap to cover the newly exposed area.
                if (centerDiffX < 0)
                {
                    if (centerDiffY <= 0)
                    {
                        // traveling UP/WEST, draw new rows.
                        var firstX = centerCellX - 15;
                        var firstY = centerCellY;
                        for (uint y = 0; y < 2; y++)
                            for (uint x = 0; x < 16; x++)
                                InternalQueueMapBlock(map, firstX + x + ((y + 1) / 2), firstY - x + (y / 2));
                    }
                    if (centerDiffY >= 0)
                    {
                        // traveling LEFT/WEST, draw a new column.
                        var firstX = centerCellX - 15;
                        var firstY = centerCellY + 0;
                        for (uint y = 0; y < 32; y++)
                            InternalQueueMapBlock(map, firstX + ((y + 1) / 2), firstY + (y / 2));
                    }
                }
                else if (centerDiffX > 0)
                {
                    if (centerDiffY <= 0)
                    {
                        // traveling RIGHT/EAST, draw a new column.
                        var firstX = centerCellX + 0;
                        var firstY = centerCellY - 15;
                        for (uint y = 0; y < 32; y++)
                            InternalQueueMapBlock(map, firstX + ((y + 1) / 2), firstY + (y / 2));
                    }

                    if (centerDiffY >= 0)
                    {
                        // travelling DOWN/EAST, draw new rows.
                        var firstX = centerCellX + 0;
                        var firstY = centerCellY + 15;
                        for (uint y = 0; y < 2; y++)
                            for (uint x = 0; x < 16; x++)
                                InternalQueueMapBlock(map, firstX + ((y + 1) / 2) + x, firstY - x + (y / 2));
                    }
                }
                else if (centerDiffY != 0)
                {
                    if (centerDiffY < 0)
                    {
                        // traveling NORTH, draw a new row and column.
                        var firstX = centerCellX - 15;
                        var firstY = centerCellY + 0;
                        for (uint y = 0; y < 2; y++)
                            for (uint x = 0; x < 16; x++)
                                InternalQueueMapBlock(map, firstX + x + ((y + 1) / 2), firstY - x + (y / 2));
                        firstX = centerCellX + 0;
                        firstY = centerCellY - 15;
                        for (uint y = 0; y < 32; y++)
                            InternalQueueMapBlock(map, firstX + ((y + 1) / 2), firstY + (y / 2));
                    }
                    else if (centerDiffY > 0)
                    {
                        // traveling SOUTH, draw a new row and column.
                        var firstX = centerCellX - 15;
                        var firstY = centerCellY + 0;
                        for (uint y = 0; y < 32; y++)
                            InternalQueueMapBlock(map, firstX + ((y + 1) / 2), firstY + (y / 2));
                        firstX = centerCellX - 0;
                        firstY = centerCellY + 15;
                        for (uint y = 0; y < 2; y++)
                            for (uint x = 0; x < 16; x++)
                                InternalQueueMapBlock(map, firstX + ((y + 1) / 2) + x, firstY - x + (y / 2));
                    }
                }
            }
            if (_queuedToDrawBlocks.Count > 0)
            {
                InternalDrawQueuedMapBlocks();
                //_spriteBatch.GraphicsDevice.Textures[3] = null;
                //Texture.SetData(_textureData);
                //_spriteBatch.GraphicsDevice.Textures[3] = Texture;
            }
        }

        private void InternalQueueMapBlock(Map map, uint cellx, uint celly)
        {
            var chunkIndex = (cellx % BlockCacheWidth) + (celly % BlockCacheHeight) * BlockCacheWidth;
            var chunk = _blockCache[chunkIndex];
            if (chunk == null || chunk.X != cellx || chunk.Y != celly)
            {
                // the chunk is not in our cache! Try loading it from the map?
                var mapBlock = map.GetMapChunk(cellx, celly);
                // if the chunk is not loaded in memory, load it from the filesystem.
                if (mapBlock == null) _blockCache[chunkIndex] = new MiniMapChunk(cellx, celly, map.MapData);
                // get the colors for this chunk from the map chunk, which will have already sorted the objects.
                else _blockCache[chunkIndex] = new MiniMapChunk(mapBlock);
            }
            else _blockColors = chunk.Colors;
            _queuedToDrawBlocks.Add(chunkIndex);
        }

        private void InternalDrawQueuedMapBlocks()
        {
            var chunks = _queuedToDrawBlocks.GetEnumerator();
            for (var i = 0; i < _queuedToDrawBlocks.Count; i++)
            {
                var chunk = _blockCache[_queuedToDrawBlocks[i]];
                uint cellX32 = chunk.X % 32, cellY32 = chunk.Y % 32;
                _blockColors = chunk.Colors;
                // now draw the chunk
                if (cellX32 == 0 && cellY32 == 0)
                {
                    // draw the chunk split out over four corners of the texture.
                    var chunkindex = 0;
                    for (uint tiley = 0; tiley < 8; tiley++)
                    {
                        var drawy = (cellX32 * 8 + cellY32 * 8 - 8 + tiley) % Stride;
                        var drawx = (cellX32 * 8 - cellY32 * 8 - tiley) % Stride;
                        for (var tilex = 0; tilex < 8; tilex++)
                        {
                            var color = _blockColors[chunkindex++];
                            _textureData[drawy * Stride + drawx] = color;
                            if (drawy == 255) _textureData[drawx] = color;
                            else _textureData[drawy * Stride + Stride + drawx] = color;
                            drawx = (drawx + 1) % Stride;
                            drawy = (drawy + 1) % Stride;
                        }
                    }
                }
                else if (cellX32 + cellY32 == 32)
                {
                    // draw the chunk split on the top and bottom of the texture.
                    var chunkindex = 0;
                    for (var tiley = 0; tiley < 8; tiley++)
                    {
                        var drawy = (cellX32 * 8 + cellY32 * 8 - 8 + tiley) % Stride;
                        var drawx = (cellX32 * 8 - cellY32 * 8 - tiley) % Stride;
                        for (var tilex = 0; tilex < 8; tilex++)
                        {
                            var color = _blockColors[chunkindex++];
                            _textureData[drawy * Stride + drawx] = color;
                            if (drawy == 255) _textureData[drawx] = color;
                            else _textureData[drawy * Stride + Stride + drawx] = color;
                            drawx = (drawx + 1) % Stride;
                            drawy = (drawy + 1) % Stride;
                        }
                    }
                }
                else if (cellX32 == cellY32)
                {
                    // draw the chunk split on the left and right side of the texture.
                    var chunkindex = 0;
                    for (var tiley = 0; tiley < 8; tiley++)
                    {
                        var drawy = (cellX32 * 8 + cellY32 * 8 - 8 + tiley) % Stride;
                        var drawx = (cellX32 * 8 - cellY32 * 8 - tiley) % Stride;
                        for (var tilex = 0; tilex < 8; tilex++)
                        {
                            var color = _blockColors[chunkindex++];
                            _textureData[drawy * Stride + drawx] = color;
                            _textureData[drawy * Stride + Stride + drawx] = color;
                            drawx = (drawx + 1) % Stride;
                            drawy = (drawy + 1) % Stride;
                        }
                    }
                }
                else
                {
                    // draw the chunk normally.
                    var chunkindex = 0;
                    for (var tiley = 0; tiley < 8; tiley++)
                    {
                        var drawy = (cellX32 * 8 + cellY32 * 8 - 8 + tiley) % Stride;
                        var drawx = (cellX32 * 8 - cellY32 * 8 - tiley) % Stride;
                        for (var tilex = 0; tilex < 8; tilex++)
                        {
                            var color = _blockColors[chunkindex++];
                            _textureData[drawy * Stride + drawx] = color;
                            _textureData[drawy * Stride + Stride + drawx] = color;
                            drawx = (drawx + 1) % Stride;
                            drawy = (drawy + 1) % Stride;
                        }
                    }
                }
            }
            _queuedToDrawBlocks.Clear();
        }
    }
}
