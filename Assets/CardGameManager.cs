using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardGameManager : MonoBehaviour {

    public static CardGameManager Instance;

    public GameObject PowerCardPrefab;
    public GameObject WordCardPrefab;

    //public TextMeshProUGUI Parchment;
    public Text Parchment;
    public Transform Room;
    public GameObject Hand;
    public Transform TooltipBox;
    public Transform TurnArrow;
    public Transform DrawCard;
    public List<Transform> ArrowPositions = new List<Transform>();
    public Transform LoginMenu;
    public Transform ToggleGroup;
    public Transform VotePopUp;
    public Transform PlayerName_InputField;
    public Transform TableDraw;
    public Transform PlayerNames;
    public Transform EndGameMenu;
    public Transform AnimationObjects;


    public string PlayerName = "Player";
    public GameObject LocalPlayer;
    public List<bool> Voted = new List<bool>() { false, false, false, false };
    public List<int> TotalVote = new List<int>() { 0, 0, 0, 0 };
    public int CurrentChapter = 0;
    public int MaxChapter = 3;
    public int ChapterLength = 40;
    public int TotalTurn = 1;
    public int Turn = 0;
    public int TurnDirection = 1;
    public bool CanDraw;
    public string Puncuation = " ";
    public PlayerController LastPlayer;
    public List<int> Score = new List<int>() { 0, 0, 0, 0 };
    public List<Vector2> WrittenWords = new List<Vector2>();
    public List<int> SittingOrder = new List<int>();
    public List<PowerCard> PowerCards = new List<PowerCard>(); //10
    public string[] Nouns; //30
    public string[] Adjectives; //30
    public string[] Adverbs; //10
    public string[] Verbs; //20
    public string[] HelpingVerbs; //20
    public string[] QuestionWords; // 10


    public void Awake()
    {
        Instance = this;
        CanDraw = false;
    }
    void Start () {
        LoadCards();
        LoginMenu.parent.gameObject.SetActive(true);
    }

    public Card RandomCard()
    {
        int random = Random.Range(0, 100);

        if (random < 10)
        {
            return RandomPowerCard();
        }
        else if (random >= 10 && random < 50)
        {
            return WordCard.CreateCard((Nouns[Random.Range(0, Nouns.Length)]));
        }
        else if (random >= 50 && random < 65)
        {
            return WordCard.CreateCard((Adjectives[Random.Range(0, Adjectives.Length)]));
        }
        else if (random >= 65 && random < 70)
        {
            return WordCard.CreateCard((Adverbs[Random.Range(0, Adverbs.Length)]));
        }
        else if (random >= 70 && random < 85)
        {
            return WordCard.CreateCard((Verbs[Random.Range(0, Verbs.Length)]));
        }
        else if (random >= 85 && random < 95)
        {
            return WordCard.CreateCard((HelpingVerbs[Random.Range(0, HelpingVerbs.Length)]));
        }
        else if (random >= 95 && random < 100)
        {
            return WordCard.CreateCard((QuestionWords[Random.Range(0, QuestionWords.Length)]));
        }
        return null;
    }
    public PowerCard RandomPowerCard()
    {
        return PowerCards[Random.Range(0, PowerCards.Count)];
    }

    public Card GetCardByText(string cardText)
    {
        foreach (var item in PowerCards)
        {
            if (item.CardText == cardText)
                return item;
        }
        return WordCard.CreateCard(cardText);
    }

    public void SeatPlayers()
    {
        for (int i = 0; i < 4; i++)
        {
            SittingOrder.Add((LocalPlayer.GetComponent<PlayerController>().PlayerID + i) % 4);
        }
    }

    private void LoadCards()
    {
        
        PowerCards.AddRange(Resources.LoadAll<PowerCard>("Cards/Power"));
        LoadTextCards();
    }

    private void LoadTextCards()
    {
        Nouns = Resources.Load<TextAsset>("Cards/Word/Nouns").text.Split('\n');
        Adjectives = Resources.Load<TextAsset>("Cards/Word/Adjectives").text.Split('\n');
        Adverbs = Resources.Load<TextAsset>("Cards/Word/Adverbs").text.Split('\n');
        Verbs = Resources.Load<TextAsset>("Cards/Word/Verbs").text.Split('\n');
        HelpingVerbs = Resources.Load<TextAsset>("Cards/Word/HelpingVerbs").text.Split('\n');
        QuestionWords = Resources.Load<TextAsset>("Cards/Word/QuestionWords").text.Split('\n');

    }

    private Card GetWordCard(string text)
    {
        return WordCard.CreateCard(text);
    }
    

    public void ToggleText()
    {
        foreach (var item in ToggleGroup.GetComponent<ToggleGroup>().ActiveToggles())
        {
            Puncuation = item.GetComponentInChildren<TextMeshProUGUI>().text;
        }
        //CardGameManager.Instance.LocalPlayer.GetPhotonView().RPC("DrawCard", PhotonTargets.All, 1);
    }

    public void ResetToggle()
    {
        Puncuation = " ";
        ToggleGroup.GetComponent<ToggleGroup>().SetAllTogglesOff();
    }
    
    public void EndGame()
    {
        //score ve totalvote
        for (int i = 0; i < 4; i++)
        {
            Score[i] = ((TotalVote[i] / 10 + 1) * Score[i]);
        }

        int j = 0;
        foreach (Transform player in Room)
        {
            EndGameMenu.GetChild(j).GetChild(0).GetComponent<TextMeshProUGUI>().text = player.gameObject.GetPhotonView().owner.NickName;
            EndGameMenu.GetChild(j).GetChild(1).GetComponent<TextMeshProUGUI>().text = Score[player.GetComponent<PlayerController>().PlayerID].ToString();
            j++;
        }

        EndGameMenu.parent.parent.gameObject.SetActive(true);

    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
