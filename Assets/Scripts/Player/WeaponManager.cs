using UnityEngine;

/// <summary>
/// Управляет выбором и переключением оружия
/// </summary>
public class WeaponManager : MonoBehaviour
{
    public Camera playerCamera;
    public Transform weaponHolder; // пустой объект в руке, куда вешаем оружие

    public Weapon[] weapons;

    private int currentIndex = 0;

    private void Start()
    {
        EquipWeapon(0);
    }

    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
            NextWeapon();
        else if (scroll < 0f)
            PrevWeapon();
    }

    private void EquipWeapon(int index)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].gameObject.SetActive(i == index);
        }
        weapons[index].playerCamera = playerCamera;
        currentIndex = index;
    }

    private void NextWeapon()
    {
        int next = (currentIndex + 1) % weapons.Length;
        EquipWeapon(next);
    }

    private void PrevWeapon()
    {
        int prev = (currentIndex - 1 + weapons.Length) % weapons.Length;
        EquipWeapon(prev);
    }
}
