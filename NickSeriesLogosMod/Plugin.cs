using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Reflection;
using BepInEx.Configuration;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;

namespace NickSeriesLogosMod
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static Plugin Instance;
        public static Dictionary<string, Sprite> logoSpritesDict;
        private void Awake()
        {
            Logger.LogDebug($"Plugin {PluginInfo.PLUGIN_NAME} is loaded!");

            if (Instance)
            {
                DestroyImmediate(this);
                return;
            }
            Instance = this;

            var config = this.Config;
            config.SettingChanged += OnConfigSettingChanged;

            // Harmony patches
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            logoSpritesDict = new Dictionary<string, Sprite>();

            // Load all the image resources
            StartCoroutine(LoadImages());
        }

        IEnumerator LoadImages()
        {
            var resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                var resourceName = resourceNames[i];
                var fileName = resourceName.Substring("NickSeriesLogosMod.Images.".Length);
                var id = fileName.Substring(0, fileName.IndexOf("."));
                LogInfo($"Loading resource: \"{resourceName}\" for id: \"{id}\"");
                Sprite imageSprite = SMU.Utilities.ImageHelper.LoadSpriteFromResources(resourceName);
                logoSpritesDict.Add(id, imageSprite);
                yield return null;
            }
        }

        static void OnConfigSettingChanged(object sender, EventArgs args)
        {
            LogDebug($"{PluginInfo.PLUGIN_NAME} OnConfigSettingChanged");
            Plugin.Instance?.Config?.Reload();
        }

        internal static void LogDebug(string message) => Instance.Log(message, LogLevel.Debug);
        internal static void LogInfo(string message) => Instance.Log(message, LogLevel.Info);
        internal static void LogWarning(string message) => Instance.Log(message, LogLevel.Warning);
        internal static void LogError(string message) => Instance.Log(message, LogLevel.Error);
        private void Log(string message, LogLevel logLevel) => Logger.Log(logLevel, message);
    }
}
