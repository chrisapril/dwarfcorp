﻿// Resource.cs
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
using DwarfCorp.GameStates;

namespace DwarfCorp
{
    /// <summary>
    /// A resource is a kind of item that can be bought or sold, and can be used
    /// to build things.
    /// </summary>
    public class Resource
    {
        public ResourceLibrary.ResourceType Type { get; set; }
        public string ResourceName { get { return ResourceLibrary.ResourceNames[Type]; }}
        public float MoneyValue { get; set; }
        public string Description { get; set; }
        public NamedImageFrame Image { get; set; }
        public List<ResourceTags> Tags { get; set; }
        public float FoodContent { get; set; }
        public bool SelfIlluminating { get; set; }
        public bool IsFlammable { get; set; }

        public enum ResourceTags
        {
            Food,
            Material,
            Precious
        }

        public Resource()
        {
            
        }

        public Resource(ResourceLibrary.ResourceType type, float money, string description, NamedImageFrame image, params ResourceTags[] tags)
        {
            Type = type;
            MoneyValue = money;
            Description = description;
            Image = image;
            Tags = new List<ResourceTags>();
            Tags.AddRange(tags);
            FoodContent = 0;
        }
    }

}