using System.Drawing;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;

//Kollar om csv filen finns, om inte skapas en.
const string FILE_NAME = "list.csv";
if (!File.Exists(FILE_NAME))
{
    StreamWriter writer = new StreamWriter(FILE_NAME);
    writer.Close();
}

//Skapar en array och laddar in alla tasks från csv-filen
Task[] tasks = new Task[0];
tasks = LoadFileToArray();
tasks = SortArrayByDatetime(tasks);

//Huvudloop
while (true)
{
    Console.Clear();
    PrintMenu();

    PrintTasks(tasks);

    Console.Write("\nVälj ett alternativ från menyn: ");
    int choice;

    try
    {
        choice = int.Parse(Console.ReadLine());
    }
    catch
    {
        Console.WriteLine("Välj ett giltigt val");
        Console.ReadKey();
        continue;
    }

    switch (choice)
    {
        case 1:
            PrintTasks(tasks);
            Console.ReadKey();
            break;
        case 2:
            tasks = AddTask(tasks);
            break;
        case 3:
            tasks = MarkTaskCompleted(tasks);
            break;
        case 4:
            Environment.Exit(0);
            break;
    }
}

static void PrintMenu()
{
    Console.WriteLine("Meny");
    Console.WriteLine("1. Visa lista");
    Console.WriteLine("2. Lägg till");
    Console.WriteLine("3. Ändra status");
    Console.WriteLine("4. Avsluta program");
}
static void PrintTasks(Task[] tasks)
{
    int i = 1;
    Console.WriteLine($"\n\n{"Nr.",-4}{"Beskrivning",-55}{"Deadline",-13}{"Time",-7}Status\n");
    //numrerna är för regelbunden spacing mellan utskrifterna.
    foreach (Task task in tasks)
    {
        string status = "Klar";
        if (!task.isCompleted) status = "Inte klar";

        Console.ForegroundColor = ColorChecker(task);
        Console.WriteLine($"{i + ".",-3} {task.description,-55}{task.deadLine.ToString("yyyy/MM/dd"),-13}{task.estimatedHours,-7}{status}");
        i++;
    }
    Console.ForegroundColor = ConsoleColor.Gray; //resettar till defaultfärgen för att undvika fel

}//Funktion som skriver ut alla tasks i listan med hjälp av formatering.

static Task[] SortArrayByDatetime(Task[] tasks)
{
    //Struntar i sortering om listan är tom.
    if (tasks.Length == 0)
    {
        return tasks;
    }

    Array.Sort(tasks, (x, y) => y.deadLine.CompareTo(x.deadLine));
    return tasks;

    
    /*
    bool sorted = false;

    //Kör sorteringsalgoritm tills arrayen är sorterad.
    do
    {
        sorted = true;
        int i = 0;
        foreach (Task task in tasks)
        {
            //Jämför datetime med nästa plats i arrayen och byter platser utefter det
            if ((FindIndex(tasks, task) >= 0) && (FindIndex(tasks, task) < (tasks.Length - 1)))
            {
                if (tasks[i + 1].DeadLine < task.DeadLine)
                {
                    sorted = false;
                    Task tempTask1 = task;
                    Task tempTask2 = tasks[i + 1];

                    tasks[i] = tempTask2;
                    tasks[i + 1] = tempTask1;

                }
                else if (tasks[i+1].DeadLine == task.DeadLine)
                {
                    continue;
                }
            }
            i++;
        }
    } while (!sorted);
    
    static int FindIndex(Task[] tasks, Task task)
    {
        int index = -1;
        for (int i = 0; i < tasks.Length; i++)
        {
            if (tasks[i].Equals(task))
            {
                index = i;
                break;
            }
        }
        return index;
    }
    */
}

static Task[] AddTask(Task[] tasks)
{
    string input = "";
    string description = "";
    double estimatedHours = 0;
    DateTime deadLine = DateTime.Now;
    bool isCompleted = false;

    bool repeat = true;
    do
    {
        Console.Clear();
        repeat = false;
        try
        {

            Console.WriteLine("Add task:");
            Console.Write("Task description: ");
            description = Console.ReadLine();

            Console.Write("Task deadline(yyyy/mm/dd): ");
            deadLine = DateTime.Parse(Console.ReadLine());

            Console.Write("Estimated time for task (hours): ");
            estimatedHours = Convert.ToDouble(Console.ReadLine());

            Console.Write("Is the task completed? (y/n): ");
            input = Console.ReadLine();
            input = input.ToLower();
        }
        catch (Exception)
        {
            Console.WriteLine("Fel format, testa igen");
            Console.ReadKey();
            repeat = true;
        }
    } while (repeat);

    if (input == "y")
    {
        isCompleted = true;
    }
    else
    {
        isCompleted = false;
    }

    tasks = AppendCSV(tasks, description, deadLine, estimatedHours, isCompleted);

    return tasks;

    static Task[] AppendCSV(Task[] tasks, string description, DateTime deadLine, double estimatedHours, bool isCompleted)
    {
        StreamWriter writer = new StreamWriter(FILE_NAME, true);
        string appendString = description + ";" + deadLine + ";" + estimatedHours + ";" + isCompleted;
        writer.WriteLine(appendString);
        writer.Close();

        tasks = LoadFileToArray();
        tasks = SortArrayByDatetime(tasks);
        return tasks;
    }//Lägger till en task i CSV-filen och updaterar sedan arrayen med tasks utefter detta.

}//Funktion för att lägga till en task

static Task[] MarkTaskCompleted(Task[] tasks)
{
    while (true)
    {
        Console.Clear();

        Console.WriteLine("Ändra status\n");
        PrintTasks(tasks);

        Console.Write("Vilken/vilka punkter vill du ändra status på?(3,1,6 om flera): ");
        string input = Console.ReadLine();

        if (input.All(char.IsDigit))
        {
            int index = int.Parse(input);
            index -= 1;
            tasks[index].isCompleted = !tasks[index].isCompleted; //Ändrar "iscompleted" till motsatt 
        }
        else
        {
            int length = 0;
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (Char.IsDigit(c))
                    length++;
            }

            string[] data = input.Split(new char[] { ',' }, length); //separerar inputs baserat på ","

            //Går igenom varje input med samma funktion som ovan
            foreach (string input2 in data)
            {
                int index = int.Parse(input2);
                index -= 1;
                tasks[index].isCompleted = !tasks[index].isCompleted;
            }
        }

        File.Delete(FILE_NAME);

        StreamWriter writer = new StreamWriter(FILE_NAME);

        int counter = 0;
        foreach (Task task in tasks)
        {
            string appendString = task.description + ";" + task.deadLine + ";" + task.estimatedHours + ";" + task.isCompleted;
            if (counter != 0)
            {
                writer.WriteLine(appendString);
            }
            else if (counter == 0)
            {
                writer.WriteLine(appendString);
            }
            counter++;
        }
        writer.Close();
        return tasks;
    }
}

static ConsoleColor ColorChecker(Task task)
{
    ConsoleColor color = ConsoleColor.Gray;
    //DateTime dateTime = new DateTime(2023, 09, 08);
    DateTime dateTime = DateTime.Now;

    double daysUntilDeadline = Convert.ToDouble((dateTime - task.deadLine).TotalDays);

    //Status
    //0: default (övriga fall)
    //1: completed
    //2: less than 3 days left
    //3: deadline passed

    //Console.WriteLine(daysUntilDeadline);
    if (task.isCompleted)
    {
        color = ConsoleColor.Green;
    }
    else if (daysUntilDeadline >= 0) //jämför om deadline är efter nuvarande datetime
    {
        color = ConsoleColor.Red;
    }
    else if (daysUntilDeadline >= -3)
    {
        color = ConsoleColor.Yellow;
    }
    else
    {
        color = ConsoleColor.White;
    }
    return color;
}//Byter färg på text baserat på status av task

static Task[] LoadFileToArray()
{
    //List<Task> tempTasks = new List<Task>();

    Task[] tempTasks = new Task[File.ReadAllLines(FILE_NAME).Length];
    StreamReader reader = new StreamReader(FILE_NAME);

    //While loop som läser in alla tasks från csv filen och sedan sparar dem i en array av structs
    int counter = 0;
    while (!reader.EndOfStream)
    {

        string Line;
        Line = reader.ReadLine();

        //splittrar csv datan med varje ";"
        string[] data = Line.Split(new char[] { ';' }, 4);

        if (data.Length != 4) continue;

        Task temp = new Task();


        //konerterar den splittrade datan och matar in den i en temp-task
        string description = data[0];
        DateTime deadLine = DateTime.Parse(data[1]);
        double estimatedHours = Convert.ToDouble(data[2]);
        bool isCompleted = Convert.ToBoolean(data[3]);

        temp.description = description;
        temp.deadLine = deadLine;
        temp.estimatedHours = estimatedHours;
        temp.isCompleted = isCompleted;

        tempTasks[counter] = temp;
        counter++;
    }

    reader.Close();
    return tempTasks;
}//skapar och fyller en array med tasks från CSV

struct Task
{
    public string description { get; set; }
    public DateTime deadLine { get; set; }
    public double estimatedHours { get; set; }
    public bool isCompleted { get; set; }
}