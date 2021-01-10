using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Control;

namespace RPG.Combat
{
    public class WeaponPickups : MonoBehaviour, IRaycastable
    {
        [SerializeField] Weapon weapon = null;
        [SerializeField] float respawnTime = 5;

        private void OnTriggerEnter(Collider other) 
        {
            if(other.gameObject.tag.Equals("Player"))
            {
                Pickup(other.GetComponent<Fighter>());
            }
        }

        //5 sn de bir pickup nesnelerini geri getiriyoruz
        private IEnumerator HideForSeconds(float seconds)
        {
            ShowPickUp(false);
            yield return new WaitForSeconds(seconds);
            ShowPickUp(true);
        }

        //Pickup nesnelerini gösteren yada kaybeden fonksiyon
        private void ShowPickUp(bool shouldShow)
        {
            //Dış collider a işlem yapılıyor
            GetComponent<Collider>().enabled = shouldShow;
            //alt nesne olan silah görünümüne işlem yapılıyor
            foreach(Transform child in transform)
            {
                child.gameObject.SetActive(shouldShow);
            }
        }

        //yerden alma fonksiyonu
        private void Pickup(Fighter fighter)
        {
            //yerden alınan silah kuşanıldı
            fighter.EquipWeapon(weapon);
            //yerden alınan silah bir müddet gözden kayboldu
            StartCoroutine(HideForSeconds(respawnTime));
        }
        
        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }
        
        //IrayCastable Interface fonksiyonu
        public bool HandleRaycast(PlayerController callingController)
        {
            //eğer left mouse a tıklanmışsa
            if (Input.GetMouseButtonDown(0))
            {
                //Nesneyi yerden al
                Pickup(callingController.GetComponent<Fighter>());
            }
            return true;
        }
    }

}