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
        public float CurrentCharge = 0f;
        public float MaximumCharge = 200f;

        public float EnergyTakenPerTick = 1f;
        public float TickTime = 1f;

        private float CurrentTimer = 0;
        private float ManaUsed = 0.5f;
        private float StaminaGain = 1f;
        private float AdditionalStaminaGain = 0.5f;
        private float BurntResourceRestore = 0.5f;
        private float StaminaGainUsedPrime = 10f;
        private float ManaGainUsedPrime = 10f;

        //to check if the engine is empty
        public bool IsEmpty => CurrentCharge > 0 ? true : false;
        //The Current Charge Level as a normalized percentage (eg 0-1) 0 being 0% and 1 being 100%
        public float ChargeAsNormalizedPercent => CurrentCharge / MaximumCharge;
        //enum for the breakthrough passives
        public enum BreakthroughBonus
        {
            NONE,
            STAMINA,
            MANA
        }
        public BreakthroughBonus Breakthrough = ItemEngine_Charge.BreakthroughBonus.NONE;

        //check for breakthrough on equip
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

        public override void OnStatusEffectRemoved(StatusEffect Status)
        {
            //if Charged is used it will add 10 energy
            if (Status.IdentifierName == "Charged")
            {
                AddEnergy(10);
                EquippedCharacter.StatusEffectMngr.AddStatusEffect("Possessed");
            }
            //if Prime is used with Stamina engine it will give health and stamina
            if (Status.IdentifierName == "Prime" && Breakthrough == BreakthroughBonus.STAMINA)
            {
                EquippedCharacter.Stats.AffectStamina(StaminaGainUsedPrime);
                EquippedCharacter.Stats.AffectHealth(StaminaGainUsedPrime);
            }
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
            if (EquippedCharacter == null)
            {
                return;
            }
            //add stamina if the engine is active and in combat
            //and remove energy

            EquippedCharacter.UpdateCombatStatus();

            if (EquippedCharacter.InCombat)
            {
                if (HasEnergy(EnergyTakenPerTick))
                {
                    EquippedCharacter.Stats.AffectStamina(StaminaGain);
                    RemoveEnergy(EnergyTakenPerTick);

                    BreakthroughBonus mode = GetBreakThroughMode(EquippedCharacter);

                    switch (mode)
                    {
                        case BreakthroughBonus.STAMINA:
                            DoStaminaBreakThroughTick(EquippedCharacter);
                            break;
                        case BreakthroughBonus.MANA:
                            DoManaBreakThroughTick(EquippedCharacter);
                            break;
                    }
                }
            }
            else if (!EquippedCharacter.InCombat)
            {
                AddEnergy(40);                 
            }


            //do this after you've changed the energy levels so you dont have to wait until the next tick.
            //remove energized if energy drops below 50%
            if (ChargeAsNormalizedPercent < 0.5f)
            {
                EquippedCharacter.StatusEffectMngr.RemoveStatusWithIdentifierName("Energized");
            }
        }
        public virtual void DoManaBreakThroughTick(Character Character)
        {
            //EquippedCharacter.Stats.UseMana(StaminaGain);
            if (Character.Stats.m_burntHealth > 0 || Character.Stats.m_burntStamina > 0)
            {
                Character.Stats.UseMana(null, ManaUsed);
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

        public bool HasEnergy(float amount)
        {
            return CurrentCharge >= amount;
        }

        public virtual void OnEnergyFull()
        {
            EquippedCharacter.StatusEffectMngr.AddStatusEffect("Energized");
        }


        public virtual void OnEnergyEmpty()
        {
            if (EquippedCharacter == null)
            {
                return;
            }

            if (EquippedCharacter)
            {
                //add drawback on player when he reaches 0 energy
                EquippedCharacter.StatusEffectMngr.AddStatusEffect("Drawback");
            }
        }
        
    }
}
