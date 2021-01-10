using RPG.Movement;
using RPG.Core;
using UnityEngine;
using GameDevTV.Utils;
using System;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour,IAction, ISaveable, IModifierProvider
    {
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] Weapon defaultWeapon = null;

        Health target;
        float timeSinceLastAttack = Mathf.Infinity;

        LazyValue<Weapon> currentWeapon;

        private void Awake()
        {
            //Race Condition önlüyoruz
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        private Weapon SetupDefaultWeapon()
        {
            AttachWeapon(defaultWeapon);
            return defaultWeapon;
        }

        private void Start() 
        {
            //LazyValue çalışması için yapmak zorunlu
            currentWeapon.ForceInit();
        }

        private void Update() 
        {
            timeSinceLastAttack += Time.deltaTime;

            //target null ise (saldıracak bir hedef yok ise) update e devam etme
            if(target == null) return;
            
            //eper target ölmüşse hiç birşey yapma
            if(target.IsDead()) return;

            //saldıracak bir hedef varsa ancak saldırma menzili dışındaysa
            if(target != null && !GetIsInRange())
            {
                //saldırı menziline girene kadar target a git
                GetComponent<Mover>().MoveTo(target.transform.position, 1f);
            }
            //saldıracak bir hedef varsa ve hedef saldırı menzilindeyse 
            else
            {
                //hareket etmeyi bırak ve saldır
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
                
            }
        }

        //silah spawn'ladığımız fonksiyon
        public void EquipWeapon(Weapon weapon)
        {
            //mevcut silah yeni silaha eşitleniyor
            currentWeapon.value = weapon;
            AttachWeapon(weapon);
            
        }

        //Silahı tutturan fonksiyon
        private void AttachWeapon(Weapon weapon)
        {
            Animator animator = GetComponent<Animator>();
            weapon.Spawn(rightHandTransform, leftHandTransform, animator);        
        }

        //Target ın can componentini döndüren fonksiyon
        public Health GetTarget()
        {
            return target;
        }

        //Animator deki "attack" trigger ını etkinleştiren fonksiyon
        private void AttackBehaviour()
        {
            //eğer bir önceki saldırıdan sonra 1 sn geçtiyse tekrar saldırı yapılabilir
            if(timeSinceLastAttack > timeBetweenAttacks)
            {
                //düşmana bakıyoruz
                transform.LookAt(target.transform);
                TriggerAttack();
                timeSinceLastAttack = 0;
                //Hit() eventini tetikleyen fonksiyonda gene burası
            }
        }

        //Attack için animator deki aktif etme işlemleri
        private void TriggerAttack()
        {
            //stopAttack trigger ını resetledik
            GetComponent<Animator>().ResetTrigger("stopAttack");
            //saldırı animasyonu aktif ediliyor
            GetComponent<Animator>().SetTrigger("attack");
        }

        //Animator tarafından otomatik çağırılan fonksiyon
        void Hit()
        {
            //Basestat tan level in damage verisini aldık
            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);

            //target null sa birşey yapma
            if(target == null) return;

            //silahın projectile ı varsa yani ok atması gerekiyorsa veya başka bir materyal atıyorsa
            if(currentWeapon.value.HasProjectile())
            {
                //projectile başlatılıyor
                currentWeapon.value.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
            }
            //bir projectile ı yoksa silah sword demektir
            else
            {
                //Hasar alma fonksiyonunu çağırıp parametre olarak silah gücünü verdik
                target.TakeDamage(gameObject, damage);
            }
            
        }

        //Animator tarafından otomatik çağırılan fonksiyon(Bow animasyonu tarafından çağrılıyor)
        void Shoot()
        {
            Hit();
        }

        private bool GetIsInRange()
        {
            //eğer uzaklık "weaponRange" den küçükse true, büyükse false
            return Vector3.Distance(transform.position, target.transform.position) < currentWeapon.value.GetRange();
        }

        //Düşman ölse bile collider ı duruyor bu collider ın arkadasındaki
        //bir başka düşmana saldırmak için bu sistem mi oluşturduk
        public bool CanAttack(GameObject combatTarget)
        {
            if(combatTarget == null ) return false;
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        //PlayerControlde bu fonksiyon çağırılıyor oradan gelen combatTarget
        //değeri local değer target a atanıyor
        public void Attack(GameObject combatTarget)
        {
            //ActionScheduler sınıfı aksiyonları başlatıp durduruyor
            //saldırı yapabilmek için saldırıdan önceki aksiyonu durdurduk
            //saldırı aksiyonu başlaması için de bu sınıfı aksiyon olarak gönderdik
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        //Interface fonksiyonu
        public void Cancel()
        {
            //saldırıyı cancel lıyoruz
            StopAttack();
            target = null;
            //düşmana ilerleme işleminide durduruyoruz
            GetComponent<Mover>().Cancel();

        }

        //Attack durdurmak için Animator de aktif etme işlemleri
        private void StopAttack()
        {
            //saldırı triger ını resetledik
            GetComponent<Animator>().ResetTrigger("attack");
            //saldırı durdurma trigger ını aktif ettik
            GetComponent<Animator>().SetTrigger("stopAttack");
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeapon.value.GetPercentageBonus();
            }
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeapon.value.GetDamage();
            }
        }

        //Save sistemi için yakalama fonksiyonu
        public object CaptureState()
        {
            return currentWeapon.value.name;
        }

        //Save sistemi için yükleme fonksiyonu
        public void RestoreState(object state)
        {
            //kayıt dosyasından silahın adını alıyoruz
            //Resource dan kayıtlı silahı alıyoruz
            string weaponName = (string) state;
            Weapon weapon = UnityEngine.Resources.Load<Weapon>(weaponName);
            EquipWeapon(weapon);

        }
    }
}