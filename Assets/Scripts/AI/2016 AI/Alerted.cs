using UnityEngine;
using System.Collections;

public class Alerted : MonoBehaviour {

    public AudioSource IntenseSource;
    public AudioSource LessIntenseSource;
    public AudioClip Intense;
    public AudioClip LessIntense;

	// Use this for initialization
	void Start () {
        LessIntenseSource.clip = LessIntense;
        LessIntenseSource.Play();
        IntenseSource.clip = Intense;
        IntenseSource.Play();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void intense()
    {
        LessIntenseSource.mute = true;
        IntenseSource.mute = false;

    }

    public void lessintense()
    {
        IntenseSource.mute = true;
        LessIntenseSource.mute = false;

    }

    public void stopAudio()
    {
        LessIntenseSource.Stop();
        IntenseSource.Stop();
    }
}
