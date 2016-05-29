using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;

namespace ModActions
{
    public static class Execute 
    {
        public static void ExectueAction(ModActionData action, PartModule pm, KSPActionParam param, string val)
        {
            //Debug.Log("Action trigger");
            switch (action.Identifier)
            {
                case 1: //Vertical velocity turn off
                    {

                        foreach (Part p in pm.vessel.parts.Where(p => p.Modules.Contains("TWR1Data")))
                        {
                            if ((bool)p.Modules["TWR1Data"].Fields.GetValue("masterModule"))
                            {
                                p.Modules["TWR1Data"].Fields.SetValue("controlMode", 0);
                                p.Modules["TWR1Data"].Fields.SetValue("TWR1Engaged", false);
                                p.Modules["TWR1Data"].Fields.SetValue("TWR1VelocitySetpoint", 0f);
                                p.Modules["TWR1Data"].GetType().GetField("TWR1HeightCtrl").SetValue(p.Modules["TWR1Data"], false);
                            }
                        }
                        break;
                    }
                case 2: //vertical velocity set velocity directly
                    {

                        float sp;
                        if (float.TryParse(val, out sp))
                        {
                            foreach (Part p in pm.vessel.parts.Where(p => p.Modules.Contains("TWR1Data")))
                            {
                                if ((bool)p.Modules["TWR1Data"].Fields.GetValue("masterModule"))
                                {
                                    p.Modules["TWR1Data"].Fields.SetValue("controlMode", 1);
                                    p.Modules["TWR1Data"].Fields.SetValue("TWR1Engaged", true);
                                    p.Modules["TWR1Data"].Fields.SetValue("TWR1VelocitySetpoint", sp);
                                    p.Modules["TWR1Data"].GetType().GetField("TWR1HeightCtrl").SetValue(p.Modules["TWR1Data"], false);
                                }
                            }
                        }

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
                case 4: //vertical velocity, change velocity
                    {
                        foreach (Part p in pm.vessel.parts.Where(p => p.Modules.Contains("TWR1Data")))
                        {
                            float sp;
                            if (float.TryParse(val, out sp))
                            {
                                if ((bool)p.Modules["TWR1Data"].Fields.GetValue("masterModule"))
                                {
                                    float speedSet;
                                    if ((bool)p.Modules["TWR1Data"].Fields.GetValue("TWR1Engaged")) //if already in vel control, use setpoint, otherwise engage it using current vertical velocity
                                    {
                                        speedSet = (float)p.Modules["TWR1Data"].Fields.GetValue("TWR1VelocitySetpoint") + sp;
                                    }
                                    else
                                    {
                                        speedSet = (float)p.vessel.verticalSpeed + sp;
                                    }
                                    p.Modules["TWR1Data"].GetType().GetField("TWR1HeightCtrl").SetValue(p.Modules["TWR1Data"], false);
                                    p.Modules["TWR1Data"].Fields.SetValue("controlMode", 1);
                                    p.Modules["TWR1Data"].Fields.SetValue("TWR1Engaged", true);
                                    p.Modules["TWR1Data"].Fields.SetValue("TWR1VelocitySetpoint", speedSet);
                                }
                            }
                        }

                        break;
                    }
                case 5: //Vertical velocity height turn off
                    {

                        foreach (Part p in pm.vessel.parts.Where(p => p.Modules.Contains("TWR1Data")))
                        {
                            if ((bool)p.Modules["TWR1Data"].Fields.GetValue("masterModule"))
                            {
                                p.Modules["TWR1Data"].Fields.SetValue("controlMode", 0);
                                p.Modules["TWR1Data"].Fields.SetValue("TWR1Engaged", false);
                                p.Modules["TWR1Data"].Fields.SetValue("TWR1VelocitySetpoint", 0f);
                                p.Modules["TWR1Data"].GetType().GetField("TWR1HeightCtrl").SetValue(p.Modules["TWR1Data"], false);
                            }
                        }
                        break;
                    }
                case 6: //vertical velocity set height directly
                    {

                        float sp;
                        if (float.TryParse(val, out sp))
                        {
                            foreach (Part p in pm.vessel.parts.Where(p => p.Modules.Contains("TWR1Data")))
                            {
                                if ((bool)p.Modules["TWR1Data"].Fields.GetValue("masterModule"))
                                {
                                    if (!(bool)p.Modules["TWR1Data"].GetType().GetField("TWR1OrbitDropAllow").GetValue(p.Modules["TWR1Data"])) //enter orbit drop mode also?
                                    {
                                        if ((bool)p.Modules["TWR1Data"].GetType().GetField("TWR1HCOrbitDrop").GetValue(p.Modules["TWR1Data"]))
                                        {
                                            p.Modules["TWR1Data"].GetType().GetField("TWR1HCOrbitDrop").SetValue(p.Modules["TWR1Data"], true);
                                            p.Modules["TWR1Data"].GetType().GetField("TWR1HCThrustWarningTime").SetValue(p.Modules["TWR1Data"], 0);
                                        }
                                        else
                                        {
                                            p.Modules["TWR1Data"].GetType().GetField("TWR1HCOrbitDrop").SetValue(p.Modules["TWR1Data"], false);
                                        }
                                    }
                                    p.Modules["TWR1Data"].Fields.SetValue("controlMode", 1);
                                    p.Modules["TWR1Data"].Fields.SetValue("TWR1Engaged", true);
                                    p.Modules["TWR1Data"].GetType().GetField("TWR1HeightCtrl").SetValue(p.Modules["TWR1Data"], true);
                                    p.Modules["TWR1Data"].GetType().GetField("TWR1HCTarget").SetValue(p.Modules["TWR1Data"], sp);
                                }
                            }
                        }

                        break;
                    }
                case 7: //vertical velocity  height change
                    {

                        float sp;
                        if (float.TryParse(val, out sp))
                        {
                            foreach (Part p in pm.vessel.parts.Where(p => p.Modules.Contains("TWR1Data")))
                            {
                                if ((bool)p.Modules["TWR1Data"].Fields.GetValue("masterModule"))
                                {
                                    if (!(bool)p.Modules["TWR1Data"].GetType().GetField("TWR1OrbitDropAllow").GetValue(p.Modules["TWR1Data"])) //enter orbit drop mode also?
                                    {
                                        if ((bool)p.Modules["TWR1Data"].GetType().GetField("TWR1HCOrbitDrop").GetValue(p.Modules["TWR1Data"]))
                                        {
                                            p.Modules["TWR1Data"].GetType().GetField("TWR1HCOrbitDrop").SetValue(p.Modules["TWR1Data"], true);
                                            p.Modules["TWR1Data"].GetType().GetField("TWR1HCThrustWarningTime").SetValue(p.Modules["TWR1Data"], 0);
                                        }
                                        else
                                        {
                                            p.Modules["TWR1Data"].GetType().GetField("TWR1HCOrbitDrop").SetValue(p.Modules["TWR1Data"], false);
                                        }
                                    }
                                    p.Modules["TWR1Data"].Fields.SetValue("controlMode", 1);
                                    p.Modules["TWR1Data"].Fields.SetValue("TWR1Engaged", true);
                                    p.Modules["TWR1Data"].GetType().GetField("TWR1HeightCtrl").SetValue(p.Modules["TWR1Data"], true);
                                    p.Modules["TWR1Data"].GetType().GetField("TWR1HCTarget").SetValue(p.Modules["TWR1Data"], (float)p.Modules["TWR1Data"].Fields.GetValue("TWR1HCTarget") + sp);
                                }
                            }
                        }

                        break;
                    }
                case 8: //stock docking port, stock command module, control from here
                    {
                        pm.vessel.SetReferenceTransform(pm.part);
                        break;
                    }
                case 9: //stock command RCS on
                    {
                        pm.vessel.ActionGroups.SetGroup(KSPActionGroup.RCS, true);
                        break;
                    }
                case 10: //stock command RCS off
                    {
                        pm.vessel.ActionGroups.SetGroup(KSPActionGroup.RCS, false);
                        break;
                    }
                case 11: //stock command RCS toggle
                    {
                        if (param.type == KSPActionType.Activate)
                        {
                            pm.vessel.ActionGroups.SetGroup(KSPActionGroup.RCS, true);
                        }
                        else
                        {
                            pm.vessel.ActionGroups.SetGroup(KSPActionGroup.RCS, false);
                        }
                        break;
                    }
                case 12:
                    {
                        pm.vessel.ActionGroups.SetGroup(KSPActionGroup.SAS, false);
                        break;
                    }
                case 13:
                    {
                        if (pm.vessel.Autopilot.CanSetMode(VesselAutopilot.AutopilotMode.StabilityAssist))
                        {
                            pm.vessel.ActionGroups.SetGroup(KSPActionGroup.SAS, true);
                            pm.vessel.Autopilot.SetMode(VesselAutopilot.AutopilotMode.StabilityAssist);
                            if (pm.vessel == FlightGlobals.ActiveVessel)
                            {
                                MonoBehaviorMethods.setSASUI(0);
                            }
                        }
                        break;
                    }
                case 14:
                    {
                        if (pm.vessel.Autopilot.CanSetMode(VesselAutopilot.AutopilotMode.Prograde))
                        {
                            pm.vessel.ActionGroups.SetGroup(KSPActionGroup.SAS, true);
                            pm.vessel.Autopilot.SetMode(VesselAutopilot.AutopilotMode.Prograde);
                            if (pm.vessel == FlightGlobals.ActiveVessel)
                            {
                                MonoBehaviorMethods.setSASUI(1);
                            }
                        }
                        break;
                    }
                case 15:
                    {
                        if (pm.vessel.Autopilot.CanSetMode(VesselAutopilot.AutopilotMode.Retrograde))
                        {
                            pm.vessel.ActionGroups.SetGroup(KSPActionGroup.SAS, true);
                            pm.vessel.Autopilot.SetMode(VesselAutopilot.AutopilotMode.Retrograde);
                            if (pm.vessel == FlightGlobals.ActiveVessel)
                            {
                                MonoBehaviorMethods.setSASUI(2);
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        if (pm.vessel.Autopilot.CanSetMode(VesselAutopilot.AutopilotMode.Normal))
                        {
                            pm.vessel.ActionGroups.SetGroup(KSPActionGroup.SAS, true);
                            pm.vessel.Autopilot.SetMode(VesselAutopilot.AutopilotMode.Normal);
                            if (pm.vessel == FlightGlobals.ActiveVessel)
                            {
                                MonoBehaviorMethods.setSASUI(3);
                            }
                        }
                        break;
                    }
                case 17:
                    {
                        if (pm.vessel.Autopilot.CanSetMode(VesselAutopilot.AutopilotMode.Antinormal))
                        {
                            pm.vessel.ActionGroups.SetGroup(KSPActionGroup.SAS, true);
                            pm.vessel.Autopilot.SetMode(VesselAutopilot.AutopilotMode.Antinormal);
                            if (pm.vessel == FlightGlobals.ActiveVessel)
                            {
                                MonoBehaviorMethods.setSASUI(4);
                            }
                        }
                        break;
                    }
                case 18:
                    {
                        if (pm.vessel.Autopilot.CanSetMode(VesselAutopilot.AutopilotMode.RadialIn))
                        {
                            pm.vessel.ActionGroups.SetGroup(KSPActionGroup.SAS, true);
                            pm.vessel.Autopilot.SetMode(VesselAutopilot.AutopilotMode.RadialIn);
                            if (pm.vessel == FlightGlobals.ActiveVessel)
                            {
                                MonoBehaviorMethods.setSASUI(5);
                            }
                        }
                        break;
                    }
                case 19:
                    {
                        if (pm.vessel.Autopilot.CanSetMode(VesselAutopilot.AutopilotMode.RadialOut))
                        {
                            pm.vessel.ActionGroups.SetGroup(KSPActionGroup.SAS, true);
                            pm.vessel.Autopilot.SetMode(VesselAutopilot.AutopilotMode.RadialOut);
                            if (pm.vessel == FlightGlobals.ActiveVessel)
                            {
                                MonoBehaviorMethods.setSASUI(6);
                            }
                        }
                        break;
                    }
                case 20:
                    {
                        if (pm.vessel.Autopilot.CanSetMode(VesselAutopilot.AutopilotMode.Maneuver))
                        {
                            pm.vessel.ActionGroups.SetGroup(KSPActionGroup.SAS, true);
                            pm.vessel.Autopilot.SetMode(VesselAutopilot.AutopilotMode.Maneuver);
                            if (pm.vessel == FlightGlobals.ActiveVessel)
                            {
                                MonoBehaviorMethods.setSASUI(9);
                            }
                        }
                        break;
                    }
                case 21:
                    {
                        if (pm.vessel.Autopilot.CanSetMode(VesselAutopilot.AutopilotMode.Target))
                        {
                            pm.vessel.ActionGroups.SetGroup(KSPActionGroup.SAS, true);
                            pm.vessel.Autopilot.SetMode(VesselAutopilot.AutopilotMode.Target);
                            if (pm.vessel == FlightGlobals.ActiveVessel)
                            {
                                MonoBehaviorMethods.setSASUI(7);
                            }
                        }
                        break;
                    }
                case 22:
                    {
                        if (pm.vessel.Autopilot.CanSetMode(VesselAutopilot.AutopilotMode.AntiTarget))
                        {
                            pm.vessel.ActionGroups.SetGroup(KSPActionGroup.SAS, true);
                            pm.vessel.Autopilot.SetMode(VesselAutopilot.AutopilotMode.AntiTarget);
                            if (pm.vessel == FlightGlobals.ActiveVessel)
                            {
                                MonoBehaviorMethods.setSASUI(8);
                            }
                        }
                        break;
                    }
                case 23: //stock command, set throttle
                    {
                        float sp;
                        if (float.TryParse(val, out sp))
                        {
                            if (pm.vessel == FlightGlobals.ActiveVessel)
                            {
                                FlightInputHandler.state.mainThrottle = Mathf.Clamp(sp * .01f, 0f, 1f);
                            }
                            else
                            {
                                pm.vessel.ctrlState.mainThrottle = Mathf.Clamp(sp * .01f, 0f, 1f);
                            }
                        }

                        break;
                    }
                case 24: //stock command, change throttle
                    {
                        float sp;
                        if (float.TryParse(val, out sp))
                        {
                            if (pm.vessel == FlightGlobals.ActiveVessel)
                            {
                                float newThrottle = FlightInputHandler.state.mainThrottle + (sp * .01f);
                                FlightInputHandler.state.mainThrottle = Mathf.Clamp(newThrottle, 0f, 1f);
                            }
                            else
                            {
                                float newThrottle = pm.vessel.ctrlState.mainThrottle + (sp * .01f);
                                pm.vessel.ctrlState.mainThrottle = Mathf.Clamp(newThrottle, 0f, 1f);
                            }
                        }

                        break;
                    }
                case 25: //moduleControlSurface & moduleAeroSurface
                    {
                        foreach(ModuleControlSurface CS in pm.part.Modules.OfType<ModuleControlSurface>())
                        {
                            CS.ignorePitch = false;
                        }
                        break;
                    }
                case 26: //moduleControlSurface & moduleAeroSurface
                    {
                        foreach (ModuleControlSurface CS in pm.part.Modules.OfType<ModuleControlSurface>())
                        {
                            CS.ignorePitch = true;
                        }
                        break;
                    }
                case 27: //moduleControlSurface & moduleAeroSurface
                    {
                        foreach (ModuleControlSurface CS in pm.part.Modules.OfType<ModuleControlSurface>())
                        {
                            if (param.type == KSPActionType.Activate)
                            {
                                CS.ignorePitch = false;
                            }
                            else
                            {
                                CS.ignorePitch = true;
                            }
                        }
                        break;
                    }
                case 28: //moduleControlSurface & moduleAeroSurface
                    {
                        foreach (ModuleControlSurface CS in pm.part.Modules.OfType<ModuleControlSurface>())
                        {
                            CS.ignoreYaw = false;
                        }
                        break;
                    }
                case 29: //moduleControlSurface & moduleAeroSurface
                    {
                        foreach (ModuleControlSurface CS in pm.part.Modules.OfType<ModuleControlSurface>())
                        {
                            CS.ignoreYaw = true;
                        }
                        break;
                    }
                case 30: //moduleControlSurface & moduleAeroSurface
                    {
                        foreach (ModuleControlSurface CS in pm.part.Modules.OfType<ModuleControlSurface>())
                        {
                            if (param.type == KSPActionType.Activate)
                            {
                                CS.ignoreYaw = false;
                            }
                            else
                            {
                                CS.ignoreYaw = true;
                            }
                        }
                        break;
                    }
                case 31: //moduleControlSurface & moduleAeroSurface
                    {
                        foreach (ModuleControlSurface CS in pm.part.Modules.OfType<ModuleControlSurface>())
                        {
                            CS.ignoreRoll = false;
                        }
                        break;
                    }
                case 32: //moduleControlSurface & moduleAeroSurface
                    {
                        foreach (ModuleControlSurface CS in pm.part.Modules.OfType<ModuleControlSurface>())
                        {
                            CS.ignoreRoll = true;
                        }
                        break;
                    }
                case 33: //moduleControlSurface & moduleAeroSurface
                    {
                        foreach (ModuleControlSurface CS in pm.part.Modules.OfType<ModuleControlSurface>())
                        {
                            if (param.type == KSPActionType.Activate)
                            {
                                CS.ignoreRoll = false;
                            }
                            else
                            {
                                CS.ignoreRoll = true;
                            }
                        }
                        break;
                    }
                case 34:
                    {
                        float sp;
                        if(float.TryParse(val, out sp))
                        {
                            foreach (PartModule pm2 in pm.part.Modules)
                            {
                                if (pm2.moduleName == "FARControllableSurface")
                                {
                                    pm2.Fields.SetValue("pitchaxis", sp);
                                }
                            }
                        }
                        break;
                    }
                case 35:
                    {
                        float sp;
                        if (float.TryParse(val, out sp))
                        {
                            foreach (PartModule pm2 in pm.part.Modules)
                            {
                                if (pm2.moduleName == "FARControllableSurface")
                                {
                                    float curVal = (float)pm2.Fields.GetValue("pitchaxis");
                                    sp = curVal + sp;
                                    pm2.Fields.SetValue("pitchaxis", sp);
                                }
                            }
                        }
                        break;
                    }
                case 36:
                    {
                        float sp;
                        if (float.TryParse(val, out sp))
                        {
                            foreach (PartModule pm2 in pm.part.Modules)
                            {
                                if (pm2.moduleName == "FARControllableSurface")
                                {
                                    pm2.Fields.SetValue("yawaxis", sp);
                                }
                            }
                        }
                        break;
                    }
                case 37:
                    {
                        float sp;
                        if (float.TryParse(val, out sp))
                        {
                            foreach (PartModule pm2 in pm.part.Modules)
                            {
                                if (pm2.moduleName == "FARControllableSurface")
                                {
                                    float curVal = (float)pm2.Fields.GetValue("yawaxis");
                                    sp = curVal + sp;
                                    pm2.Fields.SetValue("yawaxis", sp);
                                }
                            }
                        }
                        break;
                    }
                case 38:
                    {
                        float sp;
                        if (float.TryParse(val, out sp))
                        {
                            foreach (PartModule pm2 in pm.part.Modules)
                            {
                                if (pm2.moduleName == "FARControllableSurface")
                                {
                                    pm2.Fields.SetValue("rollaxis", sp);
                                }
                            }
                        }
                        break;
                    }
                case 39:
                    {
                        float sp;
                        if (float.TryParse(val, out sp))
                        {
                            foreach (PartModule pm2 in pm.part.Modules)
                            {
                                if (pm2.moduleName == "FARControllableSurface")
                                {
                                    float curVal = (float)pm2.Fields.GetValue("rollaxis");
                                    sp = curVal + sp;
                                    pm2.Fields.SetValue("yawaxis", sp);
                                }
                            }
                        }
                        break;
                    }
                case 40: //ModuleWheel discontinued in KSP 1.1, left just in case for now
                    {
                        foreach (ModuleWheel mWheel in pm.part.Modules.OfType<ModuleWheel>())
                        {
                            BaseAction whlBrakes = mWheel.Actions.Find(ba => ba.name == "BrakesAction");
                            whlBrakes.actionGroup = whlBrakes.actionGroup | KSPActionGroup.Brakes;
                        }
                        break;
                    }
                case 41:
                    {
                        foreach (ModuleWheel mWheel in pm.part.Modules.OfType<ModuleWheel>())
                        {
                            BaseAction whlBrakes = mWheel.Actions.Find(ba => ba.name == "BrakesAction");
                            whlBrakes.actionGroup &= ~KSPActionGroup.Brakes;
                        }
                        break;
                    }
                case 42:
                    {
                        foreach (ModuleWheel mWheel in pm.part.Modules.OfType<ModuleWheel>())
                        {
                            BaseAction whlBrakes = mWheel.Actions.Find(ba => ba.name == "BrakesAction");
                            if ((whlBrakes.actionGroup & KSPActionGroup.Brakes) == KSPActionGroup.Brakes)
                            {
                                whlBrakes.actionGroup &= ~KSPActionGroup.Brakes;
                            }
                            else
                            {
                                whlBrakes.actionGroup = whlBrakes.actionGroup | KSPActionGroup.Brakes;
                            }
                        }
                        break;
                    }
                case 43: //stock sci lab clean experimentsher.
                    {
                        foreach (ModuleScienceLab sciLab in pm.part.Modules.OfType<ModuleScienceLab>())
                        {
                            sciLab.CleanModulesEvent();
                        }
                        break;
                    }
                case 44: //pump on
                    {
                        //foreach(PartModule pm4 in pm.part.Modules)
                        //{
                        //    Debug.Log("module " + pm4.moduleName);
                        //    if(pm4.moduleName=="GPOSpeedPump")
                        //    {
                        //        foreach(BaseField fld in pm4.Fields)
                        //        {
                        //            Debug.Log(fld.name);
                        //        }
                        //    }
                        //}
                        pm.part.Modules["GPOSpeedPump"].Fields.SetValue("_autoPump", true);
                        break;
                    }
                case 45: //pump off
                    {
                        pm.part.Modules["GPOSpeedPump"].Fields.SetValue("_autoPump", false);
                        break;
                    }
                case 46: //pump toggle
                    {
                        if ((bool)pm.part.Modules["GPOSpeedPump"].Fields.GetValue("_autoPump"))
                        {
                            pm.part.Modules["GPOSpeedPump"].Fields.SetValue("_autoPump", false);
                        }
                        else
                        {
                            pm.part.Modules["GPOSpeedPump"].Fields.SetValue("_autoPump", true);
                        }
                        break;
                    }
                case 47: //balance on
                    {
                        pm.part.Modules["GPOSpeedPump"].Fields.SetValue("_autoBalance", true);
                        break;
                    }
                case 48: //bal off
                    {
                        pm.part.Modules["GPOSpeedPump"].Fields.SetValue("_autoBalance", false);
                        break;
                    }
                case 49: //bal toggle
                    {
                        if ((bool)pm.part.Modules["GPOSpeedPump"].Fields.GetValue("_autoBalance"))
                        {
                            pm.part.Modules["GPOSpeedPump"].Fields.SetValue("_autoBalance", false);
                        }
                        else
                        {
                            pm.part.Modules["GPOSpeedPump"].Fields.SetValue("_autoBalance", true);
                        }
                        break;
                    }
                case 50: //set pump level
                    {
                        int setpoint;
                        if (int.TryParse(val, out setpoint))
                        {
                            pm.part.Modules["GPOSpeedPump"].Fields.SetValue("_pumpLevel", Mathf.Clamp((float)setpoint, 0f, 16f));
                        }
                        break;
                    }
                case 51: //change pump level
                    {
                        int setpoint;
                        if (int.TryParse(val, out setpoint))
                        {
                            float setpoint2 = (float)pm.part.Modules["GPOSpeedPump"].Fields.GetValue("_pumpLevel") + (float)setpoint;
                            pm.part.Modules["GPOSpeedPump"].Fields.SetValue("_pumpLevel", Mathf.Clamp(setpoint2, 0f, 16f));
                        }
                        break;
                    }
                case 52: //land aid on
                    {
                        if (pm.vessel == FlightGlobals.ActiveVessel)
                        {
                            Type calledType = Type.GetType("RCSLandAid.RCSLandingAid, RCSLandAid");
                            //Debug.Log("type " + calledType.ToString());
                            MonoBehaviour LandAidGUI = (MonoBehaviour)UnityEngine.Object.FindObjectOfType(calledType);
                            // Debug.Log("method " + LandAidGUI.name);
                            MethodInfo myMethod = calledType.GetMethod("SetHoverOn", BindingFlags.Instance | BindingFlags.Public);
                            // Debug.Log("method3 " + myMethod.Name);
                            myMethod.Invoke(LandAidGUI, null);
                        }
                        else
                        {
                            foreach (Part p in pm.vessel.parts.Where(p => p.Modules.Contains("RCSLandingAidModule")))
                            {
                                if ((bool)p.Modules["RCSLandingAidModule"].Fields.GetValue("isMasterModule"))
                                {
                                    p.Modules["RCSLandingAidModule"].Fields.SetValue("controlState",1);
                                }
                            }
                        }
                        break;
                    }
                case 53: //land aid off
                    {
                        if(pm.vessel == FlightGlobals.ActiveVessel)
                        {
                        Type calledType = Type.GetType("RCSLandAid.RCSLandingAid, RCSLandAid");
                        //Debug.Log("type " + calledType.ToString());
                        MonoBehaviour LandAidGUI = (MonoBehaviour)UnityEngine.Object.FindObjectOfType(calledType);
                        // Debug.Log("method " + LandAidGUI.name);
                        MethodInfo myMethod = calledType.GetMethod("SetHoverOff", BindingFlags.Instance | BindingFlags.Public);
                        // Debug.Log("method3 " + myMethod.Name);
                        myMethod.Invoke(LandAidGUI, null);
                        }
                        else
                        {
                            foreach (Part p in pm.vessel.parts.Where(p => p.Modules.Contains("RCSLandingAidModule")))
                            {
                                if ((bool)p.Modules["RCSLandingAidModule"].Fields.GetValue("isMasterModule"))
                                {
                                    p.Modules["RCSLandingAidModule"].Fields.SetValue("controlState",0);
                                }
                            }
                        }
                        break;
                    }
                case 54:
                    {
                        int landAidEngaged = 0;
                        foreach (Part p in pm.vessel.parts.Where(p => p.Modules.Contains("RCSLandingAidModule")))
                        {
                            if ((bool)p.Modules["RCSLandingAidModule"].Fields.GetValue("isMasterModule"))
                            {
                                landAidEngaged = (int)p.Modules["RCSLandingAidModule"].Fields.GetValue("controlState");
                            }
                        }
                        if(landAidEngaged == 1)
                        {
                            goto case 53;
                        }
                        else
                        {
                            goto case 52;
                        }
                        break;
                    }
                case 55: //position hold off
                    {
                        if (pm.vessel == FlightGlobals.ActiveVessel)
                        {
                            Type calledType = Type.GetType("RCSLandAid.RCSLandingAid, RCSLandAid");
                            //Debug.Log("type " + calledType.ToString());
                            MonoBehaviour LandAidGUI = (MonoBehaviour)UnityEngine.Object.FindObjectOfType(calledType);
                            // Debug.Log("method " + LandAidGUI.name);
                            MethodInfo myMethod = calledType.GetMethod("SetHoverOff", BindingFlags.Instance | BindingFlags.Public);
                            // Debug.Log("method3 " + myMethod.Name);
                            myMethod.Invoke(LandAidGUI, null);
                        }
                        else
                        {
                            foreach (Part p in pm.vessel.parts.Where(p => p.Modules.Contains("RCSLandingAidModule")))
                            {
                                if ((bool)p.Modules["RCSLandingAidModule"].Fields.GetValue("isMasterModule"))
                                {
                                    p.Modules["RCSLandingAidModule"].Fields.SetValue("controlState", 0);
                                    p.Modules["RCSLandingAidModule"].Fields.SetValue("selectingTarget", false);
                                }
                            }
                        }
                        break;
                    }
                case 56: //position hold here
                    {
                        if (pm.vessel == FlightGlobals.ActiveVessel)
                        {
                            Type calledType = Type.GetType("RCSLandAid.RCSLandingAid, RCSLandAid");
                            //Debug.Log("type " + calledType.ToString());
                            MonoBehaviour LandAidGUI = (MonoBehaviour)UnityEngine.Object.FindObjectOfType(calledType);
                            // Debug.Log("method " + LandAidGUI.name);
                            MethodInfo myMethod = calledType.GetMethod("SetHoldOnHere", BindingFlags.Instance | BindingFlags.Public);
                            // Debug.Log("method3 " + myMethod.Name);
                            myMethod.Invoke(LandAidGUI, null);
                        }
                        else
                        {
                            foreach (Part p in pm.vessel.parts.Where(p => p.Modules.Contains("RCSLandingAidModule")))
                            {
                                if ((bool)p.Modules["RCSLandingAidModule"].Fields.GetValue("isMasterModule"))
                                {
                                    p.Modules["RCSLandingAidModule"].Fields.SetValue("controlState", 2);
                                    p.Modules["RCSLandingAidModule"].Fields.SetValue("targetLocation", FlightGlobals.ActiveVessel.transform.position);
                                    //p.Modules["RCSLandingAidModule"].Fields.SetValue("selectingTarget", false);
                                }
                            }
                        }
                        break;
                    }
                case 57: //position hold at mouse click
                    {
                        if (pm.vessel == FlightGlobals.ActiveVessel)
                        {
                            Type calledType = Type.GetType("RCSLandAid.RCSLandingAid, RCSLandAid");
                            //Debug.Log("type " + calledType.ToString());
                            MonoBehaviour LandAidGUI = (MonoBehaviour)UnityEngine.Object.FindObjectOfType(calledType);
                            // Debug.Log("method " + LandAidGUI.name);
                            MethodInfo myMethod = calledType.GetMethod("SetHoldOnLink", BindingFlags.Instance | BindingFlags.Public);
                            // Debug.Log("method3 " + myMethod.Name);
                            myMethod.Invoke(LandAidGUI, null);
                        }
                        else
                        {
                            foreach (Part p in pm.vessel.parts.Where(p => p.Modules.Contains("RCSLandingAidModule")))
                            {
                                if ((bool)p.Modules["RCSLandingAidModule"].Fields.GetValue("isMasterModule"))
                                {
                                    p.Modules["RCSLandingAidModule"].Fields.SetValue("controlState", 2);
                                    p.Modules["RCSLandingAidModule"].Fields.SetValue("targetLocation", FlightGlobals.ActiveVessel.transform.position);
                                    //p.Modules["RCSLandingAidModule"].Fields.SetValue("selectingTarget", false);
                                }
                            }
                        }
                        break;
                    }
                case 58: //new wheel system in KSP 1.1, toggle suspencion
                    {
                        foreach(ModuleWheelBase pm2 in pm.part.Modules.OfType<ModuleWheelBase>())
                        {
                            pm2.suspensionEnabled = !pm2.suspensionEnabled;
                        }
                        break;
                    }
                case 59: //enable friction control
                    {
                        foreach (ModuleWheelBase pm2 in pm.part.Modules.OfType<ModuleWheelBase>())
                        {
                            pm2.autoFriction = true;
                        }
                        break;
                    }
                case 60: //disable friction control
                    {
                        foreach (ModuleWheelBase pm2 in pm.part.Modules.OfType<ModuleWheelBase>())
                        {
                            pm2.autoFriction = false;
                        }
                        break;
                    }
                case 61: //add brakes to brake action group
                    {
                        foreach (ModuleWheels.ModuleWheelBrakes pm2 in pm.part.Modules.OfType<ModuleWheels.ModuleWheelBrakes>())
                        {
                                BaseAction whlBrakes = pm2.Actions.Find(ba => ba.name == "Brake");
                                whlBrakes.actionGroup = whlBrakes.actionGroup | KSPActionGroup.Brakes;
                        }
                        break;
                    }
                case 62: //remove brakes from brake action group
                    {
                        foreach (ModuleWheels.ModuleWheelBrakes pm2 in pm.part.Modules.OfType<ModuleWheels.ModuleWheelBrakes>())
                        {
                            BaseAction whlBrakes = pm2.Actions.Find(ba => ba.name == "Brake");
                            whlBrakes.actionGroup &= ~KSPActionGroup.Brakes;
                        }
                        break;
                    }
                case 63: //toggle brakes in brake group
                    {
                        foreach (ModuleWheels.ModuleWheelBrakes pm2 in pm.part.Modules.OfType<ModuleWheels.ModuleWheelBrakes>())
                        {
                            BaseAction whlBrakes = pm2.Actions.Find(ba => ba.name == "Brake");
                            if ((whlBrakes.actionGroup & KSPActionGroup.Brakes) == KSPActionGroup.Brakes)
                            {
                                whlBrakes.actionGroup &= ~KSPActionGroup.Brakes;
                            }
                            else
                            {
                                whlBrakes.actionGroup = whlBrakes.actionGroup | KSPActionGroup.Brakes;
                            }
                        }
                        break;
                    }
                case 64: //turn brakes on
                    {
                        foreach (ModuleWheels.ModuleWheelBrakes pm2 in pm.part.Modules.OfType<ModuleWheels.ModuleWheelBrakes>())
                        {
                            pm2.brakeInput = 1f;
                        }
                        break;
                    }
                case 65: //turn brakes off
                    {
                        foreach (ModuleWheels.ModuleWheelBrakes pm2 in pm.part.Modules.OfType<ModuleWheels.ModuleWheelBrakes>())
                        {
                            pm2.brakeInput = 0f;
                        }
                        break;
                    }
                case 66: //toggle brakes
                    {
                        foreach (ModuleWheels.ModuleWheelBrakes pm2 in pm.part.Modules.OfType<ModuleWheels.ModuleWheelBrakes>())
                        {
                            if(pm2.brakeInput == 0f)
                            {
                                pm2.brakeInput = 1f;
                            }
                            else
                            {
                                pm2.brakeInput = 0f;
                            }
                        }
                        break;
                    }
                case 67: //set brake strength
                    {
                        float setVal = 0f;
                        if (float.TryParse(val, out setVal))
                        {
                            setVal = Mathf.Clamp(setVal, 0f, 100f);
                            foreach (ModuleWheels.ModuleWheelBrakes pm2 in pm.part.Modules.OfType<ModuleWheels.ModuleWheelBrakes>())
                            {
                                pm2.brakeTweak = setVal;
                            }
                        }
                        break;
                    }
                case 68: //chnage brake strength
                    {
                        float setVal = 0f;
                        if (float.TryParse(val, out setVal))
                        {
                            foreach (ModuleWheels.ModuleWheelBrakes pm2 in pm.part.Modules.OfType<ModuleWheels.ModuleWheelBrakes>())
                            {
                                float tempVal = pm2.brakeTweak + setVal;
                                pm2.brakeTweak = Mathf.Clamp(tempVal, 0f, 100f);
                            }
                        }
                        break;
                    }
                case 69: //deploy landing legs/gear
                    {
                        foreach (ModuleWheels.ModuleWheelDeployment pm2 in pm.part.Modules.OfType<ModuleWheels.ModuleWheelDeployment>())
                        {
                            if (pm2.part.ShieldedFromAirstream && !pm2.shieldedCanDeploy)
                            {
                                ScreenMessages.PostScreenMessage("Landing Legs/Gear deploy action failed.", 5f, ScreenMessageStyle.UPPER_LEFT);
                            }
                            else
                            {
                                pm2.fsm.RunEvent(pm2.on_deploy);
                            }
                        }
                        break;
                    }
                case 70: //retract landing legs/gear
                    {
                        foreach (ModuleWheels.ModuleWheelDeployment pm2 in pm.part.Modules.OfType<ModuleWheels.ModuleWheelDeployment>())
                        {
                                pm2.fsm.RunEvent(pm2.on_retract);
                        }
                        break;
                    }
                case 71: //traction control on
                    {
                        foreach (ModuleWheels.ModuleWheelMotor pm2 in pm.part.Modules.OfType<ModuleWheels.ModuleWheelMotor>())
                        {
                            pm2.autoTorque = true;
                        }
                        break;
                    }
                case 72: //traction control off
                    {
                        foreach (ModuleWheels.ModuleWheelMotor pm2 in pm.part.Modules.OfType<ModuleWheels.ModuleWheelMotor>())
                        {
                            pm2.autoTorque = false;
                        }
                        break;
                    }
                case 73: //set traction control intensity
                    {
                        float setVal = 0f;
                        if (float.TryParse(val, out setVal))
                        {
                            foreach (ModuleWheels.ModuleWheelMotor pm2 in pm.part.Modules.OfType<ModuleWheels.ModuleWheelMotor>())
                            {
                                pm2.tractionControlScale = Mathf.Clamp(setVal, 0f, 5f);
                            }
                        }
                        break;
                    }
                case 74: //change traction control intensity
                    {
                        float setVal = 0f;
                        if (float.TryParse(val, out setVal))
                        {
                            foreach (ModuleWheels.ModuleWheelMotor pm2 in pm.part.Modules.OfType<ModuleWheels.ModuleWheelMotor>())
                            {
                                pm2.tractionControlScale = Mathf.Clamp(pm2.tractionControlScale + setVal, 0f, 5f);
                            }
                        }
                        break;
                    }
                case 75: //enable motor
                    {
                        
                            foreach (ModuleWheels.ModuleWheelMotor pm2 in pm.part.Modules.OfType<ModuleWheels.ModuleWheelMotor>())
                            {
                                pm2.motorEnabled = true;
                            }
                        
                        break;
                    }
                case 76: //disable motor
                    {

                        foreach (ModuleWheels.ModuleWheelMotor pm2 in pm.part.Modules.OfType<ModuleWheels.ModuleWheelMotor>())
                        {
                            pm2.motorEnabled = false;
                        }

                        break;
                    }
                case 77: //toggle motor
                    {

                        foreach (ModuleWheels.ModuleWheelMotor pm2 in pm.part.Modules.OfType<ModuleWheels.ModuleWheelMotor>())
                        {
                            pm2.motorEnabled = !pm2.motorEnabled;
                        }

                        break;
                    }
                case 78: //set motor %
                    {
                        float setVal = 0f;
                        if (float.TryParse(val, out setVal))
                        {
                            foreach (ModuleWheels.ModuleWheelMotor pm2 in pm.part.Modules.OfType<ModuleWheels.ModuleWheelMotor>())
                            {
                                pm2.driveLimiter = Mathf.Clamp(setVal, 0f, 100f);
                            }
                        }

                        break;
                    }
                case 79: //change motor %
                    {
                        float setVal = 0f;
                        if (float.TryParse(val, out setVal))
                        {
                            foreach (ModuleWheels.ModuleWheelMotor pm2 in pm.part.Modules.OfType<ModuleWheels.ModuleWheelMotor>())
                            {
                                pm2.driveLimiter = Mathf.Clamp(pm2.driveLimiter+ setVal, 0f, 100f);
                            }
                        }

                        break;
                    }
                case 80: //motor direction normal
                    {
                        
                            foreach (ModuleWheels.ModuleWheelMotor pm2 in pm.part.Modules.OfType<ModuleWheels.ModuleWheelMotor>())
                            {
                                pm2.motorInverted = false;
                            }
                        

                        break;
                    }
                case 81: //motor direction inverted
                    {

                        foreach (ModuleWheels.ModuleWheelMotor pm2 in pm.part.Modules.OfType<ModuleWheels.ModuleWheelMotor>())
                        {
                            pm2.motorInverted = true;
                        }


                        break;
                    }
                case 82: //toggle motor direction 
                    {

                        foreach (ModuleWheels.ModuleWheelMotor pm2 in pm.part.Modules.OfType<ModuleWheels.ModuleWheelMotor>())
                        {
                            pm2.motorInverted = !pm2.motorInverted;
                        }


                        break;
                    }
                case 83: //differential steering on
                    {
                        foreach (ModuleWheels.ModuleWheelMotorSteering pm2 in pm.part.Modules.OfType<ModuleWheels.ModuleWheelMotorSteering>())
                        {
                            pm2.steeringEnabled = true;
                        }
                        break;
                    }
                case 84: //differential steering off
                    {
                        foreach (ModuleWheels.ModuleWheelMotorSteering pm2 in pm.part.Modules.OfType<ModuleWheels.ModuleWheelMotorSteering>())
                        {
                            pm2.steeringEnabled = false;
                        }
                        break;
                    }
                case 85: //differential steering toggle
                    {
                        foreach (ModuleWheels.ModuleWheelMotorSteering pm2 in pm.part.Modules.OfType<ModuleWheels.ModuleWheelMotorSteering>())
                        {
                            pm2.steeringEnabled = !pm2.steeringEnabled;
                        }
                        break;
                    }
                case 86: //differential steering direction normal
                    {
                        foreach (ModuleWheels.ModuleWheelMotorSteering pm2 in pm.part.Modules.OfType<ModuleWheels.ModuleWheelMotorSteering>())
                        {
                            pm2.steeringInvert = false;
                        }
                        break;
                    }
                case 87: //differential steering direction invert
                    {
                        foreach (ModuleWheels.ModuleWheelMotorSteering pm2 in pm.part.Modules.OfType<ModuleWheels.ModuleWheelMotorSteering>())
                        {
                            pm2.steeringInvert = true;
                        }
                        break;
                    }
                case 88: //differential steering direction toggle
                    {
                        foreach (ModuleWheels.ModuleWheelMotorSteering pm2 in pm.part.Modules.OfType<ModuleWheels.ModuleWheelMotorSteering>())
                        {
                            pm2.steeringInvert = !pm2.steeringInvert;
                        }
                        break;
                    }
                case 89: //normal steering on
                    {
                        foreach (ModuleWheels.ModuleWheelSteering pm2 in pm.part.Modules.OfType<ModuleWheels.ModuleWheelSteering>())
                        {
                            pm2.steeringEnabled = true;
                        }
                        break;
                    }
                case 90: //normal steering of
                    {
                        foreach (ModuleWheels.ModuleWheelSteering pm2 in pm.part.Modules.OfType<ModuleWheels.ModuleWheelSteering>())
                        {
                            pm2.steeringEnabled = false;
                        }
                        break;
                    }
                case 91: //normal steering toggle
                    {
                        foreach (ModuleWheels.ModuleWheelSteering pm2 in pm.part.Modules.OfType<ModuleWheels.ModuleWheelSteering>())
                        {
                            pm2.steeringEnabled = !pm2.steeringEnabled;
                        }
                        break;
                    }
                case 92: //normal steering direction
                    {
                        foreach (ModuleWheels.ModuleWheelSteering pm2 in pm.part.Modules.OfType<ModuleWheels.ModuleWheelSteering>())
                        {
                            pm2.steeringInvert = false;
                        }
                        break;
                    }
                case 93: //normal steering invert
                    {
                        foreach (ModuleWheels.ModuleWheelSteering pm2 in pm.part.Modules.OfType<ModuleWheels.ModuleWheelSteering>())
                        {
                            pm2.steeringInvert = true;
                        }
                        break;
                    }
                case 94: //normal steering invert toggle
                    {
                        foreach (ModuleWheels.ModuleWheelSteering pm2 in pm.part.Modules.OfType<ModuleWheels.ModuleWheelSteering>())
                        {
                            pm2.steeringInvert = !pm2.steeringInvert;
                        }
                        break;
                    }
            } //close switch bracket
            MonoBehaviorMethods.resetPartWindows();
        }
    }

    public class MonoBehaviorMethods : MonoBehaviour
    {
        public static void setSASUI(int mode)
        {
            KSP.UI.Screens.Flight.SASDisplay SASdisp = FindObjectOfType<KSP.UI.Screens.Flight.SASDisplay>();//.modeButtons;
            //SASbtns.ElementAt<RUIToggleButton>(mode).SetTrue(true, true);

        }

        public static void resetPartWindows()
        {
            UIPartActionWindow[] partWins = FindObjectsOfType<UIPartActionWindow>();
            foreach (UIPartActionWindow partWin in partWins)
            {
                partWin.displayDirty = true;
            }
        }
    }
}
