using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] Transform target;
        void LateUpdate()
        {
            //Follow Camera gameobjesi sürekli player ı takip ediyor
            //Main Camera ise Follow Camera nın alt nesnesi olarak hareket ediyor
            transform.position = target.position;
        }

    }
}