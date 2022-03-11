using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Photon.Pun;
using Photon.Realtime;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Slider healthSlider, gunSlider;
    public Slider respawnCountdown;
    public PlayerHealth currentHealth;
    public Button respawnButton;
    public ConnectAndJoinRandom NetworkManager;
    public UnityEvent OnStartGame = new UnityEvent();
    public UnityEvent OnDead = new UnityEvent();
    public UnityEvent OnRespawn = new UnityEvent();
    public Image hitFlash;
    public PlayerShooting currentShooting;
    public Image playerArtwork;
    public Text nameText;
    private void Awake()
    {
        instance = this;
        OnStartGame.AddListener(LockCursor);
        OnDead.AddListener(UnLockCursor);
    }
    public void UnLockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void SetPlayerInfo(Sprite artWork, string playerName)
    {
        playerArtwork.sprite = artWork;
        nameText.text = playerName;
    }
    public void GetHit()
    {
        StartCoroutine(GetHitFlashEffect());
    }
    IEnumerator GetHitFlashEffect()
    {
        hitFlash.color = new Color(1,0,0,0.1f);
        yield return new WaitForSeconds(0.2f);
        hitFlash.color = new Color(1, 0, 0, 0);
    }
    void Update()
    {
        if (currentHealth != null)
        {
            healthSlider.value = currentHealth.CurrentHealth;
        }
        if (currentShooting != null)
        {
            gunSlider.value = currentShooting.Armor;
        }


        respawnDeltaCoutdown -= Time.deltaTime;
        respawnCountdown.value = respawnDeltaCoutdown;
    }
    float respawnDeltaCoutdown = 0;
    public void StartRespawn()
    {
        
        respawnButton.gameObject.SetActive(false);
        respawnCountdown.gameObject.SetActive(true);
        respawnDeltaCoutdown = 3;
        Invoke("Respawn", 3);
    }

    void Respawn()
    {
        PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
        respawnButton.gameObject.SetActive(true);
        respawnCountdown.gameObject.SetActive(false);
        NetworkManager.RespawnPlayer();
        OnRespawn.Invoke();
    }
    public GameObject damageText;
    public RectTransform rect;

    public void ShowHitDamage(Vector3 hitPosition , int damageValue)
    {
        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(hitPosition);
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * rect.sizeDelta.x) - (rect.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * rect.sizeDelta.y) - (rect.sizeDelta.y * 0.5f)));
        GameObject newDamageText = Instantiate(damageText, transform);
        newDamageText.transform.localPosition = WorldObject_ScreenPosition + new Vector2(Random.Range(0f,50f), Random.Range(0f, 50f));
        newDamageText.GetComponent<Text>().text = "-" + damageValue;
        Destroy(newDamageText, 0.2f);
    }
}
