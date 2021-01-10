using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        Health health;

        //Start metotlarından önce can hesaplansın diye Awake de yaptık
        private void Awake()
        {
            //Player dan health componentini aldık
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
        }

        private void Update()
        {
            //Health verisi isteidğmiiz formatta ekrandaki text e yazdırdık
            GetComponent<Text>().text = String.Format("{0:0}/{1:0}", health.GetHealthPoints(), health.GetMaxHealthPoints());
        }
    }
}
