using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;
using UnityEngine.AI;
using RPG.Saving;
using RPG.Attributes;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] Transform target;
        [SerializeField] float maxSpeed = 5.66f;
        NavMeshAgent navMeshAgent;
        
        Health health;

        private void Awake() 
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }

        void Update()
        {
            //ölünce false döndürecek böylece ölen nesne'nin navmeshagent ı kapanacak
            navMeshAgent.enabled = !health.IsDead();
            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            //ActionScheduler sınıfı aksiyonları başlatıp durduruyor
            //hareket yapabilmek için saldırıdan önceki aksiyonu durdurduk
            //hareket aksiyonu başlaması için de bu sınıfı aksiyon olarak gönderdik
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedFraction);
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            //temas edilen noktaya doğru git
            navMeshAgent.destination = destination;
            //character lerin hızını ayarlıyoruz
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);//değeri 0 ile 1 arasında tutuyor
            //ilerleme durduysa diye tekrar ilerlemeyi aktif ediyoruz
            navMeshAgent.isStopped = false;
            
        }

        //NavMeshAgent'ı durduran fonksiyon (NMA hedefine varıncaya kadar durmaz)
        //Interface fonksiyonu
        public void Cancel()
        {
            //ilerlemeyi durdurduk
            navMeshAgent.isStopped = true;
        }

        //Animator deki değeri güncelleyen fonksiyon
        private void UpdateAnimator()
        {
            //Nav Mesh Agent ın hızını yerel değişken olarak aldık
            Vector3 velocity = navMeshAgent.velocity;
            //hızı kullanarak player objesine göre yumuşattık
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            //z eksenindeki değişimi Animator e göndermek için speed adıyla aldık
            float speed = localVelocity.z;
            //speed i Animator e gönderdik(local değeri global yaptık)
            GetComponent<Animator>().SetFloat("forwardSpeed", speed);
        }

        //Mover kayıt veriyapısı
        [System.Serializable]
        struct MoverSaveData
        {
            public SerializableVector3 position;
            public SerializableVector3 rotation;
        }

        //konum noktasını yakaladığımız fonksiyon
        public object CaptureState()
        {
            //bir sözlük oluşturduk player konum ve rotasyonunu kaydederken
            //bu sözlükteki verileri ayrı ayrı işleyip kaydediyoruz
            MoverSaveData data = new MoverSaveData();
            data.position = new SerializableVector3(transform.position);
            data.rotation = new SerializableVector3(transform.eulerAngles);

            //Konum vektörü Serializable
            return data;
        }

        //yakalanın konum noktasını characterlere aktardığımız fonksiyon
        public void RestoreState(object state)
        {
            //state i struct a göre cast ettik
            MoverSaveData data = (MoverSaveData)state;
            navMeshAgent.enabled = false;
            //position ve rotation ı kayıtlı yerden aldık
            transform.position = data.position.ToVector();
            transform.eulerAngles = data.rotation.ToVector();
            navMeshAgent.enabled = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
    }
}
