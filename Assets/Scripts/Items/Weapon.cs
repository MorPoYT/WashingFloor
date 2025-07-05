using UnityEngine;
using System.Collections;

/// <summary>
/// Универсальное оружие, берет настройки из WeaponData
/// </summary>
public class Weapon : MonoBehaviour
{
    public WeaponData weaponData;
    public Camera playerCamera;

    private int currentAmmo;
    private int totalAmmo;
    private bool isReloading;
    private float lastFireTime;


    private void OnEnable()
    {
        if (weaponData.fireMode != FireMode.Melee)
        {
            currentAmmo = weaponData.magazineSize;
            totalAmmo = weaponData.totalAmmo;
        }
    }

    private void Update()
    {
        if (isReloading)
            return;

        bool shootInput = false;
        if (weaponData.fireMode == FireMode.SemiAuto)
            shootInput = Input.GetMouseButtonDown(0);
        else if (weaponData.fireMode == FireMode.FullAuto)
            shootInput = Input.GetMouseButton(0);
        else if (weaponData.fireMode == FireMode.Melee)
            shootInput = Input.GetMouseButtonDown(0);

        if (shootInput)
            TryShoot();

        if (weaponData.fireMode != FireMode.Melee && Input.GetKeyDown(KeyCode.R))
        {
            if (currentAmmo < weaponData.magazineSize && totalAmmo > 0)
                StartCoroutine(Reload());
        }
    }

    private void TryShoot()
    {
        if (Time.time - lastFireTime < weaponData.fireRate)
            return;

        lastFireTime = Time.time;

        if (weaponData.fireMode == FireMode.Melee)
        {
            // МЕЛИЕ
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, weaponData.meleeRange))
            {
                IDamageable dmg = hit.collider.GetComponent<IDamageable>();
                if (dmg != null)
                    dmg.TakeDamage(weaponData.damage);
            }
            Debug.Log("Удар ближнего боя");
        }
        else
        {
            if (currentAmmo <= 0)
            {
                Debug.Log("Нет патронов");
                return;
            }
            currentAmmo--;

            // Рандомный разброс
            Vector3 direction = playerCamera.transform.forward;
            direction = Quaternion.Euler(
                Random.Range(-weaponData.spreadAngle, weaponData.spreadAngle),
                Random.Range(-weaponData.spreadAngle, weaponData.spreadAngle),
                0) * direction;

            Ray ray = new Ray(playerCamera.transform.position, direction);
            if (Physics.Raycast(ray, out RaycastHit hit, weaponData.shootDistance))
            {
                IDamageable dmg = hit.collider.GetComponent<IDamageable>();
                if (dmg != null)
                    dmg.TakeDamage(weaponData.damage);
            }

            Debug.Log($"Выстрел из {weaponData.weaponName}");
        }
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Перезарядка...");
        yield return new WaitForSeconds(weaponData.reloadTime);

        int needed = weaponData.magazineSize - currentAmmo;
        int ammoToLoad = Mathf.Min(needed, totalAmmo);

        currentAmmo += ammoToLoad;
        totalAmmo -= ammoToLoad;

        Debug.Log($"Перезаряжено. {currentAmmo}/{totalAmmo}");

        isReloading = false;
    }
}
