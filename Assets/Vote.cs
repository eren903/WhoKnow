using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Vote : MonoBehaviour {

    public int votedFor = -1;

    void OnEnable()
    {
        int i = 0;
        foreach (Transform player in CardGameManager.Instance.Room)
        {
            if(!(player.GetComponent<PlayerController>().PlayerID == 
                CardGameManager.Instance.LocalPlayer.GetComponent<PlayerController>().PlayerID))
            {
                transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = player.GetComponent<PlayerController>().PlayerID.ToString();
                transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = player.gameObject.GetPhotonView().owner.NickName;
                i++;
            }            
        }
    }

    public void ButtonVote()
    {
        votedFor = int.Parse(EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
        CardGameManager.Instance.LocalPlayer.GetPhotonView().RPC("SyncVote", PhotonTargets.All, votedFor);
        //EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
        transform.parent.parent.gameObject.SetActive(false);
    }
}
