using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardText : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameObject disableImage, coinArea, coinPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetContent(string content, Sprite sprite, bool disableFlag)
    {
        if (content.StartsWith("<coin>"))
        {
            string[] tmp = content.Split("<coin>");
            for (int i = 0; i < int.Parse(tmp[1]); i++)
            {
                Transform coin = Instantiate(coinPrefab, Vector3.zero, Quaternion.identity).transform;
                coin.transform.SetParent(coinArea.transform, false);
            }

        }
        else
        {
            text.text = content;
        }
        if (sprite != null) image.sprite = sprite;
        if (disableImage != null)
        {
            disableImage.SetActive(disableFlag);
        }
    }

    public void SetContent(string content)
    {
        SetContent(content, null, false);

    }
}
