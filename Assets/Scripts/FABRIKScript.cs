using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FABRIKScript : MonoBehaviour
{
    public List<GameObject> segments;
    public Transform target;
    LineRenderer lr;

    [Range(1, 40)]
    public int iterations = 10;
    [Range(0.001f, 1f)]
    public float accuracy = 0.001f;

    float[] segmentLengths;

    float fullLength;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = segments.Count;

        InitializeArm();
    }

    void Update()
    {
        ResolveIK();

        UpdateLine();
    }

    void InitializeArm()
    {
        segmentLengths = new float[segments.Count];

        for (int i = 1;  i < segments.Count; i++)
        {
            segmentLengths[i] = (segments[i - 1].transform.position - segments[i].transform.position).magnitude;
            fullLength += segmentLengths[i];
        }
    }

    void ResolveIK()
    {
        float sqrDistanceToTarget = (target.position - segments[0].transform.position).sqrMagnitude;
        if (sqrDistanceToTarget >= fullLength * fullLength)
        {
            //direction towards the target
            Vector3 dir = (target.position - segments[0].transform.position).normalized;


            for (int i = 1; i < segments.Count; i++)
            {
                segments[i].transform.position = segments[i - 1].transform.position + dir * segmentLengths[i - 1];
            }
        }
        else
        {
            for (int iteration = 0; iteration < iterations; iteration++)
            {
                BackwardPropagation();

                ForwardPropagation();

                //check if more iterations needed
                float sqdistance = (segments[segments.Count - 1].transform.position - target.position).sqrMagnitude;
                if (sqdistance < accuracy * accuracy)
                    break;
            }
        }
    }

    void BackwardPropagation()
    {
        //Backwards
        for (int i = segments.Count - 1; i > 0; i--)
        {
            if (i == segments.Count - 1)
            {
                segments[i].transform.position = target.position;
            }
            else
            {
                segments[i].transform.position = segments[i + 1].transform.position + (segments[i].transform.position - segments[i + 1].transform.position).normalized * segmentLengths[i];
            }
        }
    }

    void ForwardPropagation()
    {
        //Forwards
        for (int i = 1; i < segments.Count; i++)
        {
            segments[i].transform.position = segments[i - 1].transform.position + (segments[i].transform.position - segments[i - 1].transform.position).normalized * segmentLengths[i - 1];
        }
    }

    void UpdateLine()
    {
        for (int i = 0; i < segments.Count; i++)
        {
            lr.SetPosition(i, segments[i].transform.position);
        }
    }
}
