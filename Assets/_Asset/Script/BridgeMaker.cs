using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using XYH;

public class BridgeMaker : MonoBehaviour
{
    [System.NonSerialized]
    public AI_TYPE type;


    [System.NonSerialized]
    public Vector3 Begin;

    [System.NonSerialized]
    public Vector3 End;


    void OnTriggerEnter(Collider other)
    {
        BridgeMaker otherMaker = other.gameObject.GetComponent<BridgeMaker>();
        if (otherMaker && otherMaker.type == AI_TYPE.SHIP && IsEnableMaker())
        {
            Vector3 pos = other.ClosestPoint(transform.position);

            ResetPosition(otherMaker, ref pos);

            BridgeController.Instance.BuildNewBridge(type, pos, transform.rotation);

           // GetComponent<MeshRenderer>().enabled = true;
           // otherMaker.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    private bool IsEnableMaker()
    {
        return type == AI_TYPE.CAR || type == AI_TYPE.TRAIN; // airline is most high
    }

    private void ResetPosition(BridgeMaker InOtherMaker, ref Vector3 InPosition)
    {
        Vector2 begin_1 = new Vector2(Begin.x, Begin.z);
        Vector2 end_1 = new Vector2(End.x, End.z);

        Vector2 begin_2 = new Vector2(InOtherMaker.Begin.x, InOtherMaker.Begin.z);
        Vector2 end_2 = new Vector2(InOtherMaker.End.x, InOtherMaker.End.z);

        Vector2 result = Math.intersectionOf2Lines(begin_1, end_1, begin_2, end_2);

        InPosition.x = result.x;
        InPosition.z = result.y;
        InPosition.y += 15;
    }
}
