using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using System.Text;

namespace AIOverhaulPatcher.Utilities
{
    public static class ExLoadOrderUtilities
    {
        public static ISkyrimModGetter? GetModByFileName( this LoadOrder<IModListing<ISkyrimModGetter>>? LoadOrder, string Name)
        {
            if (LoadOrder == null) return null;
            var Mods = LoadOrder.Keys.Where(x => x.FileName.ToLower() == Name.ToLower()).ToList();
            return (Mods.Count > 0) ?LoadOrder[Mods.First()].Mod : null;
        }
        //public static TMajor? GetWinningOverride<TMajor> (this TMajor record , SynthesisState<ISkyrimMod, ISkyrimModGetter> state) where TMajor : class, IMajorRecordCommonGetter
        //{
        //    if (record == null || state == null) return null;
        //    var result = state.LoadOrder.PriorityOrder.WinningOverrides<TMajor>().Where(x => x.FormKey.ID == record.FormKey.ID).ToList();
        //    return (result.Count > 0) ? result.First() : null;
        //}

        //public static TMajor? GetWinningOverrideAtOrder<TMajor>(this TMajor record, int Order, SynthesisState<ISkyrimMod, ISkyrimModGetter> state) where TMajor : class, IMajorRecordCommonGetter
        //{
        //    if (record == null || state == null) return null;
        //    var result = state.LoadOrder.PriorityOrder.Take(Order + 1).Select(x => x.Mod).Select(x=>x?.GetTopLevelGroupGetter<TMajor>()).SelectMany(x=>x?.Items).Where(x => (long) x.FormKey.ID == (long) record.FormKey.ID).ToList();
        //    return (result.Count > 0) ? result.Last() : null;
        //}





        public static int GetFileOrder (this LoadOrder<IModListing<ISkyrimModGetter>>? LoadOrder, string Filename)
        {
            if (Filename == null || LoadOrder == null) return -1;
            return LoadOrder.Keys.ToList().FindIndex(0, LoadOrder.Keys.Count(), x => x.FileName.ToLower() == Filename.ToLower());
        }
 
        public static int GetOrder<TMajor>(this TMajor record, SynthesisState<ISkyrimMod, ISkyrimModGetter> state) where TMajor : class, IMajorRecordCommonGetter
        {
            if (record == null || state == null) return -1;
            return state.LoadOrder.GetFileOrder(record.FormKey.ModKey.FileName  );
        }

        //public static List<TMajor> GetMasterAndOverrides<TMajor>(this TMajor record, SynthesisState<ISkyrimMod, ISkyrimModGetter> state) where TMajor : class, IMajorRecordCommonGetter
        //{
        //    return state.LoadOrder.SelectMany(x => x.Value.Mod?.GetTopLevelGroupGetter<TMajor>()).Where(x => x.Value.FormKey.ID == record.FormKey.ID).Select(x => x.Value).ToList();
        //}
        //public static List<TMajor> GetOverrides<TMajor>(this TMajor record, SynthesisState<ISkyrimMod, ISkyrimModGetter> state) where TMajor : class, IMajorRecordCommonGetter
        //{
        //    var order = record.GetOrder(state);
        //    return state.LoadOrder.PriorityOrder.Reverse().Skip(order+1).Select(x => x.Mod).SelectMany(x=>x?.GetTopLevelGroupGetter<TMajor>()).Where(x => x.Value.FormKey.ID == record.FormKey.ID).Select(x => x.Value).ToList();
        //}
        //public static List<TMajor> GetOverriden<TMajor>(this TMajor record, SynthesisState<ISkyrimMod, ISkyrimModGetter> state) where TMajor : class, IMajorRecordCommonGetter
        //{
        //    var order = record.GetOrder(state);
        //    return state.LoadOrder.PriorityOrder.Reverse().Take(order).Select(x => x.Mod).SelectMany(x => x?.GetTopLevelGroupGetter<TMajor>()).Where(x => x.Value.FormKey.ID == record.FormKey.ID).Select(x => x.Value).ToList();
        //}
        //public static TMajor GetMaster<TMajor>(this TMajor record, SynthesisState<ISkyrimMod, ISkyrimModGetter> state) where TMajor : class, IMajorRecordCommonGetter
        //{
        //    return state.LoadOrder.PriorityOrder.Reverse().Select(x => x.Mod).SelectMany(x=>x?.GetTopLevelGroupGetter<TMajor>()).Where(x => x.Value.FormKey.ID == record.FormKey.ID).Select(x => x.Value).First();
        //}
        //public static TMajor? GetFromFile<TMajor>(this TMajor record, string FileName, SynthesisState<ISkyrimMod, ISkyrimModGetter> state) where TMajor : class, IMajorRecordCommonGetter
        //{
        //    return state?.LoadOrder?.GetModByFileName(FileName)?.GetTopLevelGroupGetter<TMajor>().Where(x => x.Value.FormKey.ID == record.FormKey.ID).FirstOrDefault()?.Value;
        //}
        //public static TMajor? GetFromMod<TMajor>(this TMajor record, ISkyrimModGetter? Mod) where TMajor : class, IMajorRecordCommonGetter
        //{
        //    return Mod?.GetTopLevelGroupGetter<TMajor>().Where(x => x.Value.FormKey.ID == record.FormKey.ID).FirstOrDefault()?.Value;
        //}


        public static void SetEssential(this NpcConfiguration.Flag Flags , bool value) => Flags = value==Flags.HasFlag(NpcConfiguration.Flag.Essential)?Flags: value ? (Flags & (~NpcConfiguration.Flag.Protected)) | NpcConfiguration.Flag.Essential : (Flags & (~NpcConfiguration.Flag.Essential));
        public static void SetProtected(this NpcConfiguration.Flag Flags, bool value, bool ignoreIfEssential = true) => Flags =  value == Flags.HasFlag(NpcConfiguration.Flag.Protected) ||(Flags.HasFlag(NpcConfiguration.Flag.Essential) && ignoreIfEssential)? Flags : value ? (Flags & (~NpcConfiguration.Flag.Essential)) | NpcConfiguration.Flag.Essential : (Flags & (~NpcConfiguration.Flag.Protected));

        public static bool IsProtected(this INpcGetter npc) => npc.Configuration.Flags.HasFlag(NpcConfiguration.Flag.Protected);
        public static bool IsEssential(this INpcGetter npc) => npc.Configuration.Flags.HasFlag(NpcConfiguration.Flag.Essential);




    }
}
