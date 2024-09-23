using System;
using System.Globalization;
 
public interface IPerson
{
   string Name { get; }
   string Patronomic { get; }
   string Lastname { get; }
   DateTime Date { get; }
   int Age { get; }
}

public class Student : IPerson
{
    public string Lastname { get; }
    public string Name { get; }
    public string Patronomic { get; }
    public DateTime Date { get; }
    public int Age { get{return CalculateAge(); } }
    public int Course { get; }
    public int Group { get; }
    public float Score { get; }

    public Student(string name, string lastname, string patronomic, DateTime date, int course, int group, float score)
    {
        Name = name;
        Patronomic = patronomic;
        Lastname = lastname;
        Date = date;
        Course = course;
        Group = group;
        Score = score;
    }

    private int CalculateAge()
    { 
        var today = DateTime.Today;
        var age = today.Year - Date.Year;
        if (today.Month < Date.Month || (today.Month == Date.Month && today.Day < Date.Day))
        {
            age--;
        }
        return age;
    }

    public override string ToString()
    {   
        string dateOfBirthFormatted = Date.ToString("dd-MM-yyyy");
        return $"{Lastname} {Name} {Patronomic}; {dateOfBirthFormatted}; {Age}; {Course}; {Group}; {Score}";
    }

    public static Student Parse(string text)
    {   
        var parts = text.Split(';');
        if (parts.Length != 7) throw new FormatException("Invalid format of input string");

        string lastName = parts[0].Trim();
        string firstName = parts[1].Trim();
        string patronymic = parts[2].Trim();
        DateTime dateOfBirth = DateTime.Parse(parts[3].Trim());
        if (dateOfBirth > DateTime.Now) throw new Exception("The date of birth should not be the future");

        int course = int.Parse(parts[4].Trim());
        int group = int.Parse(parts[5].Trim());
        float score = float.Parse(parts[6].Trim());

        return new Student(firstName, lastName, patronymic, dateOfBirth, course, group, score);
    }
}

public class Teacher : IPerson
{
    public string Lastname { get; }
    public string Name { get; }
    public string Patronomic { get; }
    public DateTime Date { get; }
    public int Age { get{return CalculateAge(); } }
    public string Department { get; }
    public int Experience { get; }

    public enum Position
    {
        Postgraduate,
        Professor,
        Docent,
        Senior_Lecturer,
        Junior_Researcher, 
        Researcher
    }

    public Position Post { get; }

    public Teacher(string name, string patronomic, string lastname, DateTime date, string department, int experience, Position post)
    {
        Name = name;
        Patronomic = patronomic;
        Lastname = lastname;
        Date = date;
        Department = department;
        Experience = experience;
        Post = post;
    }

    private int CalculateAge()
    {   
        var today = DateTime.Today;
        var age = today.Year - Date.Year;
        if (today.Month < Date.Month || (today.Month == Date.Month && today.Day < Date.Day))
        {
            age--;
        }
        return age;
    }

    public override string ToString()
    {
        string dateOfBirthFormatted = Date.ToString("dd-MM-yyyy");
        return $"{Lastname} {Name} {Patronomic}; {dateOfBirthFormatted}; {Age}; {Department}; {Experience}; {Post}";
    }

    public static Teacher Parse(string text)
    {
        var parts = text.Split(';');
        if (parts.Length != 7) throw new FormatException("Invalid format of input string");
		string lastName = parts[0].Trim();
        string firstName = parts[1].Trim();
        string patronymic = parts[2].Trim();
        DateTime dateOfBirth = DateTime.Parse(parts[3].Trim());
        if (dateOfBirth > DateTime.Now) throw new Exception("The date of birth should not be the future");
        string department = parts[4].Trim();
        int experience = int.Parse(parts[5].Trim());
        Position post = Enum.Parse<Position>(parts[6].Trim());
        return new Teacher(firstName, patronymic, lastName, dateOfBirth, department, experience, post);
    }
}
public interface IUniversity
{
   IEnumerable<IPerson> Persons { get; }   // отсортировать в соответствии с вариантом 1-й лабы
   IEnumerable<Student> Students { get; }  // отсортировать в соответствии с вариантом 1-й лабы
   IEnumerable<Teacher> Teachers { get; }  // отсортировать в соответствии с вариантом 1-й лабы

   void Add(IPerson person);
   void Remove(IPerson person);

   IEnumerable<IPerson> FindByLastName(string lastName);

   // Для нечетных вариантов. Выдать всех студентов, чей средний балл выше заданного.
   // Отсортировать по среднему баллу
   IEnumerable<Student > FindByAvrPoint(float avrPoint);

   // Для четных вариантов. Выдать всех преподавателей, название кафедры которых содержит
   // заданный текст. Отсортировать по должности.
   IEnumerable<Teacher> FindByDepartment(string text);
}

public class University: IUniversity{
    
   private List<IPerson> persons = new List<IPerson>();
   public IEnumerable<IPerson> Persons => persons.OrderBy(x => x.Lastname);
   public IEnumerable<Student> Students => persons.OfType<Student>().OrderBy(x => x.Lastname);
   public IEnumerable<Teacher> Teachers => persons.OfType<Teacher>().OrderBy(x => x.Lastname);
   public void Add(IPerson person)
    {
        persons.Add(person);
    }

    public void Remove(IPerson person)
    {
        persons.Remove(person);
    }
     public IEnumerable<IPerson> FindByLastName(string lastName)
    {
    return persons.Where(x => x.Lastname.Equals(lastName, StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<Student> FindByAvrPoint(float avrPoint)
    {
        return persons.OfType<Student>().Where(x => x.Score > avrPoint).OrderByDescending(x => x.Score);
    }
    public IEnumerable<Teacher> FindByDepartment(string text)
    {
        return persons.OfType<Teacher>().Where(x => x.Department.Contains(text, StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.Post);
    }
            
                      

}
class Program
{
    static University university = new University();

    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("1. Add a student");
            Console.WriteLine("2. Add a teacher");
            Console.WriteLine("3. Find by last name");
            Console.WriteLine("4. Show students with a score above average");
            Console.WriteLine("5. Remove a person from the database");
            Console.WriteLine("6. Exit");
            int choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    AddStudent();
                    break;
                case 2:
                    AddTeacher();
                    break;
                case 3:
                    FindByLastName();
                    break;
                case 4:
                    FindStudentsByAverageScore();
                    break;
                case 5:
                        RemovePerson();
                    break;
                case 6:
                    return;
                default:
                    Console.WriteLine("Wrong choice.");
                    break;
            }
        }
    }
    static void AddStudent()
    {
        Console.WriteLine("Enter the student's details (Last name; First name; Patronymic; Date of birth; Course; Group; Score):");
        string data = Console.ReadLine();
        try
        {
            Student student = Student.Parse(data);
            university.Add(student);
            Console.WriteLine("Student added.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
	static void AddTeacher()
    {
        Console.WriteLine("Enter the data of the teacher (Last name; First name; Patronymic; Date of birth; Department; Length of service; Position(P.S.Postgraduate, Professor, Docent, Senior_Lecturer, Junior_Researcher, Researcher)):");
        string data = Console.ReadLine();
        try
        {
            Teacher teacher = Teacher.Parse(data);
            university.Add(teacher);
            Console.WriteLine("The teacher has been added.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static void FindByLastName()
    {
        Console.WriteLine("Enter your last name to search for:");
        string lastName = Console.ReadLine();
        var people = university.FindByLastName(lastName);
         if (!people.Any())
        {
            Console.WriteLine("No person found with the specified last name.");
        }
        else{
            foreach (var person in people)
            {
                Console.WriteLine(person);
            }
        }
    }

   static void FindStudentsByAverageScore()
{
    try
    {
        Console.WriteLine("Enter the minimum score:");
        float score = float.Parse(Console.ReadLine());
        var students = university.FindByAvrPoint(score);

        if (!students.Any())
        {
            Console.WriteLine("No students found with a score above the given value.");
        }
        else
        {
            foreach (var student in students)
            {
                Console.WriteLine(student);
            }
        }
    }
    catch (FormatException)
    {
        Console.WriteLine("Invalid format. Please enter a valid number.");
    }
}

    static void RemovePerson()
{
    Console.WriteLine("Enter the last name to delete:");
    string lastName = Console.ReadLine();
    var people = university.FindByLastName(lastName);
    
    if (!people.Any())
    {
        Console.WriteLine("No person found with the specified last name.");
    }
    else if (people.Count() > 1)
    {
        Console.WriteLine("Multiple people found with the same last name. Please enter the first name to delete:");
         foreach (var person in people)
            {
                Console.WriteLine(person);
            }
        string firstName = Console.ReadLine();
        var personToRemove = people.FirstOrDefault(x => x.Name.Equals(firstName, StringComparison.OrdinalIgnoreCase));
        
        if (personToRemove != null)
        {
            university.Remove(personToRemove);
            Console.WriteLine("Deleted");
        }
        else
        {
            Console.WriteLine("No person found with the specified name and last name.");
        }
    }
    else
    {
        university.Remove(people.First());
        Console.WriteLine("Deleted");
    }
}
}