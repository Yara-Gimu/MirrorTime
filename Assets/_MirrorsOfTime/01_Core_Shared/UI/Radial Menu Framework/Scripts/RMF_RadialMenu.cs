using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem; // النظام الجديد
using TMPro; // (التعديل الأول) إضافة مكتبة TextMeshPro

[AddComponentMenu("Radial Menu Framework/RMF Core Script")]
public class RMF_RadialMenu : MonoBehaviour {

    [HideInInspector]
    public RectTransform rt;

    [Tooltip("Adjusts the radial menu for use with a gamepad or joystick.")]
    public bool useGamepad = false;

    [Tooltip("With lazy selection, you only have to point your mouse (or joystick) in the direction of an element to select it.")]
    public bool useLazySelection = true;

    [Tooltip("If set to true, a pointer with a graphic of your choosing will aim in the direction of your mouse.")]
    public bool useSelectionFollower = true;

    [Tooltip("If using the selection follower, this must point to the rect transform of the selection follower's container.")]
    public RectTransform selectionFollowerContainer;

    [Tooltip("This is the text object that will display the labels of the radial elements.")]
    public TextMeshProUGUI textLabel; // (التعديل الثاني) تغيير النوع إلى TextMeshProUGUI

    [Tooltip("This is the list of radial menu elements. Order-dependent.")]
    public List<RMF_RadialMenuElement> elements = new List<RMF_RadialMenuElement>();

    [Tooltip("Controls the total angle offset for all elements.")]
    public float globalOffset = 0f;
    
    [Tooltip("اسحبي الأكشن (OpenRadialMenu) من مجلداتك هنا لتأكيد الاختيار عند فك الزر")]
    public InputActionReference openMenuAction;

    [HideInInspector]
    public float currentAngle = 0f;

    [HideInInspector]
    public int index = 0; 

    private int elementCount;
    private float angleOffset; 
    private int previousActiveIndex = 0; 
    private PointerEventData pointer;

    void Awake() {
        pointer = new PointerEventData(EventSystem.current);
        rt = GetComponent<RectTransform>();

        if (rt == null)
            Debug.LogError("Radial Menu: Rect Transform could not be found.");

        elementCount = elements.Count;
        angleOffset = (360f / (float)elementCount);

        for (int i = 0; i < elementCount; i++) {
            if (elements[i] == null) continue;
            elements[i].parentRM = this;
            elements[i].setAllAngles((angleOffset * i) + globalOffset, angleOffset);
            elements[i].assignedIndex = i;
        }
    }

    void Start() {
        if (useGamepad) {
            EventSystem.current.SetSelectedGameObject(gameObject, null); 
            if (useSelectionFollower && selectionFollowerContainer != null)
                selectionFollowerContainer.rotation = Quaternion.Euler(0, 0, -globalOffset); 
        }
    }

    void Update() {
        // 1. قراءة حركة عصا التحكم (الكنترولر) لتوجيه السهم
        Vector2 stickInput = Vector2.zero;
        bool joystickMoved = false;

        if (Gamepad.current != null)
        {
            stickInput = Gamepad.current.leftStick.ReadValue();
            joystickMoved = stickInput.sqrMagnitude > 0.1f; 
        }

        // 2. قراءة حركة الماوس لتوجيه السهم
        float rawAngle;
        if (!useGamepad) {
            Vector2 mousePos = Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
            rawAngle = Mathf.Atan2(mousePos.y - rt.position.y, mousePos.x - rt.position.x) * Mathf.Rad2Deg;
        } else {
            rawAngle = Mathf.Atan2(stickInput.y, stickInput.x) * Mathf.Rad2Deg;
        }

        // 3. حساب الزاوية
        if (!useGamepad)
            currentAngle = normalizeAngle(-rawAngle + 90 - globalOffset + (angleOffset / 2f));
        else if (joystickMoved)
            currentAngle = normalizeAngle(-rawAngle + 90 - globalOffset + (angleOffset / 2f));

        // 4. اختيار القدرة وتأكيدها
        if (angleOffset != 0 && useLazySelection) {
            index = (int)(currentAngle / angleOffset);

            if (elements[index] != null) {
                selectButton(index);

                // التأكيد باستخدام نظام الـ Actions الاحترافي عند رفع الإصبع عن الزر
                if (openMenuAction != null && openMenuAction.action.WasReleasedThisFrame()) {
                    ExecuteEvents.Execute(elements[index].button.gameObject, pointer, ExecuteEvents.submitHandler);
                }
            }
        }

        // 5. تحديث مؤشر الاتجاه (السهم الداخلي)
        if (useSelectionFollower && selectionFollowerContainer != null) {
            if (!useGamepad || joystickMoved)
                selectionFollowerContainer.rotation = Quaternion.Euler(0, 0, rawAngle + 270);
        } 
    }

    private void selectButton(int i) {
        if (elements[i].active == false) {
            elements[i].highlightThisElement(pointer); 
            if (previousActiveIndex != i) 
                elements[previousActiveIndex].unHighlightThisElement(pointer); 
        }
        previousActiveIndex = i;
    }

    private float normalizeAngle(float angle) {
        angle = angle % 360f;
        if (angle < 0) angle += 360;
        return angle;
    }
}