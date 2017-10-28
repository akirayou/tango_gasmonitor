using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suicide : MonoBehaviour {
    private float lifeTime = 0;
    // Use this for initialization
    void Start ()
    {
        Color col=GetComponent<MeshRenderer>().material.color;
        col.a = 0.4f;
        GetComponent<Renderer>().material.color = col;

    }
	
	// Update is called once per frame
	void Update () {
        lifeTime += Time.deltaTime;

        if (lifeTime > 33)
        {
            Destroy(this.gameObject);
        }else if (lifeTime > 28)
        {

            Color col = this.GetComponent<Renderer>().material.color;
            col.a = 0.4f * (1f-(lifeTime - 28f) / (33-28));
            this.GetComponent<Renderer>().material.color = col;

        }
		
	}
}
