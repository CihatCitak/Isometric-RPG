using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public class PatrolPath : MonoBehaviour
    {
        const float waypointGizmoRadius = 0.3f;

        private void OnDrawGizmos() 
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                //Waypointleri bir küp olarak çözdük ve aralarında çizgiler oluşturduk
                int j = GetNextIndex(i);
                Gizmos.DrawSphere(GetWayPoint(i), waypointGizmoRadius);
                Gizmos.DrawLine(GetWayPoint(i), GetWayPoint(j));
            }
            
        }

        //Waypoint ler ile bir döngü oluşturmak için bir sonraki indeksi döndüren fonksiyon
        public int GetNextIndex(int i)
        {
            //eğer bir sonraki waypoint yoksa ilk waypoint i gönder
            if(i + 1 == transform.childCount) return 0;
            return i + 1;
        }

        //girilen indeksin alt objesinin konumunu döndüren fonksiyon
        public Vector3 GetWayPoint(int i)
        {
            return transform.GetChild(i).position;
        }
    }
}
