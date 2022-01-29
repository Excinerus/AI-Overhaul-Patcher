using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using System.Text;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;

namespace AIOverhaulPatcher.Utilities
{
    public static class ExLoadOrderUtilities
    {
        public static ISkyrimModGetter? GetModByFileName( this ILoadOrder<IModListing<ISkyrimModGetter>>? LoadOrder, string Name)
        {
            if (LoadOrder == null) return null;
            var Mods = LoadOrder.Keys.Where(x => ((string) x.FileName).ToLower() == Name.ToLower()).ToList();
            return (Mods.Count > 0) ?LoadOrder[Mods.First()].Mod : null;
        }
 

        public static int GetFileOrder (this ILoadOrder<IModListing<ISkyrimModGetter>>? LoadOrder, string Filename)
        {
            if (Filename == null || LoadOrder == null) return -1;
            return LoadOrder.Keys.ToList().FindIndex(0, LoadOrder.Keys.Count(), x => ((string)x.FileName).ToLower() == Filename.ToLower());
        }
 
        public static int GetOrder<TMajor>(this TMajor record, SynthesisState<ISkyrimMod, ISkyrimModGetter> state) where TMajor : class, IMajorRecordGetter
        {
            if (record == null || state == null) return -1;

            return state.LoadOrder.HasMod(record.FormKey.ModKey, true) ? state.LoadOrder.ListedOrder.Select(x => x.ModKey).ToList().IndexOf(record.FormKey.ModKey) : -1;
        }

        //public static void SetEssential(this NpcConfiguration.Flag Flags , bool value) => Flags = value==Flags.HasFlag(NpcConfiguration.Flag.Essential)?Flags: value ? (Flags & (~NpcConfiguration.Flag.Protected)) | NpcConfiguration.Flag.Essential : (Flags & (~NpcConfiguration.Flag.Essential));
        //public static void SetProtected(this NpcConfiguration.Flag Flags, bool value, bool ignoreIfEssential = true) => Flags =  value == Flags.HasFlag(NpcConfiguration.Flag.Protected) ||(Flags.HasFlag(NpcConfiguration.Flag.Essential) && ignoreIfEssential)? Flags : value ? (Flags & (~NpcConfiguration.Flag.Essential)) | NpcConfiguration.Flag.Essential : (Flags & (~NpcConfiguration.Flag.Protected));

        public static bool IsProtected(this INpcGetter npc) => npc.Configuration.Flags.HasFlag(NpcConfiguration.Flag.Protected);
        public static bool IsEssential(this INpcGetter npc) => npc.Configuration.Flags.HasFlag(NpcConfiguration.Flag.Essential);




    }
}
