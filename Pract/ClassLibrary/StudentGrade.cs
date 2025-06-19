using System;
using System.Xml.Serialization;

namespace StudentGradesLibrary
{
    [Serializable]
    public class StudentGrade
    {
        public enum Subject { Mathematics, Physics, Chemistry, Biology, Literature }

        public Subject Course { get; set; }
        public double Grade { get; set; }

        [XmlIgnore]
        public bool IsPassed => Grade >= 60.0;

        public StudentGrade() { }

        public StudentGrade(Subject course, double grade)
        {
            Course = course;
            Grade = grade;
        }
    }
}