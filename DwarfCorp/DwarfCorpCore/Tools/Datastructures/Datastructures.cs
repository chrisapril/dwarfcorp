﻿// Datastructures.cs
// 
//  Modified MIT License (MIT)
//  
//  Copyright (c) 2015 Completely Fair Games Ltd.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// The following content pieces are considered PROPRIETARY and may not be used
// in any derivative works, commercial or non commercial, without explicit 
// written permission from Completely Fair Games:
// 
// * Images (sprites, textures, etc.)
// * 3D Models
// * Sound Effects
// * Music
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using DwarfCorp.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DwarfCorp
{

    /// <summary>
    /// Honestly, this is just a helper class where a bunch of other miscellanious
    /// stuff is thrown at this time. Most of it has to do with utilities for certain
    /// data structures (such as 2D or 3D arrays).
    /// </summary>
    internal class Datastructures
    {
        public static Vector2 SafeMeasure(SpriteFont font, string text)
        {
            Vector2 extents = Vector2.One;

            if(text == null)
            {
                return extents;
            }

            try
            {
                extents = font.MeasureString(text);
            }
            catch(ArgumentException e)
            {
                Console.Error.WriteLine(e.Message);
                extents.X = text.Length * 20;
            }

            return extents;
        }

        public static EventWaitHandle WaitFor(WaitHandle[] waitHandles)
        {
            int iHandle = WaitHandle.WaitAny(waitHandles, 500);

            if (iHandle == System.Threading.WaitHandle.WaitTimeout)
            {
                return null;
            }

            if (iHandle > 0 && iHandle < waitHandles.Length)
            {
                EventWaitHandle wh = (EventWaitHandle) waitHandles[iHandle];

                return wh;   
            }

            return null;
        }

        public static List<int> RandomIndices(int max)
        {
            List<int> toReturn = new List<int>(max);
            List<int> indices = new List<int>(max);

            for(int i = 0; i < max; i++)
            {
                indices.Add(i);
            }

            for(int i = 0; i < max; i++)
            {
                int r = PlayState.Random.Next(indices.Count);

                toReturn.Add(indices[r]);
                indices.RemoveAt(r);
            }

            return toReturn;
        }

        public static IEnumerable<TKey> RandomKeys<TKey, TValue>(IDictionary<TKey, TValue> dict)
        {
            Random rand = new Random();
            List<TKey> values = Enumerable.ToList(dict.Keys);


            int size = dict.Count;

            if(size > 0)
            {
                while(true)
                {
                    yield return values[rand.Next(size)];
                }
            }
        }

        public static T SelectRandom<T>(IEnumerable<T> list)
        {
            var enumerable = list as IList<T> ?? list.ToList();
            return enumerable.ElementAt(PlayState.Random.Next(enumerable.Count()));
        }

        public static T[,] RotateClockwise<T>(T[,] A)
        {
            int nr = A.GetLength(0);
            int nc = A.GetLength(1);

            T[,] toReturn = new T[nc, nr];

            for(int r = 0; r < nc; r++)
            {
                for(int c = 0; c < nr; c++)
                {
                    toReturn[r, c] = A[c, r];
                }
            }

            for(int r = 0; r < nc; r++)
            {
                for(int c = 0; c < nr / 2; c++)
                {
                    Swap(ref toReturn[r, c], ref toReturn[r, nr - c - 1]);
                }
            }

            return toReturn;
        }

        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }
    }

}