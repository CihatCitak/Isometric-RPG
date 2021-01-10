using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Utils;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        //değeri 1 ile 99 arasında tutuyoruz
        [Range(1, 99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpParticleEffect = null;
        [SerializeField] bool shouldUseModifiers = false;

        public event Action onLevelUp;

        LazyValue<int> currentLevel;

        Experience experience;

        private void Awake()
        {
            experience = GetComponent<Experience>();

            //Race Condition engelliyoruz
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start()
        {
            currentLevel.ForceInit();
        }
        
        //Script aktifken çalışan fonksiyon
        private void OnEnable()
        {
            if (experience != null)
            {
                experience.onExperienceGained += UpdateLevel;
            }
        }
        
        //Script aktifken çalışan fonksiyon
        private void OnDisable()
        {
            if (experience != null)
            {
                experience.onExperienceGained -= UpdateLevel;
            }
        }

        //Level atlatan fonksiyon Action tarafından çağırılıyor
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

        //LevelUp efectini çağıran fonksiyon
        private void LevelUpEffect()
        {
            Instantiate(levelUpParticleEffect, transform);
        }

        //Stat değerlerini proression dan alan fonksiyon
        public float GetStat(Stat stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifier(stat) / 100);
        }

        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        //Level i geri döndüren fonksiyon
        public int GetLevel()
        {
            if (currentLevel.value < 1)
            {
                currentLevel.value = CalculateLevel();
            }
            return currentLevel.value;
        }

        private float GetAdditiveModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0;

            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        private float GetPercentageModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0;

            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetPercentageModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        //Level ı hedaplayıp geri döndüren fonksiyon
        //LazyValue için de ilk atama olarak kullanılıyor
        public int CalculateLevel()
        {  
            Experience experience = GetComponent<Experience>();
            //henüz Exp kazanmamışsak yani 1 level isek
            if(experience == null) return startingLevel;

            //şuanki exp verisi
            float currentXP = experience.GetPoints();

            //progression da tanımlanmış level sayısı verisi (en yüksek level)
            int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);

            for(int level = 1; level <= penultimateLevel; level++)
            {
                //sahip olunan Exp hangi Level'in karşılığı olan 
                //Exp ten küçükse Player o Level dir
                float XPToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);
                if(XPToLevelUp > currentXP)
                {
                    return level;
                }
            }

            //Eğer yukardaki şarta uygun level yoksa en yüksek level i 1 arttır
            return penultimateLevel + 1;
        }
    }

}