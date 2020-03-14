using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System.Text;


[RequireComponent(typeof(InventoryComponent))]
public class WeaponHolder : MonoBehaviour, IObservable, IMessageSender
{
    /// <summary>
    /// Максимальное количество слотов под оружие
    /// </summary>
    [SerializeField] private int MaxWeapons = 3;

    /// <summary>
    /// Слой капсулы с оружием
    /// </summary>
    [SerializeField] private LayerMask WeaponCapsuleLayer;

    /// <summary>
    /// Радиус в пределах которого происходит поиск оружия
    /// </summary>
    [SerializeField] private float GetUpRadius;

    /// <summary>
    /// Кнопка поднятия оружия
    /// </summary>
    [SerializeField] private KeyCode GetUpWeaponKey;

    /// <summary>
    /// Цвет выделаяемого текста
    /// </summary>
    [SerializeField] private Color HighlightTextColor;
    


    /// <summary>
    /// Список оружия
    /// </summary>
    public List<WeaponComponent> Weapons { get; private set; }
    
    /// <summary>
    /// Текущий индекс оружия в списке
    /// </summary>
    private int currentWeaponIndex;

    /// <summary>
    /// Оружие, которое нельзя выбрасывать или заменять
    /// </summary>
    private int lockedWeaponIndex;

    /// <summary>
    /// Инвентарь игрока
    /// </summary>
    private InventoryComponent playerInventory;

    /// <summary>
    /// Возвращает добавленное оружие
    /// </summary>
    public event Action<WeaponComponent> OnWeaponAdded = delegate { };
    public event Action<WeaponComponent, WeaponInfo> OnWeaponAddedExtended = delegate { };

    /// <summary>
    /// Возвращает новое оружие
    /// </summary>
    public event Action<WeaponComponent> OnWeaponChanged = delegate { };
    public event Action<WeaponComponent, WeaponInfo> OnWeaponChangedExtended = delegate { };

    /// <summary>
    /// Возвращает убранное из списка оружие
    /// </summary>
    public event Action<WeaponComponent> OnWeaponRemoved = delegate { };
    public event Action<WeaponComponent, WeaponInfo> OnWeaponRemovedExtended = delegate { };

    /// <summary>
    /// Срабатывает при вызове функции ChangeWeapon вне зависимости от результата
    /// </summary>
    public event Action OnWeaponChangeInitiated = delegate { };

    /// <summary>
    /// Возвращает отсортированный по расстоянию список оружия вокруг компонента. Список всегда заполнен
    /// </summary>
    public event Action<IOrderedEnumerable<WeaponCapsule>> OnWeaponAroundChecked = delegate { };

    public event Action<IObservable> OnForceUnsubcribe = delegate { };

    /// <summary>
    /// Возвращает сообщение
    /// </summary>
    public event Action<string> OnMessageSend = delegate { };

    private List<WeaponCapsule> weaponCapsulesAround;

    /// <summary>
    /// Закешированный экземпляр, передаваемый в события
    /// </summary>
    private WeaponInfo weaponInfo;

    private StringBuilder messageBuilder;

    void Awake()
    {
        Weapons = new List<WeaponComponent>(MaxWeapons);
        weaponInfo = new WeaponInfo();
        messageBuilder = new StringBuilder(70);

        weaponCapsulesAround = new List<WeaponCapsule>(5);
    }

    // Start is called before the first frame update
    void Start()
    {
        playerInventory = GetComponent<InventoryComponent>();
        if (playerInventory == null)
            throw new System.Exception("Инвентарь игрока не найден в WeaponHolder");
        
        

        lockedWeaponIndex = 0;
        
    }

    private void Update()
    {
        weaponCapsulesAround.Clear();

        var colliders = Physics.OverlapSphere(transform.position, GetUpRadius, WeaponCapsuleLayer, QueryTriggerInteraction.Collide);

        // Проход по всем попавшим объектам
        for(int i = 0; i < colliders.Length; i++)
        {
            var weaponCapsule = colliders[i].GetComponent<WeaponCapsule>();

            // Добавляем компонент во временный список ближайшего оружия
            if (weaponCapsule != null)
                weaponCapsulesAround.Add(weaponCapsule);
        }

        // Если хотя бы одна капсула рядом
        if(weaponCapsulesAround.Count != 0)
        {
            // Сортированная по дистанции коллекция
            var orderedWeaponsAround = weaponCapsulesAround.OrderBy(x => (x.transform.position - transform.position).magnitude);

            OnWeaponAroundChecked(orderedWeaponsAround);

            var firstElement = orderedWeaponsAround.First();
           
            // Отправка сообщения UI
            var message = CreateMessageText(firstElement.ReadWeapon().Name);

            OnMessageSend(message);

            // Если игрок нажал кнопку подбора оружия
            if (Input.GetKeyDown(GetUpWeaponKey))
            {
                AddWeapon(firstElement);
            }
        }
        else
        {
            OnMessageSend("");
        }
    }

    /// <summary>
    /// Возвращает текст сообщения для UI
    /// </summary>
    private string CreateMessageText(string weaponName)
    {
        messageBuilder.Clear();

        var colorCode = ColorUtility.ToHtmlStringRGBA(HighlightTextColor);

        messageBuilder.Append("Press <color=#");
        messageBuilder.Append(colorCode);
        messageBuilder.Append(">");
        messageBuilder.Append(GetUpWeaponKey.ToString());
        messageBuilder.Append("</color>");

        messageBuilder.Append(" to take ");

        messageBuilder.Append("<color=#");
        messageBuilder.Append(colorCode);
        messageBuilder.Append(">");
        messageBuilder.Append(weaponName);
        messageBuilder.Append("</color>");

        return messageBuilder.ToString();
    }

    /// <summary>
    /// Добавление нового оружия
    /// </summary>
    /// <param name="newWeapon">Новое оружие</param>
    /// <param name="swaped">Сменило ли новое оружие старое</param>
    /// <returns></returns>
    public WeaponComponent AddWeapon(WeaponCapsule weaponCapsule)
    {
        var newWeapon = weaponCapsule.ReadWeapon();

        // Подбор первого оружия
        if(Weapons.Count == 0)
        {
            newWeapon = weaponCapsule.TakeWeapon();

            lockedWeaponIndex = 0;
            Weapons.Add(newWeapon);

            weaponInfo.IndexInList = 0;
            weaponInfo.IsLocked = true;

            OnWeaponAdded(newWeapon);
            OnWeaponChanged(newWeapon);

            OnWeaponAddedExtended(newWeapon, weaponInfo);
            OnWeaponChangedExtended(newWeapon, weaponInfo);

            return newWeapon;
        }

        // Если нет места
        if (Weapons.Count == MaxWeapons)
        {
            // Если текущее оружие неизменяемо. То ничего не происходит
            if (currentWeaponIndex == lockedWeaponIndex)
                return Weapons[currentWeaponIndex];

            var capsPosition = weaponCapsule.transform.position;
            newWeapon = weaponCapsule.TakeWeapon();

            // Одинаково для удаления и добавления
            weaponInfo.IndexInList = currentWeaponIndex;
            weaponInfo.IsLocked = false;

            // Удаление оружия
            OnWeaponRemoved(Weapons[currentWeaponIndex]);
            OnWeaponRemovedExtended(Weapons[currentWeaponIndex], weaponInfo);
            
            //Weapons[currentWeaponIndex] = newWeapon;
            
            // Добавление оружия
            OnWeaponAdded(newWeapon);
            OnWeaponChanged(newWeapon);

            OnWeaponAddedExtended(newWeapon, weaponInfo);
            OnWeaponChangedExtended(newWeapon, weaponInfo);

            // Спавн капсулы для удаленного оружия
            var capsule = PoolManager.Instance.Take("WeaponCapsule") as WeaponCapsule;
            capsule.SetPosition(capsPosition);
            capsule.SetWeapon(Weapons[currentWeaponIndex]);

            Weapons[currentWeaponIndex] = newWeapon;


            return newWeapon;
        }

        newWeapon = weaponCapsule.TakeWeapon();

        Weapons.Add(newWeapon);
        OnWeaponAdded(newWeapon);

        weaponInfo.IndexInList = Weapons.Count - 1;
        weaponInfo.IsLocked = false;

        OnWeaponAddedExtended(newWeapon, weaponInfo);

        return Weapons[currentWeaponIndex];
    }

    /// <summary>
    /// Получение ссылки на оружие. Возвращает null, если нет оружия
    /// </summary>
    /// <param name="index">Индекс в списке оружия</param>
    /// <param name="changed">Сменилось ли оружие на новое</param>
    /// <returns></returns>
    public WeaponComponent ChangeWeapon(int index, out bool changed)
    {
        OnWeaponChangeInitiated();

        changed = false;

        if(Weapons.Count == 0)
        {
            // Нет оружия
            return null;
        }

        if(index > Weapons.Count-1)
        {
            // Если слоты пустые
            return null;
        }

        if(currentWeaponIndex == index)
        {
            // Не сменилось
            return Weapons[currentWeaponIndex];
        }
        else
        {
            // Сменилось
            currentWeaponIndex = index;
            changed = true;

            weaponInfo.IndexInList = currentWeaponIndex;
            weaponInfo.IsLocked = (currentWeaponIndex == lockedWeaponIndex);

            OnWeaponChanged(Weapons[currentWeaponIndex]);
            OnWeaponChangedExtended(Weapons[currentWeaponIndex], weaponInfo);

            return Weapons[index];
        }
    }

    public WeaponComponent GetCurrentWeapon()
    {
        if (Weapons.Count == 0)
            return null;

        return Weapons[currentWeaponIndex];
    }

    public void OnDrawGizmosSelected()
    {
        if (enabled == false)
            return;

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, GetUpRadius);
    }
}


/// <summary>
/// Класс содержит информацию об оружии в WeaponHolder
/// </summary>
public struct WeaponInfo
{
    /// <summary>
    /// Индекс оружия в списке оружий
    /// </summary>
    public int IndexInList { get; set; }

    /// <summary>
    /// Заблокировано ли оружие WeaponHolder`ом. 
    /// </summary>
    public bool IsLocked { get; set; }
}


