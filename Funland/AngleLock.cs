using ExamplePlugin;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class AngleLock : MonoBehaviour
{
    public AimConstraint aimConstraint;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var item in transform.parent.GetComponentsInChildren<Renderer>())
        {
            item.material.shader = FunlandPlugin.defaultShader;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localRotation.x > .8f || transform.localRotation.x < 0f)
        {
            aimConstraint.constraintActive = false;
            aimConstraint.transform.localRotation = new Quaternion(.8f, aimConstraint.transform.localRotation.y, aimConstraint.transform.localRotation.z, aimConstraint.transform.localRotation.w);
            if (aimConstraint.transform.localRotation.w < 0f)
            {
                aimConstraint.transform.localRotation = new Quaternion(aimConstraint.transform.localRotation.x, aimConstraint.transform.localRotation.y, aimConstraint.transform.localRotation.z, aimConstraint.transform.localRotation.w * -1);
            }
        }
        else
        {
            aimConstraint.constraintActive = true;

        }
    }
}
