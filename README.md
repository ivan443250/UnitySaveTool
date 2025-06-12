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
В таком примере все работает просто: есть ключ константа с помощью которого мы можем класть и доставать значение здоровья из PlayerPrefs, но есть несколько недостатков.

