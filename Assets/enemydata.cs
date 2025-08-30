[System.Serializable]
public class EnemyData
{
    public int health;

    // Position + rotation
    public float posX, posY, posZ;
    public float rotY;

    // Turret
    public float turretRotY;

    // Shooting
    public float fireTimer;
}
