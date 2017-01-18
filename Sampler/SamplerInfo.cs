﻿using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace Sampler
{
    public class SamplerInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "Sampler";
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
                return new Guid("ebaf1068-68c3-4322-bdda-6bb65ec9b20c");
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
