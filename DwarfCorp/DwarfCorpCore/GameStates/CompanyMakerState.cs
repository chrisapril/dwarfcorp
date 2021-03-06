﻿// CompanyMakerState.cs
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
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DwarfCorp.GameStates
{

    /// <summary>
    /// This game state allows the player to design their own dwarf company.
    /// </summary>
    public class CompanyMakerState : GameState
    {
        public DwarfGUI GUI { get; set; }
        public SpriteFont DefaultFont { get; set; }
        public Drawer2D Drawer { get; set; }
        public Panel MainWindow { get; set; }
        public int EdgePadding { get; set; }
        public GridLayout Layout { get; set; }
        public InputManager Input { get; set; }

        public ImagePanel CompanyLogoPanel { get; set; }

        private LineEdit CompanyNameEdit { get; set; }
        private LineEdit CompanyMottoEdit { get; set; }
        private ColorPanel CompanyColorPanel { get; set; }

        public TextGenerator TextGenerator { get; set; }
        public static Color DefaultColor = Color.DarkRed;
        public static string DefaultName = "Greybeard & Sons";
        public static string DefaultMotto = "My beard is in the work!";
        public static NamedImageFrame DefaultLogo = new NamedImageFrame(ContentPaths.Logos.grebeardlogo);
        public static string CompanyName { get; set; }
        public static string CompanyMotto { get; set; }
        public static NamedImageFrame CompanyLogo { get; set; }
        public static Color CompanyColor { get; set; }

        public CompanyMakerState(DwarfGame game, GameStateManager stateManager) :
            base(game, "CompanyMakerState", stateManager)
        {
            CompanyName = DefaultName;
            CompanyMotto = DefaultMotto;
            CompanyLogo = DefaultLogo;
            CompanyColor = DefaultColor;
            EdgePadding = 32;
            Input = new InputManager();
            Drawer = new Drawer2D(Game.Content, Game.GraphicsDevice);
            TextGenerator = new TextGenerator();
        }

        public override void OnEnter()
        {
            IsInitialized = true;
            DefaultFont = Game.Content.Load<SpriteFont>(ContentPaths.Fonts.Default);
            GUI = new DwarfGUI(Game, DefaultFont, Game.Content.Load<SpriteFont>(ContentPaths.Fonts.Title), Game.Content.Load<SpriteFont>(ContentPaths.Fonts.Small), Input);
            MainWindow = new Panel(GUI, GUI.RootComponent)
            {
                LocalBounds = new Rectangle(EdgePadding, EdgePadding, Game.GraphicsDevice.Viewport.Width - EdgePadding * 2, Game.GraphicsDevice.Viewport.Height - EdgePadding * 2)
            };
            Layout = new GridLayout(GUI, MainWindow, 10, 3);

            Label title = new Label(GUI, Layout, "Create a Company", GUI.TitleFont);
            Layout.SetComponentPosition(title, 0, 0, 1, 1);

            Label companyNameLabel = new Label(GUI, Layout, "Name", GUI.DefaultFont);
            Layout.SetComponentPosition(companyNameLabel, 0, 1, 1, 1);

            CompanyNameEdit = new LineEdit(GUI, Layout, CompanyName);
            Layout.SetComponentPosition(CompanyNameEdit, 1, 1, 1, 1);

            Button randomButton = new Button(GUI, Layout, "Random", GUI.DefaultFont, Button.ButtonMode.PushButton, null)
            {
                ToolTip = "Randomly generate a name"
            };
            Layout.SetComponentPosition(randomButton, 2, 1, 1, 1);
            randomButton.OnClicked += randomButton_OnClicked;

            Label companyMottoLabel = new Label(GUI, Layout, "Motto", GUI.DefaultFont);
            Layout.SetComponentPosition(companyMottoLabel, 0, 2, 1, 1);

            CompanyMottoEdit = new LineEdit(GUI, Layout, CompanyMotto);
            Layout.SetComponentPosition(CompanyMottoEdit, 1, 2, 1, 1);
            CompanyMottoEdit.OnTextModified += companyMottoEdit_OnTextModified;

            CompanyNameEdit.OnTextModified += companyNameEdit_OnTextModified;

            Button randomButton2 = new Button(GUI, Layout, "Random", GUI.DefaultFont, Button.ButtonMode.PushButton, null)
            {
                ToolTip = "Randomly generate a motto"
            };
            Layout.SetComponentPosition(randomButton2, 2, 2, 1, 1);
            randomButton2.OnClicked += randomButton2_OnClicked;

            Label companyLogoLabel = new Label(GUI, Layout, "Logo", GUI.DefaultFont);
            Layout.SetComponentPosition(companyLogoLabel, 0, 3, 1, 1);

            CompanyLogoPanel = new ImagePanel(GUI, Layout, CompanyLogo)
            {
                KeepAspectRatio = true,
                AssetName = CompanyLogo.AssetName
            };
            Layout.SetComponentPosition(CompanyLogoPanel, 1, 3, 1, 1);


            Button selectorButton = new Button(GUI, Layout, "Select", GUI.DefaultFont, Button.ButtonMode.PushButton, null)
            {
                ToolTip = "Load a custom company logo"
            };
            Layout.SetComponentPosition(selectorButton, 2, 3, 1, 1);
            selectorButton.OnClicked += selectorButton_OnClicked;

            Label companyColorLabel = new Label(GUI, Layout, "Color", GUI.DefaultFont);
            Layout.SetComponentPosition(companyColorLabel, 0, 4, 1, 1);

            CompanyColorPanel = new ColorPanel(GUI, Layout) {CurrentColor = DefaultColor};
            Layout.SetComponentPosition(CompanyColorPanel, 1, 4, 1, 1);
            CompanyColorPanel.OnClicked += CompanyColorPanel_OnClicked;


            Button apply = new Button(GUI, Layout, "Continue", GUI.DefaultFont, Button.ButtonMode.ToolButton, GUI.Skin.GetSpecialFrame(GUISkin.Tile.Check));
            Layout.SetComponentPosition(apply, 2, 9, 1, 1);

            apply.OnClicked += apply_OnClicked;

            Button back = new Button(GUI, Layout, "Back", GUI.DefaultFont, Button.ButtonMode.ToolButton, GUI.Skin.GetSpecialFrame(GUISkin.Tile.LeftArrow));
            Layout.SetComponentPosition(back, 1, 9, 1, 1);

            back.OnClicked += back_onClicked;

            base.OnEnter();
        }

        public List<Color> GenerateDefaultColors()
        {
            List<Color> toReturn = new List<Color>();
            for (int h = 0; h < 255; h += 16)
            {
                for (int v = 64; v < 255; v += 64)
                {
                    for (int s = 128; s < 255; s += 32)
                    {
                        toReturn.Add(new HSLColor((float) h, (float) s, (float) v));
                    }

                }
            }
        
            return toReturn;
        }

        void CompanyColorPanel_OnClicked()
        {
            ColorDialog colorDialog = ColorDialog.Popup(GUI, GenerateDefaultColors());
            colorDialog.OnColorSelected += colorDialog_OnColorSelected;
        }

        void colorDialog_OnColorSelected(Color arg)
        {
            CompanyColor = arg;
            CompanyColorPanel.CurrentColor = arg;
        }

        private void selectorButton_OnClicked()
        {
            List<SpriteSheet> sprites = new List<SpriteSheet>()
            {
                new SpriteSheet(ContentPaths.Logos.grebeardlogo, 32),
                new SpriteSheet(ContentPaths.Logos.logos, 32)
            };
            ImageFrameLoadDialog dialog = ImageFrameLoadDialog.Popup(GUI, sprites);
            dialog.OnTextureSelected += Loader_OnTextureSelected;
        }

        
        private void Loader_OnTextureSelected(NamedImageFrame arg)
        {
            CompanyLogo = arg;
            CompanyLogoPanel.Image = arg;
            CompanyLogoPanel.AssetName = arg.AssetName;
        }
         

        private void randomButton2_OnClicked()
        {
            List<List<string>> atoms = TextGenerator.GetAtoms(ContentPaths.Text.Templates.mottos);
            CompanyMotto = TextGenerator.GenerateRandom(Datastructures.SelectRandom(atoms).ToArray());
            CompanyMottoEdit.Text = CompanyMotto;
        }

        private void randomButton_OnClicked()
        {
            var templates = TextGenerator.GetAtoms(ContentPaths.Text.Templates.company_exploration);
            CompanyName = TextGenerator.GenerateRandom(Datastructures.SelectRandom(templates).ToArray());
            CompanyNameEdit.Text = CompanyName;
        }

        private void companyMottoEdit_OnTextModified(string arg)
        {
            CompanyMotto = arg;
        }

        private void apply_OnClicked()
        {
            CompanyName = CompanyNameEdit.Text;
            CompanyMotto = CompanyMottoEdit.Text;
            CompanyLogo = new NamedImageFrame(CompanyLogoPanel.AssetName, CompanyLogoPanel.Image.SourceRect);
            StateManager.PopState();
        }

        private void back_onClicked()
        {
            StateManager.PopState();
        }

        private void companyNameEdit_OnTextModified(string arg)
        {
            CompanyName = arg;
        }

        public override void Update(DwarfTime gameTime)
        {
            MainWindow.LocalBounds = new Rectangle(EdgePadding, EdgePadding, Game.GraphicsDevice.Viewport.Width - EdgePadding * 2, Game.GraphicsDevice.Viewport.Height - EdgePadding * 2);
            Input.Update();
            GUI.Update(gameTime);
            base.Update(gameTime);
        }


        private void DrawGUI(DwarfTime gameTime, float dx)
        {
            RasterizerState rasterizerState = new RasterizerState()
            {
                ScissorTestEnable = true
            };

            GUI.PreRender(gameTime, DwarfGame.SpriteBatch);
            DwarfGame.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, rasterizerState);
            Drawer.Render(DwarfGame.SpriteBatch, null, Game.GraphicsDevice.Viewport);
            GUI.Render(gameTime, DwarfGame.SpriteBatch, new Vector2(dx, 0));
            GUI.PostRender(gameTime);
            DwarfGame.SpriteBatch.End();
        }

        public override void Render(DwarfTime gameTime)
        {
            if(Transitioning == TransitionMode.Running)
            {
                Game.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
                DrawGUI(gameTime, 0);
            }
            else if(Transitioning == TransitionMode.Entering)
            {
                float dx = Easing.CubeInOut(TransitionValue, -Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Width, 1.0f);
                DrawGUI(gameTime, dx);
            }
            else if(Transitioning == TransitionMode.Exiting)
            {
                float dx = Easing.CubeInOut(TransitionValue, 0, Game.GraphicsDevice.Viewport.Width, 1.0f);
                DrawGUI(gameTime, dx);
            }


            base.Render(gameTime);
        }
    }

}