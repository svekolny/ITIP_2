string? fileName = Manager.GetFileName();
LinkedList<Sportsman> sportsmen = Manager.LoadSportsmen(fileName);

uint choice;
do
{
    Manager.DisplayMenu();
    choice = Manager.GetChoice();
    Manager.DoAction( ref sportsmen, choice);
} while (choice is not 0);

Manager.SaveSportsmen(fileName, sportsmen);

Console.ReadLine();

struct Sportsman
{
    public string LastName;
    public string Gender;
    public string SportType;
    public uint BirthYear;
    public uint Height;
}

class Manager
{
    static public string? GetFileName()
    {
        Console.WriteLine("Введите имя файла:\n");
        string? result = Console.ReadLine();
        if (result != null) result += ".txt";
        return result;
    }

    static public LinkedList<Sportsman> LoadSportsmen(string? fileName)
    {
        LinkedList<Sportsman> sportsmen;

        if (fileName == null)
        {
            Console.WriteLine("Введённая строка пуста. Будет открыт файл с названием по умолчанию");
            fileName = AppDomain.CurrentDomain.BaseDirectory + @"default.txt";
            if (File.Exists(fileName)) { sportsmen = ReadLines(fileName); return sportsmen; }
            return sportsmen = new LinkedList<Sportsman>();
        }

        //Если файл с указанным именем не существует в папке
        string[] file = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, fileName);
        if (file.Length == 0)
        {
            Console.WriteLine("Введённая строка неверна. В папке проекта будет создан файл с указанным ранее названием.");
            return sportsmen = new LinkedList<Sportsman>();
        }

        sportsmen = ReadLines(fileName);
        return sportsmen;
    }

    static private LinkedList<Sportsman> ReadLines(string fileName)
    {
        StreamReader reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + fileName);
        LinkedList<Sportsman> result = new LinkedList<Sportsman>();

        string? line;
        Sportsman sportsman = new Sportsman();
        while ((line = reader.ReadLine()) != null)
        {
            string[] parts = line.Split(' ');
            if (parts.Length == 5)
            {
                sportsman.LastName = parts[0];
                sportsman.Gender = parts[1];
                sportsman.SportType = parts[2];
                sportsman.BirthYear = uint.Parse(parts[3]);
                sportsman.Height = uint.Parse(parts[4]);

                result.AddLast(sportsman);
            }
        }
        reader.Close();
        return result;
    }

    static public void DisplayMenu()
    {
        Console.WriteLine("Выберите пункт меню:\n");
        string menu = "1. Просмотреть список спортсменов\n" +
            "2. Добавить нового спортсмена по индексу\n" +
            "3. Добавить спортсмена в начало списка\n" +
            "4. Добавить спортсмена в конец списка\n" +
            "5. Удалить спортсмена по индексу\n" +
            "6. Изменить значения элемента по индексу\n" +
            "7. Найти самого высокого спортсмена, занимающегося плаванием, среди мужчин\n" +
            "8. Вывести сведения о спортсменках, выступающих в юниорском разряде (14-17лет)\n" +
            "9. Отсортировать спортсменов по фамилии\n" +
            "0. Выход из программы\n";
        Console.WriteLine(menu);
    }

    static public uint GetChoice()
    {
        string? input;
        uint result;

        Console.WriteLine("Ваш выбор:");
        do { input = Console.ReadLine(); }
        while (uint.TryParse(input, out result) && result > 9);

        return result;
    }

    static public void DoAction(ref LinkedList<Sportsman> sportsmen, uint choice)
    {
        Sportsman sportsman;
        List<Sportsman>? list;
        int index;
        switch (choice)
        {
            case 1:
                PrintTable(sportsmen);
                break;
            case 2:
                sportsman = GetSportsman();
                list = sportsmen.ToList();
                if (list.Count != 0)
                {
                    index = GetIndex(list.Count());
                    list.Insert(index, sportsman);
                }
                else list.Insert(0, sportsman);
                sportsmen = new LinkedList<Sportsman>(list);
                list = null;
                break;
            case 3:
                sportsman = GetSportsman();
                sportsmen.AddFirst(sportsman);
                break;
            case 4:
                sportsman = GetSportsman();
                sportsmen.AddLast(sportsman);
                break;
            case 5:
                if (sportsmen.Count == 0) {Console.WriteLine("Список пуст."); break;}
                list = sportsmen.ToList();
                index = GetIndex(list.Count());
                list.RemoveAt(index);
                sportsmen = new LinkedList<Sportsman>(list);
                list = null;
                break;
            case 6:
                if (sportsmen.Count == 0) {Console.WriteLine("Список пуст."); break;}
                list = sportsmen.ToList();
                index = GetIndex(list.Count());
                ChangeSportsman(index, ref list);
                sportsmen = new LinkedList<Sportsman>(list);
                list = null;
                break;
            case 7:
                Sportsman? result = FindTallestSwimMale(sportsmen);
                if (result == null) Console.WriteLine("Спортсмен не найден");
                else { Console.WriteLine($"Спортсмен найден: {result.Value.LastName}, {result.Value.BirthYear} г.р."); }
                break;
            case 8:
                LinkedList<Sportsman>? results = FindAllJuniorFemale(sportsmen);
                if (results == null) Console.WriteLine("Спортсменки не найдены");
                else { Console.WriteLine("Список найденных спортсменок:"); PrintTable(results); }
                break;
            case 9:
                if (sportsmen.Count == 0) {Console.WriteLine("Список пуст."); break;}
                sportsmen = SortSportsmenByLastName(sportsmen);
                break;
            case 0:
                Console.WriteLine("\nВыход из программы...\n");
                break;
            default:
                Console.WriteLine("Ошибка: Неизвестный пункт меню");
                break;
        }
    }

    private static void PrintTable(LinkedList<Sportsman> sportsmen)
    {
        string[] headers = { "Фамилия", "Пол", "Вид спорта", "Год рождения", "Рост" };

        int[] columnWidths = new int[headers.Length];
        for (int i = 0; i < headers.Length; i++)
        {
            columnWidths[i] = headers[i].Length;
        }

        foreach (var item in sportsmen)
        {
            columnWidths[0] = Math.Max(columnWidths[0], item.LastName.Length);
            columnWidths[1] = Math.Max(columnWidths[1], item.Gender.Length);
            columnWidths[2] = Math.Max(columnWidths[2], item.SportType.Length);
            columnWidths[3] = Math.Max(columnWidths[3], item.BirthYear.ToString().Length);
            columnWidths[4] = Math.Max(columnWidths[4], item.Height.ToString().Length);
        }

        // Вывод заголовков
        PrintRow(headers, columnWidths);
        PrintSeparator(columnWidths);

        // Вывод данных
        foreach (var item in sportsmen)
        {
            PrintRow(new string[] { item.LastName, item.Gender, item.SportType, item.BirthYear.ToString(), item.Height.ToString() }, columnWidths);
        }
    }

    private static void PrintRow(string[] columns, int[] columnWidths)
    {
        for (int i = 0; i < columns.Length; i++)
        {
            Console.Write(columns[i].PadRight(columnWidths[i]) + " | ");
        }
        Console.WriteLine();
    }

    private static void PrintSeparator(int[] columnWidths)
    {
        for (int i = 0; i < columnWidths.Length; i++)
        {
            Console.Write(new string('-', columnWidths[i]) + "-+-");
        }
        Console.WriteLine();
    }

    public static Sportsman GetSportsman()
    {
        string? input;
        Sportsman sportsman = new();
        Console.WriteLine("Введите данные нового спортсмена:");
        Console.Write("Фамилия: ");
        do
        {
            input = Console.ReadLine();
            Console.WriteLine();
        } while (input.Length < 2 || input.Length > 20 || input.Contains(' ') || input.Contains('\n'));
        sportsman.LastName = input;

        char[] letters = {'М','Ж','M','F'};
        Console.Write("Пол (М/Ж, M/F):");
        do
        {
            input = Console.ReadLine();
            Console.WriteLine();
        } while (input.Length != 1 || !letters.Any(letter => input.ToUpper().Contains(letter)));
        if (input == "F" || input == "Ж") sportsman.Gender = "Женский";
        else sportsman.Gender = "Мужской";

        Console.Write("Вид спорта (при необходимости запись через _): ");
        do
        {
            input = Console.ReadLine();
            Console.WriteLine();
        } while (input.Length < 2 || input.Length > 30 || input.Contains(' ') || input.Contains('\n'));
        sportsman.SportType = input;

        Console.Write("Год рождения: ");
        do
        {
            input = Console.ReadLine();
            Console.WriteLine();
        } while (!uint.TryParse(input, out sportsman.BirthYear) || sportsman.BirthYear < 1900 || sportsman.BirthYear > 2024);

        Console.Write("Рост: ");
        do
        {
            input = Console.ReadLine();
            Console.WriteLine();
        } while (!uint.TryParse(input, out sportsman.Height) || sportsman.Height < 40 || sportsman.Height > 250);

        return sportsman;
    }

    private static int GetIndex(int total)
    {
        string? input;
        int result;
        Console.Write("Выберите индекс: ");
        do
        {
            input = Console.ReadLine();
            Console.WriteLine();
        } while (!int.TryParse(input, out result) || result < 0 || result > total - 1);
        return result;
    }

    private static void ChangeSportsman(int index, ref List<Sportsman> sportsmen)
    {
        Console.WriteLine("Выберите параметр, который хотите изменить:");
        string str = "1. Фамилия\n" +
            "2. Пол\n" +
            "3. Вид спорта\n" +
            "4. Год рождения\n" +
            "5. Рост\n";
        Console.WriteLine(str);
        Console.WriteLine("Ваш выбор: ");
        string? input;
        int choice;
        Sportsman temp = sportsmen[index]; // Для изменения значения структуры в коллекции все действия сначала нужно проделать на копии
        do
        {
            input = Console.ReadLine();
            Console.WriteLine();
        } while (!int.TryParse(input, out choice) || choice < 1 || choice > 5);
        switch (choice)
        {
            case 1:
                Console.Write("Введите новую фамилию: ");
                do
                {
                    input = Console.ReadLine();
                    Console.WriteLine();
                } while (input.Length < 2 || input.Length > 20 || input.Contains(' ') || input.Contains('\n'));

                temp.LastName = input;
                sportsmen[index] = temp;

                break;
            case 2:
                char[] letters = {'М','Ж','M','F'};
                Console.Write("Введите новый пол (М/Ж, M/F):");
                do
                {
                    input = Console.ReadLine();
                    Console.WriteLine();
                } while (input.Length != 1 || !letters.Any(letter => input.ToUpper().Contains(letter)));
                if (input == "F" || input == "Ж") temp.Gender = "Женский";
                else temp.Gender = "Мужской";

                sportsmen[index] = temp;

                break;
            case 3:
                Console.Write("Введите новый вид спорта: ");
                do
                {
                    input = Console.ReadLine();
                    Console.WriteLine();
                } while (input.Length < 2 || input.Length > 30 || input.Contains(' ') || input.Contains('\n'));

                temp.SportType = input;
                sportsmen[index] = temp;

                break;
            case 4:
                Console.Write("Введите новый год рождения: ");
                do
                {
                    input = Console.ReadLine();
                    Console.WriteLine();
                } while (!uint.TryParse(input, out temp.BirthYear) || temp.BirthYear < 1900 || temp.BirthYear > 2024);

                sportsmen[index] = temp;

                break;
            case 5:
                Console.Write("Введите новый рост: ");
                do
                {
                    input = Console.ReadLine();
                    Console.WriteLine();
                } while (!uint.TryParse(input, out temp.Height) || temp.Height < 40 || temp.Height > 250);
                sportsmen[index] = temp;

                break;
            default: Console.WriteLine("Ошибка меню выбора изменения параметра: неизвестный пункт меню"); break;
        }
    }
    private static Sportsman? FindTallestSwimMale(LinkedList<Sportsman> sportsmen)
    {
        Sportsman result = new();
        bool isFound = false;
        foreach (var item in sportsmen)
        {
            if (item.Gender.ToLower() == "мужской" && item.SportType.ToLower() == "плавание" && item.Height > result.Height) { result = item; isFound = true; }
        }
        if (!isFound) return null;
        return result;
    }
    private static LinkedList<Sportsman>? FindAllJuniorFemale(LinkedList<Sportsman> sportsmen)
    {
        LinkedList<Sportsman> result = new();
        bool isFound = false;

        foreach (var item in sportsmen)
        {
            if (item.Gender.ToLower() == "женский" && item.BirthYear <= 2010 && item.BirthYear >= 2007) { result.AddLast(item); isFound = true; }
        }

        if (!isFound) return null;
        return result;
    }

    public static void SaveSportsmen(string fileName, LinkedList<Sportsman> sportsmen)
    {
        StreamWriter writer = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + fileName);
        foreach (var sportsman in sportsmen)
        {
            string line = $"{sportsman.LastName} {sportsman.Gender} {sportsman.SportType} {sportsman.BirthYear} {sportsman.Height}";
            writer.WriteLine(line);
        }
        writer.Close();
    }
    private static LinkedList<Sportsman> SortSportsmenByLastName(LinkedList<Sportsman> sportsmen)
    {
        return new LinkedList<Sportsman>(sportsmen.OrderBy(s => s.LastName));
    }
}