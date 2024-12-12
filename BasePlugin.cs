using BepInEx;
using HarmonyLib;
using MTM101BaldAPI.Registers;
using MTM101BaldAPI;
using BepInEx.Configuration;

namespace NoTimeLeft
{
    [BepInPlugin("detectivebaldi.pluspacks.notimeleft", "No Time Left Pack", "1.2.0.0")]
    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi")]
    public class BasePlugin : BaseUnityPlugin
    {
#pragma warning disable CS8618

        public static BasePlugin Current;

        public ConfigEntry<float> ManualTimeLimit;

#pragma warning restore CS8618

        public void Awake()
        {
            Current = this;

            Harmony Harmony = new Harmony("detectivebaldi.pluspacks.notimeleft");

            Harmony.PatchAllConditionals();

            ManualTimeLimit = Config.Bind<float>("General", "Manual Time Limit", 0.0f, "The amount of time allocated to beat the level before the Lights Out event begins, in seconds. If set to -1.0, the default times are used.");

            GeneratorManagement.Register(this, GenerationModType.Addend, GenerateCallback);
        }

        public void GenerateCallback(string LName, int LNumber, SceneObject LSceneObject)
        {
            if (ManualTimeLimit.Value != -1.0f && LName.StartsWith("F"))
            {
                LSceneObject.levelObject.timeLimit = ManualTimeLimit.Value;

                LSceneObject.MarkAsNeverUnload();
            }
        }
    }
}