#region netDxf, Copyright(C) 2013 Daniel Carvajal, Licensed under LGPL.

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

namespace netDxf.Header
{
    /// <summary>
    /// Strings system variables
    /// </summary>
    internal static class HeaderVariableCode
    {
        /// <summary>
        /// The AutoCAD drawing database version number.
        /// </summary>
        public const string AcadVer = "$ACADVER";

        /// <summary>
        /// Next available handle.
        /// </summary>
        public const string HandleSeed = "$HANDSEED";

        /// <summary>
        /// Angle 0 direction.
        /// </summary>
        public const string Angbase = "$ANGBASE";

        /// <summary>
        /// 1 = Clockwise angles, 0 = Counterclockwise.
        /// </summary>
        public const string Angdir = "$ANGDIR";

        /// <summary>
        /// Attribute visibility.
        /// </summary>
        public const string AttMode = "$ATTMODE";

        /// <summary>
        /// Units format for angles.
        /// </summary>
        public const string AUnits = "$AUNITS";

        /// <summary>
        /// Units precision for angles.
        /// </summary>
        public const string AUprec = "$AUPREC";

        /// <summary>
        /// Current entity color.
        /// </summary>
        public const string CeColor = "$CECOLOR";

        /// <summary>
        /// Current entity linetype scale.
        /// </summary>
        public const string CeLtScale = "$CELTSCALE";

        /// <summary>
        /// Current entity lineweight.
        /// </summary>
        public const string CeLweight = "$CELWEIGHT";

        /// <summary>
        /// Current entity linetype name.
        /// </summary>
        public const string CeLtype = "$CELTYPE";

        /// <summary>
        /// Current layer name.
        /// </summary>
        public const string CLayer = "$CLAYER";

        /// <summary>
        /// Current multiline justification.
        /// </summary>
        public const string CMLJust = "$CMLJUST";

        /// <summary>
        /// Current multiline scale.
        /// </summary>
        public const string CMLScale = "$CMLSCALE";

        /// <summary>
        /// Current multiline style name.
        /// </summary>
        public const string CMLStyle = "$CMLSTYLE";

        /// <summary>
        /// Current dimension style name
        /// </summary>
        public const string DimStyle = "$DIMSTYLE";

        /// <summary>
        /// Default text height.
        /// </summary>
        public const string TextSize = "$TEXTSIZE";

        /// <summary>
        /// Current text style name.
        /// </summary>
        public const string TextStyle = "$TEXTSTYLE";

        /// <summary>
        /// Units format for coordinates and distances.
        /// </summary>
        public const string LUnits = "$LUNITS";

        /// <summary>
        /// Units precision for coordinates and distances.
        /// </summary>
        public const string LUprec = "$LUPREC";

        /// <summary>
        /// Drawing code page; set to the system code page when a new drawing is created, but not otherwise maintained by AutoCAD.
        /// </summary>
        public const string DwgCodePage = "$DWGCODEPAGE";

        /// <summary>
        /// Controls symbol table naming.
        /// </summary>
        /// <remarks>
        /// Controls symbol table naming:<br />
        /// 0 = Release 14 compatibility. Limits names to 31 characters in length.<br />
        /// Names can include the letters A to Z, the numerals 0 to 9,
        /// and the special characters dollar sign ($), underscore (_), and hyphen (-).<br />
        /// 1 = AutoCAD 2000.<br />
        /// Names can be up to 255 characters in length, 
        /// and can include the letters A to Z, the numerals 0 to 9, spaces, 
        /// and any special characters not used for other purposes by Microsoft Windows and AutoCAD.
        /// </remarks>
        public const string Extnames = "$EXTNAMES";

        /// <summary>
        /// Default drawing units for AutoCAD DesignCenter blocks.
        /// </summary>
        /// <remarks>
        /// Also applies to raster image units, eventhought they have the RasterVariables object and units in ImageDef.
        /// </remarks>
        public const string Insunits = "$INSUNITS";

        /// <summary>
        /// User name that saved the file.
        /// </summary>
        public const string LastSavedBy = "$LASTSAVEDBY";

        /// <summary>
        /// Controls the display of lineweights on the Model or Layout tab.
        /// </summary>
        /// <remarks>
        /// 0 = Lineweight is not displayed
        /// 1 = Lineweight is displayed
        /// </remarks>
        public const string LwDisplay = "$LWDISPLAY";


        /// <summary>
        /// Global linetype scale.
        /// </summary>
        public const string LtScale = "$LTSCALE";

        /// <summary>
        /// Controls the <see cref="PointShape">shape</see> to draw a point entity.
        /// </summary>
        public const string PdMode = "$PDMODE";

        /// <summary>
        /// Controls the size of the point figures, except for PDMODE values 0 (Dot) and 1 (Empty).
        /// </summary>
        /// <remarks>
        /// A setting of 0 generates the point at 5 percent of the drawing area height.<br />
        /// A positive PDSIZE value specifies an absolute size for the point figures.<br />
        /// A negative value is interpreted as a percentage of the viewport size.<br />
        /// </remarks>
        public const string PdSize = "$PDSIZE";

        /// <summary>
        /// Governs the generation of linetype patterns around the vertices of a 2D polyline.
        /// </summary>
        /// <remarks>
        /// 1 = Linetype is generated in a continuous pattern around vertices of the polyline.<br />
        /// 0 = Each segment of the polyline starts and ends with a dash.
        /// </remarks>
        public const string PLineGen = "$PLINEGEN";
    }
}