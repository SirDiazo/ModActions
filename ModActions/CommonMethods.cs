using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;
using System.Reflection;
using KSP.UI.Screens;

namespace ModActions
{

    public class ModActionData
    {
        
        public int Identifier; //unique identifier, used for SWITCH statement
        public string ModuleName; //partModule name for filter, show this action if selected part has this module
        public string Description; //action group description, used for BaseAction.guiName, first column (is editable), resets when ActionValue type changes to default action value
        public string Name; //Mod name, second column (is selectable)
        public string ActionGroup; //Action group, third column (is selectable), break up large mods such as mechjeb into regions
        public string ActionActual; //our actual action, fourth column
        public string ActionValue; //default value, if non-editable display as lable, otherwise as text box, fifth column
        public string ActionDataType; //type of last column?

        public override string ToString() //give this class a useful ToString() function
        {
            return Identifier.ToString() + " " + ModuleName + " " + Description + " " + Name + " " + ActionGroup + " " + ActionActual + " " + ActionValue + " " + ActionDataType.ToString();
        }

        public ModActionData() //blank constructor
        {

        }

        public ModActionData(ModActionData orig) //copy constructor
        {
            //ModActionData copy = new ModActionData();
            Identifier = orig.Identifier;
            ModuleName = string.Copy(orig.ModuleName);
            Description = string.Copy(orig.Description);
            Name = string.Copy(orig.Name);
            ActionGroup = string.Copy(orig.ActionGroup);
            ActionActual = string.Copy(orig.ActionActual);
            ActionValue = string.Copy(orig.ActionValue);
            ActionDataType = string.Copy(orig.ActionDataType);

        }

    }


    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class ModActionsMainMenu : PartModule
    {
        public void Start()
        {
            Debug.Log("ModActions Ver. 1.4b Starting.....");
            if (!StaticMethods.ListPopulated) //populate our list if this is first load
            {
                StaticMethods.AllActionsList = new List<ModActionData>();
                string[] configFiles = Directory.GetFiles(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "GameData/Diazo/ModActions"); //full path of all files in save dir
                foreach (string str in configFiles)
                {
                    if (str.EndsWith(".actions"))
                    {
                        ConfigNode loadingNode = ConfigNode.Load(str);
                        foreach (AssemblyLoader.LoadedAssembly Asm in AssemblyLoader.loadedAssemblies)
                        {
                            if (Asm.dllName == loadingNode.GetValue("assemblyname") || "Stock" == loadingNode.GetValue("assemblyname"))
                            {
                                string modName = loadingNode.GetValue("modname");
                                string pmName = loadingNode.GetValue("pmname");
                                foreach (ConfigNode actNode in loadingNode.nodes)
                                {
                                    string actgroup = actNode.GetValue("name");
                                    foreach (ConfigNode typeNode in actNode.nodes)
                                    {
                                        StaticMethods.AllActionsList.Add(new ModActionData() { Identifier = int.Parse(typeNode.GetValue("ident")), ModuleName = pmName, Description = "", Name = modName, ActionGroup = actgroup, ActionActual = typeNode.GetValue("name"), ActionValue = typeNode.GetValue("data"), ActionDataType = typeNode.GetValue("ActionData") });
                                    }
                                }
                                loadingNode.SetValue("assemblyname", "gibberishToPreventThisFileFromLoadingTwice");
                            }
                        }

                    }
                }
                StaticMethods.pmTypes = new Dictionary<string, Type>();
                foreach (AssemblyLoader.LoadedAssembly Asm in AssemblyLoader.loadedAssemblies)
                {
                    Type[] typeList = Asm.assembly.GetTypes();
                    foreach (Type t in typeList)
                    {
                        if (t.IsSubclassOf(typeof(PartModule)) && !StaticMethods.pmTypes.ContainsKey(t.Name))
                        {
                            StaticMethods.pmTypes.Add(t.Name, t);
                        }
                    }
                }
                StaticMethods.ListPopulated = true;
                //Debug.Log("ModActs Type Count is " + StaticMethods.pmTypes.Count);

                //foreach (ModActionData md in StaticMethods.AllActionsList) //for debugging, lists all actions
                //{
                //    Debug.Log("ModAction " + md.ToString());
                //}
            }
        }
    }

    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class ModActionsEditor : PartModule
    {
        private bool buttonCreated = false; MainGUIWindow ourWin;
        ConfigNode settings;
        float winTop;
        float winLeft;
        Part lastSelectedPart;
        IButton MABtn;
        bool showWin;
        ApplicationLauncherButton ModActsEditorButton;
        float lastUpdateTime;

        public void Start()
        {
            GameEvents.onEditorScreenChange.Add(WinChangeAction);
            settings = ConfigNode.Load(KSPUtil.ApplicationRootPath + "GameData/Diazo/ModActions/ModActions.settings");
            winTop = float.Parse(settings.GetValue("EdWinTop"));
            winLeft = float.Parse(settings.GetValue("EdWinLeft"));
            if (ToolbarManager.ToolbarAvailable) //check if toolbar available, load if it is
            {

                MABtn = ToolbarManager.Instance.add("ModActs", "MABtn");
                MABtn.TexturePath = "Diazo/ModActions/Btn";
                MABtn.ToolTip = "Mod Actions";
                MABtn.OnClick += (e) =>
                {
                    showWin = !showWin;
                    if (ourWin != null)
                    {
                        ourWin.drawWin = showWin;
                    }
                };
            }
            else
            {
                //now using stock toolbar as fallback
                //ModActsEditorButton = ApplicationLauncher.Instance.AddModApplication(onStockToolbarClick, onStockToolbarClick, DummyVoid, DummyVoid, DummyVoid, DummyVoid, ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH, (Texture)GameDatabase.Instance.GetTexture("Diazo/ModActions/BtnStock", false));
                StartCoroutine("AddButtons");
            }
        }

        IEnumerator AddButtons()
        {
            while (!ApplicationLauncher.Ready)
            {
                yield return null;
            }
            if (!buttonCreated)
            {
                ModActsEditorButton = ApplicationLauncher.Instance.AddModApplication(onStockToolbarClick, onStockToolbarClick, DummyVoid, DummyVoid, DummyVoid, DummyVoid, ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH, (Texture)GameDatabase.Instance.GetTexture("Diazo/ModActions/BtnStock", false));
                //GameEvents.onGUIApplicationLauncherReady.Remove(AddButtons);
                //CLButton.onLeftClick(StockToolbarClick);
                //CLButton.onRightClick = (Callback)Delegate.Combine(CLButton.onRightClick, rightClick); //combine delegates together
                buttonCreated = true;
            }
        }

        public void onStockToolbarClick()
        {
            //Debug.Log("Modacts buton clik");
            showWin = !showWin;
            if (ourWin != null)
            {
                ourWin.drawWin = showWin;
            }
        }

        public void DummyVoid()
        {

        }

        public void OnGUI()
        {
            //Debug.Log("Modacts calling GUI!" + ourWin.copyMode);
            if(showWin && ourWin != null)
            { 
                //Debug.Log("Modacts calling GUI!2");
                ourWin.OnGUI();
            }
        }

        public void OnDisable()
        {


            if (ourWin != null)
            {
                winTop = ourWin.MainWindowRect.y;
                winLeft = ourWin.MainWindowRect.x;
                ourWin.drawWin = false;
                //ourWin.Kill();
            }
            settings.RemoveValue("EdWinTop");
            settings.RemoveValue("EdWinLeft");
            settings.AddValue("EdWinTop", winTop);
            settings.AddValue("EdWinLeft", winLeft);
            settings.Save(KSPUtil.ApplicationRootPath + "GameData/Diazo/ModActions/ModActions.settings");
            ourWin = null;
            if (ToolbarManager.ToolbarAvailable) //if toolbar loaded, destroy button on leaving scene
            {
                MABtn.Destroy();
            }
            else
            {
                ApplicationLauncher.Instance.RemoveModApplication(ModActsEditorButton);
            }
        }

        public void WinChangeAction(EditorScreen scrn)
        {
            //Debug.Log("ModActs win change");
            if (EditorLogic.fetch.editorScreen == EditorScreen.Actions)
            {
               // Debug.Log("ModActs win change1");
                if (ourWin == null) //initialize our window if not already extant, this event triggers twice per panels change
                {
                    ourWin = new MainGUIWindow(EditorLogic.SortedShipList, winTop, winLeft,true);                   
                    ourWin.drawWin = showWin;
                    try //getselectedparts returns null somewhere above it in the hierchy, do it this way for simplicities sake
                    {
                       // Debug.Log("ModActs win change1a1");
                        ourWin.SetPart(EditorActionGroups.Instance.GetSelectedParts().First());
                       // Debug.Log("ModActs win change1a2");
                        lastSelectedPart = EditorActionGroups.Instance.GetSelectedParts().First();
                    }
                    catch
                    {
                      //  Debug.Log("ModActs win change1a");
                        ourWin.SetPart(null);
                        lastSelectedPart = null;
                    }
                }
            }
            else //moving away from actions panel, null our window
            {
               // Debug.Log("ModActs win chang2e");
                if (ourWin != null)
                {
                    ourWin.drawWin = false;
                    //ourWin.Kill();
                }
                ourWin = null;
            }
        }
        public void Update()
        {
            if (EditorLogic.fetch.editorScreen == EditorScreen.Actions)
            {
                try
                {
                    //Debug.Log("MA 1");
                    if (EditorActionGroups.Instance.GetSelectedParts().First() != lastSelectedPart) //check if selected part has changed
                    {
                       //Debug.Log("MA 2");
                       foreach (Part p in EditorActionGroups.Instance.GetSelectedParts())
                       {
                           //Debug.Log("MA " + p.ToString() + "|" + EditorActionGroups.Instance.GetSelectedParts().Count);
                       }
                        ourWin.SetPart(EditorActionGroups.Instance.GetSelectedParts().First());
                        //Debug.Log("MA 2a");
                        lastSelectedPart = EditorActionGroups.Instance.GetSelectedParts().First(); 
                       // Debug.Log("MA 2b");
                    }
                }
                catch(Exception e) //error trap if GetSelecetedParts above is null,
                {
                    //Debug.Log("MA Get sle parts null " + e);
                    //do nothing, if null nothing should happen
                }

                if (Time.time > lastUpdateTime + 5 && ourWin != null)
                {
                    ourWin.UpdateCheck();
                    lastUpdateTime = Time.time;
                }

            }
            if (ourWin != null)
            {
                ourWin.Update();
            }
            //foreach(Part p in EditorLogic.SortedShipList)
            //{
            //    Debug.Log("MA start");
            //    if(p.Modules.Contains<ModuleRCS>())
            //    {
            //        Debug.Log("MA " + p.Modules.OfType<ModuleModActions>().First().modActionsList.Count);
            //    }
            //}
            //foreach(Part p in EditorLogic.SortedShipList)
            //{
            //    foreach(PartModule pm in p.Modules)
            //    {
            //        Debug.Log("MA " + p.partName + "|" + pm.moduleName);
            //    }
            //}
        }
    }

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class ModActionsFlight : PartModule
    {
        private bool buttonCreated = false;
        MainGUIWindow ourWin;
        ConfigNode settings;
        float winTop;
        float winLeft;
        Part lastSelectedPart;
        bool ShowModActs;
        IButton MABtn;
        ApplicationLauncherButton ModActsFlightButton;
        float lastUpdateTime;
        public bool showKSPui = true;
        bool showBtn = true;

        public void Start()
        {

            settings = ConfigNode.Load(KSPUtil.ApplicationRootPath + "GameData/Diazo/ModActions/ModActions.settings");
            winTop = float.Parse(settings.GetValue("FltWinTop"));
            winLeft = float.Parse(settings.GetValue("FltWinLeft"));
            if(int.Parse(settings.GetValue("FlightBtnVis"))==0) //0 hide win, 1 show win, 2 (or any other value) only show if agx installed
            {
                showBtn = false;
            }
            else if(int.Parse(settings.GetValue("FlightBtnVis")) == 1)
            {
                showBtn = true;
            }
            else
            {
                showBtn = false;
                foreach (AssemblyLoader.LoadedAssembly Asm in AssemblyLoader.loadedAssemblies)
                {
                    if (Asm.dllName == "AGExt")
                    {
                        //Debug.Log("RemoteTech found");
                        //AGXRemoteTechQueue.Add(new AGXRemoteTechQueueItem(group, FlightGlobals.ActiveVessel.rootPart.flightID, Planetarium.GetUniversalTime() + 10, force, forceDir));
                        showBtn = true;
                    }
                }
            }
            if (showBtn)
            {
                if (ToolbarManager.ToolbarAvailable) //check if toolbar available, load if it is
                {

                    MABtn = ToolbarManager.Instance.add("ModActs", "MABtn");
                    MABtn.TexturePath = "Diazo/ModActions/Btn";
                    MABtn.ToolTip = "Mod Actions";
                    MABtn.OnClick += (e) =>
                    {
                        ShowModActs = !ShowModActs;
                        if (ShowModActs)
                        {
                            if (ourWin == null)
                            {
                                ourWin = new MainGUIWindow(FlightGlobals.ActiveVessel.Parts, winTop, winLeft, showKSPui);
                            }
                            ourWin.SetPart(null);
                            lastSelectedPart = null;
                            ourWin.drawWin = ShowModActs;
                        }
                        else
                        {
                            if (ourWin != null)
                            {
                                ourWin.drawWin = false;
                            //ourWin.Kill();
                            ourWin = null;
                            }
                        }
                    };
                }
                else
                {
                    //now using stock toolbar as fallback
                    //ModActsFlightButton = ApplicationLauncher.Instance.AddModApplication(onStockToolbarClick, onStockToolbarClick, DummyVoid, DummyVoid, DummyVoid, DummyVoid, ApplicationLauncher.AppScenes.FLIGHT, (Texture)GameDatabase.Instance.GetTexture("Diazo/ModActions/BtnStock", false));
                    StartCoroutine("AddButtons");
                }
            }
            GameEvents.onHideUI.Add(onHideMyUI);
            GameEvents.onShowUI.Add(onShowMyUI);
        }

        void onHideMyUI()
        {
            if(ourWin != null)
            {
                ourWin.showKSPui = false;
            }
            showKSPui = false;
        }
        void onShowMyUI()
        {
            if(ourWin != null)
            {
                ourWin.showKSPui = true;
            }
            showKSPui = true;
        }

        IEnumerator AddButtons()
        {
            while (!ApplicationLauncher.Ready)
            {
                yield return null;
            }
            if (!buttonCreated)
            {
                ModActsFlightButton = ApplicationLauncher.Instance.AddModApplication(onStockToolbarClick, onStockToolbarClick, DummyVoid, DummyVoid, DummyVoid, DummyVoid, ApplicationLauncher.AppScenes.FLIGHT, (Texture)GameDatabase.Instance.GetTexture("Diazo/ModActions/BtnStock", false));
                //GameEvents.onGUIApplicationLauncherReady.Remove(AddButtons);
                //CLButton.onLeftClick(StockToolbarClick);
                //CLButton.onRightClick = (Callback)Delegate.Combine(CLButton.onRightClick, rightClick); //combine delegates together
                buttonCreated = true;
            }
        }

        public void OnGUI()
        {
            if(ShowModActs && ourWin != null)
            {
                ourWin.OnGUI();
            }
        }

        public void onStockToolbarClick()
        {
            string errLine = "1";
            try
            {
                errLine = "2";
                ShowModActs = !ShowModActs;
                errLine = "3";
                if (ShowModActs)
                {
                    errLine = "4";
                    if (ourWin == null)
                    {
                        //Debug.Log("make win");
                        errLine = "5";
                        ourWin = new MainGUIWindow(FlightGlobals.ActiveVessel.Parts, winTop, winLeft, showKSPui);
                    }
                    errLine = "6";
                    ourWin.SetPart(null);
                    errLine = "7";
                    lastSelectedPart = null;
                    errLine = "8";
                    ourWin.drawWin = ShowModActs;
                }
                else
                {
                    errLine = "9";
                    if (ourWin != null)
                    {
                        errLine = "10";
                        ourWin.drawWin = false;
                        //ourWin.Kill();
                        ourWin = null;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("ModActs StockBtnClick " + errLine + " " + e);
            }
        }

        public void DummyVoid()
        {

        }

        public void OnDisable()
        {



            if (ourWin != null)
            {
               // Debug.Log("ModActions Flight Dis b");
                winTop = ourWin.MainWindowRect.y;
                winLeft = ourWin.MainWindowRect.x;
                ourWin.drawWin = false;
                //ourWin.Kill();
            }
            settings.RemoveValue("FltWinTop");
            settings.RemoveValue("FltWinLeft");
            settings.AddValue("FltWinTop", winTop);
            settings.AddValue("FltWinLeft", winLeft);
            //Debug.Log("ModActions Flight Dis A");
            settings.Save(KSPUtil.ApplicationRootPath + "GameData/Diazo/ModActions/ModActions.settings");
          //  Debug.Log("ModActions Flight Dis c");
            ourWin = null;
            if (ToolbarManager.ToolbarAvailable) //if toolbar loaded, destroy button on leaving scene
            {
                //Debug.Log("ModActions Flight Dis d");
                MABtn.Destroy();
            }
            else
            {
                //Debug.Log("ModActions Flight Dis e");
                ApplicationLauncher.Instance.RemoveModApplication(ModActsFlightButton);
            }
           // Debug.Log("ModActions Flight Dis f");
        }


        public void Update()
        {

            if (ourWin != null)
            {
                if (ourWin.selectedPart != lastSelectedPart) //check if selected part has changed
                {
                    ourWin.SetPart(lastSelectedPart);
                }

                //if (ourWin != null)
                //{
                //    ourWin.Update();
                //}

                if (Time.time > lastUpdateTime + 5 && ourWin != null)
                {
                    ourWin.UpdateCheck();
                    lastUpdateTime = Time.time;
                }

                if (Input.GetKeyDown(KeyCode.Mouse0) && ShowModActs)
                {

                    Part selPart = new Part();
                    selPart = SelectPartUnderMouse();
                    if (selPart != null)
                    {
                        lastSelectedPart = selPart;
                    }
                }
            }
            //foreach(Part p in FlightGlobals.ActiveVessel.Parts)
            //{
            //    Debug.Log("modacts " + p.name + " " + p.HighlightActive + "|" + Mouse.HoveredPart); 
            //}
        }

        public Part SelectPartUnderMouse()
        {
            //FlightCamera CamTest = new FlightCamera();
            //CamTest = FlightCamera.fetch;
            //Ray ray = CamTest.mainCamera.ScreenPointToRay(Input.mousePosition);
            //LayerMask RayMask = new LayerMask();
            //RayMask = 1 << 0;
            //RaycastHit hit;
            //if (Physics.Raycast(ray, out hit, Mathf.Infinity, RayMask))
            //{
            //    Part hitPart = (Part)UIPartActionController.GetComponentUpwards("Part", hit.collider.gameObject); //how to find small parts that are "inside" the large part they are attached to.
            //    if (FlightGlobals.ActiveVessel.parts.Contains(hitPart))
            //    {
            //        return hitPart;
            //    }
            //    else
            //    {
            //        return null;
            //    }
            //    //return FlightGlobals.ActiveVessel.Parts.Find(p => p.gameObject == hit.transform.gameObject);
            //}
            //return null;

            return Mouse.HoveredPart;
        }
    }

    public static class StaticMethods //static data that should never change
    {
        public static bool ListPopulated = false;
        public static List<ModActionData> AllActionsList;
        public static Dictionary<string, Type> pmTypes;
    }

    //public class VslResTest
    //{
    //    Dictionary<Vessel, Dictionary<string, double>> allVesselResources;

    //    public void Update() //replace with your trigger
    //    {
    //        foreach (Vessel vsl in FlightGlobals.Vessels) //cycles all vessels in game, will probably nullref on vessels outside physics range
    //        {
    //            if(allVesselResources.ContainsKey(vsl)) //if vessel is present in our saved data remove it to avoid duplicate data
    //            {
    //                allVesselResources.Remove(vsl);
    //            }
    //            allVesselResources.Add(vsl,new Dictionary<string,double>()); //note the new, that zeros all values for the vessel
    //            Dictionary<string, double> vesselResources = allVesselResources[vsl]; //create a shortcut reference to this vessel's dictionary
    //            foreach(Part p in vsl.parts) //cycle through parts on the current vessel
    //            {
    //                foreach(PartResource pRes in p.Resources) //cycle through all resources on this part
    //                {
    //                    if(!vesselResources.ContainsKey(pRes.resourceName)) //check if resource exists already
    //                    {
    //                        vesselResources.Add(pRes.resourceName, 0f); //add resources with zero amount if it doesn't exist
    //                    }
    //                    vesselResources[pRes.resourceName] += pRes.amount;
    //                }
    //            }
    //        } //close foreach cycling through vessels
    //        Debug.Log("Electric charge on focus vessel is " + allVesselResources[FlightGlobals.ActiveVessel].["ElectricCharge"]);
    //    }
    //}


}
