using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using StormCloud.ViewModel;
using StructureEngine.Model;
using StormCloud.Types;
using StormCloud.Parameters;

// namespaces for opening console
using System.Windows.Forms;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace StormCloud
{
    public class InterOptComponentStructure : GH_Component
    {
        // OPEN CONSOLE
        [DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr GetStdHandle(int nStdHandle);
        [DllImport("kernel32.dll",
            EntryPoint = "AllocConsole",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int AllocConsole();
        private const int STD_OUTPUT_HANDLE = -11;
        private const int MY_CODE_PAGE = 437;  

        // CONSTRUCTOR
        public InterOptComponentStructure(): base("Interactive Optimization", "InterOpt", "Interactive Evolutionary Optimization", "StormCloud", "Optimization")
        {
            DesignView = new DesignToolVM();
            compstruc = new ComputedStructure();
            //paramval = new List<decimal>();
            AllocConsole();
            Console.WriteLine("CONSTRUCT COMPONENT");
        }

        public DesignToolVM DesignView;

        public ComputedStructure compstruc;

        //public List<decimal> paramval;

        public override void CreateAttributes()
        {
            base.m_attributes = new InterOptComponentAttributes(this);
        }

        // INPUT PARAMETERS
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new StructureParameter());
            pManager.AddNumberParameter("Design Variables", "DVar", "Design Variables To Be Considered in the Interactive Evolutionary Optimization", GH_ParamAccess.list);
        }


        // OUTPUT PARAMETERS
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        // SOLVER
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            StructureType structuretype = new StormCloud.Types.StructureType();
            //List<decimal> inputval = new List<decimal>();
            
            if (!DA.GetData(0, ref structuretype)) { return; }
            //if (!DA.GetDataList(1, inputval)) { return; }

            ComputedStructure comp = structuretype.Value;
            
            compstruc = (ComputedStructure)structuretype.Value.CloneImpl();
            //paramval = inputval;

            DesignView.UpdateCurrentModel(comp);             
        }

        // Icon
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return null;
            }
        }

        // Guid
        public override Guid ComponentGuid
        {
            get { return new Guid("{5af70174-31f5-4931-92ac-3da6da3481bb}"); }
        }
    }
}
