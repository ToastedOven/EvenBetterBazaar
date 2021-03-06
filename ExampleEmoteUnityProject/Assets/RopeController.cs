using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeController : MonoBehaviour
{
    public
    GameObject ropeSegmentPrefab;
    public
    GameObject anchorPoint;
    public
    GameObject attachPoint;
    List<GameObject> ropeList = new List<GameObject>();
    // Start is called before the first frame update
    Vector3 direction;
    Vector3 currentPos;
    void Start()
    {
        int max = 1000;
        direction = (attachPoint.transform.position - anchorPoint.transform.position).normalized; //so go from anchor point to attach point for everything
        direction *= .2f;
        //Debug.Log($"--------------  {attachPoint}, distance is {Vector3.Distance(anchorPoint.transform.position, attachPoint.transform.position)} so it should theoretically only take {Vector3.Distance(anchorPoint.transform.position, attachPoint.transform.position) / .2f} segments");
        GameObject g1 = GameObject.Instantiate(ropeSegmentPrefab);
        g1.transform.parent = transform;
        ropeList.Add(g1);
        GameObject g2;
        CharacterJoint joint = anchorPoint.GetComponent<CharacterJoint>();
        joint.connectedBody = g1.GetComponent<Rigidbody>();
        currentPos = anchorPoint.transform.position + direction;
        g1.transform.position = currentPos;
        while (Vector3.Distance(currentPos, attachPoint.transform.position) > .2f && max > 0)
        {
            //Debug.Log($"max is: {max}   Distance to attach point is: {Vector3.Distance(currentPos, attachPoint.transform.position)}   current pos is: {currentPos}");
            g2 = GameObject.Instantiate(ropeSegmentPrefab);
            g2.transform.parent = transform;
            ropeList.Add(g2);
            g1.GetComponent<CharacterJoint>().connectedBody = g2.GetComponent<Rigidbody>();
            currentPos += direction;
            g2.transform.position = currentPos;
            g1 = g2;
            max--;
        }
        //Debug.Log($"TOO CLOSE, CURRENT DISTANCE IS: {Vector3.Distance(currentPos, attachPoint.transform.position)}");
        g2 = GameObject.Instantiate(ropeSegmentPrefab);
        g2.transform.parent = transform;
        ropeList.Add(g2);
        g1.GetComponent<CharacterJoint>().connectedBody = g2.GetComponent<Rigidbody>();
        float maff = (Vector3.Distance(currentPos, attachPoint.transform.position) / .2f);
        currentPos = anchorPoint.transform.position + (direction * (Vector3.Distance(currentPos, attachPoint.transform.position) / .2f));
        g2.transform.position = currentPos;
        g2.GetComponent<CharacterJoint>().connectedBody = attachPoint.GetComponent<Rigidbody>();
        g2.GetComponent<CharacterJoint>().anchor = new Vector3(0,-.1f * maff, 0);
        g2.GetComponent<CharacterJoint>().connectedAnchor = new Vector3(0, -.1f * maff, 0);
    }

    // Update is called once per frame
    void Update()
    {
        ropeList[0].GetComponent<LineRenderer>().SetPosition(0, ropeList[0].transform.position);
        ropeList[0].GetComponent<LineRenderer>().SetPosition(1, anchorPoint.transform.position);
        for (int i = 1; i < ropeList.Count; i++)
        {
            ropeList[i].GetComponent<LineRenderer>().SetPosition(0, ropeList[i].transform.position);
            ropeList[i].GetComponent<LineRenderer>().SetPosition(1, ropeList[i - 1].transform.position);
        }
    }
}
