using RPG.Combat;
using RPG.Movement;
using UnityEngine;
using RPG.Attributes;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        //CursorMaping structer ı
        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float maxNavMeshProjectionDistance = 1f;
        [SerializeField] float maxNavPathLength = 40f;

        Health health;
        private void Awake()
        {
            health = GetComponent<Health>();
        }
        
        void Update()
        {
            //UI ile etkileşim
            //if (InteractWithUI()) return;

            //Ölüyse cursor.non yap 
            if (health.IsDead())
            {
                SetCursor(CursorType.None);
                return;
            }

            //Biri çalışırken diğeri çalışamıyor "if(fonksiyon()) return;"
            if(InteractWithComponent()) return;
            if(InteractWithMovement()) return;

            //Cursor hiçbirşey yapılamaz tipinde gösteriliyor
            SetCursor(CursorType.None);

        }

        //UI ile etileşime girildiğinde Cursor değiştiren fonksiyon
        private bool InteractWithUI()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                SetCursor(CursorType.UI);
                return true;
            }
            return false;
        }

        //CombatTarget ve Fighter.cs ile etkileşime girdiğimiz fonksiyon
        private bool InteractWithComponent()
        {
            //Duvar arkasında kalan düşmanlarada saldıra bilmek için
            //Ray'in değdiği bütün nesneleri alıyoruz
            RaycastHit[] hits = RaycastAllSorted();
            foreach(RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    //GetComponent<Fighter>().Attack(target.gameObject);
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
                //Cursor Saldırı tipinde gösteriliyor
                //SetCursor(CursorType.Combat);
                //return true;
            }
            return false;
        }

        //Raycast in değdiği olayları sıralayan fonksiyon
        RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            float[] distances = new float[hits.Length];
            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }
            Array.Sort(distances, hits);
            return hits;
        }

        //Mover.cs'le etkileşime girdiğimiz fonksiyon
        private bool InteractWithMovement()
        {
            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);

            //eğer bir collider temsı varsa 
            if (hasHit)
            {
                //eğer mouse sol tuşuna basılıyorsa
                if (Input.GetMouseButton(0))
                {
                    //Fonksiyonu Mover.cs den çağırıyoruz
                    GetComponent<Mover>().StartMoveAction(target, 1f);
                }
                //Cursor Hareket tipinde gösteriliyor
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();

            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (!hasHit) return false;

            NavMeshHit navMeshHit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(
                hit.point, out navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);
            if (!hasCastToNavMesh) return false;

            target = navMeshHit.position;

            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);
            if (!hasPath) return false;
            if (path.status != NavMeshPathStatus.PathComplete) return false;
            if (GetPathLength(path) > maxNavPathLength) return false;

            return true;
        }

        private float GetPathLength(NavMeshPath path)
        {
            float total = 0;
            if (path.corners.Length < 2) return total;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }

            return total;
        }

        //Cursor şeklini değiştiren fonksiyon
        private void SetCursor(CursorType type)
        {
            //Tipe göre cursor mapi oluşturuldu
            CursorMapping mapping = GetCursorMapping(type);
            //Cursor şekli değiştirildi
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        //CursorMap'i oluşturan fonksiyon
        private CursorMapping GetCursorMapping(CursorType type)
        {
            //Enum ile oluşturulan bütün mapingler için
            foreach (CursorMapping mapping in cursorMappings)
            {
                //Doğru tipte maping ise
                if (mapping.type == type)
                {
                    //maping i döndür
                    return mapping;
                }
            }
            //doğru maping yoksa movement mapingi döndür
            return cursorMappings[0];
        }


        //Ray'i birden fazla çağıracağımız için bir fonksiyon haline getirdik
        private static Ray GetMouseRay()
        {
            //Ray Ray in temas bilgisi ve temas eedip etmeme bool u
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }

}