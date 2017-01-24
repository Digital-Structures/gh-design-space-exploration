using System;

namespace JMetalCSharp.Core
{
    /// <summary>
    /// This abstract class is the base for defining new types of variables.
    /// Many methods of <code>Variable</code> (<code>getValue</code>,
    /// <code>setValue</code>,<code>
    /// getLowerLimit</code>,<code>setLowerLimit</code>,<code>getUpperLimit</code>,
    /// <code>setUpperLimit</code>)
    /// are not applicable to all the subclasses of <code>Variable</code>.
    /// For this reason, they are defined by default as giving a fatal error.
    /// </summary>
    public abstract class Variable
    {
        /// <summary>
        /// Creates an exact copy of a <code>Variable</code> object.
        /// </summary>
        /// <returns>the copy of the object.</returns>
        public abstract Variable DeepCopy();

        /// <summary>
        /// Get or Set the double value representating the encodings.variable.
        /// It is used in subclasses of <code>Variable</code> (i.e. <code>Real</code> 
        /// and <code>Int</code>).
        /// As not all objects belonging to a subclass of <code>Variable</code> have a 
        /// double value, a call to this method it is considered a fatal error by 
        /// default, and the program is terminated. Those classes requiring this method 
        /// must redefine it.
        /// </summary>
        public object Value
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Get or Set the lower bound value of a encodings.variable. As not all
        /// objects belonging to a subclass of <code>Variable</code> have a lower bound,
        /// a call to this method is considered a fatal error by default,
        /// and the program is terminated.
        /// Those classes requiring this method must redefine it.
        /// </summary>
        public object LowerBound
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Get or Set the upper bound value of a encodings.variable. As not all
        /// objects belonging to a subclass of <code>Variable</code> have an upper 
        /// bound, a call to this method is considered a fatal error by default, and the 
        /// program is terminated. Those classes requiring this method must redefine it.
        /// </summary>
        public object UpperBound
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the type of the encodings.variable. The types are defined in class Problem.
        /// </summary>
        /// <returns>The type of the encodings.variable</returns>
        public Type GetVariableType()
        {
            return this.GetType();
        }
    }
}
