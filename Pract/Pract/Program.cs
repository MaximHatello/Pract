using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

public class StudentGrade
{
    public enum Subject { Математика, Фізика, Хімія, Біологія, Англійська }

    public Subject Course { get; set; }
    public double Grade { get; set; }
    public bool IsPassed => Grade >= 60.0;

    public StudentGrade() { }

    public StudentGrade(Subject course, double grade)
    {
        Course = course;
        Grade = grade;
    }
}

public class ListNode
{
    public StudentGrade Data { get; set; }
    public ListNode Previous { get; set; }
    public ListNode Next { get; set; }

    public ListNode(StudentGrade data)
    {
        Data = data;
    }
}

public class DoublyLinkedList : IEnumerable<StudentGrade>
{
    private ListNode head;
    private ListNode tail;
    private int count;

    public int Count => count;

    public void AddFirst(StudentGrade data)
    {
        ListNode newNode = new ListNode(data);
        if (head == null)
        {
            head = tail = newNode;
        }
        else
        {
            newNode.Next = head;
            head.Previous = newNode;
            head = newNode;
        }
        count++;
    }

    public bool RemoveAt(int index)
    {
        if (index < 0 || index >= count)
            throw new IndexOutOfRangeException("Невірний індекс");

        if (index == 0)
        {
            RemoveFirst();
            return true;
        }

        if (index == count - 1)
        {
            RemoveLast();
            return true;
        }

        ListNode current = GetNodeAt(index);
        current.Previous.Next = current.Next;
        current.Next.Previous = current.Previous;
        count--;
        return true;
    }

    private void RemoveFirst()
    {
        if (head == null) return;
        head = head.Next;
        if (head != null) head.Previous = null;
        else tail = null;
        count--;
    }

    private void RemoveLast()
    {
        if (tail == null) return;
        tail = tail.Previous;
        if (tail != null) tail.Next = null;
        else head = null;
        count--;
    }

    public StudentGrade this[int index]
    {
        get
        {
            if (index < 0 || index >= count)
                throw new IndexOutOfRangeException("Невірний індекс");
            return GetNodeAt(index).Data;
        }
        set
        {
            if (index < 0 || index >= count)
                throw new IndexOutOfRangeException("Невірний індекс");
            GetNodeAt(index).Data = value;
        }
    }

    private ListNode GetNodeAt(int index)
    {
        ListNode current;
        if (index < count / 2)
        {
            current = head;
            for (int i = 0; i < index; i++)
                current = current.Next;
        }
        else
        {
            current = tail;
            for (int i = count - 1; i > index; i--)
                current = current.Previous;
        }
        return current;
    }

    public (double min, double max) GetMinMaxGrades()
    {
        if (count == 0)
            throw new InvalidOperationException("Список порожній");

        double min = 101.0;
        double max = -1.0;

        ListNode current = head;
        while (current != null)
        {
            if (current.Data.Grade < min) min = current.Data.Grade;
            if (current.Data.Grade > max) max = current.Data.Grade;
            current = current.Next;
        }

        return (min, max);
    }

    public List<StudentGrade> FindFailedMathStudents()
    {
        List<StudentGrade> result = new List<StudentGrade>();
        ListNode current = head;

        while (current != null)
        {
            if (current.Data.Course == StudentGrade.Subject.Математика &&
                current.Data.Grade < 60.0)
            {
                result.Add(current.Data);
            }
            current = current.Next;
        }

        return result;
    }

    public IEnumerator<StudentGrade> GetEnumerator()
    {
        ListNode current = head;
        while (current != null)
        {
            yield return current.Data;
            current = current.Next;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Serialize(string filePath)
    {
        List<StudentGrade> grades = new List<StudentGrade>(this);
        XmlSerializer serializer = new XmlSerializer(typeof(List<StudentGrade>));

        using (FileStream stream = new FileStream(filePath, FileMode.Create))
        {
            serializer.Serialize(stream, grades);
        }
    }

    public void Deserialize(string filePath)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(List<StudentGrade>));
        List<StudentGrade> grades;

        using (FileStream stream = new FileStream(filePath, FileMode.Open))
        {
            grades = (List<StudentGrade>)serializer.Deserialize(stream);
        }

        Clear();
        foreach (var grade in grades)
        {
            AddFirst(grade);
        }
    }

    private void Clear()
    {
        head = tail = null;
        count = 0;
    }
}

class Program
{
    static void Main()
    {
        DoublyLinkedList list = new DoublyLinkedList();

        Console.OutputEncoding = System.Text.Encoding.UTF8;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Меню керування списком оцінок");
            Console.WriteLine("1. Додати оцінку на початок");
            Console.WriteLine("2. Видалити оцінку за номером");
            Console.WriteLine("3. Змінити оцінку за номером");
            Console.WriteLine("4. Показати список оцінок");
            Console.WriteLine("5. Знайти мін/макс оцінки");
            Console.WriteLine("6. Знайти невдалі з математики");
            Console.WriteLine("7. Зберегти список у файл");
            Console.WriteLine("8. Завантажити список з файлу");
            Console.WriteLine("9. Вийти");
            Console.Write("Виберіть опцію: ");

            string choice = Console.ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    AddGrade(list);
                    break;
                case "2":
                    RemoveGrade(list);
                    break;
                case "3":
                    UpdateGrade(list);
                    break;
                case "4":
                    DisplayList(list);
                    break;
                case "5":
                    FindMinMax(list);
                    break;
                case "6":
                    FindFailedMath(list);
                    break;
                case "7":
                    SerializeList(list);
                    break;
                case "8":
                    DeserializeList(list);
                    break;
                case "9":
                    return;
                default:
                    Console.WriteLine("Невірний вибір!");
                    WaitForUser();
                    break;
            }
        }
    }

    static void AddGrade(DoublyLinkedList list)
    {
        try
        {
            Console.WriteLine("Доступні предмети:");
            int index = 1;
            foreach (var subject in Enum.GetValues(typeof(StudentGrade.Subject)))
            {
                Console.WriteLine($"{index}. {subject}");
                index++;
            }

            Console.Write("Виберіть предмет (1-5): ");
            int subjectId = int.Parse(Console.ReadLine());

            if (subjectId < 1 || subjectId > 5)
            {
                Console.WriteLine("Невірний номер предмета!");
                WaitForUser();
                return;
            }

            Console.Write("Введіть оцінку (0-100): ");
            double grade = double.Parse(Console.ReadLine());

            if (grade < 0 || grade > 100)
            {
                Console.WriteLine("Оцінка повинна бути від 0 до 100!");
                WaitForUser();
                return;
            }

            StudentGrade.Subject selectedSubject = (StudentGrade.Subject)(subjectId - 1);
            list.AddFirst(new StudentGrade(selectedSubject, grade));

            Console.WriteLine("\nОцінка успішно додана!");
            DisplayList(list);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
            WaitForUser();
        }
    }

    static void RemoveGrade(DoublyLinkedList list)
    {
        try
        {
            if (list.Count == 0)
            {
                Console.WriteLine("Список порожній!");
                WaitForUser();
                return;
            }

            DisplayList(list);
            Console.Write("\nВведіть номер для видалення (1-{0}): ", list.Count);
            int userIndex = int.Parse(Console.ReadLine());

            if (userIndex < 1 || userIndex > list.Count)
            {
                Console.WriteLine("Невірний номер!");
                WaitForUser();
                return;
            }

            list.RemoveAt(userIndex - 1);

            Console.WriteLine("\nОцінка успішно видалена!");
            DisplayList(list);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
            WaitForUser();
        }
    }

    static void DisplayList(DoublyLinkedList list)
    {
        Console.WriteLine("\nСписок оцінок:");
        Console.WriteLine("══════════════════════════════════════════════════");
        Console.WriteLine("| Номер | Предмет      | Оцінка | Зараховано |");
        Console.WriteLine("══════════════════════════════════════════════════");

        int index = 1;
        foreach (var grade in list)
        {
            Console.WriteLine($"| {index,5} | {grade.Course,-12} | {grade.Grade,6:F1} | {grade.IsPassed,-10} |");
            index++;
        }

        Console.WriteLine("══════════════════════════════════════════════════");
        Console.WriteLine($"Всього елементів: {list.Count}");
        WaitForUser();
    }

    static void FindMinMax(DoublyLinkedList list)
    {
        try
        {
            var (min, max) = list.GetMinMaxGrades();
            Console.WriteLine($"Мінімальна оцінка: {min:F1}");
            Console.WriteLine($"Максимальна оцінка: {max:F1}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
        }
        WaitForUser();
    }

    static void FindFailedMath(DoublyLinkedList list)
    {
        var failedStudents = list.FindFailedMathStudents();

        Console.WriteLine("Студенти, які не склали математику:");
        Console.WriteLine("════════════════════════════════════════════════");
        Console.WriteLine("| Предмет      | Оцінка | Зараховано |");
        Console.WriteLine("════════════════════════════════════════════════");

        foreach (var student in failedStudents)
        {
            Console.WriteLine($"| {student.Course,-12} | {student.Grade,6:F1} | {student.IsPassed,-10} |");
        }

        Console.WriteLine("════════════════════════════════════════════════");
        Console.WriteLine($"Знайдено: {failedStudents.Count} студентів");
        WaitForUser();
    }

    static void SerializeList(DoublyLinkedList list)
    {
        try
        {
            Console.Write("Введіть ім'я файлу для збереження: ");
            string fileName = Console.ReadLine();
            list.Serialize(fileName);
            Console.WriteLine("Список успішно збережено!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка серіалізації: {ex.Message}");
        }
        WaitForUser();
    }

    static void DeserializeList(DoublyLinkedList list)
    {
        try
        {
            Console.Write("Введіть ім'я файлу для завантаження: ");
            string fileName = Console.ReadLine();
            list.Deserialize(fileName);
            Console.WriteLine("Список успішно завантажено!");
            DisplayList(list);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка десеріалізації: {ex.Message}");
            WaitForUser();
        }
    }

    static void UpdateGrade(DoublyLinkedList list)
    {
        try
        {
            if (list.Count == 0)
            {
                Console.WriteLine("Список порожній!");
                WaitForUser();
                return;
            }

            DisplayList(list);
            Console.Write("Введіть номер оцінки для зміни (1-{0}): ", list.Count);
            int userIndex = int.Parse(Console.ReadLine());
            int index = userIndex - 1;

            if (index < 0 || index >= list.Count)
            {
                Console.WriteLine("Невірний номер!");
                WaitForUser();
                return;
            }
            
            StudentGrade current = list[index];
            Console.WriteLine("\nПоточні дані:");
            Console.WriteLine($"Предмет: {current.Course}");
            Console.WriteLine($"Оцінка: {current.Grade}");

            Console.Write("Виберіть новий предмет (1-5): ");
            int subjectId = int.Parse(Console.ReadLine());
            if (subjectId < 1 || subjectId > 5)
            {
                Console.WriteLine("Невірний номер предмета!");
                WaitForUser();
                return;
            }

            Console.Write("Введіть нову оцінку (0-100): ");
            double newGrade = double.Parse(Console.ReadLine());
            if (newGrade < 0 || newGrade > 100)
            {
                Console.WriteLine("Оцінка повинна бути від 0 до 100!");
                WaitForUser();
                return;
            }

            StudentGrade.Subject selectedSubject = (StudentGrade.Subject)(subjectId - 1);
            list[index] = new StudentGrade(selectedSubject, newGrade);

            Console.WriteLine("\nОцінка успішно оновлена!");
            DisplayList(list);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
            WaitForUser();
        }
    }

    static void WaitForUser()
    {
        Console.WriteLine("\nНатисніть будь-яку клавішу для повернення в меню...");
        Console.ReadKey();
    }
}