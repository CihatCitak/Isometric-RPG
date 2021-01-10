using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        BaseStats baseStats;

        //Start metotlarından önce can hesaplansın diye Awake de yaptık
        private void Awake()
        {
            //Player dan BaseStats componentini aldık
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        }

        private void Update()
        {
            //BaseStats dan Level verisi isteidğmiz formatta ekrandaki text e yazdırdık
            GetComponent<Text>().text = String.Format("{0:0}", baseStats.GetLevel());
        }
    }
}