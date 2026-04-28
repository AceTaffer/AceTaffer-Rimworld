using HarmonyLib;
using Verse;

namespace PumpkinShield
{
    /// <summary>
    /// 模组主入口类，在游戏加载时应用 Harmony 补丁。
    /// </summary>
    public class PumpkinShieldMod : Mod
    {
        public PumpkinShieldMod(ModContentPack content) : base(content)
        {
            Log.Message("[PVZRimWorld] PumpkinShieldMod loaded.");
            Harmony harmony = new Harmony("acct.PVZRimWorld.PumpkinShield");
            harmony.PatchAll();
        
        }
       
      
    }
}