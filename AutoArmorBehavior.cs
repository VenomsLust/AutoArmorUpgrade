using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
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

        private Dictionary<ItemObject.ItemTypeEnum, List<EquipmentElement>> sortedArmor = new Dictionary<ItemObject.ItemTypeEnum, List<EquipmentElement>>();

        private List<EquipmentElement> sortedHorseArmor = new List<EquipmentElement>();

        private List<EquipmentElement> sortedHorses = new List<EquipmentElement>();


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
        /// Calculates the armor score for the specified equipment element based on its protective values and weight.
        /// </summary>
        /// <remarks>The armor score is determined by summing the modified armor values for the head,
        /// body, legs, and arms, then subtracting half the element's weight. This provides a balance between protection
        /// and encumbrance.</remarks>
        /// <param name="element">The equipment element for which to calculate the armor score. Must not be empty.</param>
        /// <returns>A floating-point value representing the calculated armor score. Returns -1 if the equipment element is
        /// empty.</returns>
        public float CalculateArmorScore(EquipmentElement element) {
            if (element.IsEmpty || element.Item == null) {
                return -1f;
            }

            return element.GetModifiedHeadArmor() + element.GetModifiedBodyArmor() + element.GetModifiedLegArmor() + element.GetModifiedArmArmor() - (element.Weight * 0.5f);
        }


        /// <summary>
        /// Calculates the armor score for a horse based on the specified equipment element.
        /// </summary>
        /// <param name="element">The equipment element representing the horse armor to evaluate. Must not be empty.</param>
        /// <returns>A floating-point value representing the modified body armor score of the horse. Returns -1 if the equipment
        /// element is empty.</returns>
        public float CalculateHorseArmorScore(EquipmentElement element) {
            if (element.IsEmpty) {
                return -1f;
            }
            return element.GetModifiedMountBodyArmor();
        }


        /// <summary>
        /// Calculates a composite score for a mount based on its speed, maneuverability, and charge damage attributes.
        /// </summary>
        /// <remarks>The score is determined by weighting the mount's speed, maneuverability, and charge
        /// damage. This method is intended for use in scenarios where mounts are compared or ranked based on their
        /// performance attributes.</remarks>
        /// <param name="element">The equipment element representing the mount to evaluate. Must contain a valid item with a horse component.</param>
        /// <returns>A floating-point value representing the calculated mount score. Returns -1 if the element is empty or does
        /// not contain a valid horse component.</returns>
        public float CalculateMountScore(EquipmentElement element) {
            if (element.Item?.HorseComponent is HorseComponent horse) {
                return horse.Speed * 10.0f + horse.Maneuver * 0.10f + horse.ChargeDamage * 0.10f;
            }

            return -1f;
        }


        /// <summary>
        /// Determines the item type associated with a specified equipment slot.
        /// </summary>
        /// <param name="slot">The equipment slot for which to retrieve the corresponding item type.</param>
        /// <returns>The item type that corresponds to the specified equipment slot.</returns>
        /// <exception cref="ArgumentException">Thrown if the specified slot does not correspond to a valid equipment slot.</exception>
        private ItemObject.ItemTypeEnum GetItemTypeForSlot(EquipmentIndex slot) {
            switch (slot) {
                case EquipmentIndex.Head:
                    return ItemObject.ItemTypeEnum.HeadArmor;
                case EquipmentIndex.Body:
                    return ItemObject.ItemTypeEnum.BodyArmor;
                case EquipmentIndex.Leg:
                    return ItemObject.ItemTypeEnum.LegArmor;
                case EquipmentIndex.Gloves:
                    return ItemObject.ItemTypeEnum.HandArmor;
                case EquipmentIndex.Cape:
                    return ItemObject.ItemTypeEnum.Cape;
                case EquipmentIndex.HorseHarness:
                    return ItemObject.ItemTypeEnum.HorseHarness;
                case EquipmentIndex.Horse:
                    return ItemObject.ItemTypeEnum.Horse;
                default:
                    throw new ArgumentException("Invalid equipment slot: " + slot);
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

            // First, we loop through the loot and organize all armor pieces by their type (head, body, leg, gloves, cape)
            foreach (ItemRosterElement element in party.ItemRoster) {
                if (0 < element.Amount && element.EquipmentElement.Item is ItemObject item) {
                    ItemObject.ItemTypeEnum type = item.ItemType;
                    if (type == ItemObject.ItemTypeEnum.Invalid) {
                        continue; // Skip items that don't have a valid type
                    }
                    switch (type) {
                        case ItemObject.ItemTypeEnum.Horse:
                            sortedHorses.Add(element.EquipmentElement);
                            continue;
                        case ItemObject.ItemTypeEnum.OneHandedWeapon:
                            continue;
                        case ItemObject.ItemTypeEnum.TwoHandedWeapon:
                            continue;
                        case ItemObject.ItemTypeEnum.Polearm:
                            continue;
                        case ItemObject.ItemTypeEnum.Arrows:
                            continue;
                        case ItemObject.ItemTypeEnum.Bolts:
                            continue;
                        case ItemObject.ItemTypeEnum.SlingStones:
                            continue;
                        case ItemObject.ItemTypeEnum.Shield:
                            continue;
                        case ItemObject.ItemTypeEnum.Bow:
                            continue;
                        case ItemObject.ItemTypeEnum.Crossbow:
                            continue;
                        case ItemObject.ItemTypeEnum.Sling:
                            continue;
                        case ItemObject.ItemTypeEnum.Thrown:
                            continue;
                        case ItemObject.ItemTypeEnum.HeadArmor:
                        case ItemObject.ItemTypeEnum.BodyArmor:
                        case ItemObject.ItemTypeEnum.LegArmor:
                        case ItemObject.ItemTypeEnum.HandArmor:
                        case ItemObject.ItemTypeEnum.Cape:
                            if (!sortedArmor.ContainsKey(type)) {
                                sortedArmor[type] = new List<EquipmentElement>();
                            }
                            sortedArmor[type].Add(element.EquipmentElement);
                            continue;
                        case ItemObject.ItemTypeEnum.HorseHarness:
                            sortedHorseArmor.Add(element.EquipmentElement);
                            continue;
                        default:
                            continue; // Not an armor piece we care about
                    }
                }
            }

            foreach (List<EquipmentElement> list in sortedArmor.Values) {
                if (1 < list.Count) {
                    list.Sort((a, b) => CalculateArmorScore(b).CompareTo(CalculateArmorScore(a)));
                }
            }

            if (1 < sortedHorseArmor.Count) {
                sortedHorseArmor.Sort((a, b) => CalculateHorseArmorScore(b).CompareTo(CalculateHorseArmorScore(a)));
            }

            if (1 < sortedHorses.Count) {
                sortedHorses.Sort((a, b) => CalculateMountScore(b).CompareTo(CalculateMountScore(a)));
            }

            for (int i = 0; i < party.MemberRoster.Count; i++) {
                TroopRosterElement member = party.MemberRoster.GetElementCopyAtIndex(i);
                if (member.Character?.HeroObject is Hero hero && hero.IsAlive) {
                    UpgradeHeroArmor(hero, party.ItemRoster, EquipmentIndex.Head);
                    UpgradeHeroArmor(hero, party.ItemRoster, EquipmentIndex.Body);
                    UpgradeHeroArmor(hero, party.ItemRoster, EquipmentIndex.Leg);
                    UpgradeHeroArmor(hero, party.ItemRoster, EquipmentIndex.Gloves);
                    UpgradeHeroArmor(hero, party.ItemRoster, EquipmentIndex.Cape);
                    if (!hero.BattleEquipment[EquipmentIndex.Horse].IsEmpty) {
                        UpgradeHeroHorseArmor(hero, party.ItemRoster);
                        UpgradeHeroHorse(hero, party.ItemRoster);
                    }
                }
            }

            sortedArmor.Clear();
            sortedHorseArmor.Clear();
            sortedHorses.Clear();
        }


        /// <summary>
        /// Upgrades the specified hero's armor in the given equipment slot by replacing it with the highest-scoring
        /// available armor from the roster.
        /// </summary>
        /// <remarks>If a better armor item is available in the roster for the specified slot, it will be
        /// equipped on the hero and the previous armor will be returned to the roster. The method does not perform any
        /// upgrade if no suitable armor is found.</remarks>
        /// <param name="hero">The hero whose armor will be upgraded. Cannot be null.</param>
        /// <param name="itemRoster">The item roster containing available armor items for swapping. Cannot be null.</param>
        /// <param name="slot">The equipment slot to upgrade, indicating which piece of armor will be replaced.</param>
        private void UpgradeHeroArmor(Hero hero, ItemRoster itemRoster, EquipmentIndex slot) {
            ItemObject.ItemTypeEnum targetType = GetItemTypeForSlot(slot);
            if (sortedArmor.ContainsKey(targetType) && sortedArmor[targetType] is List<EquipmentElement> list) {
                if (0 < list.Count && list[0] is EquipmentElement element) {
                    if (CalculateArmorScore(hero.BattleEquipment[slot]) < CalculateArmorScore(element)) {
                        int index = -1;
                        EquipmentElement old = ApplySwap(hero, slot, element, itemRoster, out index);
                        if (index < 0 || itemRoster.GetElementNumber(index) < 1 || !itemRoster.GetElementCopyAtIndex(index).EquipmentElement.IsEqualTo(element)) {
                            list.RemoveAt(0);
                        }
                        if (!old.IsEmpty) {
                            list.Add(old);
                            list.Sort((a, b) => CalculateArmorScore(b).CompareTo(CalculateArmorScore(a)));
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Upgrades the horse armor equipped by the specified hero using the highest-scoring available armor from the
        /// sorted list.
        /// </summary>
        /// <remarks>If the hero's current horse armor is replaced, the previous armor is returned to the
        /// sorted list and re-sorted by score. This method does not perform any upgrade if no suitable horse armor is
        /// available.</remarks>
        /// <param name="hero">The hero whose horse armor will be upgraded. Cannot be null.</param>
        /// <param name="itemRoster">The item roster containing available equipment for swapping horse armor. Cannot be null.</param>
        private void UpgradeHeroHorseArmor(Hero hero, ItemRoster itemRoster) {
            if (0 < sortedHorseArmor.Count && sortedHorseArmor[0] is EquipmentElement element) {
                if (CalculateHorseArmorScore(hero.BattleEquipment[EquipmentIndex.HorseHarness]) < CalculateHorseArmorScore(element)) {
                    int index = -1;
                    EquipmentElement old = ApplySwap(hero, EquipmentIndex.HorseHarness, element, itemRoster, out index);
                    if (index < 0 || itemRoster.GetElementNumber(index) < 1 || !itemRoster.GetElementCopyAtIndex(index).EquipmentElement.IsEqualTo(element)) {
                        sortedHorseArmor.RemoveAt(0);
                    }
                    if (!old.IsEmpty) {
                        sortedHorseArmor.Add(old);
                        sortedHorseArmor.Sort((a, b) => CalculateHorseArmorScore(b).CompareTo(CalculateHorseArmorScore(a)));
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
        /// <param name="itemRoster">The item roster used to manage equipment changes during the upgrade process. Cannot be null.</param>
        private void UpgradeHeroHorse(Hero hero, ItemRoster itemRoster) {
            float currentScore = CalculateMountScore(hero.BattleEquipment[EquipmentIndex.Horse]);
            for (int i = 0; i < sortedHorses.Count; i++) {
                if (sortedHorses[i] is EquipmentElement element) {
                    if (currentScore < CalculateMountScore(element)) {
                        if (element.Item.Difficulty <= hero.GetSkillValue(DefaultSkills.Riding)) {
                            int index = -1;
                            EquipmentElement old = ApplySwap(hero, EquipmentIndex.Horse, element, itemRoster, out index);
                            if (index < 0 || itemRoster.GetElementNumber(index) < 1 || !itemRoster.GetElementCopyAtIndex(index).EquipmentElement.IsEqualTo(element)) {
                                sortedHorses.RemoveAt(i);
                            }
                            if (!old.IsEmpty) {
                                sortedHorses.Add(old);
                                sortedHorses.Sort((a, b) => CalculateMountScore(b).CompareTo(CalculateMountScore(a)));
                            }
                            break;
                        }
                    } else {
                        break;
                    }
                }
            }
        }


        /// <summary>
        /// Swaps the specified equipment item into the given slot of the hero's battle equipment and updates the item
        /// roster accordingly.
        /// </summary>
        /// <remarks>If the slot previously contained an item, that item is returned to the item roster.
        /// The item roster is decremented for the new equipment and incremented for the old equipment, if
        /// applicable.</remarks>
        /// <param name="hero">The hero whose equipment will be modified. Cannot be null.</param>
        /// <param name="slot">The equipment slot to update with the new equipment item.</param>
        /// <param name="newEquip">The new equipment element to place in the specified slot. Must reference a valid item present in the item
        /// roster.</param>
        /// <param name="itemRoster">The item roster to update when removing the new equipment and returning the old equipment, if any.</param>
        /// <returns>The equipment element that was previously in the specified slot. Returns an empty element if the slot was
        /// empty.</returns>
        private EquipmentElement ApplySwap(Hero hero, EquipmentIndex slot, EquipmentElement newEquip, ItemRoster itemRoster, out int index) {
            EquipmentElement oldEquip = hero.BattleEquipment[slot];
            hero.BattleEquipment[slot] = newEquip;
            index = itemRoster.AddToCounts(newEquip, -1);
            if (!oldEquip.IsEmpty) {
                itemRoster.AddToCounts(oldEquip, 1);
                InformationManager.DisplayMessage(new InformationMessage($"{hero.Name} replaced {oldEquip.GetModifiedItemName()} with {newEquip.GetModifiedItemName()}.", Color.FromUint(0xFFFFFF00)));
            } else {
                InformationManager.DisplayMessage(new InformationMessage($"{hero.Name} equipped {newEquip.GetModifiedItemName()}.", Color.FromUint(0xFFFFFF00)));
            }

            return oldEquip;
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
