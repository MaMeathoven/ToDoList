
//Deklaration
const string FILE_NAME = "list.csv";
List<Task> tasks = new List<Task>();

//Kollar om csv filen finns, om inte skapas en.
if (!File.Exists(FILE_NAME))
{
    File.Create(FILE_NAME);
}

//Laddar in alla redan existerande tasks från csv dokumentet
tasks = loadFileToList();

//Main loop för programmet
while(true){
    printMenu();
    printTasks(tasks);
    //skriv ut lista
    break;
}
Console.Read();

static void printMenu()
{
    Console.WriteLine("Meny");
    Console.WriteLine("1. Lägg till");
    Console.WriteLine("2. Ta bort");
}
static void printTasks(List<Task> tasks)
{
    Console.WriteLine($"\n\n{"Task",-55}{"Deadline",-13}{"Time",-7}Status\n");
    foreach  (Task task in tasks)
    {
        Console.WriteLine($"{task.Description,-55}{task.DeadLine.ToString("yyyy/MM/dd"), -13}{task.EstimatedHours, -7}{task.IsCompleted}");
    }
}

static List<Task> loadFileToList()
{
    List<Task> tempTasks = new List<Task>();
    StreamReader reader = new StreamReader(FILE_NAME);
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

        tempTasks.Add(temp);
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