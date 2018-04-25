using OA.Core;
using OA.Ultima.Core.Graphics;
using OA.Ultima.Resources;
using OA.Ultima.World.Entities;
using OA.Ultima.World.Entities.Items;
using OA.Ultima.World.Entities.Mobiles;
using OA.Ultima.World.Input;
using OA.Ultima.World.Maps;
using System;
using UnityEngine;

namespace OA.Ultima.World.EntityViews
{
    /// <summary>
    /// An abstract class that can be attached to an entity and used to maintain data for a 'View'.
    /// </summary>
    public abstract class AEntityView
    {
        public static Techniques Technique = Techniques.Default;
        public readonly AEntity Entity;
        protected IResourceProvider Provider;

        public AEntityView(AEntity entity)
        {
            Entity = entity;
            SortZ = Entity.Z;
            Provider = Service.Get<IResourceProvider>();
        }

        public PickType PickType = PickType.PickNothing;

        // ============================================================================================================
        // Sort routines
        // ============================================================================================================

        public int SortZ;

        // ============================================================================================================
        // Draw methods and properties
        // ============================================================================================================

        public float Rotation;
        public static float PI = (float)Math.PI;

        protected bool DrawFlip;
        protected RectInt DrawArea = RectInt.zero;
        protected Texture2DInfo DrawTexture;
        protected Vector3 HueVector = Vector3.zero;

        protected bool IsShadowCastingView;
        protected float DrawShadowZDepth;

        public virtual bool Draw(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOver, Map map, bool roofHideFlag)
        {
            VertexPositionNormalTextureHue[] vertexBuffer;
            if (Rotation != 0)
            {
                var w = DrawArea.width / 2f;
                var h = DrawArea.height / 2f;
                var center = drawPosition - new Vector3(DrawArea.x - IsometricRenderer.TILE_SIZE_INTEGER + w, DrawArea.Y + h, 0);
                var sinx = (float)Math.Sin(Rotation) * w;
                var cosx = (float)Math.Cos(Rotation) * w;
                var siny = -(float)Math.Sin(Rotation) * h;
                var cosy = -(float)Math.Cos(Rotation) * h;
                // 2   0    
                // |\  |     
                // |  \|     
                // 3   1
                vertexBuffer = VertexPositionNormalTextureHue.PolyBufferFlipped;
                vertexBuffer[0].Position = center;
                vertexBuffer[0].Position.X += cosx - -siny;
                vertexBuffer[0].Position.Y -= sinx + -cosy;
                vertexBuffer[1].Position = center;
                vertexBuffer[1].Position.X += cosx - siny;
                vertexBuffer[1].Position.Y += -sinx + -cosy;
                vertexBuffer[2].Position = center;
                vertexBuffer[2].Position.X += -cosx - -siny;
                vertexBuffer[2].Position.Y += sinx + cosy;
                vertexBuffer[3].Position = center;
                vertexBuffer[3].Position.X += -cosx - siny;
                vertexBuffer[3].Position.Y += sinx + -cosy;
            }
            else if (DrawFlip)
            {
                // 2   0    
                // |\  |     
                // |  \|     
                // 3   1
                vertexBuffer = VertexPositionNormalTextureHue.PolyBufferFlipped;
                vertexBuffer[0].Position = drawPosition;
                vertexBuffer[0].Position.X += DrawArea.X + IsometricRenderer.TILE_SIZE_FLOAT;
                vertexBuffer[0].Position.Y -= DrawArea.Y;
                vertexBuffer[0].TextureCoordinate.Y = 0;
                vertexBuffer[1].Position = vertexBuffer[0].Position;
                vertexBuffer[1].Position.Y += DrawArea.Height;
                vertexBuffer[2].Position = vertexBuffer[0].Position;
                vertexBuffer[2].Position.X -= DrawArea.Width;
                vertexBuffer[2].TextureCoordinate.Y = 0;
                vertexBuffer[3].Position = vertexBuffer[1].Position;
                vertexBuffer[3].Position.X -= DrawArea.Width;

                /*if (m_YClipLine != 0)
                {
                    if (m_YClipLine > vertexBuffer[3].Position.Y)
                        return false;
                    else if (m_YClipLine > vertexBuffer[0].Position.Y)
                    {
                        float uvStart = (m_YClipLine - vertexBuffer[0].Position.Y) / DrawTexture.Height;
                        vertexBuffer[0].Position.Y = vertexBuffer[2].Position.Y = m_YClipLine;
                        vertexBuffer[0].TextureCoordinate.Y = vertexBuffer[2].TextureCoordinate.Y = uvStart;
                    }
                }*/
            }
            else
            {
                // 0---1    
                //    /     
                //  /       
                // 2---3
                vertexBuffer = VertexPositionNormalTextureHue.PolyBuffer;
                vertexBuffer[0].Position = drawPosition;
                vertexBuffer[0].Position.X -= DrawArea.X;
                vertexBuffer[0].Position.Y -= DrawArea.Y;
                vertexBuffer[0].TextureCoordinate.Y = 0;
                vertexBuffer[1].Position = vertexBuffer[0].Position;
                vertexBuffer[1].Position.X += DrawArea.Width;
                vertexBuffer[1].TextureCoordinate.Y = 0;
                vertexBuffer[2].Position = vertexBuffer[0].Position;
                vertexBuffer[2].Position.Y += DrawArea.Height;
                vertexBuffer[3].Position = vertexBuffer[1].Position;
                vertexBuffer[3].Position.Y += DrawArea.Height;
                /*if (m_YClipLine != 0)
                {
                    if (m_YClipLine >= vertexBuffer[3].Position.Y)
                        return false;
                    else if (m_YClipLine > vertexBuffer[0].Position.Y)
                    {
                        float uvStart = (m_YClipLine - vertexBuffer[0].Position.Y) / DrawTexture.Height;
                        vertexBuffer[0].Position.Y = vertexBuffer[1].Position.Y = m_YClipLine;
                        vertexBuffer[0].TextureCoordinate.Y = vertexBuffer[1].TextureCoordinate.Y = uvStart;
                    }
                }*/
            }
            if (vertexBuffer[0].Hue != HueVector)
                vertexBuffer[0].Hue = vertexBuffer[1].Hue = vertexBuffer[2].Hue = vertexBuffer[3].Hue = HueVector;
            if (!spriteBatch.DrawSprite(DrawTexture, vertexBuffer, Technique))
                return false; // the vertex buffer was not on screen, return false (did not draw)
            Pick(mouseOver, vertexBuffer);
            if (IsShadowCastingView)
            {
                spriteBatch.DrawShadow(DrawTexture, vertexBuffer, new Vector2(
                    drawPosition.x + IsometricRenderer.TILE_SIZE_FLOAT_HALF,
                    drawPosition.y + (Entity.Position.Offset.x + Entity.Position.Offset.y) * IsometricRenderer.TILE_SIZE_FLOAT_HALF - ((Entity.Position.Z_offset + Entity.Z) * 4) + IsometricRenderer.TILE_SIZE_FLOAT_HALF),
                    DrawFlip, DrawShadowZDepth);
            }
            return true;
        }

        /// <summary>
        /// Used by DeferredView to draw an object without first determining if it should be deferred.
        /// Should only be implemented for those views that call CheckDefer(), Otherwise, using only
        /// Draw() will suffice. See MobileView for an example of use.
        /// </summary>
        public virtual bool DrawInternal(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOver, Map map, bool roofHideFlag)
        {
            return false;
        }

        /// <summary>
        /// Draws all overheads, starting at [yOffset] pixels above the Entity's anchor point on the ground.
        /// </summary>
        /// <param name="yOffset"></param>
        public void DrawOverheads(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOver, Map map, int yOffset)
        {
            for (int i = 0; i < Entity.Overheads.Count; i++)
            {
                AEntityView view = Entity.Overheads[i].GetView();
                view.DrawArea = new RectInt((view.DrawTexture.width / 2) - 22, yOffset + view.DrawTexture.height, view.DrawTexture.width, view.DrawTexture.height);
                OverheadsView.AddView(view, drawPosition);
                yOffset += view.DrawTexture.height;
            }
        }

        protected virtual void Pick(MouseOverList mouseOver, VertexPositionNormalTextureHue[] vertexBuffer)
        {
            // override this method if the view should be pickable.
        }

        // ============================================================================================================
        // Y Clipping (used during deferred draws.
        // ============================================================================================================

        protected int _yClipLine;

        /// <summary>
        /// Between the time when this is called and when ClearYClipLine() is called, this view will only draw sprites below
        /// the specified y line.
        /// </summary>
        /// <param name="y"></param>
        public virtual void SetYClipLine(float y)
        {
            _yClipLine = (int)y;
        }

        public virtual void ClearYClipLine()
        {
            _yClipLine = 0;
        }

        // ============================================================================================================
        // Deferred drawing code
        // ============================================================================================================

        protected void CheckDefer(Map map, Vector3 drawPosition)
        {
            MapTile deferToTile;
            Direction checkDirection;
            if (Entity is Mobile && ((Mobile)Entity).IsMoving)
            {
                var facing = (Entity as Mobile).DrawFacing;
                if (
                    ((facing & Direction.FacingMask) == Direction.Left) ||
                    ((facing & Direction.FacingMask) == Direction.South) ||
                    ((facing & Direction.FacingMask) == Direction.East))
                {
                    deferToTile = map.GetMapTile(Entity.Position.X, Entity.Position.Y + 1);
                    checkDirection = facing & Direction.FacingMask;
                }
                else if ((facing & Direction.FacingMask) == Direction.Down)
                {
                    deferToTile = map.GetMapTile(Entity.Position.X + 1, Entity.Position.Y + 1);
                    checkDirection = Direction.Down;
                }
                else
                {
                    deferToTile = map.GetMapTile(Entity.Position.X + 1, Entity.Position.Y);
                    checkDirection = Direction.East;
                }
            }
            else
            {
                deferToTile = map.GetMapTile(Entity.Position.X, Entity.Position.Y + 1);
                checkDirection = Direction.South;
            }
            if (deferToTile != null)
            {
                if (Entity is Mobile)
                {
                    var mobile = Entity as Mobile;
                    // This calculates the z position of the mobile as if it had moved into the next tile.
                    // Strictly speaking, this isn't necessary, but looks nice for mobiles that are walking.
                    var z = MobileMovementCheck.GetNextZ(mobile, Entity.Position, checkDirection); 
                    var deferred = new DeferredEntity(mobile, drawPosition, z);
                    deferToTile.OnEnter(deferred);
                }
                else
                {
                    var deferred = new DeferredEntity(Entity, drawPosition, Entity.Z);
                    deferToTile.OnEnter(deferred);
                }
            }
        }

        protected bool CheckUnderSurface(Map map, int x, int y)
        {
            return UnderSurfaceCheck(map, x, y) && UnderSurfaceCheck(map, x + 1, y + 1) && UnderSurfaceCheck(map, x + 2, y + 2);
        }

        bool UnderSurfaceCheck(Map map, int x, int y)
        {
            MapTile tile;
            AEntity e0, e1;
            if ((tile = map.GetMapTile(x, y)) != null)
            {
                if (tile == null)
                    return false;
                if (tile.IsZUnderEntityOrGround(Entity.Position.Z, out e0, out e1))
                {
                    if (e0 == null || !(e0 is Item))
                        return false;
                    var item = e0 as Item;
                    if (item.ItemData.IsRoof || item.ItemData.IsSurface || (item.ItemData.IsWall && !item.ItemData.IsDoor))
                        return true;
                }
            }
            return false;
        }
    }
}
