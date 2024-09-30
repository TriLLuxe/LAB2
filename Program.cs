using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.IO;

public interface IPerson
{
    string Name { get; }
    string Patronomic { get; }
    string Lastname { get; }
    DateTime Date { get; }
    int Age { get; }
}

public record Student(
    string Name, 
    string Lastname, 
    string Patronomic, 
    DateTime Date, 
    int Course, 
    int Group, 
    float Score
) : IPerson
{
    public int Age => CalculateAge();

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
        return $"{Lastname} {Name} {Patronomic}; {dateOfBirthFormatted}; {Course}; {Group}; {Score}; {Age}";
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

public record Teacher(
    string Name, 
    string Patronomic, 
    string Lastname, 
    DateTime Date, 
    string Department, 
    int Experience, 
    Teacher.Position Post
) : IPerson
{
    public enum Position
    {
        Postgraduate,
        Professor,
        Docent,
        Senior_Lecturer,
        Junior_Researcher,
        Researcher
    }

    public int Age => CalculateAge();

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
        return $"{Lastname} {Name} {Patronomic}; {dateOfBirthFormatted}; {Department}; {Experience}; {Post}; {Age}";
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
    IEnumerable<IPerson> Persons { get; }   
    IEnumerable<Student> Students { get; }  
    IEnumerable<Teacher> Teachers { get; }  

    void Add(IPerson person);
    void Remove(IPerson person);

    IEnumerable<IPerson> FindByLastName(string lastName);

    IEnumerable<Student> FindByAvrPoint(float avrPoint);
    IEnumerable<Teacher> FindByDepartment(string text);
}

public class University : IUniversity
{
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
        if (File.Exists("Students.txt"))
        {
            string[] Students = File.ReadAllLines("Students.txt");
            foreach (string s in Students)
            {
                university.Add(Student.Parse(s));
            }
        }
        
        if (File.Exists("Teachers.txt"))
        {
            string[] Teachers = File.ReadAllLines("Teachers.txt");
            foreach (string s in Teachers)
            {
                university.Add(Teacher.Parse(s));
            }
        }

        while (true)
        {
            Console.WriteLine("1. Add a student");
            Console.WriteLine("2. Add a teacher");
            Console.WriteLine("3. Find by last name");
            Console.WriteLine("4. Show students with a score above average");
            Console.WriteLine("5. Remove a student");
            Console.WriteLine("6. Remove a teacher");
            Console.WriteLine("7. Display the list of students");
            Console.WriteLine("8. Display a list of teachers");
            Console.WriteLine("9. Save students database");
            Console.WriteLine("10. Save teachers database");
            Console.WriteLine("11. Exit");
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
                    RemoveStudent();
                    break;
                case 6:
                    RemoveTeacher();
                    break;
                case 7:
                    PrintStudents();
                    break;
                case 8:
                    PrintTeachers();
                    break;
                case 9:
                    SaveStudents();
                    break;
                case 10:
                    SaveTeachers();
                    break;
                case 11:
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
        Console.WriteLine("Enter the data of the teacher (Last name; First name; Patronymic; Date of birth; Department; Length of service; Position(P.S.Postgraduate, Professor, Docent, Senior_Lecturer, Junior_Researcher, Researcher)): ");
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
        var people = university.FindByLastName(lastName).ToList();
        if (!people.Any())
        {
            Console.WriteLine("No person found with the specified last name.");
        }
        else
        {
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

    static void RemoveStudent()
    {
        Console.WriteLine("Enter the last name of the student to delete:");
        string lastName = Console.ReadLine();
        var students = university.Students.Where(s => s.Lastname.Equals(lastName, StringComparison.OrdinalIgnoreCase)).ToList();

        if (!students.Any())
        {
            Console.WriteLine("No student found with the specified last name.");
        }
        else
        {
            var student = students.First();
            university.Remove(student);
            Console.WriteLine("Student removed.");
        }
    }

    static void RemoveTeacher()
    {
        Console.WriteLine("Enter the last name of the teacher to delete:");
        string lastName = Console.ReadLine();
        var teachers = university.Teachers.Where(t => t.Lastname.Equals(lastName, StringComparison.OrdinalIgnoreCase)).ToList();

        if (!teachers.Any())
        {
            Console.WriteLine("No teacher found with the specified last name.");
        }
        else
        {
            var teacher = teachers.First();
            university.Remove(teacher);
            Console.WriteLine("Teacher removed.");
        }
    }

    static void PrintStudents()
    {
        Console.WriteLine("Список студентов:");
        foreach(var students in university.Students)
        {
            Console.WriteLine(students.ToString());
        }
    }

    static void PrintTeachers()
    {
        Console.WriteLine("Список преподавателей:");
        foreach(var teachers in university.Teachers)
        {
            Console.WriteLine(teachers.ToString());
        }
    }

    static void SaveStudents()
    {
        List<string> output = new List<string>();
        foreach(var student in university.Students)
            output.Add(student.ToString());
        File.WriteAllLines("Students.txt", output);
        Console.WriteLine("Students saved to file.");
    }

    static void SaveTeachers()
    {
        List<string> output = new List<string>();
        foreach(var teacher in university.Teachers)
            output.Add(teacher.ToString());
        File.WriteAllLines("Teachers.txt", output);
        Console.WriteLine("Teachers saved to file.");
    }
}
