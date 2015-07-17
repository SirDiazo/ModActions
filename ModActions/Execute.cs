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
                case 40:
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
                        pm.part.Modules["GPOSpeedPump"].Fields.SetValue("AutoPump", true);
                        break;
                    }
                case 45: //pump off
                    {
                        pm.part.Modules["GPOSpeedPump"].Fields.SetValue("AutoPump", false);
                        break;
                    }
                case 46: //pump toggle
                    {
                        if ((bool)pm.part.Modules["GPOSpeedPump"].Fields.GetValue("AutoPump"))
                        {
                            pm.part.Modules["GPOSpeedPump"].Fields.SetValue("AutoPump", false);
                        }
                        else
                        {
                            pm.part.Modules["GPOSpeedPump"].Fields.SetValue("AutoPump", true);
                        }
                        break;
                    }
                case 47: //balance on
                    {
                        pm.part.Modules["GPOSpeedPump"].Fields.SetValue("AutoBalance", true);
                        break;
                    }
                case 48: //bal off
                    {
                        pm.part.Modules["GPOSpeedPump"].Fields.SetValue("AutoBalance", false);
                        break;
                    }
                case 49: //bal toggle
                    {
                        if ((bool)pm.part.Modules["GPOSpeedPump"].Fields.GetValue("AutoBalance"))
                        {
                            pm.part.Modules["GPOSpeedPump"].Fields.SetValue("AutoBalance", false);
                        }
                        else
                        {
                            pm.part.Modules["GPOSpeedPump"].Fields.SetValue("AutoBalance", true);
                        }
                        break;
                    }
                case 50: //set pump level
                    {
                        int setpoint;
                        if (int.TryParse(val, out setpoint))
                        {
                            pm.part.Modules["GPOSpeedPump"].Fields.SetValue("PumpLevel", Mathf.Clamp((float)setpoint,0f,16f));
                        }
                        break;
                    }
                case 51: //change pump level
                    {
                        int setpoint;
                        if (int.TryParse(val, out setpoint))
                        {
                            float setpoint2 = (float)pm.part.Modules["GPOSpeedPump"].Fields.GetValue("PumpLevel") + (float)setpoint;
                            pm.part.Modules["GPOSpeedPump"].Fields.SetValue("PumpLevel", Mathf.Clamp(setpoint2, 0f, 16f));
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
                
            } //close switch bracket
        }
    }

    public class MonoBehaviorMethods : MonoBehaviour
    {
        public static void setSASUI(int mode)
        {
            RUIToggleButton[] SASbtns = FindObjectOfType<VesselAutopilotUI>().modeButtons;
            SASbtns.ElementAt<RUIToggleButton>(mode).SetTrue(true, true);
        }
    }
}
