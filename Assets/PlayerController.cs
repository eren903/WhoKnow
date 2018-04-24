using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using System.Linq;

public class PlayerController : Photon.MonoBehaviour {
    

    public int PlayerID;
    public string PlayerName;
    public List<Card> Cards = new List<Card>();


    public int seed;

    private void Start()
    {
    }

    public void InstantiateCard(Card card)
    {
        Type cardType = card.GetType();
        if (cardType == typeof(PowerCard))
        {
            //Instantiate(Resources.Load<PowerCard>("Prefabs/PowerCard"), CardGameManager.Instance.Hand.transform, worldPositionStays: false);
            var tempcard = Instantiate(CardGameManager.Instance.PowerCardPrefab, CardGameManager.Instance.Hand.transform, worldPositionStays: false);
            tempcard.GetComponent<CardHolder>().Card = card;
            tempcard.transform.Find("OuterLayer").GetComponent<Image>().color = ((PowerCard)card).Color;
            tempcard.GetComponentInChildren<TextMeshProUGUI>().text = ((PowerCard)card).CardText;
            tempcard.name = ((PowerCard)card).CardText;
        }
        else if (cardType == typeof(WordCard))
        {
            //Instantiate(Resources.Load<PowerCard>("Prefabs/WordCard"), CardGameManager.Instance.Hand.transform, worldPositionStays: false);
            var tempcard = Instantiate(CardGameManager.Instance.WordCardPrefab, CardGameManager.Instance.Hand.transform, worldPositionStays: false);
            tempcard.GetComponent<CardHolder>().Card = card;
            tempcard.GetComponentInChildren<TextMeshProUGUI>().text = ((WordCard)card).CardText;
            tempcard.name = ((WordCard)card).CardText;
        }
    }

    public void SetPlayerNames()
    {
        for (int i = 0; i < CardGameManager.Instance.SittingOrder.Count; i++)
        {
            foreach (Transform player in CardGameManager.Instance.Room)
            {
                if(player.GetComponent<PlayerController>().PlayerID == CardGameManager.Instance.SittingOrder[i])
                    CardGameManager.Instance.PlayerNames.GetChild(i).GetComponent<TextMeshProUGUI>().text = player.gameObject.GetPhotonView().owner.NickName;
                
            }
        }
    }

    IEnumerator DelayedSetPlayerName()
    {
        yield return new WaitForSeconds(1);
        SetPlayerNames();

    }

    #region Networked methods

    [PunRPC]
    public void AddToRoom()
    {
        gameObject.transform.SetParent(CardGameManager.Instance.Room);
    }
    [PunRPC]
    public void InitGame()
    {

        if (photonView.isMine)
        {
            photonView.RPC("DrawCard", PhotonTargets.All, 7);
            CardGameManager.Instance.SeatPlayers();
            photonView.RPC("SyncArrow", PhotonTargets.All);
            StartCoroutine(DelayedSetPlayerName());
        }


        //gameObject.transform.SetParent(CardGameManager.Instance.Room);
    }

    [PunRPC]
    public void PlayCard(int i)
    {
        //DebugText.text += "\n" + CardGameManager.Instance.RandomSeed;
        if (photonView.isMine)
        {
            var card = Cards[i];
            Cards.Remove(Cards[i]);
            card.PlayCard(this);
            CardGameManager.Instance.CanDraw = false;
            CardGameManager.Instance.TableDraw.gameObject.SetActive(false);
            CardGameManager.Instance.Turn = (CardGameManager.Instance.Turn + CardGameManager.Instance.TurnDirection) % 4;
            
            if (CardGameManager.Instance.Turn < 0)
            {
                CardGameManager.Instance.Turn += 4;
            }
            photonView.RPC("SyncTurn", PhotonTargets.All, CardGameManager.Instance.Turn);
            photonView.RPC("SyncArrow", PhotonTargets.All);
        }
        else
        {
            Cards.Remove(Cards[i]);
        }
        
    }

    [PunRPC]
    public void DrawCard(int count)
    {
        for (int i = 0; i < count; i++)
        {   
            if (photonView.isMine)
            {
                var card = CardGameManager.Instance.RandomCard();

                InstantiateCard(card);

                Cards.Add(card);
                photonView.RPC("SyncDraw", PhotonTargets.Others, card.CardText);
            }

        }

    }

    [PunRPC]
    public void SyncDraw(string cardText)
    {
        Cards.Add(CardGameManager.Instance.GetCardByText(cardText));
    }

    [PunRPC]
    public void SyncVote(int votedPlayer)
    {
        CardGameManager.Instance.Voted[PlayerID] = true;
        CardGameManager.Instance.TotalVote[votedPlayer] += 1;
        bool tempBool = true;
        Debug.Log("asd");

        foreach (var vote in CardGameManager.Instance.Voted)
        {
            if (!vote)
            {
                tempBool = false;
                Debug.Log(" " + tempBool + " " + vote);
            }

            Debug.Log(" asdasd" + tempBool + " " + vote);
        }

        if (tempBool)
        {

            Debug.Log("4kişi de oyladı");
            for (int i = 0; i < CardGameManager.Instance.Voted.Count; i++)
            {
                CardGameManager.Instance.Voted[i] = false;
            }
            //CardGameManager.Instance.TotalVote[CardGameManager.Instance.VotePopUp.GetComponent<Vote>().votedFor] += 1;
            CardGameManager.Instance.VotePopUp.parent.parent.gameObject.SetActive(false);
            CardGameManager.Instance.TotalTurn++;
            SyncArrow();
            CardGameManager.Instance.TotalTurn--;

            if(CardGameManager.Instance.CurrentChapter == CardGameManager.Instance.MaxChapter)
            {
                //End game
                //Son oynayan client ve kopyaları
                CardGameManager.Instance.EndGame();

            }
            CardGameManager.Instance.CurrentChapter++;
            AddText("\n");
        }

    }

    [PunRPC]
    public void SyncTurn(int turn)
    {
        CardGameManager.Instance.Turn = turn;
        CardGameManager.Instance.TotalTurn += 1;
        if (turn == PlayerID)
        {
            CardGameManager.Instance.CanDraw = true;
        }


    }

    [PunRPC]
    public void SetPlayerID(int id, string name)
    {
        PlayerID = id;
        photonView.name = name;
        PlayerName = name;
        if (photonView.isMine)
        {
            photonView.owner.NickName = name;
        }

    }
    [PunRPC]
    public void RemoveCard(int i)
    {
        Cards.RemoveAt(i);
        if (photonView.isMine)
        {
            Destroy(CardGameManager.Instance.Hand.transform.GetChild(i).gameObject);
        }
    }

    [PunRPC]
    public void AddText(string cardText)
    {
        CardGameManager.Instance.Parchment.text += cardText;
        CardGameManager.Instance.WrittenWords.Add(new Vector2(PlayerID, CardGameManager.Instance.Parchment.text.LastIndexOf(' ')));
        CardGameManager.Instance.Score[PlayerID] += 1;
        CardGameManager.Instance.Parchment.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 0f;
    }

    [PunRPC]
    public void SyncArrow()
    {
        var arrow = CardGameManager.Instance.TurnArrow;
        int temp=-1;
        for (int i = 0; i < CardGameManager.Instance.SittingOrder.Count; i++)
        {
            if (CardGameManager.Instance.SittingOrder[i] == CardGameManager.Instance.Turn)
            {
                temp = i;
            }
        }

        //Chapter bittiyse oylama kısmına geç, oylamayı beklesin herkes
        if (CardGameManager.Instance.TotalTurn != 0)
        {
            if (0 == CardGameManager.Instance.TotalTurn % CardGameManager.Instance.ChapterLength)
            {
                CardGameManager.Instance.VotePopUp.parent.parent.gameObject.SetActive(true);
                temp = -77;
            }
        }
        switch (temp)
        {
            case 0: // Our turn starts
                arrow.transform.position = CardGameManager.Instance.ArrowPositions[0].position;
                CardGameManager.Instance.CanDraw = true;
                CardGameManager.Instance.TableDraw.gameObject.SetActive(true);
                CardGameManager.Instance.ResetToggle();
                break;
            case 1: // Our turn ends
                arrow.transform.position = CardGameManager.Instance.ArrowPositions[1].position;
                break;
            case 2:
                arrow.transform.position = CardGameManager.Instance.ArrowPositions[2].position;
                break;
            case 3:
                arrow.transform.position = CardGameManager.Instance.ArrowPositions[3].position;
                break;
            case -77:
                //Vote...
                break;
            default:
                break;
        }
    }
    
    #region PowerCards
    //   [PunRPC]
    //   public void Template()
    //   {

    //   }

    [PunRPC]
    public void SkipPlayer(int count)
    {
        CardGameManager.Instance.Turn = (CardGameManager.Instance.Turn + count * CardGameManager.Instance.TurnDirection) % 4;
    }
    [PunRPC]
    public void RemoveOne()
    {
        foreach (Transform player in CardGameManager.Instance.Room)
        {
            if (player.GetComponent<PlayerController>().PlayerID != PlayerID)
            {
                var pc = player.GetComponent<PlayerController>();
                if (pc.photonView.isMine)
                {
                    if (pc.Cards.Count > 0)
                    {
                        var randomNumber = UnityEngine.Random.Range(0, pc.Cards.Count);
                        pc.photonView.RPC("RemoveCard", PhotonTargets.All, randomNumber);
                    }
                }
            }
        }
        
    }
    [PunRPC]
    public void RemoveLastText()
    {
        CardGameManager.Instance.Score[(int)CardGameManager.Instance.WrittenWords.Last().x] -= 1;
        CardGameManager.Instance.WrittenWords.RemoveAt(CardGameManager.Instance.WrittenWords.Count - 1);        
        CardGameManager.Instance.Parchment.text = CardGameManager.Instance.Parchment.text.Remove(CardGameManager.Instance.Parchment.text.LastIndexOf(' '));
    }
    [PunRPC]
    public void SwapHand(int targetPlayerID)
    {
        foreach (Transform player in CardGameManager.Instance.Room)
        {
            var pc = player.GetComponent<PlayerController>();
            if (pc.PlayerID == targetPlayerID)
            {
                var tempCards = pc.Cards;

                pc.Cards = Cards;
                Cards = tempCards;
                
                //Kartlarını veren adamın bilgisayarı
                if (pc.gameObject.GetPhotonView().isMine)
                {
                    foreach (Transform child in CardGameManager.Instance.Hand.transform)
                    {
                        Destroy(child.gameObject);
                    }
                    for (int i = 0; i < pc.Cards.Count; i++)
                    {
                        InstantiateCard(pc.Cards[i]);
                    }
                }
                //Bizim bilgisayarımız
                if (photonView.isMine)
                {
                    Debug.Log("pc count :" + pc.Cards.Count);
                    foreach (Transform child in CardGameManager.Instance.Hand.transform)
                    {
                        Destroy(child.gameObject);
                    }
                    for (int i = 0; i < Cards.Count; i++)
                    {
                        InstantiateCard(Cards[i]);
                    }
                }

            }
        }



    }
    [PunRPC]
    public void ReplaceHand()
    {
        var tempCount = Cards.Count;
        Cards.Clear();
        if (photonView.isMine)
        {
            foreach (Transform child in CardGameManager.Instance.Hand.transform)
            {
                Destroy(child.gameObject);
            }
            photonView.RPC("DrawCard", PhotonTargets.All, tempCount);
        }

    }
    [PunRPC]
    public void ReverseTurn()
    {
        CardGameManager.Instance.TurnDirection *= -1;
    }
    [PunRPC]
    public void RandomPower()
    {
        if (photonView.isMine)
        {
            var card = CardGameManager.Instance.RandomPowerCard();

            InstantiateCard(card);

            Cards.Add(card);
            photonView.RPC("SyncDraw", PhotonTargets.Others, card.CardText);
        }

    }


    #endregion

    #endregion


}
