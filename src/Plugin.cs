using BepInEx;

namespace Silksong.FsmUtil;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    private void Awake()
    {
        InternalLogger.Logger = Logger;

        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} has loaded!");
    }
}