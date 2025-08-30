using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private int aliveEnemies;

    void Awake()
    {
        // ✅ Simpler: GameManager resets every scene reload
        Instance = this;
    }

    void Start()
    {
        ResetEnemyCount();
    }

    public void EnemyKilled()
    {
        aliveEnemies = Mathf.Max(0, aliveEnemies - 1);
        Debug.Log("📉 Enemy killed. Remaining enemies = " + aliveEnemies);

        if (aliveEnemies == 0)
        {
            Debug.Log("🎉 All enemies destroyed → Mission Complete!");
            StartCoroutine(ShowMissionCompleteDelayed());
        }
    }

    private IEnumerator ShowMissionCompleteDelayed()
    {
        yield return new WaitForSeconds(1f);
        UIManager ui = FindObjectOfType<UIManager>();
        if (ui != null) ui.ShowMissionComplete();
    }

    public void ResetEnemyCount()
    {
        // ✅ Freshly count active enemies every time
        EnemyHealth[] enemies = FindObjectsOfType<EnemyHealth>();
        aliveEnemies = 0;
        foreach (var e in enemies)
        {
            if (e.gameObject.activeInHierarchy) aliveEnemies++;
        }
        Debug.Log("🔄 Enemy counter reset → " + aliveEnemies);
    }
}
