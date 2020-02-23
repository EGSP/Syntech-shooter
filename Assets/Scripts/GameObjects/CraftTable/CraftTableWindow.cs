using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class CraftTableWindow : MonoBehaviour
{
    // Позиция камеры в активном состоянии 
    [SerializeField] private Transform CameraPosition;

    // Канвас управление разделом крафта
    [SerializeField] private Canvas WindowCanvas;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Открытие окна
    /// </summary>
    /// <param name="_PlayerController">Компонент управления игрока</param>
    public abstract void Open(PlayerControllerComponent _PlayerController);

    /// <summary>
    /// Закрытие окна
    /// </summary>
    public abstract void Close();
}
