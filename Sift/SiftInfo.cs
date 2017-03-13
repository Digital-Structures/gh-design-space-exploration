using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace Sift
{
    public class SiftInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "Sift";
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
                return new Guid("a6c11426-e90b-45c8-a363-668e78d6f247");
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
