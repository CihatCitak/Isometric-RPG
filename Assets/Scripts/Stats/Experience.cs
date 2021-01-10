using UnityEngine;
using RPG.Saving;
using System;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        //toplam dneyim puanı
        [SerializeField] float experiencePoints = 0;
        
        //Action nesnesi
        public event Action onExperienceGained;

        //deneyim puanını geri döndüren fonksiyon
        public float GetPoints()
        {
            return experiencePoints;
        }

        //deneyim puanını arttıran fonksiyon
        public void GainExperience(float experience)
        {
            experiencePoints += experience;
            //Action ı çağırdık
            onExperienceGained();
        }

        //deneyim ğpuanı kayıt ediliyor
        public object CaptureState()
        {
            return experiencePoints;
        }

        //deneyim puanı load ediliyor
        public void RestoreState(object state)
        {
            experiencePoints = (float)state;
        }
    }
}