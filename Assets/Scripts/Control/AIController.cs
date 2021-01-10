using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
using GameDevTV.Utils;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        //Player a görüş menzili değişkeni
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionTime = 3f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float wayPointTolerance = 1f;
        [SerializeField] float waypointDwellTime = 3f;
        [Range(0,1)]//değeri 0 ile 1 arasında tutuyor
        [SerializeField] float patrolSpeedFraction = 0.2f;

        Health health;
        Fighter fighter;
        GameObject player;
        Mover mover;

        LazyValue<Vector3> guardPosition;

        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        int currentWaypointIndex = 0;

        private void Awake() 
        {
            health = GetComponent<Health>();
            fighter = GetComponent<Fighter>();
            player = GameObject.FindWithTag("Player");
            mover = GetComponent<Mover>();

            //Race Condition önlemek için yapılıyor
            guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }

        //LazyValue ataması için kullanılan fonksiyon
        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        private void Start() 
        {
            //sahneye başlangıç noktasını koruma noktası olarak alıyoruz
            //LazyValue çalışması için zorunlu kod
            guardPosition.ForceInit();
        }

        private void Update()
        {

            //Ölüyse hiçbirşey yapma
            if (health.IsDead()) return;

            //Player menzildeyse ve saldırıla bilir durumdaysa
            if (InAttackRangeOfPlayer() && fighter.CanAttack(player))
            {
                AttackBehaviour();
            }
            //eğer düşmanın son görülmesinden bu yana "3sn(suspicionTime)" kadar süre geçmemişse
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehaviour();

            }
            //düşmanı son gördükten bu yana "3sn(suspicionTime)" kadar süre geçmemişse
            else
            {
                PatrolBehaviour();
            }

            UpdateTimers();

        }

        private void UpdateTimers()
        {
            //düşmanı son görme zamanını sayıyoruz
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
        }

        //Patrol yapmayı sağlayan fonksiyon
        void PatrolBehaviour()
        {
            Vector3 nextPosition = guardPosition.value;

            if(patrolPath != null)
            {
                if(AtWaypoint())
                {
                    timeSinceArrivedAtWaypoint = 0;
                    CycleWayPoint();
                }
                nextPosition = GetCurrentWayPoint();
            }

            
            if(timeSinceArrivedAtWaypoint > waypointDwellTime)
            {
                mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            }
        }

        //Waypointe ulaşılıp ulaşılmadığını ölçen fonksiyon
        private bool AtWaypoint()
        {
            //Waypoint e 1m(1f) kalınca false döndürüyor
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWayPoint());
            return distanceToWaypoint < wayPointTolerance;
        }

        //Enson gidilen noktadan bir sonraki noktayı tespit eden fonksiyon
        private void CycleWayPoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        //gidilecek noktanın kordinatını döndüren fonksiyon
        private Vector3 GetCurrentWayPoint()
        {
            return patrolPath.GetWayPoint(currentWaypointIndex);
        }

        //Son Aksiyonu iptal et
        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        //Player a saldır
        private void AttackBehaviour()
        {
            //düşmanı görme zamanını sıfırladık
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);
        }

        //Enemy ile player arasındaki mesafe ölçülüyor
        private bool InAttackRangeOfPlayer()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            return distanceToPlayer < chaseDistance;

        }

        //kendi gizmozumuzu çiziyoruz Unity bu fonksiyonu kendisi çağırıyor
        private void OnDrawGizmosSelected() 
        {
            //Enemy etrafına chaseDistance çağında mavi bir silindir çizdik
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position,chaseDistance);
        }
    }

}