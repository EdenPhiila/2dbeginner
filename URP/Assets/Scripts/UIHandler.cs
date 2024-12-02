using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    private VisualElement m_Healthbar;
    private VisualElement m_Manabar;
    public static UIHandler instance { get; private set; }

    // UI dialogue window variables
    public float displayTime = 4.0f;
    private VisualElement m_NonPlayerDialogue;
    private VisualElement m_NonPlayerDialogue1;
    private float m_TimerDisplay;


    // Awake is called when the script instance is being loaded (in this situation, when the game scene loads)
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        m_Healthbar = uiDocument.rootVisualElement.Q<VisualElement>("HealthBar");
        SetHealthValue(1.0f);

        // Changes the new Mana Bar
        m_Manabar = uiDocument.rootVisualElement.Q<VisualElement>("ManaBar");
        SetManaValue(1.0f);

        m_NonPlayerDialogue = uiDocument.rootVisualElement.Q<VisualElement>("NPCDialogue");
        m_NonPlayerDialogue.style.display = DisplayStyle.None;
        m_TimerDisplay = -1.0f;

        m_NonPlayerDialogue1 = uiDocument.rootVisualElement.Q<VisualElement>("NPCDialogue 1");
        m_NonPlayerDialogue1.style.display = DisplayStyle.None;


    }



    private void Update()
    {
        if (m_TimerDisplay > 0)
        {
            m_TimerDisplay -= Time.deltaTime;
            if (m_TimerDisplay < 0)
            {
                m_NonPlayerDialogue.style.display = DisplayStyle.None;
                m_NonPlayerDialogue1.style.display = DisplayStyle.None;
            }


        }
    }


    public void SetHealthValue(float percentage)
    {
        m_Healthbar.style.width = Length.Percent(100 * percentage);
    }

    //Connected to the new mana section of the HUD
    public void SetManaValue(float percentage)
    {
        m_Manabar.style.width = Length.Percent(100 * percentage);
    }

    public void DisplayDuckDialogue()
    {
        if (m_TimerDisplay > 0)
        { return; }
        m_NonPlayerDialogue.style.display = DisplayStyle.Flex;
        m_TimerDisplay = displayTime;
    }
    
    public void DisplayFrogDialogue()
    {
        if (m_TimerDisplay > 0)
        { return; }
        m_NonPlayerDialogue1.style.display = DisplayStyle.Flex;
        m_TimerDisplay = displayTime;
    }

}