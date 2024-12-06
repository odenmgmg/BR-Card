using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AvatarImage : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    public Image image;
    public SpriteData spriteData;
    private TitleManager titleManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(TitleManager titleManager,SpriteData spriteData)
    {
        this.titleManager = titleManager;
        this.spriteData = spriteData;
        image.sprite = spriteData.sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
       titleManager.SetAvatar(spriteData);
    }
}
