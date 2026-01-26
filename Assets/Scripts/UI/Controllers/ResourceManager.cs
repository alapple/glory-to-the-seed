using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/*
 * Manage the resource bar and the potato amount display of the quest
 */

//TODO: Change once ui is ready
public class ResourceManager : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;

    private VisualElement _root;

    //TODO: change label names to real ones
    private Label _vodkaAmount;
    private Label _questAmount;
    private Label _potatoAmount;
    private Label _megaphoneAmount;
    private Label _sealAmount;
    private Label _repairMaterialsAmount;
    private Label _workersAmount;

    private QuestAmountRandomizer _questAmountRandomizer;

    void Awake()
    {
        _root = uiDocument.rootVisualElement;

        //TODO: Set name to real one once created
        _vodkaAmount = _root.Q<Label>("Resources_Vodka");
        _questAmount = _root.Q<Label>("Quest_Request");
        _potatoAmount = _root.Q<Label>("Resources_Potato");
        _megaphoneAmount = _root.Q<Label>("Resources_Megaphone");
        _sealAmount = _root.Q<Label>("Resources_Seal");
        _repairMaterialsAmount = _root.Q<Label>("Resources_RepairMaterials");
        _workersAmount = _root.Q<Label>("Resources_Workers");
    }

    public void SetResources(Dictionary<string, int> resources, int potatoAmount)
    {
        _vodkaAmount.text = resources["Vodka"].ToString();
        _questAmount.text = potatoAmount.ToString();
        _potatoAmount.text = resources["Potato"].ToString();
        _megaphoneAmount.text = resources["Megaphone"].ToString();
        _sealAmount.text = resources["Seal"].ToString();
        _repairMaterialsAmount.text = resources["RepairMaterials"].ToString();
        _workersAmount.text = resources["Workers"].ToString();
    }

    public void ChangeResources(string uiTreeName, int amount)
    {
        switch (uiTreeName)
        {
            case "Vodka":
                _vodkaAmount.text = (Int32.Parse(_vodkaAmount.text) + amount).ToString();
                break;
            case "Potato":
                _potatoAmount.text = (Int32.Parse(_potatoAmount.text) + amount).ToString();
                break;
            case "Megaphone":
                _megaphoneAmount.text = (Int32.Parse(_megaphoneAmount.text) + amount).ToString();
                break;
            case "Seal":
                _sealAmount.text = (Int32.Parse(_sealAmount.text) + amount).ToString();
                break;
            case "RepairMaterials":
                _repairMaterialsAmount.text = (Int32.Parse(_repairMaterialsAmount.text) + amount).ToString();
                break;
            case "Workers":
                _workersAmount.text = (Int32.Parse(_workersAmount.text) + amount).ToString();
                break;
        }
    }
}