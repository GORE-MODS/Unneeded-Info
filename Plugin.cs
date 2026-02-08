using BepInEx;
using HarmonyLib;
using Photon.Pun;
using System;
using System.Collections;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using static UnneededInfo.ColorLib;

namespace UnneededInfo
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        private string latestVersion = "";
        private bool updateAvailable = false;
        private bool versionChecked = false;
        void Awake()
        {
            var harmony = new Harmony(PluginInfo.Name);
            harmony.PatchAll();
            Logger.LogInfo($"{PluginInfo.Name} v{PluginInfo.Version} loaded.");

            SceneManager.sceneLoaded += OnSceneLoaded;

            StartCoroutine(CheckForUpdates());
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            StartCoroutine(ChangeCOC());
        }
        private IEnumerator ChangeCOC()
        {
            yield return new WaitForSeconds(1.5f);

            GameObject cocBoard = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/COCBodyText_TitleData");
            GameObject headText = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/CodeOfConductHeadingText");

            if (cocBoard == null)
            {
                Debug.LogWarning("COCBodyText_TitleData not found at the expected path. Aborting to avoid NullReferenceException.");
                yield break;
            }

            if (headText == null)
            {
                Debug.LogWarning("CodeOfConductHeadingText not found at the expected path. Aborting to avoid NullReferenceException.");
                yield break;
            }

            var playfabComponent = cocBoard.GetComponent<PlayFabTitleDataTextDisplay>();
            if (playfabComponent != null)
            {
                playfabComponent.enabled = false;
            }
            else
            {
                Debug.LogWarning("PlayFabTitleDataTextDisplay component not found on COC board. Skipping disabling it.");
            }

            var playfabComponent2 = headText.GetComponent<PlayFabTitleDataTextDisplay>();
            if (playfabComponent2 != null)
            {
                playfabComponent2.enabled = false;
            }
            else
            {
                Debug.LogWarning("PlayFabTitleDataTextDisplay component not found on heading text. Skipping disabling it.");
            }

            var tmpText = cocBoard.GetComponent<TextMeshPro>();
            var tmpText2 = headText.GetComponent<TextMeshPro>();

            if (tmpText == null)
            {
                yield break;
            }

            if (tmpText2 == null)
            {
                yield break;
            }

            tmpText.text = "Loading Unneeded Info Created by GORE";
            tmpText.fontSize = 74f;
            tmpText.alignment = TextAlignmentOptions.Center;
            tmpText.enableVertexGradient = true;
            tmpText2.text = $"Unneeded Info V{PluginInfo.Version}";
            tmpText2.color = Color.red;
        }
        void Update()
        {
            GameObject cocBoard = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/COCBodyText_TitleData");
            var tmpText = cocBoard.GetComponent<TextMeshPro>();

            if (tmpText == null) return;

            string updateLine = "";

            if (versionChecked)
            {
                updateLine = updateAvailable
                    ? $"Update Available: v{latestVersion}"
                    : "Mod is up to date";
            }
            else
            {
                updateLine = "Checking for updates...";
            }

            tmpText.text =
                $"FPS: {Mathf.Ceil(1f / Time.unscaledDeltaTime)}\n" +
                $"Ping: {PhotonNetwork.GetPing()}\n" +
                $"IsMaster: {PhotonNetwork.IsMasterClient}\n" +
                $"Time: {DateTime.Now:HH:mm:ss}\n" +
                $"Server Region: {PhotonNetwork.CloudRegion}\n" +
                $"Your Player ID: {PhotonNetwork.LocalPlayer.UserId}\n" +
                updateLine;

            tmpText.ForceMeshUpdate(true);
        }
        private IEnumerator CheckForUpdates()
        {
            using (UnityWebRequest www = UnityWebRequest.Get(PluginInfo.VersionUrl))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Logger.LogWarning("Version check failed: " + www.error);
                    yield break;
                }

                latestVersion = www.downloadHandler.text
                    .Replace("\uFEFF", "") // BOM BING POW
                    .Trim();

                Logger.LogInfo($"Installed version: '{PluginInfo.Version}'");
                Logger.LogInfo($"Latest version from GitHub: '{latestVersion}'");

                updateAvailable = latestVersion != PluginInfo.Version;
                versionChecked = true;
            }
        }
    }
}
