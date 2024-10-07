using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LobbyNetworkManager : NetworkManager
{
    [Header("Spawn Points")]
    [SerializeField]
    [Tooltip("Char is indicate color and transform is relatiable spawn point")]
    List<SpawnPoint> _spawnPoints;

    public Dictionary<NetworkConnectionToClient, char> ActivePlayers = new();

    public override void OnStartServer()
    {
        NetworkServer.RegisterHandler<ReadyRequest>(OnReadyRequest);
    }

    public override void OnStopServer()
    {
        ActivePlayers.Clear();
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient connection)
    {
        if (ActivePlayers.TryGetValue(connection, out char color))
        {
            Transform startTransform = _spawnPoints.Find(spawnPoint => spawnPoint.Color == color).SpawnTransform;

            GameObject player = Instantiate(playerPrefab, startTransform.position, startTransform.rotation);

            player.name = color.ToString();
            NetworkServer.AddPlayerForConnection(connection, player);
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient connection)
    {
        if(ActivePlayers.TryGetValue(connection, out char color))
        {
            ActivePlayers.Remove(connection);
            NetworkServer.RemovePlayerForConnection(connection, RemovePlayerOptions.Destroy);
        }
    }
    

    private void OnReadyRequest(NetworkConnectionToClient connection, ReadyRequest readyRequest)
    {
        bool accepted = !ActivePlayers.Any(x => x.Value == readyRequest.Color);

        if (accepted)
        {
            ActivePlayers.Add(connection, readyRequest.Color);
        }

        connection.Send(new ReadyResponse(accepted));
    }
}


[Serializable]
public struct SpawnPoint
{
    [SerializeField]
    public char Color;
    [SerializeField]
    public Transform SpawnTransform;
}

public class ConnectedPlayer
{
    public ConnectedPlayer(NetworkConnectionToClient connection, char color, bool isReady)
    {
        Connection = connection;
        Color = color;
        IsReady = isReady;
    }

    public NetworkConnectionToClient Connection;
    public char Color;
    public bool IsReady;
}