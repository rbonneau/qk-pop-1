﻿#pragma warning disable 414     //Variable assigned and not used: instance

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * AI Path
 * 
 * This script is used to "draw" checkpoints on a navmesh to make a path.
 * This path is then used by the AI script on an entity. The path constitute
 * of a list of gameobjects, which display an icon. It then returns a list of
 * vector3 for the AI to use.
 */

public class AIPath : MonoBehaviour {

	public List<GameObject> checkpoints;
	public int[] types = {0, 1, 2, 3};

	// temporary
	public int PathType = 0;
	public int NofLoops = 0;
    private GameObject instance;


    public void Awake()
    {
        instance = Resources.Load("checkpoint") as GameObject;
    }
	public void addCheckpoint(GameObject new_point)
	{
		Checkpoint c = new_point.GetComponent<Checkpoint>();
		c.path_reference = this;
		checkpoints.Add(new_point);
	}

	public void clearList(){
		foreach (GameObject g in checkpoints) {
			DestroyImmediate(g);
		}
		checkpoints.Clear();
	}

	public List<Vector3> getPoints(){
		List<Vector3> points = new List<Vector3>();

		foreach(GameObject c in checkpoints){

			if(c == null){
				checkpoints.Remove(c);
			}
			else{
				Checkpoint data = c.GetComponent<Checkpoint>();
				points.Add(data.getPosition());
			}
		}
		return points;
	}

    public List<Quaternion> getRotations()
    {
        List<Quaternion> rotations = new List<Quaternion>();
        foreach (GameObject r in checkpoints)
        {

            if (r == null)
            {
                checkpoints.Remove(r);
            }
            else
            {
                Checkpoint data = r.GetComponent<Checkpoint>();
                rotations.Add(data.getRotation());
            }
        }
        return rotations;
    }

    public List<bool> getSearch()
    {
        List<bool> search = new List<bool>();

        foreach (GameObject s in checkpoints)
        {

            if (s == null)
            {
                checkpoints.Remove(s);
            }
            else
            {
                Checkpoint data = s.GetComponent<Checkpoint>();
                search.Add(data.getSearch());
            }
        }
        return search;
    }

    public List<float> getMinAngle()
    {
        List<float> minAngle = new List<float>();
        foreach (GameObject minA in checkpoints)
        {
            if (minA == null)
            {
                checkpoints.Remove(minA);
            }
            else
            {
                Checkpoint data = minA.GetComponent<Checkpoint>();
                minAngle.Add(data.getMinAngle());
            }
        }
        return minAngle;
    }

    public List<float> getMaxAngle()
    {
        List<float> maxAngle = new List<float>();
        foreach (GameObject maxA in checkpoints)
        {
            if (maxA == null)
            {
                checkpoints.Remove(maxA);
            }
            else
            {
                Checkpoint data = maxA.GetComponent<Checkpoint>();
                maxAngle.Add(data.getMaxAngle());
            }
        }
        return maxAngle;
    }

    public List<float> getTurnSpeed()
    {
        List<float> turnSpeed = new List<float>();
        foreach (GameObject ts in checkpoints)
        {
            if (ts == null)
            {
                checkpoints.Remove(ts);
            }
            else
            {
                Checkpoint data = ts.GetComponent<Checkpoint>();
                turnSpeed.Add(data.getTurnSpeed());
            }
        }
        return turnSpeed;
    }

    public List<int> getLoopCount()
    {
        List<int> loopCount = new List<int>();
        foreach (GameObject lc in checkpoints)
        {
            if (lc == null)
            {
                checkpoints.Remove(lc);
            }
            else
            {
                Checkpoint data = lc.GetComponent<Checkpoint>();
                loopCount.Add(data.getLoopCount());
            }
        }
        return loopCount;
    }

    void OnDrawGizmosSelected()
	{
		List<Vector3> points = getPoints();
		int length = points.Count;
		Gizmos.color = Color.red;
		
		for (int i = 0; i < length; i++) {
			if(i-1 >= 0){
				Vector3 start = points[i-1];
				Vector3 end = points[i];
				Gizmos.DrawLine(start, end);
			}
		}
	}
}
