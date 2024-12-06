using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TimeIcon : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Image image;
    [SerializeField]
    private SpriteDataBase spriteDataBase;
    [SerializeField]
    private LocalManager localManager;

    private bool isMoon = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoon) image.sprite = spriteDataBase.data.Find(x => x.fileName == "’‹").sprite;
        if (isMoon) image.sprite = spriteDataBase.data.Find(x => x.fileName == "–é").sprite;
    }

    public void Change()
    {
        Change(!isMoon);
    }
    public void Change(bool flag)
    {
        isMoon = flag;
        //localManager.ChangeTime();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        localManager.ChangeTime();
    }
}
