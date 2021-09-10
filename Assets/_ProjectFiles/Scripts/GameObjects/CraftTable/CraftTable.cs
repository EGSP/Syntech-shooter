using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class CraftTable : MonoBehaviour
{
    /// <summary>
    /// Приоритет в интерфейсе
    /// </summary>
    [SerializeField] private int UIPriority;

    [Space(10)]
    [Header("OpenSettings")]
    // Кнопка открытия стола
    [SerializeField] private KeyCode OpenKey;

    // Дальность открытия стола
    [SerializeField] private float OpenRadius;

    //Смещение центра радиуса
    [SerializeField] private Vector3 RadiusOffset;

    [Space(10)]
    [Header("UseSettings")]
    // Кнопка переключения разделов влево
    [SerializeField] private KeyCode LeftKey;

    // Кнопка переключения разделов вправо 
    [SerializeField] private KeyCode RightKey;

    // Слой по которому производится поиск игрока
    [SerializeField] private LayerMask PlayerLayer;

    [SerializeField] private Transform spaceTransform;
    /// <summary>
    /// Позиция со свободным пространством, в котором можно спавнить объекты
    /// </summary>
    public Transform SpaceTransform { get => spaceTransform; private set => spaceTransform = value; }

    [Space(10)]
    [Header("Windows")]
    // Окна стола крафта или по-другому разделы
    [SerializeField] private CraftTableWindow[] Windows;

    // Индекс окна, открываемого по стандарту
    [SerializeField] private int StartWindowIndex;

    [SerializeField] private Camera WindowsCamera;

    // Контроллер игрока, может быть null
    private PlayerControllerComponent PlayerController;

    // Индекс текущего открытого окна
    private int currentWindowIndex;

    // Открыт ли сейчас стол крафта
    private bool IsOpened;

    // Start is called before the first frame update
    void Awake()
    {
        if (Windows == null)
            throw new System.Exception("Windows[] is null");

        if (SpaceTransform == null)
            throw new System.Exception("SpacePosition is null");

        if (WindowsCamera == null)
            throw new System.Exception("Camera is null");

        // Ограничение значение индекса в пределах количества окон во избежание ошибок
        StartWindowIndex = Mathf.Clamp(StartWindowIndex, 0, Windows.Length-1);

        for(int i = 0; i < Windows.Length; i++)
        {
            Windows[i].Initialize();
            Windows[i].SetCraftTable(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(OpenKey))
        {
            var colls = Physics.OverlapSphere(transform.position+RadiusOffset, OpenRadius, PlayerLayer);
            for(var i = 0; i < colls.Length; i++)
            {
                var playerController = colls[i].GetComponent<PlayerControllerComponent>();

                // Если игрок в радиусе открытия
                if(playerController != null)
                {
                    PlayerController = playerController;

                    Open();
                    return;
                }
            }
        }

        // Если открыт стол
        if(IsOpened == true)
        {
            // Выход из стола крафта
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Close();
                return;
            }

            if (Input.GetKeyDown(LeftKey))
            {
                var curIndex = currentWindowIndex;
                currentWindowIndex--;

                // Если меньше нуля, то начинаем с правого конца
                if (currentWindowIndex < 0)
                    currentWindowIndex = Windows.Length - 1;

                // Меняем окно
                ChangeWindow(curIndex, currentWindowIndex);
                return;
            }

            if (Input.GetKeyDown(RightKey))
            {
                var curIndex = currentWindowIndex;
                currentWindowIndex++;

                // Если больше количества окон, то начинаем с левого конца
                if (currentWindowIndex > Windows.Length-1)
                    currentWindowIndex = 0;

                // Меняем окно
                ChangeWindow(curIndex, currentWindowIndex);
                return;
            }
        }
    }

    /// <summary>
    /// Открытие стола крафта.
    /// </summary>
    private void Open()
    {
        IsOpened = true;

        PlayerController.DeactivateControll();
        PlayerController.DeactivateCamera();
        // Включаем камеру окон
        WindowsCamera.enabled = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        UIController.Instance.OpenCraftTable(UIPriority);

        currentWindowIndex = StartWindowIndex;

        OpenWindow(currentWindowIndex);

        
    }
    
    /// <summary>
    /// Закрытие стола крафта.
    /// </summary>
    public void Close()
    {
        IsOpened = false;
        
        // Отключаем камеру окон
        WindowsCamera.enabled = false;
        PlayerController.ActivateCamera();
        PlayerController.ActivateControll();

        UIController.Instance.CloseCraftTable(UIPriority);

        CloseWindow(currentWindowIndex);
        
        PlayerController = null;
    }

    /// <summary>
    /// Смена текущего окна. Закрывает текущее окно и открывает новое
    /// </summary>
    private void ChangeWindow(int _CurrentWindowIndex,int _NextWindowIndex)
    {
        CloseWindow(_CurrentWindowIndex);
        OpenWindow(_NextWindowIndex);
    }

    /// <summary>
    /// Открытие конкретного окна. Не закрывает другие окна
    /// </summary>
    /// <param name="_OpenWindowIndex">Индекс окна, которое нужно открыть</param>
    private void OpenWindow(int _OpenWindowIndex)
    {
        var window = Windows[_OpenWindowIndex];
        window.Open(PlayerController);

        AlignCamera(window);
    }

    /// <summary>
    /// Закрытие конкретного окна.
    /// </summary>
    /// <param name="_CloseWindowIndex">Индекс окна, которое нужно закрыть</param>
    private void CloseWindow(int _CloseWindowIndex)
    {
        Windows[_CloseWindowIndex].Close();
    }

    /// <summary>
    /// Изменяет положение камеры в зависимости от настроек окна
    /// </summary>
    /// <param name="window">Окно с настройками</param>
    private void AlignCamera(CraftTableWindow window)
    {
        WindowsCamera.transform.position = window.CameraPosition.position;
        WindowsCamera.transform.rotation = window.CameraPosition.rotation;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position+RadiusOffset, OpenRadius);
    }
}
