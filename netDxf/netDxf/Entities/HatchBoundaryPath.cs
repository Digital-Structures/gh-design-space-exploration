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
using System.Collections.ObjectModel;

namespace netDxf.Entities
{
    /// <summary>
    /// Defines the boundary path type of the hatch.
    /// </summary>
    /// <remarks>Bit flag.</remarks>
    [Flags]
    public enum BoundaryPathTypeFlag
    {
        /// <summary>
        /// Default.
        /// </summary>
        Default = 0,
        /// <summary>
        /// External.
        /// </summary>
        External = 1,
        /// <summary>
        /// Polyline.
        /// </summary>
        Polyline = 2,
        /// <summary>
        /// Derived.
        /// </summary>
        Derived = 4,
        /// <summary>
        /// Textbox.
        /// </summary>
        Textbox = 8,
        /// <summary>
        /// Outermost.
        /// </summary>
        Outermost = 16
    }

    /// <summary>
    /// Represent a loop of a <see cref="Hatch">hatch</see> boundaries.
    /// </summary>
    /// <remarks>
    /// The first loop will be considered as the Outermost path.<br />
    /// The entities that make a loop can be any combination of lines, polylines, circles, arcs, ellipses, and splines.<br />
    /// The entities that define a loop must define a closed path and they have to be on the same plane as the hatch, 
    /// if these conditions are not met the result will be unpredictable.<br />
    /// The normal and the elevation will be omited (the hatch elevation and normal will be used instead).
    /// Only the x and y coordinates of the line, ellipse, the circle, and spline will be used.
    /// Circles, full ellipses, closed polylines, closed splines are closed paths so only one should exist in the data list.
    /// Lines, arcs, ellipse arcs, open polylines, and open splines are open paths so more enties should exist to make a closed loop.
    /// </remarks>
    public class HatchBoundaryPath
    {
        #region private fields

        private readonly List<EntityObject> data;
        private BoundaryPathTypeFlag pathTypeFlag;
        private int numberOfEdges;

        #endregion

        #region constructor
        /// <summary>
        /// Initializes a new instance of the <c>Hatch</c> class.
        /// </summary>
        /// <param name="data">List of entities that makes a loop for the hatch boundary paths.</param>
        public HatchBoundaryPath(List<EntityObject> data)
        {
            if (data == null)
                    throw new ArgumentNullException("data");
            this.data = data;
            SetInternalInfo();
        }
        #endregion

        #region public properties

        /// <summary>
        /// Gets the list of entities that makes a loop for the hatch boundary paths.
        /// </summary>
        public ReadOnlyCollection<EntityObject> Data
        {
            get { return data.AsReadOnly(); }
        }

        /// <summary>
        /// Gets the boundary path type flag.
        /// </summary>
        public BoundaryPathTypeFlag PathTypeFlag
        {
            get { return pathTypeFlag; }
            internal set { pathTypeFlag = value; }
        }

        /// <summary>
        /// Gets the number of edges that make up the boundary path.
        /// </summary>
        public int NumberOfEdges
        {
            get { return numberOfEdges; }
        }

        #endregion

        #region private methods

        private void SetInternalInfo()
        {
            numberOfEdges = 0;
            pathTypeFlag = BoundaryPathTypeFlag.Derived | BoundaryPathTypeFlag.External;

            // the first loop will be considered as the Outermost path
            foreach (EntityObject entity in data)
            {
                // it seems that AutoCAD does not have problems on creating loops that theorically does not make sense, like, for example an internal loop that is made of a single arc.
                // so if AutoCAD is ok with that I am too, the program that make use of this information will take care of this inconsistencies
                switch (entity.Type)
                {
                    case EntityType.Arc:
                        // a single arc is not a closed path
                        //if (data.Count <= 1) throw new ArgumentException("A single arc does not make closed loop.");
                        numberOfEdges += 1;
                        break;
                    case EntityType.Circle:
                        // a circle is a closed loop
                        //if (data.Count>1) throw new ArgumentException("A circle is a closed loop, there can be only per path.");
                        numberOfEdges += 1;
                        break;
                    case EntityType.Ellipse:
                        // a full ellipse is a closed loop
                        //if (((Ellipse)entity).IsFullEllipse && data.Count > 1) throw new ArgumentException("A full ellipse is a closed loop, there can be only per path.");
                        // a single ellipse arc is not a closed path
                        //if (!((Ellipse)entity).IsFullEllipse && data.Count <= 1) throw new ArgumentException("A single ellipse arc does not make closed loop.");
                        numberOfEdges += 1;
                        break;
                    case EntityType.Line:
                        // a single line is not a closed path
                        //if (data.Count <= 1) throw new ArgumentException("Only a line does not make closed loop.");
                        numberOfEdges += 1;
                        break;
                    case EntityType.LightWeightPolyline:
                        if (((LwPolyline)entity).IsClosed)
                        {
                            pathTypeFlag = BoundaryPathTypeFlag.Derived | BoundaryPathTypeFlag.External | BoundaryPathTypeFlag.Polyline;
                            numberOfEdges += ((LwPolyline)entity).Vertexes.Count;
                        }
                        else
                        {
                            // open polylines will be exploded before being written in the dxf file 
                            // for an open polyline the number of edges is equal the number of vertexes    
                            numberOfEdges += ((LwPolyline)entity).Vertexes.Count - 1;
                        }
                        break;
                    case EntityType.Spline:
                        // a closed spline is a closed loop
                        //if (((Spline)entity).IsClosed && data.Count > 1) throw new ArgumentException("A closed spline is a closed loop, there can be only per path.");
                        // an open spline is not a closed path
                        //if (!((Spline)entity).IsClosed && data.Count <= 1) throw new ArgumentException("An open spline does not make closed loop.");
                        numberOfEdges += 1;
                        break;
                    default:
                        throw new ArgumentException("The entity type " + entity.Type + " unknown or not implemented as part of a hatch boundary.");
                }
            }

        }

        #endregion

    }
}