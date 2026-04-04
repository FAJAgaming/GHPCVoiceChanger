using MelonLoader;
using MelonLoader.Utils;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AddressableAssets;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using GHPC.Effects.Voices;
using GHPC.Mission;
using GHPC.Player;
using GHPC.Crew;

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
            CustomVoiceManager.Init();
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            VoiceHandlerPatch.CacheProtocols();
            CustomVoiceManager.UpdateActiveVehicle();
        }
    }

    public static class Config
    {
        static MelonPreferences_Category _cat;
        public static Dictionary<string, MelonPreferences_Entry<string>> VehicleMap = new Dictionary<string, MelonPreferences_Entry<string>>();
        public static Dictionary<string, MelonPreferences_Entry<bool>> MixedVoicelinesMap = new Dictionary<string, MelonPreferences_Entry<bool>>();

        static void Add(string key, string defaultNationality, string description)
        {
            var entry = _cat.CreateEntry(key, defaultNationality, description);
            VehicleMap[key] = entry;
            var mixed = _cat.CreateEntry(key + "_MixedVoicelines", true, description + " Mixed Voicelines");
            MixedVoicelinesMap[key] = mixed;
        }

        public static bool IsCustomVoice(string value)
        {
            string v = value.Trim('"');
            return v != "Russian" && v != "German" && v != "American";
        }

        public static string GetValue(MelonPreferences_Entry<string> entry)
        {
            return entry.Value.Trim('"');
        }

        public static bool GetMixedVoicelines(string key)
        {
            if (MixedVoicelinesMap.TryGetValue(key, out var entry))
                return entry.Value;
            return true;
        }

        public static void Init()
        {
            _cat = MelonPreferences.CreateCategory("GHPCVoiceChanger", "VoiceChanger - Vehicle Voice Settings");
            string R = "Russian", G = "German", A = "American";

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

    public static class CustomVoiceManager
    {
        public static string VoicePacksFolder;
        public static string ActiveCustomFolder;
        public static bool ActiveMixedVoicelines = true;
        public static string ActiveVehicleKey;
        public static Sound PendingSound;

        static Dictionary<string, Dictionary<string, Sound>> _loadedSounds = new Dictionary<string, Dictionary<string, Sound>>();

        // Cache: packFolder -> clipKey -> list of matching file paths
        static Dictionary<string, Dictionary<string, List<string>>> _clipKeyToFiles = new Dictionary<string, Dictionary<string, List<string>>>();

        // Cache: packFolder -> all files in pack (path -> normalizedName)
        static Dictionary<string, Dictionary<string, string>> _packFiles = new Dictionary<string, Dictionary<string, string>>();

        static readonly string[] _states = { "combat", "panicked", "regular", "calm" };

        public static void Init()
        {
            VoicePacksFolder = Path.Combine(MelonEnvironment.ModsDirectory, "VoiceChanger");
            Directory.CreateDirectory(VoicePacksFolder);
            MelonLogger.Msg($"VoiceChanger voice packs folder: {VoicePacksFolder}");
        }

        public static void UpdateActiveVehicle()
        {
            var playerInput = PlayerInput.Instance;
            if (playerInput == null || playerInput.CurrentPlayerUnit == null)
            {
                ActiveCustomFolder = null;
                ActiveVehicleKey = null;
                ActiveMixedVoicelines = true;
                return;
            }

            string vehicleName = playerInput.CurrentPlayerUnit.gameObject.name;
            string key = vehicleName.Split(' ')[0].Replace("(Clone)", "");
            ActiveVehicleKey = key;

            if (!Config.VehicleMap.TryGetValue(key, out var entry))
            {
                ActiveCustomFolder = null;
                ActiveMixedVoicelines = true;
                return;
            }

            string value = Config.GetValue(entry);
            ActiveMixedVoicelines = Config.GetMixedVoicelines(key);

            if (!Config.IsCustomVoice(value))
            {
                ActiveCustomFolder = null;
                return;
            }

            ActiveCustomFolder = value;
            MelonLogger.Msg($"Active custom voice folder: {ActiveCustomFolder}, mixed voicelines: {ActiveMixedVoicelines}");
        }

        // Normalize a string for matching: lowercase, remove underscores and numbers at end
        static string Normalize(string s)
        {
            return s.ToLower().Replace("_", "");
        }

        // Extract state from clip key
        static string GetState(string clipKey)
        {
            string[] parts = clipKey.Split('_');
            for (int i = parts.Length - 1; i >= 0; i--)
            {
                if (int.TryParse(parts[i], out _)) continue;
                string lower = parts[i].ToLower();
                if (_states.Contains(lower)) return lower;
                break;
            }
            return null;
        }

        // Build index of all files in pack folder
        static Dictionary<string, string> GetPackFiles(string packFolder)
        {
            if (_packFiles.TryGetValue(ActiveCustomFolder, out var cached)) return cached;

            var files = new Dictionary<string, string>(); // path -> normalizedBaseName
            if (!Directory.Exists(packFolder)) return files;

            foreach (var file in Directory.GetFiles(packFolder, "*", SearchOption.AllDirectories))
            {
                string ext = Path.GetExtension(file).ToLower();
                if (ext != ".wav" && ext != ".ogg") continue;

                // Get base name without number suffix and extension
                string baseName = Path.GetFileNameWithoutExtension(file);
                // Strip trailing _N number
                var match = System.Text.RegularExpressions.Regex.Match(baseName, @"^(.+?)(_\d+)?$");
                string actionName = match.Groups[1].Value;
                files[file] = Normalize(actionName);
            }

            _packFiles[ActiveCustomFolder] = files;
            return files;
        }

        // Find all files matching a clip key
        static List<string> FindMatchingFiles(string clipKey)
        {
            string folder = Path.Combine(VoicePacksFolder, ActiveCustomFolder);
            var packFiles = GetPackFiles(folder);
            string state = GetState(clipKey);
            string normalizedClipKey = Normalize(clipKey);

            var stateMatches = new List<string>();
            var rootMatches = new List<string>();

            foreach (var kvp in packFiles)
            {
                string filePath = kvp.Key;
                string normalizedAction = kvp.Value;

                if (!normalizedClipKey.Contains(normalizedAction)) continue;

                // Check if file is in a state subfolder
                string relativePath = filePath.Substring(folder.Length).TrimStart(Path.DirectorySeparatorChar);
                string[] pathParts = relativePath.Split(Path.DirectorySeparatorChar);

                if (pathParts.Length > 1)
                {
                    // File is in a subfolder
                    string subFolder = pathParts[0].ToLower();
                    if (state != null && subFolder == state)
                        stateMatches.Add(filePath);
                    // Don't add to rootMatches - it's in a subfolder
                }
                else
                {
                    rootMatches.Add(filePath);
                }
            }

            // State-specific files take priority, fall back to root
            return stateMatches.Count > 0 ? stateMatches : rootMatches;
        }

        static bool GetSound(string filePath, out Sound sound)
        {
            if (!_loadedSounds.TryGetValue(ActiveCustomFolder, out var soundMap))
            {
                soundMap = new Dictionary<string, Sound>();
                _loadedSounds[ActiveCustomFolder] = soundMap;
            }

            if (soundMap.TryGetValue(filePath, out sound))
                return true;

            RESULT result = RuntimeManager.CoreSystem.createSound(filePath, MODE.DEFAULT | MODE._2D, out sound);
            if (result != RESULT.OK)
            {
                MelonLogger.Warning($"Failed to load custom sound {filePath}: {result}");
                return false;
            }

            soundMap[filePath] = sound;
            MelonLogger.Msg($"Loaded custom sound: {filePath}");
            return true;
        }

        public static bool HasCustomSound(string simpleKey)
        {
            if (string.IsNullOrEmpty(ActiveCustomFolder)) return false;

            string folder = Path.Combine(VoicePacksFolder, ActiveCustomFolder);
            var packFiles = GetPackFiles(folder);
            string normalizedKey = Normalize(simpleKey);

            foreach (var normalizedAction in packFiles.Values)
            {
                if (normalizedKey.Contains(normalizedAction) || normalizedAction.Contains(normalizedKey))
                    return true;
            }

            return false;
        }

        public static bool TryGetCustomSound(string clipKey, out Sound sound)
        {
            sound = default;
            if (string.IsNullOrEmpty(ActiveCustomFolder)) return false;

            // Check clip key cache
            if (!_clipKeyToFiles.TryGetValue(ActiveCustomFolder, out var keyCache))
            {
                keyCache = new Dictionary<string, List<string>>();
                _clipKeyToFiles[ActiveCustomFolder] = keyCache;
            }

            if (!keyCache.TryGetValue(clipKey, out var paths))
            {
                paths = FindMatchingFiles(clipKey);
                keyCache[clipKey] = paths;
            }

            if (paths.Count == 0) return false;

            string filePath = paths[Random.Range(0, paths.Count)];
            return GetSound(filePath, out sound);
        }

        public static void ClearCache()
        {
            foreach (var soundMap in _loadedSounds.Values)
                foreach (var snd in soundMap.Values)
                    snd.release();
            _loadedSounds.Clear();
            _clipKeyToFiles.Clear();
            _packFiles.Clear();
            PendingSound = default;
        }
    }

    [HarmonyPatch(typeof(CrewVoiceHandler), "PlayVoiceLine")]
    public static class CrewVoiceHandlerSuppressPatch
    {
        static bool Prefix(string clipKey, ref VoiceLineReceipt __result)
        {
            if (string.IsNullOrEmpty(CustomVoiceManager.ActiveCustomFolder)) return true;
            if (CustomVoiceManager.ActiveMixedVoicelines) return true;
            if (CustomVoiceManager.HasCustomSound(clipKey)) return true;

            __result = null;
            return false;
        }
    }

    [HarmonyPatch(typeof(VoicePlayer), "PlayAudioEventImmediate")]
    public static class VoicePlayerPatch
    {
        static readonly FieldInfo _sourcesField = typeof(VoicePlayer)
            .GetField("_sources", BindingFlags.NonPublic | BindingFlags.Instance);
        static readonly FieldInfo _lastActiveRoleEventsField = typeof(VoicePlayer)
            .GetField("_lastActiveRoleEvents", BindingFlags.NonPublic | BindingFlags.Instance);
        static readonly FieldInfo _eventNameField = typeof(VoicePlayer)
            .GetField("_eventName", BindingFlags.NonPublic | BindingFlags.Instance);
        static readonly FieldInfo _voiceEventCallbackField = typeof(VoicePlayer)
            .GetField("_voiceEventCallback", BindingFlags.NonPublic | BindingFlags.Instance);

        static bool Prefix(VoicePlayer __instance, CrewPosition role, string soundKey, bool incapacitationCheck, ref FmodEventHandle __result)
        {
            if (string.IsNullOrEmpty(CustomVoiceManager.ActiveCustomFolder)) return true;
            if (!CustomVoiceManager.TryGetCustomSound(soundKey, out Sound customSound)) return true;

            try
            {
                var sources = (ConcurrentDictionary<int, FmodEventHandle>)_sourcesField.GetValue(__instance);
                var lastActiveRoleEvents = (FmodEventHandle[])_lastActiveRoleEventsField.GetValue(__instance);
                string eventName = (string)_eventNameField.GetValue(__instance);
                EVENT_CALLBACK callback = (EVENT_CALLBACK)_voiceEventCallbackField.GetValue(__instance);

                FmodEventHandle handle = new FmodEventHandle(eventName, callback);
                if (!sources.TryAdd(handle.Id, handle))
                {
                    handle.Dispose();
                    __result = null;
                    return false;
                }

                lastActiveRoleEvents[(int)role] = handle;
                handle.SetUserData($"{handle.Id},{soundKey}");
                CustomVoiceManager.PendingSound = customSound;
                handle.Start();
                __result = handle;
                MelonLogger.Msg($"Playing custom sound for: {soundKey}");
                return false;
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning($"Custom sound playback failed: {e.Message}");
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(VoicePlayer), "OnVoiceEventCallback")]
    public static class VoicePlayerCallbackPatch
    {
        static bool Prefix(EVENT_CALLBACK_TYPE type, System.IntPtr instancePtr, System.IntPtr paramPtr)
        {
            if (type != EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND) return true;
            if (CustomVoiceManager.PendingSound.handle == System.IntPtr.Zero) return true;

            Sound customSound = CustomVoiceManager.PendingSound;
            CustomVoiceManager.PendingSound = default;

            PROGRAMMER_SOUND_PROPERTIES props = (PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(paramPtr, typeof(PROGRAMMER_SOUND_PROPERTIES));
            props.sound = customSound.handle;
            props.subsoundIndex = -1;
            Marshal.StructureToPtr(props, paramPtr, false);

            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerInput), "SetPlayerUnit")]
    public static class PlayerInputPatch
    {
        static void Postfix()
        {
            CustomVoiceManager.UpdateActiveVehicle();
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
            foreach (var protocol in Resources.FindObjectsOfTypeAll<VoiceProtocolDataScriptable>())
            {
                if (_russianProtocol  == null && protocol.name.StartsWith("USSR")) _russianProtocol  = protocol;
                if (_germanProtocol   == null && protocol.name.StartsWith("DE"))   _germanProtocol   = protocol;
                if (_americanProtocol == null && protocol.name.StartsWith("US"))   _americanProtocol = protocol;
            }

            foreach (var handler in Resources.FindObjectsOfTypeAll<CrewVoiceHandler>())
            {
                var p = (VoiceProtocolDataScriptable)_protocolField.GetValue(handler);
                if (p == null) continue;
                if (_russianProtocol  == null && p.name.StartsWith("USSR")) _russianProtocol  = p;
                if (_germanProtocol   == null && p.name.StartsWith("DE"))   _germanProtocol   = p;
                if (_americanProtocol == null && p.name.StartsWith("US"))   _americanProtocol = p;
            }

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

            string value = Config.GetValue(entry);
            if (Config.IsCustomVoice(value)) return;

            VoiceProtocolDataScriptable target = value switch
            {
                "Russian"  => _russianProtocol,
                "German"   => _germanProtocol,
                "American" => _americanProtocol,
                _ => null
            };

            if (target == null)
            {
                CacheProtocols();
                target = value switch
                {
                    "Russian"  => _russianProtocol,
                    "German"   => _germanProtocol,
                    "American" => _americanProtocol,
                    _ => null
                };
            }

            if (target == null)
            {
                MelonLogger.Warning($"Protocol for '{value}' not cached yet, skipping {vehicleName}");
                return;
            }

            if (target == current) return;

            _protocolField.SetValue(__instance, target);
            MelonLogger.Msg($"Set {vehicleName} -> {value} ({target.name})");
        }
    }
}