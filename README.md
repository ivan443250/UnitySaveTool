# UnitySaveTool
Библиотека для удобного сохранения информации в вашем приложении на Unity
## Основные преимущества:
* **Простота использования**
    - Если в проекте используется DI фреймворк, то сохраняемые типы данных будут зарегистрированы как зависимости и DI контейнер автоматически внедрит их.
    - Библиотека уже поддерживает такую работу с Zenject, для работы с другими DI фреймворками смотри здесь.
* **Возможность выбирать контекст сохранения**
    - **[Глобальный контекст]** Для сохранения данных в области видимости всего проекта (Настройки, кеш и т.д.)
    - **[Контекст игрового прогресса]** Для данных игрового прогресса которые не привязаны к конкретной сцене (Инвентарь игрока, количество смертей и т.д.)
    - **[Контекст сцены]** Для сохранения данных о конкретной сцене. Данные сцены защищены от доступа или изменения из другой сцены (Случайно сгенерированная карта, позиции передвинутых игроком предметов и т.д.)
* **Модульность и расширяемость**
    - Можно заменить или расширить функционал отдельных частей, зарегистрировав собственные реализации интерфейсов библиотеки в DI контейнере
* **Выбор между синхронной и асинхронной работой системы сохранения**
## Проблема, которую решает библиотека
Представим, что есть класс TestSaveData, который нужно сохранить:
```C#
[Serializable]
public class TestSaveData
{
    // Базовые типы данных
    public int playerLevel = 0;
    public float playerHealth = 0f;
    public string playerName = "";
    public bool isPremium = false;

    // Дата и время
    public DateTime lastSaveTime = default;
    public TimeSpan playTime = default;

    // Коллекции
    public List<string> unlockedAchievements = new List<string>();
    public Dictionary<string, int> inventory = new Dictionary<string, int>();

    // Массивы
    public Vector3[] checkpoints = Array.Empty<Vector3>();

    // Вложенные классы
    public PlayerStats stats = new PlayerStats();
    public List<QuestProgress> activeQuests = new List<QuestProgress>();

    // Специфичные для Unity типы
    public Color playerColor = default;
    public Vector3 playerPosition = default;
    public Quaternion playerRotation = default;

    // Перечисления
    public Difficulty difficulty = default;
    public CharacterClass characterClass = default;

    // Словарь магазина
    public SerializableDictionary<string, ShopItem> shopItems = new SerializableDictionary<string, ShopItem>();

    // Перечисления
    public enum Difficulty { Easy, Normal, Hard }
    public enum CharacterClass { Warrior, Mage, Rogue, Archer }
}
```
А также класс Player, который изменяет состояния в TestSaveData:
```C#
public class Player : MonoBehaviour
{
    private TestSaveData _testSaveData;

    public void ChangeData()
    {
        //Изменяю _testSaveData
    }
}
```
Нет причин пере сохранения TestSaveData в середине жизненного цикла класса Player. Поэтому достаточно просто гарантировать, что вначале жизненного цикла класса Player класс TestSaveData загрузился из памяти, а в конце был сохранен обратно. Вот обычная реализация этой идеи на практике (JSON + File IO):
```C#
public class Player : MonoBehaviour
{
    private TestSaveData _testSaveData;
    private string _savePath;

    public void ChangeData()
    {
        //Изменяю _testSaveData
    }

    private void Awake()
    {
        _savePath = Path.Combine(Application.persistentDataPath, "saveData.json");
        LoadData();
    }

    private void OnDestroy()
    {
        SaveData();
    }

    private void LoadData()
    {
        if (File.Exists(_savePath))
        {
            string jsonData = File.ReadAllText(_savePath);
            _testSaveData = JsonUtility.FromJson<TestSaveData>(jsonData);
        }
        else
        {
            _testSaveData = new TestSaveData();
        }
    }

    private void SaveData()
    {
        string jsonData = JsonUtility.ToJson(_testSaveData, true);
        File.WriteAllText(_savePath, jsonData);
    }
}
```

Такая реализация будет работать, но есть ряд больших недостатков:
* Нарушается один из принципов SOLID (Single Responsibility Principle). Подразумевается, что класс Player обрабатывает и изменяет информацию во время игры (Для примера это метод ChangeData). Но кроме этого Player также занимается логикой сохранения этой информации на довольно низком уровне (Вплоть до записи в файл)
* Большое количество объектов в игре, также требующих сохранения, создадут хаос в месте, где они расположены. Сохранение объектов подобным образом, без единой системы сохранения, может привести к ошибкам из-за одинакового названия файлов или еще чего-то
* Нет возможности создать еще одну ячейку игрового прогресса. Чтобы все сцены были одинаковыми, а разными на них только данные созданные игроком (ресурсы игрока, изменения на карте от действий игрока и т.д.)

Такие рассуждения могут привести к идее рефакторинга с вынесением системы сохранения в отдельный сервис. Вот пример рефакторинга с использованием Zenject:
```C#
public class Player : MonoBehaviour
{
    private TestSaveData _testSaveData;
    private string _savePath;

    private ISaveService _saveService;

    [Inject]
    public void Construct(ISaveService saveService)
    {
        _saveService = saveService;

        _savePath = Path.Combine(Application.persistentDataPath, "saveData.json");
        _testSaveData = _saveService.Load<TestSaveData>(savePath);
    }

    public void ChangeData()
    {
        //Изменяю _testSaveData
    }

    private void OnDestroy()
    {
        _saveService.Save(_testSaveData, _savePath);
    }
}
```
Несмотря на некоторые оставшиеся минусы, такое решение уже имеет место быть. С большим количеством данных все еще есть вероятность ошибок, ведь путь к файлам и их названия указываются разработчиком вручную. Также можно забыть загрузить или сохранить данные в начале и в конце жизненного цикла класса, работающего с этими данными. Несмотря на приемлемость такого способа, хочется иметь еще более простой и в то же время гибкий способ сохранять данные в игре. Именно такой способ и предоставляет библиотека UnitySaveTool.

## Решение с UnitySaveTool
Следующий пример будет показан с использованием DI фреймворка Zenject. Рекомендуется использовать библиотеку в сочетании с каким-либо DI контейнером. Как подключить DI контейнер, помимо Zenject, смотри здесь

1. Скачайте и установите в свой проект UnitySaveTool.unitypackage. Все будет установлено в [Assets => Plugins => UnitySaveTool]
2. Добавьте на префаб ProjectContext компонент SaveToolProjectInstaller:
   
![ProjectInstaller.png](ReadmeImages/ProjectInstaller.png)

3. Также следует добавить SaveToolSceneInstaller на все контексты сцен, где вы хотите использовать сохранения.

4. Чтобы указать, что тип данных не зарегистрирован в DI контейнере заранее и что библиотека должна зарегистрировать его там сама, а также чтобы показать в каком контексте сохранять этот тип данных, нужно классу данных добавить атрибут SaveToolData:
```C#
[Serializable]
[SaveToolData(SaveContext.Scene)] 
public class TestSaveData
{
    ...
}
// SaveContext.Scene указывает, что класс будет храниться в контексте сцен, на которых используется
```

5. Все, что осталось сделать это просто запросить сохраняемые данные из контейнера
```C#
public class Player : MonoBehaviour
{
    [SerializeField] private TestSaveData _testSaveData;

    [Inject]
    public void Construct(TestSaveData testSaveData)
    {
        _testSaveData = testSaveData;
    }

    public void ChangeData()
    {
        //Изменяю _testSaveData
    }
}
```

Теперь вы можете создать сколько угодно сохраняемых типов данных, которые будут использоваться на сцене. Единственное условие - это наличие у сохраняемых типов данных атрибутов:
```C#
[Serializable]
[SaveToolData(...)]
```

Все, что нужно объектам, использующим эти типы данных это просто получить эти типы из контейнера. 
Все объекты, представляющие, данные будут загружены при старте сцены, а их измененные версии будут сохранены при выходе из сцены (Работает только со ссылочными типами, так как сохраняется объект, который лежит в контейнере. В следующих версиях будет добавлен механизм установки права на изменение объекта (в том числе и вещественного), чтобы любой класс на сцене не мог изменить любой ссылочный тип данных простым получением его из контейнера)

## Как хранятся данные?
После сохранений структура файлов по пути Application.persistentDataPath будет выглядеть так:
```
=> Папка с названием проекта
    => UnitySaveTool
        => SavedContext.json
        => 0
            => Папка с названием сцены
                => TestSaveData.json
```
Папка "0" это папка которая будет хранить в себе данные игрового прогресса. 0 - это как бы индекс ячейки сохранения, а таких ячеек можно создать сколько угодно и настроить между ними удобное переключение. SavedContext.json как раз запоминает какая ячека выбрана и библиотека работает с ней по умолчанию. В папке UnitySaveTool помимо SavedContext.json и папок с ячейками сохранения будут также храниться данные, которые были сохранены в глобальном контексте. И наконец папки с названием сцен будут хранить в себе соответственно данные с этих сцен. 

Приемущество библиотеки - поддержка четкой структуры в которой сохраняются файлы, и набор инструментов для удобного взаимодействия с ней

## Центральная система управления сохранениями
В библиотеке за логику переключения между контекстами сохранения отвечает реализация IDataExplorer

Реализация этого интерфейса по умолчанию регистрируется в контейнер ProjectContext из установщика SaveToolProjectInstaller и вы всегда можете получть зависимость IDataExplorer из контейнера:
```C#
[Inject]
public void Construct(IDataExplorer dataExplorer)
{
    _dataExplorer = dataExplorer;
}
```

С помощью IDataExplorer можно обратиться к трем разным контекстам:
```C#
IGlobalDataContext globalContext = _dataExplorer.GlobalDataSet; // Глобальный контекст (1)
ISaveCellContext saveCellContext = _dataExplorer.SaveCellDataSet; // Контекст ячейки сохранения (2)
ISceneDataContext sceneDataContext = _dataExplorer.SceneDataSet; // Контекст сцены (3)

// Метод GetData для всех контекстов:
G globalData = globalContext.GetData<G>();
T saveCellData = saveCellContext.GetData<T>();
S sceneData = sceneDataContext.GetData<S>();

// Метод Save для всех контекстов:
globalContext.Save<G>(globalData);
saveCellContext.Save<T>(saveCellData);
sceneDataContext.Save<S>(sceneData);
```
## (1) Глобальный контекст
У приложения этот контекст всегда неизменен. Все данные, сохраненные в этом контексте будут доступны для использования, в любой ячейке cохранения и в любой сцене.
Также с помощью глобального контекста вы можете открыть ячейку сохранения:
```C#
globalContext.OpenSaveCell();
// Или
globalContext.OpenSaveCell(0);
```
Вызов метода OpenSaveCell без параметра откроет последнюю использованную ячейку, либо вы можете явно указать, какую ячейку хотите открыть.
Если ячейки с индексом, который вы указали, не существует, то папка сохранения с таким индексом будет создана автоматически.
Метод OpenSaveCell читает всю информацию (кроме папок со сценами) в папке с индексом ячейки, десериализует в объекты и кеширует. Благодаря этому получение данных (GetData<T>()) из ячейки после вызова OpenSaveCell происходит моментально.

## (2) Контекст ячейки сохранения
Контекст остается неизменным, когда игра переключается между разными сценами, но вы можете изменить контекст вручную, способом, указанным выше.

Для загрузки информации сцены можно использовать метод у ISaveCellContext:
```C#
saveCellContext.OpenScene("SampleScene"); // Где "SampleScene" это название сцены 
```

Либо если ISaveCellContext мог быть еще не открыт, можно использовать метод у IDataExplorer (рекомендуется):
```C#
_dataExplorer.OpenSceneDataSet("SampleScene");
```
Этот метод сначала проверит открыт ли ISaveCellContext и только потом вызовет у него OpenScene
## (3) Контекст сцены
Этот контекст самый низкий по уровню и не может хранить в себе другие контексты. Также он будет заменен на новый созданный ISceneDataContext при переключении сцены (если на новой сцене есть SaveToolSceneInstaller)
В отличие от свойств GlobalDataSet и SaveCellDataSet у IDataExplorer, свойство SceneDataSet может вернуть null, если в приложении еще не было открыто ни одной сцены с SaveToolSceneInstaller или если еще ни разц не вызывался метод (ISceneDataContext.OpenScene или IDataExplorer.OpenSceneDataSet)

## Итог
Такое разделение на контексты позволит избежать неприятной работы со строками (путями файлов, названиями файлов), вместо этого есть просто методы для открытия и работы с нужным контекстом
 


