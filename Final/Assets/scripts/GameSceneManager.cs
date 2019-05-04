using UnityEngine.SceneManagement;
using UnityEngine;
using SWNetwork;
using System.Collections;

/// <summary>
/// Game scene manager manages game scenes state.
/// </summary>
public class GameSceneManager : MonoBehaviour
{
    public GameObject winnerPanel;
    public GameObject gameOverPanel;

    GameDataManager gameDataManager;

    void Start()
    {
        gameDataManager = GetComponent<GameDataManager>();
    }

    public void Exit()
    {
        NetworkClient.Instance.DisconnectFromRoom();
        NetworkClient.Lobby.LeaveRoom(HandleLeaveRoom);
    }

    void HandleLeaveRoom(bool okay, SWLobbyError error)
    {
        if (!okay)
        {
            Debug.LogError(error);
        }

        Debug.Log("Left room");
        SceneManager.LoadScene("lobbyScene");
    }

    public void OnPlayerScoresChanged()
    {
        Debug.Log("OnPlayerScoreChanged");
        PlayerScores playerScores = gameDataManager.GetPropertyWithName("PlayerScores").GetValue<PlayerScores>();
        Debug.Log(playerScores);

        if (playerScores != null && playerScores.scores != null)
        {
            foreach (Score s in playerScores.scores)
            {
                if (s.score >= 3)
                {
                    if (s.playerRemoteId == NetworkClient.Instance.PlayerId)
                    {
                        winnerPanel.gameObject.SetActive(true);
                    }
                    else
                    {
                        gameOverPanel.gameObject.SetActive(true);
                    }
                    break;
                }
            }
        }
    }

    public void PlayerScored(string playerId)
    {
        // Read the current value of the "PlayerScores" SyncProperty.
        PlayerScores playerScores = gameDataManager.GetPropertyWithName("PlayerScores").GetValue<PlayerScores>();

        // Initialize the playerScores object.
        if (playerScores == null)
        {
            playerScores = new PlayerScores();
        }

        bool foundPlayerScore = false;

        // If player already have a score, increase it by 1.
        foreach (Score s in playerScores.scores)
        {
            if (s.playerRemoteId == playerId)
            {
                s.score++;
                foundPlayerScore = true;
            }
        }

        // If player has not scored yet, add a new score for the player and set its value to 1.
        if (!foundPlayerScore)
        {
            Score ps = new Score();
            ps.playerRemoteId = playerId;
            ps.score = 1;
            playerScores.scores.Add(ps);
        }

        // Modify the "PlayerScores" SyncProperty
        gameDataManager.Modify<PlayerScores>("PlayerScores", playerScores);
    }

    // OnSpawnerReady(bool alreadySetup) method is added to handle the On Ready Event.
    public void OnSpawnerReady(bool alreadySetup)
    {
        Debug.Log("OnSpawnerReady " + alreadySetup);

        // Check alreadySetup to see if the scene has been set up before. 
        // If it is true, it means the player disconnected and reconnected to the game. 
        // In this case, we should not spawn a new Player GameObject for the player.
        if (!alreadySetup)
        {
            // If alreadySetup is false, it means the player just started the game. 
            // We randomly select a SpawnPoint and ask the SceneSpawner to spawn a Player GameObject. 
            // we have 1 playerPrefabs so playerPrefabIndex is 0.
            // We have 4 spawnPoints so we generated a random int between 0 to 3.
            int spawnPointIndex = Random.Range(0, 3);
            NetworkClient.Instance.LastSpawner.SpawnForPlayer(0, spawnPointIndex);

            // Tell the spawner that we have finished setting up the scene. 
            // alreadySetup will be true when SceneSpawn becomes ready next time.
            NetworkClient.Instance.LastSpawner.PlayerFinishedSceneSetup();
        }
    }

    public void DelayedRespawnPlayer()
    {
        StartCoroutine(RespawnPlayer(1f));
    }

    IEnumerator RespawnPlayer(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        // Respawn the player at a random SpawnPoint
        int spawnPointIndex = Random.Range(0, 3);
        NetworkClient.Instance.LastSpawner.SpawnForPlayer(0, spawnPointIndex);
    }
}
