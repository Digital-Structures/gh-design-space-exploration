using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace StormCloud
{
    public class StormCloudInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "StormCloud";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return Resources.Resources.gen_icon_small;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "Interactive Evolutionary Optimization";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("a5e2d6da-de21-47ea-8fe9-3e48aaefd1f6");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "Renaud Alexis P.E. Danhaive, Caitlin T. Mueller";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "danhaive@mit.edu";
            }
        }
    }
}
