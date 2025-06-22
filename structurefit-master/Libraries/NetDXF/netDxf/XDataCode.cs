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

namespace netDxf
{
    /// <summary>
    /// Defines the extended data code.
    /// </summary>
    public sealed class XDataCode
    {
        /// <summary>
        /// Strings in extended data can be up to 255 bytes long.
        /// </summary>
        public const int String = 1000;

        /// <summary>
        /// Application names can be up to 31 bytes long.
        /// </summary>
        internal const int AppReg = 1001;

        /// <summary>
        /// An extended data control string can be either �{�or �}�.
        /// These braces enable applications to organize their data by subdividing the data into lists.
        /// The left brace begins a list, and the right brace terminates the most recent list. Lists can be nested
        /// </summary>
        internal const int ControlString = 1002;

        /// <summary>
        /// Name of the layer associated with the extended data
        /// </summary>
        public const int LayerName = 1003;

        /// <summary>
        /// Binary data is organized into variable-length chunks.
        /// The maximum length of each chunk is 127 bytes.
        /// In ASCII DXF files, binary data is represented as a string of hexadecimal digits, two per binary byte
        /// </summary>
        public const int BinaryData = 1004;

        /// <summary>
        /// Handles of entities in the drawing database
        /// </summary>
        public const int DatabaseHandle = 1005;

        /// <summary>
        /// Three real values, in the order X, Y, Z.
        /// They can be used as a point or vector record. AutoCAD never alters their value
        /// </summary>
        public const int RealX = 1010;

        /// <summary>
        /// Three real values, in the order X, Y, Z.
        /// They can be used as a point or vector record. AutoCAD never alters their value
        /// </summary>
        public const int RealY = 1020;

        /// <summary>
        /// Three real values, in the order X, Y, Z.
        /// They can be used as a point or vector record. AutoCAD never alters their value
        /// </summary>
        public const int RealZ = 1030;

        /// <summary>
        /// Unlike a simple 3D point, the world space coordinates are moved, scaled, rotated, and mirrored 
        /// along with the parent entity to which the extended data belongs. 
        /// The world space position is also stretched when the STRETCH command is applied to the parent entity and
        /// this point lies within the select window
        /// </summary>
        public const int WorldSpacePositionX = 1011;

        /// <summary>
        /// Unlike a simple 3D point, the world space coordinates are moved, scaled, rotated, and mirrored 
        /// along with the parent entity to which the extended data belongs. 
        /// The world space position is also stretched when the STRETCH command is applied to the parent entity and
        /// this point lies within the select window
        /// </summary>
        public const int WorldSpacePositionY = 1021;

        /// <summary>
        /// Unlike a simple 3D point, the world space coordinates are moved, scaled, rotated, and mirrored 
        /// along with the parent entity to which the extended data belongs. 
        /// The world space position is also stretched when the STRETCH command is applied to the parent entity and
        /// this point lies within the select window
        /// </summary>
        public const int WorldSpacePositionZ = 1031;

        /// <summary>
        /// Also a 3D point that is scaled, rotated, and mirrored along with the parent (but is not moved or stretched)
        /// </summary>
        public const int WorldSpaceDisplacementX = 1012;

        /// <summary>
        /// Also a 3D point that is scaled, rotated, and mirrored along with the parent (but is not moved or stretched)
        /// </summary>
        public const int WorldSpaceDisplacementY = 1022;

        /// <summary>
        /// Also a 3D point that is scaled, rotated, and mirrored along with the parent (but is not moved or stretched)
        /// </summary>
        public const int WorldSpaceDisplacementZ = 1032;

        /// <summary>
        /// Also a 3D point that is rotated and mirrored along with the parent (but is not moved, scaled, or stretched)
        /// </summary>
        public const int WorldDirectionX = 1013;

        /// <summary>
        /// Also a 3D point that is rotated and mirrored along with the parent (but is not moved, scaled, or stretched)
        /// </summary>
        public const int WorldDirectionY = 1023;

        /// <summary>
        /// Also a 3D point that is rotated and mirrored along with the parent (but is not moved, scaled, or stretched)
        /// </summary>
        public const int WorldDirectionZ = 1033;

        /// <summary>
        /// A real value.
        /// </summary>
        public const int Real = 1040;

        /// <summary>
        /// A real value that is scaled along with the parent entity
        /// </summary>
        public const int Distance = 1041;

        /// <summary>
        /// Also a real value that is scaled along with the parent.
        /// The difference between a distance and a scale factor is application-defined
        /// </summary>
        public const int ScaleFactor = 1042;

        /// <summary>
        /// A 16-bit integer (signed or unsigned)
        /// </summary>
        public const int Integer = 1070;

        /// <summary>
        /// A 32-bit signed (long) integer
        /// </summary>
        public const int Long = 1071;
    }
}