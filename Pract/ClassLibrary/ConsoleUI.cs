using System;
using System.Collections.Generic;

namespace StudentGradesLibrary
{
    public class ConsoleUI
    {
        private readonly DoublyLinkedList list = new DoublyLinkedList();

        public void Run()
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("Student Grades Management System");
                Console.WriteLine("1. Add grade at the beginning");
                Console.WriteLine("2. Remove grade by index");
                Console.WriteLine("3. Display all grades");
                Console.WriteLine("4. Find min and max grades");
                Console.WriteLine("5. Find failed math students");
                Console.WriteLine("6. Serialize list to file");
                Console.WriteLine("7. Deserialize list from file");
                Console.WriteLine("8. Exit");
                Console.Write("Select an option: ");

                switch (Console.ReadLine())
                {
                    case "1": AddGrade(); break;
                    case "2": RemoveGrade(); break;
                    case "3": DisplayList(); break;
                    case "4": FindMinMax(); break;
                    case "5": FindFailedMath(); break;
                    case "6": SerializeList(); break;
                    case "7": DeserializeList(); break;
                    case "8": exit = true; break;
                    default: ShowError("Invalid choice!"); break;
                }
            }
        }

        private void AddGrade()
        {
            try
            {
                Console.WriteLine("\nAvailable subjects:");
                foreach (var subject in Enum.GetValues(typeof(StudentGrade.Subject)))
                {
                    Console.WriteLine($"{(int)subject}. {subject}");
                }

                Console.Write("Select a subject (0-4): ");
                int subjectId = int.Parse(Console.ReadLine());

                Console.Write("Enter grade (0-100): ");
                double grade = double.Parse(Console.ReadLine());

                if (grade < 0 || grade > 100)
                    throw new ArgumentException("Grade must be between 0 and 100");

                StudentGrade.Subject selectedSubject = (StudentGrade.Subject)subjectId;
                list.AddFirst(new StudentGrade(selectedSubject, grade));

                ShowSuccess("Grade added successfully!");
                DisplayList();
            }
            catch (Exception ex)
            {
                ShowError($"Error: {ex.Message}");
            }
        }

        private void RemoveGrade()
        {
            try
            {
                if (list.Count == 0)
                {
                    ShowError("The list is empty!");
                    return;
                }

                DisplayList();
                Console.Write("Enter index to remove: ");
                int index = int.Parse(Console.ReadLine());

                list.RemoveAt(index);

                ShowSuccess("Grade removed successfully!");
                DisplayList();
            }
            catch (Exception ex)
            {
                ShowError($"Error: {ex.Message}");
            }
        }

        private void DisplayList()
        {
            Console.WriteLine("\nList of Grades:");
            Console.WriteLine("════════════════════════════════════════════");
            Console.WriteLine("| Index | Subject      | Grade | Passed |");
            Console.WriteLine("════════════════════════════════════════════");

            int index = 0;
            foreach (var grade in list)
            {
                Console.WriteLine($"| {index,5} | {grade.Course,-12} | {grade.Grade,5:F1} | {grade.IsPassed,-6} |");
                index++;
            }

            Console.WriteLine("════════════════════════════════════════════");
            Console.WriteLine($"Total items: {list.Count}");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void FindMinMax()
        {
            try
            {
                var (min, max) = list.GetMinMaxGrades();
                Console.WriteLine($"\nMin grade: {min:F1}");
                Console.WriteLine($"Max grade: {max:F1}");
            }
            catch (Exception ex)
            {
                ShowError($"Error: {ex.Message}");
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void FindFailedMath()
        {
            var failedStudents = list.FindFailedMathStudents();
            Console.WriteLine("\nStudents who failed Mathematics:");
            Console.WriteLine("════════════════════════════════════════════");
            Console.WriteLine("| Subject      | Grade | Passed |");
            Console.WriteLine("════════════════════════════════════════════");

            foreach (var student in failedStudents)
            {
                Console.WriteLine($"| {student.Course,-12} | {student.Grade,5:F1} | {student.IsPassed,-6} |");
            }

            Console.WriteLine("════════════════════════════════════════════");
            Console.WriteLine($"Total found: {failedStudents.Count}");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void SerializeList()
        {
            try
            {
                Console.Write("\nEnter filename to save: ");
                string fileName = Console.ReadLine();
                list.Serialize(fileName);
                ShowSuccess("List serialized successfully!");
            }
            catch (Exception ex)
            {
                ShowError($"Serialization error: {ex.Message}");
            }
        }

        private void DeserializeList()
        {
            try
            {
                Console.Write("\nEnter filename to load: ");
                string fileName = Console.ReadLine();
                list.Deserialize(fileName);
                ShowSuccess("List deserialized successfully!");
                DisplayList();
            }
            catch (Exception ex)
            {
                ShowError($"Deserialization error: {ex.Message}");
            }
        }

        private void ShowSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        private void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}