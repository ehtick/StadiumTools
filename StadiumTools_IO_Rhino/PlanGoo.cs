﻿using System.Collections.Generic;
using StadiumTools;
using Rhino;
using Rhino.Geometry;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

namespace StadiumTools
{

    /// <summary>
    /// Tier Goo wrapper class, makes sure Tier can be used in Grasshopper.  
    /// </summary>
    public class BowlPlanGoo : GH_Goo<BowlPlan>
    {
        //Constructors
        public BowlPlanGoo()
        {
            this.Value = new BowlPlan();
        }
        public BowlPlanGoo(BowlPlan plan)
        {
            this.Value = plan;
        }

        public override IGH_Goo Duplicate()
        {
            return Duplicate();
        }
        public BowlPlanGoo DuplicateTierGoo()
        {
            return new BowlPlanGoo(Value == null ? new BowlPlan() : (StadiumTools.BowlPlan)Value.Clone());
        }

        public override bool IsValid
        {
            get
            {
                if (Value == null) { return false; }
                return Value.IsValid;
            }
        }

        public override string ToString()
        {
            if (Value == null)
                return "Null Plan";
            else
                return $"Plan: PS:{this.Value.PlaySurfaceParameters.SportType.ToString()} O:{this.Value.BowlOffsets} Y:{this.Value.BowlStyle.ToString()}";
        }
        public override string TypeName
        {
            get { return ("Plan"); }
        }
        public override string TypeDescription
        {
            get { return ("Defines a single StadiumTools Plan"); }
        }

        /// <summary>
        /// Method to deep copy a list of TierGoo objects to an array of Tier objects.
        /// </summary>
        /// <param name="tierList"></param>
        /// <returns></returns>
        public static StadiumTools.Tier[] CloneListToArray(List<StadiumTools.TierGoo> tierGooList)
        {
            StadiumTools.Tier[] tierArray = new StadiumTools.Tier[tierGooList.Count];
            for (int i = 0; i < tierGooList.Count; i++)
            {
                //Unwrap the TierGoo,
                StadiumTools.Tier currentTier = tierGooList[i].Value;
                //Clone the unwrapped Tier
                object clone = currentTier.Clone();
                //add it to the array
                tierArray[i] = (StadiumTools.Tier)clone;
            }
            return tierArray;
        }

    }

}
