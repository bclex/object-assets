using OA.Ultima.Core;
using OA.Ultima.Core.Graphics;
using OA.Ultima.World.Entities;
using OA.Ultima.World.Input;
using OA.Ultima.World.Maps;
using UnityEngine;

namespace OA.Ultima.World.EntityViews
{
    public class GroundView : AEntityView
    {
        new Ground Entity
        {
            get { return (Ground)base.Entity; }
        }

        bool _drawAs3DStretched;
        bool _noDraw;

        public GroundView(Ground ground)
            : base(ground)
        {
            PickType = PickType.PickGroundTiles;
            _noDraw = (Entity.LandDataID < 3 || (Entity.LandDataID >= 0x1AF && Entity.LandDataID <= 0x1B5));
            DrawFlip = false;
            if (Entity.LandData.TextureID <= 0)
            {
                _drawAs3DStretched = false;
                DrawTexture = Provider.GetLandTexture(Entity.LandDataID);
                DrawArea = new RectInt(0, Entity.Z * 4, IsometricRenderer.TILE_SIZE_INTEGER, IsometricRenderer.TILE_SIZE_INTEGER);
            }
            else
            {
                _drawAs3DStretched = true;
                DrawTexture = Provider.GetTexmapTexture(Entity.LandData.TextureID);
            }
        }

        protected override void Pick(MouseOverList mouseOver, VertexPositionNormalTextureHue[] vertexBuffer)
        {
            // TODO: This is called when the tile is not stretched - just drawn as a 44x44 tile.
            // Because this is not written, no flat tiles can ever be picked.
        }

        public override bool Draw(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOver, Map map, bool roofHideFlag)
        {
            if (_noDraw)
                return false;
            if (_mustUpdateSurroundings)
            {
                updateSurroundingsAndNormals(Entity.Map);
                _mustUpdateSurroundings = false;
            }
            if (!_drawAs3DStretched)
                return base.Draw(spriteBatch, drawPosition, mouseOver, map, roofHideFlag);
            return Draw3DStretched(spriteBatch, drawPosition, mouseOver, map);
        }

        Vector3 _vertex0_yOffset, _vertex1_yOffset, _vertex2_yOffset, _vertex3_yOffset;
        VertexPositionNormalTextureHue[] _vertexBufferAlternate = {
            new VertexPositionNormalTextureHue(new Vector3(), new Vector3(),  new Vector3(0, 0, 0)),
            new VertexPositionNormalTextureHue(new Vector3(), new Vector3(),  new Vector3(1, 0, 0)),
            new VertexPositionNormalTextureHue(new Vector3(), new Vector3(),  new Vector3(0, 1, 0)),
            new VertexPositionNormalTextureHue(new Vector3(), new Vector3(),  new Vector3(1, 1, 0))
        };

        bool Draw3DStretched(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOver, Map map)
        {
            // this is an isometric stretched tile and needs a specialized draw routine.
            _vertexBufferAlternate[0].Position = drawPosition + _vertex0_yOffset;
            _vertexBufferAlternate[1].Position = drawPosition + _vertex1_yOffset;
            _vertexBufferAlternate[2].Position = drawPosition + _vertex2_yOffset;
            _vertexBufferAlternate[3].Position = drawPosition + _vertex3_yOffset;
            if (!spriteBatch.DrawSprite(DrawTexture, _vertexBufferAlternate, Technique))
                return false;
            if ((mouseOver.PickType & PickType) == PickType)
                if (mouseOver.IsMouseInObjectIsometric(_vertexBufferAlternate))
                    mouseOver.AddItem(Entity, _vertexBufferAlternate[0].Position);
            return true;
        }

        bool _mustUpdateSurroundings = true;
        Surroundings _surroundingTiles;
        Vector3[] _normals = new Vector3[4];

        void updateVertexBuffer()
        {
            _vertex0_yOffset = new Vector3(IsometricRenderer.TILE_SIZE_INTEGER_HALF, -(Entity.Z * 4), 0);
            _vertex1_yOffset = new Vector3(IsometricRenderer.TILE_SIZE_FLOAT, IsometricRenderer.TILE_SIZE_INTEGER_HALF - (_surroundingTiles.East * 4), 0);
            _vertex2_yOffset = new Vector3(0, IsometricRenderer.TILE_SIZE_INTEGER_HALF - (_surroundingTiles.South * 4), 0);
            _vertex3_yOffset = new Vector3(IsometricRenderer.TILE_SIZE_INTEGER_HALF, IsometricRenderer.TILE_SIZE_FLOAT - (_surroundingTiles.Down * 4), 0);
            _vertexBufferAlternate[0].Normal = _normals[0];
            _vertexBufferAlternate[1].Normal = _normals[1];
            _vertexBufferAlternate[2].Normal = _normals[2];
            _vertexBufferAlternate[3].Normal = _normals[3];
            var hue = Utility.GetHueVector(Entity.Hue);
            if (_vertexBufferAlternate[0].Hue != hue)
            {
                _vertexBufferAlternate[0].Hue =
                _vertexBufferAlternate[1].Hue =
                _vertexBufferAlternate[2].Hue =
                _vertexBufferAlternate[3].Hue = hue;
            }
        }

        static Vector2Int[] kSurroundingsIndexes = {
            new Vector2Int(0, -1), new Vector2Int(1, -1),
            new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(2, 0),
            new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1),
            new Vector2Int(0, 2), new Vector2Int(1, 2) };

        void updateSurroundingsAndNormals(Map map)
        {
            var origin = new Vector2Int(Entity.Position.X, Entity.Position.Y);
            var surroundingTilesZ = new float[kSurroundingsIndexes.Length];
            for (var i = 0; i < kSurroundingsIndexes.Length; i++)
                surroundingTilesZ[i] = map.GetTileZ(origin.x + kSurroundingsIndexes[i].x, origin.y + kSurroundingsIndexes[i].y);
            _surroundingTiles = new Surroundings(surroundingTilesZ[7], surroundingTilesZ[3], surroundingTilesZ[6]);
            var isFlat = _surroundingTiles.IsFlat && _surroundingTiles.East == Entity.Z;
            if (!isFlat)
            {
                int low = 0, high = 0, sort = 0;
                sort = map.GetAverageZ(Entity.Z, (int)_surroundingTiles.South, (int)_surroundingTiles.East, (int)_surroundingTiles.Down, ref low, ref high);
                if (sort != SortZ)
                {
                    SortZ = sort;
                    map.GetMapTile(Entity.Position.X, Entity.Position.Y).ForceSort();
                }
            }
            _normals[0] = calculateNormal(
                surroundingTilesZ[2], surroundingTilesZ[3],
                surroundingTilesZ[0], surroundingTilesZ[6]);
            _normals[1] = calculateNormal(
                Entity.Z, surroundingTilesZ[4],
                surroundingTilesZ[1], surroundingTilesZ[7]);
            _normals[2] = calculateNormal(
                surroundingTilesZ[5], surroundingTilesZ[7],
                Entity.Z, surroundingTilesZ[9]);
            _normals[3] = calculateNormal(
                surroundingTilesZ[6], surroundingTilesZ[8],
                surroundingTilesZ[3], surroundingTilesZ[10]);
            updateVertexBuffer();
        }

        public static float Y_Normal = 1f;

        Vector3 calculateNormal(float A, float B, float C, float D)
        {
            var iVector = new Vector3((A - B), Y_Normal, (C - D));
            iVector.Normalize();
            return iVector;
        }

        class Surroundings
        {
            public float Down;
            public float East;
            public float South;

            public Surroundings(float down, float east, float south)
            {
                Down = down;
                East = east;
                South = south;
            }

            public bool IsFlat
            {
                get { return Down == East && East == South; }
            }
        }
    }
}
