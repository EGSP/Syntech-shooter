using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftTable : MonoBehaviour
{
    // Кнопка открытия стола
    [SerializeField] private KeyCode OpenKey;

    // Дальность открытия стола
    [SerializeField] private float OpenRadius;

    //Смещение центра радиуса
    [SerializeField] private Vector3 RadiusOffset;

    // Кнопка переключения разделов влево
    [SerializeField] private KeyCode LeftKey;

    // Кнопка переключения разделов вправо 
    [SerializeField] private KeyCode RightKey;

    // Слой по которому производится поиск игрока
    [SerializeField] private LayerMask PlayerLayer;

    // Окна стола крафта или по-другому разделы
    [SerializeField] private CraftTableWindow[] Windows;

    // Индекс окна, открываемого по стандарту
    [SerializeField] private int StartWindowIndex;

    // Контроллер игрока, может быть null
    private PlayerControllerComponent PlayerController;

    // Индекс текущего открытого окна
    private int currentWindowIndex;

    // Открыт ли сейчас стол крафта
    private bool IsOpened;

    // Start is called before the first frame update
    void Start()
    {
        if (Windows == null)
            throw new System.Exception("Windows[] is Null");

        // Ограничение значение индекса в пределах количества окон во избежание ошибок
        StartWindowIndex = Mathf.Clamp(StartWindowIndex, 0, Windows.Length-1);
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

        currentWindowIndex = StartWindowIndex;

        OpenWindow(currentWindowIndex);
    }
    
    /// <summary>
    /// Закрытие стола крафта.
    /// </summary>
    public void Close()
    {
        IsOpened = false;

        PlayerController.ActivateCamera();
        PlayerController.ActivateControll();

        CloseWindow(currentWindowIndex);

        PlayerController = null;
    }

    /// <summary>
    /// Смена текущего окна. Закрывает текущее окно
    /// </summary>
    private void ChangeWindow(int _CurrentWindowIndex,int _NextWindowIndex)
    {
        Windows[_CurrentWindowIndex].Close();
        Windows[_NextWindowIndex].Open(PlayerController);
    }

    /// <summary>
    /// Открытие конкретного окна. Не закрывает другие окна
    /// </summary>
    /// <param name="_OpenWindowIndex">Индекс окна, которое нужно открыть</param>
    private void OpenWindow(int _OpenWindowIndex)
    {
        Windows[_OpenWindowIndex].Open(PlayerController);
    }

    /// <summary>
    /// Закрытие конкретного окна.
    /// </summary>
    /// <param name="_CloseWindowIndex">Индекс окна, которое нужно закрыть</param>
    private void CloseWindow(int _CloseWindowIndex)
    {
        Windows[_CloseWindowIndex].Close();
    }


    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position+RadiusOffset, OpenRadius);
    }
}
