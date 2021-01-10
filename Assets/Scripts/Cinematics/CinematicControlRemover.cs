using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using RPG.Core;
using RPG.Control;

namespace RPG.Cinematics
{
    public class CinematicControlRemover : MonoBehaviour 
    {
        GameObject player;

        private void Awake() 
        {
            //Player'ı aldık
            player = GameObject.FindWithTag("Player");
        }

        //Aktifken çalışan fonksiyon Çalışma sırasında Start'tan geride
        private void OnEnable()
        {
            //Bu evente bir nesnenin yaşam döngüsünde çalışması 
            //için 2 adet fonksiyon başlığı verdik
            //"played" nesne ile temas olduğu anda(nesne stack e girince) fonksiyonu çağırır
            //stoped nesne stack ten çıkınca yani nesnenin yaptığı işlem bitince fonksiyonu çağırır
            GetComponent<PlayableDirector>().played += DisableControl;
            GetComponent<PlayableDirector>().stopped += EnableControl;            
        }

        //Deaktifken çalışan fonksiyon Çalışma sırasında Start'tan geride
        private void OnDisable()
        {
            //Bu evente bir nesnenin yaşam döngüsünde çalışması 
            //için verdiğimiz 2 adet fonksiyon başlığını geri aldık
            //"played" nesne ile temas olduğu anda(nesne stack e girince) fonksiyonu çağırır
            //stoped nesne stack ten çıkınca yani nesnenin yaptığı işlem bitince fonksiyonu çağırır
            GetComponent<PlayableDirector>().played -= DisableControl;
            GetComponent<PlayableDirector>().stopped -= EnableControl;
        }

        void DisableControl(PlayableDirector pd)
        {
            //son eylemi iptal ettik
            player.GetComponent<ActionScheduler>().CancelCurrentAction();
            //animasyon esnasında Player'ı Control edemeyelim diye script'i deaktif ettik
            player.GetComponent<PlayerController>().enabled = false;
        }

        void EnableControl(PlayableDirector pd)
        {
            player.GetComponent<PlayerController>().enabled = true;
        }
    }
}
