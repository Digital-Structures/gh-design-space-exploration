using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace Radical
{
    public class RadicalInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "Radical";
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
                return new Guid("a3337425-275f-4cfc-8861-b669f646904d");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "Renaud Alexis Pierre Emile Danhaive / MIT DIGITAL STRUCTURES";
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
