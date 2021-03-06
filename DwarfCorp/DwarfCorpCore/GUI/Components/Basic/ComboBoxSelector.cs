﻿// ComboBoxSelector.cs
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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DwarfCorp
{

    /// <summary>
    /// This GUI component is the list found inside the ComboBox
    /// </summary>
    public class ComboBoxSelector : GUIComponent
    {
        public delegate void Modified(string arg);

        public event Modified OnSelectionModified;

        private List<string> Values { get; set; }
        private List<List<string>> Columns { get; set; }
        public string CurrentValue { get; set; }
        private int PixelsPerValue { get; set; }
        private ComboBox Box { get; set; }
        private Timer ClickTimer { get; set; }
        public bool Drawn { get; set; }
        public bool IsDead = false;
        public float MaxHeight = 500;
        private int ColumnWidth = 0;

        public int GetCurrentIndex()
        {
            return Values.IndexOf(CurrentValue);
        }

        public ComboBoxSelector(DwarfGUI gui, ComboBox parent, List<string> values, string currentValue) :
            base(gui, parent)
        {
            Columns = new List<List<string>>();
            Values = values;
            CurrentValue = currentValue;

            int height = 0;
            Columns.Add(new List<string>());
            int currentColumn = 0;
            int columnWidth = parent.GlobalBounds.Width - 37;
            ColumnWidth = columnWidth;
            int bestHeight = 0;
            foreach(string s in values)
            {
                List<string> column = Columns[currentColumn];
                Vector2 measure = Datastructures.SafeMeasure(GUI.DefaultFont, s);
                height += (int) measure.Y;
                column.Add(s);

                if(height > bestHeight)
                {
                    bestHeight = height;
                }

                if(height >= MaxHeight)
                {
                    height = 0;
                    Columns.Add(new List<string>());
                    currentColumn++;
                    columnWidth += ColumnWidth;
                }
            }

            List<List<string>> removals = new List<List<string>>();

            foreach(List<string> column in Columns)
            {
                if(column.Count == 0)
                {
                    removals.Add(column);
                }
            }

            foreach(List<string> column in removals)
            {
                Columns.Remove(column);
                columnWidth -= ColumnWidth;
            }

            bestHeight += 15;

            LocalBounds = new Rectangle(0, parent.GlobalBounds.Height / 2 + GUI.Skin.TileHeight / 2, columnWidth, bestHeight);
            Box = parent;

            ClickTimer = new Timer(0.1f, true, Timer.TimerMode.Real);
            InputManager.MouseClickedCallback += InputManager_MouseClickedCallback;
            Drawn = true;
        }

        private Vector2 MeasureColumn(List<string> column)
        {
            Vector2 toReturn = Vector2.Zero;
            foreach(string s in column)
            {
                toReturn.Y += Datastructures.SafeMeasure(GUI.DefaultFont, s).Y;
                toReturn.X = (float) Math.Max(toReturn.X, Datastructures.SafeMeasure(GUI.DefaultFont, s).X);
            }
            return toReturn;
        }


        private void InputManager_MouseClickedCallback(InputManager.MouseButton button)
        {
            if(ClickTimer.HasTriggered && !IsDead)
            {
                OnSelectionModified.Invoke(CurrentValue);
                Parent.RemoveChild(this);
                Box.Selector = null;
                IsDead = true;
                InputManager.MouseClickedCallback -= InputManager_MouseClickedCallback;
            }
        }


        public override void Update(DwarfTime time)
        {
            ClickTimer.Update(time);
            if(IsMouseOver && !IsDead)
            {
                //GUI.FocusComponent = Parent;
                MouseState mouse = Mouse.GetState();

                float normalizedX = Math.Min(Math.Max(((float) mouse.X - (float) GlobalBounds.X) / GlobalBounds.Width, 0), 1.0f);
                int nearestColumn = (int) (normalizedX * Columns.Count);

                if(nearestColumn >= 0 && nearestColumn < Columns.Count)
                {
                    Vector2 colMeasure = MeasureColumn(Columns[nearestColumn]);
                    float normalizedY = Math.Min(Math.Max(((float) mouse.Y - (float) GlobalBounds.Y) / colMeasure.Y, 0), 1.0f);
                    int nearestRow = (int) (normalizedY * Columns[nearestColumn].Count);

                    if(nearestRow >= 0 && nearestRow < Columns[nearestColumn].Count)
                    {
                        CurrentValue = Columns[nearestColumn][nearestRow];
                        Box.CurrentValue = CurrentValue;
                    }
                }
            }
            else
            {
                //GUI.FocusComponent = null;
            }


            base.Update(time);
        }


        public override void Render(DwarfTime time, Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
        {
            if(Drawn)
            {
                if(!GUI.DrawAfter.Contains(this))
                {
                    GUI.DrawAfter.Add(this);
                    Drawn = false;
                }
            }
            else
            {
                GUI.Skin.RenderButton(GlobalBounds, batch);


                int x = 0;
                foreach(List<string> column in Columns)
                {
                    if(column.Count == 0)
                    {
                        continue;
                    }

                    float columnMeasure = MeasureColumn(column).Y;
                    PixelsPerValue = (int) columnMeasure / column.Count;
                    int h = 0;

                    foreach(string s in column)
                    {
                        Vector2 measure = Datastructures.SafeMeasure(GUI.DefaultFont, s);

                        Color c = Color.Black;

                        if(s == CurrentValue)
                        {
                            c = Color.DarkRed;
                        }

                        Drawer2D.DrawAlignedText(batch, s, GUI.DefaultFont, c, Drawer2D.Alignment.Left, new Rectangle(GlobalBounds.X + 10 + x, GlobalBounds.Y + h + 5, GlobalBounds.Width, (int) measure.Y + 5));

                        h += PixelsPerValue;
                    }

                    x += ColumnWidth;
                }
                Drawn = true;
                base.Render(time, batch);
            }
        }
    }

}