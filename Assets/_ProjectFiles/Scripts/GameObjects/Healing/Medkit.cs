using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Medkit : MonoBehaviour
{
    public bool Used;

    public abstract LifeComponentEffect Use();
       
    // Заменить на пулл
    public void Destroy()
    {
        Destroy(gameObject);
    }
}
