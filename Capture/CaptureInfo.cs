using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace Capture
{
    public class CaptureInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "Capture";
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
                return new Guid("0ef73a29-d890-4819-85c6-cc7e9973dcf1");
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
