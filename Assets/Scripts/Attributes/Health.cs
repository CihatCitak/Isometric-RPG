using UnityEngine;
using RPG.Saving;
using RPG.Core;
using RPG.Stats;
using GameDevTV.Utils;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float regenerationPercentage = 85;
        [SerializeField] TakeDamageEvent takeDamage;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {
        }

        LazyValue<float> healthPoints;

        bool isDeath = false;

        private void Awake()
        {
            //Race Condition engelliyoruz
            healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        //LazyValue değer ataması için kullanılan fonksiyon 
        private float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void Start() 
        {
            //LazyValue çalışması için zorunlu kod
            healthPoints.ForceInit();
        }

        //Script aktifken çalışan fonksiyon
        private void OnEnable()
        {
            GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
        }

        //Script deaktifken çalışan fonksiyon
        private void OnDisable()
        {
            GetComponent<BaseStats>().onLevelUp -= RegenerateHealth;
        }

        public bool IsDead()
        {
            return isDeath;
        }

        //hasar alınan fonksiyon
        public void TakeDamage(GameObject instigator, float damage)
        {
            print(gameObject.name + " took damage: " + damage);

            //Health değeri negatif olmasın diye bu şekilde atama yapıldı
            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);
            //eğer can 0 olursa ölüm animasyonu çalışsın
            if(healthPoints.value == 0)
            {
                //öldü
                Die();
                //deneyim verdi
                AwardExperience(instigator);
            }
            else
            {
                //***Unity eventi çağrılıyor***
                takeDamage.Invoke(damage);
            }
        }

        //Can değerini geri döndüren fonksiyon
        public float GetHealthPoints()
        {
            return healthPoints.value;
        }

        //Level in en yüksek can değerini döndüren fonksiyon
        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }



        //Health değerini %100 cinsinden hesaplayıp geri döndüren fonksiyon
        public float GetPercentage()
        {
            return 100 * GetFraction();
        }
        public float GetFraction()
        {
            return healthPoints.value / GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        //ölüm animasyounun aktif eden fonksiyon
        private void Die()
        {
            //daha önce ölüm animasyonu çalışmışsa bir daha çalışmıyor
            if(isDeath) return;

            isDeath = true; 
            //ölüm animasyonu aktif edildi
            GetComponent<Animator>().SetTrigger("die");
            //Player ve Enemy Bir işlem yapamıyor
            GetComponent<ActionScheduler>().CancelCurrentAction();
            //eğitimde yok ben ekledim
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
            
        }


        private void AwardExperience(GameObject instigator)
        {
            //deneyimi kazanacak objenin deneyim componenti alınıyor
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) return;

            //deneyim kazanıldı
            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }

        //LevelUp olduğunda healtpoint i %85 yapan fonksiyon
        private void RegenerateHealth()
        {
            float regenHealthPoints = GetComponent<BaseStats>().GetStat(Stat.Health) * (regenerationPercentage / 100);
            //Healthpoint 85 den büyükse mevcut point korunuyor
            healthPoints.value = Mathf.Max(healthPoints.value, regenHealthPoints);
        }

        //can bilgisini yakaladığımız fonksiyon
        public object CaptureState()
        {
            return healthPoints;
        }

        //yakalanın can bilgisi characterlere aktardığımız fonksiyon
        public void RestoreState(object state)
        {
            healthPoints.value = (float)state;
            //sahne yüklendiğinde daha önce canı 0 olanlar ölüyor
            if (healthPoints.value == 0)
            {
                Die();
            }

        }
    }
}