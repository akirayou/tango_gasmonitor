using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;
public class MicSens : MonoBehaviour {
    AudioSource audioSource;
    string deviceName = "";
    public Text sensValueText;
    public LineRenderer graph;
    public float Value = 0;
    const int bufLen = 2048;
    // Use this for initialization
    const int SampleRate = 10000;
    const int SampleTime = 100;
    void Start () {
        audioSource = this.GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.clip = Microphone.Start(deviceName, true, SampleTime, SampleRate);
        while (!(Microphone.GetPosition(deviceName) > 0)) { } // ※２これを待たずにPlayしても失敗する
        audioSource.Play();
    }
    int count = 0;
    int med_count = 0;
    int[] med = new int[5];
    int[] sb = new int[5];
    float maxValue = 1.0f;
    // Update is called once per frame

    int nextPos = 0;
    void Update () {
        count++;

        var tmp = new float[bufLen];
        int pos = Microphone.GetPosition(deviceName);

        //must be nextPos+bufLen < pos  but, pos is cyclic
        if (pos < nextPos - bufLen)
        {
            if (pos + audioSource.clip.samples - nextPos - bufLen < 0) return;
        }
        else
        {
            if (pos - nextPos - bufLen < 0) return;
        }
        //for In case of buffer over flow
        if (pos - nextPos > bufLen * 3) nextPos = pos - bufLen;

        //get nextData from nextPos
        audioSource.clip.GetData(tmp,nextPos );

        int state = 0;
        int skipCount = 0;


        float level = 0.25f*maxValue;
        
        const int span = 20;//1ms@10kHz
        const int skipLen = 80;//8ms @10kHz
        if(graph != null)graph.positionCount = bufLen - span;
        float lpf = tmp[0];
        int start = -1;
        int end = -1;
        float tmpMax = -1;
        for (int i = 0; i < bufLen-span; i++)
        {
            lpf = tmp[i] * 0.05f + 0.95f * lpf;
            tmp[i] -= lpf;
            //tmp[i] *= -1;
            float absp = Mathf.Abs(tmp[i] + tmp[i + 1]);
            if (tmpMax < absp) tmpMax =absp ;


            if (skipCount != 0) skipCount--;
            if (state == 0)
            {
                if (tmp[i] > level)
                    for (int j = i; j < i + 10; j++) if (tmp[j] < -level) { state = 1; start = i; skipCount = skipLen; break; }
            }
            if (start < span*3) { start = -1;state = 0; }
            if (state == 1 && skipCount==0)
            {
                if (tmp[i] > level)
                    for (int j = i; j < i + 10; j++) if (tmp[j] < -level) { state = 2;end = i; skipCount = skipLen; break; }
            }
            if (graph ==null && state == 2) break;    
        }

        if(graph!=null)for (int i = 0; i < bufLen - span; i++)
        {
            int s = 0;
            if (start < i &&  i<end) s = 1;
            graph.SetPosition(i, new Vector3(0.8f * (float)(i-start) / bufLen - 0.4f, tmp[i] * 0.2f + s * 0.2f, 0.0f));
            
        }

        maxValue = maxValue * 0.8f + tmpMax * 0.2f;
        int val=end - start;
        val -= 10 * 10;//10ms is base time
        if (val < 0) val = 0;
        if (end < 0) val = -1;
        //Debug.Log("Estimate value is " + val.ToString());

        if (val > -1)
        {
            nextPos = nextPos + end-span;
            med_count++;
            med[med_count % med.Length] = val;
            Array.Copy(med, sb, med.Length);
            Array.Sort(sb);
            int mv=sb[sb.Length / 2];


            float s = (900-mv)/ 900.0f;
            if (s < 0) s = 0;
            if (1 < s) s = 1;
            Value = s;
            if(sensValueText != null)
            {
                sensValueText.text = ((int)( Value * 1000)).ToString();
            }
        }
        else
        {
            nextPos = nextPos + bufLen;
        }
        if (nextPos > audioSource.clip.samples) nextPos -= audioSource.clip.samples;



    }
}
