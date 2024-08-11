using BananaOS;
using BepInEx;
using System;
using UnityEngine;
using Utilla;

namespace DashMonkebananaos
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        void OnEnable()
        {
            HarmonyPatches.ApplyHarmonyPatches();
        }

        void OnDisable()
        {

            HarmonyPatches.RemoveHarmonyPatches();
        }


    }
}
