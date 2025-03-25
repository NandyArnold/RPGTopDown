using UnityEngine;

public class DontDestroyGeneral : MonoBehaviour
{

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

}
