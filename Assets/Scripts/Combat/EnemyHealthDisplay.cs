using System;
using RPG.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Fighter fighter;

        //Start metotlarından önce can hesaplansın diye Awake de yaptık
        private void Awake()
        {            
            //Player dan Fighter componentini aldık
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }

        private void Update()
        {
            //bir hedef yoksa
            if (fighter.GetTarget() == null)
            {
                //herhangi bir can değeri gösterme
                GetComponent<Text>().text = "N/A";
                return;
            }
            //hedefin health componentini aldık ve ekranda gösterdik
            Health health = fighter.GetTarget();
            GetComponent<Text>().text = String.Format("{0:0}/{1:0}", health.GetHealthPoints(), health.GetMaxHealthPoints());
        }
    }
}