using BepInEx;
using HarmonyLib;
using MTM101BaldAPI.Registers;
using MTM101BaldAPI;

namespace NoTimeLeft
{
    [BepInPlugin("detectivebaldi.pluspacks.notimeleft", "No Time Left Pack", "1.0.0.0")]
    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi")]
    public class BasePlugin : BaseUnityPlugin
    {
        public void Awake()
        {
            Harmony harmony = new Harmony("detectivebaldi.pluspacks.notimeleft");

            harmony.PatchAllConditionals();

            GeneratorManagement.Register(this, GenerationModType.Addend, generateCallback);
        }

        public void generateCallback(string lName, int lNumber, SceneObject lSceneObject)
        {
            lSceneObject.levelObject.timeLimit = 0.0f;

            lSceneObject.MarkAsNeverUnload();
        }
    }
}