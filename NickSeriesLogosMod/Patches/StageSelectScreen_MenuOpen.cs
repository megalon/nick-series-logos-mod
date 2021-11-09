using System;
using System.Collections.Generic;
using System.Text;
using Nick;
using HarmonyLib;
using UnityEngine;
using SMU;
using UnityEngine.UI;

namespace NickSeriesLogosMod.Patches
{
    [HarmonyPatch(typeof(StageSelectScreen), "MenuOpen")]
    class StageSelectScreen_MenuOpen
    {
        private static GameObject showLogoObj;
        static void Postfix(StageSelectScreen __instance)
        {
            string stagesHierarchyPathLocal  = "Canvas/MainContainer/LargePreviewLocal/StageMask/RenderVisualizer/Stages";
            string stagesHierarchyPathOnline = "Canvas/MainContainer/LargePreviewOnline/StageMask/RenderVisualizer/Stages";

            float scale = 0.125f;
            float x = 568;
            float ylocal = 791;
            float yonline = 550;

            var stagesLocal  = __instance.gameObject.transform.Find(stagesHierarchyPathLocal);
            var stagesOnline = __instance.gameObject.transform.Find(stagesHierarchyPathOnline);

            if (stagesLocal == null)
            {
                Plugin.LogError($"Could not find \"{stagesHierarchyPathLocal}\"");
                return;
            }

            showLogoObj = new GameObject("Show Logo");
            RectTransform rectTransform = showLogoObj.AddComponent<RectTransform>();

            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.localScale = new Vector3(scale, scale, scale);
            rectTransform.anchoredPosition = new Vector2(x, ylocal);

            showLogoObj.AddComponent<CanvasRenderer>();
            showLogoObj.AddComponent<Image>();

            PopulateStages(stagesLocal);

            rectTransform.anchoredPosition = new Vector2(x, yonline);
            PopulateStages(stagesOnline);
        }

        private static void PopulateStages(Transform stages)
        {
            // Attach image to each child object
            for (int i = 0; i < stages.childCount; ++i)
            {
                Transform stage = stages.GetChild(i);
                RenderImage renderImage = stage.GetComponent<RenderImage>();
                var showId = renderImage.StageMetaData.showId;

                // Skip empty string
                if (showId == null || showId.Equals(string.Empty)) continue;

                // Skip if we don't have an image for this id
                if (!Plugin.logoSpritesDict.ContainsKey(showId))
                {
                    Plugin.LogWarning($"No image found for show \"{showId}\". Skipping...");
                    continue;
                }

                try
                {
                    GameObject obj = GameObject.Instantiate(showLogoObj, stage);
                    Image img = obj.GetComponent<Image>();
                    img.sprite = Plugin.logoSpritesDict[showId];

                    // 16 is menuUI
                    obj.layer = 16;

                    Plugin.LogDebug($"Loaded image for show \"{showId}\"");
                }
                catch
                {
                    Plugin.LogError($"Could not load image for {showId}!");
                }
            }
        }
    }
}
