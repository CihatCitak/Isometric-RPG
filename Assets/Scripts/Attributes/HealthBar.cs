using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Health healthComponent = null;
        [SerializeField] RectTransform foreground = null;
        [SerializeField] Canvas rootCanvas = null;

        void Update()
        {
            //eğer değer 0 ile 1(0.9999999) arasındaysa değer 1 ken yani
            //düşmana henüz saldırılmamışsa can barı gözükmüyor
            if (Mathf.Approximately(healthComponent.GetFraction(), 0)
            || Mathf.Approximately(healthComponent.GetFraction(), 1))
            {
                //değer sıfırdan düşük veya 1 den yüksekse healthbarı ekrandan kaldırıyoruz
                rootCanvas.enabled = false;
                return;
            }

            rootCanvas.enabled = true;
            //HealthBar Bardaki resmin(foreground) scale değerini cana göre ayarlıyarak gösterioruz
            foreground.localScale = new Vector3(healthComponent.GetFraction(), 1, 1);
        }
    }
}