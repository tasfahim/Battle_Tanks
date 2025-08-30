using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public PlayerHealth player;

    public void SaveGame()
    {
        SaveData data = new SaveData();

        // --- Save Player ---
        if (player != null)
        {
            data.playerHealth = player.GetHealth();

            Vector3 pos = player.transform.position;
            data.playerPosX = pos.x;
            data.playerPosY = pos.y;
            data.playerPosZ = pos.z;

            data.playerRotY = player.transform.eulerAngles.y;

            TankController tc = player.GetComponent<TankController>();
            if (tc != null)
            {
                if (tc.turret != null)
                    data.turretRotY = tc.turret.localEulerAngles.y;

                if (tc.muzzle != null)
                    data.muzzleRotX = tc.muzzle.localEulerAngles.x;
            }
        }

        // --- Save Enemies ---
        EnemyTank[] enemies = FindObjectsOfType<EnemyTank>();
        foreach (EnemyTank et in enemies)
        {
            EnemyHealth eh = et.GetComponent<EnemyHealth>();
            if (eh == null) continue;

            EnemyData ed = new EnemyData();
            ed.health = eh.GetHealth();

            Vector3 pos = et.transform.position;
            ed.posX = pos.x;
            ed.posY = pos.y;
            ed.posZ = pos.z;

            ed.rotY = et.transform.eulerAngles.y;
            if (et.turret != null) ed.turretRotY = et.turret.localEulerAngles.y;

            // Save fire cooldown (private field access)
            var field = typeof(EnemyTank).GetField("fireTimer",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            ed.fireTimer = (float)field.GetValue(et);

            data.enemies.Add(ed);
        }

        // Save JSON
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("SaveGame", json);
        PlayerPrefs.Save();

        Debug.Log("✅ Game Saved: " + json);
    }

    public void LoadGame()
    {
        if (!PlayerPrefs.HasKey("SaveGame"))
        {
            Debug.LogWarning("⚠ No save data found!");
            return;
        }

        string json = PlayerPrefs.GetString("SaveGame");
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // --- Load Player ---
        if (player != null)
        {
            player.SetHealth(data.playerHealth);

            Vector3 savedPos = new Vector3(data.playerPosX, data.playerPosY, data.playerPosZ);
            player.GetComponent<TankController>()?.TeleportTo(savedPos);

            // Restore body rotation
            player.transform.rotation = Quaternion.Euler(0, data.playerRotY, 0);

            TankController tc = player.GetComponent<TankController>();
            if (tc != null)
            {
                if (tc.turret != null)
                    tc.turret.localRotation = Quaternion.Euler(0, data.turretRotY, 0);

                if (tc.muzzle != null)
                    tc.muzzle.localRotation = Quaternion.Euler(data.muzzleRotX, 0, 0);
            }
        }

        // --- Load Enemies ---
        EnemyTank[] enemies = FindObjectsOfType<EnemyTank>();
        for (int i = 0; i < data.enemies.Count && i < enemies.Length; i++)
        {
            EnemyData ed = data.enemies[i];
            EnemyTank et = enemies[i];

            Rigidbody er = et.GetComponent<Rigidbody>();
            if (er != null)
            {
                // Use physics-safe reposition
                er.position = new Vector3(ed.posX, ed.posY, ed.posZ);
                er.rotation = Quaternion.Euler(0, ed.rotY, 0);
                er.velocity = Vector3.zero;
                er.angularVelocity = Vector3.zero;
            }
            else
            {
                et.transform.position = new Vector3(ed.posX, ed.posY, ed.posZ);
                et.transform.rotation = Quaternion.Euler(0, ed.rotY, 0);
            }

            if (et.turret != null)
                et.turret.localRotation = Quaternion.Euler(0, ed.turretRotY, 0);

            EnemyHealth eh = et.GetComponent<EnemyHealth>();
            if (eh != null) eh.SetHealth(ed.health);

            // Restore fire cooldown
            var field = typeof(EnemyTank).GetField("fireTimer",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field.SetValue(et, ed.fireTimer);
        }

        Debug.Log("📥 Game Loaded: " + json);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5)) SaveGame(); // Save with F5
        if (Input.GetKeyDown(KeyCode.F9)) LoadGame(); // Load with F9
    }
}
