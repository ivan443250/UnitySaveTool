# UnitySaveTool
Библиотека для удобного сохранения информации в вашем приложении на Unity

# Введение
Предположим, вам нужно сделать механизм сохранения некоторых внутриигровых данных. Самое простое, что может прийти в голову это PlayerPrefs:
```c#
public class Player : MonoBehaviour
{
    private const string _healthKey = "PlayerHealth";
    private const int _maxHealth = 100;

    private int _health;

    private void Start()
    {
        _health = PlayerPrefs.GetInt(_healthKey, _maxHealth);
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetInt(_healthKey, _health);
        PlayerPrefs.Save();
    }
}
```
В настолько простом примере все работает: мы сохранили текущее значение здоровья игрока в PlayerPrefs с помощью ключа "PlayerHealth". 
