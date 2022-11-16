using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Funland.EmoteSkeletonHanders
{
    internal class EmoteSkeletonChecker : MonoBehaviour
    {
        float timer = 0;
        void Start()
        {
            GetComponent<Renderer>().material.color = Color.white;
        }
        void Update()
        {
            if (timer < 0)
            {
                GetComponent<Renderer>().material.color = Color.white;
            }
            else
            {
                timer -= Time.deltaTime;
            }
        }
        void OnTriggerEnter(Collider other)
        {
            BoneMapper mapper = other.gameObject.GetComponent<ModelLocator>().modelTransform.GetComponentInChildren<BoneMapper>();
            if (mapper)
            {
                GetComponent<Renderer>().material.color = Color.green;
            }
            else
            {
                GetComponent<Renderer>().material.color = Color.red;
                other.gameObject.GetComponent<CharacterMotor>().velocity *= -2;
                AkSoundEngine.PostEvent("Play_NoEmoteSkeleton", gameObject);
            }
            timer = 1;
        }
    }
}
