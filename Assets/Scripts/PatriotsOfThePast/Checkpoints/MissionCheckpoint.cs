﻿using UnityEngine;
using System.Collections;

[RequireComponent (typeof (SphereCollider))]
public class MissionCheckpoint : Placeholder {
    public string mission = ""; //this string is for to check for a completed pre-requisite quest
    public float minDist = 0.0f; //this is the minimum distance away from the Checkpoint that CheckpointTrigger needs to be

    void OnDrawGizmos() {
        Gizmos.DrawIcon (transform.position, "checkpointGizmo.png");
    }
}