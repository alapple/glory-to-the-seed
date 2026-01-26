using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

/*
 * Manage the accept and the correct calls to randomizers and controllers on accept
 */
public class QuestsController : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;

    private VisualElement _root;

    private ResourceRandomizer _resourceRandomizer;
    private QuestAmountRandomizer _questAmountRandomizer;

    [FormerlySerializedAs("recourceManager")] [SerializeField]
    public ResourceManager resourceManager;


    void Awake()
    {
        _root = uiDocument.rootVisualElement;
        //TODO: change name to real one once created
        var acceptBtn = _root.Q<Button>("AcceptButton");

        acceptBtn.RegisterCallback<ClickEvent>(OnQuestAccept);
    }

    void Start()
    {
        _resourceRandomizer = new ResourceRandomizer();
        _questAmountRandomizer = new QuestAmountRandomizer();
    }

    void OnQuestAccept(ClickEvent clickEvent)
    {
        var resources = _resourceRandomizer.ReturnResources();
        int potatoAmount = _questAmountRandomizer.GetQuestAmount();
        resourceManager.SetResources(resources, potatoAmount);
    }
}