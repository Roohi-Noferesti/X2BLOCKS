using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;


public class LeaderBoard : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private TextMeshProUGUI l1, l2, l3, l4, l5, l6, l7, l8, l9, l10;
    [SerializeField] private string Url;

    public GameObject LeaderBoardPanel;

    public void OpenLeaderBoard()
    {
        LeaderBoardPanel.SetActive(true);
        ConnectToServer();
    }

    public void CloseLeaderBoard()
    {
        LeaderBoardPanel.SetActive(false);
    }
    
    public void ConnectToServer()
    {
        StartCoroutine(Post(Url));
    }

    class Player
    {
         public int highscore;
         public string id;
    }
    

    IEnumerator Post(string url)
    {
        Debug.Log(SystemInfo.deviceUniqueIdentifier);
        Player player = new Player() {
            highscore = PlayerPrefs.GetInt("highscore"),
            id=SystemInfo.deviceUniqueIdentifier
        };
        
        var json = JsonUtility.ToJson(player);
        using (UnityWebRequest www=UnityWebRequest.PostWwwForm(Url,json))
        {
            www.SetRequestHeader("content-type", "application/json");
            www.uploadHandler.contentType = "application/json";
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
            yield return www.SendWebRequest();
            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                if (www.isDone)
                {
                    // handle the result
                    var result = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                    l1.text = result;
                    Debug.Log(result);
                }
                else
                {
                    //handle the problem
                    Debug.Log("Error! data couldn't get.");
                }
            }
        }
    }
    
}
