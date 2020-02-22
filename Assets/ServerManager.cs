using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;

[Serializable]
public enum MoveType {
    One,
    Two,
    Three
}

[Serializable]
public class Move {
    public int MoveNumber;
    public DateTimeOffset Time;
    public MoveType MoveType;
    public Dictionary<string, object> Data;
}

public class ServerManager : MonoBehaviour {
    static HubConnection connection;

    void Start () {
        connection = new HubConnectionBuilder ()
            .WithUrl ("https://localhost:5001/gameHub")
            .AddMessagePackProtocol ()
            .Build ();

        connection.On<string> ("ReceiveMessage", message => { Debug.Log ($"ReceiveMessage: {message}"); });

        connection.On<Move> ("ReceiveMove", ReceiveMove);

        connection.On<List<Move>> ("ReceiveMoves", ReceiveMoves);

        Connect ();

        JoinGame ("game");

        SendSimpleMessage ("Hello World!");

        SendDictionary ();

        SendMove ();

        SendMoves ();

        LeaveGame ("game");
    }

    #region Receiving

    void ReceiveMove (Move move) {
        Debug.Log ($"ReceiveMove MoveNumber: {move.MoveNumber}, Time: {move.Time.ToString ()}, MoveType: {move.MoveType}");
    }

    void ReceiveMoves (List<Move> moves) {
        Debug.Log ($"ReceiveMoves");

        foreach (var move in moves) {
            Debug.Log ($"MoveNumber: {move.MoveNumber}, Time: {move.Time.ToString ()}, MoveType: {move.MoveType}");
        }
    }

    #endregion

    #region Sending

    async void SendSimpleMessage (string message) {
        try {
            await connection.InvokeAsync<string> ("SendMessage", message);
        }
        catch (Exception ex) {
            Debug.LogError (ex.Message);
        }
    }

    async void SendDictionary () {
        var data = new Dictionary<string, object> ();
        data.Add ("roll", 6);

        try {
            await connection.InvokeAsync ("SendDictionary", data, DateTimeOffset.UtcNow);
        }
        catch (Exception ex) {
            Debug.LogError (ex.Message);
        }
    }

    async void Connect () {
        try {
            await connection.StartAsync ();

            Debug.Log ("Connection started");
        }
        catch (Exception ex) {
            Debug.LogError (ex.Message);
        }
    }

    async void JoinGame (string game) {
        try {
            await connection.InvokeAsync ("JoinGroup", game);
        }
        catch (Exception ex) {
            Debug.LogError (ex.Message);
        }
    }

    async void LeaveGame (string game) {
        try {
            await connection.InvokeAsync ("LeaveGroup", game);
        }
        catch (Exception ex) {
            Debug.LogError (ex.Message);
        }
    }

    async void SendMove () {
        try {
            var move = new Move {
                MoveNumber = 1,
                MoveType = MoveType.One,
                Time = DateTimeOffset.UtcNow,
                Data = new Dictionary<string, object> {
                    {
                        "stringValue", "hello world"
                    }, {
                        "intValue", 6
                    }, {
                        "floatValue", 6.6f
                    }
                }
            };

            await connection.InvokeAsync<Move> ("SendMove", move);
        }
        catch (Exception ex) {
            Debug.LogError (ex.Message);
        }
    }

    async void SendMoves () {
        try {
            var moves = new List<Move> {
                new Move {
                    MoveNumber = 1,
                    MoveType = MoveType.Two,
                    Time = DateTimeOffset.UtcNow,
                    Data = new Dictionary<string, object> {
                        {
                            "stringValue", "hello world"
                        }, {
                            "intValue", 6
                        }, {
                            "floatValue", 6.6f
                        }
                    }
                },
                new Move {
                    MoveNumber = 2,
                    MoveType = MoveType.Three,
                    Time = DateTimeOffset.UtcNow,
                    Data = new Dictionary<string, object> {
                        {
                            "stringValue", "hello world"
                        }, {
                            "intValue", 6
                        }, {
                            "floatValue", 6.6f
                        }
                    }
                }
            };

            await connection.InvokeAsync<List<Move>> ("SendMoves", moves);
        }
        catch (Exception ex) {
            Debug.LogError (ex.Message);
        }
    }

    #endregion
}