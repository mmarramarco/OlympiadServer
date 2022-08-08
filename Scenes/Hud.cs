using Godot;
using System.Linq;
using System;

public class Hud : Control
{
    private Server _server;
    private bool _firstPlayerDecided = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _server = GetNode<Server>("/root/Server");
    }

    [Remote]
    public void ReadyToFight(bool isReady)
    {
        // TODO : Rock paper scissor before the game? or coin flip?
        var senderId = GetTree().GetRpcSenderId();
        var player = _server.Players.First(_ => _.PeerId == senderId);
        player.IsReady = isReady;
        GD.Print($"'{player.Pseudo}' ({player.PeerId}) is {(isReady ? "" : "not")} ready");

        // if (Players.Count != MaxPlayers) return;
        Rpc("ShowPlayerReadyIcon", senderId, isReady);

        var allPlayersReady = true;
        _server.Players.ForEach(_ => allPlayersReady = allPlayersReady && _.IsReady);

        if (allPlayersReady)
        {
            _server.Players.ForEach(_ => RpcId(_.PeerId, "StartMulligan"));
            GD.Print("Started mulligan.");
        }
    }
}
