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

namespace netDxf.Objects
{

    /// <summary>
    /// Image display quality (screen only).
    /// </summary>
    public enum ImageDisplayQuality
    {
        /// <summary>
        /// Draft quality.
        /// </summary>
        Draft = 0,
        /// <summary>
        /// High quality.
        /// </summary>
        High = 1
    }

    /// <summary>
    /// AutoCAD units for inserting images.
    /// This is what one AutoCAD unit is equal to for the purpose of inserting and scaling images with an associated resolution.
    /// </summary>
    public enum ImageUnits
    {
        /// <summary>
        /// None.
        /// </summary>
        None = 0,
        /// <summary>
        /// Millimeters.
        /// </summary>
        Millimeters = 1,
        /// <summary>
        /// Centimeters.
        /// </summary>
        Centimeters = 2,
        /// <summary>
        /// Meters.
        /// </summary>
        Meters = 3,
        /// <summary>
        /// Kilometers.
        /// </summary>
        Kilometers = 4,
        /// <summary>
        /// Inches.
        /// </summary>
        Inches = 5,
        /// <summary>
        /// Feet.
        /// </summary>
        Feet = 6,
        /// <summary>
        /// Yards.
        /// </summary>
        Yards = 7,
        /// <summary>
        /// Miles.
        /// </summary>
        Miles = 8
    }

    /// <summary>
    /// Represents the variables applied to bitmaps.
    /// </summary>
    public class RasterVariables :
        DxfObject
    {

        #region private fields

        private bool displayFrame;
        private ImageDisplayQuality quality;
        private ImageUnits units;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>RasterVariables</c> class.
        /// </summary>
        public RasterVariables()
            : base(DxfObjectCode.RasterVariables)
        {
            this.displayFrame = true;
            this.quality = ImageDisplayQuality.High;
            this.units = ImageUnits.Millimeters;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets if the image frame is shown.
        /// </summary>
        public bool DisplayFrame
        {
            get { return displayFrame; }
            set { displayFrame = value; }
        }

        /// <summary>
        /// Gets or sets the image display quality (screen only).
        /// </summary>
        public ImageDisplayQuality DisplayQuality
        {
            get { return quality; }
            set { quality = value; }
        }

        /// <summary>
        /// Gets or sets the AutoCAD units for inserting images.
        /// </summary>
        /// <remarks>
        /// This is what one AutoCAD unit is equal to for the purpose of inserting and scaling images with an associated resolution.
        /// It is recommended to use the same units as the header variable $INSUNITS.
        /// </remarks>
        public ImageUnits Units
        {
            get { return units; }
            set { units = value; }
        }

        #endregion
    }
}
