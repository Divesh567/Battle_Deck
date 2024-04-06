using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyView : MonoBehaviour
{
   public void Initialize()
   {
        
   }


    public void KillEnemey()
    {
        Destroy(this.gameObject);
    }
}
