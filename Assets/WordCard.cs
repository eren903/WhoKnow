using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WordCard", menuName = "Cards/Word Card")]
public class WordCard : Card
{
    public static WordCard CreateCard(string text)
    {
        var card = CreateInstance<WordCard>();
        card.CardText = text;
        return card;
    }
    public override void PlayCard(PlayerController pc)
    {
        Debug.Log(CardText);
        pc.photonView.RPC("AddText", PhotonTargets.All, CardText + CardGameManager.Instance.Puncuation);
    }

    
}
