using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;


///
/// HERE IS THE MOST IMPORTANT PART OF ALL OF GODOT'S MULTIPLAYER
/// 
/// MULTIPLYER CALLS FOLLOW NODE TREE PATHS
/// 
/// CALLING A REMOTE FUNCTION ON /root/lobby/12345678 MAKES THE SERVER LOOK FOR /root/lobby/12345678
/// 


/// <summary>
/// The server.
/// </summary>
public class Server : Node
{
    private readonly NetworkedMultiplayerENet _network = new NetworkedMultiplayerENet();
    private int DefaultPort = 1909;
    private int MaxPlayers = 2;
    public List<PlayerModel> Players = new List<PlayerModel>();
    public List<PlayerModel> Spectators = new List<PlayerModel>(); // TODO : implement spectating.
    private Board _board;
    private Random _random = new Random();
    private bool _firstPlayerDecided = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        OS.WindowMinimized = true; // it's a headless, let's just hide it.
        StartServer();
    }

    [Remote]
    public void AddPlayer(string pseudo)
    {
        var peerId = GetTree().GetRpcSenderId();
        var player = new PlayerModel
        {
            Pseudo = pseudo,
            PeerId = peerId
        };

        Players.Add(player);
        GD.Print($"Added player '{pseudo}', with peer '{peerId}'. Now '{Players.Count}' player(s) in the list.");

        if (Players.Count == MaxPlayers)
        {
            DecideFirstPlayer();
        }
    }

    [Remote]
    public void StartLoading()
    {
        _board = GetNode<Board>("/root/MainScene/PathFinding/Board");
        var senderId = GetTree().GetRpcSenderId();

        // Add the player remote doll to the server.
        var resource = ResourceLoader.Load("res://Scenes/Player.tscn");
        var packedScene = (PackedScene)resource;
        var playerNode = (Player)packedScene.Instance();
        playerNode.Name = $"{senderId}";
        _board.AddChild(playerNode);

        // Send back the field to the player that called this.
        var field = _board.GetField();
        var model = JsonConvert.SerializeObject(field);
        RpcId(senderId, "EndLoading", model); // TODO : only end loading when both players are ready. Do a "search for opponent" message while doing field generation too.
    }

    private void StartServer()
    {
        _network.CreateServer(DefaultPort, MaxPlayers);
        GetTree().NetworkPeer = _network;
        GD.Print("Server started.");

        _network.Connect("peer_connected", this, nameof(PeerConnected));
        _network.Connect("peer_disconnected", this, nameof(PeerDisconnected));
    }

    private void PeerConnected(int playerId)
    {
        GD.Print($"Player '{playerId}' connected.");
    }

    private void PeerDisconnected(int playerId)
    {
        var player = Players.Find(_ => _.PeerId == playerId);
        Players.Remove(player);
        GD.Print($"Player '{player.Pseudo}' disconnected.");
        GD.Print($"Removed player '{player.Pseudo}', with peer '{player.PeerId}'. Now '{Players.Count}' player(s) in the list.");
    }

    private void DecideFirstPlayer()
    {
        if (_firstPlayerDecided)
        {
            return;
        }

        var index = _random.Next(Players.Count);
        Players[index].IsFirstPlayer = true;
    }
}
