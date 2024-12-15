using MTM101BaldAPI.OptionsAPI;
using MTM101BaldAPI.Reflection;
using MTM101BaldAPI.UI;
using System.Linq;
using TMPro;
using UnityEngine;

namespace NoTimeLeft
{
    public class NoTimeLeftOptions : CustomOptionsCategory
    {
        public override void Build()
        {
            TextMeshProUGUI textMeshProUGUI = CreateText("textMeshProUGI", "Custom\nTime Limit", new Vector3(-100.0f, 25.0f), BaldiFonts.ComicSans24, TextAlignmentOptions.Center, new Vector2(1024, 25.0f), Color.black);

            AdjustmentBars adjustmentBars = CreateBars(() => {}, "", new Vector3(-25.0f, 25.0f), 10);

            adjustmentBars.Adjust(BasePlugin.Current.CustomTimeLimit);

            AddTooltip(adjustmentBars, "The amount of time allocated to beat the level before the Lights Out event begins,\nin minutes.");

            MenuToggle menuToggle = CreateToggle("menuToggle", "Use Custom Time Limit", BasePlugin.Current.UseCustomTimeLimit, new Vector3(100.0f, -35.0f), 1024.0f);

            AddTooltip(menuToggle, "Whether the allocated custom level time limit should be used.");

            StandardMenuButton standardMenuButton = CreateApplyButton(() => {});

            standardMenuButton.OnPress.AddListener(() =>
            {
                PlayerPrefs.SetInt("CustomTimeLimit", (int) ReflectionHelpers.ReflectionGetVariable(adjustmentBars, "val"));

                PlayerPrefs.SetInt("UseCustomTimeLimit", menuToggle.Value ? 1 : 0);

                PlayerPrefs.Save();

                BasePlugin.Current.CustomTimeLimit = PlayerPrefs.GetInt("CustomTimeLimit");

                BasePlugin.Current.UseCustomTimeLimit = PlayerPrefs.GetInt("UseCustomTimeLimit") == 1.0f;

                SceneObject[] sceneObjects = Resources.FindObjectsOfTypeAll<SceneObject>().Where(x => x.levelObject != null).ToArray();

                for (int i = 0; i < sceneObjects.Length; i++)
                {
                    SceneObject sceneObject = sceneObjects[i];

                    BasePlugin.Current.GenerateCallback(sceneObject.levelTitle, sceneObject.levelNo, sceneObject);
                }
            });

            AddTooltip(standardMenuButton, "Apply new options.");
        }
    }
}
