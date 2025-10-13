public enum cat
{
    Philosophy = 1,
    Anatomy,
    Mathematics
}
class Person
{
    private string FIO;
    private DateOnly Birthday; 
    private string Gender;

    public Person(string fio, DateOnly birthday, string gender)
    {
        FIO = fio;
        Birthday = birthday;
        Gender = gender;
    }
    public virtual void Print()
    {
        Console.WriteLine($"FIO: {FIO}\nBirthday: {Birthday}\nGender: {Gender} ");
    }
}

class Student : Person
{
    private int StudentNumberID;
    private bool PCExperience;
    private cat[] course;
    private string TheHealthGroup;

    public Student(int id, string fio, DateOnly birthday, string gender, bool PCExperience, string TheHealthGroup, cat[] course)
        : base(fio, birthday, gender) 
    {
        StudentNumberID = id;
        this.PCExperience = PCExperience;
        this.course = course;
        this.TheHealthGroup = TheHealthGroup;
    }

    public override void Print()
    {
        Console.WriteLine("Student");
        base.Print();
        Console.WriteLine($"StudentNumberID: {StudentNumberID}\nPCExperience: {PCExperience}\nTheHealthGroup: {TheHealthGroup}");
        Console.Write("Courses: ");
        for (int i = 0; i < course.Length; i++)
        {
            Console.Write(course[i]);
            if (i < course.Length - 1)
                Console.Write(", ");
        }
        Console.WriteLine("\n");
    }
}
class Teacher : Person
{
    private cat Subject;
    private int ExperienceYears;

    public Teacher(string fio, DateOnly birthday, string gender, cat subject, int PCExperience)
        : base(fio, birthday, gender)
    {
        Subject = subject;
        ExperienceYears = PCExperience;
    }

    public override void Print()
    {
        Console.WriteLine("Teacher");
        base.Print();
        Console.WriteLine($"Subject: {Subject}\nExperienceYears: {ExperienceYears}\n");
    }
}
class Program
{
    static List<Student> students = new List<Student>();
    static List<Teacher> teachers = new List<Teacher>();
    static int ID = 4;
    static void Main(string[] args)
    {
     Student student1 = new Student(
    id: 1,
    fio: "Иванов Иван Иванович",
    birthday: new DateOnly(2003, 5, 15),
    gender: "Мужской",
    PCExperience: true,
    TheHealthGroup: "Основная",
    course: new cat[] {(cat)1}
    );
    Student student2 = new Student(
    id: 2,
    fio: "Петрова Анна Сергеевна",
    birthday: new DateOnly(2002, 8, 22),
    gender: "Женский",
    PCExperience: false,
    TheHealthGroup: "Подготовительная",
    course: new cat[] { (cat)2, (cat)3 }
    );
    Student student3 = new Student(
    id: 3,
    fio: "Сидоров Алексей Петрович",
    birthday: new DateOnly(2004, 1, 10),
    gender: "Мужской",
    PCExperience: true,
    TheHealthGroup: "Специальная",
    course: new cat[] {(cat)1, (cat)2}
    );
    Teacher teacher1 = new Teacher(
    fio: "Козлова Мария Владимировна",
    birthday: new DateOnly(1980, 3, 12),
    gender: "Женский",
    subject: (cat)3,
    PCExperience: 15
    );
    Teacher teacher2 = new Teacher(
    fio: "Николаев Дмитрий Сергеевич",
    birthday: new DateOnly(1975, 11, 5),
    gender: "Мужской",
    subject: (cat)1,
    PCExperience: 20
    );
    Teacher teacher3 = new Teacher(
    fio: "Васильева Елена Игоревна",
    birthday: new DateOnly(1990, 7, 30),
    gender: "Женский",
    subject: (cat)2,
    PCExperience: 8
    );
        bool cont = true;
        while (cont)
        {
            Console.WriteLine("\n=== МЕНЮ ===");
            Console.WriteLine("1. Добавить cтудента");
            Console.WriteLine("2. Добавить преподавателя");
            Console.WriteLine("3. Добавить курс"); 
            Console.WriteLine("4. Вывод информации по студентам");
            Console.WriteLine("5. Вывод информации по преподавателям"); 
            Console.WriteLine("6. Вывод информации по курсам");
            Console.WriteLine("0. Выход");
            Console.Write("Выберите пункт меню: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    break;
                case "2":
                    break;
                case "3":
                    break;
                case "4":
                    break;
                case "5":
                    break;
                case "6":
                    break;
                case "0":
                    Console.WriteLine("Завершение программы");
                    return;
                default:
                    Console.WriteLine("Неверный выбор");
                    break;
            }
            Console.WriteLine("Хотите вернуться в меню? (1-да, 0-нет)");
            string end = Console.ReadLine();
            cont = (end == "1");
        }
    }
}

