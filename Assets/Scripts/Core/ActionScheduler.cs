using UnityEngine;

namespace RPG.Core
{
    public class ActionScheduler : MonoBehaviour
    {
        IAction currentAction;
        //Bir action(Saldırı, Yürüme...) yapılmak istendiğinde o an yapılan
        //action iptal edilip yenisi devreye giriyor
        public void StartAction(IAction action)
        {
            //eğer yeni gelen aksiyon eski aksiyon ile aynıysa(class bazında) birşey yapma
            if(currentAction == action) return;

            //güncel aksiyon nulldan farklıysa Aksiyonu iptal ediyoruz
            if(currentAction != null)
            {
                currentAction.Cancel();
            }
            currentAction = action;

        }

        //Ölüm durumunda bir aksiyon yapılmaması için kullacağız
        public void CancelCurrentAction()
        {
            //aksiyon yapılmsaını engelliyoruz
            StartAction(null);
        }
    }
}