using UnityEngine;
using System.Collections.Generic;
using System;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] characterClasses = null;

        //2 boyutlu bir sözlük oluşturduk (bir nevi iç içe farklı tiplere sahip bir dizi)
        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;

        //Stat değerini progressiondan çeken fonksiyon
        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            BuildLookup();

            //class a ve stat a uygun olan level geri döndürülüyor
            float[] levels = lookupTable[characterClass][stat];

            //eğer uygun level yoksa 0 döndürüyoru
            if (levels.Length < level)
            {
                return 0;
            }

            //uygun level geri döndürülüyor
            return levels[level - 1];

            //yorum satırları kodun eski hali yeni hali daha performanslı çünkü
            //bu fonksiyon update ile çağırılıyor yani her frame de tekrarlanıyorlar
            //---------------------------------------------------------------------------------------------
            /*//bütün class lar için
            foreach (ProgressionCharacterClass progressionClass in characterClasses)
            {
                //eğer progressio daki class character in classıyla aynı değilse birşey yapma
                if(progressionClass.characterClass != characterClass) continue;

                //bütün stat lar için (health ve experience)
                foreach (ProgressionStat progressionStat in progressionClass.stats)
                {
                    //eğer stat lar aynı değilse birşey yapma
                    if(progressionStat.stat != stat) continue;

                    //eğer leveller uyuşmuyorsa yani daha düşük yada daha yüksek level
                    //statları yanlışlıkla atanmasın sadece characterin level i atansın
                    if(progressionStat.levels.Length < level) continue;

                    //program bu koda kadar gelmişse class, stat, level eşleşti demektir
                    //level verisini geri döndürüyoruz
                    return progressionStat.levels[level - 1];
                }
            }
            return 0;*/
            //----------------------------------------------------------------------------------------------

        }

        //LookUp ilk oluşturma fonksiyonu
        private void BuildLookup()
        {
            //daha önce lookUpTable oluşturulmuş ise bir daha oluşturm
            if (lookupTable != null) return;

            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            //bütün class lar için
            foreach (ProgressionCharacterClass progressionClass in characterClasses)
            {
                //içerideki sözlük oluşturuldu
                var statLookupTable = new Dictionary<Stat, float[]>();

                //bütün stat lar için(health ve experience)
                foreach (ProgressionStat progressionStat in progressionClass.stats)
                {
                    //iç sözlüğün atamaları yapılıyor
                    statLookupTable[progressionStat.stat] = progressionStat.levels;
                }

                //iç deki sözlük alınıyor
                lookupTable[progressionClass.characterClass] = statLookupTable;
            }
        }

        //Level sayısını geri döndüren fonksiyon
        public int GetLevels(Stat stat, CharacterClass characterClass)
        {
            //Table yoksa oluşturuyoru
            BuildLookup();

            //level ler alındı level sayısı geri döndürüldü
            float[] levels = lookupTable[characterClass][stat];
            return levels.Length;
        }


        [System.Serializable]
        class ProgressionCharacterClass
        {
            public CharacterClass characterClass;
            public ProgressionStat[] stats;
        }

        [System.Serializable]
        class ProgressionStat
        {
            public Stat stat;
            public float[] levels;
        }
    }
}