using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {

        [SerializeField] float speed = 1;
        [SerializeField] bool isHoming = false;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifeTime = 10f;
        [SerializeField] float lifeAfterImpact = 2f;
        [SerializeField] GameObject[] destroyOnHit = null;

        Health target = null;
        GameObject instigator = null;
        float damage = 0f;

        private void Start()
        {
            //hedefe bak
            transform.LookAt(GetAimLocation());
        }

        void Update()
        {
            //bir hedef yoksa birşey yapma
            if (target == null) return;

            //okun takip etme özelliği varsa ve hedef yaşıyorsa
            if (isHoming && !target.IsDead())
            {
                //sürekli hedefe bak hedefe bak
                transform.LookAt(GetAimLocation());
            }
            //hedefe doğru git
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        //hedefi atıyan fonksiyon
        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            //Projetile ın target ve damage nesnesini yeni gelen target a eşitledik
            this.target = target;
            this.damage = damage;
            this.instigator = instigator;

            //10sn sonra fırlattığımız nesne kendisini yok edecek
            Destroy(gameObject, maxLifeTime);
        }

        //Hedefin tam yükseklik merkezini işan noktası olarak döndüren fonksiyon
        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            //eğer coolider yoksa
            if (targetCapsule == null)
            {
                return target.transform.position;
            }

            //collider ın tam orta noktası hesaplandı
            return target.transform.position + Vector3.up * targetCapsule.height / 2;
        }

        //Ok bir yere çarptığında
        private void OnTriggerEnter(Collider other)
        {
            //eğer hedeften başka bir yere çarpmışsa birşey yapma
            if (other.GetComponent<Health>() != target) return;
            //hedef ölmüşse ok ölüye çarpmıyor devam ediyor
            if (target.IsDead()) return;
            //hedef hasar aldı
            target.TakeDamage(instigator, damage);

            //ok ilerlemesin diye hızı 0 yaptık
            speed = 0;

            //bir hit effect varsa
            if (hitEffect != null)
            {
                //çarpmanın olduğu yerde effect i çağırıyoruz
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            }

            //nesne çarptığı anda nesnenin kendisi yok ediliyor
            foreach (GameObject toDestroy in destroyOnHit)
            {
                Destroy(toDestroy);
            }
            //nesne çarptıktan sonra effect tamamlansın diye 2sn daha nesne yaşıyor
            Destroy(gameObject, lifeAfterImpact);
        }
    }
}
