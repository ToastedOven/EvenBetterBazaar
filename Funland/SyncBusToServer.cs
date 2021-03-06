using Funland;
using R2API.Networking.Interfaces;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;


class SyncBusToServer : INetMessage
{
    NetworkInstanceId netId;
    Vector3 position;
    Vector3 rotation;
    Vector3 velocity;
    public SyncBusToServer()
    {

    }

    public SyncBusToServer(NetworkInstanceId netId, Vector3 position, Vector3 rotation, Vector3 velocity)
    {
        this.netId = netId;
        this.position = position;
        this.rotation = rotation;
        this.velocity = velocity;
    }

    public void Deserialize(NetworkReader reader)
    {
        netId = reader.ReadNetworkId();
        position = reader.ReadVector3();
        rotation = reader.ReadVector3();
        velocity = reader.ReadVector3();
    }

    public void OnReceived()
    {
        new SyncBusToClient(netId, position, rotation, velocity).Send(R2API.Networking.NetworkDestination.Clients);
    }

    public void Serialize(NetworkWriter writer)
    {
        writer.Write(netId);
        writer.Write(position);
        writer.Write(rotation);
        writer.Write(velocity);
    }
}
