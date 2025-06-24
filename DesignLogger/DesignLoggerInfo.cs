using System;
using System.Drawing;
using Grasshopper.Kernel;

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using Rhino.Geometry;

namespace DesignLogger
{
    public class DesignLoggerInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "DesignLogger";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("4ed68dc8-3f2e-41d4-9d0c-63f3c67ade55");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
