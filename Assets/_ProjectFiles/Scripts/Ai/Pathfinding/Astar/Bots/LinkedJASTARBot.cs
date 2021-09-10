using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkedJASTARBot : MonoBehaviour
{
    public float speed;

    public float StartDelay;
    private float startdelay;

    public List<LinkedAPoint> WayPoints;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        startdelay -= Time.deltaTime;

        if (startdelay <= 0)
        {

        }
    }



}
