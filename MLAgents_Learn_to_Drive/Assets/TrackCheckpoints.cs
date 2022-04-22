using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TrackCheckpoints : MonoBehaviour
{
    public event EventHandler<CarCheckPointEventArgs> OnCarCorrectCheckpoint;
    public event EventHandler<CarCheckPointEventArgs> OnCarWrongCheckpoint;

    public class CarCheckPointEventArgs : EventArgs
    {
        public Transform carTransform { get; set; }
    }
    //public delegate void OnPlayerCorrectCheckpoint(object sender, CarCheckPointEventArgs e);
    [SerializeField] private List<Transform> carTransformList;
    private List<CheckpointSingle> checkpointSingleList;
    private List<int > nextCheckpointSingleIndexList;

    // Start is called before the first frame update
    private void Awake()
    {
        checkpointSingleList = new List<CheckpointSingle>();
        //Transform checkpointsTransform = transform.Find("Checkpoints");
        foreach(Transform checkpointSingleTransform in transform)
        {
            CheckpointSingle checkpointSingle = checkpointSingleTransform.GetComponent<CheckpointSingle>();
            checkpointSingle.SetTrackCheckpoints(this);
            checkpointSingleList.Add(checkpointSingle);
        }
        nextCheckpointSingleIndexList = new List<int>();
        foreach (Transform carTransform in carTransformList)
            nextCheckpointSingleIndexList.Add(0);
    }

    public void CarThroughCheckpoint(CheckpointSingle checkpointSingle, Transform carTransform)
    {
        int nextCheckpointSingleIndex = nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform)];
        CarCheckPointEventArgs e = new CarCheckPointEventArgs();
        e.carTransform = carTransform;
        if (checkpointSingleList.IndexOf(checkpointSingle) == nextCheckpointSingleIndex)
        {
            nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform)] = (nextCheckpointSingleIndex + 1) % checkpointSingleList.Count;
            OnCarCorrectCheckpoint?.Invoke(this, e);
        }
        else
        {
            OnCarWrongCheckpoint?.Invoke(this, e);

        }
    }
    public void ResetCheckpoint(Transform carTransform)
    {
        nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform)] = 0;
    }
    public CheckpointSingle GetNextCheckpoint(Transform carTransform)
    {
        return checkpointSingleList[nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform)]];
    }
}
