using BepInEx;
using HarmonyLib;
using MTM101BaldAPI.OptionsAPI;
using MTM101BaldAPI.Registers;
using MTM101BaldAPI;
using System.Collections.Generic;
using UnityEngine;

namespace NoTimeLeft
{
    [BepInPlugin("detectivebaldi.pluspacks.notimeleft", "No Time Left Pack", "1.2.0.2")]
    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi")]
    public class BasePlugin : BaseUnityPlugin
    {
        public static BasePlugin current;

        public int customTimeLimit;

        public bool useCustomTimeLimit;

        public Dictionary<SceneObject, float> defaultTimeLimits;

        public void Awake()
        {
            current = this;

            Harmony harmony = new("detectivebaldi.pluspacks.notimeleft");

            harmony.PatchAllConditionals();

            if (PlayerPrefs.GetInt("noTimeLeft$Init", 0) == 0)
            {
                PlayerPrefs.SetInt("noTimeLeft$Init", 1);

                PlayerPrefs.SetInt("customTimeLimit", 0);

                PlayerPrefs.SetInt("useCustomTimeLimit", 1);

                PlayerPrefs.Save();
            }

            customTimeLimit = PlayerPrefs.GetInt("customTimeLimit");

            useCustomTimeLimit = PlayerPrefs.GetInt("useCustomTimeLimit") == 1.0f;

            defaultTimeLimits = [];

            CustomOptionsCore.OnMenuInitialize += (OptionsMenu optionsMenu, CustomOptionsHandler customOptionsHandler) => customOptionsHandler.AddCategory<NoTimeLeftOptions>("NoTimeLeft\nOptions");

            GeneratorManagement.Register(this, GenerationModType.Finalizer, GenerateCallback);
        }

        public void GenerateCallback(string name, int index, SceneObject sceneObject)
        {
            CustomLevelObject[] levelObjects = sceneObject.GetCustomLevelObjects();

            sceneObject.MarkAsNeverUnload();

            for (int i = 0; i < levelObjects.Length; i++)
            {
                if (!name.StartsWith("F"))
                    continue;

                CustomLevelObject levelObject = levelObjects[i];

                if (!defaultTimeLimits.ContainsKey(sceneObject))
                    defaultTimeLimits[sceneObject] = levelObject.timeLimit;

                levelObject.timeLimit = useCustomTimeLimit ? customTimeLimit * 60.0f : defaultTimeLimits[sceneObject];
            }
        }
    }
}
