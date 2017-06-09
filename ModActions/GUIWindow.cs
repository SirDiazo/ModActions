﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;

namespace ModActions
{
    class MainGUIWindow//DO NOT INHERIT ANYTHING, BREAKS THIS CLASS FOR REASONS UNKNOWN, think it gets garbage collected too soon?
    {

        //public bool enabled = true;
        public Rect MainWindowRect; //main GUI window rectangle
        public List<Part> parts; //list of parts of our current vessel
        public Part selectedPart; //our selected part
        Texture2D partLoc;
        Texture2D buttonGray;
        Texture2D buttonRed;
        Texture2D buttonGreen;
        bool includeSymmetryParts = true;
        GUISkin thisSkin;
        Vector2 MainWinScroll;
        ModuleModActions dataPM;
        int selectingID;
        SelectType selType;
        List<string> allModNames;
        List<String> modNames;
        List<String> actionGroupNames;
        Vector2 savedScrollLocation;
        public bool drawWin;
        string selectingParts;
        string selectingPartsName;
        Camera cameraPos;
        public bool copyMode;
        Part copyPart;
        public bool showKSPui = true;
        


        public MainGUIWindow(List<Part> prts, float winTop, float winLeft, bool showingKSPui) //effectively our Start() method
        {
            
            savedScrollLocation = new Vector2(0, 0);
            //RenderingManager.AddToPostDrawQueue(5, ModActsDraw);
            MainWindowRect = new Rect(winLeft, winTop, 530, 135);
            parts = prts;
            modNames = new List<String>();
            actionGroupNames = new List<string>();
            partLoc = new Texture2D(21, 21);
            //partLoc = GameDatabase.Instance.databaseTexture.Find(tx => tx.name == "PartLocCircle").texture;
            partLoc = GameDatabase.Instance.GetTexture("Diazo/ModActions/PartLocCircle", false);
            buttonGray = GameDatabase.Instance.GetTexture("Diazo/ModActions/ButtonTexture", false);
            buttonRed = GameDatabase.Instance.GetTexture("Diazo/ModActions/ButtonTextureRed", false);
            buttonGreen = GameDatabase.Instance.GetTexture("Diazo/ModActions/ButtonTextureGreen", false);
            thisSkin = (GUISkin)MonoBehaviour.Instantiate(HighLogic.Skin);
            thisSkin.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            thisSkin.label.fontStyle = FontStyle.Normal;
            thisSkin.label.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            thisSkin.scrollView.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            thisSkin.label.alignment = TextAnchor.MiddleCenter;
            thisSkin.button.normal.background = buttonGray;
            thisSkin.button.hover.background = buttonGray;
            thisSkin.button.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            thisSkin.button.hover.textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            thisSkin.button.fontStyle = FontStyle.Normal;
            thisSkin.textField.fontStyle = FontStyle.Normal;
            thisSkin.textField.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            thisSkin.textField.hover.textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            MainWinScroll = new Vector2(0, 0);
            allModNames = new List<string>();
            foreach (ModActionData mData in StaticMethods.AllActionsList)
            {
                if (!allModNames.Contains(mData.Name))
                {
                    allModNames.Add(mData.Name);
                }
            }
            allModNames.Sort();
            if (HighLogic.LoadedSceneIsEditor)
            {
                cameraPos = EditorLogic.fetch.editorCamera;
            }
            else
            {
                cameraPos = FlightCamera.fetch.mainCamera;
            }
            selType = SelectType.NoPart;
            //Debug.Log("init okay");
            showKSPui = showingKSPui;

        }

        //public void Kill() //effectively our OnDisable() method
        //{
        //    RenderingManager.RemoveFromPostDrawQueue(0, ModActsDraw);

        //}

        public void OnGUI()
        {
            //Debug.Log("Modacts GUI method");
            string errLine = "1";
            try
            {
                errLine = "2";
                if (drawWin && showKSPui)
                {
                    errLine = "3";
                    MainWindowRect = GUI.Window(67346779, MainWindowRect, DrawMainWindow, ""); //our main window
                    errLine = "4";
                    List<Vector3> partScreenPos = new List<Vector3>();
                    errLine = "5";
                    if (selType == SelectType.NoPart) //draw circle position icons
                    {
                        errLine = "6";
                        if (selectingParts != null)
                        {
                            errLine = "7";
                            foreach (Part p in parts)
                            {
                                errLine = "8";
                                bool addThisPM = false;
                                errLine = "8a";
                                foreach (PartModule pm3 in p.Modules)
                                {
                                    errLine = "8b";
                                    if (StaticMethods.pmTypes[selectingParts].IsAssignableFrom(pm3.GetType()))
                                    {
                                        errLine = "8c";
                                        addThisPM = true;
                                    }
                                }
                                errLine = "8d";
                                //if (p.Modules.Contains(selectingParts))
                                if (addThisPM)
                                {
                                    errLine = "9";
                                    partScreenPos.Add(cameraPos.WorldToScreenPoint(p.transform.position));
                                }
                            }
                        }
                    }
                    else if (selectedPart != null)
                    {
                        errLine = "10";
                        partScreenPos.Add(cameraPos.WorldToScreenPoint(selectedPart.transform.position));
                        errLine = "11";
                        if (includeSymmetryParts)
                        {
                            errLine = "12";
                            foreach (Part p in selectedPart.symmetryCounterparts)
                            {
                                errLine = "3";
                                partScreenPos.Add(cameraPos.WorldToScreenPoint(p.transform.position));
                            }
                        }
                    }
                    errLine = "14";
                    foreach (Vector3 vect in partScreenPos)
                    {
                        errLine = "15";
                        Rect partCenterWinD = new Rect(vect.x - 15, (Screen.height - vect.y) - 15, 31, 31);
                        errLine = "16";
                        GUI.DrawTexture(partCenterWinD, partLoc);
                    }
                    errLine = "17";
                }
            }
            catch (Exception e)
            {
                Debug.Log("ModActs ModActs Draw Fail " + errLine + " " + e);
            }
        }

        public void DrawMainWindow(int WindowID)
        {

            string errLine = "1";
            try
            {
                thisSkin.label.fontStyle = FontStyle.Bold;
                GUI.Label(new Rect(10, 5, 100, 20), "Description", thisSkin.label);
                GUI.Label(new Rect(110, 5, 100, 20), "Mod", thisSkin.label);
                //GUI.Label(new Rect(210, 5, 100, 20), "Action Type", thisSkin.label); //moved other stuff 50 pixels
                thisSkin.label.fontStyle = FontStyle.Normal;
                errLine = "2";
                if (GUI.Button(new Rect(250, 5, 100, 20), "Clear Selection", thisSkin.button))
                {
                    SetPart(null);
                    selectingParts = null;
                    selectingPartsName = null;
                }
                if (includeSymmetryParts)
                {
                    thisSkin.button.normal.background = buttonGreen;
                    thisSkin.button.hover.background = buttonGreen;
                }
                else
                {
                    thisSkin.button.normal.background = buttonRed;
                    thisSkin.button.hover.background = buttonRed;
                }
                errLine = "3";


                if (GUI.Button(new Rect(350, 5, 100, 20), includeSymmetryParts ? "Symmetry: Yes" : "Symmetry: No", thisSkin.button))
                {
                    includeSymmetryParts = !includeSymmetryParts;
                }
                thisSkin.button.normal.background = buttonGray;
                thisSkin.button.hover.background = buttonGray;
                errLine = "4";

                if (copyMode)
                {
                    thisSkin.button.normal.background = buttonRed;
                    thisSkin.button.hover.background = buttonRed;
                }
                
                errLine = "4a";
                if (GUI.Button(new Rect(450, 5, 80, 20), copyMode ? "Copying" : "Copy?", thisSkin.button))
                {
                    if(!copyMode)
                    {
                        copyPart = selectedPart;
                        copyMode = true;
                    }
                    else
                    {
                        copyPart = null;
                        copyMode = false;
                    }
                }
                thisSkin.button.normal.background = buttonGray;
                thisSkin.button.hover.background = buttonGray;

                MainWinScroll = GUI.BeginScrollView(new Rect(5, 25, 520, 105), MainWinScroll, new Rect(0, 0, 500, 600));

                if (selType == SelectType.NoPart)
                {

                    errLine = "5";
                    GUI.Label(new Rect(5, 5, 500, 20), "Select a part on the vessel, or a mod below to see its associated parts:", thisSkin.label);
                    if (allModNames.Count > 0)
                    {
                        for (int iVert = 1; iVert <= 20; iVert++)
                        {
                            for (int iHoriz = 1; iHoriz <= 5; iHoriz++)
                            {
                                int buttonCount = iHoriz + ((iVert - 1) * 5);
                                if (allModNames.ElementAt(buttonCount - 1) == selectingPartsName)
                                {
                                    thisSkin.button.normal.background = buttonGreen;
                                    thisSkin.button.hover.background = buttonGreen;
                                }
                                else
                                {
                                    thisSkin.button.normal.background = buttonGray;
                                    thisSkin.button.hover.background = buttonGray;
                                }
                                if (GUI.Button(new Rect((iHoriz - 1) * 100, (iVert) * 20, 100, 20), allModNames.ElementAt(buttonCount - 1), thisSkin.button))
                                {
                                    selectingParts = StaticMethods.AllActionsList.Where(md => md.Name == allModNames.ElementAt(buttonCount - 1)).First().ModuleName;
                                    selectingPartsName = allModNames.ElementAt(buttonCount - 1);
                                }
                                thisSkin.button.normal.background = buttonGray;
                                thisSkin.button.hover.background = buttonGray;


                                if (buttonCount == allModNames.Count())
                                {
                                    goto Breakout; //all buttons drawn, need to breakout of the loop
                                }
                            }
                        }
                    }
                    else
                    {
                        GUI.Label(new Rect(5, 35, 500, 20), "ModActions Critial error, no actions loaded.", thisSkin.label);
                        GUI.Label(new Rect(5, 65, 500, 20), "Check your installation path and/or file a bug report.", thisSkin.label);
                    }

                }
                else if (selType == SelectType.None) //display our actions list
                {
                    errLine = "6";
                    //Debug.Log("1");
                    for (int i = 1; i <= 30; i++)
                    {
                        errLine = "6a " + i;
                        // Debug.Log("2");
                        GUI.Label(new Rect(0, 1 + (20 * (i - 1)), 20, 20), i.ToString() + ":", thisSkin.label);
                        //Debug.Log("3");
                        if (dataPM.modActionsList.ContainsKey(i))
                        {
                            errLine = "6b";
                            //  Debug.Log("4:" + i + dataPM.modActionsList[i].Description);
                            dataPM.modActionsList[i].Description = GUI.TextField(new Rect(21, 1 + (20 * (i - 1)), 80, 20), dataPM.modActionsList[i].Description, thisSkin.textField);
                            //.Log("5");
                            if (GUI.Button(new Rect(100, 0 + (20 * (i - 1)), 100, 20), dataPM.modActionsList[i].Name, thisSkin.button))
                            {
                                errLine = "6c";
                                //Debug.Log("6");
                                selectingID = i;
                                selType = SelectType.Name;
                                savedScrollLocation = MainWinScroll;
                                MainWinScroll = new Vector2(0, 0);
                            }
                            if (GUI.Button(new Rect(200, 0 + (20 * (i - 1)), 100, 20), dataPM.modActionsList[i].ActionGroup, thisSkin.button))
                            {
                                errLine = "6d";
                                //Debug.Log("6");
                                selectingID = i;
                                selType = SelectType.ActionGroup;
                                actionGroupNames.Clear();
                                foreach (ModActionData md in StaticMethods.AllActionsList.Where(d => d.Name == dataPM.modActionsList[i].Name))
                                {
                                    if (!actionGroupNames.Contains(md.ActionGroup))
                                    {
                                        actionGroupNames.Add(md.ActionGroup);
                                    }
                                }
                                savedScrollLocation = MainWinScroll;
                                MainWinScroll = new Vector2(0, 0);
                            }
                            if (GUI.Button(new Rect(300, 0 + (20 * (i - 1)), 100, 20), dataPM.modActionsList[i].ActionActual, thisSkin.button))
                            {
                                errLine = "6e";
                                //Debug.Log("6");
                                selectingID = i;
                                selType = SelectType.ActionActual;
                                actionGroupNames.Clear();
                                foreach (ModActionData md in StaticMethods.AllActionsList.Where(d => d.Name == dataPM.modActionsList[i].Name && d.ActionGroup == dataPM.modActionsList[i].ActionGroup))
                                {
                                    if (!actionGroupNames.Contains(md.ActionActual))
                                    {
                                        actionGroupNames.Add(md.ActionActual);
                                    }
                                }
                                //Debug.Log("count " + actionGroupNames.Count);
                                savedScrollLocation = MainWinScroll;
                                MainWinScroll = new Vector2(0, 0);
                            }
                            Color GUIbackup = GUI.backgroundColor;
                            if (dataPM.modActionsList[i].ActionDataType == "no")
                            {
                                errLine = "6f";//leave blank as we show nothing if this colum stays blank
                            }
                            else if (dataPM.modActionsList[i].ActionDataType == "float")
                            {
                                errLine = "6g";
                                float tempVal;
                                //Color GUIbackup = GUI.backgroundColor;
                                if (float.TryParse(dataPM.modActionsList[i].ActionValue, out tempVal))
                                {
                                    //blank, do nothing
                                }
                                else
                                {
                                    GUI.backgroundColor = new Color(1, 0, 0);
                                }
                            }
                            else if (dataPM.modActionsList[i].ActionDataType == "int")
                            {
                                errLine = "6g";
                                int tempVal;
                                //Color GUIbackup = GUI.backgroundColor;
                                if (int.TryParse(dataPM.modActionsList[i].ActionValue, out tempVal))
                                {
                                    //blank, do nothing
                                }
                                else
                                {
                                    GUI.backgroundColor = new Color(1, 0, 0);
                                }
                            }

                            if (dataPM.modActionsList[i].ActionDataType != "no")
                            {
                                string origString = string.Copy(dataPM.modActionsList[i].ActionValue);
                                dataPM.modActionsList[i].ActionValue = GUI.TextField(new Rect(400, 0 + (20 * (i - 1)), 100, 20), dataPM.modActionsList[i].ActionValue, thisSkin.textField);
                                GUI.backgroundColor = GUIbackup;
                                if (origString != dataPM.modActionsList[i].ActionValue)
                                {
                                    
                                    SyncSymmetry(dataPM.part, dataPM.modActionsList[i], i);
                                }
                            }
                            //Debug.Log("7");
                        }
                        else
                        {

                            errLine = "6h";
                            if (GUI.Button(new Rect(101, 1 + (20 * (i - 1)), 100, 20), "Click to select", thisSkin.button))
                            {
                                selectingID = i;
                                selType = SelectType.Name;
                                savedScrollLocation = MainWinScroll;
                                MainWinScroll = new Vector2(0, 0);
                            }
                        }
                    }
                }

                else if (selType == SelectType.Name)
                {

                    errLine = "7";
                    for (int iVert = 1; iVert <= 20; iVert++)
                    {
                        for (int iHoriz = 1; iHoriz <= 5; iHoriz++)
                        {
                            int buttonCount = iHoriz + ((iVert - 1) * 5);
                            if (GUI.Button(new Rect((iHoriz - 1) * 100, (iVert - 1) * 20, 100, 20), modNames.ElementAt(buttonCount - 1), thisSkin.button))
                            {
                                SelectModName(modNames.ElementAt(buttonCount - 1), selectingID);
                                MainWinScroll = savedScrollLocation;
                            }
                            if (buttonCount == modNames.Count())
                            {
                                goto Breakout; //all buttons drawn, need to breakout of the loop
                            }
                        }
                    }
                }
                else if (selType == SelectType.ActionGroup) //copied code not yet updated
                {
                    errLine = "8";
                    for (int iVert = 1; iVert <= actionGroupNames.Count; iVert++)
                    {
                        if (GUI.Button(new Rect(200, (iVert - 1) * 20, 100, 20), actionGroupNames.ElementAt(iVert - 1), thisSkin.button))
                        {
                            SelectActionName(dataPM.modActionsList[selectingID].Name, actionGroupNames.ElementAt(iVert - 1), selectingID);
                            MainWinScroll = savedScrollLocation;
                        }
                    }
                }
                else if (selType == SelectType.ActionActual) //copied code not yet updated
                {
                    errLine = "9";
                    for (int iVert = 1; iVert <= actionGroupNames.Count; iVert++)
                    {
                        if (GUI.Button(new Rect(300, (iVert - 1) * 20, 100, 20), actionGroupNames.ElementAt(iVert - 1), thisSkin.button))
                        {
                            SelectActionType(actionGroupNames.ElementAt(iVert - 1), selectingID);
                            MainWinScroll = savedScrollLocation;
                        }
                    }
                }
                errLine = "10";
            Breakout:
                errLine = "11";
                GUI.EndScrollView();
                GUI.DragWindow();
            }
            catch (Exception e)
            {
                Debug.Log("ModActs DrawWin Fail " + errLine + " " + e);
            }
        }

        public void SetPart(Part p) //called from Flight or Editor KSPAddons when selected part changes.
        {
            string errLine = "1";
            try
            {
                selectedPart = p;
                errLine = "2";
                if (selectedPart != null)
                {
                    errLine = "3";
                    dataPM = p.Modules.OfType<ModuleModActions>().First();
                    RefreshModNames(true);
                    if (copyMode && copyPart != null)
                    {
                        errLine = "4";
                        dataPM.modActionsList.Clear();
                        ModuleModActions copySource = (ModuleModActions)copyPart.Modules["ModuleModActions"];
                        foreach (KeyValuePair<int, ModActionData> kvp in copySource.modActionsList)
                        {
                            if (dataPM.part.Modules.Contains(kvp.Value.ModuleName))
                            {
                                dataPM.modActionsList.Add(kvp.Key, new ModActionData(kvp.Value));
                            }
                        }
                        errLine = "5";
                        if (includeSymmetryParts)
                        {
                            foreach (Part p3 in dataPM.part.symmetryCounterparts)
                            {
                                ModuleModActions symDataPM = (ModuleModActions)p3.Modules["ModuleModActions"];
                                symDataPM.modActionsList.Clear();
                                foreach (KeyValuePair<int, ModActionData> kvp in copySource.modActionsList)
                                {
                                    if (symDataPM.part.Modules.Contains(kvp.Value.ModuleName))
                                    {
                                        symDataPM.modActionsList.Add(kvp.Key, new ModActionData(kvp.Value));
                                    }
                                }
                            }
                        }
                    }
                    errLine = "6";
                    copyMode = false;
                    copyPart = null;
                    selType = SelectType.None;
                }
                else
                {
                    errLine = "7";
                    dataPM = null;
                    RefreshModNames(false);
                    selType = SelectType.NoPart;
                }
            }
            catch(Exception e)
            {
                Debug.Log("ModActions GuiWIN SetPart Fail " + errLine + "|" + e);
            }

        }

        public void RefreshModNames(bool validPart) //update our list of available mods for the selected part, if valid part is false, no part selected, just clear list
        {
            string errLine = "1";
            try
            {
                modNames.Clear();
                errLine = "1";
                if (validPart)
                {
                    errLine = "2";
                    foreach (ModActionData modData in StaticMethods.AllActionsList)
                    {
                        errLine = "3";
                        //Debug.Log("MA " + modData.ModuleName);
                        //foreach(KeyValuePair<string,Type> kvp in StaticMethods.pmTypes)
                        //{
                        //    Debug.Log(kvp.Value.ToString());

                        //}
                        
                        bool addThisModData = false;
                        //if (selectedPart.Modules.Contains(modData.ModuleName))
                        if (modData.ModuleName == "All")
                        {
                            addThisModData = true;
                        }
                        else
                        {
                            Type modDataType2 = StaticMethods.pmTypes[modData.ModuleName];
                            foreach (PartModule pm in selectedPart.Modules)
                            {
                                errLine = "4";
                                if (modDataType2.IsAssignableFrom(pm.GetType()))
                                {
                                    errLine = "5";
                                    addThisModData = true;
                                }
                            }
                        }
                        errLine = "6";
                        if (addThisModData)
                        {
                            errLine = "7";
                            if (!modNames.Contains(modData.Name))
                            {
                                errLine = "8";
                                modNames.Add(modData.Name);
                            }
                        }
                    }
                    errLine = "9";
                    modNames.Sort();
                }
                errLine = "10";
                modNames.Insert(0, "Clear Action"); //clear action always comes first
            }
            catch(Exception e)
            {
                Debug.Log("ModActs GUI RefreshModNames fail " + errLine + "|" + e);
            }

        }

       

        public void SelectModName(String mName, int selID) //click on second column
        {
            if (mName == "Clear Action") //delete this action
            {
                dataPM.modActionsList.Remove(selID); //delete it
                dataPM.Actions.Find(ba => ba.name == "Action" + selID.ToString()).active = false; //hide the action from the GUI
                UpdateCheck();
            }
            else //select new action
            {
                List<ModActionData> listToAdd = new List<ModActionData>();
                listToAdd.AddRange(StaticMethods.AllActionsList.Where(item => item.Name == mName)); //find all ModActionData for this mod
                if (dataPM.modActionsList.ContainsKey(selID)) //if action already exists, overwrite it
                {

                    dataPM.modActionsList.Remove(selID);
                }
                dataPM.modActionsList.Add(selID, new ModActionData(listToAdd.OrderBy(dt => dt.Identifier).First()));
                dataPM.modActionsList[selID].Description = dataPM.modActionsList[selID].ActionActual;//add the new action
                dataPM.Actions.Find(ba => ba.name == "Action" + selID.ToString()).active = true; //action should now show
                UpdateCheck();
                //dataPM.Actions.Find(ba => ba.name == "Action" + selID.ToString()).guiName = dataPM.modActionsList[selID].Description; //guiName will update later
            }
            //action is selected, return our window to showing the actions list
            selectingID = 0; //should never be check when SelectType.None is true
            selType = SelectType.None;
        }

        public void SelectActionName(String mName, string aGroup, int selID) //click on second column
        {

            List<ModActionData> listToAdd = new List<ModActionData>();
            listToAdd.AddRange(StaticMethods.AllActionsList.Where(item => item.Name == mName && item.ActionGroup == aGroup)); //find all ModActionData for this mod
            if (dataPM.modActionsList.ContainsKey(selID)) //if action already exists, overwrite it
            {
                dataPM.modActionsList.Remove(selID);
            }
            dataPM.modActionsList.Add(selID, new ModActionData(listToAdd.OrderBy(dt => dt.Identifier).First()));
            dataPM.modActionsList[selID].Description = dataPM.modActionsList[selID].ActionActual;//add the new action
            dataPM.Actions.Find(ba => ba.name == "Action" + selID.ToString()).active = true; //action should now show
            UpdateCheck();
            //dataPM.Actions.Find(ba => ba.name == "Action" + selID.ToString()).guiName = dataPM.modActionsList[selID].Description; //UpdateCheck method does this
            //Debug.Log(dataPM.modActionsList[selID].ToString());


            //action is selected, return our window to showing the actions list
            selectingID = 0; //should never be check when SelectType.None is true
            selType = SelectType.None;
            MainWinScroll = savedScrollLocation;
        }

        public void SelectActionType(string aGroup, int selID) //click on second column
        {

            List<ModActionData> listToAdd = new List<ModActionData>();
            listToAdd.AddRange(StaticMethods.AllActionsList.Where(item => item.Name == dataPM.modActionsList[selID].Name && item.ActionGroup == dataPM.modActionsList[selID].ActionGroup && item.ActionActual == aGroup)); //find all ModActionData for this mod
            if (dataPM.modActionsList.ContainsKey(selID)) //if action already exists, overwrite it
            {
                dataPM.modActionsList.Remove(selID);
            }
            dataPM.modActionsList.Add(selID, new ModActionData(listToAdd.OrderBy(dt => dt.Identifier).First()));
            dataPM.modActionsList[selID].Description = dataPM.modActionsList[selID].ActionActual;//add the new action
            dataPM.Actions.Find(ba => ba.name == "Action" + selID.ToString()).active = true; //action should now show
            UpdateCheck();
            // dataPM.Actions.Find(ba => ba.name == "Action" + selID.ToString()).guiName = dataPM.modActionsList[selID].Description; //UpdateCheck does this
            //Debug.Log(dataPM.modActionsList[selID].ToString());


            //action is selected, return our window to showing the actions list
            selectingID = 0; //should never be check when SelectType.None is true
            selType = SelectType.None;
            MainWinScroll = savedScrollLocation;
        }

        public void Update()
        {

        }

        public void UpdateCheck()
        {
            string errLine = "1";
            try
            {
                errLine = "2";
                if (dataPM != null)
                {
                    errLine = "2a";
                    for (int i = 1; i <= 30; i++) //monitor our descriptions to see if they've changed, array is zero indexed
                    {
                        errLine = "3";
                        if (dataPM.modActionsList.ContainsKey(i)) //if action doesnt exist it is hidden, so don't bother with updating htose guiNames
                        {
                            errLine = "4";
                            if (dataPM.Actions.Where(a => a.name == "Action" + i).First().guiName != dataPM.modActionsList[i].Description) //action description has changed
                            {
                                errLine = "5";
                                dataPM.Actions.Where(a => a.name == "Action" + i).First().guiName = dataPM.modActionsList[i].Description;
                                errLine = "6";
                                if (includeSymmetryParts)
                                {
                                    SyncSymmetry(dataPM.part, dataPM.modActionsList[i], i);
                                }

                            }
                        }

                    }
                }
                if (HighLogic.LoadedSceneIsEditor)
                {
                    errLine = "7";
                    KSP.UI.Screens.EditorActionGroups.Instance.SelectGroup(); //used to refresh stock editor, find new ksp 1.1 method
                    try
                    {
                        StaticMethodsUnity.RefreshAGXEditor(); 
                    }
                    catch
                    {
                        //silently fail if AGX is not ready
                    }
                }
                else if (HighLogic.LoadedSceneIsFlight)
                {
                    errLine = "8";
                    StaticMethodsUnity.RefreshAGXFlight();
                }
            }
            catch (Exception e)
            {
                Debug.Log("ModActions GUI UpdateCheck Fail " + errLine + " " + e);
            }
        }

        public void SyncSymmetry(Part p, ModActionData modData, int actID)
        {
            //Debug.Log("MA Sync sym called " + p.symmetryCounterparts.Count + "|" + p.partName + "|" +actID + "|" + modData.ToString());
            foreach (Part symP in p.symmetryCounterparts)
            {
                //Debug.Log("MA1");
                ModuleModActions symPM = symP.Modules.OfType<ModuleModActions>().First();
                if (modData == null && symPM.modActionsList.ContainsKey(actID))
                {
                    //Debug.Log("MA2");
                    symPM.modActionsList.Remove(actID);
                    symPM.Actions.Find(ba => ba.name == "Action" + actID.ToString()).active = false;
                    //symPM.Actions.Where()
                }
                else if (modData != null)
                {
                    //Debug.Log("MA3");
                    if (symPM.modActionsList.ContainsKey(actID))
                    {
                        //Debug.Log("MA5");
                        symPM.modActionsList.Remove(actID);
                    } 
                    symPM.modActionsList.Add(actID, new ModActionData(modData));
                    symPM.Actions.Where(a => a.name == "Action" + actID).First().guiName = new string(modData.Description.ToCharArray());
                    symPM.Actions.Find(ba => ba.name == "Action" + actID.ToString()).active = true;
                }
                // symPM.modActionsList[i] = new ModActionData(dataPM.modActionsList[i]);
                //symPM.Actions.Where(a => a.name == "Action" + i).First().guiName = new string(s)

            }
        }

    }

    class StaticMethodsUnity : PartModule
    {
        public static void RefreshAGXEditor() //move to using new GameEvents system from KSP 1.2
        {
            EventData<string> AGExtRefreshActions = GameEvents.FindEvent<EventData<string>>("AGExtRefreshActionsEvent");
            if(AGExtRefreshActions!= null)
            {
                AGExtRefreshActions.Fire(null);
            }

        }//mess of reflection to update AGX if present.
        //{
        //    private EventData<string> AGExtEventRefreshActions;
    
        //AGExtEventRefreshActions = GameEvents.FindEvent<EventData<string>>("AGExtEventRefreshActionsEvent");
        //AGExtEventRefreshActions.Fire();

        //don't forget to remove them in OnDestroy:
        //if (onKerbalFrozenEvent != null) onKerbalFrozenEvent.Remove(onKerbalFrozen);
        //Type linkType = Type.GetType("ActionGroupsExtended.AGXEditor, AGExt");
        //if (linkType != null)
        //{
        //    object[] linkObjs = FindObjectsOfType(linkType);
        //    linkType.InvokeMember("RefreshPartActions", BindingFlags.InvokeMethod | BindingFlags.Public, null, linkObjs.First(), null);
        //}

        //}
    
        public static void RefreshAGXFlight() //mess of reflection to update AGX if present, use new gameEvents system from KSP 1.2 now.
        {
            
                EventData<string> AGExtRefreshActions = GameEvents.FindEvent<EventData<string>>("AGExtRefreshActionsEvent");
                if (AGExtRefreshActions != null)
                {
                    AGExtRefreshActions.Fire(null);
                }

            
            //Type linkType = Type.GetType("ActionGroupsExtended.AGXFlight, AGExt");
            //if (linkType != null)
            //{
            //    object[] linkObjs = FindObjectsOfType(linkType);
            //    Debug.Log("Link " + linkObjs.First().GetType());
            //    linkType.InvokeMember("RefreshPartActions", BindingFlags.InvokeMethod | BindingFlags.Public, null, linkObjs.First(), null);
            //}
        }
    }
    enum SelectType
    {
        None,
        Name,
        ActionGroup,
        ActionType,
        ActionActual,
        NoPart
    }
}
