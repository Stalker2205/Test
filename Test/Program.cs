using System.Linq;
using System.Text;
using Newtonsoft.Json;
bool key = false;
bool Pathkey = false;
string? FilePath = null; ;

List<Worker> workers = new List<Worker>();
while (!Pathkey)
{
    Console.WriteLine("Введите полный путь к фаулу");
    FilePath = Console.ReadLine();
    if (!File.Exists(FilePath))
        Console.WriteLine("файла не существует");
    else
        try
        {
            Deserialize();
            Pathkey = true;
        }
        catch (Exception) { Console.WriteLine("Выбранный формат не поддерживается"); }
}
while (!key)
{
    string comm;
    Console.WriteLine("Введите команду:\n add \n update \n get \n delete \n getall\n exit");
    comm = Console.ReadLine().ToLower();
    switch (comm)
    {
        case "add":
            {
                Console.WriteLine("Ведите:FirstName,LastName,SalaryPerHour");
                try
                {
                    add(Console.ReadLine(),
                        Console.ReadLine(),
                        Convert.ToDecimal(Console.ReadLine()));
                }
                catch (Exception) { Console.WriteLine("Введите корректные значения"); goto case "add"; }
                break;
            }
        case "update":
            {
                bool key1 = false;
                Console.WriteLine("Введите:id для изменения");
                int id;
                try
                {
                    id = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception) { Console.WriteLine("Введике корректный id"); goto case "update"; }
                Worker worker = get(id);
                if (worker == null)
                {
                    Console.WriteLine("Такого id не существует"); goto case "update";
                }
                while (!key1)
                {
                    Console.WriteLine("Доступные поля: FirstName, LastName, SalaryPerHour");
                    string? UpdatedField = Console.ReadLine();
                    if (UpdatedField == null)
                    {
                        Console.WriteLine("Введите изменяемое поле");
                    }
                    else
                    {
                        Console.WriteLine($"Введите {UpdatedField} ");
                        string? Data = Console.ReadLine();
                        if (Data == null)
                        {
                            Console.WriteLine("Введите данные");
                        }
                        else
                        {
                            try
                            {
                                update(id, UpdatedField.ToLower(), Data);
                                key1 = true;
                            }
                            catch (ArgumentException) { Console.WriteLine("Введите корректные аргументы"); }
                        }
                    }

                }
                break;
            }
        case "get":
            {
                int id;
                Console.WriteLine("Введите id");
                try
                {
                    id = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception)
                {
                    Console.WriteLine("Id - числовое значение");
                    goto case "get";
                }
                Worker? worker = get(id);
                if (worker == null)
                {
                    Console.WriteLine("Такого id не существует");
                    goto case "get";
                }
                else
                {
                    Console.WriteLine($"id = {worker.Id.ToString()}" +
                                      $" FirstName = {worker.FirstName}" +
                                      $" LastName = {worker.LastName}" +
                                      $" SalaryPerHour = {worker.SalaryPerHour.ToString()}");
                }
                break;
            }
        case "delete":
            {
                int id;
                Console.WriteLine("Введите id");
                try
                {
                    id = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception)
                {
                    Console.WriteLine("Id - числовое значение");
                    goto case "get";
                }
                try
                {
                    delete(id);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("Данного id не существует");
                    goto case "get";
                }
                break;
            }
        case "getall":
            {
                List<string> allWorkers = getall();
                foreach (var item in allWorkers)
                {
                    Console.WriteLine(item);
                }
                break;
            }
        case "exit":
            {
                key = true;
                break;
            }
        default:
            Console.WriteLine("Введите корректную команду");
            break;

    }
}


List<string> getall()
{
    List<string> AllWorker = new List<string>();
    foreach (var item in workers)
    {
        string info = $"id = {item.Id} " +
                      $"FirstName = {item.FirstName} " +
                      $"LastName = {item.LastName} " +
                      $"SalaryPerHour = {item.SalaryPerHour}";
        AllWorker.Add(info);
    }
    return AllWorker;
}
void delete(int id)
{
    Worker? worker = get(id);
    if (worker == null)
        throw new ArgumentException("this id does not exist");
    else
    {
        workers.Remove(worker);
        serialize(workers);
        Deserialize();
    }
}
Worker? get(int id)
{
    Worker? worker = new Worker();
    try
    {
        worker = workers.Where(x => x.Id == id).First();
    }
    catch (Exception) { worker = null; }
    if (worker == null)
        return null;
    else
        return worker;
}
void update(int Id, string editableаield, string meaning)
{
    Worker? worker = get(Id);
    if (worker == null)
        throw new InvalidDataException("this id does not exist");
    else
    {
        switch (editableаield)
        {
            case "firstname":
                {
                    worker.FirstName = meaning;
                    serialize(workers);
                    break;
                }
            case "lastname":
                {
                    worker.LastName = meaning;
                    serialize(workers);
                    break;
                }
            case "salaryperhour":
                {
                    try
                    {
                        worker.SalaryPerHour = Convert.ToDecimal(meaning);
                    }
                    catch (Exception)
                    {
                        throw new ArgumentException("this field is not decimal");
                    }
                    serialize(workers);
                    break;
                }
            default:
                {
                    throw new ArgumentException("this field does not exist");
                }
        }
    }
    Deserialize();
}
void add(string FirstName, string LastName, decimal SalaryPerHour)
{
    int MaxId = workers.Select(x => x.Id).Max();
    Worker worker = new Worker(++MaxId, FirstName, LastName, SalaryPerHour);
    workers.Add(worker);
    serialize(workers);
    Deserialize();
}


void serialize(List<Worker> wor)
{
    using (StreamWriter fs = new StreamWriter(FilePath))
    {
        var serializer = new JsonSerializer();
        using (var jsonTextWriter = new JsonTextWriter(fs))
        {
            serializer.Serialize(fs, wor);
        }
    }
}
void Deserialize()
{
    try
    {
        using (StreamReader file = File.OpenText(FilePath))
        {
            JsonSerializer serializer = new JsonSerializer();
            try
            {
                workers = (List<Worker>)serializer.Deserialize(file, typeof(List<Worker>));
            }
            catch (Exception)
            {
                throw new Exception("invalid format json");
            }

        }
    }
    catch (Exception)
    {
        throw new FileNotFoundException("FileNotFound");
    }
}


public class Worker
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public decimal? SalaryPerHour { get; set; }
    public Worker() { }
    public Worker(int id, string firstName, string lastName, decimal salaryPerHour)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        SalaryPerHour = salaryPerHour;
    }
}
