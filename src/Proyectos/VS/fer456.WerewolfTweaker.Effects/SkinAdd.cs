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
    public class SkinAdd
    {
        [Tunable]
        internal static bool kInstantiator = false;
        private static CASPart SkinAddMask = new CASPart();
        [PersistableStatic]
        public static Dictionary<Sim, bool> skinnedSims = new Dictionary<Sim, bool>();
        private static bool debugOn = false;
        private static Sim lastTriggered = null;
        private static ResourceKey maskPartSkin = ResourceKey.kInvalidResourceKey;
        public static EventListener sSkinAddBuffListener;

        static SkinAdd()
        {
            SkinAdd.sSkinAddBuffListener = null;
            World.OnWorldLoadFinishedEventHandler += new EventHandler(SkinAdd.OnWorldLoadFinishedHandler);
            World.OnWorldQuitEventHandler += new EventHandler(SkinAdd.World_OnWorldQuitEventHandler);
        }

        public static int cleanWereSkinAdds(object obj)
        {
            try
            {
                foreach (KeyValuePair<Sim, bool> pair in skinnedSims)
                {
                    if (skinnedSims[pair.Key])
                    {
                        Simulator.AddObject(new OneShotFunctionWithParams(new FunctionWithParam(SkinAdd.runClean), pair.Key));
                    }
                }
                skinnedSims.Clear();
            }
            catch (Exception exception)
            {
                SimpleMessageDialog.Show("Werewolf Tweaker - Uninstalling Wizard", "An error is ocurred when uninstalling. Check notification bar");
                StyledNotification.Show(new StyledNotification.Format("Failed the removal of the sim dictionary (skinnedSims) \n" + exception, StyledNotification.NotificationStyle.kDebugAlert));
            }
            return 1;
        }
        private static ListenerAction OnClean(Event e)
        {
            Sim actor = e.Actor as Sim;
            if (actor != null)
            {
                Simulator.AddObject(new OneShotFunctionWithParams(new FunctionWithParam(SkinAdd.RemoveMakeup), actor));
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
                        Simulator.AddObject(new OneShotFunctionWithParams(new FunctionWithParam(SkinAdd.ProcessBuff), actor));
                    }
                }
                else if (skinnedSims.ContainsKey(actor))
                {
                    Simulator.AddObject(new OneShotFunctionWithParams(new FunctionWithParam(SkinAdd.RemoveMakeup), actor));
                    skinnedSims.Remove(actor);
                }
            }
            return ListenerAction.Keep;
        }

        public static void OnWorldLoadFinishedHandler(object sender, EventArgs e)
        {
            maskPartSkin = ResourceKey.Parse(0x034AEECB + "-" + 0x00000000 + "-" + 0x9F734D5B9D920083);
            sSkinAddBuffListener = EventTracker.AddListener(EventTypeId.kGotBuff, new ProcessEventDelegate(SkinAdd.OnGotBuff));
            PartSearch search = new PartSearch();
            foreach (CASPart part in search)
            {
                if (part.Key == maskPartSkin)
                {
                    SkinAddMask = part;
                }
            }
            search.Reset();
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
                            if (!skinnedSims.ContainsKey(sim))
                            {
                                skinnedSims.Add(sim, true);
                            }
                            else
                            {
                                skinnedSims[sim] = true;
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
                            if (!skinnedSims.ContainsKey(sim))
                            {
                                skinnedSims.Remove(sim);
                            }
                            else
                            {
                                skinnedSims[sim] = false;
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
            if ((key.SimDescription.IsWerewolf && skinnedSims.ContainsKey(key)) && skinnedSims[key])
            {
                skinnedSims[key] = false;
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
                SimpleMessageDialog.Show("SkinAdd - Debugging Enabled", msg);
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
                    builder.AddPart(SkinAddMask);
                }
                else
                {
                    builder.RemovePart(SkinAddMask);
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
                                        builder.AddPart(SkinAddMask);                                        
                                    }
                                    else
                                    {
                                        builder.RemovePart(SkinAddMask);
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
                SimpleMessageDialog.Show("Werewolf Tweaker - Skin", sim.Name + "\nApply skin failed!\n" + exception);
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
                skinnedSims = null;
            }
        }
    }
}
