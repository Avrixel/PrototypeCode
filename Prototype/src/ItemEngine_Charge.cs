using OutwardModTemplate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AvrixelPrototype
{
    public class ItemEngine_Charge : BaseInventoryEquippable
    {
        public float CurrentCharge;
        public float MaximumCharge;

        public float EnergyTakenPerTick = 10f;
        public float TickTime = 10f;

        private float CurrentTimer = 0;
        private string StatStackUID = "PrototypeStaminaStatBoost";
        private float StaminaGain = 1.5f;


        public override void OnEquip(Character CharacterToEquip)
        {
            base.OnEquip(CharacterToEquip);

            //add stamina regen buff on equip
            if (EquippedCharacter && CurrentCharge >= EnergyTakenPerTick)
            {
                if (!EquippedCharacter.Stats.m_staminaRegen.m_rawStack.ContainsKey(StatStackUID))
                {
                    EquippedCharacter.Stats.m_staminaRegen.m_rawStack.Add(StatStackUID, new StatStack(StatStackUID, StaminaGain));
                }
            }
        }

        public override void Update()
        {
            base.Update();

            CurrentTimer += Time.deltaTime;
            if (IsEquipped && CurrentTimer >= TickTime)
            {
                OnTick();
            }
        }

        public virtual void OnTick()
        {
            RemoveEnergy(EnergyTakenPerTick);
        }

        public void AddEnergy(float amount)
        {
            CurrentCharge += amount;

            if (CurrentCharge >= MaximumCharge)
            {
                CurrentCharge = MaximumCharge;
                //full
                OnEnergyFull();
            }
        }

        public void RemoveEnergy(float amount)
        {
            CurrentCharge -= amount;

            if (CurrentCharge <= 0)
            {
                CurrentCharge = 0;
                //empty
                OnEnergyEmpty();
            }
        }

        public virtual void OnEnergyFull()
        {

        }


        public virtual void OnEnergyEmpty()
        {
            if (EquippedCharacter)
            {
                if (EquippedCharacter.Stats.m_staminaRegen.m_rawStack.ContainsKey(StatStackUID))
                {
                    EquippedCharacter.Stats.m_staminaRegen.m_rawStack.Remove(StatStackUID);
                }
            }
        }
        
    }
}
