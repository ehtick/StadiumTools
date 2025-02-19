﻿using System;
using System.Collections.Generic;
using System.Drawing;
using Rhino;
using Grasshopper.Kernel;
using GHA_StadiumTools.Properties;
using Rhino.Geometry;
using StadiumTools;

namespace GHA_StadiumTools
{
    /// <summary>
    /// Create a custom GH component called ST_ConstructSuperRiser using the GH_Component as base. 
    /// </summary>
    public class ST_FilletedRadialRectangle : GH_Component
    {
        /// <summary>
        /// A custom component for input parameters to generate a new spectator. 
        /// </summary>
        public ST_FilletedRadialRectangle()
            : base(nameof(ST_FilletedRadialRectangle), "FRR", "Construct a Filleted Radial Rectangle from parameters", "StadiumTools", "Debug")
        {
        }

        /// <summary>
        /// Registers all input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "Pl", "Origin Plane of Rectangle", GH_ParamAccess.item, Rhino.Geometry.Plane.WorldXY);
            pManager.AddNumberParameter("Length", "x", "Length of rectangle(X)", GH_ParamAccess.item, 100);
            pManager.AddNumberParameter("Width", "y", "Width of rectangle (Y)", GH_ParamAccess.item, 40);
            pManager.AddNumberParameter("Side Radaii", "sR", "Radaii of each side of the rectangle", GH_ParamAccess.list, new double[4] { 160, 160, 160, 160});
            pManager.AddNumberParameter("Fillet Radaii", "fR", "Radaii of each corner of the rectangle", GH_ParamAccess.list, new double[4] { 10, 10, 10, 10 });
            pManager.AddNumberParameter("Division", "dL", "The segment length of each side of the rectangle", GH_ParamAccess.list, new double[4] { 10, 10, 10, 10 });
            pManager.AddIntegerParameter("Corner Bays", "cB", "The number of corner bays", GH_ParamAccess.item, 3);
            pManager.AddBooleanParameter("P.O.C", "POC", "Point-on-Center. True if a discontinuity point is at the center of each side", GH_ParamAccess.list, new bool[4] { false, true, false, true });
        }

        //Set parameter indixes to names (for readability)
        private static int IN_Plane = 0;
        private static int IN_Length = 1;
        private static int IN_Width = 2;
        private static int IN_Side_Radaii = 3;
        private static int IN_Fillet_Radaii = 4;
        private static int IN_Division = 5;
        private static int IN_Corner_Bays = 6;
        private static int IN_POC = 7;
        private static int OUT_Curves = 0;
        private static int OUT_Planes = 1;

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "C", "Curves of Filleted Radial Rectangle", GH_ParamAccess.list);
            pManager.AddPlaneParameter("Planes", "P", "Perpendicular planes to the curves", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ST_FilletedRadialRectangle.HandleErrors(DA, this);
            ST_FilletedRadialRectangle.FilletedRadialRectangleFromDA(DA);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Resources.ST_debug;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("943e60b4-92d7-434a-b474-351071f59ef3");

        //Methods
        private static void FilletedRadialRectangleFromDA(IGH_DataAccess DA)
        {
            double tolerance = Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
            //Item Container (Destination)
            var planeItem = new Rhino.Geometry.Plane();
            double length = 0.0;
            double width = 0.0;
            int cornerDiv = 0;

            List<double> sideRadaii = new List<double>();
            List<double> filletRadaii = new List<double>();
            List<double> divLen = new List<double>();
            List<bool> pointAtCenter = new List<bool>();

            //Get & Set Polyline
            if (!DA.GetData<Rhino.Geometry.Plane>(IN_Plane, ref planeItem)) { return; }
            StadiumTools.Pln3d pln3d = StadiumTools.IO.Pln3dFromPlane(planeItem);
            if (!DA.GetData<double>(IN_Length, ref length)) { return; }
            if (!DA.GetData<double>(IN_Width, ref width)) { return; }
            if (!DA.GetData<int>(IN_Corner_Bays, ref cornerDiv)) { return; }
            if (!DA.GetDataList<double>(IN_Side_Radaii, sideRadaii)) { return; }
            if (!DA.GetDataList<double>(IN_Fillet_Radaii, filletRadaii)) { return; }
            if (!DA.GetDataList<double>(IN_Division, divLen)) { return; }
            if (!DA.GetDataList<bool>(IN_POC, pointAtCenter)) { return; }

            StadiumTools.Pline[] plines = StadiumTools.Boundary.RadialFilletedNonUniform
            (pln3d,
            length,
            width,
            sideRadaii.ToArray(),
            filletRadaii.ToArray(),
            divLen.ToArray(),
            cornerDiv,
            pointAtCenter.ToArray(),
            tolerance,
            out List<Pln3d> planes
            );

            List<Rhino.Geometry.Plane> rcPlanes = StadiumTools.IO.PlanesFromPln3ds(planes); 
            List<PolylineCurve> polyLineCurves = StadiumTools.IO.PolylineCurveListFromPlines(plines);
            DA.SetDataList(OUT_Curves, polyLineCurves);
            DA.SetDataList(OUT_Planes, rcPlanes);
        }

        private static void HandleErrors(IGH_DataAccess DA, GH_Component thisComponent)
        {
            //error handling here
        }
    }
}
