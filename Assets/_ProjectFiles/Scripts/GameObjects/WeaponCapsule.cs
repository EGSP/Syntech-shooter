using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WeaponSystem;

public class WeaponCapsule : PooledObject
{
    /// <summary>
    /// Подбираемое оружие
    /// </summary>
    [Tooltip("Подбираемое оружие")]
    [SerializeField] private WeaponComponent Weapon;

    [Space(10)]
    [SerializeField] private float RotationSpeed;

    [Tooltip("Количество цветов должно совпадать с количеством значений редкости оружия")]
    [SerializeField] private Color[] Colors;

    /// <summary>
    /// Меш с шейдером капсулы
    /// </summary>
    private Mesh mesh;

    protected override void Awake()
    {
        base.Awake();

        mesh = GetComponent<MeshFilter>().mesh;

        if (Weapon != null)
            SetWeapon(Weapon);
    }

    /// <summary>
    /// Установка нового оружия.
    /// </summary>
    /// <param name="weapon">Оружие. Оно не должно быть null</param>
    public void SetWeapon(WeaponComponent weapon)
    {
        if (weapon == null)
            throw new System.Exception("New weapon is null");

        gameObject.SetActive(true);

        Weapon = weapon;

        Weapon.gameObject.SetActive(true);
        Weapon.transform.SetParent(transform, true);
        Weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localEulerAngles = Vector3.zero;

        // Изменение цвета вершин на значение соответствующее рекости
        mesh.colors = ColorToArray(
            mesh.vertices.Length,
            Colors[(int)Weapon.Rarity]);
    }

    /// <summary>
    /// Получение оружия
    /// </summary>
    public WeaponComponent TakeWeapon()
    {
        var weapon = Weapon;
        Weapon = null;

        InsertToPool();

        return weapon;
    }

    /// <summary>
    /// Получение ссылки на оружие без изменения состояния
    /// </summary>
    public WeaponComponent ReadWeapon()
    {
        return Weapon;
    }

    /// <summary>
    /// Установка позиции в мировых координатах
    /// </summary>
    /// <param name="position">Новая позиция</param>
    public void SetPosition(Vector3 position)
    {
        transform.position = position;
        transform.localEulerAngles = Vector3.zero;

        transform.parent = null;
    }

    /// <summary>
    /// Утилизация ненужной капсулы с оружием
    /// </summary>
    public void Utilize()
    {
        Destroy(gameObject);
    }

    public void Update()
    {
        if (Weapon != null)
            Weapon.transform.Rotate(0, RotationSpeed * Time.deltaTime, 0);
    }

    private Color[] ColorToArray(int length, Color color)
    {
        Color[] colors = new Color[length];

        for (int i = 0; i < length; i++)
            colors[i] = color;

        return colors;
    }
}
