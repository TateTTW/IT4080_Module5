using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ArenaGame : NetworkBehaviour
{
    public Player playerPrefab;
    public Player hostPrefab;

    public Camera arenaCamera;

    private int positionIndex = 0;
    private Vector3[] startPositions = new Vector3[]
    {
        new Vector3(4, 6, 0),
        new Vector3(-4, 6, 0),
        new Vector3(0, 6, 4),
        new Vector3(0, 6, -4)
    };

    private int colorIndex = 0;
    private Color[] playerColors = new Color[]
    { 
        Color.blue,
        Color.red,
        Color.green,
        Color.yellow
    };


    // Start is called before the first frame update
    void Start()
    {
        if (IsClient && arenaCamera != null)
        {
            arenaCamera.enabled = false;
            arenaCamera.GetComponent<AudioListener>().enabled = false;
        }

        if (IsServer)
        {
            SpawnPlayers();
        }
    }

    private void SpawnPlayers()
    {
        
        foreach(ulong clientId in NetworkManager.ConnectedClientsIds)
        {
            Debug.Log("clientId: " + clientId);
            Player playerSpawn = Instantiate(NetworkManager.ServerClientId == clientId ? hostPrefab : playerPrefab, NextPosition(), Quaternion.identity);
            playerSpawn.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
            playerSpawn.playerColorNetVar.Value = NextColor();
        }
    }

    private Vector3 NextPosition()
    {
        Vector3 pos = startPositions[positionIndex];
        positionIndex += 1;
        if (positionIndex > startPositions.Length - 1)
        {
            positionIndex = 0;
        }
        return pos;
    }

    private Color NextColor()
    {
        Color color = playerColors[colorIndex];
        colorIndex++;
        if (colorIndex > playerColors.Length - 1)
        {
            colorIndex = 0;
        }
        return color;
    }

}
