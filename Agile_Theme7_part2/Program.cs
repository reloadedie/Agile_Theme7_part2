
namespace Agile_Theme7_part2
{

    public class Program
    {
        static double GetMethodNumber()
        {
            Console.WriteLine("\nВведите номер метода");
            double.TryParse(Console.ReadLine(), out double number_method);
            return number_method;
        }

        static void Main(string[] args)
        {
            Console_WriteMethods();
            double number = GetMethodNumber();
            Console_SetTaskMethod(number);
        }

        static void Console_SetTaskMethod(double number_method)
        {
            var tasks = new Dictionary<double, Action>
            {
                {1, Task1.StartTask},
                {2, Task2.StartTask},
                {3, Task3.StartTask},
                {4, Task4.StartTask},
                {5, Task5.StartTask},
                {6, Task6.StartTask}
            };

            Task.Run(() => tasks[number_method]()).Wait();
        }

        static void Console_WriteMethods()
        {
            Console_MessageWithColor("\nРешение задач для доклада к заданию7 (часть 2) по предмету " +
                "'Управление на основе Agile' (тема 5)", color:ConsoleColor.Yellow);

            Console_MessageWithColor("\tОрганизация данных: замена значение ссылкой, замена ссылки значением, дублирование видимых данных, " +
                "\n\tсамоинкапсуляция поля, замена поля-массива объектом, замена простого поля объектом");
            
            ConsoleColor blue = ConsoleColor.Blue;
            Console_MessageWithColor("\n1. Замена значения ссылкой", blue);
            Console_MessageWithColor("2. Замена ссылки значением", blue);
            Console_MessageWithColor("3. Дублирование видимых данных", blue);
            Console_MessageWithColor("4. Самоинкапсуляция поля", blue);
            Console_MessageWithColor("5. Замена поля-массива объектом", blue);
            Console_MessageWithColor("6. Замена простого поля объектом", blue);
        }

        static void Console_MessageWithColor(string? text, ConsoleColor? color = null)
        {
            Console.ForegroundColor = color ?? ConsoleColor.White;
           // Console.ForegroundColor = color.GetValueOrDefault(ConsoleColor.White);
            Console.WriteLine($"{text}");
            Console.ResetColor();
        }
    }
}             