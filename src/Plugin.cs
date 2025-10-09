using BepInEx;

namespace Silksong.FsmUtil;

[BepInAutoPlugin("org.silksong-modding.fsmutil")]
public partial class Plugin : BaseUnityPlugin
{
    private void Awake()
    {
        InternalLogger.Logger = Logger;

        Logger.LogInfo($"Plugin {Name} ({Id}) has loaded!");
    }
}