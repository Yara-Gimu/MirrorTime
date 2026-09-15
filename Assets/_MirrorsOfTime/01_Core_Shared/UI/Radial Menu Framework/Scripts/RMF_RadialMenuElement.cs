using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.Localization.Settings; // هذي المكتبة السحرية للترجمة

[AddComponentMenu("Radial Menu Framework/RMF Element")]
public class RMF_RadialMenuElement : MonoBehaviour {

    [HideInInspector]
    public RectTransform rt;
    [HideInInspector]
    public RMF_RadialMenu parentRM;

    [Tooltip("Each radial element needs a button. This is generally a child one level below this primary radial element game object.")]
    public Button button;

    [Tooltip("This is the text label that will appear in the center of the radial menu when this option is moused over. Best to keep it short.")]
    public string label;

    [Tooltip("اسحبي صورة HoverGlow اللي سويناها هنا")]
    public GameObject hoverGlow;

    [HideInInspector]
    public float angleMin, angleMax;

    [HideInInspector]
    public float angleOffset;

    [HideInInspector]
    public bool active = false;

    [HideInInspector]
    public int assignedIndex = 0;
    // Use this for initialization

    private CanvasGroup cg;

    void Awake() {

        rt = gameObject.GetComponent<RectTransform>();

        if (gameObject.GetComponent<CanvasGroup>() == null)
            cg = gameObject.AddComponent<CanvasGroup>();
        else
            cg = gameObject.GetComponent<CanvasGroup>();


        if (rt == null)
            Debug.LogError("Radial Menu: Rect Transform for radial element " + gameObject.name + " could not be found. Please ensure this is an object parented to a canvas.");

        if (button == null)
            Debug.LogError("Radial Menu: No button attached to " + gameObject.name + "!");

    }

    void Start () {

        rt.rotation = Quaternion.Euler(0, 0, -angleOffset); //Apply rotation determined by the parent radial menu.

        //If we're using lazy selection, we don't want our normal mouse-over effects interfering, so we turn raycasts off.
        if (parentRM.useLazySelection)
            cg.blocksRaycasts = false;
        else {

            //Otherwise, we have to do some magic with events to get the label stuff working on mouse-over.

            EventTrigger t;

            if (button.GetComponent<EventTrigger>() == null) {
                t = button.gameObject.AddComponent<EventTrigger>();
                t.triggers = new System.Collections.Generic.List<EventTrigger.Entry>();
            } else
                t = button.GetComponent<EventTrigger>();



            EventTrigger.Entry enter = new EventTrigger.Entry();
            enter.eventID = EventTriggerType.PointerEnter;
            enter.callback.AddListener((eventData) => { setParentMenuLable(label); });


            EventTrigger.Entry exit = new EventTrigger.Entry();
            exit.eventID = EventTriggerType.PointerExit;
            exit.callback.AddListener((eventData) => { setParentMenuLable(""); });

            t.triggers.Add(enter);
            t.triggers.Add(exit);



        }

    }
	
    //Used by the parent radial menu to set up all the approprate angles. Affects master Z rotation and the active angles for lazy selection.
    public void setAllAngles(float offset, float baseOffset) {

        angleOffset = offset;
        angleMin = offset - (baseOffset / 2f);
        angleMax = offset + (baseOffset / 2f);

    }

  public void highlightThisElement(PointerEventData p) {
        ExecuteEvents.Execute(button.gameObject, p, ExecuteEvents.selectHandler);
        active = true;
        setParentMenuLable(label);
        
        // تشغيل التوهج الخلفي المموه
        if (hoverGlow != null) hoverGlow.SetActive(true); 
        
    }

    //Sets the label of the parent menu. Is set to public so you can call this elsewhere if you need to show a special label for something.
 // Sets the label of the parent menu.
    public void setParentMenuLable(string l) {

        if (parentRM.textLabel != null)
        {
            // هنا نربط نظام الترجمة حقك!
            // بدال ما نعرض النص مباشرة، نرسل المفتاح (l) لدالة الترجمة
            string localizedText = GetLocalizedText(l); 
            
            parentRM.textLabel.text = localizedText;
        }
    }

// دالة مساعدة تجلب النص المترجم بناءً على النظام اللي تستخدمينه
    private string GetLocalizedText(string key)
    {
        // إذا كان المفتاح فاضي، لا تسوي شيء
        if (string.IsNullOrEmpty(key) || key == " ") 
            return " ";

        // هنا السكربت بيروح لجدولك "MyGameText" ويبحث عن المفتاح ويجيب الترجمة!
        return LocalizationSettings.StringDatabase.GetLocalizedString("MyGameText", key);
    }

public void unHighlightThisElement(PointerEventData p) {
        ExecuteEvents.Execute(button.gameObject, p, ExecuteEvents.deselectHandler);
        active = false;

        if (!parentRM.useLazySelection)
            setParentMenuLable(" ");
            
        // إطفاء التوهج الخلفي
        if (hoverGlow != null) hoverGlow.SetActive(false); 
        
    }
 




}
