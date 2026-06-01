using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace HexNoLavaCraters
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private const string PluginGuid = "com.hex.nolavacraters";
        private const string PluginName = "HexNoLavaCraters";
        private const string PluginVersion = "1.0.0";

        private Harmony _harmonyInstance;

        internal static ManualLogSource Log;
        internal static Plugin Instance;

        private void Awake()
        {
            Instance = this;
            Log = Logger;

            _harmonyInstance = new Harmony(PluginGuid);
            _harmonyInstance.PatchAll();

            Log.LogInfo($"{PluginName} v{PluginVersion} loaded.");
        }

        private void OnDestroy()
        {
            Log.LogInfo($"{PluginName} v{PluginVersion} unloaded.");

            _harmonyInstance?.UnpatchSelf();
            _harmonyInstance = null;
            Instance = null;
            Log = null;
        }

        [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
        internal static class PatchZnetSceneAwake
        {
            private static void Postfix(ZNetScene __instance)
            {
                var blobExplosionPrefab = __instance.GetPrefab("BlobLava_explosion");

                if(blobExplosionPrefab == null)
                {
                    return;
                }

                var aoe = blobExplosionPrefab.GetComponent<Aoe>();

                if(aoe == null)
                {
                    return;
                }

                var terrainPrefab = aoe.m_spawnOnHitTerrain;

                if(terrainPrefab == null)
                {
                    return;
                }

                var terrainOp = terrainPrefab.GetComponent<TerrainOp>();

                if(terrainOp == null)
                {
                    return;
                }

                terrainOp.m_settings.m_raise = false;

                Log.LogInfo("Disabled lava blob self destruct crater.");
            }
        }
    }
}
