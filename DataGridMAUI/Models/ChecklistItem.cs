namespace DataGridMAUI
{
    public class ChecklistItem(string task, bool isCompleted = false)
    {
        public bool IsCompleted { get; set; } = isCompleted;
        public string Task { get; set; } = task;
    }
}