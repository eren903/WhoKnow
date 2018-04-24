using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class RoomGUI : MonoBehaviour, IPointerClickHandler
{
    LoginMenu LoginMenu;

    private void Start()
    {
        LoginMenu = GameObject.FindGameObjectWithTag("LoginMenu").GetComponent<LoginMenu>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        LoginMenu.SelectedRoom = transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;

        foreach (Transform room in LoginMenu.RoomContent)
        {
            room.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
        }
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.red;

    }
}
