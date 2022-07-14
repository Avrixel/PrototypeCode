using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using OutwardModTemplate;
using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AvrixelPrototype
{

    [BepInPlugin(GUID, NAME, VERSION)]
    public class Prototype : BaseUnityPlugin
    {
        // Choose a GUID for your project. Change "myname" and "mymod".
        public const string GUID = "avrixel.prototype";
        // Choose a NAME for your project, generally the same as your Assembly Name.
        public const string NAME = "Prototype";
        // Increment the VERSION when you release a new version of your mod.
        public const string VERSION = "1.0.0";

        // For accessing your BepInEx Logger from outside of this class (MyMod.Log)
        internal static ManualLogSource Log;

        public static int DefaultEngine = -9000;



        // Awake is called when your plugin is created. Use this to set up your mod.
        internal void Awake()
        {
            Log = this.Logger;

            SL.OnPacksLoaded += SL_OnPacksLoaded;

            // Harmony is for patching methods. If you're not patching anything, you can comment-out or delete this line.
            new Harmony(GUID).PatchAll();
        }

        private void SL_OnPacksLoaded()
        {
            Item Engine = ResourcesPrefabManager.Instance.GetItemPrefab(DefaultEngine);
            Engine.gameObject.AddComponent<ItemEngine_Charge>();
        }

        public enum PrototypeItemEngineIDS
        {
            BaseEngine = -9000
        }
    }
}
