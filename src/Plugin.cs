using BepInEx;

namespace Silksong.FsmUtil;

[BepInAutoPlugin("org.silksong-modding.fsmutil")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public partial class Plugin : BaseUnityPlugin
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
{
    private void Awake()
    {
        InternalLogger.Logger = Logger;

        Logger.LogInfo($"Plugin {Name} ({Id}) has loaded!");
    }
}