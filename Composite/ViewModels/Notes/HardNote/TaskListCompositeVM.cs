using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class TaskListCompositeVM : CompositeBaseVM, IDisposable
    {
        [ObservableProperty] string _text = string.Empty;
        [ObservableProperty] string _status = "Выполнено 0 %";
        [ObservableProperty] bool _isCompleted;

        public ObservableCollection<SubTaskCompositeVM> SubTasks { get; set; } = new();

        public TaskListCompositeVM() => Id = Guid.NewGuid();

        [RelayCommand] void AddSubTask()
        {
            SubTasks.Add(new SubTaskCompositeVM(CalculatingPercentTask));
            CalculatingPercentTask();
        }
        [RelayCommand] void DeleteSubTask(SubTaskCompositeVM subtask)
        {
            SubTasks.Remove(subtask);
            CalculatingPercentTask();
        }
        public void CalculatingPercentTask()
        {
            IsCompleted = false;

            if (SubTasks == null || SubTasks.Count == 0)
            {
                IsCompleted = false;
                Status = "Выполнено 0 %";
                return;
            }

            double completedSubTasks = 0.0;

            foreach (var subTask in SubTasks)
            {
                if (subTask.IsCompleted == true) completedSubTasks++;
            }

            double persent = completedSubTasks / SubTasks.Count * 100;

            if (persent == 0)
            {
                Status = "Выполнено 0 %";
                return;
            }
            else if (persent == 100)
            {
                Status = "Выполнено 100 %";
                IsCompleted = true;
                return;
            }

            Status = "Выполнено " + persent.ToString("F2") + " %";
        }
        [RelayCommand] void ExecuteAllSubTask()
        {
            if(IsCompleted == true)
            {
                Status = "Выполнено 100 %";
                foreach(var subTask in SubTasks) subTask.IsCompleted = true;
            }
            else
            {
                Status = "Выполнено 0 %";
                foreach (var subTask in SubTasks) subTask.IsCompleted = false;
            }
        }


        public override object Clone()
        {
            var taskList = new TaskListCompositeVM()
            {
                Tag = Tag,
                Comment = Comment,
                Text = Text,
                Status = Status,
                IsCompleted = IsCompleted
            };

            foreach (var subTask in SubTasks) taskList.SubTasks.Add((SubTaskCompositeVM)subTask.Clone());

            return taskList;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Text = string.Empty;
                Status = string.Empty;
                IsCompleted = false;
                Tag = string.Empty;
                Comment = string.Empty;
                foreach (var subTask in SubTasks) subTask.Dispose();
                SubTasks.Clear();
                SubTasks = null;
            }
            base.Dispose(disposing);
        }
    }
}