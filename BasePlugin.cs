using BepInEx;
using HarmonyLib;
using MTM101BaldAPI.Registers;
using MTM101BaldAPI;
using BepInEx.Configuration;
using MTM101BaldAPI.SaveSystem;
using System.IO;
using System.Linq;

namespace NoTimeLeft
{
    [BepInPlugin("detectivebaldi.pluspacks.notimeleft", "No Time Left Pack", "1.1.0.0")]
    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi")]
    public class BasePlugin : BaseUnityPlugin
    {
#pragma warning disable CS8618

        public static BasePlugin Current;

        public ConfigEntry<bool> EraseTimeLimit;

#pragma warning restore CS8618

        public void Awake()
        {
            Current = this;

            Harmony Harmony = new Harmony("detectivebaldi.pluspacks.notimeleft");

            Harmony.PatchAllConditionals();

            GeneratorManagement.Register(this, GenerationModType.Addend, GenerateCallback);

            EraseTimeLimit = Config.Bind<bool>("General", "Erase Time Limit", true, "If enabled, the school timer will be set to 0:00 once Baldi finishes counting.");

            ModdedSaveGame.AddSaveHandler(new NoTimeLimitSave());
        }

        public void GenerateCallback(string LName, int LNumber, SceneObject LSceneObject)
        {
            if (EraseTimeLimit.Value)
            {
                LSceneObject.levelObject.timeLimit = 0.0f;

                LSceneObject.MarkAsNeverUnload();
            }
        }
    }

    public class NoTimeLimitSave : ModdedSaveGameIOBinary
    {
        public override PluginInfo pluginInfo => BasePlugin.Current.Info;

        public override void Load(BinaryReader Reader)
        {
            Reader.ReadByte();
        }

        public override void Reset()
        {

        }

        public override void Save(BinaryWriter Writer)
        {
            Writer.Write((byte)0);
        }

        public override string[] GenerateTags()
        {
            if (BasePlugin.Current.EraseTimeLimit.Value)
                return new string[1] {"EraseTimeLimit"};

            return new string[0];
        }

        public override string DisplayTags(string[] tags)
        {
            if (tags.Contains("EraseTimeLimit"))
                return "Erase Time Limit";

            return "Keep Time Limit";
        }
    }
}