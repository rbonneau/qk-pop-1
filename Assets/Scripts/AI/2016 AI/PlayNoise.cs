using UnityEngine;
using System.Collections;

public class PlayNoise : MonoBehaviour
{
    private bool countdown = false;
    private float timer = 1f;

    // Use this for initialization
    void Start()
    {
        this.GetComponent<SphereCollider>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (countdown == true)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                this.GetComponent<SphereCollider>().enabled = false;
                countdown = false;
                timer = 5;
            }
        }

    }
    void Sound()
    {
        this.GetComponent<SphereCollider>().enabled = true;
        countdown = true;
    }
}

