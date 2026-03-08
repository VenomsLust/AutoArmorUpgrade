using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;


namespace AutoArmorUpgrade {
    /// <summary>
    /// A campaign behavior that automatically upgrades the armor of heroes in the main party when collecting loot.
    /// This behavior listens for loot collection events and evaluates available armor items against currently equipped gear,
    /// equipping superior items as they are found.
    /// The armor score is calculated based on the protective values of the equipment and its weight, providing a balanced assessment of its effectiveness.
    /// This behavior enhances gameplay by ensuring that heroes are always equipped with the best available armor without requiring manual inventory management.
    /// </summary>
    public class AutoArmorBehavior : CampaignBehaviorBase {

        private bool _pendingUpgrade = true;


        /// <summary>
        /// Registers event handlers required for the current instance.
        /// </summary>
        /// <remarks>Call this method to subscribe the instance to relevant campaign events. This enables
        /// the instance to respond to loot collection events during the campaign lifecycle. This method is typically
        /// called during initialization and should not be called multiple times for the same instance.</remarks>
        public override void RegisterEvents() {
            CampaignEvents.OnCollectLootsItemsEvent.AddNonSerializedListener(this, OnCollectLootsItems);
            CampaignEvents.ItemsLooted.AddNonSerializedListener(this, OnItemsLooted);
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, OnHourlyTick);
            CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, OnGameMenuOpened);
        }


        /// <summary>
        /// Handles the event triggered when the game menu is opened.
        /// </summary>
        /// <param name="args">Provides data for the menu opening event, including context and any relevant parameters.</param>
        private void OnGameMenuOpened(MenuCallbackArgs args) {
            _pendingUpgrade = true;
        }


        /// <summary>
        /// Handles the event when items are looted by a party, performing hero upgrades if the main party acquires
        /// items.
        /// </summary>
        /// <remarks>This method only triggers hero upgrades when the main player party loots items. No
        /// action is taken for other parties or if the item roster is empty.</remarks>
        /// <param name="party">The party that has looted items. Must not be null. Only actions are taken if this is the main player party.</param>
        /// <param name="roster">The roster of items that have been looted. Must not be null or empty for upgrades to occur.</param>
        private void OnItemsLooted(MobileParty party, ItemRoster roster) {
            if (party != null && party == MobileParty.MainParty && roster != null && !roster.IsEmpty()) {
                _pendingUpgrade = true;
            }
        }


        /// <summary>
        /// Scans the specified loot roster for potential equipment upgrades for all heroes in the main party.
        /// </summary>
        /// <remarks>This method only performs the scan if the provided party is the main party and the
        /// loot roster is not empty. A notification message is displayed to inform the player when the scan
        /// begins.</remarks>
        /// <param name="party">The party to evaluate for equipment upgrades. Must be the main party to trigger the scan.</param>
        /// <param name="roster">The item roster containing loot to be scanned for possible upgrades. Must not be null or empty.</param>
        private void OnCollectLootsItems(PartyBase party, ItemRoster roster) {
            if (party != null && party == PartyBase.MainParty && roster != null && !roster.IsEmpty()) {
                _pendingUpgrade = true;
            }
        }


        /// <summary>
        /// Handles hourly update logic, performing hero upgrades for the main party if an upgrade is pending.
        /// </summary>
        /// <remarks>This method is intended to be called once per in-game hour to process any pending
        /// hero upgrades. It does not perform any action if no upgrade is pending.</remarks>
        private void OnHourlyTick() {
            if (_pendingUpgrade) {
                _pendingUpgrade = false;
                UpgradeAllHeroes(PartyBase.MainParty);
            }
        }


        /// <summary>
        /// Upgrades the equipment of all hero characters in the specified party using available armor items from the
        /// party's item roster.
        /// </summary>
        /// <remarks>Only hero characters in the party are considered for upgrades. The method organizes
        /// available armor items by type and assigns the best available equipment to each hero based on a calculated
        /// armor score. If either the member roster or item roster is null, the method performs no action.</remarks>
        /// <param name="party">The party whose hero members will be upgraded. Must have a non-null member and item roster.</param>
        private void UpgradeAllHeroes(PartyBase party) {
            if (party.MemberRoster == null || party.ItemRoster == null) {
                return;
            }

            Dictionary<ItemObject.ItemTypeEnum, List<EquipmentElement>> buckets = new Dictionary<ItemObject.ItemTypeEnum, List<EquipmentElement>>();

            // First, we loop through the loot and organize all armor pieces by their type (head, body, leg, gloves, cape)
            foreach (ItemRosterElement element in party.ItemRoster) {
                if (0 < element.Amount && element.EquipmentElement.Item is ItemObject item) {
                    ItemObject.ItemTypeEnum type = item.ItemType;
                    if (type == ItemObject.ItemTypeEnum.Invalid) {
                        continue; // Skip items that don't have a valid type
                    }

                    if (!buckets.ContainsKey(type)) {
                        buckets[type] = new List<EquipmentElement>();
                    }
                    buckets[type].Add(element.EquipmentElement);
                }
            }

            foreach (KeyValuePair<ItemObject.ItemTypeEnum, List<EquipmentElement>> kvp in buckets) {
                if (1 < kvp.Value.Count) {
                    if (EquipmentScores.GetScoreFunc(kvp.Key) is Func<EquipmentElement, float> score) {
                        kvp.Value.Sort((a, b) => score(b).CompareTo(score(a)));
                    }
                }
            }

            for (int i = 0; i < party.MemberRoster.Count; i++) {
                TroopRosterElement member = party.MemberRoster.GetElementCopyAtIndex(i);
                if (member.Character?.HeroObject is Hero hero && hero.IsAlive) {
                    if (buckets.ContainsKey(ItemObject.ItemTypeEnum.HeadArmor)) {
                        UpgradeHeroArmor(hero, party.ItemRoster, buckets[ItemObject.ItemTypeEnum.HeadArmor], EquipmentIndex.Head);
                    }
                    if (buckets.ContainsKey(ItemObject.ItemTypeEnum.BodyArmor)) {
                        UpgradeHeroArmor(hero, party.ItemRoster, buckets[ItemObject.ItemTypeEnum.BodyArmor], EquipmentIndex.Body);
                    }
                    if (buckets.ContainsKey(ItemObject.ItemTypeEnum.LegArmor)) {
                        UpgradeHeroArmor(hero, party.ItemRoster, buckets[ItemObject.ItemTypeEnum.LegArmor], EquipmentIndex.Leg);
                    }

                    if (buckets.ContainsKey(ItemObject.ItemTypeEnum.HandArmor)) {
                        UpgradeHeroArmor(hero, party.ItemRoster, buckets[ItemObject.ItemTypeEnum.HandArmor], EquipmentIndex.Gloves);
                    }

                    if (buckets.ContainsKey(ItemObject.ItemTypeEnum.Cape)) {
                        UpgradeHeroArmor(hero, party.ItemRoster, buckets[ItemObject.ItemTypeEnum.Cape], EquipmentIndex.Cape);
                    }

                    if (!hero.BattleEquipment[EquipmentIndex.Horse].IsEmpty) {
                        if (buckets.ContainsKey(ItemObject.ItemTypeEnum.HorseHarness)) {
                            UpgradeHeroArmor(hero, party.ItemRoster, buckets[ItemObject.ItemTypeEnum.HorseHarness], EquipmentIndex.HorseHarness);
                        }

                        if (buckets.ContainsKey(ItemObject.ItemTypeEnum.Horse)) {
                            UpgradeHeroHorse(hero, party.ItemRoster, buckets[ItemObject.ItemTypeEnum.Horse]);
                        }
                    }

                    UpgradeHeroWeaponSlot(hero, party.ItemRoster, buckets, EquipmentIndex.Weapon0);
                    UpgradeHeroWeaponSlot(hero, party.ItemRoster, buckets, EquipmentIndex.Weapon1);
                    UpgradeHeroWeaponSlot(hero, party.ItemRoster, buckets, EquipmentIndex.Weapon2);
                    UpgradeHeroWeaponSlot(hero, party.ItemRoster, buckets, EquipmentIndex.Weapon3);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="element"></param>
        private static void SortedInsert(List<EquipmentElement> list, EquipmentElement element, Func<EquipmentElement, float> getScore) {
            if (list.Count == 0) {
                list.Add(element);
                return;
            }
            int low = 0;
            int high = list.Count - 1;
            float score = getScore(element);
            if (score < getScore(list[low])) {
                low++;
                if (0 < high && getScore(list[high]) < score) {
                    high--;
                    while (low < high) {
                        int mid = (low + high) >> 1;
                        float iScore = getScore(list[mid]);
                        if (iScore < score) {
                            high = mid - 1;
                        } else if (score < iScore) {
                            low = mid + 1;
                        } else {
                            list.Insert(mid, element);
                            return;
                        }
                    }
                    list.Insert(low, element);
                } else {
                    list.Add(element);
                }
            } else {
                list.Insert(0, element);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hero"></param>
        /// <param name="inventory"></param>
        /// <param name="sorted"></param>
        /// <param name="slot"></param>
        private static void UpgradeHeroArmor(Hero hero, ItemRoster inventory, List<EquipmentElement> sorted, EquipmentIndex slot) {
            if (0 < sorted.Count && sorted[0] is EquipmentElement top) {
                EquipmentElement existing = hero.BattleEquipment[slot];
                if (EquipmentScores.GetScoreFunc(existing.Item.ItemType) is Func<EquipmentElement, float> score) {
                    if (score(existing) < score(top)) {
                        DoSwap(hero, top, inventory, sorted, slot);
                    }
                }
            }
        }


        /// <summary>
        /// Upgrades the specified hero's horse to the highest-scoring available mount from the sorted horse list,
        /// updating the hero's equipment and the item roster accordingly.
        /// </summary>
        /// <remarks>If the hero's current horse is replaced, the previous mount is returned to the sorted
        /// horse list and re-sorted by score. This method does not perform any action if no suitable horse is available
        /// in the sorted list.</remarks>
        /// <param name="hero">The hero whose horse equipment will be upgraded. Cannot be null.</param>
        /// <param name="inventory">The item roster used to manage equipment changes during the upgrade process. Cannot be null.</param>
        private void UpgradeHeroHorse(Hero hero, ItemRoster inventory, List<EquipmentElement> sorted) {
            EquipmentElement existing = hero.BattleEquipment[EquipmentIndex.Horse];
            if (EquipmentScores.GetScoreFunc(ItemObject.ItemTypeEnum.Horse) is Func<EquipmentElement, float> getScore) {
                float currentScore = getScore(existing);
                for (int i = 0; i < sorted.Count; i++) {
                    if (sorted[i] is EquipmentElement element) {
                        if (currentScore < getScore(element)) {
                            if (element.Item.Difficulty <= hero.GetSkillValue(DefaultSkills.Riding)) {
                                DoSwap(hero, element, inventory, sorted, EquipmentIndex.Horse);
                                break;
                            }
                        } else {
                            break;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hero"></param>
        /// <param name="inventory"></param>
        /// <param name="buckets"></param>
        /// <param name="slot"></param>
        private void UpgradeHeroWeaponSlot(Hero hero, ItemRoster inventory, Dictionary<ItemObject.ItemTypeEnum, List<EquipmentElement>> buckets, EquipmentIndex slot) {
            EquipmentElement existing = hero.BattleEquipment[slot];
            if (existing.IsEmpty || existing.Item == null) {
                return;
            }
            if (buckets.TryGetValue(existing.Item.ItemType, out List<EquipmentElement> sorted)) {
                if (0 < sorted.Count) {
                    if (existing.Item.ItemType == ItemObject.ItemTypeEnum.Bow) {
                        UpgradeHeroBow(hero, inventory, sorted, existing, slot);
                    } else {
                        UpgradeHeroWeaponSlot(hero, inventory, sorted, existing, slot);
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hero"></param>
        /// <param name="inventory"></param>
        /// <param name="sorted"></param>
        /// <param name="existing"></param>
        /// <param name="slot"></param>
        private void UpgradeHeroWeaponSlot(Hero hero, ItemRoster inventory, List<EquipmentElement> sorted, EquipmentElement existing, EquipmentIndex slot) {
            if (EquipmentScores.GetScoreFunc(existing.Item.ItemType) is Func<EquipmentElement, float> getScore) {
                int heroSkill = hero.GetSkillValue(existing.Item.RelevantSkill);
                float eScore = getScore(existing);
                int index = 0;
                EquipmentElement top = sorted[index];
                while (eScore < getScore(top)) {
                    if (top.Item.Difficulty <= heroSkill) {
                        DoSwap(hero, top, inventory, sorted, slot);
                        return;
                    }

                    index++;
                    if (index < sorted.Count) {
                        top = sorted[index];
                    } else {
                        break;
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hero"></param>
        /// <param name="inventory"></param>
        /// <param name="sorted"></param>
        /// <param name="existing"></param>
        /// <param name="slot"></param>
        private void UpgradeHeroBow(Hero hero, ItemRoster inventory, List<EquipmentElement> sorted, EquipmentElement existing, EquipmentIndex slot) {
            int heroSkill = hero.GetSkillValue(DefaultSkills.Bow);
            bool isMounted = !hero.BattleEquipment[EquipmentIndex.Horse].IsEmpty;
            bool horseMaster = hero.GetPerkValue(DefaultPerks.Bow.HorseMaster);
            float eScore = EquipmentScores.CalcBowScore(existing);
            int index = 0;
            EquipmentElement top = sorted[index];
            while (eScore < EquipmentScores.CalcBowScore(top)) {
                if (top.Item.Difficulty <= heroSkill) {
                    bool isLongbow = top.Item.WeaponComponent.PrimaryWeapon.ItemUsage.Contains("long");
                    if (!isMounted || (!isLongbow || horseMaster)) {
                        DoSwap(hero, top, inventory, sorted, slot);
                        return;
                    }
                }

                index++;
                if (index < sorted.Count) {
                    top = sorted[index];
                } else {
                    break;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hero"></param>
        /// <param name="element"></param>
        /// <param name="inventory"></param>
        /// <param name="sorted"></param>
        /// <param name="slot"></param>
        private static void DoSwap(Hero hero, EquipmentElement element, ItemRoster inventory, List<EquipmentElement> sorted, EquipmentIndex slot) {
            EquipmentElement existing = hero.BattleEquipment[slot];
            if (existing.IsEmpty) {
                string name = element.GetModifiedItemName().ToString();
                InformationManager.DisplayMessage(new InformationMessage($"{hero.Name} equipped {name}.", Color.FromUint(0xFFFFFF00)));
            } else {
                string oldName = existing.GetModifiedItemName().ToString();
                string newName = element.GetModifiedItemName().ToString();
                InformationManager.DisplayMessage(new InformationMessage($"{hero.Name} replaced {oldName} with {newName}.", Color.FromUint(0xFFFFFF00)));
            }
            hero.BattleEquipment[slot] = element;
            int index = inventory.AddToCounts(element, -1);
            if (index < 0 || inventory.GetElementNumber(index) < 1 || !inventory.GetElementCopyAtIndex(index).EquipmentElement.IsEqualTo(element)) {
                sorted.Remove(element);
            }

            if (!existing.IsEmpty) {
                inventory.AddToCounts(existing, 1);
                SortedInsert(sorted, existing, EquipmentScores.GetScoreFunc(existing.Item.ItemType));
            }
        }


        /// <summary>
        /// Synchronizes data with the specified data store. This implementation performs no operation.
        /// </summary>
        /// <remarks>Override this method in a derived class to provide custom synchronization
        /// logic.</remarks>
        /// <param name="dataStore">The data store to synchronize with. This parameter is ignored in this implementation.</param>
        public override void SyncData(IDataStore dataStore) {
            // noop
        }
    }
}
