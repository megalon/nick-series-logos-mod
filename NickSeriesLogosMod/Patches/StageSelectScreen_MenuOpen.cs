﻿using System;
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
        static void Postfix(StageSelectScreen __instance)
        {
            var stages = __instance.gameObject.transform.Find("Canvas/MainContainer/LargePreviewLocal/StageMask/RenderVisualizer/Stages");

            if (stages != null)
            {
                GameObject showLogoObj = new GameObject("Show Logo");
                RectTransform rectTransform = showLogoObj.AddComponent<RectTransform>();

                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.sizeDelta = Vector2.zero;
                rectTransform.localScale = new Vector3(0.125f, 0.125f, 0.125f);
                rectTransform.anchoredPosition = new Vector2(568, 791);

                showLogoObj.AddComponent<CanvasRenderer>();
                showLogoObj.AddComponent<Image>();

                // Attach image to each child object
                for (int i = 0; i < stages.childCount; ++i)
                {
                    Transform stage = stages.GetChild(i);
                    RenderImage renderImage = stage.GetComponent<RenderImage>();
                    var showId = renderImage.StageMetaData.showId;

                    // Skip empty string
                    if (showId == null || showId.Equals(string.Empty)) continue;

                    // Skip if we don't have an image for this id
                    if (!Plugin.logoSpritesDict.ContainsKey(showId)) continue;

                    try
                    {
                        GameObject obj = GameObject.Instantiate(showLogoObj, stage);
                        Image img = obj.GetComponent<Image>();
                        img.sprite = Plugin.logoSpritesDict[showId];

                        // 16 is menuUI
                        obj.layer = 16;

                        Plugin.LogDebug($"Loaded image for show \"{showId}\"");
                    } catch
                    {
                        Debug.LogWarning($"No image found for {showId}. Skipping...");
                    }
                }
            }
            else
            {
                Plugin.LogError("Could not find \"Canvas/MainContainer/LargePreviewLocal/StageMask/RenderVisualizer/Stages\"");
            }
        }
    }
}