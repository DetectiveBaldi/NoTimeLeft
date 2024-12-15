using BepInEx;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.OptionsAPI;
using MTM101BaldAPI.Registers;
using System.Collections.Generic;
using UnityEngine;

namespace NoTimeLeft
{
    [BepInPlugin("detectivebaldi.pluspacks.notimeleft", "No Time Left Pack", "1.2.0.1")]
    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi")]
    public class BasePlugin : BaseUnityPlugin
    {
#pragma warning disable CS8618

        public static BasePlugin Current;

        public int CustomTimeLimit;

        public bool UseCustomTimeLimit;

        public Dictionary<SceneObject, float> DefaultTimeLimits;

#pragma warning restore CS8618

        public void Awake()
        {
            Current = this;

            Harmony Harmony = new Harmony("detectivebaldi.pluspacks.notimeleft");

            Harmony.PatchAllConditionals();

            if (PlayerPrefs.GetInt("NoTimeLeft$Init", 0) == 0)
            {
                PlayerPrefs.SetInt("NoTimeLeft$Init", 1);

                PlayerPrefs.SetInt("CustomTimeLimit", 0);

                PlayerPrefs.SetInt("UseCustomTimeLimit", 1);

                PlayerPrefs.Save();
            }

            CustomTimeLimit = PlayerPrefs.GetInt("CustomTimeLimit");

            UseCustomTimeLimit = PlayerPrefs.GetInt("UseCustomTimeLimit") == 1.0f;

            DefaultTimeLimits = new Dictionary<SceneObject, float>();

            CustomOptionsCore.OnMenuInitialize += (OptionsMenu optionsMenu, CustomOptionsHandler customOptionsHandler) => customOptionsHandler.AddCategory<NoTimeLeftOptions>("NoTimeLeft\nOptions");

            GeneratorManagement.Register(this, GenerationModType.Finalizer, GenerateCallback);
        }

        public void GenerateCallback(string LName, int LNumber, SceneObject LSceneObject)
        {
            if (LName.StartsWith("F"))
            {
                if (!DefaultTimeLimits.ContainsKey(LSceneObject))
                    DefaultTimeLimits[LSceneObject] = LSceneObject.levelObject.timeLimit;

                LSceneObject.levelObject.timeLimit = UseCustomTimeLimit ? CustomTimeLimit * 60.0f : DefaultTimeLimits[LSceneObject];

                LSceneObject.MarkAsNeverUnload();
            }
        }
    }
}