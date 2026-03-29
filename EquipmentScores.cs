using System;
using System.Runtime.CompilerServices;
using TaleWorlds.Core;

using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using TaleWorlds.Library;

namespace AutoArmorUpgrade {
    internal static class EquipmentScores {

        public static float WGT_HEAD_ARMOR = 1.0f;
        public static float WGT_CAPE_ARMOR = 1.0f;
        public static float WGT_BODY_ARMOR = 1.0f;
        public static float WGT_HAND_ARMOR = 1.0f;
        public static float WGT_ARM_ARMOR = 1.0f;
        public static float WGT_LEG_ARMOR = 1.0f;
        public static float WGT_ARMOR_WGT = 0.5f;
        public static float WGT_MOUNT_ARMOR_WGT = 0.01f;
        public static float WGT_MOUNT_SPD = 3.0f;
        public static float WGT_MOUNT_MNV = 0.5f;
        public static float WGT_MOUNT_CHG = 0.05f;
        public static float WGT_MOUNT_HP = 0.1f;
        public static float WGT_MOUNT_ARM = 1.0f;
        public static float WGT_SHIELD_HP = 2.0f;
        public static float WGT_SHIELD_SZ = 1.0f;
        public static float WGT_SHIELD_SPD = 0.75f;
        public static float WGT_1H_SLASH_DMG = 2.25f;
        public static float WGT_1H_THRUST_DMG = 0.75f;
        public static float WGT_1H_LENGTH = 1.50f;
        public static float WGT_1H_HANDLE = 1.00f;
        public static float WGT_1H_WGT = 2.00f;
        public static float WGT_2H_SLASH_DMG = 2.25f;
        public static float WGT_2H_THRUST_DMG = 0.75f;
        public static float WGT_2H_LENGTH = 1.50f;
        public static float WGT_2H_HANDLE = 1.00f;
        public static float WGT_2H_WGT = 2.00f;
        public static float WGT_POLE_SWING_DMG = 2.00f;
        public static float WGT_POLE_THRUST_DMG = 2.00f;
        public static float WGT_POLE_LENGTH = 0.50f;
        public static float WGT_POLE_HANDLE = 1.00f;
        public static float WGT_POLE_WGT = 1.00f;
        public static float WGT_BOW_DMG = 1.0f;
        public static float WGT_BOW_ACC = 0.02f;
        public static float WGT_BOW_HANDLE = 0.01f;
        public static float WGT_XBOW_DMG = 1.0f;
        public static float WGT_XBOW_ACC = 0.02f;
        public static float WGT_XBOW_HANDLE = 0.01f;
        public static float WGT_ARROW_DMG = 1.0f;
        public static float WGT_BOLT_DMG = 1.0f;

        private const string WeightsFileName = "weights.json";

        [DataContract]
        private sealed class WeightConfig {
            [DataMember] public float? WGT_HEAD_ARMOR { get; set; }
            [DataMember] public float? WGT_CAPE_ARMOR { get; set; }
            [DataMember] public float? WGT_BODY_ARMOR { get; set; }
            [DataMember] public float? WGT_HAND_ARMOR { get; set; }
            [DataMember] public float? WGT_ARM_ARMOR { get; set; }
            [DataMember] public float? WGT_LEG_ARMOR { get; set; }
            [DataMember] public float? WGT_ARMOR_WGT { get; set; }
            [DataMember] public float? WGT_MOUNT_ARMOR_WGT { get; set; }
            [DataMember] public float? WGT_MOUNT_SPD { get; set; }
            [DataMember] public float? WGT_MOUNT_MNV { get; set; }
            [DataMember] public float? WGT_MOUNT_CHG { get; set; }
            [DataMember] public float? WGT_MOUNT_HP { get; set; }
            [DataMember] public float? WGT_MOUNT_ARM { get; set; }
            [DataMember] public float? WGT_SHIELD_HP { get; set; }
            [DataMember] public float? WGT_SHIELD_SZ { get; set; }
            [DataMember] public float? WGT_SHIELD_SPD { get; set; }
            [DataMember] public float? WGT_1H_SLASH_DMG { get; set; }
            [DataMember] public float? WGT_1H_THRUST_DMG { get; set; }
            [DataMember] public float? WGT_1H_LENGTH { get; set; }
            [DataMember] public float? WGT_1H_HANDLE { get; set; }
            [DataMember] public float? WGT_1H_WGT { get; set; }
            [DataMember] public float? WGT_2H_SLASH_DMG { get; set; }
            [DataMember] public float? WGT_2H_THRUST_DMG { get; set; }
            [DataMember] public float? WGT_2H_LENGTH { get; set; }
            [DataMember] public float? WGT_2H_HANDLE { get; set; }
            [DataMember] public float? WGT_2H_WGT { get; set; }
            [DataMember] public float? WGT_POLE_SWING_DMG { get; set; }
            [DataMember] public float? WGT_POLE_THRUST_DMG { get; set; }
            [DataMember] public float? WGT_POLE_LENGTH { get; set; }
            [DataMember] public float? WGT_POLE_HANDLE { get; set; }
            [DataMember] public float? WGT_POLE_WGT { get; set; }
            [DataMember] public float? WGT_BOW_DMG { get; set; }
            [DataMember] public float? WGT_BOW_ACC { get; set; }
            [DataMember] public float? WGT_BOW_HANDLE { get; set; }
            [DataMember] public float? WGT_XBOW_DMG { get; set; }
            [DataMember] public float? WGT_XBOW_ACC { get; set; }
            [DataMember] public float? WGT_XBOW_HANDLE { get; set; }
            [DataMember] public float? WGT_ARROW_DMG { get; set; }
            [DataMember] public float? WGT_BOLT_DMG { get; set; }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Apply(ref float field, float? value) {
            if (value.HasValue) {
                field = value.Value;
            }
        }


        private static string GetWeightsPath() {
            string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (!string.IsNullOrEmpty(assemblyDirectory)) {
                DirectoryInfo moduleDirectory = Directory.GetParent(assemblyDirectory)?.Parent?.Parent;
                if (moduleDirectory != null) {
                    return Path.Combine(moduleDirectory.FullName, WeightsFileName);
                }
            }

            return Path.Combine(BasePath.Name, "Modules", "AutoArmorUpgrade", WeightsFileName);
        }


        public static void LoadWeightsFromJson() {
            try {
                string weightsPath = GetWeightsPath();
                if (!File.Exists(weightsPath)) {
                    return;
                }

                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(WeightConfig));
                using (FileStream stream = File.OpenRead(weightsPath)) {
                    if (!(serializer.ReadObject(stream) is WeightConfig config)) {
                        return;
                    }

                    Apply(ref WGT_HEAD_ARMOR, config.WGT_HEAD_ARMOR);
                    Apply(ref WGT_CAPE_ARMOR, config.WGT_CAPE_ARMOR);
                    Apply(ref WGT_BODY_ARMOR, config.WGT_BODY_ARMOR);
                    Apply(ref WGT_HAND_ARMOR, config.WGT_HAND_ARMOR);
                    Apply(ref WGT_ARM_ARMOR, config.WGT_ARM_ARMOR);
                    Apply(ref WGT_LEG_ARMOR, config.WGT_LEG_ARMOR);
                    Apply(ref WGT_ARMOR_WGT, config.WGT_ARMOR_WGT);
                    Apply(ref WGT_MOUNT_ARMOR_WGT, config.WGT_MOUNT_ARMOR_WGT);
                    Apply(ref WGT_MOUNT_SPD, config.WGT_MOUNT_SPD);
                    Apply(ref WGT_MOUNT_MNV, config.WGT_MOUNT_MNV);
                    Apply(ref WGT_MOUNT_CHG, config.WGT_MOUNT_CHG);
                    Apply(ref WGT_MOUNT_HP, config.WGT_MOUNT_HP);
                    Apply(ref WGT_MOUNT_ARM, config.WGT_MOUNT_ARM);
                    Apply(ref WGT_SHIELD_HP, config.WGT_SHIELD_HP);
                    Apply(ref WGT_SHIELD_SZ, config.WGT_SHIELD_SZ);
                    Apply(ref WGT_SHIELD_SPD, config.WGT_SHIELD_SPD);
                    Apply(ref WGT_1H_SLASH_DMG, config.WGT_1H_SLASH_DMG);
                    Apply(ref WGT_1H_THRUST_DMG, config.WGT_1H_THRUST_DMG);
                    Apply(ref WGT_1H_LENGTH, config.WGT_1H_LENGTH);
                    Apply(ref WGT_1H_HANDLE, config.WGT_1H_HANDLE);
                    Apply(ref WGT_1H_WGT, config.WGT_1H_WGT);
                    Apply(ref WGT_2H_SLASH_DMG, config.WGT_2H_SLASH_DMG);
                    Apply(ref WGT_2H_THRUST_DMG, config.WGT_2H_THRUST_DMG);
                    Apply(ref WGT_2H_LENGTH, config.WGT_2H_LENGTH);
                    Apply(ref WGT_2H_HANDLE, config.WGT_2H_HANDLE);
                    Apply(ref WGT_2H_WGT, config.WGT_2H_WGT);
                    Apply(ref WGT_POLE_SWING_DMG, config.WGT_POLE_SWING_DMG);
                    Apply(ref WGT_POLE_THRUST_DMG, config.WGT_POLE_THRUST_DMG);
                    Apply(ref WGT_POLE_LENGTH, config.WGT_POLE_LENGTH);
                    Apply(ref WGT_POLE_HANDLE, config.WGT_POLE_HANDLE);
                    Apply(ref WGT_POLE_WGT, config.WGT_POLE_WGT);
                    Apply(ref WGT_BOW_DMG, config.WGT_BOW_DMG);
                    Apply(ref WGT_BOW_ACC, config.WGT_BOW_ACC);
                    Apply(ref WGT_BOW_HANDLE, config.WGT_BOW_HANDLE);
                    Apply(ref WGT_XBOW_DMG, config.WGT_XBOW_DMG);
                    Apply(ref WGT_XBOW_ACC, config.WGT_XBOW_ACC);
                    Apply(ref WGT_XBOW_HANDLE, config.WGT_XBOW_HANDLE);
                    Apply(ref WGT_ARROW_DMG, config.WGT_ARROW_DMG);
                    Apply(ref WGT_BOLT_DMG, config.WGT_BOLT_DMG);
                }
            } catch (Exception e) {
                InformationManager.DisplayMessage(new InformationMessage($"Exception caught loading weights.json! {e.Message}.", Color.FromUint(0xFFFFFF00)));
            }
        }


        /// <summary>
        /// Calculates a weighted score for head armor based on protection value and weight.
        /// </summary>
        /// <param name="element">The head armor equipment element to evaluate.</param>
        /// <returns>The calculated score, or -1 if the element is not head armor.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcHeadArmorScore(EquipmentElement element) {
            if (element.Item?.ItemType == ItemObject.ItemTypeEnum.HeadArmor) {
                float head = element.GetModifiedHeadArmor() * WGT_HEAD_ARMOR;
                float wgt = element.Weight * WGT_ARMOR_WGT;
                return head - wgt;
            }
            return -1f;
        }


        /// <summary>
        /// Calculates a weighted score for capes based on body and arm armor protection and weight.
        /// </summary>
        /// <param name="element">The cape equipment element to evaluate.</param>
        /// <returns>The calculated score, or -1 if the element is not a cape.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcCapeArmorScore(EquipmentElement element) {
            if (element.Item?.ItemType == ItemObject.ItemTypeEnum.Cape) {
                float body = element.GetModifiedBodyArmor() * WGT_BODY_ARMOR;
                float arm = element.GetModifiedArmArmor() * WGT_ARM_ARMOR;
                float wgt = element.Weight * WGT_ARMOR_WGT;
                return body + arm - wgt;
            }
            return -1f;
        }


        /// <summary>
        /// Calculates a weighted score for body armor based on body, arm, and leg protection and weight.
        /// </summary>
        /// <param name="element">The body armor equipment element to evaluate.</param>
        /// <returns>The calculated score, or -1 if the element is not body armor.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcBodyArmorScore(EquipmentElement element) {
            if (element.Item?.ItemType == ItemObject.ItemTypeEnum.BodyArmor) {
                float body = element.GetModifiedBodyArmor() * WGT_BODY_ARMOR;
                float arm = element.GetModifiedArmArmor() * WGT_ARM_ARMOR;
                float leg = element.GetModifiedLegArmor() * WGT_LEG_ARMOR;
                float wgt = element.Weight * WGT_ARMOR_WGT;
                return body + arm + leg - wgt;
            }
            return -1f;
        }


        /// <summary>
        /// Calculates a weighted score for hand armor based on arm protection and weight.
        /// </summary>
        /// <param name="element">The hand armor equipment element to evaluate.</param>
        /// <returns>The calculated score, or -1 if the element is not hand armor.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcHandArmorScore(EquipmentElement element) {
            if (element.Item?.ItemType == ItemObject.ItemTypeEnum.HandArmor) {
                float arm = element.GetModifiedArmArmor() * WGT_ARM_ARMOR;
                float wgt = element.Weight * WGT_ARMOR_WGT;
                return arm - wgt;
            }
            return -1f;
        }


        /// <summary>
        /// Calculates a weighted score for leg armor based on protection value and weight.
        /// </summary>
        /// <param name="element">The leg armor equipment element to evaluate.</param>
        /// <returns>The calculated score, or -1 if the element is not leg armor.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcLegArmorScore(EquipmentElement element) {
            if (element.Item?.ItemType == ItemObject.ItemTypeEnum.LegArmor) {
                float leg = element.GetModifiedLegArmor() * WGT_LEG_ARMOR;
                float wgt = element.Weight * WGT_ARMOR_WGT;
                return leg - wgt;
            }
            return -1f;
        }


        /// <summary>
        /// Calculates a weighted score for mounts based on speed, maneuverability, charge damage, and hit points.
        /// </summary>
        /// <param name="element">The mount equipment element to evaluate.</param>
        /// <returns>The calculated score, or -1 if the element is not a mount.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcMountScore(EquipmentElement element) {
            if (element.Item?.HorseComponent is HorseComponent horse) {
                float spd = horse.Speed * WGT_MOUNT_SPD;
                float mnv = horse.Maneuver * WGT_MOUNT_MNV;
                float chg = horse.ChargeDamage * WGT_MOUNT_CHG;
                float hp = horse.HitPoints * WGT_MOUNT_HP;
                return spd + mnv + chg + hp;
            }
            return -1f;
        }


        /// <summary>
        /// Calculates a weighted score for mount armor (horse harness) based on protection and weight.
        /// </summary>
        /// <param name="element">The mount armor equipment element to evaluate.</param>
        /// <returns>The calculated score, or -1 if the element is not mount armor.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcMountArmorScore(EquipmentElement element) {
            if (element.Item?.ItemType == ItemObject.ItemTypeEnum.HorseHarness) {
                float body = element.GetModifiedMountBodyArmor() * WGT_MOUNT_ARM;
                float wgt = element.Weight * WGT_MOUNT_ARMOR_WGT;
                return body - wgt;
            }
            return -1f;
        }


        /// <summary>
        /// Calculates a weighted score for shields based on hit points, handling speed, size, and weight.
        /// </summary>
        /// <param name="element">The shield equipment element to evaluate.</param>
        /// <returns>The calculated score, or -1 if the element is not a shield.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcShieldScore(EquipmentElement element) {
            if (element.Item?.WeaponComponent is WeaponComponent shield) {
                if (shield.PrimaryWeapon is WeaponComponentData shieldData && shieldData.IsShield) {
                    float hp = shieldData.MaxDataValue * WGT_SHIELD_HP;
                    float speed = shieldData.Handling * WGT_SHIELD_SPD;
                    float size = shieldData.BodyArmor * WGT_SHIELD_SZ;
                    float wgt = element.Weight * WGT_ARMOR_WGT;
                    return hp + speed + size - wgt;
                }
            }
            return -1f;
        }


        /// <summary>
        /// Calculates a weighted score for bows based on missile damage, speed, accuracy, and rate of fire.
        /// </summary>
        /// <param name="element">The bow equipment element to evaluate.</param>
        /// <returns>The calculated score, or -1 if the element is not a bow.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcBowScore(EquipmentElement element) {
            if (element.Item?.WeaponComponent is WeaponComponent bow) {
                if (bow.PrimaryWeapon is WeaponComponentData bowData) {
                    float dmg = element.GetModifiedMissileDamageForUsage(0) * WGT_BOW_DMG;
                    float spd = element.GetModifiedMissileSpeedForUsage(0) * 0.01f;
                    float acc = bowData.Accuracy * WGT_BOW_ACC;
                    float rof = bowData.Handling * WGT_BOW_HANDLE;
                    return dmg * (spd + acc + rof);
                }
            }
            return -1f;
        }


        /// <summary>
        /// Calculates a weighted score for crossbows based on missile damage, speed, accuracy, and rate of fire.
        /// </summary>
        /// <param name="element">The crossbow equipment element to evaluate.</param>
        /// <returns>The calculated score, or -1 if the element is not a crossbow.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcCrossbowScore(EquipmentElement element) {
            if (element.Item?.WeaponComponent is WeaponComponent bow) {
                if (bow.PrimaryWeapon is WeaponComponentData bowData) {
                    float dmg = element.GetModifiedMissileDamageForUsage(0) * WGT_XBOW_DMG;
                    float spd = element.GetModifiedMissileSpeedForUsage(0) * 0.01f;
                    float acc = bowData.Accuracy * WGT_XBOW_ACC;
                    float rof = bowData.Handling * WGT_XBOW_HANDLE;
                    return dmg * (spd + acc + rof);
                }
            }
            return -1f;
        }


        /// <summary>
        /// Calculates a weighted score for arrows based on missile damage and stack count.
        /// </summary>
        /// <param name="element">The arrow equipment element to evaluate.</param>
        /// <returns>The calculated score, or -1 if the element is not arrows.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcArrowScore(EquipmentElement element) {
            if (element.Item?.ItemType == ItemObject.ItemTypeEnum.Arrows && element.Item.WeaponComponent is WeaponComponent arrow) {
                if (arrow.PrimaryWeapon is WeaponComponentData arrowData) {
                    float dmg = Math.Max(0.1f, element.GetModifiedMissileDamageForUsage(0)) * WGT_ARROW_DMG;
                    return dmg * arrowData.MaxDataValue;
                }
            }
            return -1f;
        }


        /// <summary>
        /// Calculates a weighted score for bolts based on missile damage and stack count.
        /// </summary>
        /// <param name="element">The bolt equipment element to evaluate.</param>
        /// <returns>The calculated score, or -1 if the element is not bolts.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcBoltScore(EquipmentElement element) {
            if (element.Item?.ItemType == ItemObject.ItemTypeEnum.Bolts && element.Item.WeaponComponent is WeaponComponent bolts) {
                if (bolts.PrimaryWeapon is WeaponComponentData arrowData) {
                    float dmg = Math.Max(0.1f, element.GetModifiedMissileDamageForUsage(0)) * WGT_BOLT_DMG;
                    return dmg * arrowData.MaxDataValue;
                }
            }
            return -1f;
        }


        /// <summary>
        /// Calculates a weighted score for one-handed weapons based on swing and thrust damage, speed, reach, handling, and weight.
        /// </summary>
        /// <param name="element">The one-handed weapon equipment element to evaluate.</param>
        /// <returns>The calculated score, or -1 if the element is not a one-handed weapon.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcOneHandedScore(EquipmentElement element) {
            if (element.Item?.ItemType == ItemObject.ItemTypeEnum.OneHandedWeapon) {
                if (element.Item.WeaponComponent is WeaponComponent weapon) {
                    float thrustDmg = element.GetModifiedThrustDamageForUsage(0) * WGT_1H_THRUST_DMG;
                    float thrustSpd = element.GetModifiedThrustSpeedForUsage(0) * 0.01f;

                    float swingDmg = element.GetModifiedSwingDamageForUsage(0) * WGT_1H_SLASH_DMG;
                    float swingSpd = element.GetModifiedSwingSpeedForUsage(0) * 0.01f;

                    float reach = weapon.PrimaryWeapon.WeaponLength * WGT_1H_LENGTH;

                    float handling = element.GetModifiedHandlingForUsage(0) * WGT_1H_HANDLE;

                    float weight = element.Weight * WGT_1H_WGT;

                    return (swingDmg * swingSpd) + (thrustDmg * thrustSpd) + reach + handling - weight;
                }
            }
            return -1f;
        }


        /// <summary>
        /// Calculates a weighted score for two-handed weapons based on swing and thrust damage, speed, reach, handling, and weight.
        /// </summary>
        /// <param name="element">The two-handed weapon equipment element to evaluate.</param>
        /// <returns>The calculated score, or -1 if the element is not a two-handed weapon.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcTwoHandedScore(EquipmentElement element) {
            if (element.Item?.ItemType == ItemObject.ItemTypeEnum.TwoHandedWeapon) {
                if (element.Item.WeaponComponent is WeaponComponent weapon) {
                    float thrustDmg = element.GetModifiedThrustDamageForUsage(0) * WGT_2H_THRUST_DMG;
                    float thrustSpd = element.GetModifiedThrustSpeedForUsage(0) * 0.01f;

                    float swingDmg = element.GetModifiedSwingDamageForUsage(0) * WGT_2H_SLASH_DMG;
                    float swingSpd = element.GetModifiedSwingSpeedForUsage(0) * 0.01f;

                    float reach = weapon.PrimaryWeapon.WeaponLength * WGT_2H_LENGTH;

                    float handling = element.GetModifiedHandlingForUsage(0) * WGT_2H_HANDLE;

                    float weight = element.Weight * WGT_2H_WGT;

                    return (swingDmg * swingSpd) + (thrustDmg * thrustSpd) + reach + handling - weight;
                }
            }
            return -1f;
        }


        /// <summary>
        /// Calculates a weighted score for polearms based on swing and thrust damage, speed, reach, handling, and weight.
        /// </summary>
        /// <param name="element">The polearm equipment element to evaluate.</param>
        /// <returns>The calculated score, or -1 if the element is not a polearm.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcPolearmScore(EquipmentElement element) {
            if (element.Item?.ItemType == ItemObject.ItemTypeEnum.Polearm) {
                if (element.Item.WeaponComponent is WeaponComponent weapon) {
                    float thrustDmg = element.GetModifiedThrustDamageForUsage(0) * WGT_POLE_THRUST_DMG;
                    float thrustSpd = element.GetModifiedThrustSpeedForUsage(0) * 0.01f;

                    float swingDmg = element.GetModifiedSwingDamageForUsage(0) * WGT_POLE_SWING_DMG;
                    float swingSpd = element.GetModifiedSwingSpeedForUsage(0) * 0.01f;

                    float reach = weapon.PrimaryWeapon.WeaponLength * WGT_POLE_LENGTH;

                    float handling = element.GetModifiedHandlingForUsage(0) * WGT_POLE_HANDLE;

                    float weight = element.Weight * WGT_POLE_WGT;

                    return (swingDmg * swingSpd) + (thrustDmg * thrustSpd) + reach + handling - weight;
                }
            }
            return -1f;
        }


        /// <summary>
        /// Calculates a weighted score for thrown weapons based on missile damage, speed, and stack count.
        /// </summary>
        /// <param name="element">The thrown weapon equipment element to evaluate.</param>
        /// <returns>The calculated score, or -1 if the element is not a thrown weapon.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcThrownScore(EquipmentElement element) {
            if (element.Item?.ItemType == ItemObject.ItemTypeEnum.Thrown) {
                float dmg = element.GetModifiedMissileDamageForUsage(0);
                float spd = element.GetModifiedMissileSpeedForUsage(0) * 0.01f;
                float cnt = element.GetModifiedStackCountForUsage(0);
                return (dmg * spd) * cnt;
            }
            return -1f;
        }


        /// <summary>
        /// Returns true for item types that should be bucketed by WeaponClass sub-type
        /// (i.e. types where two items can share the same ItemTypeEnum but be fundamentally
        /// different weapon categories — sword vs axe, bow vs crossbow, etc.).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWeaponType(ItemObject.ItemTypeEnum type) {
            switch (type) {
                case ItemObject.ItemTypeEnum.OneHandedWeapon:
                case ItemObject.ItemTypeEnum.TwoHandedWeapon:
                case ItemObject.ItemTypeEnum.Polearm:
                case ItemObject.ItemTypeEnum.Bow:
                case ItemObject.ItemTypeEnum.Crossbow:
                case ItemObject.ItemTypeEnum.Thrown:
                case ItemObject.ItemTypeEnum.Shield:
                case ItemObject.ItemTypeEnum.Arrows:
                case ItemObject.ItemTypeEnum.Bolts:
                    return true;
                default:
                    return false;
            }
        }


        /// <summary>
        /// Lookup table mapping ItemTypeEnum values to their corresponding score calculation functions.
        /// Entries are indexed by (int)ItemTypeEnum and provide fast O(1) access to scoring functions.
        /// </summary>
        private static Func<EquipmentElement, float>[] funcs = {
            null,                // Invalid
            CalcMountScore,      // Horse
            CalcOneHandedScore,  // OneHandedWeapon
            CalcTwoHandedScore,  // TwoHandedWeapon
            CalcPolearmScore,    // Polearm,
            CalcArrowScore,      // Arrows,
            CalcBoltScore,       // Bolts,
            null,                // SlingStones,
            CalcShieldScore,     // Shield,
            CalcBowScore,        // Bow,
            CalcCrossbowScore,   // Crossbow,
            null,                // Sling,
            CalcThrownScore,     // Thrown,
            null,                // Goods,
            CalcHeadArmorScore,  // HeadArmor,
            CalcBodyArmorScore,  // BodyArmor,
            CalcLegArmorScore,   // LegArmor,
            CalcHandArmorScore,  // HandArmor,
            null,                // Pistol,
            null,                // Musket,
            null,                // Bullets,
            null,                // Animal,
            null,                // Book,
            CalcBodyArmorScore,  // ChestArmor,
            CalcCapeArmorScore,  // Cape,
            CalcMountArmorScore, // HorseHarness,
            null                 // Banner
        };


        /// <summary>
        /// Retrieves the score calculation function for a given item type.
        /// </summary>
        /// <param name="itemType">The item type to retrieve the scoring function for.</param>
        /// <returns>The scoring function delegate for the item type, or null if no scoring function exists for that type.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<EquipmentElement, float> GetScoreFunc(ItemObject.ItemTypeEnum itemType) {
            return funcs[(int)itemType];
        }
    }
}
