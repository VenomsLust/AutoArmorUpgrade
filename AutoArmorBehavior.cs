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

    internal enum MountFamily {
        Unknown,
        Horse,
        Camel,
    }


    /// <summary>
    /// Composite key used to bucket weapon items by both their item type (OneHandedWeapon, TwoHandedWeapon, etc.)
    /// and their specific weapon class (Sword, Axe, Mace, etc.), so heroes are only upgraded within the same
    /// weapon sub-type and never have their sword swapped for an axe.
    /// </summary>
    internal readonly struct WeaponBucketKey : IEquatable<WeaponBucketKey> {
        public readonly ItemObject.ItemTypeEnum ItemType;
        public readonly WeaponClass WeaponClass;

        public WeaponBucketKey(ItemObject.ItemTypeEnum itemType, WeaponClass weaponClass) {
            ItemType = itemType;
            WeaponClass = weaponClass;
        }

        public bool Equals(WeaponBucketKey other) => ItemType == other.ItemType && WeaponClass == other.WeaponClass;
        public override bool Equals(object obj) => obj is WeaponBucketKey other && Equals(other);
        public override int GetHashCode() => ((int)ItemType ^ (int)WeaponClass);
    }


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
        /// Determines the mount family (horse or camel) for a given equipment element.
        /// </summary>
        /// <param name="element">The equipment element that represents a mount or mount armor.</param>
        /// <returns>The mount family (Horse, Camel, or Unknown if the element is not a mount).</returns>
        internal static MountFamily GetMountFamily(EquipmentElement element) {
            if (element.Item is ItemObject item) {
                if (item.HorseComponent is HorseComponent mount) {
                    return mount.Monster.MonsterUsage.Contains("camel") ? MountFamily.Camel : MountFamily.Horse;
                } else if (item.ArmorComponent is ArmorComponent armor) {
                    return item.StringId.Contains("camel") ? MountFamily.Camel : MountFamily.Horse;
                }
            }

            return MountFamily.Unknown;
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
            try {
                if (party.MemberRoster == null || party.ItemRoster == null) {
                    return;
                }

                // Armor, horse, and ammo items are bucketed by ItemTypeEnum alone (no sub-type needed).
                Dictionary<ItemObject.ItemTypeEnum, List<EquipmentElement>> armorBuckets = new Dictionary<ItemObject.ItemTypeEnum, List<EquipmentElement>>();

                // Weapon items are bucketed by (ItemTypeEnum, WeaponClass) so that swords never
                // compete with axes, maces, etc. even though they share the same ItemTypeEnum.
                Dictionary<WeaponBucketKey, List<EquipmentElement>> weaponBuckets = new Dictionary<WeaponBucketKey, List<EquipmentElement>>();

                Dictionary<ItemObject.ItemTypeEnum, List<EquipmentElement>> buckets = new Dictionary<ItemObject.ItemTypeEnum, List<EquipmentElement>>();

                foreach (ItemRosterElement element in party.ItemRoster) {
                    if (element.Amount <= 0 || !(element.EquipmentElement.Item is ItemObject item)) {
                        continue;
                    }

                    ItemObject.ItemTypeEnum type = item.ItemType;
                    if (type == ItemObject.ItemTypeEnum.Invalid) {
                        continue;
                    }

                    if (EquipmentScores.IsWeaponType(type)) {
                        WeaponClass wc = item.WeaponComponent?.PrimaryWeapon?.WeaponClass ?? WeaponClass.Undefined;
                        WeaponBucketKey key = new WeaponBucketKey(type, wc);
                        if (!weaponBuckets.ContainsKey(key)) {
                            weaponBuckets[key] = new List<EquipmentElement>();
                        }
                        weaponBuckets[key].Add(element.EquipmentElement);
                    } else {
                        if (!armorBuckets.ContainsKey(type)) {
                            armorBuckets[type] = new List<EquipmentElement>();
                        }
                        armorBuckets[type].Add(element.EquipmentElement);
                    }
                }

                // Sort armor/horse buckets descending by score.
                foreach (KeyValuePair<ItemObject.ItemTypeEnum, List<EquipmentElement>> kvp in armorBuckets) {
                    if (1 < kvp.Value.Count) {
                        if (EquipmentScores.GetScoreFunc(kvp.Key) is Func<EquipmentElement, float> score) {
                            kvp.Value.Sort((a, b) => score(b).CompareTo(score(a)));
                        }
                    }
                }

                // Sort weapon buckets descending by score.
                foreach (KeyValuePair<WeaponBucketKey, List<EquipmentElement>> kvp in weaponBuckets) {
                    if (1 < kvp.Value.Count) {
                        if (EquipmentScores.GetScoreFunc(kvp.Key.ItemType) is Func<EquipmentElement, float> score) {
                            kvp.Value.Sort((a, b) => score(b).CompareTo(score(a)));
                        }
                    }
                }

                for (int i = 0; i < party.MemberRoster.Count; i++) {
                    TroopRosterElement member = party.MemberRoster.GetElementCopyAtIndex(i);
                    if (member.Character?.HeroObject is Hero hero && hero.IsAlive) {
                        if (armorBuckets.TryGetValue(ItemObject.ItemTypeEnum.HeadArmor, out var headBucket)) {
                            UpgradeHeroArmor(hero, party.ItemRoster, headBucket, EquipmentIndex.Head);
                        }
                        if (armorBuckets.TryGetValue(ItemObject.ItemTypeEnum.BodyArmor, out var bodyBucket)) {
                            UpgradeHeroArmor(hero, party.ItemRoster, bodyBucket, EquipmentIndex.Body);
                        }
                        if (armorBuckets.TryGetValue(ItemObject.ItemTypeEnum.LegArmor, out var legBucket)) {
                            UpgradeHeroArmor(hero, party.ItemRoster, legBucket, EquipmentIndex.Leg);
                        }
                        if (armorBuckets.TryGetValue(ItemObject.ItemTypeEnum.HandArmor, out var handBucket)) {
                            UpgradeHeroArmor(hero, party.ItemRoster, handBucket, EquipmentIndex.Gloves);
                        }
                        if (armorBuckets.TryGetValue(ItemObject.ItemTypeEnum.Cape, out var capeBucket)) {
                            UpgradeHeroArmor(hero, party.ItemRoster, capeBucket, EquipmentIndex.Cape);
                        }

                        if (!hero.BattleEquipment[EquipmentIndex.Horse].IsEmpty) {
                            if (armorBuckets.TryGetValue(ItemObject.ItemTypeEnum.Horse, out var horseBucket)) {
                                UpgradeHeroHorse(hero, party.ItemRoster, horseBucket);
                            }
                        }
                        if (!hero.BattleEquipment[EquipmentIndex.HorseHarness].IsEmpty) {
                            if (armorBuckets.TryGetValue(ItemObject.ItemTypeEnum.HorseHarness, out var horseArmorBucket)) {
                                UpgradeHeroHorseArmor(hero, party.ItemRoster, horseArmorBucket, EquipmentIndex.HorseHarness);
                            }
                        }

                        UpgradeHeroWeaponSlot(hero, party.ItemRoster, weaponBuckets, EquipmentIndex.Weapon0);
                        UpgradeHeroWeaponSlot(hero, party.ItemRoster, weaponBuckets, EquipmentIndex.Weapon1);
                        UpgradeHeroWeaponSlot(hero, party.ItemRoster, weaponBuckets, EquipmentIndex.Weapon2);
                        UpgradeHeroWeaponSlot(hero, party.ItemRoster, weaponBuckets, EquipmentIndex.Weapon3);
                    }
                }

            } catch (Exception e) {
                InformationManager.DisplayMessage(new InformationMessage($"Exception caught in UpgradeAllHeroes! {e.Message}.", Color.FromUint(0xFFFFFF00)));
            }
        }


        /// <summary>
        /// Inserts an equipment element into a sorted list maintaining descending score order.
        /// Uses binary search to find the insertion position, ensuring O(log n) search time.
        /// </summary>
        /// <param name="list">The sorted list to insert into.</param>
        /// <param name="element">The equipment element to insert.</param>
        /// <param name="getScore">The scoring function used to determine element value and maintain sort order.</param>
        private static void SortedInsert(List<EquipmentElement> list, EquipmentElement element, Func<EquipmentElement, float> getScore) {
            try {
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
            } catch (Exception e) {
                InformationManager.DisplayMessage(new InformationMessage($"Exception caught in SortedInsert! {e.Message}.", Color.FromUint(0xFFFFFF00)));
            }
        }


        /// <summary>
        /// Attempts to upgrade a hero's armor in a specific slot by comparing the currently equipped item against the best available option.
        /// </summary>
        /// <param name="hero">The hero whose armor will be upgraded.</param>
        /// <param name="inventory">The item roster to manage equipment changes.</param>
        /// <param name="sorted">The list of available armor items, pre-sorted by score in descending order.</param>
        /// <param name="slot">The equipment slot to potentially upgrade (Head, Body, Leg, Gloves, or Cape).</param>
        private static void UpgradeHeroArmor(Hero hero, ItemRoster inventory, List<EquipmentElement> sorted, EquipmentIndex slot) {
            try {
                if (0 < sorted.Count && sorted[0] is EquipmentElement top) {
                    EquipmentElement existing = hero.BattleEquipment[slot];
                    if (EquipmentScores.GetScoreFunc(top.Item.ItemType) is Func<EquipmentElement, float> score) {
                        if (score(existing) < score(top)) {
                            DoSwap(hero, top, inventory, sorted, slot);
                        }
                    }
                }
            } catch (Exception e) {
                InformationManager.DisplayMessage(new InformationMessage($"Exception caught in UpgradeHeroArmor! {e.Message}.", Color.FromUint(0xFFFFFF00)));
            }
        }


        /// <summary>
        /// Attempts to upgrade a hero's horse armor by finding the best-matching harness for their current mount family.
        /// Only upgrades if the family matches (horse harness for horses, camel harness for camels).
        /// </summary>
        /// <param name="hero">The hero whose horse armor will be upgraded.</param>
        /// <param name="inventory">The item roster to manage equipment changes.</param>
        /// <param name="sorted">The list of available horse armor items, pre-sorted by score in descending order.</param>
        /// <param name="slot">The equipment slot for horse armor (HorseHarness).</param>
        private static void UpgradeHeroHorseArmor(Hero hero, ItemRoster inventory, List<EquipmentElement> sorted, EquipmentIndex slot) {
            try {
                if (0 < sorted.Count) {
                    EquipmentElement existing = hero.BattleEquipment[slot];
                    MountFamily eFamily = GetMountFamily(existing);
                    if (EquipmentScores.GetScoreFunc(existing.Item.ItemType) is Func<EquipmentElement, float> score) {
                        for (int i = 0; i < sorted.Count; i++) {
                            if (sorted[i] is EquipmentElement element) {
                                if (score(existing) < score(element)) {
                                    if (eFamily == GetMountFamily(element)) {
                                        DoSwap(hero, element, inventory, sorted, slot);
                                        break;
                                    }
                                } else {
                                    break;
                                }
                            }
                        }
                    }
                }
            } catch (Exception e) {
                InformationManager.DisplayMessage(new InformationMessage($"Exception caught in UpgradeHeroHorseArmor! {e.Message}.", Color.FromUint(0xFFFFFF00)));
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
            try {
                if (0 < sorted.Count) {
                    EquipmentElement existing = hero.BattleEquipment[EquipmentIndex.Horse];
                    MountFamily eFamily = GetMountFamily(existing);
                    if (EquipmentScores.GetScoreFunc(ItemObject.ItemTypeEnum.Horse) is Func<EquipmentElement, float> getScore) {
                        float currentScore = getScore(existing);
                        for (int i = 0; i < sorted.Count; i++) {
                            if (sorted[i] is EquipmentElement element) {
                                if (currentScore < getScore(element)) {
                                    if (eFamily == GetMountFamily(element)) {
                                        if (element.Item.Difficulty <= hero.GetSkillValue(DefaultSkills.Riding)) {
                                            DoSwap(hero, element, inventory, sorted, EquipmentIndex.Horse);
                                            break;
                                        }
                                    }
                                } else {
                                    break;
                                }
                            }
                        }
                    }
                }
            } catch (Exception e) {
                InformationManager.DisplayMessage(new InformationMessage($"Exception caught in UpgradeHeroHorse! {e.Message}.", Color.FromUint(0xFFFFFF00)));
            }
        }


        /// <summary>
        /// Attempts to upgrade a hero's currently equipped weapon in a specific slot by finding a better option from an available list.
        /// Respects the hero's skill level requirement and weapon type matching.
        /// </summary>
        /// <param name="hero">The hero whose weapon will be upgraded.</param>
        /// <param name="inventory">The item roster to manage equipment changes.</param>
        /// <param name="buckets">The weapon buckets grouped by (ItemTypeEnum, WeaponClass) to find compatible weapons.</param>
        /// <param name="slot">The weapon slot to potentially upgrade (Weapon0, Weapon1, Weapon2, or Weapon3).</param>
        private void UpgradeHeroWeaponSlot(Hero hero, ItemRoster inventory, Dictionary<WeaponBucketKey, List<EquipmentElement>> buckets, EquipmentIndex slot) {
            try {
                EquipmentElement existing = hero.BattleEquipment[slot];
                if (existing.IsEmpty || existing.Item == null) {
                    return;
                }
                WeaponClass wc = existing.Item.WeaponComponent?.PrimaryWeapon?.WeaponClass ?? WeaponClass.Undefined;
                WeaponBucketKey key = new WeaponBucketKey(existing.Item.ItemType, wc);
                if (buckets.TryGetValue(key, out List<EquipmentElement> sorted)) {
                    if (0 < sorted.Count) {
                        if (existing.Item.ItemType == ItemObject.ItemTypeEnum.Bow) {
                            UpgradeHeroBow(hero, inventory, sorted, existing, slot);
                        } else {
                            UpgradeHeroWeaponSlot(hero, inventory, sorted, existing, slot);
                        }
                    }
                }
            } catch (Exception e) {
                InformationManager.DisplayMessage(new InformationMessage($"Exception caught in UpgradeHeroWeaponSlot! {e.Message}.", Color.FromUint(0xFFFFFF00)));
            }
        }


        /// <summary>
        /// Attempts to upgrade a hero's currently equipped weapon by finding a better option from a sorted list of the same weapon type.
        /// Verifies the hero meets the difficulty requirement before performing the upgrade.
        /// </summary>
        /// <param name="hero">The hero whose weapon will be upgraded.</param>
        /// <param name="inventory">The item roster to manage equipment changes.</param>
        /// <param name="sorted">The list of available weapons of the same type, pre-sorted by score in descending order.</param>
        /// <param name="existing">The hero's currently equipped weapon in this slot.</param>
        /// <param name="slot">The weapon slot to potentially upgrade.</param>
        private void UpgradeHeroWeaponSlot(Hero hero, ItemRoster inventory, List<EquipmentElement> sorted, EquipmentElement existing, EquipmentIndex slot) {
            try {
                if (EquipmentScores.GetScoreFunc(existing.Item.ItemType) is Func<EquipmentElement, float> getScore) {
                    int heroSkill = hero.GetSkillValue(existing.Item.RelevantSkill);
                    float eScore = getScore(existing);
                    for (int i = 0; i < sorted.Count; ++i) {
                        EquipmentElement top = sorted[i];
                        if (eScore < getScore(top)) {
                            if (top.Item.Difficulty <= heroSkill) {
                                DoSwap(hero, top, inventory, sorted, slot);
                                return;
                            }
                        } else {
                            break;
                        }
                    }
                }
            } catch (Exception e) {
                InformationManager.DisplayMessage(new InformationMessage($"Exception caught in UpgradeHeroWeaponSlot! {e.Message}.", Color.FromUint(0xFFFFFF00)));
            }
        }


        /// <summary>
        /// Attempts to upgrade a hero's bow with special logic that prevents equipping longbows while mounted (unless the hero has HorseMaster perk).
        /// </summary>
        /// <param name="hero">The hero whose bow will be upgraded.</param>
        /// <param name="inventory">The item roster to manage equipment changes.</param>
        /// <param name="sorted">The list of available bows, pre-sorted by score in descending order.</param>
        /// <param name="existing">The hero's currently equipped bow.</param>
        /// <param name="slot">The weapon slot where the bow is equipped.</param>
        private void UpgradeHeroBow(Hero hero, ItemRoster inventory, List<EquipmentElement> sorted, EquipmentElement existing, EquipmentIndex slot) {
            try {
                int heroSkill = hero.GetSkillValue(DefaultSkills.Bow);
                bool isMounted = !hero.BattleEquipment[EquipmentIndex.Horse].IsEmpty;
                bool horseMaster = hero.GetPerkValue(DefaultPerks.Bow.HorseMaster);
                float eScore = EquipmentScores.CalcBowScore(existing);
                for (int i = 0; i < sorted.Count; ++i) {
                    EquipmentElement top = sorted[i];
                    if (eScore < EquipmentScores.CalcBowScore(top)) {
                        if (top.Item.Difficulty <= heroSkill) {
                            bool isLongbow = top.Item.WeaponComponent.PrimaryWeapon.ItemUsage.Contains("long");
                            if (!isMounted || (!isLongbow || horseMaster)) {
                                DoSwap(hero, top, inventory, sorted, slot);
                                return;
                            }
                        }
                    } else {
                        break;
                    }
                }
            } catch (Exception e) {
                InformationManager.DisplayMessage(new InformationMessage($"Exception caught in UpgradeHeroBow! {e.Message}.", Color.FromUint(0xFFFFFF00)));
            }
        }


        /// <summary>
        /// Performs an equipment swap by equipping a new item and returning the old item to inventory and sorted list.
        /// Displays appropriate feedback messages to the player about the equipment change.
        /// </summary>
        /// <param name="hero">The hero whose equipment will be swapped.</param>
        /// <param name="element">The new equipment element to equip.</param>
        /// <param name="inventory">The item roster to manage equipment changes.</param>
        /// <param name="sorted">The sorted list to remove the new item from and re-insert the old item into.</param>
        /// <param name="slot">The equipment slot being swapped.</param>
        private static void DoSwap(Hero hero, EquipmentElement element, ItemRoster inventory, List<EquipmentElement> sorted, EquipmentIndex slot) {
            try {
                EquipmentElement existing = hero.BattleEquipment[slot];
                if (existing.IsEmpty) {
                    string name = element.GetModifiedItemName()?.ToString();
                    InformationManager.DisplayMessage(new InformationMessage($"{hero.Name} equipped {name}.", Color.FromUint(0xFFFFFF00)));
                } else {
                    string oldName = existing.GetModifiedItemName()?.ToString();
                    string newName = element.GetModifiedItemName()?.ToString();
                    InformationManager.DisplayMessage(new InformationMessage($"{hero.Name} replaced {oldName} with {newName}.", Color.FromUint(0xFFFFFF00)));
                }
                hero.BattleEquipment[slot] = element;
                int index = inventory.AddToCounts(element, -1);
                if (index < 0 || inventory.GetElementNumber(index) < 1 || !inventory.GetElementCopyAtIndex(index).EquipmentElement.IsEqualTo(element)) {
                    int idx = sorted.IndexOf(element);
                    if (-1 < idx) {
                        sorted.RemoveAt(idx);
                    }
                }

                if (!existing.IsEmpty && existing.Item != null) {
                    inventory.AddToCounts(existing, 1);
                    SortedInsert(sorted, existing, EquipmentScores.GetScoreFunc(existing.Item.ItemType));
                }
            } catch (Exception e) {
                InformationManager.DisplayMessage(new InformationMessage($"Exception caught in DoSwap! {e.Message}.", Color.FromUint(0xFFFFFF00)));
            }
        }


        /// <summary>
        /// Synchronizes data with the specified data store. This implementation performs no operation.
        /// </summary>
        /// <remarks>Override this method in a derived class to provide custom synchronization logic.</remarks>
        /// <param name="dataStore">The data store to synchronize with. This parameter is ignored in this implementation.</param>
        public override void SyncData(IDataStore dataStore) {
            EquipmentScores.LoadWeightsFromJson();
        }
    }
}
