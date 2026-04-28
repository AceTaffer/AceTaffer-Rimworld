using HarmonyLib;
using Verse;

namespace TotemOfUndying
{
    public class TotemOfUndyingMod : Mod
    {
        public TotemOfUndyingMod(ModContentPack content) : base(content)
        {
            Log.Message("[TotemOfUndying] Loaded.");
            Harmony harmony = new Harmony("acct.TotemOfUndying");
            harmony.PatchAll();
        }
    }
}