using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : ScriptableObject {

    public string CardText;

    public virtual void PlayCard(PlayerController pc)
    {

    }

    public bool CanPlay() // oynayabiliyorsan true döndür
    {
        if (!(CardGameManager.Instance.LocalPlayer.GetComponent<PlayerController>().PlayerID == CardGameManager.Instance.Turn))
        {
            return false;
        }
        if (this.GetType() == typeof(WordCard))
        {
            return true;
        }
        switch (((PowerCard)this).PowerType)
        {
            case PowerType.DrawTwo:
                return !(CardGameManager.Instance.Hand.transform.childCount > 8);
            case PowerType.DrawThree:
                return !(CardGameManager.Instance.Hand.transform.childCount > 7);
            case PowerType.SkipOne:
                break;
            case PowerType.SkipTwo:
                break;
            case PowerType.RemoveOneFromOthers:
                break;
            case PowerType.RemoveLastWord:
                return CardGameManager.Instance.Parchment.text.Length > 0;
            case PowerType.SwapHand:
                break;
            case PowerType.ReplaceHand:
                break;
            case PowerType.ReverseTurn:
                break;
            case PowerType.RandomPowerCard:
                break;
            default:
                return false;
        }


        return true;
    }
}
