using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        Experience experience;

        //Start metotlarından önce can hesaplansın diye Awake de yaptık
        private void Awake()
        {
            //Player dan experience componentini aldık
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }

        private void Update()
        {
            //Experience verisi isteidğmiiz formatta ekrandaki text e yazdırdık
            GetComponent<Text>().text = String.Format("{0:0}", experience.GetPoints());
        }
    }
}