using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    // --- Player ---
    public int playerHealth;
    public float playerPosX, playerPosY, playerPosZ;
    public float playerRotY;
    public float turretRotY;
    public float muzzleRotX;

    // --- Enemies ---
    public List<EnemyData> enemies = new List<EnemyData>();

    // --- Optional game info ---
    public int currentScore;
    public int currentWave;
}
