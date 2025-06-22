namespace Tilde
{
    using Grasshopper.Kernel;
    using System;
    using System.Drawing;

    public class TildeInfo : GH_AssemblyInfo
    {
        public override string AuthorContact
        {
            get
            {
                return "";
            }
        }

        public override string AuthorName
        {
            get
            {
                return "Microsoft";
            }
        }

        public override string Description
        {
            get
            {
                return "";
            }
        }

        public override Bitmap Icon
        {
            get
            {
                return null;
            }
        }

        public override Guid Id
        {
            get
            {
                return new Guid("b8ba2684-20bc-4180-ab2e-8adf0064e38d");
            }
        }

        public override string Name
        {
            get
            {
                return "Tilde";
            }
        }
    }
}

