
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Serialization;

public class CaptureButton : MonoBehaviour
{
    static float _flashTimer = 2.5f;
    [SerializeField] Button flashbutton;
    [SerializeField] Image cooldownFill;
    [SerializeField] Image flashImage;
    [SerializeField] float flashDuration = 0.5f;
    [SerializeField] GameManager gameManager;
    

    private bool _readyToFlash;
    float _countdown;
    
    private void Awake()
    {
        flashbutton.onClick.AddListener(OnCapturePressed);
    }

    void Start()
    {
        _countdown = _flashTimer;
    }

    private void Update()
    {

        if (gameManager.IsPlaying == false)
        {
            flashbutton.gameObject.SetActive(false);
            return;
        }
        else
        {
            flashbutton.gameObject.SetActive(true);
        }
        
        if (_readyToFlash == false)
        {
            _countdown -= Time.deltaTime;
            flashbutton.interactable = false;
            cooldownFill.fillAmount = _countdown / _flashTimer;
            
            if (_countdown <= 0)
            {
                _readyToFlash = true;
                flashbutton.interactable = true;
            }
        }
    }

    void OnCapturePressed()
    {
        if (_readyToFlash)
        {
            StartCoroutine(FlashRoutine());
            Debug.Log("Flashing");
        }
    }

    IEnumerator FlashRoutine()
    {
        _readyToFlash = false;
        _countdown = _flashTimer;
        
        gameManager.TryCapture();
        
        flashImage.gameObject.SetActive(true);
        float t = flashDuration;
        while (t > 0)
        {
            t -= Time.deltaTime;
            Color c = flashImage.color;
            c.a = Mathf.Clamp01(t / flashDuration);
            flashImage.color = c;
            yield return null;
        }
        flashImage.gameObject.SetActive(false);
    }
}
