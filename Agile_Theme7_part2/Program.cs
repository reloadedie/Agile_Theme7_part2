
using System.Diagnostics;
using System.Reflection;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

            Console.ReadKey();
        }

        static void Console_SetTaskMethod(double number_method)
        {
            var tasks = new Dictionary<double, Action>
            {
                {1, Task1.StartTask},
                {2, Task2.StartTask},
                {3, RunTask3Go},
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

        public static void RunTask3Go()
        {
            try
            {
                Console.WriteLine("3. Дублирование видимых данных (язык Go)");

                Console.OutputEncoding = Encoding.UTF8;
               // Console.InputEncoding = Encoding.UTF8;

                string _go_exe_path = Path.GetFullPath(
                    Path.Combine(
                            AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\GoTasks\Task3.exe"));

                if(!File.Exists(_go_exe_path))
                {
                    Console.WriteLine("Файл с Go не найден");
                    return;
                }

                var goProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = _go_exe_path,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        StandardOutputEncoding = Encoding.UTF8,
                    }
                };

                StringBuilder _output = new ();
                StringBuilder _errors = new ();

                goProcess.OutputDataReceived += (sender, e) =>
                {
                    if(e.Data != null)
                    {
                        _output.Append(e.Data);
                        Console.WriteLine($"[Go] {e.Data}");
                    }
                };
                goProcess.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        _errors.Append(e.Data);
                        Console.WriteLine($"[Go (error)]{e.Data}");
                    }
                };

                goProcess.Start();

                // асинхронное чтение (важно, иначе дедлок)
                goProcess.BeginOutputReadLine();
                goProcess.BeginErrorReadLine();

                bool exited = goProcess.WaitForExit(30000); // 30 секунд максимум

                if (exited)
                {
                    Console_MessageWithColor($"Go программа завершена (код: {goProcess.ExitCode})", ConsoleColor.Green);
                }
                else
                {
                    Console_MessageWithColor("Go программа не завершилась вовремя, принудительно закрываем", ConsoleColor.Red);
                    goProcess.Kill();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
            }
        }
    }
}             