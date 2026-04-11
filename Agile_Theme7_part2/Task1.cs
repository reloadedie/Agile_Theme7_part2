using System.Collections.Concurrent;

namespace Agile_Theme7_part2
{
    #region Как было до этого

    /// <summary>
    /// заказ
    /// </summary>
    public class Order
    {
        public Guid Guid { get; }

        /// <summary>
        /// Валюта <CurrencyRate>
        /// </summary>
        public CurrencyRate Currency { get; set; }

        /// <summary>
        /// количество <decimal>
        /// </summary>
        public decimal Amount { get; set; }

        public decimal GetTotalInRubles() => Amount * Currency.Rate;

        public decimal GetCurrencyRateInOrder()
        {
            Console.WriteLine($"В заказе номер {Guid} для валюты {Currency.Code} " +
                $"количество составляет {Amount}, курс этой валюты: {Currency.Rate}");
            return Currency.Rate;
        }
    }

    /// <summary>
    /// Курс валют (старый вариант)
    /// </summary>
    public class CurrencyRate
    {
        public string Code { get; set; }
        public decimal Rate { get; set; }

        /// <summary>
        /// Действия при создании обьекта
        /// Конструктор, в котором можно обновлять данные этого экземпляра при создании
        /// </summary>
        /// <param name="code"></param>
        /// <param name="rate"></param>
        public CurrencyRate(string code, decimal rate)
        {
            Code = code; // Но код валюты должен быть уникален
            Rate = rate; // Курс валюты - это общие данные. все должны видеть одно значение
            _allInstances.Add(this);
        }

        #region Попытка автоматизировать процесс замены курса у всех

        // создание небезопасной коллекции - List
        private static readonly List<CurrencyRate> _allInstances = new(); // сокращенная форма создания объекта без = new List<CurrencyRate>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code">код валюты</param>
        public static void GetCountByCode(string code)
        {
            Console.WriteLine($"Существуют {_allInstances.Where(i => i.Code == code).Count()} объекта с кодом {code}");
            for (int i = 0; i < _allInstances.Count; i++)
            {
                var currency = _allInstances[i];
                Console.WriteLine($"Обьект с кодом {currency.Code} и курсом {currency.Rate}");
            }
        }

        /// <summary>
        /// массовое обновление курса (у всех инстансов заменой)
        /// </summary>
        /// <param name="code">код валюты</param>
        /// <param name="newRate">новый курс</param>
        public static void UpdateRateForAll(string code, decimal newRate)
        {
            foreach (var instance in _allInstances.Where(i => i.Code == code)) // поиск по всем кодам
            {
                Console.WriteLine($" Обновлен курс валюты у {instance.Code}, новое значение: {newRate}, " +
                    $"старое было {instance.Rate}, дата изменения: {DateTime.Now}");
                instance.Rate = newRate;
            }
        }
        #endregion

    }

    /// <summary>
    /// задача - замена значения ссылкой
    /// </summary>
    public class Task1
    {
        public static void StartTask()
        {
            var usdMorning = new CurrencyRate("USD", 50.00m);
            var morningOrder = new Order { Currency = usdMorning, Amount = 100 };
            // предположим, что курс вырос вечером на 50 рублей. нужно сделать новый заказ

            #region попытка обновить курс валюты (уже существующей)

            // нельзя создавать новую сущность с таким же кодом - противоречие
            var usdEvening = new CurrencyRate("USD", 100.00m);

            // если посмотреть заказ
            morningOrder.GetCurrencyRateInOrder(); // вернет 50.00

            // попытка указать уже существующему объекту
            usdMorning.Rate = 100.00m;
            morningOrder.GetCurrencyRateInOrder(); // вернет 100.00
            // и для каждого курса придется вставлять значения вручную?

            // попытка автоматизировать исправление курса: написание UpdateRateForAll
            CurrencyRate.GetCountByCode("USD");
            CurrencyRate.UpdateRateForAll("USD", 150.00m);

            // а что, если на самом деле объектов несколько? предположим, что закралась ошибка
            Console.WriteLine("Добавили еще заказов");
            var _11AM_Order = new Order { Currency = new CurrencyRate("USD", 100.00m), Amount = 50 };
            var _12AM_Order = new Order { Currency = new CurrencyRate("USD", 120.00m), Amount = 50 };

            CurrencyRate.GetCountByCode("USD");
            CurrencyRate.UpdateRateForAll("USD", 160.00m);

            // окей, к каким проблемам это может привести?
            // 1. Много экземпляров класса CurrencyRate, при больших данных нагрузка памяти (медленная работа)
            // 2. Рассогласование данных (отчеты о сделках могут быть неправильными в моменте до того, как обновили курсы)
            // 3. Сложность обновления данных по курсам составляет O(n), что во времени
            var orders = new List<Order>();

            for (int i = 0; i < 1000; i++)
            {
                orders.Add(new Order { Currency = new CurrencyRate("RUB", 100), Amount = 50 });
            }

            // устанавливаем значение курса "RUB" = 155 в списке "orders"
            foreach (var order in orders)
            {
                order.Currency.Rate = 155.00m; // Сложность O(n) --> 1 единица времени на 1 элемент
            }

            // вот теперь подходим к решению этой проблемы - "замена значения ссылкой"

            Task1Solution.StartTask1_Solution();
            #endregion

        }

    }

    #endregion

    #region Решение: замена значения ссылкой

    public class Task1Solution
    {
        public static void StartTask1_Solution()
        {
            Console.Write("\n Новое решение");

            var usd = NewCurrencyRate.Get("USD");
            usd.Rate = 100.00m;

            var rub = NewCurrencyRate.Get("RUB");
            var rub1 = NewCurrencyRate.Get("RUB", 100.00m); // обновится (поступило значение)
            var rub2 = NewCurrencyRate.Get("RUB", 120.00m); // не обновится
            NewCurrencyRate.GetCountByCode("RUB");

            var _10PM_order = new NewOrder { Currency = usd, Amount = 150.00m };
            var _11PM_order = new NewOrder { Currency = usd, Amount = 130.02m };

            usd.Rate = 200;
            _11PM_order.GetCurrencyRateInOrder();

            decimal cource = 100.00m;
            var yen = NewCurrencyRate.Get("YEN", cource);
            var von = NewCurrencyRate.Get("VON", cource);

            var count = NewCurrencyRate.GetCountToRateByCode(cource);
            Console.WriteLine($"\nДля курса {cource} количество валют составило {count}");

            NewCurrencyRate.GetAllList();
        }
    }

    // Обеспечить, чтобы изменения бизнес-данных (курс валюты, ставка НДС,
    // список статусов) были видны во всех местах использования без ручного
    // обновления каждой копии

    // В разных объектах хранятся одинаковые данные, созданные через new
    // При изменении данных в одном месте другие объекты не видят изменений
    // Нужен единый источник правды

    // курс валют обновляется раз в час. Все заказы должны видеть новый курс

    /// <summary>
    /// новый класс заказа
    /// </summary>
    public class NewOrder()
    {
        public Guid Guid { get; } = Guid.NewGuid();

        /// <summary>
        /// НОВЫЙ обьект курса (курс<NewCurrencyRate>)
        /// </summary>
        public NewCurrencyRate Currency { get; set; }

        /// <summary>
        /// Количество валюты в заказе 
        /// </summary>
        public decimal Amount { get; set; }

        public decimal GetTotalInRubles() => Amount * Currency.Rate;

        public decimal GetCurrencyRateInOrder()
        {
            Console.WriteLine($"В заказе номер {Guid} для валюты {Currency.Code} " +
                $"количество составляет {Amount}, курс этой валюты: {Currency.Rate}");
            return Currency.Rate;
        }
    }

    /// <summary>
    /// Курс валют (новый) по валютной паре (код валюты - курс валюты)
    /// </summary>
    public class NewCurrencyRate
    {
        public string Code { get; } = string.Empty;
        public decimal Rate { get; set; }

        /// <summary>
        /// Потокобезопасный словарь (уникальные ключи) (работа по сегментам в случае исключения).
        /// string уникален, NewCurrencyRate может быть любым
        /// </summary>
        private static readonly ConcurrentDictionary<string, NewCurrencyRate> _cache = new();

        /// <summary>
        /// Метод с паттерном "Фабрика" - всегда возвращает 
        /// один и тот же объект для одного кода валюты
        /// возвращает объект, если нашел по коду, или создает новый 
        /// через приватный конструктор (начальное значение 0)
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static NewCurrencyRate Get(string code, decimal? rate = null)
        {
            var currency = _cache.GetOrAdd(code, instance =>
                    new NewCurrencyRate(instance));

            if (rate.HasValue && currency.Rate == 0)
            {
                currency.Rate = rate.Value;
            }

            return currency;
        }

        // обновили курс в одном месте — все заказы его видят

        // нужно гарантировать, что для одного кода валюты (например, "USD")
        // во всей программе существует только один объект CurrencyRate

        /// <summary>
        /// Приватный конструктор (никто не может создать объект напрямую)
        /// </summary>
        /// <param name="code"></param>
        private NewCurrencyRate(string code)
        {
            Code = code;
            Rate = 0; // Начальный курс, позже будет установлен
        }

        #region Дополнения

        // В новом решении захочется сделать метод Create, лучше этого не делать (чтобы не запутаться)

        /// <summary>
        /// ОДИН метод (вместо двух) - если кода нет, возврат всех объектов
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static int GetCountByCode(string? code)
        {
            if (string.IsNullOrEmpty(code))
            {
                Console.WriteLine($"\n Всего элементов в списке новых валют: {(_cache.Count)}");
                return _cache.Count;
            }

            Console.WriteLine($"\n Для курса с кодом {code} есть {(_cache.ContainsKey(code) ? 1 : 0)} совпадений");
            return _cache.ContainsKey(code) ? 1 : 0; // тернарный оператор, если true --> 1, если false --> 0
        }

        /// <summary>
        /// Вывести количество валют по заданному курсу
        /// </summary>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static int GetCountToRateByCode(decimal? rate)
        {
            return _cache.Values.Count(c => c.Rate == rate);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void GetAllList()
        {
            Console.WriteLine("\nВ новом списке валют:");
            foreach (var item in _cache.Values)
            {
                Console.WriteLine($"\n\t{item.Code} имеет курс {item.Rate}");
            }
        }
        #endregion

    }
    #endregion
}

