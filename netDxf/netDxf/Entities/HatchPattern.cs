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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace netDxf.Entities
{

    /// <summary>
    /// Predefined hatch pattern name.
    /// </summary>
    internal sealed class PredefinedHatchPatternName
    {
        /// <summary>
        /// Solid.
        /// </summary>
        public const string Solid = "SOLID";
        /// <summary>
        /// Line.
        /// </summary>
        public const string Line = "LINE";
        /// <summary>
        /// Net.
        /// </summary>
        public const string Net = "NET";
        /// <summary>
        /// Dots.
        /// </summary>
        public const string Dots = "DOTS";
    }

    /// <summary>
    /// Hatch pattern style.
    /// </summary>
    public enum HatchStyle
    {
        /// <summary>
        /// Hatch “odd parity” area.
        /// </summary>
        Normal = 0,
        /// <summary>
        /// Hatch outermost area only.
        /// </summary>
        Outer = 1,
        /// <summary>
        /// Hatch through entire area.
        /// </summary>
        Ignore = 2
    }

    /// <summary>
    /// Hatch pattern type.
    /// </summary>
    public enum HatchType
    {
        /// <summary>
        /// User defined.
        /// </summary>
        UserDefined = 0,
        /// <summary>
        /// Predefined.
        /// </summary>
        Predefined = 1,
        /// <summary>
        /// Custom.
        /// </summary>
        Custom = 2
    }

    /// <summary>
    /// Hatch pattern fill type.
    /// </summary>
    public enum FillType
    {  
        /// <summary>
        /// Pattern fill.
        /// </summary>
        PatternFill = 0,
        /// <summary>
        /// Solid fill.
        /// </summary>
        SolidFill = 1    
    }

    /// <summary>
    /// Represents a <see cref="Hatch">hatch</see> pattern style.
    /// </summary>
    public class HatchPattern
    {

        #region private fields

        private readonly string name;
        private HatchStyle style;
        private FillType fill;
        private HatchType type;
        private double angle;
        private double scale;
        private string description;
        private List<HatchPatternLineDefinition> lineDefinitions;

        #endregion

        #region constructor

        /// <summary>
        /// Initializes a new instance of the <c>HatchPattern</c> class.
        /// </summary>
        /// <param name="name">Pattern name, always stored as uppercase.</param>
        /// <param name="description">Description of the pattern (optional, this information is not saved in the dxf file). By default it will use the supplied name.</param>
        public HatchPattern(string name, string description = null)
        {
            this.name = name.ToUpper();
            this.description = string.IsNullOrEmpty(description) ? name : description;
            this.style = HatchStyle.Normal;
            this.fill = this.name == PredefinedHatchPatternName.Solid ? FillType.SolidFill : FillType.PatternFill;
            this.type = HatchType.UserDefined;
            this.angle = 0.0;
            this.scale = 1.0;
            this.lineDefinitions = new List<HatchPatternLineDefinition>();
        }

        #endregion

        #region predefined patterns

        /// <summary>
        /// Solid hatch pattern.
        /// </summary>
        /// <remarks>The predefined pattern values are based on the acad.pat file of AutoCAD.</remarks>
        public static HatchPattern Solid
        {
            get
            {
                HatchPattern pattern = new HatchPattern(PredefinedHatchPatternName.Solid, "Solid fill") { type = HatchType.Predefined };
                // this is the pattern line definition for solid fills as defined in the acad.pat, but it is not needed
                //HatchPatternLineDefinition lineDefinition = new HatchPatternLineDefinition
                //                                                {
                //                                                    Angle = 45,
                //                                                    Origin = Vector2.Zero,
                //                                                    Delta = new Vector2(0.0, 0.125)
                //                                                };
                //pattern.LineDefinitions.Add(lineDefinition);
                return pattern;
            }
        }

        /// <summary>
        /// Lines hatch pattern.
        /// </summary>
        /// <remarks>The predefined pattern values are based on the acad.pat file of AutoCAD.</remarks>
        public static HatchPattern Line
        {
            get
            {
                HatchPattern pattern = new HatchPattern(PredefinedHatchPatternName.Line, "Parallel horizontal lines");
                HatchPatternLineDefinition lineDefinition = new HatchPatternLineDefinition
                                                                {
                                                                    Angle = 0,
                                                                    Origin = Vector2.Zero,
                                                                    Delta = new Vector2(0.0, 0.125)
                                                                };
                pattern.LineDefinitions.Add(lineDefinition);
                pattern.type = HatchType.Predefined;
                return pattern;
            }
        }

        /// <summary>
        /// Net or squares hatch pattern.
        /// </summary>
        /// <remarks>The predefined pattern values are based on the acad.pat file of AutoCAD.</remarks>
        public static HatchPattern Net
        {
            get
            {
                HatchPattern pattern = new HatchPattern(PredefinedHatchPatternName.Net, "Horizontal / vertical grid");
                HatchPatternLineDefinition lineDefinition = new HatchPatternLineDefinition
                                                                {
                                                                    Angle = 0,
                                                                    Origin = Vector2.Zero,
                                                                    Delta = new Vector2(0.0, 0.125)
                                                                };
                pattern.LineDefinitions.Add(lineDefinition);

                lineDefinition = new HatchPatternLineDefinition
                                     {
                                         Angle = 90,
                                         Origin = Vector2.Zero,
                                         Delta = new Vector2(0.0, 0.125)
                                     };
                pattern.LineDefinitions.Add(lineDefinition);
                pattern.type = HatchType.Predefined;
                return pattern;
            }
        }

        /// <summary>
        /// Dots hatch pattern.
        /// </summary>
        /// <remarks>The predefined pattern values are based on the acad.pat file of AutoCAD.</remarks>
        public static HatchPattern Dots
        {
            get
            {
                HatchPattern pattern = new HatchPattern(PredefinedHatchPatternName.Dots, "A series of dots");
                HatchPatternLineDefinition lineDefinition = new HatchPatternLineDefinition
                                                                {
                                                                    Angle = 0,
                                                                    Origin = Vector2.Zero,
                                                                    Delta = new Vector2(0.03125, 0.0625),
                                                                    DashPattern = new List<double>{0, -0.0625}
                                                                };
                pattern.LineDefinitions.Add(lineDefinition);
                pattern.type = HatchType.Predefined;
                return pattern;
            }
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the hatch pattern name.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets or sets the hatch description (optional, this information is not saved in the dxf file).
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        /// <summary>
        /// Gets the hatch style (always Normal).
        /// </summary>
        /// <remarks>Only normal style is implemented.</remarks>
        public HatchStyle Style
        {
            get { return style; }
            internal set { style = value; }
        }

        /// <summary>
        /// Gets or sets the hatch pattern type.
        /// </summary>
        public HatchType Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// Gets the solid fill flag.
        /// </summary>
        public FillType Fill
        {
            get { return fill; }
            internal set {fill = value;}
        }

        /// <summary>
        /// Gets or sets the pattern angle between 0 and 360 degrees.
        /// </summary>
        public double Angle
        {
            get { return angle; }
            set
            {
                if (value < 0 || value > 360)
                    throw new ArgumentOutOfRangeException("value", "The angle must be between 0 and 360 degrees");
                angle = value;
            }
        }

        /// <summary>
        /// Gets or sets the pattern scale (not aplicable in Solid fills).
        /// </summary>
        public double Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        /// <summary>
        /// Gets or sets the definition of the lines that make up the pattern (not aplicable in Solid fills).
        /// </summary>
        public List<HatchPatternLineDefinition> LineDefinitions
        {
            get { return lineDefinitions; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                lineDefinitions = value;
            }
        }


        #endregion

        #region public methods

        /// <summary>
        /// Creates a new hatch pattern from the definition in a pat file.
        /// </summary>
        /// <param name="file">Pat file where the definition is located.</param>
        /// <param name="patternName">Name of the pattern definition that wants to be read (ignore case).</param>
        /// <returns>A Hatch pattern defined by the pat file.</returns>
        public static HatchPattern FromFile(string file, string patternName)
        {
            HatchPattern pattern = null;    
            StreamReader reader;
            try
            {
#if SILVERLIGHT
                reader = new StreamReader(File.OpenRead(file), new Encoding437()); // TODO: code page 20127, US-ASCII?
#else
                reader = new StreamReader(File.OpenRead(file), Encoding.ASCII);
#endif
            }
            catch (Exception ex)
            {
                throw (new Exception("Unknown error reading pat file: " + file, ex));
            }

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if(line==null) throw new Exception("Unknown error reading pat file: " + file);
                // lines starting with semicolons are comments
                if(line.StartsWith(";")) continue;
                // every pattern definition starts with '*'
                if (!line.StartsWith("*")) continue;

                // reading pattern name and description
                int endName = line.IndexOf(','); // the first semicolon divides the name from the description that might contain more semicolons
                string name = line.Substring(1, endName - 1);
                string description = line.Substring(endName + 1, line.Length - endName - 1);
                
                // remove start and end spaces
                description = description.Trim();
                if (!name.Equals(patternName, StringComparison.InvariantCultureIgnoreCase)) continue;

                // we have found the pattern name, the next lines of the file contains the pattern definition
                line = reader.ReadLine();
                if (line == null) throw new Exception("Unknown error reading pat file: " + file);
                pattern = new HatchPattern(name, description);

                while (!reader.EndOfStream && !line.StartsWith("*") && !string.IsNullOrEmpty(line))
                {
                    List<double> dashPattern = new List<double>();
                    string[] tokens = line.Split(',');
                    double angle = double.Parse(tokens[0]);
                    Vector2 origin = new Vector2(double.Parse(tokens[1]),double.Parse(tokens[2]));
                    Vector2 delta = new Vector2(double.Parse(tokens[3]), double.Parse(tokens[4]));
                    // the rest of the info is optional if it exists define the dash pattern definition
                    for (int i = 5; i < tokens.Length; i++)
                        dashPattern.Add(double.Parse(tokens[i]));

                    HatchPatternLineDefinition lineDefinition = new HatchPatternLineDefinition
                                                                    {
                                                                        Angle = angle, 
                                                                        Origin = origin, 
                                                                        Delta = delta, 
                                                                        DashPattern = dashPattern
                                                                    };

                    pattern.lineDefinitions.Add(lineDefinition);
                    pattern.type = HatchType.UserDefined;
                    line = reader.ReadLine();
                    if (line == null) throw new Exception("Unknown error reading pat file: " + file);
                    line = line.Trim();
                }
                // there is no need to continue parsing the file, the info has been read
                break;
            }

            reader.Close(); 
            return pattern; 
        }

        #endregion

    }
}
