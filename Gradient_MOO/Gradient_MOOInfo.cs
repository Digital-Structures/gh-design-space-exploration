using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace Gradient_MOO
{
    public class Gradient_MOOInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "GradientMOO";
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
                return new Guid("af31a962-9d4a-45d2-9e81-082e822e33ae");
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
