using SWNetwork;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages player Health point. Also updates player health bar when its health point changes
/// </summary>
public class PlayerHP : MonoBehaviour
{
    public int maxHp = 100;
    public Slider hpSlider;
    public GameObject explode;
    public int currentHP;

    NetworkID networkId;
    SyncPropertyAgent syncPropertyAgent;
    RemoteEventAgent remoteEventAgent;

    void Start()
    {
        hpSlider.minValue = 0;
        hpSlider.maxValue = maxHp;

        networkId = GetComponent<NetworkID>();
        syncPropertyAgent = gameObject.GetComponent<SyncPropertyAgent>();
        remoteEventAgent = gameObject.GetComponent<RemoteEventAgent>();
    }

    public void OnHPReady()
    {
        Debug.Log("OnHPPropertyReady");

        // Get the current value of the "hp" SyncProperty.
        currentHP = syncPropertyAgent.GetPropertyWithName("hp").GetIntValue();

        // Check if the local player has ownership of the GameObject. 
        // Source GameObject can modify the "hp" SyncProperty.
        // Remote duplicates should only be able to read the "hp" SyncProperty.
        if (networkId.IsMine)
        {
            int version = syncPropertyAgent.GetPropertyWithName("hp").version;

            if (version != 0)
            {
                // You can check the version of a SyncProperty to see if it has been initialized. 
                // If version is not 0, it means the SyncProperty has been modified before. 
                // Probably the player got disconnected from the game. 
                // Set hpSlider's value to currentHP to restore player's hp.
                hpSlider.value = currentHP;
            }
            else
            {
                // If version is 0, you can call the Modify() method on the SyncPropertyAgent to initialize player's hp to maxHp.
                syncPropertyAgent.Modify("hp", maxHp);
                hpSlider.value = maxHp;
            }
        }
        else
        {
            hpSlider.value = currentHP;
        }
    }

    public void OnHpChanged()
    {
        // Update the hpSlider when player hp changes
        currentHP = syncPropertyAgent.GetPropertyWithName("hp").GetIntValue();
        hpSlider.value = currentHP;

        if (currentHP == 0)
        {
            // invoke the "killed" remote event when hp is 0. 
            if (networkId.IsMine)
            {
                remoteEventAgent.Invoke("killed");
            }
        }
    }

    public void RemoteKilled()
    {
        Instantiate(explode, transform.position, Quaternion.identity);

        // Only the source player GameObject should be respawned. 
        // SceneSpawner will handle the remote duplicate creation for the respawned player.
        if (networkId.IsMine)
        {
            GameSceneManager gameSceneManager = FindObjectOfType<GameSceneManager>();

            // Call the DelayedRespawnPlayer() method you just added to the GameSceneManager.cs script. 
            gameSceneManager.DelayedRespawnPlayer();

            // NetworkID will find its SceneSpawner and remove its spawn records.
            // The Networked GameObject will be destroyed across the network.
            networkId.Destroy();
        }
    }

    public void ResetHP()
    {
        hpSlider.value = maxHp;
    }

    public void GotHit(int damage, string ownerId)
    {
        // Only the source GameObject can modify the "hp" SyncProperty.
        if (networkId.IsMine)
        {
            currentHP = syncPropertyAgent.GetPropertyWithName("hp").GetIntValue();

            // Check if the player is already dead.
            if (currentHP == 0)
            {
                return;
            }

            Debug.Log("Got hit: bullet owner= " + ownerId);

            Debug.Log("Got hit: old currentHP= " + currentHP);

            if (currentHP > 0)
            {
                currentHP = currentHP - damage;

                // if hp is lower than 0, set it to 0.
                if (currentHP < 0)
                {
                    currentHP = 0;
                }

                if (currentHP == 0)
                {
                    // call the PlayerScored() method if player hp reached 0. 
                    // GameSceneManager will update the player's score.
                    GameSceneManager gameSceneManager = FindObjectOfType<GameSceneManager>();
                    gameSceneManager.PlayerScored(ownerId);
                }
            }

            Debug.Log("Got hit: new currentHP= " + currentHP);

            // Apply damage and modify the "hp" SyncProperty.
            syncPropertyAgent.Modify("hp", currentHP);
        }
    }
}
