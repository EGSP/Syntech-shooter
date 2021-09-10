using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

using AIB.AIBehaviours;

using TMPro;
public class RobotOverview : MonoBehaviour
{
    [Header("Window")]
    [SerializeField] private CraftRobotWindow CraftRobotWindow;
    
    [Header("Characters Logic")]
    [SerializeField] private RobotCharacters robotCharacters;
    /// <summary>
    /// Логика отрисовки характеристик
    /// </summary>
    public RobotCharacters RobotCharacters { get => robotCharacters; private set => robotCharacters = value; }

    [Header("UIElements")]
    [SerializeField] private RobotBuildPanel robotBuildPanel;
    public RobotBuildPanel RobotBuildPanel { get => robotBuildPanel; private set => robotBuildPanel = value; }
    
    [SerializeField] private RectTransform DefaultPanel;

    [Space(10)]
    [SerializeField] private RectTransform CharactersParent;

    [Space(10)]
    [SerializeField] private TMP_Text RobotName;
    [SerializeField] private TMP_Text RobotOwner;
    //Реальное изображение робота
    [SerializeField] private Image RobotImage;

  

    [Header("Prefabs")]
    [SerializeField] private RobotCharacterElement robotCharacterElement;
    /// <summary>
    /// Элемент характеристики
    /// </summary>
    public RobotCharacterElement RobotCharacterElement { get => robotCharacterElement; }

    /// <summary>
    /// Количество заранее создаваемых элементов характеристик
    /// </summary>
    [SerializeField] private int charactersBaseCount;
    public int CharactersBaseCount { get; private set; }

    public bool ToBuild
    {
        get => toBuild;
        set
        {
            if (toBuild == value)
                return;

            toBuild = value;

            if(toBuild == true)
            {
                BuildState();
            }
            else
            {
                DefaultState();
            }
        }
    }
    private bool toBuild;

    /// <summary>
    /// Текущий используемый робот
    /// </summary>
    public SignalAIBehaviour Robot
    {
        get => robot;
        set
        {
            if (robot == null)
            {
                // Очистка изображения
                UpdateRobotImage();
            }
            else
            {
                ClearCharacters();
            }

            robot = value;

            RobotCharacters.Robot = Robot;
            RobotName.text = Robot.RobotName;

            var data = CraftRobotWindow.RobotBuilder.Data
                .FirstOrDefault(x => x.CompanionBundle.ID == robot.ID);

            if (data != null)
            {
                // Если строим робота
                if (ToBuild == true)
                {
                    UpdateRobotImage(data);
                }
                else
                {
                    UpdateRobotImage(1);
                }
            }

        }
    }
    private SignalAIBehaviour robot;
    

    // Активные элементы берутся из неактивных. После отработки возвращаются
    private Stack<RobotCharacterElement> InactiveElements;
    private Stack<RobotCharacterElement> ActiveElements;


    private void Awake()
    {
        InactiveElements = new Stack<RobotCharacterElement>();
        ActiveElements = new Stack<RobotCharacterElement>();

        RobotBuildPanel.OnDetailsChange += UpdateRobotImage;

        for(int i = 0; i < charactersBaseCount; i++)
        {
            var instance = CreateCharacterElement(RobotCharacterElement);

            instance.transform.SetParent(CharactersParent, false);
            instance.gameObject.SetActive(false);

            InactiveElements.Push(instance);
        }

        robotCharacters.SetOverview(this);
    }
    
    /// <summary>
    /// Добавляет характеристику
    /// </summary>
    /// <param name="name">Название характеристики</param>
    /// <param name="value">Значение характеристики</param>
    /// <param name="opacity">Заполненность бара</param>
    public RobotCharacterElement AddCharacter(string name, float value, float opacity)
    {
        if (InactiveElements.Count > 0)
        {
            // Установка значений и активизация характеристики
            var character = InactiveElements.Pop();
            character.Name = name;
            character.Value = value;
            character.Opacity = opacity;

            character.gameObject.SetActive(true);
            ActiveElements.Push(character);

            return character;
        }

        InactiveElements.Push(CreateCharacterElement(RobotCharacterElement));

        // Попытка создать снова
        return AddCharacter(name, value, opacity);
    }

    /// <summary>
    /// Очищает и отключает все поля характеристик
    /// </summary>
    private void ClearCharacters()
    {
        var count = ActiveElements.Count;
        for(int i =0; i< count; i++)
        {
            var character = ActiveElements.Pop();
            character.gameObject.SetActive(false);
            InactiveElements.Push(character);
        }
    }
    

    /// <summary>
    /// Создание элементов характеристик
    /// </summary>
    /// <param name="prefab">Префаб с которого будет создана копия</param>
    private RobotCharacterElement CreateCharacterElement(RobotCharacterElement prefab)
    {
        var instance = Instantiate(prefab);

        return instance;
    }

    /// <summary>
    /// Открывает строительную панель
    /// </summary>
    private void BuildState()
    {
        RobotBuildPanel.gameObject.SetActive(true);
        DefaultPanel.gameObject.SetActive(false);

        RobotOwner.text = "TO BUILD";
    }

    /// <summary>
    /// Открывает стандартную панель
    /// </summary>
    private void DefaultState()
    {
        DefaultPanel.gameObject.SetActive(true);
        RobotBuildPanel.gameObject.SetActive(false);

        RobotOwner.text = "YOUR";
    }

    /// <summary>
    /// Обновление изображение робота
    /// </summary>
    /// <param name="data">Не может быть null</param>
    private void UpdateRobotImage(RobotBuilderData data)
    {
        RobotImage.sprite = data.CompanionBundle.companionImage;

        var material = RobotImage.material;
        material.SetFloat("_Opacity", data.BuildOpacity);
    }

    /// <summary>
    /// Очищает все параметры изображения
    /// </summary>
    private void UpdateRobotImage()
    {
        RobotImage.sprite = null;
    }

    /// <summary>
    /// Устанавливает opacity 
    /// </summary>
    private void UpdateRobotImage(float opacity)
    {
        var material = RobotImage.material;
        material.SetFloat("_Opacity", opacity);
    }
}