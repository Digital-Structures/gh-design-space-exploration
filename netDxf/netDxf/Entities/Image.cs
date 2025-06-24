﻿#region netDxf, Copyright(C) 2013 Daniel Carvajal, Licensed under LGPL.

//                        netDxf library
// Copyright (C) 2013 Daniel Carvajal (haplokuon@gmail.com)
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 

#endregion

using System;
using netDxf.Objects;

namespace netDxf.Entities
{

    /// <summary>
    /// Represents a raster image <see cref="EntityObject">entity</see>.
    /// </summary>
    public class Image :
        EntityObject 
    {
        #region private fields

        private Vector3 position;
        private double width;
        private double height;
        private double rotation;
        private ImageDef imageDef;
        private bool clipping;
        private float brightness;
        private float contrast;
        private float fade;
        private ImageDisplayFlags displayOptions;
        private ImageClippingBoundary clippingBoundary;

        #endregion

        #region contructors

        internal Image()
            : base(EntityType.Image, DxfObjectCode.Image)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Image</c> class.
        /// </summary>
        /// <param name="imageDefinition">Image definition.</param>
        /// <param name="position">Image <see cref="Vector3">position</see> in world coordinates.</param>
        public Image(ImageDef imageDefinition, Vector3 position)
            : base(EntityType.Image, DxfObjectCode.Image)
        {
            this.imageDef = imageDefinition;
            this.position = position;
            this.width = imageDefinition.Width * imageDefinition.OnePixelSize.X;
            this.height = imageDefinition.Height * imageDefinition.OnePixelSize.Y;
            this.rotation = 0;
            this.clipping = false;
            this.brightness = 50.0f;
            this.contrast = 50.0f;
            this.fade = 0.0f;
            this.displayOptions = ImageDisplayFlags.ShowImage | ImageDisplayFlags.ShowImageWhenNotAlignedWithScreen | ImageDisplayFlags.UseClippingBoundary;
            this.clippingBoundary = new ImageClippingBoundary(-0.5, -0.5, imageDefinition.Width, imageDefinition.Height);
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the image <see cref="Vector3">position</see> in world coordinates.
        /// </summary>
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// Gets or sets the height of the image in drawing units.
        /// </summary>
        public double Height
        {
            get { return height; }
            internal set { height = value; }
        }

        /// <summary>
        /// Gets or sets the width of the image in drawing units.
        /// </summary>
        public double Width
        {
            get { return width; }
            internal set { width = value; }
        }

        /// <summary>
        /// Gets or sets the image rotation in degrees.
        /// </summary>
        public double Rotation
        {
            get { return this.rotation; }
            set { this.rotation = value; }
        }

        /// <summary>
        /// Gets the <see cref="ImageDef">image definition</see>.
        /// </summary>
        public ImageDef Definition
        {
            get { return imageDef; }
            internal set { imageDef = value; }
        }

        /// <summary>
        /// Gets or sets the clipping state: false = off, true = on.
        /// </summary>
        public bool Clipping
        {
            get { return clipping; }
            set { clipping = value; }
        }

        /// <summary>
        /// Gets or sets the brightness value (0-100; default = 50)
        /// </summary>
        public float Brightness
        {
            get { return brightness; }
            set
            {
                if (value < 0 && value > 100)
                    throw new ArgumentOutOfRangeException("value", "Accepted brightness values range from 0 to 100.");
                brightness = value;
            }
        }

        /// <summary>
        /// Gets or sets the contrast value (0-100; default = 50)
        /// </summary>
        public float Contrast
        {
            get { return contrast; }
            set
            {
                if (value < 0 && value > 100)
                    throw new ArgumentOutOfRangeException("value", "Accepted contrast values range from 0 to 100.");
                contrast = value;
            }
        }

        /// <summary>
        /// Gets or sets the fade value (0-100; default = 0)
        /// </summary>
        public float Fade
        {
            get { return fade; }
            set
            {
                if (value < 0 && value > 100)
                    throw new ArgumentOutOfRangeException("value", "Accepted fade values range from 0 to 100.");
                fade = value;
            }
        }

        /// <summary>
        /// Gets or sets the image display options.
        /// </summary>
        public ImageDisplayFlags DisplayOptions
        {
            get { return displayOptions; }
            set { displayOptions = value; }
        }

        /// <summary>
        /// Gets or sets the image clipping boundary.
        /// </summary>
        /// <remarks>
        /// Set as null to restore the default clipping boundary.
        /// </remarks>
        public ImageClippingBoundary ClippingBoundary
        {
            get { return clippingBoundary; }
            set { clippingBoundary = value ?? new ImageClippingBoundary(-0.5, -0.5, this.imageDef.Width, this.imageDef.Height); }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Sets the scale of the image.
        /// </summary>
        /// <param name="scale">X and Y scale of the image.</param>
        public void SetScale(Vector2 scale)
        {
            SetScale(scale.X, scale.Y);
        }
        /// <summary>
        /// Sets the scale of the image.
        /// </summary>
        /// <param name="scale">Uniform scale of the image.</param>
        public void SetScale(double scale)
        {
            SetScale(scale, scale);
        }
        /// <summary>
        /// Sets the scale of the image.
        /// </summary>
        /// <param name="scaleX">X scale of the image.</param>
        /// <param name="scaleY">Y scale of the image.</param>
        public void SetScale(double scaleX, double scaleY)
        {
            this.width = this.imageDef.Width * this.imageDef.OnePixelSize.X * scaleX;
            this.height = this.imageDef.Height * this.imageDef.OnePixelSize.Y * scaleY;
        }
        #endregion

    }
}
