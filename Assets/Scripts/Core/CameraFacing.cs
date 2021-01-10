using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class CameraFacing : MonoBehaviour
    {
        void Update()
        {
            //CAnvas sürekli kamera ya bakıyor böylece oyuncu yazıları ters veya açılı görmüyor
            transform.forward = Camera.main.transform.forward;
        }
    }
}