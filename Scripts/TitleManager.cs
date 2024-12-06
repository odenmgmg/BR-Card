using System.IO;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class TitleManager : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputName, inputRoomID;
    [SerializeField]
    private TMP_Dropdown inputRuleID;
    [SerializeField]
    public Image avatar;
    [SerializeField] private GameObject avatarObj, avatarListArea, infoWindow,formatPrefab,formatArea;

    [SerializeField]
    private SpriteDataBase avatarSprites;
    [SerializeField]
    private LocalData localData;

    [SerializeField]
    private CSVDataIO csvData;
    [SerializeField]
    private AudioVolume audioVolume;

    [SerializeField]
    private DeckManager deckManager;
    [SerializeField]
    private LocalManager localManager;

    private bool initFlag = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initFlag = false;
        csvData.LoadUserFile(localData);


    }

    // Update is called once per frame
    void Update()
    {
        if (localData != null & !initFlag)
        {
            audioVolume.Setup(localData);
            avatarSprites = csvData.FetchAvatarSpriteData(avatarSprites);
            avatar.sprite = avatarSprites.data.Find(x => x.ID == localData.avatarID).sprite;
            inputName.text = localData.userName;
            foreach (Transform n in avatar.transform)
            {
                Destroy(n.gameObject);
            }
            foreach (var spriteData in avatarSprites.data)
            {
                AvatarImage tmp = Instantiate(avatarObj).GetComponent<AvatarImage>();
                tmp.Init(this, spriteData);
                tmp.transform.SetParent(avatarListArea.transform, false);
                tmp.transform.localPosition = new Vector3(0, 0, 50);
                tmp.transform.localScale = Vector3.one;

            }
            initFlag = true;
        }
    }

    public void OpenDeckMenu()
    {
        infoWindow.SetActive(true);
        deckManager.OpenDeckMenu(localManager);
    }
    public void CloseDeckMenu()
    {
        infoWindow.SetActive(false);
        deckManager.CloseDeckMenu();
    }

    public void StartMatching()
    {
        localData.roomID = inputRoomID.text;
        localData.ruleID = ((Rule)inputRuleID.value).ToString();
        localData.isMatching = true;
        SceneManager.LoadScene("BladeRondo");

    }
    public void StartWatching()
    {
        localData.roomID = inputRoomID.text;
        localData.ruleID = ((Rule)inputRuleID.value).ToString();
        localData.isMatching=false;
        SceneManager.LoadScene("BladeRondo");

    }



    public void SetAvatar(SpriteData spriteData)
    {
        avatar.sprite = spriteData.sprite;
        localData.avatarID = spriteData.ID;
        csvData.SaveUserFile(localData);

    }
    public void SetPlayerName()
    {
        string tmp = inputName.text;
        localData.userName = tmp;
        csvData.SaveUserFile(localData);
    }

    public void SetBGMVolume(float bgm)
    {
        localData.bgm = bgm;
        csvData.SaveUserFile(localData);

    }
    public void SetSEVolume(float se)
    {
        localData.se = se;
        csvData.SaveUserFile(localData);

    }


}
