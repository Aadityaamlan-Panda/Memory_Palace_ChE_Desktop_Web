using UnityEditor;
using System;

/// <summary>
/// Sets Meta XR Simulator environment variables every time Unity loads.
/// This is needed because launchctl/shell env vars often don't reach Unity on macOS.
/// The simulator app is launched automatically by OpenXR when these are set.
/// </summary>
[InitializeOnLoad]
public static class XRSimEnvBootstrap
{
    const string JSON  = "/Applications/MetaXRSimulator.app/Contents/Resources/MetaXRSimulator/meta_openxr_simulator.json";
    const string CFG   = "/Applications/MetaXRSimulator.app/Contents/Resources/MetaXRSimulator/config/sim_core_configuration.json";
    const string PREF_KEY = "XRSimBootstrap_Enabled";

    static XRSimEnvBootstrap()
    {
        // Only auto-set if the user has activated the simulator via Meta menu
        // (stored as an EditorPrefs flag set by ActivateSimulatorPersistent)
        if (!EditorPrefs.GetBool(PREF_KEY, false)) return;

        if (!System.IO.File.Exists(JSON)) return;

        SetIfEmpty("XR_RUNTIME_JSON",          JSON);
        SetIfEmpty("XR_SELECTED_RUNTIME_JSON", JSON);
        SetIfEmpty("META_XRSIM_CONFIG_JSON",   CFG);
    }

    static void SetIfEmpty(string key, string value)
    {
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(key)))
            Environment.SetEnvironmentVariable(key, value);
    }

    [MenuItem("Meta/Meta XR Simulator/Activate Persistent (Mac Fix)")]
    public static void ActivatePersistent()
    {
        EditorPrefs.SetBool(PREF_KEY, true);
        Environment.SetEnvironmentVariable("XR_RUNTIME_JSON",          JSON);
        Environment.SetEnvironmentVariable("XR_SELECTED_RUNTIME_JSON", JSON);
        Environment.SetEnvironmentVariable("META_XRSIM_CONFIG_JSON",   CFG);
        UnityEngine.Debug.Log("[XRSimBootstrap] Simulator env vars set. Ready to Play.");
        EditorUtility.DisplayDialog("XR Simulator Ready",
            "Environment variables set.\n\nOpen XRScene and press Play.\nThis persists across Unity restarts.", "OK");
    }

    [MenuItem("Meta/Meta XR Simulator/Deactivate Persistent (Mac Fix)")]
    public static void DeactivatePersistent()
    {
        EditorPrefs.SetBool(PREF_KEY, false);
        Environment.SetEnvironmentVariable("XR_RUNTIME_JSON",          "");
        Environment.SetEnvironmentVariable("XR_SELECTED_RUNTIME_JSON", "");
        Environment.SetEnvironmentVariable("META_XRSIM_CONFIG_JSON",   "");
        UnityEngine.Debug.Log("[XRSimBootstrap] Simulator env vars cleared.");
    }

    [MenuItem("Meta/Meta XR Simulator/Check Status (Mac Fix)")]
    public static void CheckStatus()
    {
        string runtime = Environment.GetEnvironmentVariable("XR_RUNTIME_JSON");
        string selected = Environment.GetEnvironmentVariable("XR_SELECTED_RUNTIME_JSON");
        bool active = !string.IsNullOrEmpty(runtime);
        bool appExists = System.IO.File.Exists(JSON);
        string msg = $"App installed: {appExists}\nEnv vars set this session: {active}\nPersistent (survives restart): {EditorPrefs.GetBool(PREF_KEY, false)}\n\nXR_RUNTIME_JSON:\n{(string.IsNullOrEmpty(runtime) ? "(not set)" : runtime)}";
        UnityEngine.Debug.Log("[XRSimBootstrap] " + msg);
        EditorUtility.DisplayDialog("XR Simulator Status", msg, "OK");
    }
}
