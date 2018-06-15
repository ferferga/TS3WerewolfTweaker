using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WerewolfTweaker.Effects
{
    public class WolfFeet
    {
        [Tunable]
        internal static bool kInstantiator = false;
        private static CASPart WolfFeetMaskChildUnisex = new CASPart();
        private static CASPart WolfFeetMaskTeenFemale = new CASPart();
        private static CASPart WolfFeetMaskTeenMale = new CASPart();
        private static CASPart WolfFeetMaskAdultMale = new CASPart();
        private static CASPart WolfFeetMaskAdultFemale = new CASPart();
        private static CASPart WolfFeetMaskElderFemale = new CASPart();
        private static CASPart WolfFeetMaskElderMale = new CASPart();
        [PersistableStatic]
        public static Dictionary<Sim, bool> transformedSims = new Dictionary<Sim, bool>();                
        private static bool debugOn = false;
        private static Sim lastTriggered = null;
        private static ResourceKey maskPartChildUnisex = ResourceKey.kInvalidResourceKey;
        private static ResourceKey maskPartTeenFemale = ResourceKey.kInvalidResourceKey;
        private static ResourceKey maskPartTeenMale = ResourceKey.kInvalidResourceKey;
        private static ResourceKey maskPartAdultMale = ResourceKey.kInvalidResourceKey;
        private static ResourceKey maskPartAdultFemale = ResourceKey.kInvalidResourceKey;
        private static ResourceKey maskPartElderFemale = ResourceKey.kInvalidResourceKey;
        private static ResourceKey maskPartElderMale = ResourceKey.kInvalidResourceKey;
        public static EventListener sWolfFeetBuffListener;

        static WolfFeet()
        {
            WolfFeet.sWolfFeetBuffListener = null;
            World.OnWorldLoadFinishedEventHandler += new EventHandler(WolfFeet.OnWorldLoadFinishedHandler);
            World.OnWorldQuitEventHandler += new EventHandler(WolfFeet.World_OnWorldQuitEventHandler);
        }

        public static int cleanWerewolfFeets(object obj)
        {
            try
            {
                foreach (KeyValuePair<Sim, bool> pair in transformedSims)
                {
                    if (transformedSims[pair.Key])
                    {
                        Simulator.AddObject(new OneShotFunctionWithParams(new FunctionWithParam(WolfFeet.runClean), pair.Key));
                    }
                }
                transformedSims.Clear();
            }
            catch (Exception exception)
            {
                SimpleMessageDialog.Show("Werewolf Tweaker - Uninstalling Wizard", "An error is ocurred when uninstalling. Check notification bar");
                StyledNotification.Show(new StyledNotification.Format("Failed the removal of the sim dictionary (transformedSims) \n" + exception, StyledNotification.NotificationStyle.kDebugAlert));
            }
            return 1;
        }
        private static ListenerAction OnClean(Event e)
        {
            Sim actor = e.Actor as Sim;
            if (actor != null)
            {
                Simulator.AddObject(new OneShotFunctionWithParams(new FunctionWithParam(WolfFeet.RemoveMakeup), actor));
            }
            return ListenerAction.Keep;
        }

        private static ListenerAction OnGotBuff(Event e)
        {
            Sim actor = e.Actor as Sim;
            if (actor != null)
            {
                if (actor.SimDescription.IsWerewolf)
                {
                    if ((lastTriggered == null) || (lastTriggered != actor))
                    {
                        Simulator.AddObject(new OneShotFunctionWithParams(new FunctionWithParam(WolfFeet.ProcessBuff), actor));
                    }
                }
                else if (transformedSims.ContainsKey(actor))
                {
                    Simulator.AddObject(new OneShotFunctionWithParams(new FunctionWithParam(WolfFeet.RemoveMakeup), actor));
                    transformedSims.Remove(actor);
                }
            }
            return ListenerAction.Keep;
        }

        public static void OnWorldLoadFinishedHandler(object sender, EventArgs e)
        {
            maskPartChildUnisex = ResourceKey.Parse(0x034AEECB + "-" + 0x00000000 + "-" + 0x9F734D5B9D920083);
            maskPartAdultFemale = ResourceKey.Parse(0x034AEECB + "-" + 0x00000000 + "-" + 0x0805FC6BD47FF5C7);
            maskPartAdultMale = ResourceKey.Parse(0x034AEECB + "-" + 0x00000000 + "-" + 0x8FFB29C402EFE024);
            maskPartTeenMale = ResourceKey.Parse(0x034AEECB + "-" + 0x00000000 + "-" + 0xCD9D1D26E4ABD38A);
            maskPartTeenFemale = ResourceKey.Parse(0x034AEECB + "-" + 0x00000000 + "-" + 0x02172C0B1C6D80F1);
            maskPartElderFemale = ResourceKey.Parse(0x034AEECB + "-" + 0x00000000 + "-" + 0xA13212F706933D83);
            maskPartElderMale = ResourceKey.Parse(0x034AEECB + "-" + 0x00000000 + "-" + 0xFDF0F0D989D601D0);
            sWolfFeetBuffListener = EventTracker.AddListener(EventTypeId.kGotBuff, new ProcessEventDelegate(WolfFeet.OnGotBuff));
            PartSearch search = new PartSearch();
            foreach (CASPart part in search)
            {
                if (part.Key == maskPartChildUnisex)
                {
                    WolfFeetMaskChildUnisex = part;
                }
            }
            search.Reset();
            PartSearch search2 = new PartSearch();
            foreach (CASPart part in search2)
            {
                if (part.Key == maskPartTeenFemale)
                {
                    WolfFeetMaskTeenFemale = part;
                }
            }
            search2.Reset();
            PartSearch search3 = new PartSearch();
            foreach (CASPart part in search3)
            {
                if (part.Key == maskPartTeenMale)
                {
                    WolfFeetMaskTeenMale = part;
                }
            }
            search3.Reset();
            PartSearch search4 = new PartSearch();
            foreach (CASPart part in search4)
            {
                if (part.Key == maskPartAdultFemale)
                {
                    WolfFeetMaskAdultFemale = part;
                }
            }
            search4.Reset();
            PartSearch search5 = new PartSearch();
            foreach (CASPart part in search5)
            {
                if (part.Key == maskPartAdultMale)
                {
                    WolfFeetMaskAdultMale = part;
                }
            }
            search5.Reset();
            PartSearch search6 = new PartSearch();
            foreach (CASPart part in search6)
            {
                if (part.Key == maskPartElderFemale)
                {
                    WolfFeetMaskElderFemale = part;
                }
            }
            search6.Reset();
            PartSearch search7 = new PartSearch();
            foreach (CASPart part in search7)
            {
                if (part.Key == maskPartElderMale)
                {
                    WolfFeetMaskElderMale = part;
                }
            }
            search7.Reset();
        }
        private static void ProcessBuff(object obj)
        {
            Sim sim = obj as Sim;
            bool flag = false;
            using (IEnumerator<BuffInstance> enumerator = sim.BuffManager.Buffs.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    BuffInstance current = enumerator.Current;
                    if (current.mBuffGuid == BuffNames.Werewolf)
                    {
                        flag = true;
                        if (current.mStartTime > (SimClock.CurrentTicks - 0x2dL))
                        {
                            lastTriggered = sim;
                            SetFeet(sim, true);
                            if (!transformedSims.ContainsKey(sim))
                            {
                                transformedSims.Add(sim, true);
                            }
                            else
                            {
                                transformedSims[sim] = true;
                            }
                            lastTriggered = null;
                            return;
                        }
                    }
                }
            }
            using (IEnumerator<BuffInstance> enumerator = sim.BuffManager.Buffs.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    BuffInstance current = enumerator.Current;
                    if (current.mBuffGuid == BuffNames.WhatJustHappened && current.mBuffOrigin == Origin.FromWerewolfTransformation)
                    {
                        flag = true;
                        if (current.mStartTime > (SimClock.CurrentTicks - 0x2dL))
                        {
                            lastTriggered = sim;
                            SetFeet(sim, false);
                            if (!transformedSims.ContainsKey(sim))
                            {
                                transformedSims.Remove(sim);
                            }
                            else
                            {
                                transformedSims[sim] = false;
                            }
                            lastTriggered = null;
                            return;
                        }
                    }
                }
            }
        }

        private static void RemoveMakeup(object obj)
        {
            Sim key = obj as Sim;
            if ((key.SimDescription.IsWerewolf && transformedSims.ContainsKey(key)) && transformedSims[key])
            {
                transformedSims[key] = false;
                SetFeet(key, false);
            }
        }

        private static void runClean(object obj)
        {
            Sim sim = obj as Sim;
            if (sim != null)
                try
                {
                    {
                        SimpleMessageDialog.Show("Werewolf Tweaker - Uninstalling Wizard", "Cleaning skin of: " + sim.FullName + "...");
                        SetFeet(sim, false);
                        SimpleMessageDialog.Show("Werewolf Tweaker - Uninstalling Wizard", "Cleaned skin of: " + sim.FullName + "!");
                    }
                }
                catch (Exception exception)
                {
                    SimpleMessageDialog.Show("Werewolf Tweaker - Uninstalling Wizard", "Failed the removal process of the skin of: " + sim.FullName + "!" + "\nCheck notification bar.");
                    StyledNotification.Show(new StyledNotification.Format("Failed the uninstall process in the removal of the skin of: " + sim.FullName + "!\nTechinical info to get more information about the error: \n" + exception, StyledNotification.NotificationStyle.kDebugAlert));
                }
        }

        private static void sendDebugMsg(string msg)
        {
            if (debugOn)
            {
                SimpleMessageDialog.Show("WolfFeet - Debugging Enabled", msg);
            }
        }

        public static void SetFeet(Sim sim, bool addRemove)
        {
            try
            {
                SimBuilder builder = new SimBuilder();
                SimDescription simDescription = sim.SimDescription;
                OutfitCategories currentOutfitCategory = sim.CurrentOutfitCategory;
                int currentOutfitIndex = sim.CurrentOutfitIndex;
                builder.Clear(false);
                SimOutfit currentOutfit = sim.CurrentOutfit;
                OutfitUtils.SetOutfit(builder, currentOutfit, simDescription);
                if (addRemove)
                {
                    sendDebugMsg(sim.FullName + "\nAdding feets.");                    
                    simDescription.AddOutfit(currentOutfit, OutfitCategories.Naked);
                    if (simDescription.Child)
                    {
                        builder.AddPart(WolfFeetMaskChildUnisex);
                    }
                    if (simDescription.Teen && simDescription.IsFemale)
                    {
                        builder.AddPart(WolfFeetMaskTeenFemale);
                    }
                    if (simDescription.Teen && simDescription.IsMale)
                    {
                        builder.AddPart(WolfFeetMaskTeenMale);
                    }
                    if (simDescription.Adult && simDescription.YoungAdult && simDescription.IsFemale)
                    {
                        builder.AddPart(WolfFeetMaskAdultFemale);
                    }
                    if (simDescription.Adult && simDescription.YoungAdult && simDescription.IsMale)
                    {
                        builder.AddPart(WolfFeetMaskAdultMale);
                    }
                    if (simDescription.Elder && simDescription.IsFemale)
                    {
                        builder.AddPart(WolfFeetMaskElderFemale);
                    }
                    if (simDescription.Elder && simDescription.IsMale)
                    {
                        builder.AddPart(WolfFeetMaskElderMale);
                    }
                }
                else
                {
                    if (simDescription.Child)
                    {
                        builder.RemovePart(WolfFeetMaskChildUnisex);
                    }
                    if (simDescription.Teen && simDescription.IsFemale)
                    {
                        builder.RemovePart(WolfFeetMaskChildUnisex);
                    }
                    if (simDescription.Teen && simDescription.IsMale)
                    {
                        builder.RemovePart(WolfFeetMaskTeenMale);
                    }
                    if (simDescription.YoungAdult && simDescription.Adult && simDescription.IsMale)
                    {
                        builder.RemovePart(WolfFeetMaskAdultMale);
                    }
                    if (simDescription.YoungAdult && simDescription.Adult && simDescription.IsFemale)
                    {
                        builder.RemovePart(WolfFeetMaskAdultFemale);
                    }
                    if (simDescription.Elder && simDescription.IsFemale)
                    {
                        builder.RemovePart(WolfFeetMaskElderFemale);
                    }
                    if (simDescription.Elder && simDescription.IsMale)
                    {
                        builder.RemovePart(WolfFeetMaskElderMale);
                    }
                }
                SimOutfit outfit = new SimOutfit(builder.CacheOutfit(simDescription.FullName + currentOutfitCategory.ToString() + currentOutfitIndex.ToString()));
                if (simDescription.GetOutfitCount(currentOutfitCategory) > currentOutfitIndex)
                {
                    simDescription.RemoveOutfit(currentOutfitCategory, currentOutfitIndex, true);
                }
                simDescription.AddOutfit(outfit, currentOutfitCategory, currentOutfitIndex);
                if (simDescription.CreatedSim != null)
                {
                    sendDebugMsg("Updated: " + currentOutfitCategory.ToString() + "-" + currentOutfitIndex.ToString());
                    simDescription.CreatedSim.RefreshCurrentOutfit(false);
                }
                foreach (OutfitCategories categories2 in Enum.GetValues(typeof(OutfitCategories)))
                {
                    if (categories2 != OutfitCategories.Special)
                    {
                        ArrayList list = simDescription.GetCurrentOutfits()[categories2] as ArrayList;
                        if (list != null)
                        {
                            int count = list.Count;
                            for (int i = 0; i < count; i++)
                            {
                                if ((categories2 != currentOutfitCategory) || (i != currentOutfitIndex))
                                {
                                    builder.Clear(false);
                                    SimOutfit outfit3 = list[i] as SimOutfit;
                                    OutfitUtils.SetOutfit(builder, outfit3, simDescription);
                                    if (addRemove)
                                    {
                                        simDescription.AddOutfit(currentOutfit, OutfitCategories.Naked);
                                        if (simDescription.Child)
                                        {
                                            builder.AddPart(WolfFeetMaskChildUnisex);
                                        }
                                        if (simDescription.Teen && simDescription.IsFemale)
                                        {
                                            builder.AddPart(WolfFeetMaskTeenFemale);
                                        }
                                        if (simDescription.Teen && simDescription.IsMale)
                                        {
                                            builder.AddPart(WolfFeetMaskTeenMale);
                                        }
                                        if (simDescription.Adult && simDescription.YoungAdult && simDescription.IsFemale)
                                        {
                                            builder.AddPart(WolfFeetMaskAdultFemale);
                                        }
                                        if (simDescription.Adult && simDescription.YoungAdult && simDescription.IsMale)
                                        {
                                            builder.AddPart(WolfFeetMaskAdultMale);
                                        }
                                        if (simDescription.Elder && simDescription.IsFemale)
                                        {
                                            builder.AddPart(WolfFeetMaskElderFemale);
                                        }
                                        if (simDescription.Elder && simDescription.IsMale)
                                        {
                                            builder.AddPart(WolfFeetMaskElderMale);
                                        }
                                    }
                                    else
                                    {
                                        if (simDescription.Child)
                                        {
                                            builder.RemovePart(WolfFeetMaskChildUnisex);
                                        }
                                        if (simDescription.Teen && simDescription.IsFemale)
                                        {
                                            builder.RemovePart(WolfFeetMaskChildUnisex);
                                        }
                                        if (simDescription.Teen && simDescription.IsMale)
                                        {
                                            builder.RemovePart(WolfFeetMaskTeenMale);
                                        }
                                        if (simDescription.YoungAdult && simDescription.Adult && simDescription.IsMale)
                                        {
                                            builder.RemovePart(WolfFeetMaskAdultMale);
                                        }
                                        if (simDescription.YoungAdult && simDescription.Adult && simDescription.IsFemale)
                                        {
                                            builder.RemovePart(WolfFeetMaskAdultFemale);
                                        }
                                        if (simDescription.Elder && simDescription.IsFemale)
                                        {
                                            builder.RemovePart(WolfFeetMaskElderFemale);
                                        }
                                        if (simDescription.Elder && simDescription.IsMale)
                                        {
                                            builder.RemovePart(WolfFeetMaskElderMale);
                                        }
                                    }
                                    SimOutfit outfit4 = new SimOutfit(builder.CacheOutfit(simDescription.FullName + categories2.ToString() + i.ToString()));
                                    if (simDescription.GetOutfitCount(categories2) > i)
                                    {
                                        simDescription.RemoveOutfit(categories2, i, true);
                                    }
                                    simDescription.AddOutfit(outfit4, categories2, i);
                                    sendDebugMsg("Updated: " + categories2.ToString() + "-" + i.ToString());
                                    Sleep(0);
                                }
                            }
                        }
                    }
                }
                SimOutfit outfit5 = simDescription.GetOutfit(OutfitCategories.Everyday, 0);
                if (outfit5 != null)
                {
                    ThumbnailManager.GenerateHouseholdSimThumbnail(outfit5.Key, outfit5.Key.InstanceId, 0, ThumbnailSizeMask.Large | ThumbnailSizeMask.ExtraLarge | ThumbnailSizeMask.Medium | ThumbnailSizeMask.Small, ThumbnailTechnique.Default, true, false, simDescription.AgeGenderSpecies);
                }
            }
            catch (Exception exception)
            {
                SimpleMessageDialog.Show("Werewolf Tweaker - Wolf Feet", sim.Name + "\nApply feet failed!\n" + exception);
            }
        }

        public static bool Sleep(uint value)
        {
            try
            {
                Simulator.Sleep(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void World_OnWorldQuitEventHandler(object sender, EventArgs e)
        {
            if (PersistStatic.MainMenuLoading)
            {
                transformedSims = null;
            }
        }
    }
}
