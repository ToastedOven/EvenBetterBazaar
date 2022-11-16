using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Funland.EmoteSkeletonHanders
{
    internal class EmoteSkeletonGiver : MonoBehaviour
    {
        struct masterObjectPair
        {
            public masterObjectPair(CharacterMaster _master, GameObject _originalBody)
            {
                master = _master;
                originalBody = _originalBody;
            }
            public CharacterMaster master;
            public GameObject originalBody;
        }
        static List<masterObjectPair> masterObjectPairs = new List<masterObjectPair>();
        void Start()
        {
            On.RoR2.SceneCatalog.OnActiveSceneChanged += (orig, self, newscene) =>
            {
                if (NetworkServer.active)
                {
                    while (masterObjectPairs.Count != 0)
                    {
                        masterObjectPairs[0].master.bodyPrefab = masterObjectPairs[0].originalBody;
                        masterObjectPairs.RemoveAt(0);
                    }
                }
                orig(self, newscene);
            };
        }
        void Update()
        {

        }
        void OnTriggerEnter(Collider other)
        {
            if (NetworkServer.active)
            {
                BoneMapper mapper = other.gameObject.GetComponent<ModelLocator>().modelTransform.GetComponentInChildren<BoneMapper>();
                if (!mapper)
                {
                    GameObject bodyPrefab = RoR2Content.Survivors.Commando.bodyPrefab;
                    CharacterMaster master = other.GetComponentInChildren<CharacterBody>().master;
                    masterObjectPairs.Add(new masterObjectPair(master, master.bodyPrefab));
                    master.bodyPrefab = bodyPrefab;
                    master.DestroyBody();
                    master.SpawnBody(other.transform.position + new Vector3(0, 1, 0), other.transform.rotation);
                }
            }
        }
    }
}
