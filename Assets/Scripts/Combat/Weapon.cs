using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] GameObject equippedPrefab = null;
        [SerializeField] float weaponDamage = 5f;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float percentageBonus = 0;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;

        const string weaponName = "Weapon";
        
        //silah'ı spawnlıyan fonksiyon
        public void Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            //bir önceki silahı yok ediyoruz
            DestroyOldWeapon(rightHand, leftHand);

            if(equippedPrefab != null)
            {
                Transform handTransform = GetTransform(rightHand, leftHand);
                //prefab ı player ın elin konumuna göre ürettik
                GameObject weapon = Instantiate(equippedPrefab, handTransform);
                //silahın ismini aldık     //bir önceki silahı yok ediyoruz
                weapon.name = weaponName;
            }


            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            //eğer override varsa
            if (animatorOverride != null)
            {
                //Animator ü elimizdeki silaha göre override ediyoruz
                animator.runtimeAnimatorController = animatorOverride;
            }
            //override yoksa
            else if (overrideController != null)
            {
                //animator ü restledik yeni alınan silah bir override içermiyorsa eski 
                //silah animasyonu ile değilde unarmed animasyonu ile devam etmek için bunu yaptık
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            //sağ elde silah var mı diye kontrol ediyoruz sağ elde silah yoksa
            //sol elde var mı diye kontrol ediyoruz gene yoksa birşey yapmıyoruz
            //herhangi bir elde silah tespit edildiyse o silahın ismini değiştirip yok ediyoruz
            Transform oldWeapon = rightHand.Find(weaponName);
            if(oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);
            }
            if(oldWeapon == null) return;

            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }
        
        //silah sağ el içinse silahın oluşturulacağı konumu sağ el değilse 
        //sol el konumunu geri döndüren fonksiyon
        private Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;
            if (isRightHanded) handTransform = rightHand;
            else handTransform = leftHand;
            return handTransform;
        }

        //Projectile olup olmadığını döndüren fonksiyon
        public bool HasProjectile()
        {
            //projectile nulldan farklıysa true
            return projectile != null;
        }

        //Projectile ı başlatan fonksiyon
        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, GetTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, calculatedDamage);
        }

        //silah hasar verisi geri döndüren fonksiyon
        public float GetDamage()
        {
            return weaponDamage;
        }
        
        //Hasar bonusun döndüren fonksiyon
        public float GetPercentageBonus()
        {
            return percentageBonus;
        }

        //silah menzil verisini geri döndüren fonksiyon
        public float GetRange()
        {
            return weaponRange;
        }
    }
}
