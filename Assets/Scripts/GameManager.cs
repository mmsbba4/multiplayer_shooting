using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Slider healthSlider;
    public Slider respawnCountdown;
    public PlayerHealth currentHealth;
    public Button respawnButton;
    public Photon.Pun.UtilityScripts.ConnectAndJoinRandom NetworkManager;
    public UnityEvent OnStartGame = new UnityEvent();
    public UnityEvent OnDead = new UnityEvent();
    public UnityEvent OnRespawn = new UnityEvent();
    private void Awake()
    {
        instance = this;
    }
    public void Death()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        OnDead.Invoke();
    }
    void Update()
    {
        if (currentHealth != null)
        {
            healthSlider.value = currentHealth.CurrentHealth;
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
        respawnButton.gameObject.SetActive(true);
        respawnCountdown.gameObject.SetActive(false);
        NetworkManager.RespawnPlayer();
        OnRespawn.Invoke();
    }
}
