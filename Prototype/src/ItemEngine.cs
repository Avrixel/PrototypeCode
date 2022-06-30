using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OutwardModTemplate
{
    public class ItemEngine : MonoBehaviour
    {
        public Item ParentItem => GetComponent<Item>();
        public Character EquippedCharacter => ParentItem.OwnerCharacter;
        public bool IsEquipped = true;
        public float timer = 0;
        public float ticktime = 1;
        public float staminaValue = 0.25f;

        public virtual void OnEquip()
        {
            AvrixelPrototype.Prototype.Log.LogMessage("Engine equiped");
            // IsEquipped = true;
        }

        public virtual void OnUnEquip()
        {
            AvrixelPrototype.Prototype.Log.LogMessage("Engine unequiped");
            //  IsEquipped = false;
        }


        public virtual void Update()
        {
            timer += Time.deltaTime;
            if (timer >= ticktime)
            {
                timer = 0;
                OnTick();
            }
            
        }
        public virtual void OnTick()
        {
            if (IsEquipped && EquippedCharacter != null)
            {
                AvrixelPrototype.Prototype.Log.LogMessage("Engine discharge");
                EquippedCharacter.Stats.AffectStamina(staminaValue);

            }
        }
    }


    [HarmonyPatch(typeof(CharacterInventory), nameof(CharacterInventory.TakeItem), new Type[] { typeof(Item), typeof(bool) })]
    public class TakeItemToPouchPatch
    {
        static void Prefix(CharacterInventory __instance, ref Item takenItem, bool _tryToEquip)
        {
            ItemEngine itemEngine = takenItem.GetComponent<ItemEngine>();
            AvrixelPrototype.Prototype.Log.LogMessage("Take item to pouch");
            if (itemEngine != null)
            {

                itemEngine.OnEquip();
            }           
        }
    }

    [HarmonyPatch(typeof(CharacterInventory), nameof(CharacterInventory.RemoveItem))]
    public class RemoveItemFromPouchPatch
    {
        static void Prefix(CharacterInventory __instance, ref int _itemID, int _quantity)
        {
            AvrixelPrototype.Prototype.Log.LogMessage("Remove Item");
            List<Item> items =  __instance.GetOwnedItems(_itemID);
            foreach (var item in items)
            {
                if (__instance.Pouch.Contains(item.UID))
                {
                    ItemEngine itemEngine = item.GetComponent<ItemEngine>();

                    if (itemEngine != null)
                    {
                        itemEngine.OnUnEquip();
                    }
                }
            }          
        }
    }
}
