using System;
using System.Collections.Generic;
using System.Linq;

public class Course
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Course(int id, string name, string description = "")
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public override string ToString()
    {
        return Name;
    }
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
        Console.WriteLine($"ФИО: {FIO}\nДата рождения: {Birthday}\nПол: {Gender}");
    }
    public string GetFIO() => FIO;
    public DateOnly GetBirthday() => Birthday;
    public string GetGender() => Gender;
}

class Student : Person
{
    private int StudentNumberID;
    private bool PCExperience;
    private List<Course> courses;
    private string TheHealthGroup;

    public Student(int id, string fio, DateOnly birthday, string gender, bool PCExperience, string TheHealthGroup, Course[] course)
        : base(fio, birthday, gender)
    {
        StudentNumberID = id;
        this.PCExperience = PCExperience;
        this.courses = new List<Course>(course);
        this.TheHealthGroup = TheHealthGroup;
    }

    public override void Print()
    {
        Console.WriteLine("=== СТУДЕНТ ===");
        base.Print();
        Console.WriteLine($"ID студента: {StudentNumberID}\nОпыт работы с ПК: {PCExperience}\nГруппа здоровья: {TheHealthGroup}");
        Console.Write("Курсы: ");
        if (courses.Count == 0)
        {
            Console.WriteLine("нет курсов");
        }
        else
        {
            for (int i = 0; i < courses.Count; i++)
            {
                Console.Write(courses[i].Name);
                if (i < courses.Count - 1)
                    Console.Write(", ");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    public void AddCourse(Course course)
    {
        if (!courses.Any(c => c.Id == course.Id))
        {
            courses.Add(course);
            Console.WriteLine($"Курс '{course.Name}' добавлен студенту {GetFIO()}");
        }
        else
        {
            Console.WriteLine($"Студент {GetFIO()} уже записан на курс '{course.Name}'");
        }
    }
    public int GetStudentNumberID() => StudentNumberID;
    public bool GetPCExperience() => PCExperience;
    public List<Course> GetCourses() => courses;
    public string GetHealthGroup() => TheHealthGroup;
}

class Teacher : Person
{
    private Course Subject;
    private int ExperienceYears;
    public Teacher(string fio, DateOnly birthday, string gender, Course subject, int experienceYears)
        : base(fio, birthday, gender)
    {
        Subject = subject;
        ExperienceYears = experienceYears;
    }
    public override void Print()
    {
        Console.WriteLine("=== ПРЕПОДАВАТЕЛЬ ===");
        base.Print();
        Console.WriteLine($"Предмет: {Subject.Name}\nСтаж работы: {ExperienceYears} лет\n");
    }
    public Course GetSubject() => Subject;
    public int GetExperienceYears() => ExperienceYears;
}

class Program
{
    static List<Student> students = new List<Student>();
    static List<Teacher> teachers = new List<Teacher>();
    static List<Course> courses = new List<Course>();
    static int studentID = 3;
    static int teacherID = 3;
    static int courseID = 3;
    static void Main(string[] args)
    {
        InitializeData();
        bool cont = true;
        while (cont)
        {
            Console.WriteLine("\n=== ГЛАВНОЕ МЕНЮ ===");
            Console.WriteLine("1. Добавить студента");
            Console.WriteLine("2. Добавить преподавателя");
            Console.WriteLine("3. Добавить курс");
            Console.WriteLine("4. Вывод информации по студентам");
            Console.WriteLine("5. Вывод информации по преподавателям");
            Console.WriteLine("6. Вывод информации по курсам");
            Console.WriteLine("7. Добавить курс к студенту");
            Console.WriteLine("0. Выход");
            Console.Write("Выберите пункт меню: ");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    AddStudent();
                    break;
                case "2":
                    AddTeacher();
                    break;
                case "3":
                    AddCourse();
                    break;
                case "4":
                    PrintStudentsInfo();
                    break;
                case "5":
                    PrintTeachersInfo();
                    break;
                case "6":
                    PrintCoursesInfo();
                    break;
                case "7":
                    AddCourseToStudent();
                    break;
                case "0":
                    Console.WriteLine("Завершение программы");
                    return;
                default:
                    Console.WriteLine("Неверный выбор");
                    break;
            }

            if (choice != "0")
            {
                Console.WriteLine("\nХотите вернуться в меню? (1-да, 0-нет)");
                string end = Console.ReadLine();
                cont = (end == "1");
            }
        }
    }

    static void InitializeData()
    {
        Course philosophy = new Course(1, "Философия", "Изучение основных философских концепций");
        Course anatomy = new Course(2, "Анатомия", "Изучение строения человеческого тела");
        Course mathematics = new Course(3, "Математика", "Изучение математических дисциплин");

        courses.Add(philosophy);
        courses.Add(anatomy);
        courses.Add(mathematics);

        Student student1 = new Student(
            id: 1,
            fio: "Иванов Иван Иванович",
            birthday: new DateOnly(2003, 5, 15),
            gender: "Мужской",
            PCExperience: true,
            TheHealthGroup: "Основная",
            course: new Course[] { philosophy }
        );

        Student student2 = new Student(
            id: 2,
            fio: "Петрова Анна Сергеевна",
            birthday: new DateOnly(2002, 8, 22),
            gender: "Женский",
            PCExperience: false,
            TheHealthGroup: "Подготовительная",
            course: new Course[] { anatomy, mathematics }
        );

        Student student3 = new Student(
            id: 3,
            fio: "Сидоров Алексей Петрович",
            birthday: new DateOnly(2004, 1, 10),
            gender: "Мужской",
            PCExperience: true,
            TheHealthGroup: "Специальная",
            course: new Course[] { philosophy, anatomy }
        );

        Teacher teacher1 = new Teacher(
            fio: "Козлова Мария Владимировна",
            birthday: new DateOnly(1980, 3, 12),
            gender: "Женский",
            subject: mathematics,
            experienceYears: 15
        );

        Teacher teacher2 = new Teacher(
            fio: "Николаев Дмитрий Сергеевич",
            birthday: new DateOnly(1975, 11, 5),
            gender: "Мужской",
            subject: philosophy,
            experienceYears: 20
        );

        Teacher teacher3 = new Teacher(
            fio: "Васильева Елена Игоревна",
            birthday: new DateOnly(1990, 7, 30),
            gender: "Женский",
            subject: anatomy,
            experienceYears: 8
        );

        students.Add(student1);
        students.Add(student2);
        students.Add(student3);
        teachers.Add(teacher1);
        teachers.Add(teacher2);
        teachers.Add(teacher3);
    }
    static void AddStudent()
    {
        Console.WriteLine("\n=== Добавление студента ===");
        studentID++;

        Console.Write("Введите ФИО: ");
        string fio = Console.ReadLine();

        Console.Write("Введите дату рождения (гггг-мм-дд): ");
        DateOnly birthday = DateOnly.Parse(Console.ReadLine());

        Console.Write("Введите пол: ");
        string gender = Console.ReadLine();

        Console.Write("Есть ли опыт работы с ПК (true/false): ");
        bool pcExperience = bool.Parse(Console.ReadLine());

        Console.Write("Введите группу здоровья: ");
        string healthGroup = Console.ReadLine();

        Console.WriteLine("\nДоступные курсы:");
        PrintCoursesInfo();

        Console.Write("Введите ID курсов через запятую: ");
        string[] courseIds = Console.ReadLine().Split(',');

        List<Course> studentCourses = new List<Course>();
        foreach (string idStr in courseIds)
        {
            if (int.TryParse(idStr.Trim(), out int courseId))
            {
                Course course = courses.FirstOrDefault(c => c.Id == courseId);
                if (course != null)
                {
                    studentCourses.Add(course);
                }
            }
        }

        Student newStudent = new Student(
            studentID,
            fio,
            birthday,
            gender,
            pcExperience,
            healthGroup,
            studentCourses.ToArray()
        );

        students.Add(newStudent);
        Console.WriteLine($"\nСтудент {fio} успешно добавлен с ID: {studentID}!");
    }

    static void AddTeacher()
    {
        Console.WriteLine("\n=== Добавление преподавателя ===");
        teacherID++;

        Console.Write("Введите ФИО: ");
        string fio = Console.ReadLine();

        Console.Write("Введите дату рождения (гггг-мм-дд): ");
        DateOnly birthday = DateOnly.Parse(Console.ReadLine());

        Console.Write("Введите пол: ");
        string gender = Console.ReadLine();

        Console.WriteLine("\nДоступные предметы:");
        PrintCoursesInfo();

        Console.Write("Введите ID предмета: ");
        int subjectId = int.Parse(Console.ReadLine());
        Course subject = courses.FirstOrDefault(c => c.Id == subjectId);

        if (subject == null)
        {
            Console.WriteLine("Предмет с таким ID не найден!");
            return;
        }

        Console.Write("Введите стаж работы (лет): ");
        int experience = int.Parse(Console.ReadLine());

        Teacher newTeacher = new Teacher(fio, birthday, gender, subject, experience);
        teachers.Add(newTeacher);
        Console.WriteLine($"\nПреподаватель {fio} успешно добавлен!");
    }
    static void AddCourse()
    {
        Console.WriteLine("\n=== Добавление курса ===");
        courseID++;

        Console.Write("Введите название курса: ");
        string name = Console.ReadLine();

        Console.Write("Введите описание курса: ");
        string description = Console.ReadLine();

        Course newCourse = new Course(courseID, name, description);
        courses.Add(newCourse);

        Console.WriteLine($"\nКурс '{name}' успешно добавлен с ID: {courseID}!");
    }

    static void PrintStudentsInfo()
    {
        Console.WriteLine("\n=== ИНФОРМАЦИЯ О СТУДЕНТАХ ===");
        if (students.Count == 0)
        {
            Console.WriteLine("Студентов нет.");
            return;
        }

        foreach (var student in students)
        {
            student.Print();
        }
    }

    static void PrintTeachersInfo()
    {
        Console.WriteLine("\n=== ИНФОРМАЦИЯ О ПРЕПОДАВАТЕЛЯХ ===");
        if (teachers.Count == 0)
        {
            Console.WriteLine("Преподавателей нет.");
            return;
        }

        foreach (var teacher in teachers)
        {
            teacher.Print();
        }
    }

    static void PrintCoursesInfo()
    {
        Console.WriteLine("\n=== ИНФОРМАЦИЯ О КУРСАХ ===");
        if (courses.Count == 0)
        {
            Console.WriteLine("Курсов нет.");
            return;
        }

        foreach (var course in courses)
        {
            Console.WriteLine($"ID: {course.Id}, Название: {course.Name}, Описание: {course.Description}");
        }
    }

    static void AddCourseToStudent()
    {
        Console.WriteLine("\n=== ДОБАВЛЕНИЕ КУРСА СТУДЕНТУ ===");

        if (students.Count == 0)
        {
            Console.WriteLine("Нет доступных студентов.");
            return;
        }

        if (courses.Count == 0)
        {
            Console.WriteLine("Нет доступных курсов.");
            return;
        }

        Console.WriteLine("Список студентов:");
        foreach (var student in students)
        {
            Console.WriteLine($"ID: {student.GetStudentNumberID()}, ФИО: {student.GetFIO()}");
        }

        Console.Write("Введите ID студента: ");
        int studentId = int.Parse(Console.ReadLine());
        Student selectedStudent = students.FirstOrDefault(s => s.GetStudentNumberID() == studentId);

        if (selectedStudent == null)
        {
            Console.WriteLine("Студент с таким ID не найден!");
            return;
        }

        Console.WriteLine("\nДоступные курсы:");
        PrintCoursesInfo();

        Console.Write("Введите ID курса для добавления: ");
        int courseId = int.Parse(Console.ReadLine());
        Course selectedCourse = courses.FirstOrDefault(c => c.Id == courseId);

        if (selectedCourse == null)
        {
            Console.WriteLine("Курс с таким ID не найден!");
            return;
        }

        selectedStudent.AddCourse(selectedCourse);
    }
}

