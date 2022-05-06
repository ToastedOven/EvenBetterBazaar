using EmotesAPI;
using ExamplePlugin;
using R2API.Networking.Interfaces;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace Funland
{
    class VengaBus : MonoBehaviour
    {
        public GameObject[] seats = new GameObject[17];
        public WheelCollider FL, FR, BL, BR;
        internal GameObject joinSpot;

        void Start()
        {
            FunlandPlugin.Venga = this;
            gameObject.GetComponent<Rigidbody>().centerOfMass = new Vector3(0, -0.9f, 0);
            List<GameObject> seatObjects = new List<GameObject>();
            for (int i = 0; i < 17; i++)
            {
                seatObjects.Add(gameObject.transform.GetChild(i).gameObject);
            }
            FR = gameObject.GetComponentsInChildren<WheelCollider>()[0];
            FL = gameObject.GetComponentsInChildren<WheelCollider>()[1];
            BR = gameObject.GetComponentsInChildren<WheelCollider>()[2];
            BL = gameObject.GetComponentsInChildren<WheelCollider>()[3];
            foreach (var item in gameObject.GetComponentsInChildren<Renderer>())
            {
                item.material.shader = FunlandPlugin.defaultShader;
                if (item.name == "windows")
                {
                    item.material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Brother/maBrotherGlassOverlay.mat").WaitForCompletion();
                }
            }

            seats = seatObjects.ToArray();
            joinSpot = gameObject.GetComponentInChildren<EmoteLocation>().gameObject;

            explosionSource = new GameObject();
            explosionSource.transform.SetParent(transform);
            explosionSource.transform.localPosition = new Vector3(0, 0, 2.851f);
        }

        public void JoinBus(BoneMapper joiner)
        {
            for (int i = 0; i < seats.Length; i++)
            {
                if (seats[i].transform.childCount == 0) //if seat is not taken
                {
                    GameObject seat = new GameObject();
                    seat.transform.SetParent(seats[i].transform);
                    seat.transform.localPosition = Vector3.zero;
                    seat.transform.localEulerAngles = new Vector3(90, 0, 0);
                    joiner.props.Add(seat);
                    joiner.AssignParentGameObject(seat, true, true, true);

                    if (i == seats.Length - 1)
                    {
                        joinSpot.transform.localPosition = new Vector3(0, -5000, 0);
                    }
                    else
                    {
                        joinSpot.transform.localPosition = new Vector3(1.741f, 0, 3.534f);
                    }
                    break;
                }
            }
        }
        GameObject explosionSource;
        internal bool hasControl = false;
        void FixedUpdate()
        {
            int motorTorque = 0;
            int brakeTorque = 0;
            hasControl = seats[0].transform.childCount != 0 && CustomEmotesAPI.localMapper && CustomEmotesAPI.localMapper.parentGameObject == seats[0].transform.GetChild(0).gameObject;
            if (Input.GetKey(KeyCode.W) && hasControl)
            {
                motorTorque += 8000;
            }
            if (Input.GetKey(KeyCode.S) && hasControl)
            {
                motorTorque -= 8000;
            }
            FL.motorTorque = motorTorque;
            FR.motorTorque = motorTorque;
            if (Input.GetKey(KeyCode.Space) && hasControl)
            {
                brakeTorque = 50;
            }
            FL.brakeTorque = brakeTorque;
            FR.brakeTorque = brakeTorque;
            BL.brakeTorque = brakeTorque;
            BR.brakeTorque = brakeTorque;
            int num = 0;
            if (Input.GetKey(KeyCode.A) && hasControl)
            {
                num -= 45;
            }
            if (Input.GetKey(KeyCode.D) && hasControl)
            {
                num += 45;
            }
            FL.steerAngle = num;
            FR.steerAngle = num;

            //UpdateWheel(FR, FR.transform);
            //UpdateWheel(FL, FL.transform);
            //UpdateWheel(BR, BR.transform);
            //UpdateWheel(BL, BL.transform);


            new BlastAttack
            {
                attacker = base.gameObject,
                damageColorIndex = DamageColorIndex.Item,
                baseDamage = 9999999,
                radius = 2.3f,
                falloffModel = BlastAttack.FalloffModel.None,
                procCoefficient = 1,
                teamIndex = TeamIndex.Player,
                damageType = DamageType.AOE,
                position = explosionSource.transform.position,
                baseForce = 0,
                attackerFiltering = AttackerFiltering.NeverHitSelf
            }.Fire();
        }
        void Update()
        {

            if (hasControl)
            {
                new SyncBusToServer(transform.GetComponent<NetworkIdentity>().netId, transform.position, transform.localEulerAngles, transform.GetComponent<Rigidbody>().velocity).Send(R2API.Networking.NetworkDestination.Server);
            }
            else if (seats[0].transform.childCount == 0 && NetworkServer.active)
            {
                new SyncBusToServer(transform.GetComponent<NetworkIdentity>().netId, transform.position, transform.localEulerAngles, transform.GetComponent<Rigidbody>().velocity).Send(R2API.Networking.NetworkDestination.Server);
            }
        }
        void UpdateWheel(WheelCollider collider, Transform wheelTransform)
        {
            Vector3 pos;
            Quaternion rot;
            collider.GetWorldPose(out pos, out rot);
            wheelTransform.rotation = rot;
            wheelTransform.position = pos;
        }
    }
}
