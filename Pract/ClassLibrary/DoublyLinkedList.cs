using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace StudentGradesLibrary
{
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
                throw new IndexOutOfRangeException("Index is out of range");

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
                    throw new IndexOutOfRangeException("Index is out of range");
                return GetNodeAt(index).Data;
            }
            set
            {
                if (index < 0 || index >= count)
                    throw new IndexOutOfRangeException("Index is out of range");
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

       // public ListNode GetFirstNode() => head;
      //  public ListNode GetNextNode(ListNode node) => node?.Next;

        public (double min, double max) GetMinMaxGrades()
        {
            if (count == 0)
                throw new InvalidOperationException("The list is empty");

            double min = double.MaxValue;
            double max = double.MinValue;

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
                if (current.Data.Course == StudentGrade.Subject.Mathematics &&
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
}