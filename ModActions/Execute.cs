using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModActions
{
    public static class Execute
    {
        public static void ExectueAction(ModActionData action, PartModule pm, KSPActionParam param, string val)
        {
            Debug.Log("Action trigger");
            switch (action.Identifier)
            {
                case 1:
                    {
                        //place holder for Vert Vel action
                        break;
                    }
                case 2:
                    {
                        //vert vel placeholder
                        break;
                    }
                case 3: //stock engines, set thrust limit
                    {
                        float percent;
                        if (float.TryParse(val, out percent))
                        {
                            foreach (PartModule pm2 in pm.part.Modules)
                            {
                                if (pm2 is ModuleEngines)
                                {
                                    ModuleEngines eng = (ModuleEngines)pm2;
                                    eng.thrustPercentage = percent;
                                }
                            }
                        }
                        else
                        {
                            Debug.Log("Set engine thrust limit fail, non-numeric number received.");
                        }
                        break;
                    }
                case 4: //stock engines, set thrust limit
                    {
                        float percent;
                        if (float.TryParse(val, out percent))
                        {
                            foreach (PartModule pm2 in pm.part.Modules)
                            {
                                if (pm2 is ModuleEnginesFX)
                                {
                                    ModuleEnginesFX eng = (ModuleEnginesFX)pm2;
                                    eng.thrustPercentage = percent;
                                }
                            }
                        }
                        else
                        {
                            Debug.Log("Set engine thrust limit fail, non-numeric number received.");
                        }
                        break;
                    }


            } //close switch bracket
        }
    }
}
