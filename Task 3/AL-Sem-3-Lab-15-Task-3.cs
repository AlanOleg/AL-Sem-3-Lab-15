using System;

public class SingleRandomizer
{
    // Поле для хранения единственного экземпляра класса
    private static readonly Lazy<SingleRandomizer> _instance =
        new Lazy<SingleRandomizer>(() => new SingleRandomizer());

    // Генератор случайных чисел
    private readonly Random _random;

    // Закрытый конструктор для предотвращения создания экземпляров извне
    private SingleRandomizer()
    {
        _random = new Random();
    }

    // Публичный статический метод для получения экземпляра
    public static SingleRandomizer Instance => _instance.Value;

    // Метод для получения следующего случайного числа
    public int GetNext()
    {
        lock (_random) // Блокируем генератор для потокобезопасного доступа
        {
            return _random.Next();
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Создадим несколько потоков для демонстрации работы
        for (int i = 0; i < 5; i++)
        {
            System.Threading.Thread thread = new System.Threading.Thread(() =>
            {
                for (int j = 0; j < 5; j++)
                {
                    int randomNumber = SingleRandomizer.Instance.GetNext();
                    Console.WriteLine($"Generated: {randomNumber}");
                }
            });

            thread.Start();
        }
    }
}