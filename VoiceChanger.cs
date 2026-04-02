using MelonLoader;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using GHPC.Effects.Voices;
using GHPC.Mission;

[assembly: MelonInfo(typeof(VoiceChanger.VoiceChangerMod), "VoiceChanger", "1.0.0", "swarog")]
[assembly: MelonGame("", "Gunner, HEAT, PC!")]

namespace VoiceChanger
{
    public class VoiceChangerMod : MelonMod
    {
        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("VoiceChanger initialized.");
            Config.Init();
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            VoiceHandlerPatch.CacheProtocols();
        }
    }

    public static class Config
    {
        static MelonPreferences_Category _cat;
        public static Dictionary<string, MelonPreferences_Entry<string>> VehicleMap = new Dictionary<string, MelonPreferences_Entry<string>>();

        static MelonPreferences_Entry<string> Add(string key, string defaultNationality, string description)
        {
            var entry = _cat.CreateEntry(key, defaultNationality, description);
            VehicleMap[key] = entry;
            return entry;
        }

        public static void Init()
        {
            _cat = MelonPreferences.CreateCategory("GHPCVoiceChanger", "VoiceChanger - Vehicle Voice Settings");
            string R = "Russian", G = "German", A = "American";

            // Soviet vehicles
            Add("T72",               G, "T-72");
            Add("T72M",              G, "T-72M (NVA)");
            Add("T72M1",             G, "T-72M1 (NVA)");
            Add("T72GILLS",          G, "T-72 Gill (NVA)");
            Add("T72UV1",            G, "T-72 Ural V1 (NVA)");
            Add("T72UV2",            G, "T-72 Ural V2 (NVA)");
            Add("T72ULEM",           G, "T-72 Ural LEM (NVA)");
            Add("T64A",              R, "T-64A");
            Add("T64A74",            R, "T-64A (1974)");
            Add("T64A79",            R, "T-64A (1979)");
            Add("T64A81",            R, "T-64A (1981)");
            Add("T64A84",            R, "T-64A (1984)");
            Add("T64B",              R, "T-64B");
            Add("T64B81",            R, "T-64B (1981)");
            Add("T64B181",           R, "T-64B1 (1981)");
            Add("T64B1",             R, "T-64B1");
            Add("T64R",              R, "T-64R");
            Add("T62",               R, "T-62");
            Add("T55A",              G, "T-55A (NVA)");
            Add("T54A",              G, "T-54A (NVA)");
            Add("T34-85",            G, "T-34-85 (NVA)");
            Add("T80B",              R, "T-80B");
            Add("BMP1",              G, "BMP-1 (NVA)");
            Add("BMP1P",             G, "BMP-1P (NVA)");
            Add("BMP1P_SA",          R, "BMP-1P (Soviet)");
            Add("BMP2",              G, "BMP-2 (NVA)");
            Add("BMP2_SA",           R, "BMP-2 (Soviet)");
            Add("PT76B",             G, "PT-76B (NVA)");

            // West German vehicles
            Add("LEO1A1",            G, "Leopard 1A1");
            Add("LEO1A1A1",          G, "Leopard 1A1A1");
            Add("LEO1A1A2",          G, "Leopard 1A1A2");
            Add("LEO1A1A3",          G, "Leopard 1A1A3");
            Add("LEO1A1A4",          G, "Leopard 1A1A4");
            Add("LEO1A3",            G, "Leopard 1A3");
            Add("LEO1A3A1",          G, "Leopard 1A3A1");
            Add("LEO1A3A2",          G, "Leopard 1A3A2");
            Add("LEO1A3A3",          G, "Leopard 1A3A3");
            Add("LEO1A4",            G, "Leopard 1A4");
            Add("MARDERA1PLUS",      G, "Marder A1+");
            Add("MARDERA1",          G, "Marder A1-");
            Add("MARDERA1_NO_ATGM",  G, "Marder A1 (no ATGM)");
            Add("MARDER1A2",         G, "Marder 1A2");

            // US vehicles
            Add("M1",                A, "M1 Abrams");
            Add("M1IP",              A, "M1IP Abrams");
            Add("M60A1",             A, "M60A1");
            Add("M60A1AOS",          A, "M60A1 AOS");
            Add("M60A1RISEP",        A, "M60A1 RISE Passive");
            Add("M60A1RISEP77",      A, "M60A1 RISE Passive (1977)");
            Add("M60A3",             A, "M60A3");
            Add("M60A3TTS",          A, "M60A3 TTS");
            Add("M2BRADLEY",         A, "M2 Bradley");
            Add("M2BRADLEY(ALT)",    A, "M2 Bradley (Alt loadout)");

            MelonPreferences.Save();
        }
    }

    [HarmonyPatch(typeof(CrewVoiceHandler), "Initialize")]
    public static class VoiceHandlerPatch
    {
        static readonly FieldInfo _protocolField = typeof(CrewVoiceHandler)
            .GetField("_voiceProtocolLineData", BindingFlags.NonPublic | BindingFlags.Instance);

        static VoiceProtocolDataScriptable _russianProtocol;
        static VoiceProtocolDataScriptable _germanProtocol;
        static VoiceProtocolDataScriptable _americanProtocol;

        public static void CacheProtocols()
        {
            // Fast path - search already loaded assets
            foreach (var protocol in Resources.FindObjectsOfTypeAll<VoiceProtocolDataScriptable>())
            {
                if (_russianProtocol  == null && protocol.name.StartsWith("USSR")) _russianProtocol  = protocol;
                if (_germanProtocol   == null && protocol.name.StartsWith("DE"))   _germanProtocol   = protocol;
                if (_americanProtocol == null && protocol.name.StartsWith("US"))   _americanProtocol = protocol;
            }

            // Check live CrewVoiceHandlers
            foreach (var handler in Resources.FindObjectsOfTypeAll<CrewVoiceHandler>())
            {
                var p = (VoiceProtocolDataScriptable)_protocolField.GetValue(handler);
                if (p == null) continue;
                if (_russianProtocol  == null && p.name.StartsWith("USSR")) _russianProtocol  = p;
                if (_germanProtocol   == null && p.name.StartsWith("DE"))   _germanProtocol   = p;
                if (_americanProtocol == null && p.name.StartsWith("US"))   _americanProtocol = p;
            }

            // If Russian still missing, load T64A prefab via Addressables to steal its protocol
            if (_russianProtocol == null)
            {
                try
                {
                    var lookup = Resources.FindObjectsOfTypeAll<UnitPrefabLookupScriptable>().FirstOrDefault();
                    if (lookup != null)
                    {
                        var meta = lookup.AllUnits.FirstOrDefault(u => u.Name == "T64A");
                        if (meta != null)
                        {
                            var go = meta.PrefabReference.LoadAssetAsync<GameObject>().WaitForCompletion();
                            if (go != null)
                            {
                                var handler = go.GetComponentInChildren<CrewVoiceHandler>(true);
                                if (handler != null)
                                {
                                    var p = (VoiceProtocolDataScriptable)_protocolField.GetValue(handler);
                                    if (p != null && p.name.StartsWith("USSR")) _russianProtocol = p;
                                }
                                meta.PrefabReference.ReleaseAsset();
                            }
                        }
                    }
                }
                catch (System.Exception e)
                {
                    MelonLogger.Warning($"Failed to load T64A for protocol cache: {e.Message}");
                }
            }

            MelonLogger.Msg($"Cached protocols - RU:{_russianProtocol?.name ?? "none"} DE:{_germanProtocol?.name ?? "none"} US:{_americanProtocol?.name ?? "none"}");
        }

        static void Postfix(CrewVoiceHandler __instance)
        {
            var current = (VoiceProtocolDataScriptable)_protocolField.GetValue(__instance);

            // Cache from live instances
            if (current != null)
            {
                if (_russianProtocol  == null && current.name.StartsWith("USSR")) _russianProtocol  = current;
                if (_germanProtocol   == null && current.name.StartsWith("DE"))   _germanProtocol   = current;
                if (_americanProtocol == null && current.name.StartsWith("US"))   _americanProtocol = current;
            }

            string vehicleName = __instance.transform.root.name;
            string key = vehicleName.Split(' ')[0].Replace("(Clone)", "");

            if (!Config.VehicleMap.TryGetValue(key, out var entry))
            {
                MelonLogger.Warning($"Unknown vehicle key: '{key}' (full name: '{vehicleName}')");
                return;
            }

            VoiceProtocolDataScriptable target = entry.Value switch
            {
                "Russian"  => _russianProtocol,
                "German"   => _germanProtocol,
                "American" => _americanProtocol,
                _ => null
            };

            // If still null, try a fresh cache scan
            if (target == null)
            {
                CacheProtocols();
                target = entry.Value switch
                {
                    "Russian"  => _russianProtocol,
                    "German"   => _germanProtocol,
                    "American" => _americanProtocol,
                    _ => null
                };
            }

            if (target == null)
            {
                MelonLogger.Warning($"Protocol for '{entry.Value}' not cached yet, skipping {vehicleName}");
                return;
            }

            if (target == current) return;

            _protocolField.SetValue(__instance, target);
            MelonLogger.Msg($"Set {vehicleName} -> {entry.Value} ({target.name})");
        }
    }
}