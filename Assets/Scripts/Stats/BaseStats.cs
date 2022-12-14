using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Utils;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [SerializeField] [Range(1,99)] int startLevel = 1;
        [SerializeField] CharacterClass _characterClass;
        [SerializeField] Progression _progression = null;
        [SerializeField] GameObject levelUpParticleEffect = null;
        [SerializeField] bool shouldUseModifiers = false;

        public event Action onLevelUp;
        private Experience experience;
        
        LazyValue<int> currentLevel ;

        private void Awake()
        {
            currentLevel = new LazyValue<int>(CalculateLevel);
            experience = GetComponent<Experience>();
        }

        private void Start()
        {
            currentLevel.ForceInit();
            
        }

        private void OnEnable()
        {
            if (experience != null)
            {
                experience.onExperienceGained += UpdateLevel;
            }
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();

            if (newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
                LevelUpEffect();
                onLevelUp();
            }
        }
        
        private void LevelUpEffect()
        {
            Instantiate(levelUpParticleEffect, transform);
            
        }

        public float GetStat(Stat stat)
        {
            return (GetS(stat) + GetAdditiveModifier(stat)) * (1 + GetPercetangeModifiers(stat)/100);
        }

      

        private float GetS(Stat stat)
        {
            return _progression.GetStat(stat,_characterClass , CalculateLevel());
        }

        private float GetAdditiveModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0;
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetAdditiveModifier(stat))
                {
                    total += modifier;
                }
            }

            return total;
        }
        
        private float GetPercetangeModifiers(Stat stat)
        {
            if (!shouldUseModifiers) return 0;
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetPercentageModifier(stat))
                {
                    total += modifier;
                }
            }

            return total;
        }

        public int GetLevel()
        {
            return currentLevel.value;
        }

        public int CalculateLevel()
        {
            Experience experience = GetComponent<Experience>();

            if (experience == null) return startLevel;
            float currentXP = experience.GetPoints();
            int  penultimateLevels= _progression.GetLevels(Stat.ExperienceToLevelUp, _characterClass);
            for (int level = 1; level <= penultimateLevels ; level++)
            {
                float XPToLevelUp = _progression.GetStat(Stat.ExperienceToLevelUp, _characterClass, level);
                if (XPToLevelUp > currentXP)
                {
                    return level;
                }
            }

            return penultimateLevels + 1;
        }
    }
}
