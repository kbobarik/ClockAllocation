using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace clock_allocation.Models
{
    public partial class Module : INotifyPropertyChanged
    {
        public int IdModule { get; set; }

        private string? _code;

        public string? Code
        {
            get => _code;
            set
            {
                if (_code != value)
                {
                    _code = value;
                    OnPropertyChanged(nameof(Code));
                }
            }
        }

        private string? _title;

        public string? Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public int? IdHeadModule { get; set; }

        public int IdCurriculum { get; set; }

        public virtual Curriculum IdCurriculumNavigation { get; set; } = null!;

        public virtual Module? IdHeadModuleNavigation { get; set; }

        private ObservableCollection<Module> _inverseIdHeadModuleNavigation;

        public virtual ObservableCollection<Module> InverseIdHeadModuleNavigation
        {
            get => _inverseIdHeadModuleNavigation;
            set
            {
                _inverseIdHeadModuleNavigation = value;
                OnPropertyChanged(nameof(Subjects));
            }
        }

        private ObservableCollection<Subject> _subjects = new ObservableCollection<Subject>();

        public virtual ObservableCollection<Subject> Subjects
        {
            get => _subjects;
            set
            {
                _subjects = value;
                OnPropertyChanged(nameof(Subjects));
            }
        }



        [NotMapped] private int _currentRow;

        [NotMapped]
        public int CurrentRow
        {
            get => _currentRow;
            set
            {
                if (_currentRow == value) return;
                _currentRow = value;
                OnPropertyChanged(nameof(CurrentRow));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return Title;
        }
    }
}