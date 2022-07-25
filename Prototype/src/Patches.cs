using HarmonyLib;
using OutwardModTemplate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace AvrixelPrototype
{
    [HarmonyPatch(typeof(ItemDisplayOptionPanel))]
    public static class ItemDisplayOptionPanelPatches
    {
        public static int CustomEquipActionID = 90909;

        public static string Equip_String = "Activate ";
        public static string UnEquip_String = "DeActivate";
        public static Color EquippableBorderColor = Color.cyan;

        [HarmonyPatch(nameof(ItemDisplayOptionPanel.GetActiveActions)), HarmonyPostfix]
        private static void EquipmentMenu_GetActiveActions_Postfix(ItemDisplayOptionPanel __instance, GameObject pointerPress, ref List<int> __result)
        {
            PlayerInventoryEquippableComponent equippableComponent = __instance.LocalCharacter.GetComponent<PlayerInventoryEquippableComponent>();
            Item CurrentItem = __instance.m_pendingItem;
            BaseInventoryEquippable baseInventoryEquippable = CurrentItem.GetComponent<BaseInventoryEquippable>();
            if (equippableComponent != null && baseInventoryEquippable != null)
            {
                __result.Add(CustomEquipActionID);
            }
       
        }



        [HarmonyPatch(nameof(ItemDisplayOptionPanel.ActionHasBeenPressed)), HarmonyPrefix]
        private static void EquipmentMenu_ActionHasBeenPressed_Prefix(ItemDisplayOptionPanel __instance, int _actionID)
        {
            if (_actionID == CustomEquipActionID)
            {
                Character owner = __instance.m_characterUI.TargetCharacter;
                PlayerInventoryEquippableComponent equippableComponent = owner.GetComponent<PlayerInventoryEquippableComponent>();
                Item CurrentItem = __instance.m_pendingItem;

                //If there's no artifact currently equipped and the artifact component exists
                if (equippableComponent != null && !equippableComponent.HasEquipped)
                {
                    //equip the current item in the artifact slot
                    equippableComponent.Equip(CurrentItem);

                    if (!Helpers.HasButtonHighLight(CurrentItem.UID))
                    {
                        Helpers.CreateButtonHighlight(CurrentItem.UID, CurrentItem.m_refItemDisplay, EquippableBorderColor, new Vector2(5, 5), __instance.m_activatedItemDisplay.transform);
                    }

                }
                else if (equippableComponent != null && equippableComponent.HasEquipped && CurrentItem == equippableComponent.EquippedItem)
                {
                    if (Helpers.HasButtonHighLight(equippableComponent.EquippedItem.UID))
                    {
                       Helpers.DestroyButtonHighLight(equippableComponent.EquippedItem.UID);
                    }

                    equippableComponent.UnEquip();
                }
                else if (equippableComponent != null && equippableComponent.HasEquipped && CurrentItem != equippableComponent.EquippedItem)
                {
                    //remove
                    if (Helpers.HasButtonHighLight(equippableComponent.EquippedItem.UID))
                    {
                        Helpers.DestroyButtonHighLight(equippableComponent.EquippedItem.UID);
                    }
                    equippableComponent.UnEquip();

                    //add
                    equippableComponent.Equip(CurrentItem);

                    if (!Helpers.HasButtonHighLight(CurrentItem.UID))
                    {
                        Helpers.CreateButtonHighlight(CurrentItem.UID, CurrentItem.m_refItemDisplay, EquippableBorderColor, new Vector2(5, 5), __instance.m_activatedItemDisplay.transform);
                    }

                }
            }
        }


        [HarmonyPatch(nameof(ItemDisplayOptionPanel.GetActionText)), HarmonyPrefix]
        private static bool EquipmentMenu_GetActionText_Prefix(ItemDisplayOptionPanel __instance, int _actionID, ref string __result)
        {
            if (_actionID == CustomEquipActionID)
            {
                Character owner = __instance.m_characterUI.TargetCharacter;
                PlayerInventoryEquippableComponent equippableComponent = owner.GetComponent<PlayerInventoryEquippableComponent>();
                Item CurrentItem = __instance.m_pendingItem;

                if (equippableComponent != null && !equippableComponent.HasEquipped)
                {
                    __result = Equip_String;

                }
                else if (equippableComponent != null && equippableComponent.HasEquipped && CurrentItem == equippableComponent.EquippedItem)
                {
                    __result = UnEquip_String;
                }
                else if (equippableComponent != null && equippableComponent.HasEquipped && CurrentItem != equippableComponent.EquippedItem)
                {
                    __result = $"Switch";
                }

                return false;
            }

            return true;
        }
    }
    [HarmonyPatch(nameof(ItemContainer.RemoveItem))]
    public class ItemContainerRemoveItem
    {
        static void ItemContainerRemoveItem_Prefix(ItemContainer __instance, Item _item)
        {
            PlayerInventoryEquippableComponent equippableComponent = __instance.OwnerCharacter.gameObject.GetComponent<PlayerInventoryEquippableComponent>();

            if (equippableComponent != null && equippableComponent.HasEquipped && equippableComponent.EquippedItem == _item)
            {
                equippableComponent.UnEquip();
            }
        }
    }

    [HarmonyPatch(nameof(CharacterInventory.NotifyItemRemoved))]
    public class CharacterInventoryNotifyItemRemoved
    {
        static void CharacterInventory_RemoveItem_Prefix(CharacterInventory __instance, Item _item, int _quantity, bool _playSound)
        {
            PlayerInventoryEquippableComponent equippableComponent = __instance.GetComponent<PlayerInventoryEquippableComponent>();

            if (equippableComponent != null && equippableComponent.HasEquipped && equippableComponent.EquippedItem == _item)
            {
                equippableComponent.UnEquip();
            }
        }
    }

    [HarmonyPatch(nameof(CharacterInventory.NotifyItemRemoved))]
    public class CharacterInventoryDropItem
    {
        static void CharacterInventory_DropItem_Prefix(CharacterInventory __instance, Item _item, Transform _newParent = null, bool _playAnim = true)
        {
            PlayerInventoryEquippableComponent equippableComponent = __instance.GetComponent<PlayerInventoryEquippableComponent>();

            if (equippableComponent != null && equippableComponent.HasEquipped && equippableComponent.EquippedItem == _item)
            {
                equippableComponent.UnEquip();
            }
        }
    }
    [HarmonyPatch(typeof(Character), nameof(Character.Awake))]
    public class CharacterAwakePatch
    {
        static void Postfix(Character __instance)
        {
            __instance.gameObject.AddComponent<PlayerInventoryEquippableComponent>();
        }
    }

    [HarmonyPatch(nameof(StatusEffectManager.AddStatusEffect))]
    public class StatusEffectManagerOnStatusAdded
    {
        static void StatusEffectManager_OnStatusAddeddPrefix(StatusEffectManager __instance, string _statusPrefabName, string[] _splitData)
        {
            PlayerInventoryEquippableComponent equippableComponent = __instance.m_character.GetComponent<PlayerInventoryEquippableComponent>();

            if (equippableComponent != null && equippableComponent.HasEquipped)
            {
                StatusEffect statusEffect = ResourcesPrefabManager.Instance.GetStatusEffectPrefab(_statusPrefabName);
                if (statusEffect)
                {
                   equippableComponent.EquippedInventoryEquippable.OnStatusEffectAdded(statusEffect);  
                }
            }
        }
    }

    [HarmonyPatch(nameof(StatusEffectManager.RemoveStatus))]
    public class StatusEffectManagerOnStatusRemoved
    {
        static void StatusEffectManager_OnStatusRemovedPrefix(StatusEffectManager __instance, string _uid)
        {
            PlayerInventoryEquippableComponent equippableComponent = __instance.m_character.GetComponent<PlayerInventoryEquippableComponent>();

            if (equippableComponent != null && equippableComponent.HasEquipped)
            {
                StatusEffect statusEffect = null;
                if (__instance.m_statuses.TryGetValue(_uid, out statusEffect))
                {
                    if (statusEffect)
                    {
                        equippableComponent.EquippedInventoryEquippable.OnStatusEffectRemoved(statusEffect);
                    }
                }
            }
        }
    }
}
