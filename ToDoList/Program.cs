
//Deklaration
using System.Drawing;

const string FILE_NAME = "list.csv";
//List<Task> tasks = new List<Task>();
Task[] tasks = new Task[0];

//Kollar om csv filen finns, om inte skapas en.
if (!File.Exists(FILE_NAME))
{
    File.Create(FILE_NAME);
}

//Laddar in alla redan existerande tasks från csv dokumentet
tasks = loadFileToArray();

//Main loop för programmet
while(true){
    printMenu();

    int choice = int.Parse(Console.ReadLine());

    switch (choice)
    {
        case 1:
            break;
        case 2:
            AddTask(tasks);
            break;
        case 3:
            break;
    }

    printTasks(tasks);
    //skriv ut lista
    break;
}
Console.Read();

static void printMenu()
{
    Console.WriteLine("Meny");
    Console.WriteLine("1. Markera en task som klar");
    Console.WriteLine("2. Lägg till");
    Console.WriteLine("3. Ta bort");



}
static void printTasks(Task[] tasks)
{

    Console.WriteLine($"\n\n{"Task",-55}{"Deadline",-13}{"Time",-7}Status\n");
    foreach  (Task task in tasks)
    {
        /*switch (ColorChecker(task))
        {
            case 0:
                Console.ForegroundColor = ConsoleColor.Gray;
                break;
            case 1:
                Console.ForegroundColor = ConsoleColor.Green;
                break;
            case 2:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case 3:
                Console.ForegroundColor = ConsoleColor.Red;
                break;

        }*/
        Console.ForegroundColor = ColorChecker(task);
        Console.WriteLine($"{task.Description,-55}{task.DeadLine.ToString("yyyy/MM/dd"), -13}{task.EstimatedHours, -7}{task.IsCompleted}");
    }

    Console.ForegroundColor = ConsoleColor.Gray; //resettar till defaultfärgen för att undvika fel

}

static Task[] AddTask(Task[] tasks)
{
    string description;
    string deadLineString;
    DateTime deadLine;
    double estimatedHours;
    bool isCompleted;
    string input;


    Console.WriteLine("Add task:");
    Console.Write("Task description: ");
    description = Console.ReadLine();

    Console.Write("Task deadline(yyyy/mm/dd): ");
    deadLineString = Console.ReadLine(); //sparar i en string för att senare omformatera till datetime

    Console.Write("Estimated time for task (hours): ");
    estimatedHours = Convert.ToDouble(Console.ReadLine());

    Console.Write("Is the task completed? (y/n): ");
    input = Console.ReadLine();
    input = input.ToLower();

    if (input == "y")
    {
        isCompleted = true;
    }
    else
    {
        isCompleted = false;
    }

    //konverterar deadlinestring till rätt format
    string[] split = deadLineString.Split(new char[] { '/' }, 3);
    deadLine = new DateTime(int.Parse(split[2]), int.Parse(split[1]), int.Parse(split[0]));


    tasks = appendCSV(tasks, description, deadLine, estimatedHours, isCompleted);

    return tasks;
}
static Task[] appendCSV(Task[] tasks, string description, DateTime deadLine, double estimatedHours, bool isCompleted)
{
    StreamWriter writer = new StreamWriter(FILE_NAME, true);
    string appendString = description + ";" + deadLine + ";" + estimatedHours + ";" + isCompleted;
    writer.Close();

    tasks = loadFileToArray();
    return tasks;
}


static ConsoleColor ColorChecker(Task task){

    //Status
    //0: default (övriga fall)
    //1: completed
    //2: less than 3 days left
    //3: deadline passed

    ConsoleColor color = ConsoleColor.Gray;
    //DateTime dateTime = new DateTime(2023, 09, 08);
    DateTime dateTime = DateTime.Now;

    double daysUntilDeadline = Convert.ToDouble((dateTime - task.DeadLine).TotalDays);

    //Console.WriteLine(daysUntilDeadline);
    if (task.IsCompleted)
    {
        color = ConsoleColor.Green;
    }
    else if (daysUntilDeadline >= 0) //jämför om deadline är efter datetime
    {
        color = ConsoleColor.Red;    }
    else if (daysUntilDeadline >= -3)
    {
        color = ConsoleColor.Yellow;
    }
    return color;
}

static Task[] loadFileToArray()
{
    //List<Task> tempTasks = new List<Task>();

    StreamReader reader = new StreamReader(FILE_NAME);
    Task[] tempTasks = new Task[File.ReadAllLines(FILE_NAME).Length];

    int counter = 0;
    while (!reader.EndOfStream)
    {
        string Line;
        Line = reader.ReadLine();

        //splittrar csv datan med varje ";"
        string[] data = Line.Split(new char[] { ';' }, 4);

        Task temp = new Task();

        //konerterar den splittrade datan och matar in den i en temp-task
        string Description = data[0];
        DateTime DeadLine = Convert.ToDateTime(data[1]);
        double EstimatedHours = Convert.ToDouble(data[2]);
        bool IsCompleted = Convert.ToBoolean(data[3]);

        temp.Description = Description;
        temp.DeadLine = DeadLine;
        temp.EstimatedHours = EstimatedHours;
        temp.IsCompleted = IsCompleted;

        tempTasks[counter] = temp;
        counter++;
    }
    reader.Close();
    return tempTasks;
}
struct Task
{
    public string Description { get; set; } //ändra namn till "Task"
    public DateTime DeadLine { get; set; }
    public double EstimatedHours { get; set; }
    public bool IsCompleted { get; set; }
}