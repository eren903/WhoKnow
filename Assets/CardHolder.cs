using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CardHolder : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public Card Card;
    public Vector3 startPos;

    bool ToolTipOpen = false;
    Vector3 MousePos;

    private void Update()
    {
        if (ToolTipOpen)
        {
            if (Input.mousePosition != MousePos)
            {
                CardGameManager.Instance.TooltipBox.gameObject.SetActive(false);
                ToolTipOpen = false;

            }
        }       
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.GetChild(0).GetComponent<Image>().raycastTarget = false;
        startPos = transform.position;

    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }
    

    public void OnEndDrag(PointerEventData eventData)
    {
        bool validDrop = false;
        var rr = eventData.pointerCurrentRaycast;
        if ( rr.gameObject != null )
        {
            if (rr.gameObject.name == "Viewport" || rr.gameObject.name == "TMTextArea")
            {
                validDrop = true;
            }
        }
        //transform.GetChild(0).GetComponent<Image>().raycastTarget = true;

        if (validDrop) //Check if the position is valid;
        {
            if (CardGameManager.Instance.LocalPlayer.GetComponent<PlayerController>().Cards[transform.GetSiblingIndex()].CanPlay())// oynayabiliyorsa devam et
            {
                CardGameManager.Instance.LocalPlayer.GetPhotonView().RPC("PlayCard", PhotonTargets.All, transform.GetSiblingIndex());
                //Card.PlayCard(CardGameManager.Instance.Player.GetComponent<PlayerController>());
                Destroy(gameObject);
            }
            else // If not return the object to original position
            {
                transform.position = startPos;
                transform.GetChild(0).GetComponent<Image>().raycastTarget = true;
            }
        }
        else // If not return the object to original position
        {
            transform.position = startPos;
            transform.GetChild(0).GetComponent<Image>().raycastTarget = true;
        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Card.GetType() == typeof(PowerCard))
        {
            MousePos = Input.mousePosition;
            CardGameManager.Instance.TooltipBox.gameObject.SetActive(true);
            CardGameManager.Instance.TooltipBox.GetComponentInChildren<TextMeshProUGUI>().text = ((PowerCard)Card).Description;
            CardGameManager.Instance.TooltipBox.position = transform.position + new Vector3(0, 60, 0);
            ToolTipOpen = true;

        }
    }
}
