using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using System;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; private set; }
    [SerializeField] private TMP_InputField inputName;
    [SerializeField] private GameObject panelJoin;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void OnDestroy()
    {
        Instance = null;
    }

    public void OnButtonHostClicked()
    {
        try
        {
            NetworkManager.Singleton.StartHost();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    public void OnButtonClientClicked()
    {
        NetworkManager.Singleton.StartClient();
    }
    public void OnButtonServerClicked()
    {
        NetworkManager.Singleton.StartServer();
    }

    public void OnButtonJoinClicked()
    {
        var text = inputName.text;
        if (string.IsNullOrEmpty(text)) { return; }
        NetworkSpawnController.Instance.CreateCharacterServerRpc(1);
    }

    public void ShowJoinPanel(bool value)
    {
        panelJoin.gameObject.SetActive(value);
    }
}
