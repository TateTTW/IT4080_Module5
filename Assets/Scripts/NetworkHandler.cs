using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkHandler : NetworkBehaviour
{
    private bool hasPrinted = false;

    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.OnClientStarted += OnClientStarted;
        NetworkManager.OnServerStarted += OnServerStarted;
    }

    private void PrintMe()
    {
        if (hasPrinted) { return; }
        Debug.Log("I am ");
        hasPrinted = true;
        if (IsServer)
        {
            Debug.Log($"the server! {NetworkManager.ServerClientId}");
        }
        if (IsHost)
        {
            Debug.Log($"the host! {NetworkManager.ServerClientId}/{NetworkManager.LocalClientId}");
        }
        if (IsClient)
        {
            Debug.Log($"a client! {NetworkManager.LocalClientId}");
        }
        if (!IsServer && !IsClient)
        {
            Debug.Log("nothing yet.");
            hasPrinted = false;
        }
    }

    // ------------------------
    // Client Actions
    // ------------------------
    private void OnClientStarted()
    {
        Debug.Log("Client Started!");
        NetworkManager.OnClientConnectedCallback += ClientOnClientConnected;
        NetworkManager.OnClientDisconnectCallback += ClientOnClientDisconnected;
        NetworkManager.OnClientStopped += ClientOnServerStopped;
        PrintMe();
    }

    private void ClientOnClientConnected(ulong clientId)
    {
        PrintMe();
        if (IsHost) {
            Debug.Log($"{clientId} has connected");
        } else {
            Debug.Log($"I {clientId}, have connected to the server");
        }
    }
    private void ClientOnClientDisconnected(ulong clientId)
    {
        if (IsHost) {
            Debug.Log($"{clientId} has disconnected from the server");
        } else {
            Debug.Log($"I {clientId}, have disconnected from the server");
        }
    }
    private void ClientOnServerStopped(bool flag)
    {
        Debug.Log("Client stopped!!");
        hasPrinted = false;
        NetworkManager.OnClientConnectedCallback -= ClientOnClientConnected;
        NetworkManager.OnClientDisconnectCallback -= ClientOnClientDisconnected;
        NetworkManager.OnServerStopped -= ClientOnServerStopped;
    }

    // ------------------------
    // Server Actions
    // ------------------------
    private void OnServerStarted()
    {
        Debug.Log("Server Started!!");
        NetworkManager.OnClientConnectedCallback += ServerOnClientConnected;
        NetworkManager.OnClientDisconnectCallback += ServerOnClientDisconnected;
        NetworkManager.OnServerStopped += ServerOnServerStopped;
        PrintMe();
    }

    private void ServerOnClientConnected(ulong clientId)
    {
        Debug.Log($"Client {clientId} connected to the server.");
    }
    private void ServerOnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Client {clientId} disconnected from the server.");
    }
    private void ServerOnServerStopped(bool flag)
    {
        Debug.Log("Server stopped!!");
        hasPrinted = false;
        NetworkManager.OnClientConnectedCallback -= ServerOnClientConnected;
        NetworkManager.OnClientDisconnectCallback -= ServerOnClientDisconnected;
        NetworkManager.OnServerStopped -= ServerOnServerStopped;
    }
}
