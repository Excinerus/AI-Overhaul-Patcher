using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using AIOverhaulPatcher.Utilities;
namespace AIOverhaulPatcher
{
    public class Program
    {

        public static int Main(string[] args)
        {
            return SynthesisPipeline.Instance.Patch<ISkyrimMod, ISkyrimModGetter>(
                args: args,
                patcher: RunPatch,
                userPreferences: new UserPreferences()
                {
                    ActionsForEmptyArgs = new RunDefaultPatcher()
                    {
                        IdentifyingModKey = "AIOPatch.esp",
                        TargetRelease = GameRelease.SkyrimSE,
                        BlockAutomaticExit = true,
                    }
                });
        }

        public static void RunPatch(SynthesisState<ISkyrimMod, ISkyrimModGetter> state)
        {

            var AIOverhaul = state.LoadOrder.GetModByFileName( "AI Overhaul.esp");
            var USLEEP = state.LoadOrder.GetModByFileName("Unofficial Skyrim Special Edition Patch.esp");
            if (USLEEP!=null ) System.Console.WriteLine("Unofficial Skyrim Special Edition Patch.esp");
            var UsleepOrder = state.LoadOrder.GetFileOrder("Unofficial Skyrim Special Edition Patch.esp");
            System.Console.WriteLine("at " + UsleepOrder);
            //Your code here!

            if (AIOverhaul == null)
            {
                
                System.Console.WriteLine("AIOverhaul.esp not found");
                return;
            }


            int bmax = 10;
            int b = 0;
            int processed = 0;
            int total = AIOverhaul.Npcs.Count;

            var AIOFormIDs = AIOverhaul.Npcs.Select(x => x.FormKey.ID).ToList();
            var winningOverrides = state.LoadOrder.PriorityOrder.WinningOverrides<INpcGetter>().Where(x => AIOFormIDs.Contains(x.FormKey.ID)).ToList();
            var USLEEPandPrior = state.LoadOrder.PriorityOrder.Reverse().Take(UsleepOrder + 1).Select(x => x.Mod).ToList();
            var masterfilenames = AIOverhaul.MasterReferences.Select(x => x.Master.FileName).ToList();
            var MasterFiles = state.LoadOrder.PriorityOrder.Reverse().Where(x => masterfilenames.Contains(x.ModKey.FileName)).ToList();
            var NPCMasters = MasterFiles.SelectMany(x => x.Mod?.GetTopLevelGroupGetter<INpcGetter>()).Select(x=>x.Value).Where(x => AIOFormIDs.Contains(x.FormKey.ID)).ToList();

            var allOverrides = state.LoadOrder.PriorityOrder.Reverse().Skip(UsleepOrder + 1).Select(x => x.Mod).SelectMany(x => x?.GetTopLevelGroupGetter<INpcGetter>()).Where(x => AIOFormIDs.Contains(x.Value.FormKey.ID)).Select(x => x.Value).ToList();
         

        System.Console.WriteLine(processed + "/" + total + " Npcs");
            foreach (var npc in AIOverhaul.Npcs)
            {

                if (b >= bmax)
                {
                    b = 0;
                    System.Console.WriteLine(processed + "/" + total + " Npcs");
                }

                var winningOverride = winningOverrides.Where(x => x.FormKey.ID == npc.FormKey.ID).FirstOrDefault();
                var Masters = NPCMasters.Where(x => x.FormKey.ID == npc.FormKey.ID).ToList();
                var winningMaster = Masters.FirstOrDefault();
                if (winningMaster == null) winningMaster = state.LoadOrder.PriorityOrder.Select(x => x.Mod).SelectMany(x => x?.GetTopLevelGroupGetter<INpcGetter>()).Where(x => x.Value.FormKey.ID == npc.FormKey.ID).Select(x => x.Value).First();
                var overrides = allOverrides.Where(x => x.FormKey.ID == npc.FormKey.ID).ToList();

                var patchNpc = state.PatchMod.Npcs.GetOrAddAsOverride(winningOverride);
                if (winningOverride != npc)
                {
                    if (npc.IsProtected()) patchNpc.Configuration.Flags.SetProtected(true, true);

                    foreach (var fac in npc.Factions)
                        if (!patchNpc.Factions.Select(x => new KeyValuePair<uint,int> (x.Faction.FormKey.ID , x.Rank)).Contains(new KeyValuePair<uint, int>(fac.Faction.FormKey.ID, fac.Rank)))
                        {
                            patchNpc.Factions.Add(  fac.DeepCopy());
                        }

                     
                    var PackagesToRemove = Masters.SelectMany(x=>x.Packages).Select(x=>x.FormKey.ID).Where(x => !npc.Packages.Select(x=>x.FormKey.ID).Contains(x)).ToList();

                    List<FormKey> PackagesToAdd = overrides.SelectMany(x => x.Packages).Select(x => x.FormKey).Where(x => !PackagesToRemove.Contains(x.ID)).Distinct().ToList();

                    patchNpc.Packages.Clear();
                    PackagesToAdd.ForEach(x => patchNpc.Packages.Add(x));


                    var OverwrittenOutfits = Masters.Select(x => x.DefaultOutfit).Select(x => x.FormKeyNullable.HasValue ? x.FormKeyNullable.Value.ID : 0).Distinct().ToList();
                    var OverwrittenSleepingOutfit = Masters.Select(x => x.SleepingOutfit).Select(x => x.FormKeyNullable.HasValue ? x.FormKeyNullable.Value.ID : 0).Distinct().ToList();

                    FormKey? OverwrittingOutfit = overrides.Select(x => x.DefaultOutfit).Select(x => x.FormKeyNullable).Where(x => x != null && !OverwrittenOutfits.Contains(x.Value.ID)).Prepend(npc.DefaultOutfit.FormKey).Last();
                    FormKey? OverwrittingSleepingOutfit = overrides.Select(x => x.SleepingOutfit).Select(x => x.FormKeyNullable).Where(x => x != null && !OverwrittenSleepingOutfit.Contains(x.Value.ID)).Prepend(npc.SleepingOutfit.FormKey).Last();

                    patchNpc.DefaultOutfit = OverwrittingOutfit;
                    patchNpc.SleepingOutfit = OverwrittingSleepingOutfit;

                    patchNpc.SpectatorOverridePackageList = npc.SpectatorOverridePackageList.FormKey;
                    patchNpc.CombatOverridePackageList = npc.CombatOverridePackageList.FormKey;
                    patchNpc.AIData.Confidence = (Confidence)Math.Min((int)patchNpc.AIData.Confidence, (int)npc.AIData.Confidence);


                    
                    if (npc.VirtualMachineAdapter != null)
                    {
                        List<IScriptEntryGetter> ScriptsToForward = npc.VirtualMachineAdapter.Scripts.Where(x => patchNpc.VirtualMachineAdapter == null || !patchNpc.VirtualMachineAdapter.Scripts.Select(x => x.Name).Contains(x.Name)).ToList();
                        if (ScriptsToForward.Count > 0)
                        {
                            if (patchNpc.VirtualMachineAdapter == null)
                                patchNpc.VirtualMachineAdapter = npc.VirtualMachineAdapter.DeepCopy();
                            else
                            {
                                ScriptsToForward.ForEach(x => patchNpc.VirtualMachineAdapter.Scripts.Add(x.DeepCopy()));
                            }
                        }

                    }


                }
                b++;
                processed++;
            }



        }
    }
}
