# UnitySaveTool
Библиотека для удобного сохранения информации в вашем приложении на Unity
Принцип работы JSON + File.IO
# Основные преимущества:
* **Простота использования**
    - Если в проекте используется DI фреймворк, то сохраняемые типы данных будут зарегистрированы как зависимости и DI контейнер автоматически внедрит их.
    - Библиотека уже поддерживает такую работу с zenject, для работы с другими DI фреймворками смотри здесь.
* **Возможность выбирать контекст сохранения**
    - **[Глобальный контекст]** Для сохранения данных в области видимости всего проекта (Настройки, кеш и т.д.)
    - **[Контекст игрового прогресса]** Для данных игрового прогресса которые не привязаны к конкретной сцене (Инвентарь игрока, количество смертей и т.д.)
    - **[Контекст сцены]** Для сохранения данных о конкретной сцене. Данные сцены защищены от доступа или изменения из другой сцены (Случайно сгенерированная карта, позиции передвинутых игроком предметов и т.д.)
* **Модульность и расширяемость**
    - Можно заменить или расширить функционал отдельных частей, зарегистрировав собственные реализации интерфейсов библиотеки в DI контейнере
# Введение 
Возьмем тестовый класс TestSaveData, который нужно сохранить:
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
    private TestSaveData testSaveData;

    public void ChangeData()
    {
        //Изменяю TestSaveData
    }
}
```
















