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

    private readonly List<ConnectedPlayer> _connectedPlayers = new();

    public override void OnStartServer()
    {
        NetworkServer.RegisterHandler<ReadyRequest>(OnReadyRequest);
    }

    public override void OnStopServer()
    {
        _connectedPlayers.Clear();
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        char color = _connectedPlayers.Find(player => player.Connection == conn).Color;

        Transform startTransform = _spawnPoints.Find(spawnPoint => spawnPoint.Color == color).SpawnTransform;

        GameObject player = Instantiate(playerPrefab, startTransform.position, startTransform.rotation);

        player.name = color.ToString();
        NetworkServer.AddPlayerForConnection(conn, player);
    }


    public override void OnServerConnect(NetworkConnectionToClient connection)
    {
        _connectedPlayers.Add(new ConnectedPlayer(connection, ' ', false));

    }

    private void OnReadyRequest(NetworkConnectionToClient connection, ReadyRequest readyRequest)
    {
        bool accepted = !_connectedPlayers.Any(player => player.Color == readyRequest.Color);

        if (accepted)
        {
            int index = _connectedPlayers.FindIndex(player => player.Connection == connection);
            _connectedPlayers[index].Color = readyRequest.Color;
            _connectedPlayers[index].IsReady = true;
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