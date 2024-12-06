using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    public BattleManager battleManager;
    public TitleManager titleManager;
    [SerializeField]
    private FuncButton funcButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {

        if (battleManager != null)
        {
            battleManager.localManager.audioSE.PlayOneShot(battleManager.localManager.audioData.audioSEClips[2]);
            switch (funcButton)
            {
                case FuncButton.TuenEnd:
                    battleManager.TurnEnd();
                    break;
                case FuncButton.PickFinish:
                    battleManager.deckManager.Finish();
                    break;
                case FuncButton.BattleEnd:
                    battleManager.BattleEnd();
                    break;
            }

        }
        if (titleManager != null)
        {
            switch (funcButton)
            {
                case FuncButton.Matching:
                    titleManager.StartMatching();
                    break;
                case FuncButton.Watching:
                    titleManager.StartWatching();
                    break;
                case FuncButton.OpenCardList:
                    titleManager.OpenDeckMenu();
                    break;
                case FuncButton.CloseCardList:
                    titleManager.CloseDeckMenu();
                    break;
            }
        }

    }
}
