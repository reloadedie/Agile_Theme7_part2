
namespace Agile_Theme7_part2
{
    /// <summary>
    /// Замена простого поля объектом
    /// </summary>
    public class Task6
    {
        /// <summary>
        /// Старт 6 задачи
        /// </summary>
        public static void StartTask()
        {
            Console.WriteLine("Задача 6 - замена простого поля объектом");

            Console.WriteLine("=== Старый подход ===");
            var oldUser = new OldUser("Иван", 25);
            Console.WriteLine($"Пользователь: {oldUser.Name}, возраст: {oldUser.Age}");
            oldUser.Age = -5; // Проблема: можно установить некорректное значение
            Console.WriteLine($"После установки некорректного возраста: {oldUser.Age}");

            // Новый подход (с объектом)
            Console.WriteLine("\n=== Новый подход ===");
            var newUser = new NewUser("Мария", new Age(25));
            Console.WriteLine($"Пользователь: {newUser.Name}, возраст: {newUser.Age.Value}");

            // Попытка создать некорректный возраст
            try
            {
                var invalidAge = new Age(-10);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            // Демонстрация дополнительных возможностей объекта Age
            Console.WriteLine("\n=== Дополнительные возможности ===");
            var age1 = new Age(18);
            var age2 = new Age(25);

            Console.WriteLine($"Возраст 1: {age1}");
            Console.WriteLine($"Возраст 2: {age2}");
            Console.WriteLine($"Возраст 1 старше возраста 2? {age1 > age2}");
            Console.WriteLine($"Возраст 1 младше возраста 2? {age1 < age2}");
            Console.WriteLine($"Возраст 1 равен возрасту 2? {age1 == age2}");

            // Методы для работы с возрастом
            var age3 = new Age(30);
            Console.WriteLine($"\nВозраст {age3} в днях (приблизительно): {age3.ToDays()}");
            Console.WriteLine($"Может голосовать: {age3.CanVote()}");
            Console.WriteLine($"Может работать: {age3.CanWork()}");

            // Изменение возраста через метод
            age3.IncreaseYear();
            Console.WriteLine($"После увеличения на год: {age3}");
        }
    }

    /// <summary>
    /// Старая версия с простым полем
    /// </summary>
    public class OldUser
    {
        public string Name { get; set; }
        public int Age { get; set; } // Простое поле - нет валидации

        public OldUser(string name, int age)
        {
            Name = name;
            Age = age;
        }
    }

    /// <summary>
    /// Новая версия с объектом Age
    /// </summary>
    public class NewUser
    {
        public string Name { get; set; }
        public Age Age { get; set; } // Теперь это объект с бизнес-логикой

        public NewUser(string name, Age age)
        {
            Name = name;
            Age = age;
        }
    }

    /// <summary>
    /// Класс, заменяющий простое поле Age
    /// </summary>
    public class Age
    {
        private readonly int _value;

        public int Value => _value;

        public Age(int value)
        {
            if (value < 0 || value > 150)
                throw new ArgumentException("Возраст должен быть между 0 и 150 годами");

            _value = value;
        }

        // Перегрузка операторов для удобства работы
        public static bool operator >(Age a, Age b) => a._value > b._value;
        public static bool operator <(Age a, Age b) => a._value < b._value;
        public static bool operator ==(Age a, Age b) => a?._value == b?._value;
        public static bool operator !=(Age a, Age b) => !(a == b);

        // Методы с бизнес-логикой
        public bool CanVote() => _value >= 18;
        public bool CanWork() => _value >= 16 && _value <= 65;
        public int ToDays() => _value * 365; // приблизительно

        public Age IncreaseYear()
        {
            return new Age(_value + 1);
        }

        public override string ToString() => $"{_value} лет";

        public override bool Equals(object obj)
        {
            if (obj is Age other)
                return _value == other._value;
            return false;
        }

        public override int GetHashCode() => _value.GetHashCode();
    }
}

