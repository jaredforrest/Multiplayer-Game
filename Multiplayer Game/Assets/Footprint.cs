using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footprint : MonoBehaviour
{
    public int destroyAfter;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, destroyAfter);
    }
 
}
