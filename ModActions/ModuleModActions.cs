using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModActions
{

    //class VesselModTest : VesselModule
    //{
    //    //public void Update()
    //    //{
    //    //    Debug.Log("test");
    //    //}

    //    public void OnSave(ConfigNode node)
    //    {
    //        Debug.Log("saving");
            
    //        node.AddValue("test val", "testingthisout");
    //    }
    //}
    class ModuleModActions : PartModule
    {
        
        public Dictionary<int,ModActionData> modActionsList; //list of modactions assigned on this part

        [KSPField(guiActive = false, guiActiveEditor = false, isPersistant = true)]
        public Part.AutoStrutMode partAutoStrutMode = Part.AutoStrutMode.Off;

        public override void OnStart(StartState state)
        {
            if (modActionsList == null)
            {
                modActionsList = new Dictionary<int, ModActionData>(); //initialise our list, if already initialise this serves to clear it
            }
            //foreach (BaseAction ba in this.Actions) //hide all actions so they don't show
            //{
            //    ba.active = false;
            //}
            for(int i = 1;i<= 30;i++) //onstart runs after onload in editor, modactionslist could have values in it
            {
                if(modActionsList.ContainsKey(i))
                {
                    this.Actions.Where(a => a.name == "Action" + i).First().active = true;
                }
                else
                {
                    this.Actions.Where(a => a.name == "Action" + i).First().active = false;
                }
            }

            if(HighLogic.LoadedSceneIsFlight)
            {
                StartCoroutine("AutoStrutType");
            }
        }

        IEnumerator AutoStrutType()
        {
            while (!this.vessel.HoldPhysics)
            {
                yield return null;
            }
            if (this.part.autoStrutMode != Part.AutoStrutMode.Off)
            {
                partAutoStrutMode = part.autoStrutMode;
            }
        }

        public override void OnLoad(ConfigNode node)
        {
            modActionsList = new Dictionary<int, ModActionData>(); //initialise our list, if already initialise this serves to clear it
            ;
            foreach(BaseAction ba in this.Actions) //hide all actions so they don't show
            {
                ba.active = false;
            }
            foreach (ConfigNode act in node.GetNodes("ModAction")) //load our data
            {
            ModActionData baseActionMod = new ModActionData(StaticMethods.AllActionsList.Find(item => item.Identifier == int.Parse(act.GetValue("Ident")))); //clone our object to pull the data that does not change
            
                baseActionMod.Description = act.GetValue("Desc");
            baseActionMod.ActionValue = act.GetValue("ActionValue");
            modActionsList.Add(int.Parse(act.GetValue("ActionPosition")), baseActionMod);
            
            }
            foreach(KeyValuePair<int,ModActionData> md in modActionsList) //for each loaded action, change the action group gui name and set it active so it shows
            {
                this.Actions["Action" + md.Key.ToString()].guiName = md.Value.Description;
                this.Actions["Action" + md.Key.ToString()].active = true; 
            }
        }

        public override void OnSave(ConfigNode node)
        {
            if (modActionsList != null)
            {
                foreach (KeyValuePair<int, ModActionData> act in modActionsList)
                {
                    ConfigNode modAction = new ConfigNode("ModAction");
                    modAction.AddValue("ActionPosition", act.Key.ToString());
                    modAction.AddValue("Ident", act.Value.Identifier.ToString()); //other fields don't change, fill in with identifier
                    modAction.AddValue("Desc", act.Value.Description);
                    modAction.AddValue("ActionValue", act.Value.ActionValue);
                    node.AddNode(modAction);
                }
            }
        }

        #region Actions

        [KSPAction("Action1")]
        public void Action1(KSPActionParam param)
        {
            Execute.ExectueAction(modActionsList[1], this, param, modActionsList[1].ActionValue);
        }

        [KSPAction("Action2")]
        public void Action2(KSPActionParam param)
        {
            Execute.ExectueAction(modActionsList[2], this, param, modActionsList[2].ActionValue);
        }

        [KSPAction("Action3")]
        public void Action3(KSPActionParam param)
        {
            Execute.ExectueAction(modActionsList[3], this, param, modActionsList[3].ActionValue);
        }

        [KSPAction("Action4")]
        public void Action4(KSPActionParam param)
        {
            Execute.ExectueAction(modActionsList[4], this, param, modActionsList[4].ActionValue);
        }

        [KSPAction("Action5")]
        public void Action5(KSPActionParam param)
        {
            Execute.ExectueAction(modActionsList[5], this, param, modActionsList[5].ActionValue);
        }

        [KSPAction("Action6")]
        public void Action6(KSPActionParam param)
        {
            Execute.ExectueAction(modActionsList[6], this, param, modActionsList[6].ActionValue);
        }

        [KSPAction("Action7")]
        public void Action7(KSPActionParam param)
        {
            Execute.ExectueAction(modActionsList[7], this, param, modActionsList[7].ActionValue);
        }

        [KSPAction("Action8")]
        public void Action8(KSPActionParam param)
        {
            Execute.ExectueAction(modActionsList[8], this, param, modActionsList[8].ActionValue);
        }

        [KSPAction("Action9")]
        public void Action9(KSPActionParam param)
        {
            Execute.ExectueAction(modActionsList[9], this, param, modActionsList[9].ActionValue);
        }

        [KSPAction("Action10")]
        public void Action10(KSPActionParam param)
        {
            Execute.ExectueAction(modActionsList[10], this, param, modActionsList[10].ActionValue);
        }

        [KSPAction("Action11")]
        public void Action11(KSPActionParam param)
        {
            Execute.ExectueAction(modActionsList[11], this, param, modActionsList[11].ActionValue);
        }

        [KSPAction("Action12")]
        public void Action12(KSPActionParam param)
        {
            Execute.ExectueAction(modActionsList[12], this, param, modActionsList[12].ActionValue);
        }

        [KSPAction("Action13")]
        public void Action13(KSPActionParam param)
        {
            Execute.ExectueAction(modActionsList[13], this, param, modActionsList[13].ActionValue);
        }

        [KSPAction("Action14")]
        public void Action14(KSPActionParam param)
        {
            Execute.ExectueAction(modActionsList[14], this, param, modActionsList[14].ActionValue);
        }

        [KSPAction("Action15")]
        public void Action15(KSPActionParam param)
        {
            Execute.ExectueAction(modActionsList[15], this, param, modActionsList[15].ActionValue);
        }

        [KSPAction("Action16")]
        public void Action16(KSPActionParam param)
        {
            Execute.ExectueAction(modActionsList[16], this, param, modActionsList[16].ActionValue);
        }

        [KSPAction("Action17")]
        public void Action17(KSPActionParam param)
        {
            Execute.ExectueAction(modActionsList[17], this, param, modActionsList[17].ActionValue);
        }

        [KSPAction("Action18")]
        public void Action18(KSPActionParam param)
        {
            Execute.ExectueAction(modActionsList[18], this, param, modActionsList[18].ActionValue);
        }

        [KSPAction("Action19")]
        public void Action19(KSPActionParam param)
        {
            Execute.ExectueAction(modActionsList[19], this, param, modActionsList[19].ActionValue);
        }

        [KSPAction("Action20")]
        public void Action20(KSPActionParam param)
        {
            Execute.ExectueAction(modActionsList[20], this, param, modActionsList[20].ActionValue);
        }

        [KSPAction("Action21")]
        public void Action21(KSPActionParam param)
        {
            Execute.ExectueAction(modActionsList[21], this, param, modActionsList[21].ActionValue);
        }

        [KSPAction("Action22")]
        public void Action22(KSPActionParam param)
        {
            Execute.ExectueAction(modActionsList[22], this, param, modActionsList[22].ActionValue);
        }

        [KSPAction("Action23")]
        public void Action23(KSPActionParam param)
        {
            Execute.ExectueAction(modActionsList[23], this, param, modActionsList[23].ActionValue);
        }

        [KSPAction("Action24")]
        public void Action24(KSPActionParam param)
        {
            Execute.ExectueAction(modActionsList[24], this, param, modActionsList[24].ActionValue);
        }

        [KSPAction("Action25")]
        public void Action25(KSPActionParam param)
        {
            Execute.ExectueAction(modActionsList[25], this, param, modActionsList[25].ActionValue);
        }

        [KSPAction("Action26")]
        public void Action26(KSPActionParam param)
        {
            Execute.ExectueAction(modActionsList[26], this, param, modActionsList[26].ActionValue);
        }

        [KSPAction("Action27")]
        public void Action27(KSPActionParam param)
        {
            Execute.ExectueAction(modActionsList[27], this, param, modActionsList[27].ActionValue);
        }

        [KSPAction("Action28")]
        public void Action28(KSPActionParam param)
        {
            Execute.ExectueAction(modActionsList[28], this, param, modActionsList[28].ActionValue);
        }

        [KSPAction("Action29")]
        public void Action29(KSPActionParam param)
        {
            Execute.ExectueAction(modActionsList[29], this, param, modActionsList[29].ActionValue);
        }

        [KSPAction("Action30")]
        public void Action30(KSPActionParam param)
        {
            Execute.ExectueAction(modActionsList[30], this, param, modActionsList[30].ActionValue);
        }
        #endregion
    }
}
