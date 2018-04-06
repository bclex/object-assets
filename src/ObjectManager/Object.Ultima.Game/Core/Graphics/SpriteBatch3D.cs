using OA.Core;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Ultima.Core.Graphics
{
    public class SpriteBatch3D
    {
        const int MAX_VERTICES_PER_DRAW = 0x4000;
        const int INITIAL_TEXTURE_COUNT = 0x800;
        const float MAX_ACCURATE_SINGLE_FLOAT = 65536; // this number is somewhat arbitrary; it's the number at which the
        // difference between two subsequent integers is +/-0.005. See http://stackoverflow.com/questions/872544/precision-of-floating-point
        float _z;
        readonly object _game;
        //readonly Effect _effect;
        readonly short[] _indexBuffer;
        static Bounds _viewportArea;
        readonly Queue<List<VertexPositionNormalTextureHue>> _vertexListQueue;
        readonly List<Dictionary<Texture2D, List<VertexPositionNormalTextureHue>>> _drawQueue;
        readonly VertexPositionNormalTextureHue[] _vertexArray;

        public SpriteBatch3D(object game)
        {
            _game = game;
            _drawQueue = new List<Dictionary<Texture2D, List<VertexPositionNormalTextureHue>>>((int)Techniques.All);
            for (var i = 0; i <= (int)Techniques.All; i++)
                _drawQueue.Add(new Dictionary<Texture2D, List<VertexPositionNormalTextureHue>>(INITIAL_TEXTURE_COUNT));
            _indexBuffer = CreateIndexBuffer(MAX_VERTICES_PER_DRAW);
            _vertexArray = new VertexPositionNormalTextureHue[MAX_VERTICES_PER_DRAW];
            _vertexListQueue = new Queue<List<VertexPositionNormalTextureHue>>(INITIAL_TEXTURE_COUNT);
            //_effect = _game.Content.Load<Effect>("Shaders/IsometricWorld");
        }

        public object GraphicsDevice
        {
            get
            {
                if (_game == null)
                    return null;
                return null; // _game.GraphicsDevice;
            }
        }

        public Matrix4x4 ProjectionMatrixWorld
        {
            get { return Matrix4x4.identity; }
        }

        //public Matrix4x4 ProjectionMatrixScreen
        //{
        //    get { return Matrix4x4.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0f, short.MinValue, short.MaxValue); }
        //}

        public void Reset(bool setZHigh = false)
        {
            _z = setZHigh ? MAX_ACCURATE_SINGLE_FLOAT : 0;
            //_viewportArea = new Bounds(new Vector3(0, 0, int.MinValue), new Vector3(_game.GraphicsDevice.Viewport.Width, _game.GraphicsDevice.Viewport.Height, int.MaxValue));
        }

        public virtual float GetNextUniqueZ()
        {
            return _z++;
        }

        /// <summary>
        /// Draws a quad on screen with the specified texture and vertices.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="vertices"></param>
        /// <returns>True if the object was drawn, false otherwise.</returns>
        public bool DrawSprite(Texture2D texture, VertexPositionNormalTextureHue[] vertices, Techniques effect = Techniques.Default)
        {
            var draw = false;
            // Sanity: do not draw if there is no texture to draw with.
            if (texture == null)
                return false;
            // Check: only draw if the texture is within the visible area.
            for (var i = 0; i < 4; i++) // only draws a 2 triangle tristrip.
                //if (_viewportArea.Contains(vertices[i].Position) == ContainmentType.Contains)
                {
                    draw = true;
                    break;
                }
            if (!draw)
                return false;
            // Set the draw position's z value, and increment the z value for the next drawn object.
            vertices[0].Position.z = vertices[1].Position.z = vertices[2].Position.z = vertices[3].Position.z = GetNextUniqueZ();
            // Get the vertex list for this texture. if none exists, dequeue existing or create a new vertex list.
            var vertexList = GetVertexList(texture, effect);
            // Add the drawn object to the vertex list.
            for (var i = 0; i < vertices.Length; i++)
                vertexList.Add(vertices[i]);
            return true;
        }

        /// <summary>
        /// Draws a special 'shadow' sprite, automatically skewing the passed vertices.
        /// </summary>
        /// <param name="texture">The texture to draw with.</param>
        /// <param name="vertices">An array of four vertices. Note: modified by this routine.</param>
        /// <param name="drawPosition">The draw position at which this sprite begins (should be the center of an isometric tile for non-moving sprites).</param>
        /// <param name="flipVertices">See AEntityView.Draw(); this is equivalent to DrawFlip.</param>
        /// <param name="z">The z depth at which the shadow sprite should be placed.</param>
        public void DrawShadow(Texture2D texture, VertexPositionNormalTextureHue[] vertices, Vector2 drawPosition, bool flipVertices, float z)
        {
            // Sanity: do not draw if there is no texture to draw with.
            if (texture == null)
                return;
            // set proper z depth for this shadow.
            vertices[0].Position.z = vertices[1].Position.z = vertices[2].Position.z = vertices[3].Position.z = z;
            // skew texture
            var skewHorizTop = (vertices[0].Position.y - drawPosition.y) * .5f;
            var skewHorizBottom = (vertices[3].Position.y - drawPosition.y) * .5f;
            vertices[0].Position.x -= skewHorizTop;
            vertices[0].Position.y -= skewHorizTop;
            vertices[flipVertices ? 2 : 1].Position.x -= skewHorizTop;
            vertices[flipVertices ? 2 : 1].Position.y -= skewHorizTop;
            vertices[flipVertices ? 1 : 2].Position.x -= skewHorizBottom;
            vertices[flipVertices ? 1 : 2].Position.y -= skewHorizBottom;
            vertices[3].Position.x -= skewHorizBottom;
            vertices[3].Position.y -= skewHorizBottom;
            var vertexList = GetVertexList(texture, Techniques.ShadowSet);
            for (var i = 0; i < vertices.Length; i++)
                vertexList.Add(vertices[i]);
        }

        public void DrawStencil(Texture2D texture, VertexPositionNormalTextureHue[] vertices)
        {
            // Sanity: do not draw if there is no texture to draw with.
            if (texture == null)
                return;
            // set proper z depth for this shadow.
            vertices[0].Position.z = vertices[1].Position.z = vertices[2].Position.z = vertices[3].Position.z = GetNextUniqueZ();
        }

        public void FlushSprites(bool doLighting)
        {
            // set up graphics device and texture sampling.
            //GraphicsDevice.BlendState = BlendState.AlphaBlend;
            //GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            //GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp; // the sprite texture sampler.
            //GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp; // hue sampler (1/2)
            //GraphicsDevice.SamplerStates[2] = SamplerState.PointClamp; // hue sampler (2/2)
            //GraphicsDevice.SamplerStates[3] = SamplerState.PointWrap; // the minimap sampler.
            //// We use lighting parameters to shade vertexes when we're drawing the world.
            //_effect.Parameters["DrawLighting"].SetValue(doLighting);
            //// set up viewport.
            //_effect.Parameters["ProjectionMatrix"].SetValue(ProjectionMatrixScreen);
            //_effect.Parameters["WorldMatrix"].SetValue(ProjectionMatrixWorld);
            //_effect.Parameters["Viewport"].SetValue(new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));
            // enable depth sorting, disable the stencil
            //SetDepthStencilState(true, false);
            DrawAllVertices(Techniques.FirstDrawn, Techniques.LastDrawn);
        }

        private void DrawAllVertices(Techniques first, Techniques last)
        {
            // draw normal objects
            for (var effect = first; effect <= last; effect++)
            {
                //switch (effect)
                //{
                //    case Techniques.Hued: _effect.CurrentTechnique = _effect.Techniques["HueTechnique"]; break;
                //    case Techniques.MiniMap: _effect.CurrentTechnique = _effect.Techniques["MiniMapTechnique"]; break;
                //    case Techniques.Grayscale: _effect.CurrentTechnique = _effect.Techniques["GrayscaleTechnique"]; break;
                //    case Techniques.ShadowSet: _effect.CurrentTechnique = _effect.Techniques["ShadowSetTechnique"]; SetDepthStencilState(true, true); break;
                //    case Techniques.StencilSet: // do nothing; break;
                //    default: Utils.Critical("Unknown effect in SpriteBatch3D.Flush(). Effect index is {effect}"); break;
                //}
                //_effect.CurrentTechnique.Passes[0].Apply();
                var vertexEnumerator = _drawQueue[(int)effect].GetEnumerator();
                while (vertexEnumerator.MoveNext())
                {
                    var texture = vertexEnumerator.Current.Key;
                    var vertexList = vertexEnumerator.Current.Value;
                    //GraphicsDevice.Textures[0] = texture;
                    //GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, CopyVerticesToArray(vertexList), 0, Math.Min(vertexList.Count, MAX_VERTICES_PER_DRAW), _indexBuffer, 0, vertexList.Count / 2);
                    vertexList.Clear();
                    _vertexListQueue.Enqueue(vertexList);
                }
                _drawQueue[(int)effect].Clear();
            }
        }

        private VertexPositionNormalTextureHue[] CopyVerticesToArray(List<VertexPositionNormalTextureHue> vertices)
        {
            var max = vertices.Count <= MAX_VERTICES_PER_DRAW ? vertices.Count : MAX_VERTICES_PER_DRAW;
            vertices.CopyTo(0, _vertexArray, 0, max);
            return _vertexArray;
        }

        public void SetLightDirection(Vector3 direction)
        {
            //_effect.Parameters["lightDirection"].SetValue(direction);
        }

        public void SetLightIntensity(float intensity)
        {
            //_effect.Parameters["lightIntensity"].SetValue(intensity);
        }

        //private void SetDepthStencilState(bool depth, bool stencil)
        //{
        //    // depth is currently ignored.
        //    var dss = new DepthStencilState();
        //    dss.DepthBufferEnable = true;
        //    dss.DepthBufferWriteEnable = true;
        //    if (stencil)
        //    {
        //        dss.StencilEnable = true;
        //        dss.StencilFunction = CompareFunction.Equal;
        //        dss.ReferenceStencil = 0;
        //        dss.StencilPass = StencilOperation.Increment;
        //        dss.StencilFail = StencilOperation.Keep;
        //    }
        //    GraphicsDevice.DepthStencilState = dss;
        //}

        private List<VertexPositionNormalTextureHue> GetVertexList(Texture2D texture, Techniques effect)
        {
            List<VertexPositionNormalTextureHue> vertexList;
            if (_drawQueue[(int)effect].ContainsKey(texture))
                vertexList = _drawQueue[(int)effect][texture];
            else
            {
                if (_vertexListQueue.Count > 0)
                {
                    vertexList = _vertexListQueue.Dequeue();
                    vertexList.Clear();
                }
                else vertexList = new List<VertexPositionNormalTextureHue>(1024);
                _drawQueue[(int)effect].Add(texture, vertexList);
            }
            return vertexList;
        }

        private short[] CreateIndexBuffer(int primitiveCount)
        {
            var indices = new short[primitiveCount * 6];
            for (var i = 0; i < primitiveCount; i++)
            {
                indices[i * 6] = (short)(i * 4);
                indices[i * 6 + 1] = (short)(i * 4 + 1);
                indices[i * 6 + 2] = (short)(i * 4 + 2);
                indices[i * 6 + 3] = (short)(i * 4 + 2);
                indices[i * 6 + 4] = (short)(i * 4 + 1);
                indices[i * 6 + 5] = (short)(i * 4 + 3);
            }
            return indices;
        }
    }
}
