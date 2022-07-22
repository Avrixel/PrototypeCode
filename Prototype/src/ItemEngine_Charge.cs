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
        public float MaximumCharge = 200f;

        public float EnergyTakenPerTick = 1f;
        public float TickTime = 1f;

        private float CurrentTimer = 0;
        private string StatStackUID = "PrototypeStaminaStatBoost";
        private float StaminaGain = 1f;
        private float AdditionalStaminaGain = 0.5f;
        private float BurntResourceRestore = 0.5f;

        //to check if the engine is empty
        public bool IsEmpty;
        //bool for the breakthrough passives
        public bool StaminaEngine= false;
        public bool ManaEngine= false;


        public override void OnEquip(Character CharacterToEquip)
        {
            base.OnEquip(CharacterToEquip);

            //check for breakthrough on equip and add 5 maxStamina for mana engine and 15 maxStamina for stamina engine
          /*  if (Character.Inventory.SkillKnowledge.IsItemLearned(-19024))
            {
                ManaEngine = true;
            }
            if (Character.Inventory.SkillKnowledge.IsItemLearned(-19025))
            {
                StaminaEngine = true;
            }*/

        }

        public override void Update()
        {
            base.Update();

            CurrentTimer += Time.deltaTime;
            if (IsEquipped && CurrentTimer >= TickTime)
            {
                CurrentTimer = 0;
                OnTick();
            }
        }

        public virtual void OnTick()
        {

            //add stamina if the engine is active and in combat
            //and remove energy
            if (IsEmpty=false && EquippedCharacter && EquippedCharacter.InCombat)
            {
                EquippedCharacter.Stats.AffectStamina(StaminaGain);
                RemoveEnergy(EnergyTakenPerTick);
                //remove 0.5 mana to remove 0.5 burnt health and stamina
                if (ManaEngine)
                {
                    //EquippedCharacter.Stats.UseMana(StaminaGain);
                    EquippedCharacter.Stats.RestoreBurntHealth(BurntResourceRestore);
                    EquippedCharacter.Stats.RestoreBurntStamina(BurntResourceRestore);
                }
                if (StaminaEngine)
                {
                    EquippedCharacter.Stats.AffectStamina(AdditionalStaminaGain);
                }
            }

            //add energy if not in combat
            if (!EquippedCharacter.InCombat && EquippedCharacter)
            {
                AddEnergy(20);
            }
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
            //check if at least some energy is in the engine
            if (CurrentCharge > 0)
            {
                IsEmpty = false;
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
                IsEmpty = true;
                //add drawback on player when he reaches 0 energy
                //EquippedCharacter.StatusEffectMngr.AddStatusEffect(Drawback);
            }
        }
        
    }
}
