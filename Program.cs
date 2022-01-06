using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

// Класс для имитации работы с файловой системой.
namespace File_Manager
{
    class Program
    {
        // Словарь кодировок.
        static private Dictionary<string, Encoding> s_encodings = new Dictionary<string, Encoding>();
        // Флаг на перезаписывание файла, если он есть в место, куда его добавляют.
        static private bool s_rewriteExistingFile = false;
        // Разделитель в файловом пути в зависимости от ос определяется в main.
        static private string s_sep = "\\";

        /// <summary>
        /// Метода определения словаря кодировок. И запуска интерфейса.
        /// </summary>

        static void Main(string[] args)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                s_sep = "/";
            }
            Directory.SetCurrentDirectory(Path.GetPathRoot(Directory.GetCurrentDirectory()));
            s_encodings.Add("UTF32", new UTF32Encoding(true, false));
            s_encodings.Add("ASCII", new ASCIIEncoding());
            s_encodings.Add("Unicode", new UnicodeEncoding());
            // Цикл работает пока не введут exit.
            int starts = 0;
            while (starts >= 0)
            {
                switch (starts)
                {
                    case 0:
                        ShowCommands();
                        Console.WriteLine();
                        starts = InputComands();
                        break;
                    case 1:
                        starts = InputComands();
                        break;
                }
            }
            Console.WriteLine("See you.");
            return;
        }

        /// <summary>
        /// Отрисовка команд (help).
        /// </summary>

        private static void ShowCommands()
        {
            Console.WriteLine("Чтобы закончить ввод содержимого в файл при вызове команды create, нажмите стрелку" +
                " вверх\nEcnodingsKeys = [UTF8(default), UTF32, ASCII, Unicode]\nGet all drivers            " +
                "    getdrives\nGo to driver                   gotodrive  driverName\nGo to path                  " +
                "   gotodir  pathToDir[full path or path from current directory]\nShow all files from directory  " +
                "gfd depth[0, 1] mask[«.smth»]\nShow file content              sfc pathToFile[full path or path" +
                " from current directory] *fileEncoding[one of EncodingsKeys] *outputEncoding[one of EncodingsKey" +
                "s]\nCreatNew .txt file             create fileName *fileEncoding[UTF8 (default), UTF32, ASCII, " +
                "Unicode]\nSumFiles                       sumfiles files’Names[ file1 file2 fil3 e.t.c]\nDelete file                    del pathToFile[full path " +
                "or path from current directory]\nCopyFile                       copy pathToFile[full path or pat" +
                "h from current directory] destDirectory *rewritefile[on or off]\nMoveFile                       " +
                "move pathToFile[full path or path from current directory] destDirectory *rewritefile[on or off]\n" +
                "End the programm               exit\nClear console                  clear\nFind the difference" +
                "            diff firstFile[original file] secondFile[second file]\nCopy all files from directory " +
                " copyall sourcedirectory targetdirectory mask *rewritefiles[on or off]");
        }

        /// <summary>
        /// Считывание команды, введенной пользователем.
        /// </summary>

        private static int InputComands()
        {
            string[] command = BeautifulInput();
            return ChooseTheCommand(command);
        }

        /// <summary>
        /// Красивый вывод с показом текущей директории.
        /// </summary>

        private static string[] BeautifulInput()
        {
            Console.Write($"{Directory.GetCurrentDirectory()}~ ");
            return Console.ReadLine().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Клавный контроллер по вызову соответствующих методов по командам. И обработка ошибок
        /// </summary>
        /// <param name="command"></param>

        private static int ChooseTheCommand(string[] command)
        {
            try
            {
                switch (command[0])
                {
                    case "help":
                        return 0;
                    case "clear":
                        Console.Clear();
                        return 1;
                    case "getdrives":
                        ShowListOfDrives();
                        return 1;
                    case "gotodrive":
                        GoToDirectory($"{command[1]}:");
                        return 1;
                    case "gotodir":
                        GoToDirectory(command[1]);
                        return 1;
                    case "gfd":
                        GetFilesFromDirectopry(command[1].Split(" ", 2));
                        return 1;
                    case "sfc":
                        ShowFileContent(command[1].Split(" ", 3));
                        return 1;
                    case "copy":
                        CopyFile(command[1].Split(" ", 3));
                        return 1;
                    case "copyall":
                        CopyeAllFilesFromDirectory(command[1].Split(" ", 4));
                        return 1;
                    case "move":
                        MoveFileTo(command[1].Split(" ", 3));
                        return 1;
                    case "del":
                        DeleteFile(command[1]);
                        return 1;
                    case "create":
                        CreatNewTxtFile(command[1].Split(" ", 3));
                        return 1;
                    case "sumfiles":
                        SumFiles(command[1].Split());
                        return 1;
                    case "diff":
                        Difference(command[1].Split(" ", 2));
                        return 1;
                    case "exit":
                        return -1;
                }
                Console.WriteLine("Wrong command");
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine("Wrong command");
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                Console.WriteLine("Wrong command");
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (DirectoryNotFoundException dirEx)
            {
                Console.WriteLine(dirEx.Message);
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
            return 1;
        }

        /// <summary>
        /// Метода конкатенации файлов.
        /// </summary>
        /// <param name="text">Файлы, которые мы конкатенируем.</param>

        private static void SumFiles(string[] text)
        {
            if (text.Length < 2)
                throw new IndexOutOfRangeException();
            // Вывод на экран.
            string outp = "";
            foreach (var i in text)
            {
                if (i.Split('.')[^1] != "txt")
                {
                    Console.WriteLine("Все файлы должны быть .txt");
                }
                outp += File.ReadAllText(i);
            }
            // Все файлы будут записаны в первый.
            File.Delete(text[0]);
            File.WriteAllText(text[0], outp, encoding: Encoding.UTF8);
            Console.WriteLine($"В файл {text[0]} записан текст:");
            Console.WriteLine(outp);
        }

        /// <summary>
        /// Метод создания нового текстового файла.
        /// </summary>
        /// <param name="text">Нахвание нового файла без .txt.</param>
        /// 
        private static void CreatNewTxtFile(string[] text)
        {
            string fileContent = ""; string list;
            // Ввод содержимого файла до тех пор, пока пользовательне нажмет стреку вверх.
            while (Console.ReadKey(false).Key != ConsoleKey.UpArrow)
            {
                list = Console.ReadLine();
                fileContent += list + "\n";
            }
            if (File.Exists(text[0]))
                File.Delete(text[0]);
            File.WriteAllText(Path.GetFullPath(text[0]) + ".txt", fileContent, text.Length == 2 ?
                s_encodings[text[1]] : new UTF8Encoding(false));
        }

        /// <summary>
        /// Метод для удаления файла.
        /// </summary>
        /// <param name="name">Имя файла.</param>

        private static void DeleteFile(string name)
        {
            //if (IsFileInUse(source))
            //{
            //   Console.WriteLine("File already in use");
            //  return;
            //}
            if (File.Exists(name))
                Console.WriteLine($"Deleted: {Path.GetFullPath(name)}");
            else
                Console.WriteLine($"No such file: {Path.GetFullPath(name)}");
            File.Delete(name);
        }

        /// <summary>
        /// Метод для копирование файла в какую-то директорию.
        /// </summary>
        /// <param name="text">Имя копируемого файла и конечная директория</param>

        private static void CopyFile(string[] text)
        {
            string start = text[0];
            string destination = text[1];
            s_rewriteExistingFile = text.Length == 3 && text[2] == "on";
            File.Copy(start, destination + s_sep + start.Split(s_sep)[^1], s_rewriteExistingFile);
        }

        /// <summary>
        /// Вывод всех файлов текущей директории.
        /// </summary>
        /// <param name="text">С какой глубиной погружения брать файлы и их маска при необходимости.</param>
        /// <param name="curDirectory">Текущая директория.</param>

        private static void GetFilesFromDirectopry(string[] text, string curDirectory = "")
        {
            // Сделано для поиска файлов в поддиректориях, там будет вызван этот же метод с другой текущей директорией.
            if (curDirectory == "")
                curDirectory = Directory.GetCurrentDirectory();
            //Console.WriteLine(string.Join(" ",text) + " "+curDirectory);
            string mask = null; uint depth;
            if (text.Length == 2)
                mask = text[1];
            // Определение на каких уровня нужно искать смотреть файлы.
            if (!uint.TryParse(text[0], out depth) || depth > 1)
                throw new IndexOutOfRangeException();
            //Console.WriteLine(depth);
            foreach (var i in Directory.GetFiles(curDirectory))
            {
                // Проверка по маске. Сплит, чтобы регулярку не ввели.
                if (mask == null || "." + i.Split('.')[^1] == mask)
                {
                    Console.WriteLine(i);
                }
            }
            if (depth == 1)
            {

                foreach (var dir in Directory.GetDirectories(curDirectory))
                {
                    try
                    {
                        // Вызов этого же метода с параметрами ("0 mask", dir).
                        GetFilesFromDirectopry(("0 " + mask).Split(' ', StringSplitOptions.RemoveEmptyEntries), dir);
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

            }
        }

        /// <summary>
        /// Вывод содержимого файла в нужной кодировке.
        /// </summary>
        /// <param name="text">Имя файла, его кодировка, кодировка вывода.</param>

        private static void ShowFileContent(string[] text)
        {
            string name = text[0]; Encoding fileEncoding = Encoding.UTF32, showEncoding = Encoding.UTF8;
            if (text.Length >= 2)
                fileEncoding = s_encodings[text[1]];
            if (text.Length == 3)
                showEncoding = s_encodings[text[1]];
            if (name.Split('.')[^1] != "txt")
            {
                Console.WriteLine("I can read only .txt files.");
                return;
            }
            // Нужно правильно декодировать байты.
            byte[] fileInBytes = fileEncoding.GetBytes(File.ReadAllText(text[0]));
            // Уже полученный байты записать в нужной кодировке и вывести.
            string contentToShow = showEncoding.GetString(fileInBytes).ToString();
            Console.WriteLine(contentToShow);
        }

        /// <summary>
        /// Перейти в другую папку. Его я использую и для выбора диска.
        /// </summary>
        /// <param name="name">Полный путь к папке или путь из текущей.</param>

        private static void GoToDirectory(string name)
        {
            Directory.SetCurrentDirectory(name);
        }

        /// <summary>
        /// Копирование файлов из одной директории в другю.
        /// </summary>
        /// <param name="text">Директория-источник, целевая директория и маска.</param>

        private static void CopyeAllFilesFromDirectory(string[] text)
        {
            string sourceDirectory = text[0], destinationDirectory = text[1], mask = text[2];
            s_rewriteExistingFile = text.Length == 4 ? text[3] == "on" : false;
            if (!Directory.Exists(destinationDirectory))
                Directory.CreateDirectory(destinationDirectory);
            // Перебор файлов всех поддиректорий и текущей директории.
            foreach (var i in Directory.GetFiles(sourceDirectory))
            {
                if (mask == null || "." + i.Split('.')[^1] == mask)
                {
                    File.Copy(i, destinationDirectory + s_sep + i.Split(s_sep)[^1], s_rewriteExistingFile);
                }
            }
            foreach (var i in Directory.GetDirectories(sourceDirectory))
            {
                try
                {
                    foreach (var j in Directory.GetFiles(sourceDirectory))
                    {
                        if (mask == null || "." + j.Split('.')[^1] == mask)
                        {
                            File.Copy(i, destinationDirectory + s_sep + i.Split(s_sep)[^1], s_rewriteExistingFile);
                        }
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (UnauthorizedAccessException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        /// <summary>
        /// Вывод списка дисков компьютера.
        /// </summary>

        private static void ShowListOfDrives()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo d in allDrives)
            {
                Console.WriteLine("Drive {0}", d.Name);
            }
        }

        /// <summary>
        /// Переместить файл в какую-то директорию.
        /// </summary>
        /// <param name="text">Путь до файла, конечная директория, возможность перезаписать его.</param>

        private static void MoveFileTo(string[] text)
        {
            string source = text[0], destination = text[1];
            s_rewriteExistingFile = text.Length == 3 && text[2] == "on";
            File.Move(source, destination + s_sep + source.Split(s_sep)[^1], s_rewriteExistingFile);
        }

        /// <summary>
        /// Метод для вывода разницы двух файлов по шаблону https://ru.wikipedia.org/wiki/Diff практически.
        /// То есть вывод такой: что нужно сделать с original для получения new.
        /// </summary>
        /// <param name="text">Original-файл, new-файл.</param>

        private static void Difference(string[] text)
        {
            string originFile = text[0], newFile = text[1];
            if (File.Exists(originFile) && File.Exists(newFile))
            {

                string[] arr2 = File.ReadAllLines(originFile);
                string[] arr1 = File.ReadAllLines(newFile);
                int[,] lcs = new int[arr1.Length + 1, arr2.Length + 1];
                (int, int)[,] path = new (int, int)[arr1.Length + 1, arr2.Length + 1];
                // Алгоритм на нахождение наибольшей общей подпоследовательности.
                LCS(in arr1, in arr2, ref lcs, ref path);
                string[] commonSub = new string[0];
                // С помощью массива пути восстанавливаем общую подпослдовательность.
                MakeSubsequence(arr1.Length, arr2.Length, ref commonSub, path, arr1);
                // На основе commonSub делается вывод по шаблону, какие строки добавлены,удалины или заменены.
                string toShow = SeparateLine(commonSub, arr2, arr1);
                Console.WriteLine(toShow);
            }
            else
            {
                throw new FileNotFoundException("Wrong file path.");
            }
        }

        /// <summary>
        /// Алгоритм на нахождение наибольшей общей подпоследовательности.
        /// https://neerc.ifmo.ru/wiki/index.php?title=Задача_о_наибольшей_общей_подпоследовательности.
        /// </summary>
        /// <param name="arr1">Первая последовательность.</param>
        /// <param name="arr2">Вторая последовательность.</param>
        /// <param name="matrix">Двумерный массив для динамического алгоритма длин подпоследовательностей.</param>
        /// <param name="path">Массив для хранения пути, создания подпоследовательности.</param>
        /// <returns>Наибольшую общую подпоследовательность.</returns>

        private static void LCS(in string[] arr1, in string[] arr2, ref int[,] matrix, ref (int, int)[,] path)
        {
            int len1 = arr1.Length, len2 = arr2.Length;
            for (int i = 0; i <= len1; i++)
            {
                matrix[i, 0] = 0;
            }
            for (int i = 1; i <= len2; i++)
            {
                matrix[0, i] = 0;
            }
            for (int i = 1; i <= len1; i++)
            {
                for (int j = 1; j <= len2; j++)
                {
                    if (arr1[i - 1] == arr2[j - 1])
                    {
                        matrix[i, j] = matrix[i - 1, j - 1] + 1;
                        path[i, j] = (i - 1, j - 1);
                    }
                    else
                    {
                        if (matrix[i - 1, j] >= matrix[i, j - 1])
                        {
                            matrix[i, j] = matrix[i - 1, j];
                            path[i, j] = (i - 1, j);
                        }
                        else
                        {
                            matrix[i, j] = matrix[i, j - 1];
                            path[i, j] = (i, j - 1);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Метод для восстановления подпоследовательности по сохраненному пути
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="result"></param>
        /// <param name="path"></param>
        /// <param name="arr1"></param>

        private static void MakeSubsequence(int i, int j, ref string[] result, (int, int)[,] path, string[] arr1)
        {
            if (i == 0 || j == 0)
                return;
            if (path[i, j] == (i - 1, j - 1))
            {
                MakeSubsequence(i - 1, j - 1, ref result, path, arr1);
                Array.Resize(ref result, result.Length + 1);
                result[^1] = arr1[i - 1];
            }
            else
            {
                if (path[i, j] == (i - 1, j))
                {
                    MakeSubsequence(i - 1, j, ref result, path, arr1);
                }
                else
                {
                    MakeSubsequence(i, j - 1, ref result, path, arr1);
                }
            }
        }

        /// <summary>
        /// Метод для формирования результата по шаблону https://ru.wikipedia.org/wiki/Diff.
        /// </summary>
        /// <param name="arr1">Спислк строк файла original.</param>
        /// <param name="arr2">Список строк файла new.</param>
        /// <param name="commonLines">Наибольшая подпоследовательность этих файлов.</param>
        /// <returns>Отформатированная строка для функции diff.</returns>

        private static string SeparateLine(string[] commonLines, string[] arr1, string[] arr2)
        {
            //  z - индекс в массиве commonLines
            int ind1 = 0, ind2 = 0, z = 0; string outp = "";
            int len1 = arr1.Length, len2 = arr2.Length, lenCom = commonLines.Length;
            while (z < lenCom)
            {
                //Console.WriteLine(z);
                // Строка есть в обоих файлах.
                if (commonLines[z] == arr1[ind1] && commonLines[z] == arr2[ind2])
                {
                    ind1++;
                    ind2++;
                    z++;
                }
                // Строки нужно добавить.
                else if (commonLines[z] == arr1[ind1])
                {
                    outp += Adding(ref ind1, ref ind2, arr2, commonLines, z);
                }
                // Строки удалить.
                else if (commonLines[z] == arr2[ind2] && arr1.Contains(commonLines[z]))
                {
                    outp += Deleting(ref ind1, ref ind2, arr1, commonLines, z);
                }
                // Сторки нужно заменить.
                else
                {
                    outp += Replacement(ref ind1, ref ind2, arr1, arr2, commonLines, z);
                }
            }
            // Если остались строки, которые нужно добавить/удалить.
            if (ind1 < len1 && ind2 < len2)
                outp += Replacement(ref ind1, ref ind2, arr1, arr2, commonLines);
            else if (ind1 < len1)
                outp += Deleting(ref ind1, ref ind2, arr1, commonLines);
            else if (ind2 < len2)
                outp += Adding(ref ind1, ref ind2, arr2, commonLines);
            return outp;
        }

        /// <summary>
        /// Метод для создания строки, соответсвующей удалению каких-то строк в первом файле.
        /// </summary>
        /// <param name="ind1">Индекс текущего элемента в певром файле.</param>
        /// <param name="ind2">Индекс текущего элемента во втором файле./param>
        /// <param name="arr1">Список строк первого файла.</param>
        /// <param name="common">Список общих строк.</param>
        /// <param name="stop">Признак нахождения совпадения.</param>
        /// <returns>Красивую строку с информацией об удалении.</returns>

        private static string Deleting(ref int ind1, ref int ind2, string[] arr1, string[] common, int stop = -1)
        {
            string deleted = ""; int cnt = 0;
            while (ind1 < arr1.Length && (stop == -1 || arr1[ind1] != common[stop]))
            {
                deleted += $"< {arr1[ind1]}\n"; cnt++; ind1++;
            }
            return $"{ind1 + 1 - cnt},{ind1}d\n" + deleted;
        }

        /// <summary>
        /// Метод для создания строки, соответсвующей замене каких-то строк в первом файле на строки второго.
        /// </summary>
        /// <param name="ind1">Индекс текущего элемента в певром файле.</param>
        /// <param name="ind2">Индекс текущего элемента во втором файле./param>
        /// <param name="arr1">Список строк первого файла.</param>
        /// <param name="arr2">Список строк второго файла.</param>
        /// <param name="common">Список общих строк.</param>
        /// <param name="stop">Признак нахождения совпадения.</param>
        /// <returns>Красивую строку с информацией о замене.</returns>

        private static string Replacement(ref int ind1, ref int ind2, string[] arr1, string[] arr2,
                                                                     string[] common, int stop = -1)
        {
            string replace = "";
            int cntDel = 0, cntAdd = 0;
            while (ind1 < arr1.Length && (stop == -1 || arr1[ind1] != common[stop]))
            {
                replace += $"< {arr1[ind1]}\n"; cntDel++; ind1++;
            }
            replace += "---\n";
            while (ind2 < arr2.Length && (stop == -1 || arr2[ind2] != common[stop]))
            {
                replace += $"> {arr2[ind2]}\n"; cntAdd++; ind2++;
            }
            return $"{ind1 - cntDel + 1},{ind1}c{ind2 - cntAdd + 1},{ind2}\n" + replace;
        }

        /// <summary>
        /// Метод для создания строки, соответсвующей добавлению каких-то строк в первом файле.
        /// </summary>
        /// <param name="ind1">Индекс текущего элемента в певром файле.</param>
        /// <param name="ind2">Индекс текущего элемента во втором файле./param>
        /// <param name="arr2">Список строк второго файла.</param>
        /// <param name="common">Список общих строк.</param>
        /// <param name="stop">Признак нахождения совпадения.</param>
        /// <returns>Красивую строку с информацией о добавлении.</returns>

        private static string Adding(ref int ind1, ref int ind2, string[] arr2, string[] common, int stop = -1)
        {
            string addition = "";
            int cnt = 0;
            while (ind2 < arr2.Length && (stop == -1 || arr2[ind2] != common[stop]))
            {
                addition += $"> {arr2[ind2]}\n"; cnt++; ind2++;
            }
            return $"{ind1}a{ind2 + 1 - cnt},{ind2}\n" + addition;
        }
    }
}