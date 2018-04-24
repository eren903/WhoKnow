using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerType
{
    DrawTwo,
    DrawThree,
    SkipOne,
    SkipTwo,
    RemoveOneFromOthers,
    RemoveLastWord,
    SwapHand,
    ReplaceHand,
    ReverseTurn,
    RandomPowerCard
}

[CreateAssetMenu(fileName = "PowerCard", menuName = "Cards/Power Card")]
public class PowerCard : Card
{
    public string Description;
    public PowerType PowerType;
    public Color Color;

    public override void PlayCard(PlayerController pc)
    {
        switch (PowerType)
        {
            case PowerType.DrawTwo:
                //playerController.photonView.RPC("DrawCard", PhotonTargets.All, 2);
                pc.DrawCard(2);
                break;
            case PowerType.DrawThree:
                //playerController.photonView.RPC("DrawCard", PhotonTargets.All, 2);
                pc.DrawCard(3);
                break;
            case PowerType.SkipOne:
                pc.photonView.RPC("SkipPlayer", PhotonTargets.All, 1);
                break;
            case PowerType.SkipTwo:
                pc.photonView.RPC("SkipPlayer", PhotonTargets.All, 2);
                break;
            case PowerType.RemoveOneFromOthers:
                pc.photonView.RPC("RemoveOne", PhotonTargets.All);
                break;
            case PowerType.RemoveLastWord:
                pc.photonView.RPC("RemoveLastText", PhotonTargets.All);
                break;
            case PowerType.SwapHand:
                pc.photonView.RPC("SwapHand", PhotonTargets.All, (pc.PlayerID + 2) % 4);
                break;
            case PowerType.ReplaceHand:
                pc.photonView.RPC("ReplaceHand", PhotonTargets.All);
                break;
            case PowerType.ReverseTurn:
                pc.photonView.RPC("ReverseTurn", PhotonTargets.All);
                break;
            case PowerType.RandomPowerCard:
                pc.photonView.RPC("RandomPower", PhotonTargets.All);
                break;
            default:
                break;
        }
    }


}
