using BepInEx;
using HarmonyLib;
using Photon.Pun;
using System;
using System.Collections;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnneededInfo.ColorLib;

namespace UnneededInfo
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        void Awake()
        {
            var harmony = new Harmony(PluginInfo.Name);
            harmony.PatchAll();
            Logger.LogInfo($"{PluginInfo.Name} v{PluginInfo.Version} loaded.");

            SceneManager.sceneLoaded += OnSceneLoaded;
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
            tmpText.fontSize = 80f;
            tmpText.alignment = TextAlignmentOptions.Center;
            tmpText.enableVertexGradient = true;
            tmpText2.text = $"Unneeded Info V{PluginInfo.Version}";
            tmpText2.enableVertexGradient = true;
            tmpText2.color = Color.red;
        }
        void Update()
        {
            GameObject cocBoard = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/COCBodyText_TitleData");
            var tmpText = cocBoard.GetComponent<TextMeshPro>();

            if (tmpText == null) return;

            tmpText.text =
                $"FPS: {Mathf.Ceil(1f / Time.unscaledDeltaTime)}\n" +
                $"Ping: {PhotonNetwork.GetPing()}\n" +
                $"Time: {DateTime.Now:HH:mm:ss}\n" +
                $"Unneeded Info v{PluginInfo.Version}";

            tmpText.ForceMeshUpdate(true);
        }
    }
}
