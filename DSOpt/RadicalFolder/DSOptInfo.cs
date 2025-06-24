using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace Radical
{
    public class DSOptInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "DSOpt";
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
                return "Radical with Stepper Component tab";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("883a603e-9de9-4e4b-b6f2-88f5cb172cd5");
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
