﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using DwarfCorp.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;


namespace DwarfCorp
{

    /// <summary>
    /// Simple class representing a geometric object with verticies, textures, and whatever else.
    /// </summary>
    [JsonObject(IsReference = true)]
    public class GeometricPrimitive
    {
        [JsonIgnore]
        public IndexBuffer IndexBuffer = null;

        public ExtendedVertex[] Vertices = null;

        [JsonIgnore]
        public VertexBuffer VertexBuffer = null;

        [JsonIgnore]
        private object VertexLock = new object();

        [OnDeserialized]
        protected void OnDeserialized(StreamingContext context)
        {
            ResetBuffer(GameState.Game.GraphicsDevice);
        }


        /// <summary>
        /// Draws the primitive to the screen.
        /// </summary>
        /// <param Name="device">GPU to draw with.</param>
        public virtual void Render(GraphicsDevice device)
        {
            lock (VertexLock)
            {
#if MONOGAME_BUILD
                device.SamplerStates[0].Filter = TextureFilter.Point;
                device.SamplerStates[1].Filter = TextureFilter.Point;
                device.SamplerStates[2].Filter = TextureFilter.Point;
                device.SamplerStates[3].Filter = TextureFilter.Point;
                device.SamplerStates[4].Filter = TextureFilter.Point;
#endif
                if (VertexBuffer == null)
                {
                    return;
                }

                if (Vertices == null || VertexBuffer == null || Vertices.Length < 3 ||  VertexBuffer.IsDisposed || VertexBuffer.VertexCount < 3)
                {
                    return;
                }

                device.SetVertexBuffer(VertexBuffer);

                if (IndexBuffer != null)
                {
                    device.Indices = IndexBuffer;
                    device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, VertexBuffer.VertexCount, 0, IndexBuffer.IndexCount / 3);
                }
                else
                {
                    device.DrawPrimitives(PrimitiveType.TriangleList, 0, Vertices.Length/3);
                }
            }
        }

        /// <summary>
        /// Draws the primitive to the screen.
        /// </summary>
        /// <param Name="device">GPU to draw with.</param>
        public virtual void RenderWireframe(GraphicsDevice device)
        {
            lock (VertexLock)
            {
                RasterizerState state = new RasterizerState();
                RasterizerState oldState = device.RasterizerState;
                state.FillMode = FillMode.WireFrame;
                device.RasterizerState = state;
                device.SetVertexBuffer(VertexBuffer);
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, Vertices.Length/3);
                device.RasterizerState = oldState;
            }
        }

        /// <summary>
        /// Resets the vertex buffer object from the verticies.
        /// <param Name="device">GPU to draw with.</param></summary>
        public virtual void ResetBuffer(GraphicsDevice device)
        {
            if(DwarfGame.ExitGame)
            {
                return;
            }

            lock (VertexLock)
            {
                if (Vertices == null || Vertices.Length <= 0 || device == null || device.IsDisposed)
                {
                    return;
                }

                if (VertexBuffer != null && !VertexBuffer.IsDisposed)
                {
                    VertexBuffer.Dispose();
                    VertexBuffer = null;
                }

            }

            VertexBuffer newBuff = new VertexBuffer(device, ExtendedVertex.VertexDeclaration, Vertices.Length,
                    BufferUsage.WriteOnly);
                newBuff.SetData(Vertices);


            lock (VertexLock)
            {
                VertexBuffer = newBuff;
            }

        }
    }

}