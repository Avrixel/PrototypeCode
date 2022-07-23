using AvrixelPrototype;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OutwardModTemplate
{
    public class BaseInventoryEquippable : MonoBehaviour
    {
        public Item ParentItem => GetComponent<Item>();
        public Character EquippedCharacter { get; private set; }
        public bool IsEquipped => EquippedCharacter != null ? true : false;

        public bool HasEquipVisual = false;
        public EquipVisualInformation EquipVisualInformation;

        private GameObject EquipVisualInstance;

        public virtual void OnEquip(Character CharacterToEquip)
        {
            EquippedCharacter = CharacterToEquip;


            if (EquipVisualInformation != null)
            {
                CreateEquipVisual();
            }
        }

        public virtual void OnUnEquip(Character CharacterToEquip)
        {
            EquippedCharacter = null;
        }


        public virtual void Update()
        {
            if (EquippedCharacter == null) return;
        }

        private GameObject CreateEquipVisual()
        {
            if (EquipVisualInstance)
            {
                DestroyEquipVisual();
            }

            if (EquipVisualInformation != null)
            {
                GameObject VisualPrefab = Helpers.GetFromAssetBundle<GameObject>(EquipVisualInformation.SLPackName, EquipVisualInformation.AssetBundleName, EquipVisualInformation.PrefabName);

                if (VisualPrefab != null)
                {
                    EquipVisualInstance = GameObject.Instantiate(VisualPrefab, GetEquipmentSlotTransform(EquipVisualInformation.EquipmentSlotID));
                    EquipVisualInstance.transform.localPosition = Vector3.zero;

                    return EquipVisualInstance;
                }           
            }

            return null;
        }


        private void DestroyEquipVisual()
        {
            if (EquipVisualInstance != null)
            {
                Destroy(EquipVisualInstance);
                EquipVisualInstance = null;
            }
        }

        private Transform GetEquipmentSlotTransform(EquipmentSlot.EquipmentSlotIDs EquipmentSlotID)
        {
            if (EquippedCharacter != null)
            {
                return EquippedCharacter.Inventory.Equipment.GetEquipmentVisualSlotTransform(EquipmentSlotID);
            }

            return null;
        }
    }
}
