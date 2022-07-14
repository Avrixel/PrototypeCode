using OutwardModTemplate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AvrixelPrototype
{
    public class PlayerInventoryEquippableComponent : MonoBehaviour
    {
        public Item EquippedItem { get; private set; }
        public BaseInventoryEquippable EquippedInventoryEquippable => EquippedItem.GetComponent<BaseInventoryEquippable>();
        public Character ParentCharacter => gameObject.GetComponent<Character>();
        public bool HasEquipped => EquippedItem != null ? true : false;

        public void Equip(Item ItemEngine)
        {
            EquippedItem = ItemEngine;

            if (EquippedItem != null && EquippedInventoryEquippable != null)
            {
                EquippedInventoryEquippable.OnEquip(ParentCharacter);
                ParentCharacter.CharacterUI.ShowInfoNotification("Equipped");
            }
        }
        
        public void UnEquip()
        {
            if (EquippedItem != null && EquippedInventoryEquippable != null)
            {
                EquippedInventoryEquippable.OnUnEquip(ParentCharacter);
                ParentCharacter.CharacterUI.ShowInfoNotification("UnEquipped");
            }

            EquippedItem = null;
        }
    }
}
