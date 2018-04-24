using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardNetworkManager : MonoBehaviour {

    private readonly string version = "0.1";

    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings(version);
        print("Connected to photon network = " + PhotonNetwork.connected);

    }

    #region Network Methods
    private void OnConnectedToMaster()
    {
        Debug.Log("connected to master");
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }
    private void OnJoinedLobby()
    {
        //Debug.Log("Joined lobby");
    }
    private void OnJoinedRoom()
    {
        if (PhotonNetwork.room.PlayerCount == PhotonNetwork.room.MaxPlayers)
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        var player = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0);
        player.GetPhotonView().RPC("AddToRoom", PhotonTargets.All);
        player.GetPhotonView().RPC("SetPlayerID", PhotonTargets.All, player.GetPhotonView().owner.ID - 1, CardGameManager.Instance.PlayerName);//Odaya katılanın klonları için 
        //player.GetPhotonView().name = "Erenmon";
        if (player.GetPhotonView().isMine)
        {
            CardGameManager.Instance.LocalPlayer = player;
            player.GetPhotonView().RPC("InitGame", PhotonTargets.All);
        }

    }

    private void OnPhotonPlayerConnected(PhotonPlayer newPlayer)//Odaya biri geldiğinde
    {
        if (PhotonNetwork.room.PlayerCount == PhotonNetwork.room.MaxPlayers)
        {
            StartGame();
        }
        //CardGameManager.Instance.LocalPlayer.GetPhotonView().RPC("SetPlayerID", PhotonTargets.All, 
        //                                                            CardGameManager.Instance.LocalPlayer.GetPhotonView().owner.ID);//kendi klonlarımın
        //CardGameManager.Instance.LocalPlayer.GetPhotonView().RPC("AddToRoom", PhotonTargets.All);
    }
    #endregion

}
