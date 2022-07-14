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


    //This patch unequips it no matter where its removed from, so even between bags, safest way to keep it stable for now.
    [HarmonyPatch(typeof(ItemContainer))]
    public class ItemContainerRemoveItem
    {
        [HarmonyPatch(nameof(ItemContainer.RemoveItem)), HarmonyPrefix]
        static void ItemContainerRemoveItem_Prefix(ItemContainer __instance, Item _itemToRemove)
        {
            PlayerInventoryEquippableComponent equippableComponent = __instance.OwnerCharacter.gameObject.GetComponentInParent<PlayerInventoryEquippableComponent>();

            if (equippableComponent != null && equippableComponent.HasEquipped && equippableComponent.EquippedItem == _itemToRemove)
            {
                if (Helpers.HasButtonHighLight(equippableComponent.EquippedItem.UID))
                {
                    Helpers.DestroyButtonHighLight(equippableComponent.EquippedItem.UID);
                }

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
}
