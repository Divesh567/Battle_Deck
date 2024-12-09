using Unity;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Turn : MonoBehaviour
{
    public ITurnObject turnObject;

    [SerializeField]
    public Image playerImage;
    [SerializeField]
    private string playerName;

    
    
}