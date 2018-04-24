using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SharedObject : MonoBehaviour {

    public int testInt;
    public string testString;
    public TextMeshProUGUI debugText;

    private void Update()
    {
        debugText.text = testInt.ToString() + "\n" + testString;
    }

    [PunRPC]
    public void changeInt(int value)
    {
        if (GetComponent<PhotonView>().photonView.isMine)
        {
            testInt = value;
        }
    }
    [PunRPC]
    public void changeString(string value)
    {
        testString = value;
    }

}
