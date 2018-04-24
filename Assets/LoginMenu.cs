using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoginMenu : MonoBehaviour {

    public GameObject RoomPrefab;

    public TextMeshProUGUI Status;
    public Transform RoomContent;
    public GameObject CreateRoom_Popup;
    public string SelectedRoom;

    RoomInfo[] Rooms;

    private void Update()
    {
        Status.text = "Status : " + PhotonNetwork.connectionStateDetailed.ToString();            
    }

    public void OnJoinedLobby()
    {
        Debug.Log("Joined lobby");
        StartCoroutine(InitRoom());
    }
    public void OnJoinedRoom()
    {
        transform.parent.gameObject.SetActive(false);
    }

    public void ListRooms()
    {
        foreach (Transform room in RoomContent)
        {
            Destroy(room.gameObject);
        }
        foreach (var room in Rooms)
        {
            var r = Instantiate(RoomPrefab, RoomContent);
            r.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = room.Name;
            r.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = room.PlayerCount.ToString() + "/4";
        }
    }

    public void Refresh()
    {
        Rooms = PhotonNetwork.GetRoomList();
        ListRooms();
    }

    public void CreateRoom()
    {
        CreateRoom_Popup.SetActive(true);
    }
    public void Create()
    {
        var inputField = CreateRoom_Popup.transform.GetChild(0).GetComponent<TMP_InputField>();
        if (inputField.text != "")
        {
            for (int i = 0; i < Rooms.Length; i++)
            {
                if (inputField.text == Rooms[i].Name)
                    return;
            }
            if (CardGameManager.Instance.PlayerName_InputField.GetComponent<TMP_InputField>().text.Length > 3)
            {
                CardGameManager.Instance.PlayerName = CardGameManager.Instance.PlayerName_InputField.GetComponent<TMP_InputField>().text;
            }
            PhotonNetwork.CreateRoom(inputField.text, new RoomOptions() { MaxPlayers = 4, IsVisible = true }, null);
        }

    }
    public void Cancel()
    {
        CreateRoom_Popup.SetActive(false);

    }
    public void JoinRoom()
    {
        if (SelectedRoom != "" && CardGameManager.Instance.PlayerName_InputField.GetComponent<TMP_InputField>().text.Length > 3)
        {
            CardGameManager.Instance.PlayerName = CardGameManager.Instance.PlayerName_InputField.GetComponent<TMP_InputField>().text;
            PhotonNetwork.JoinRoom(SelectedRoom);
        }

    }
    IEnumerator InitRoom()
    {
        yield return new WaitForSeconds(1);
        Refresh();

    }


}
