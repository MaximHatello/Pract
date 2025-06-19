
namespace StudentGradesLibrary
{
    internal class ListNode
    {
        public StudentGrade Data { get; set; }
        public ListNode Previous { get; set; }
        public ListNode Next { get; set; }

        public ListNode(StudentGrade data)
        {
            Data = data;
        }
    }
}
Console.OutputEncoding = System.Text.Encoding.UTF8;