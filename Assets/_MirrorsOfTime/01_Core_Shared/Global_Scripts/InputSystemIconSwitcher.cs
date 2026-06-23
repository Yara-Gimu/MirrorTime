using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class InputSystemIconSwitcher : MonoBehaviour
{
    [Header("--- الأيقونة الأساسية (الأولى) ---")]
    public Sprite keyboardSprite;
    public Sprite xboxSprite;
    public Sprite playStationSprite;
    public Sprite defaultSprite; // الافتراضية

    [Header("--- الأيقونة الثانوية (الإضافية لـ L3+R3) ---")]
    [Tooltip("اسحبي مجسم الصورة الثاني هنا")]
    public Image secondaryImage; 
    public Sprite xboxSpriteSecondary;
    public Sprite playStationSpriteSecondary;

    private Image primaryImage;
    private PlayerInput playerInput; // 🌟 السر هنا، سحبناه من كودك القديم

    private void Awake()
    {
        primaryImage = GetComponent<Image>();
        playerInput = FindFirstObjectByType<PlayerInput>(); // يبحث عن نظام الإدخال بذكاء
    }

    private void OnEnable()
    {
        if (playerInput != null)
        {
            // الاستماع لتغير الجهاز من النظام الأساسي مباشرة
            playerInput.onControlsChanged += OnControlsChanged;
        }
        
        // تأخير بسيط لضمان تحديث الأيقونة عند ظهورها
        Invoke(nameof(UpdateIcon), 0.05f);
    }

    private void OnDisable()
    {
        if (playerInput != null)
        {
            playerInput.onControlsChanged -= OnControlsChanged;
        }
    }

    private void OnControlsChanged(PlayerInput input)
    {
        UpdateIcon();
    }

    public void UpdateIcon()
    {
        if (primaryImage == null || playerInput == null) return;

        // 🌟 نستخدم طريقتكِ المضمونة لمعرفة الجهاز الحالي
        string currentDevice = playerInput.currentControlScheme;

        // 1. حالة الكيبورد
        if (currentDevice == "Keyboard&Mouse" || currentDevice == "Keyboard")
        {
            SetSprites(keyboardSprite, null);
        }
        // 2. حالة يد التحكم
        else if (currentDevice == "Gamepad")
        {
            Gamepad gamepad = Gamepad.current;
            bool isPS = (gamepad is UnityEngine.InputSystem.DualShock.DualShockGamepad || (gamepad != null && gamepad.name.Contains("DualSense")));

            if (isPS)
            {
                SetSprites(playStationSprite, playStationSpriteSecondary);
            }
            else
            {
                SetSprites(xboxSprite, xboxSpriteSecondary);
            }
        }
        else
        {
            SetSprites(defaultSprite, null);
        }
    }

    private void SetSprites(Sprite primary, Sprite secondary)
    {
        primaryImage.sprite = primary;
        if (primaryImage.sprite != null) primaryImage.SetNativeSize();

        if (secondaryImage != null)
        {
            if (secondary != null)
            {
                secondaryImage.gameObject.SetActive(true);
                secondaryImage.sprite = secondary;
                secondaryImage.SetNativeSize();
            }
            else
            {
                secondaryImage.gameObject.SetActive(false); // تختفي تماماً في الكيبورد!
            }
        }
    }
}