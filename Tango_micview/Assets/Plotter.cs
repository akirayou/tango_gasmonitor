using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plotter : MonoBehaviour {
    private float timeOut = 1;
    private float timeElapsed=0;
    public GameObject Indicator;
    public MicSens Sensor;
    private Vector3 sensorPos = new Vector3(0, -0.07f, 0.315f);
    // Use this for initialization
    void Start () {
		
	}

    private void plot()
    {
        float v = Sensor.Value;
        v = Mathf.Pow(v, 0.25f);
        GameObject ind = Instantiate(Indicator);
        ind.transform.position= Camera.main.transform.position + (Camera.main.transform.rotation * sensorPos);
        ind.GetComponent<Renderer>().material.color = Color.HSVToRGB(0.75f - v * 0.75f, 1, 1);

    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= timeOut)
        {
            plot();
            timeElapsed = 0;
        }
    }
}
