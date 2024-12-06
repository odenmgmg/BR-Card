using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class Info : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textName, textCost, textEffect;
    [SerializeField]
    private Image imageCardTypeBG, cardImage, typeImage;
    [SerializeField]
    private GameObject limitedImage, effectRoot, textPrefab, blockPrefab;
    [SerializeField]
    private GameObject cardPrefab, infoWimndow;
    [SerializeField]
    private TMP_ColorGradient normalGrad, limitedGrad;
    private LocalManager localManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        imageCardTypeBG.color = Color.clear;
        limitedImage.SetActive(false);
        textName.text = "";
        textCost.text = "";
        //textEffect.text = "";

    }

    // Update is called once per frame
    void Update()
    {

    }

    ///
    public void UpdateInfo(CardObject cardObject, LocalManager localManager, BattleManager battleManager)
    {
        this.localManager = localManager;

        foreach (Transform n in infoWimndow.transform)
        {
            Destroy(n.gameObject);
        }
        CardObject tmp = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity).GetComponent<CardObject>();
        tmp.Init(localManager, battleManager, cardObject.cardData, -1, -1, -1, CardArea.Pool);
        tmp.transform.SetParent(infoWimndow.transform, false);
        tmp.transform.localScale = Vector3.one * 3.5f;
        tmp.transform.localPosition = Vector3.zero;
        //tmp.animator.SetTrigger("move");
    }


    public void UpdateView(CardData cardData, LocalManager localManager)
    {
        this.localManager = localManager;
        DetailCardType detailCardType = localManager.staticData.detailCardTypes.Find(x => x.cardType == cardData.cardType);
        imageCardTypeBG.color = detailCardType.color;
        typeImage.sprite = detailCardType.image;
        cardImage.sprite = cardData.cardImage;
        limitedImage.SetActive(cardData.limited == "L");
        textName.text = cardData.cardName;
        textCost.text = cardData.cost;
        if (cardData.limited != "L") textName.colorGradientPreset = normalGrad;
        if (cardData.limited == "L") textName.colorGradientPreset = limitedGrad;

        SetText(cardData.effectText);
    }

    public void SetText(string text)
    {
        foreach (Transform n in effectRoot.transform)
        {
            Destroy(n.gameObject);
        }

        string[] lines = text.Split("<block>");
        foreach (string line in lines)
        {
            CardText cardText;
            string str = line;
            Sprite sprite = null;
            if (line.StartsWith("<box>"))
            {
                string[] tmp = line.Split("<box>");
                cardText = Instantiate(blockPrefab, Vector3.zero, Quaternion.identity).GetComponent<CardText>();
                string[] contents = tmp[1].Split("<div>");

                str = contents[1];
                SpriteData sd = localManager.iconSprites.data.Find(x => x.fileName == contents[0]);
                if (sd != null) sprite = sd.sprite;
            }
            else
            {
                cardText = Instantiate(textPrefab, Vector3.zero, Quaternion.identity).GetComponent<CardText>();
            }
            cardText.transform.SetParent(effectRoot.transform, false);
            cardText.transform.localScale = Vector3.one;
            cardText.transform.localPosition = Vector3.zero;
            ////cardText.SetContent(str, sprite);
        }
    }

}
