using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace Cluster
{
    public class ClusterInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "Cluster";
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
                return new Guid("22fdcae5-a8f3-476a-8f1d-ff5843af9d98");
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
