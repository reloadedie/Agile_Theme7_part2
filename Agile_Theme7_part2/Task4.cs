
namespace CsTasks
{
    /// <summary>
    /// 4. Самоинкапсуляция поля
    /// </summary>
    public class Task4
    {
        public static void StartTask()
        {

        }


    }

    /// <summary>
    /// 
    /// </summary>
    public class Human
    {
        public string Name;
        public int Age;

        public Human(string _name)
        {
            Name = _name;
        }

        public void Birthday()
        {
            Age++;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class NewHuman
    {
        public string Name { get; set; } = string.Empty; // автосвойство
        public int Age 
        { 
            get; // кто угодно может прочитать
            set; // кто угодно можно записать
        }

        #region
        // С помощью "get" и "set" мы можем управлять доступом к тому
        // что может происходить с полем
        // get --> чтение значения?
        // set --> запись значения? (перезапись)
        // private get --> снаружи не прочитать
        // private set --> снаружи не установить
        #endregion

        #region Какие могут быть случаи

        public string? Email 
        { 
            get; // могут прочитать
                // никто не может обновить данные
        }

        public string? Number
        {
            set
            {
                Name = value; // кто угодно может записать
            } // но прочитать не может
        }

        public string? Message
        {
            private get; // снаружи не прочитать (только внутри класса в методах)
            set; // установить откуда угодно
        }

        // часто бывает другая ситуация - private set
        public string? PassportSeries
        {
            get; // получить везде
            private set; // задать новое никогда
        }

        // поля могут регулироваться тоже
        private string? PassportCode
        {
            get; // получить никак - модификатор дает общий уровень
        }

        public string? Password
        {
            get;
            private set; // Общий уровень = private (потому что set строже)
        }

        public string? Username
        {
            get;
            internal set; // Общий уровень = internal (доступ внутри сборки)
        }

        public string Debug { get; } = "value"; // Константа по сути

        #endregion
    }
}
