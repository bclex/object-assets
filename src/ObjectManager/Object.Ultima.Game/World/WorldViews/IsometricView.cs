using OA.Core;
using OA.Ultima.Core.Graphics;
using OA.Ultima.World.Entities;
using OA.Ultima.World.Entities.Items;
using OA.Ultima.World.Input;
using OA.Ultima.World.Maps;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Ultima.World.WorldViews
{
    public class IsometricRenderer
    {
        public const float TILE_SIZE_FLOAT = 44.0f;
        public const float TILE_SIZE_FLOAT_HALF = 22.0f;
        public const int TILE_SIZE_INTEGER = 44;
        public const int TILE_SIZE_INTEGER_HALF = 22;

        /// <summary>
        /// The number of entities drawn in the previous frame.
        /// </summary>
        public int CountEntitiesRendered { get; private set; }

        /// <summary>
        /// The texture of the last drawn frame.
        /// </summary>
        public Texture2D Texture
        {
            get { return _renderTargetSprites; }
        }

        public IsometricLighting Lighting { get; private set; }

        Texture2D _renderTargetSprites;
        SpriteBatch3D _spriteBatch;
        bool _drawTerrain = true;
        bool _underSurface;
        int _drawMaxItemAltitude;
        Vector2 _drawOffset;

        public IsometricRenderer()
        {
            _spriteBatch = Service.Get<SpriteBatch3D>();
            Lighting = new IsometricLighting();
        }

        public void Update(Map map, Position3D center, MousePicking mousePick)
        {
            var pixelScale = UltimaGameSettings.UserInterface.PlayWindowPixelDoubling ? 2 : 1;
            if (_renderTargetSprites == null || _renderTargetSprites.width != UltimaGameSettings.UserInterface.PlayWindowGumpResolution.Width / pixelScale || _renderTargetSprites.height != UltimaGameSettings.UserInterface.PlayWindowGumpResolution.Height / pixelScale)
            {
                if (_renderTargetSprites != null)
                    _renderTargetSprites.Dispose();
                //_renderTargetSprites = new RenderTarget2D(
                //    _spriteBatch.GraphicsDevice,
                //    UltimaGameSettings.UserInterface.PlayWindowGumpResolution.Width / pixelScale,
                //    UltimaGameSettings.UserInterface.PlayWindowGumpResolution.Height / pixelScale,
                //    false,
                //    SurfaceFormat.Color,
                //    DepthFormat.Depth24Stencil8,
                //    0,
                //    RenderTargetUsage.DiscardContents);
            }
            DetermineIfClientIsUnderEntity(map, center);
            DrawEntities(map, center, mousePick, out _drawOffset);
        }

        private void DetermineIfClientIsUnderEntity(Map map, Position3D center)
        {
            // Are we inside (under a roof)? Do not draw tiles above our head.
            _drawMaxItemAltitude = 255;
            _drawTerrain = true;
            _underSurface = false;
            MapTile tile;
            if ((tile = map.GetMapTile(center.X, center.Y)) != null)
            {
                if (tile.IsZUnderEntityOrGround(center.Z, out AEntity underObject, out AEntity underTerrain))
                {
                    // if we are under terrain, then do not draw any terrain at all.
                    _drawTerrain = (underTerrain == null);
                    if (!(underObject == null))
                    {
                        // Roofing and new floors ALWAYS begin at intervals of 20.
                        // if we are under a ROOF, then get rid of everything above me.Z + 20
                        // (this accounts for A-frame roofs). Otherwise, get rid of everything
                        // at the object above us.Z.
                        if (underObject is Item)
                        {
                            var item = (Item)underObject;
                            if (item.ItemData.IsRoof) _drawMaxItemAltitude = center.Z - (center.Z % 20) + 20;
                            else if (item.ItemData.IsSurface || (item.ItemData.IsWall && !item.ItemData.IsDoor)) _drawMaxItemAltitude = item.Z;
                            else
                            {
                                int z = center.Z + ((item.ItemData.Height > 20) ? item.ItemData.Height : 20);
                                _drawMaxItemAltitude = z;
                            }
                        }
                        // If we are under a roof tile, do not make roofs transparent if we are on an edge.
                        if (underObject is Item && ((Item)underObject).ItemData.IsRoof)
                        {
                            var isRoofSouthEast = true;
                            if ((tile = map.GetMapTile(center.X + 1, center.Y)) != null)
                            {
                                tile.IsZUnderEntityOrGround(center.Z, out underObject, out underTerrain);
                                isRoofSouthEast = !(underObject == null);
                            }
                            if (!isRoofSouthEast)
                                _drawMaxItemAltitude = 255;
                        }
                        _underSurface = (_drawMaxItemAltitude != 255);
                    }
                }
            }
        }

        private void DrawEntities(Map map, Position3D center, MousePicking mousePicking, out Vector2 renderOffset)
        {
            if (center == null)
            {
                renderOffset = new Vector2();
                return;
            }
            // reset the spritebatch Z
            _spriteBatch.Reset();
            // set the lighting variables.
            _spriteBatch.SetLightIntensity(Lighting.IsometricLightLevel);
            _spriteBatch.SetLightDirection(Lighting.IsometricLightDirection);
            // get variables that describe the tiles drawn in the viewport: the first tile to draw,
            // the offset to that tile, and the number of tiles drawn in the x and y dimensions.
            var overDrawTilesOnSides = 3;
            var overDrawTilesAtTopAndBottom = 6;
            var overDrawAdditionalTilesOnBottom = 10;
            CalculateViewport(center, overDrawTilesOnSides, overDrawTilesAtTopAndBottom, out Vector2Int firstTile, out renderOffset, out Vector2Int renderDimensions);
            CountEntitiesRendered = 0; // Count of objects rendered for statistics and debug
            var overList = new MouseOverList(mousePicking); // List of entities mouse is over.
            var deferredToRemove = new List<AEntity>();
            for (var y = 0; y < renderDimensions.y * 2 + 1 + overDrawAdditionalTilesOnBottom; y++)
            {
                var drawPosition = new Vector3
                {
                    x = (firstTile.x - firstTile.y + (y % 2)) * TILE_SIZE_FLOAT_HALF + renderOffset.x,
                    y = (firstTile.x + firstTile.y + y) * TILE_SIZE_FLOAT_HALF + renderOffset.y
                };
                var firstTileInRow = new Vector2Int(firstTile.x + ((y + 1) / 2), firstTile.y + (y / 2));
                for (var x = 0; x < renderDimensions.x + 1; x++)
                {
                    var tile = map.GetMapTile(firstTileInRow.x - x, firstTileInRow.y + x);
                    if (tile == null)
                    {
                        drawPosition.x -= TILE_SIZE_FLOAT;
                        continue;
                    }
                    var entities = tile.Entities;
                    var draw = true;
                    for (var i = 0; i < entities.Count; i++)
                    {
                        if (entities[i] is DeferredEntity)
                            deferredToRemove.Add(entities[i]);
                        if (!_drawTerrain)
                            if ((entities[i] is Ground) || (entities[i].Z > tile.Ground.Z))
                                draw = false;
                        if ((entities[i].Z >= _drawMaxItemAltitude || (_drawMaxItemAltitude != 255 && entities[i] is Item && (entities[i] as Item).ItemData.IsRoof)) && !(entities[i] is Ground))
                            continue;
                        if (draw)
                        {
                            var view = entities[i].GetView();
                            if (view != null)
                                if (view.Draw(_spriteBatch, drawPosition, overList, map, !_underSurface))
                                    CountEntitiesRendered++;
                        }
                    }
                    foreach (var deferred in deferredToRemove)
                        tile.OnExit(deferred);
                    deferredToRemove.Clear();
                    drawPosition.x -= TILE_SIZE_FLOAT;
                }
            }
            OverheadsView.Render(_spriteBatch, overList, map, _underSurface);
            // Update the MouseOver objects
            mousePicking.UpdateOverEntities(overList, mousePicking.Position);
            // Draw the objects we just send to the spritebatch.
            //_spriteBatch.GraphicsDevice.SetRenderTarget(_renderTargetSprites);
            //_spriteBatch.GraphicsDevice.Clear(Color.black);
            _spriteBatch.FlushSprites(true);
            //_spriteBatch.GraphicsDevice.SetRenderTarget(null);
        }

        private void CalculateViewport(Position3D center, int overDrawTilesOnSides, int overDrawTilesOnTopAndBottom, out Vector2Int firstTile, out Vector2 renderOffset, out Vector2Int renderDimensions)
        {
            var pixelScale = (UltimaGameSettings.UserInterface.PlayWindowPixelDoubling) ? 2 : 1;
            renderDimensions = new Vector2Int
            {
                y = UltimaGameSettings.UserInterface.PlayWindowGumpResolution.Height / pixelScale / TILE_SIZE_INTEGER + overDrawTilesOnTopAndBottom, // the number of tiles that are drawn for half the screen (doubled to fill the entire screen).
                x = UltimaGameSettings.UserInterface.PlayWindowGumpResolution.Width / pixelScale / TILE_SIZE_INTEGER + overDrawTilesOnSides // the number of tiles that are drawn in the x-direction ( + renderExtraColumnsAtSides * 2 ).
            };
            var renderDimensionsDiff = Math.Abs(renderDimensions.x - renderDimensions.y);
            renderDimensionsDiff -= renderDimensionsDiff % 2; // make sure this is an even number...
            // when the player entity is at a higher z altitude in the world, we must offset the first row drawn so that tiles at lower altitudes are drawn.
            // The reverse is not true - at lower altitutdes, higher tiles are never on screen. This is an artifact of UO's isometric projection.
            // Note: The value of this variable MUST be a multiple of 2 and MUST be positive.
            var firstZOffset = center.Z > 0 ? (int)Math.Abs(((center.Z + center.Z_offset) / 11)) : 0;
            // this is used to draw tall objects that would otherwise not be visible until their ground tile was on screen. This may still skip VERY tall objects (those weird jungle trees?)
            firstTile = new Vector2Int(center.X - firstZOffset, center.Y - renderDimensions.y - firstZOffset);
            if (renderDimensions.y > renderDimensions.x)
            {
                firstTile.x -= renderDimensionsDiff / 2;
                firstTile.y -= renderDimensionsDiff / 2;
            }
            else
            {
                firstTile.x += renderDimensionsDiff / 2;
                firstTile.y -= renderDimensionsDiff / 2;
            }
            renderOffset.x = (((UltimaGameSettings.UserInterface.PlayWindowGumpResolution.Width / pixelScale) + ((renderDimensions.y) * TILE_SIZE_INTEGER)) / 2) - TILE_SIZE_FLOAT_HALF;
            renderOffset.x -= (int)((center.X_offset - center.Y_offset) * TILE_SIZE_FLOAT_HALF);
            renderOffset.x -= (firstTile.x - firstTile.y) * TILE_SIZE_FLOAT_HALF;
            renderOffset.x += renderDimensionsDiff * TILE_SIZE_FLOAT_HALF;
            renderOffset.y = ((UltimaGameSettings.UserInterface.PlayWindowGumpResolution.Height / pixelScale) / 2 - (renderDimensions.y * TILE_SIZE_INTEGER / 2));
            renderOffset.y += ((center.Z + center.Z_offset) * 4);
            renderOffset.y -= (int)((center.X_offset + center.Y_offset) * TILE_SIZE_FLOAT_HALF);
            renderOffset.y -= (firstTile.x + firstTile.y) * TILE_SIZE_FLOAT_HALF;
            renderOffset.y -= TILE_SIZE_FLOAT_HALF;
            renderOffset.y -= firstZOffset * TILE_SIZE_FLOAT;
        }
    }
}