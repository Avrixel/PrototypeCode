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
        public bool IsEmpty => CurrentCharge > 0 ? true : false;
        //enum for the breakthrough passives
        public enum BreakthroughBonus
        {
            NONE,
            STAMINA,
            MANA
        }
        public BreakthroughBonus Breakthrough = ItemEngine_Charge.BreakthroughBonus.NONE;

        //check for breakthrough on equip and add 5 maxStamina for mana engine and 15 maxStamina for stamina engine
        public BreakthroughBonus GetBreakThroughMode(Character Character)
        {
            if (Character.Inventory.SkillKnowledge.IsItemLearned(-19024))
            {
                return BreakthroughBonus.MANA;
            }
            else if (Character.Inventory.SkillKnowledge.IsItemLearned(-19024))
            {
                return BreakthroughBonus.STAMINA;
            }

            return BreakthroughBonus.NONE;
        }

        public override void OnEquip(Character CharacterToEquip)
        {
            base.OnEquip(CharacterToEquip);

            

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
            if (!IsEmpty && EquippedCharacter && EquippedCharacter.InCombat)
            {
                EquippedCharacter.Stats.AffectStamina(StaminaGain);
                RemoveEnergy(EnergyTakenPerTick);


                BreakthroughBonus mode = GetBreakThroughMode(EquippedCharacter);

                switch (mode)
                {
                    case BreakthroughBonus.NONE:

                        break;
                    case BreakthroughBonus.STAMINA:
                        DoStaminaBreakThroughTick(EquippedCharacter);
                        break;
                    case BreakthroughBonus.MANA:
                        DoManaBreakThroughTick(EquippedCharacter);
                        break;
                }
            }
            else if (!EquippedCharacter.InCombat && EquippedCharacter)
            {
                AddEnergy(20);
            }
        

            //add energy if not in combat
            if (!EquippedCharacter.InCombat && EquippedCharacter)
            {
                AddEnergy(20);
            }
        }
        public virtual void DoManaBreakThroughTick(Character Character)
        {
            //EquippedCharacter.Stats.UseMana(StaminaGain);
            if (Character.Stats.m_burntHealth > 0 || Character.Stats.m_burntStamina > 0)
            {
                Character.Stats.UseMana(null, BurntResourceRestore);
                Character.Stats.RestoreBurntHealth(BurntResourceRestore);
                Character.Stats.RestoreBurntStamina(BurntResourceRestore);
            }
        }
        public virtual void DoStaminaBreakThroughTick(Character Character)
        {
            //EquippedCharacter.Stats.UseMana(StaminaGain);
            Character.Stats.AffectStamina(AdditionalStaminaGain);
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
            EquippedCharacter.StatusEffectMngr.AddStatusEffect("Energized");
        }


        public virtual void OnEnergyEmpty()
        {
            if (EquippedCharacter)
            {
                //add drawback on player when he reaches 0 energy
                EquippedCharacter.StatusEffectMngr.AddStatusEffect("Drawback");
            }
        }
        
    }
}
