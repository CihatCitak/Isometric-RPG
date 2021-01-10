using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        enum DestinationIdentifier
        {
            A, B, C, D, E
        }

        [SerializeField] int sceneToLoad = -1;
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifier destination;
        [SerializeField] float fadeOutTime = 1f;
        [SerializeField] float fadeInTime = 2f;
        [SerializeField] float fadeWaitTime = 0.5f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                //SceneManager.LoadScene(sceneToLoad);
                //yeni sahne istenilen özelliklerle yüklendi
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition()
        {
            //daha sonrada silinecek
            if(sceneToLoad < 0)
            {
                Debug.LogError("Yükelecek bir Scene yok");
                yield break;
            }


            //yükleme anında yoketme
            DontDestroyOnLoad(gameObject);
            //Fader ncomponentini aldık
            Fader fader = FindObjectOfType<Fader>();

            //sahneyi yavaşça kararıyor
            yield return fader.FadeOut(fadeOutTime);

            SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
            wrapper.Save();

            //sahneyi yükledik
            yield return SceneManager.LoadSceneAsync(sceneToLoad);

            wrapper.Load();

            //doğru portal bulundu
            Portal otherPortal = GetOtherPortal();
            //player portala göre konumlandırıldı
            UpdatePlayer(otherPortal);

            wrapper.Save();

            //sahne karanlık bekliyor
            yield return new WaitForSeconds(fadeWaitTime);
            //sahne yavaşça aydınlanıyor
            yield return fader.FadeIn(fadeInTime);

            //objeyi(portal) yok et
            Destroy(gameObject);
        }

        //player ı ışınlanacağı yere uygun hale getirdiğimiz fonksiyon
        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            //navmeshagent ışınlanma yerine sorun çıkarmaın diye yazıldı
            player.GetComponent<NavMeshAgent>().enabled = false;
            player.transform.position = otherPortal.spawnPoint.position;
            //player sahneye göre bakması gereken yöne bakıyor
            player.transform.rotation = otherPortal.spawnPoint.rotation;
            player.GetComponent<NavMeshAgent>().enabled = true;

            
        }

        //ışınlanılacak portalı bulan fonksiyon
        private Portal GetOtherPortal()
        {
            //sahne başlarken 2 adet portal var birisi girilen portal diğeri çıkılan portal
            //2 portalda tespit ediliyor eğer girilen portal ise birşey yapma devam et
            //girilen portal değilse ve portal enumları uyuşuyorsa portalı döndür
            foreach(Portal portal in FindObjectsOfType<Portal>())
            {
                if(portal == this) continue;
                if(destination != portal.destination) continue;

                return portal;
            }
            return null;
        }
    }
}