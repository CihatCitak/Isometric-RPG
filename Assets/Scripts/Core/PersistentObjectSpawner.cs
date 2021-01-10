using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class PersistentObjectSpawner : MonoBehaviour
    {
        //Spawn lanacak obje ve daha önce spawnlandımı bool u
        [SerializeField] GameObject persistentObjectPrefab;
        static bool hasSpawne = false;

        private void Awake() 
        {
            //eğer daha önce spawn olduysa birşey yapma
            if(hasSpawne) return;
            
            //spawnla
            SpawnPersistentObjects();
            //spawn olduğunu belirt
            hasSpawne = true;
        }

        private void SpawnPersistentObjects()
        {
            //Prefabları gameobjeye dönüştürdük
            GameObject persistentObject = Instantiate(persistentObjectPrefab);
            //prefablar sahne geçiş esnasından yok olmasın diye komut verdik
            DontDestroyOnLoad(persistentObject);
        }
    }
}
