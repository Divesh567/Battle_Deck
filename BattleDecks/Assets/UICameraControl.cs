using UnityEngine;

public class UICameraControl : MonoBehaviour
{
    public Camera cam;
    public static UICameraControl Instance;

    private void Awake()
    {
        Instance = this;
    }
}
