using UnityEngine;

public enum FireMode
{
    SemiAuto,
    FullAuto,
    Melee
}

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName = "Weapon";
    public FireMode fireMode = FireMode.SemiAuto;

    [Header("General")]
    public float damage = 20f;
    public float fireRate = 0.2f;
    public float shootDistance = 50f;
    public float spreadAngle = 1.5f;

    [Header("Ammo")]
    public int magazineSize = 30;
    public int totalAmmo = 90;
    public float reloadTime = 2f;

    [Header("Melee")]
    public float meleeRange = 2f;
}
