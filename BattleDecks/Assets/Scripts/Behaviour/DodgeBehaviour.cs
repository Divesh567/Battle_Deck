using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Dodge", menuName = "Behaviour/Enemy/Dodge", order = 50)]
public class DodgeBehaviour : BehaviourSO
{

    [SerializeField] [Range(0,99)]
    private int _dodgeChance;

    public int dodgeChance => _dodgeChance;

    public bool CheckCanDodge()
    {
        int randomChance = Random.Range(0, 99);

        if (randomChance < dodgeChance)
        {
            
            return true;
        }

        return false;
    }
}

