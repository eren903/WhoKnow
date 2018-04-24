using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DrawCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Vector3 startPos;

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.GetComponent<Image>().raycastTarget = false;
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
        if (rr.gameObject != null)
        {
            if (rr.gameObject.name == "Hand" ||rr.gameObject.tag == "Card")
            {
                validDrop = true;
            }
        }

        if (validDrop && CardGameManager.Instance.CanDraw) //Check if the position is valid;
        {
            if (CardGameManager.Instance.LocalPlayer.GetComponent<PlayerController>().Cards.Count < 10 )
            {
                CardGameManager.Instance.LocalPlayer.GetPhotonView().RPC("DrawCard", PhotonTargets.All, 1);
                CardGameManager.Instance.CanDraw = false;
                CardGameManager.Instance.TableDraw.gameObject.SetActive(false);

            }

        }

        transform.position = startPos;
        transform.GetComponent<Image>().raycastTarget = true;

    }
    
}
