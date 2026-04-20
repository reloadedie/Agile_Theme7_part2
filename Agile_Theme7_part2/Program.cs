using CsTasks;
using System.Diagnostics;
using System.Text;

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
                {5, RunTask5Python},
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

        /// <summary>
        /// Запуск 3 задачи на языке Go
        /// </summary>
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

        /// <summary>
        /// Запуск 5 задачи на языке Python
        /// </summary>
        public static void RunTask5Python()
        {
            try
            {
                Console.WriteLine("5. Замена поля-массива объектом (язык Python)");

                Console.OutputEncoding = Encoding.UTF8;

                // Путь к Python скрипту
                string _python_script_path = Path.GetFullPath(
                    Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Python Tasks\Task5.py"));

                if (!File.Exists(_python_script_path))
                {
                    Console_MessageWithColor($"Python скрипт не найден: {_python_script_path}", ConsoleColor.Red);
                    return;
                }

                // Поиск Python interpreter
                string pythonPath = FindPythonInterpreter();

                if (string.IsNullOrEmpty(pythonPath))
                {
                    Console_MessageWithColor("Python интерпретатор не найден. Установите Python или добавьте в PATH", ConsoleColor.Red);
                    return;
                }

                Console_MessageWithColor($"Используется Python: {pythonPath}", ConsoleColor.Cyan);
                Console_MessageWithColor($"Запуск скрипта: {_python_script_path}", ConsoleColor.Cyan);

                var pythonProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = pythonPath,
                        Arguments = $"\"{_python_script_path}\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        StandardOutputEncoding = Encoding.UTF8,
                        StandardErrorEncoding = Encoding.UTF8
                    }
                };

                StringBuilder _output = new StringBuilder();
                StringBuilder _errors = new StringBuilder();

                pythonProcess.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        _output.AppendLine(e.Data);
                        Console.WriteLine($"[Python] {e.Data}");
                    }
                };

                pythonProcess.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        _errors.AppendLine(e.Data);
                        Console_MessageWithColor($"[Python Error] {e.Data}", ConsoleColor.Red);
                    }
                };

                pythonProcess.Start();

                // Асинхронное чтение (важно, иначе дедлок)
                pythonProcess.BeginOutputReadLine();
                pythonProcess.BeginErrorReadLine();

                bool exited = pythonProcess.WaitForExit(30000); 

                if (exited)
                {
                    if (pythonProcess.ExitCode == 0)
                    {
                        Console_MessageWithColor($"Python программа успешно завершена (код: {pythonProcess.ExitCode})", ConsoleColor.Green);
                    }
                    else
                    {
                        Console_MessageWithColor($"Python программа завершена с ошибкой (код: {pythonProcess.ExitCode})", ConsoleColor.Yellow);
                        if (_errors.Length > 0)
                        {
                            Console_MessageWithColor($"Ошибки:\n{_errors.ToString()}", ConsoleColor.Red);
                        }
                    }
                }
                else
                {
                    Console_MessageWithColor("Python программа не завершилась вовремя, принудительно закрываем", ConsoleColor.Red);
                    pythonProcess.Kill();
                }
            }
            catch (Exception ex)
            {
                Console_MessageWithColor($"Исключение при запуске Python: {ex.Message}", ConsoleColor.Red);
                Console_MessageWithColor($"Stack trace: {ex.StackTrace}", ConsoleColor.Red);
            }
        }

        /// <summary>
        /// Поиск установленного Python интерпретатора
        /// </summary>
        private static string FindPythonInterpreter()
        {
            string[] pythonNames = { "python", "python3", "py" };

            string[] possiblePaths =
            {
                @"python",
                @"python3",
                @"py",
                @"C:\Python39\python.exe",
                @"C:\Python310\python.exe",
                @"C:\Python311\python.exe",
                @"C:\Python312\python.exe",
                @"C:\Users\" + Environment.UserName + @"\AppData\Local\Programs\Python\Python39\python.exe",
                @"C:\Users\" + Environment.UserName + @"\AppData\Local\Programs\Python\Python310\python.exe",
                @"C:\Users\" + Environment.UserName + @"\AppData\Local\Programs\Python\Python311\python.exe",
                @"C:\Users\" + Environment.UserName + @"\AppData\Local\Microsoft\WindowsApps\python.exe"
            };

            foreach (var pythonName in pythonNames)
            {
                try
                {
                    var whichProcess = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "where",
                            Arguments = pythonName,
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true
                        }
                    };

                    whichProcess.Start();
                    string output = whichProcess.StandardOutput.ReadToEnd();
                    whichProcess.WaitForExit();

                    if (!string.IsNullOrEmpty(output))
                    {
                        string firstPath = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)[0];
                        if (File.Exists(firstPath))
                            return firstPath;
                    }
                }
                catch
                {

                }
            }

            foreach (var path in possiblePaths)
            {
                try
                {
                    if (File.Exists(path))
                        return path;
                }
                catch
                {

                }
            }

            string? pythonPath = Environment.GetEnvironmentVariable("PYTHON_HOME");
            if (!string.IsNullOrEmpty(pythonPath))
            {
                string exePath = Path.Combine(pythonPath, "python.exe");
                if (File.Exists(exePath))
                    return exePath;
            }

            return null;
        }
    }
}             