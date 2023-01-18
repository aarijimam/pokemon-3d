using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveMusic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

}
