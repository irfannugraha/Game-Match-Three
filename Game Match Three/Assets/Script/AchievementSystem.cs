using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementSystem : Observer
{

    public Image achievementBanner;
    public Text achievementText;

    TileEvent cookiesEvent, cakeEvent, candyEvent;

    void Start() {
          PlayerPrefs.DeleteAll();
          cookiesEvent = new MatchTileEvent(3);
          cakeEvent = new MatchTileEvent(10);
          candyEvent = new MatchTileEvent(5);

          foreach (var poi in FindObjectsOfType<PointOfInterest>())
          {
              poi.Register_Observer(this);
          }
    }

    public override void On_Notify(string value){
        string key;

        if (value.Equals("Cookies Event"))
        {
            cookiesEvent.On_Match();
            if (cookiesEvent.Achievement_Completed())
            {
                key = "Match first cookies";
                Notify_Achievement(key, value);
            }
        }

        if (value.Equals("Cake Event"))
        {
            cakeEvent.On_Match();
            if (cookiesEvent.Achievement_Completed())
            {
                key = "Match 10 cookies";
                Notify_Achievement(key, value);
            }
        }
        
        if (value.Equals("Candy Event"))
        {
            candyEvent.On_Match();
            if (cookiesEvent.Achievement_Completed())
            {
                key = "Match 5 candy";
                Notify_Achievement(key, value);
            }
        }

    }

    void Notify_Achievement(string key, string value){
        if (PlayerPrefs.GetInt(value) == 1)
        {
            return;
        }

        PlayerPrefs.SetInt(value, 1);
        achievementText.text = key + " Unlocked !";

        StartCoroutine(Show_Achievement_Banner());

    }

    void Active_Achievement_Banner(bool active){
        achievementBanner.gameObject.SetActive(active);
    }

    IEnumerator Show_Achievement_Banner(){
        Active_Achievement_Banner(true);
        yield return new WaitForSeconds(2f);
        Active_Achievement_Banner(false);
    }

}

public class MatchTileEvent : TileEvent
{
    private int matchCount;
    private int requiredAmount;

    public MatchTileEvent(int amout){
        requiredAmount = amout;
    }

    public override void On_Match()
    {
        matchCount++;
    }
    
    public override bool Achievement_Completed()
    {
        if(matchCount == requiredAmount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}

// public class CakeTileEvent : TileEvent
// {
//     private int matchCount;
//     private int requiredAmount;

//     public CakeTileEvent(int amout){
//         requiredAmount = amout;
//     }

//     public override void On_Match()
//     {
//         matchCount++;
//     }
    
//     public override bool Achievement_Completed()
//     {
//         if(matchCount == requiredAmount)
//         {
//             return true;
//         }
//         else
//         {
//             return false;
//         }
//     }

// }

// public class CandyTileEvent : TileEvent
// {
//     private int matchCount;
//     private int requiredAmount;

//     public CandyTileEvent(int amout){
//         requiredAmount = amout;
//     }

//     public override void On_Match()
//     {
//         matchCount++;
//     }
    
//     public override bool Achievement_Completed()
//     {
//         if(matchCount == requiredAmount)
//         {
//             return true;
//         }
//         else
//         {
//             return false;
//         }
//     }

// }

