using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using clock_allocation.Models;
using clock_allocation.Views;
using DynamicData;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;

namespace clock_allocation.ViewModels;

public class CertainCurriculumViewModel : ViewModelBase
{
    private List<AcademicPlanRecord> _academicPlanRecords;
    private Grid _curriculum;

    private Grid frozenGrid;
    private Grid scrollableGrid;

    private string _message;
    private bool _isLoading;

    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    public string Message
    {
        get => _message;
        set => this.RaiseAndSetIfChanged(ref _message, value);
    }


    private int countOfSemesters;
    private int _idCurriculum;

    private Grid FrozenGrid
    {
        get => frozenGrid;
        set => this.RaiseAndSetIfChanged(ref frozenGrid, value);
    }

    private Grid ScrollableGrid
    {
        get => scrollableGrid;
        set => this.RaiseAndSetIfChanged(ref scrollableGrid, value);
    }

    public int IdCurriculum
    {
        get => _idCurriculum;
        set => this.RaiseAndSetIfChanged(ref _idCurriculum, value);
    }

    private List<AcademicPlanRecord> AcademicPlanRecords
    {
        get => _academicPlanRecords;
        set => this.RaiseAndSetIfChanged(ref _academicPlanRecords, value);
    }

    public Grid Curriculum
    {
        get => _curriculum;
        set => this.RaiseAndSetIfChanged(ref _curriculum, value);
    }


    public CertainCurriculumViewModel(Curriculum curriculum)
    {
        if (curriculum == null) return;
        IdCurriculum = curriculum.IdCurriculum;
        ScrollableGrid = new Grid();
        FrozenGrid = new Grid();
        AcademicPlanRecords = GetAcademicPlanAsync(curriculum.Year, curriculum.IdSpecialization);
        countOfSemesters = curriculum.IdSpecializationNavigation.DurationOfTraining switch
        {
            "3 года 10 месяцев" => 8,
            "2 года 10 месяцев" => 6,
            "1 год 10 месяцев" => 4,
            _ => countOfSemesters
        };
        CreateComplexHeader();
    }

    public void DeleteCurriculum()
    {
        var curriculum = db.Curricula.Include(x => x.Modules).ThenInclude(x => x.Subjects).ThenInclude(x => x.Semesters)
            .ThenInclude(x => x.IdFormOfIntermediateCertificationNavigation)
            .FirstOrDefault(x => x.IdCurriculum == _idCurriculum);
        foreach (var module in curriculum.Modules)
        {
            foreach (var subject in module.Subjects)
            {
                foreach (var semester in subject.Semesters)
                {
                    db.Semesters.Remove(semester);
                }

                db.Subjects.Remove(subject);
            }

            db.Modules.Remove(module);
        }

        db.Curricula.Remove(curriculum);
        db.SaveChanges();
        MainViewModel.Instance.UserControl = new CurriculumView();
    }

    public void AskDeleteCurriculum()
    {
        IsLoading = true;
        Message = "Вы уверены что хотите удалить учебный план?";
    }

    public void CancelDeleteCommand()
    {
        IsLoading = false;
    }

    public void UpdateCurriculum()
    {
        var curriculumToEdit = db.Curricula
            .Include(c => c.Modules).ThenInclude(x => x.Subjects).ThenInclude(x => x.Semesters)
            .ThenInclude(x => x.IdFormOfIntermediateCertificationNavigation)
            .Include(curriculum => curriculum.IdSpecializationNavigation)
            .FirstOrDefault(c => c.IdCurriculum == IdCurriculum);

        MainViewModel.Instance.UserControl =
            new AddCurriculumView(curriculumToEdit.IdSpecializationNavigation, curriculumToEdit);
    }

    private static string GetRoman(int number) => number switch
    {
        1 => "I",
        2 => "II",
        3 => "III",
        4 => "IV",
        _ => number.ToString()
    };

    private void CreateComplexHeader()
    {
        var headers = new List<HeaderCellInfo>();
        var column = countOfSemesters switch
        {
            8 => 68,
            6 => 54,
            4 => 25,
            _ => countOfSemesters
        };
        var startColumn = countOfSemesters switch
        {
            8 => 10,
            6 => 8,
            4 => 6,
            _ => countOfSemesters
        };
        headers.AddRange([
            new HeaderCellInfo
            {
                Text = "Форма промежуточной аттестации", Column = 2, Row = 0, ColumnSpan = countOfSemesters,
                RowSpan = 5,
                IsVertical = true
            },
            new HeaderCellInfo
            {
                Text = "Учебная нагрузка обучающихся, час", Column = countOfSemesters + 2, Row = 0, ColumnSpan = 8,
                RowSpan = 1
            },
            new HeaderCellInfo
            {
                Text = "Всего объем образовательной нагрузки", Column = countOfSemesters + 2, Row = 1, RowSpan = 5,
                IsVertical = true
            },
            new HeaderCellInfo
            {
                Text = "Промежуточная аттестация", Column = countOfSemesters + 3, Row = 1, RowSpan = 5,
                IsVertical = true
            },
            new HeaderCellInfo
            {
                Text = "Самостоятельная учебная работа", Column = countOfSemesters + 4, Row = 1, RowSpan = 5,
                IsVertical = true
            },
            new HeaderCellInfo
            {
                Text = "Работа обучающихся во взаимодействии с преподавателями", Column = countOfSemesters + 5, Row = 1,
                ColumnSpan = 5
            },
            new HeaderCellInfo
                { Text = "занятия по дисциплинам и МДК", Column = countOfSemesters + 5, Row = 2, ColumnSpan = 4 },
            new HeaderCellInfo { Text = "в том числе", Column = countOfSemesters + 6, Row = 3, ColumnSpan = 3 },
            new HeaderCellInfo
                { Text = "всего занятий", Column = countOfSemesters + 5, Row = 3, RowSpan = 3, IsVertical = true },
            new HeaderCellInfo
                { Text = "уроков", Column = countOfSemesters + 6, Row = 4, RowSpan = 2, IsVertical = true },
            new HeaderCellInfo
            {
                Text = "лабораторных и практических занятий", Column = countOfSemesters + 7, Row = 4, RowSpan = 2,
                IsVertical = true
            },
            new HeaderCellInfo
            {
                Text = "курсовое проектирование", Column = countOfSemesters + 8, Row = 4, RowSpan = 2, IsVertical = true
            },
            new HeaderCellInfo
            {
                Text = "учебная и производственная практика", Column = countOfSemesters + 9, Row = 2, RowSpan = 4,
                IsVertical = true
            }
        ]);

        for (int i = 0; i < countOfSemesters; i++)
        {
            headers.Add(new HeaderCellInfo { Text = (i + 1).ToString(), Column = 2 + i, Row = 5 });
        }

        startColumn = countOfSemesters switch
        {
            8 => 18,
            6 => 16,
            4 => 14,
            _ => countOfSemesters
        };
        int sem = 1;
        int course = 1;

        while (sem <= countOfSemesters)
        {
            int semsInCourse = 2;
            int courseColumnSpan = semsInCourse * 6;

            headers.Add(new HeaderCellInfo
            {
                Text = $"{GetRoman(course)} курс",
                Column = startColumn,
                Row = 1,
                ColumnSpan = courseColumnSpan,
                IsVertical = false
            });

            for (int i = 0; i < semsInCourse; i++)
            {
                headers.Add(new HeaderCellInfo
                    { Text = $"{sem} сем.", Column = startColumn, Row = 2, RowSpan = 4, IsVertical = false });
                headers.Add(new HeaderCellInfo
                    { Text = "в том числе", Column = startColumn + 1, Row = 2, ColumnSpan = 5, IsVertical = false });
                headers.Add(new HeaderCellInfo
                {
                    Text = "промежуточная аттестация", Column = startColumn + 1, Row = 3, RowSpan = 3, IsVertical = true
                });
                headers.Add(new HeaderCellInfo
                    { Text = "учебные занятия", Column = startColumn + 2, Row = 3, RowSpan = 3, IsVertical = true });
                headers.Add(new HeaderCellInfo
                    { Text = "консультации", Column = startColumn + 3, Row = 3, RowSpan = 3, IsVertical = true });
                headers.Add(new HeaderCellInfo
                    { Text = "практика", Column = startColumn + 4, Row = 3, RowSpan = 3, IsVertical = true });
                headers.Add(new HeaderCellInfo
                {
                    Text = "самостоятельная работа", Column = startColumn + 5, Row = 3, RowSpan = 3, IsVertical = true
                });

                startColumn += 6;
                sem++;
            }

            course++;
        }

        ScrollableGrid = new Grid
        {
            Background = Brushes.White,
            RowDefinitions =
            {
                new RowDefinition(GridLength.Parse("60")),
                new RowDefinition(GridLength.Parse("60")),
                new RowDefinition(GridLength.Parse("60")),
                new RowDefinition(GridLength.Parse("60")),
                new RowDefinition(GridLength.Parse("60")),
                new RowDefinition(GridLength.Parse("50")),
            },
        };
        FrozenGrid = new Grid
        {
            Background = Brushes.White,
            RowDefinitions =
            {
                new RowDefinition(GridLength.Parse("60")),
                new RowDefinition(GridLength.Parse("60")),
                new RowDefinition(GridLength.Parse("60")),
                new RowDefinition(GridLength.Parse("60")),
                new RowDefinition(GridLength.Parse("60")),
                new RowDefinition(GridLength.Parse("50")),
            },
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star),
            }
        };
        for (var i = 0; i < column - 2; i++)
        {
            ScrollableGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        }

        FrozenGrid.Children.Add(CreateVerticalHeaderCell("Индекс", 0, 0, 1, 6));
        FrozenGrid.Children.Add(CreateHeaderCell("Наименование циклов, дисциплин...", 1, 0, 1, 6));
        foreach (var header in headers)
        {
            ScrollableGrid.Children.Add(header.IsVertical
                ? CreateVerticalHeaderCell(header.Text, header.Column, header.Row, header.ColumnSpan, header.RowSpan)
                : CreateHeaderCell(header.Text, header.Column, header.Row, header.ColumnSpan, header.RowSpan));
        }

        Curriculum = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Auto), // Для FrozenGrid
                new ColumnDefinition(GridLength.Star) // Для ScrollViewer
            },
            RowDefinitions = { new RowDefinition(GridLength.Star) }
        };
        var scrollViewer = new ScrollViewer
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Content = ScrollableGrid
        };

        // Добавляем элементы в MainGrid
        Grid.SetRow(FrozenGrid, 0);
        Grid.SetColumn(FrozenGrid, 0);
        Curriculum.Children.Add(FrozenGrid);

        Grid.SetRow(scrollViewer, 0);
        Grid.SetColumn(scrollViewer, 1);
        Curriculum.Children.Add(scrollViewer);
        var groupedByModule = AcademicPlanRecords
            .GroupBy(r => new { r.ModuleCode, r.ModuleTitle })
            .ToList();

        var currentRow = 6;

        foreach (var moduleGroup in groupedByModule)
        {
            var moduleSummary = new AcademicPlanRecord
            {
                ModuleCode = moduleGroup.Key.ModuleCode,
                ModuleTitle = moduleGroup.Key.ModuleTitle,

                TotalHours = moduleGroup.Sum(r => r.TotalHours),
                TotalIntermediateCertification = moduleGroup.Sum(r => r.TotalIntermediateCertification),
                TotalIndependentWork = moduleGroup.Sum(r => r.TotalIndependentWork),
                Classes = moduleGroup.Sum(r => r.Classes),
                Lessons = moduleGroup.Sum(r => r.Lessons),
                Practice = moduleGroup.Sum(r => r.Practice),
                CourseDesign = moduleGroup.Sum(r => r.CourseDesign),
                EducationalAndIndustrialPractice = moduleGroup.Sum(r => r.EducationalAndIndustrialPractice),

                Semester1TotalHours = (moduleGroup.Sum(r => r.Semester1TotalHours) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester1TotalHours),
                Semester1IntermediateCertification = (moduleGroup.Sum(r => r.Semester1IntermediateCertification) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester1IntermediateCertification),
                Semester1Lessons = (moduleGroup.Sum(r => r.Semester1Lessons) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester1Lessons),
                Semester1Consultations = (moduleGroup.Sum(r => r.Semester1Consultations) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester1Consultations),
                Semester1Practice = (moduleGroup.Sum(r => r.Semester1Practice) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester1Practice),
                Semester1IndependentWork = (moduleGroup.Sum(r => r.Semester1IndependentWork) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester1IndependentWork),

                Semester2TotalHours = (moduleGroup.Sum(r => r.Semester2TotalHours) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester2TotalHours),
                Semester2IntermediateCertification = (moduleGroup.Sum(r => r.Semester2IntermediateCertification) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester2IntermediateCertification),
                Semester2Lessons = (moduleGroup.Sum(r => r.Semester2Lessons) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester2Lessons),
                Semester2Consultations = (moduleGroup.Sum(r => r.Semester2Consultations) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester2Consultations),
                Semester2Practice = (moduleGroup.Sum(r => r.Semester2Practice) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester2Practice),
                Semester2IndependentWork = (moduleGroup.Sum(r => r.Semester2IndependentWork) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester2IndependentWork),

                Semester3TotalHours = (moduleGroup.Sum(r => r.Semester3TotalHours) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester3TotalHours),
                Semester3IntermediateCertification = (moduleGroup.Sum(r => r.Semester3IntermediateCertification) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester3IntermediateCertification),
                Semester3Lessons = (moduleGroup.Sum(r => r.Semester3Lessons) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester3Lessons),
                Semester3Consultations = (moduleGroup.Sum(r => r.Semester3Consultations) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester3Consultations),
                Semester3Practice = (moduleGroup.Sum(r => r.Semester3Practice) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester3Practice),
                Semester3IndependentWork = (moduleGroup.Sum(r => r.Semester3IndependentWork) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester3IndependentWork),

                Semester4TotalHours = (moduleGroup.Sum(r => r.Semester4TotalHours) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester4TotalHours),
                Semester4IntermediateCertification = (moduleGroup.Sum(r => r.Semester4IntermediateCertification) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester4IntermediateCertification),
                Semester4Lessons = (moduleGroup.Sum(r => r.Semester4Lessons) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester4Lessons),
                Semester4Consultations = (moduleGroup.Sum(r => r.Semester4Consultations) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester4Consultations),
                Semester4Practice = (moduleGroup.Sum(r => r.Semester4Practice) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester4Practice),
                Semester4IndependentWork = (moduleGroup.Sum(r => r.Semester4IndependentWork) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester4IndependentWork),

                Semester5TotalHours = (moduleGroup.Sum(r => r.Semester5TotalHours) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester5TotalHours),
                Semester5IntermediateCertification = (moduleGroup.Sum(r => r.Semester5IntermediateCertification) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester5IntermediateCertification),
                Semester5Lessons = (moduleGroup.Sum(r => r.Semester5Lessons) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester5Lessons),
                Semester5Consultations = (moduleGroup.Sum(r => r.Semester5Consultations) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester5Consultations),
                Semester5Practice = (moduleGroup.Sum(r => r.Semester5Practice) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester5Practice),
                Semester5IndependentWork = (moduleGroup.Sum(r => r.Semester5IndependentWork) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester5IndependentWork),

                Semester6TotalHours = (moduleGroup.Sum(r => r.Semester6TotalHours) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester6TotalHours),
                Semester6IntermediateCertification = (moduleGroup.Sum(r => r.Semester6IntermediateCertification) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester6IntermediateCertification),
                Semester6Lessons = (moduleGroup.Sum(r => r.Semester6Lessons) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester6Lessons),
                Semester6Consultations = (moduleGroup.Sum(r => r.Semester6Consultations) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester6Consultations),
                Semester6Practice = (moduleGroup.Sum(r => r.Semester6Practice) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester6Practice),
                Semester6IndependentWork = (moduleGroup.Sum(r => r.Semester6IndependentWork) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester6IndependentWork),

                Semester7TotalHours = (moduleGroup.Sum(r => r.Semester7TotalHours) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester7TotalHours),
                Semester7IntermediateCertification = (moduleGroup.Sum(r => r.Semester7IntermediateCertification) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester7IntermediateCertification),
                Semester7Lessons = (moduleGroup.Sum(r => r.Semester7Lessons) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester7Lessons),
                Semester7Consultations = (moduleGroup.Sum(r => r.Semester7Consultations) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester7Consultations),
                Semester7Practice = (moduleGroup.Sum(r => r.Semester7Practice) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester7Practice),
                Semester7IndependentWork = (moduleGroup.Sum(r => r.Semester7IndependentWork) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester7IndependentWork),

                Semester8TotalHours = (moduleGroup.Sum(r => r.Semester8TotalHours) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester8TotalHours),
                Semester8IntermediateCertification = (moduleGroup.Sum(r => r.Semester8IntermediateCertification) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester8IntermediateCertification),
                Semester8Lessons = (moduleGroup.Sum(r => r.Semester8Lessons) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester8Lessons),
                Semester8Consultations = (moduleGroup.Sum(r => r.Semester8Consultations) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester8Consultations),
                Semester8Practice = (moduleGroup.Sum(r => r.Semester8Practice) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester8Practice),
                Semester8IndependentWork = (moduleGroup.Sum(r => r.Semester8IndependentWork) == 0)
                    ? (int?)null
                    : moduleGroup.Sum(r => r.Semester8IndependentWork),
            };
            var sortedSubjects = moduleGroup
                .Where(r => r.SubjectCode != null && r.SubjectName != null)
                .OrderBy(r => int.TryParse(r.SubjectCode, out var code) ? code : int.MaxValue)
                .ToList();

            AddRowToGrid(moduleSummary, currentRow);
            FrozenGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
            ScrollableGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
            currentRow++;
            foreach (var subject in sortedSubjects)
            {
                FrozenGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
                ScrollableGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
                var props = typeof(AcademicPlanRecord).GetProperties().ToList();
                var propertiesToRemove = new Dictionary<int, List<string>>
                {
                    {
                        6, new List<string>
                        {
                            "Semester7Assessment", "Semester8Assessment",
                            "Semester7TotalHours", "Semester7IntermediateCertification",
                            "Semester7Lessons", "Semester7Consultations",
                            "Semester7Practice", "Semester7IndependentWork",
                            "Semester8TotalHours", "Semester8IntermediateCertification",
                            "Semester8Lessons", "Semester8Consultations",
                            "Semester8Practice", "Semester8IndependentWork"
                        }
                    },
                    {
                        4, new List<string>
                        {
                            "Semester5Assessment", "Semester6Assessment",
                            "Semester5TotalHours", "Semester5IntermediateCertification",
                            "Semester5Lessons", "Semester5Consultations",
                            "Semester5Practice", "Semester5IndependentWork",
                            "Semester6TotalHours", "Semester6IntermediateCertification",
                            "Semester6Lessons", "Semester6Consultations",
                            "Semester6Practice", "Semester6IndependentWork",
                            "Semester7Assessment", "Semester8Assessment",
                            "Semester7TotalHours", "Semester7IntermediateCertification",
                            "Semester7Lessons", "Semester7Consultations",
                            "Semester7Practice", "Semester7IndependentWork",
                            "Semester8TotalHours", "Semester8IntermediateCertification",
                            "Semester8Lessons", "Semester8Consultations",
                            "Semester8Practice", "Semester8IndependentWork"
                        }
                    }
                };

                if (propertiesToRemove.TryGetValue(countOfSemesters, out var properties))
                {
                    props.RemoveAll(p => properties.Contains(p.Name));
                }

                for (var col = 2; col < props.Count; col++)
                {
                    var value = props[col].GetValue(subject)?.ToString() ?? "";
                    if (value == "РК") value = "";

                    var cell = CreateCell(value, col - 2, currentRow, false);
                    if (col - 2 < 2)
                    {
                        FrozenGrid.Children.Add(cell);
                    }
                    else
                    {
                        ScrollableGrid.Children.Add(cell);
                    }
                }

                currentRow++;
            }
        }
    }

    private void AddRowToGrid(AcademicPlanRecord record, int row)
    {
        var props = typeof(AcademicPlanRecord).GetProperties().ToList();
        var propertiesToRemove = new Dictionary<int, List<string>>
        {
            {
                6, new List<string>
                {
                    "Semester7Assessment", "Semester8Assessment",
                    "Semester7TotalHours", "Semester7IntermediateCertification",
                    "Semester7Lessons", "Semester7Consultations",
                    "Semester7Practice", "Semester7IndependentWork",
                    "Semester8TotalHours", "Semester8IntermediateCertification",
                    "Semester8Lessons", "Semester8Consultations",
                    "Semester8Practice", "Semester8IndependentWork"
                }
            },
            {
                4, new List<string>
                {
                    "Semester5Assessment", "Semester6Assessment",
                    "Semester5TotalHours", "Semester5IntermediateCertification",
                    "Semester5Lessons", "Semester5Consultations",
                    "Semester5Practice", "Semester5IndependentWork",
                    "Semester6TotalHours", "Semester6IntermediateCertification",
                    "Semester6Lessons", "Semester6Consultations",
                    "Semester6Practice", "Semester6IndependentWork",
                    "Semester7Assessment", "Semester8Assessment",
                    "Semester7TotalHours", "Semester7IntermediateCertification",
                    "Semester7Lessons", "Semester7Consultations",
                    "Semester7Practice", "Semester7IndependentWork",
                    "Semester8TotalHours", "Semester8IntermediateCertification",
                    "Semester8Lessons", "Semester8Consultations",
                    "Semester8Practice", "Semester8IndependentWork"
                }
            }
        };

        if (propertiesToRemove.TryGetValue(countOfSemesters, out var properties))
        {
            props.RemoveAll(p => properties.Contains(p.Name));
        }

        var currentCol = 0;

        foreach (var t in props)
        {
            if (t.Name is "SubjectName" or "SubjectCode") continue;
            var value = t.GetValue(record)?.ToString() ?? "";
            var cell = CreateCell(value, currentCol, row, true);
            if (currentCol < 2)
            {
                FrozenGrid.Children.Add(cell);
            }
            else
            {
                ScrollableGrid.Children.Add(cell);
            }

            currentCol++;
        }
    }

    private Border CreateHeaderCell(string text, int column, int row, int columnSpan = 1, int rowSpan = 1)
    {
        var border = new Border
        {
            BorderBrush = Brushes.White,
            BorderThickness = new Thickness(0.5),
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Background = Brush.Parse("#B44A45"),
            Child = new TextBlock
            {
                Text = text,
                FontWeight = FontWeight.Bold,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                Foreground = Brushes.White,
                Margin = new Thickness(5)
            }
        };

        Grid.SetColumn(border, column);
        Grid.SetRow(border, row);
        Grid.SetColumnSpan(border, columnSpan);
        Grid.SetRowSpan(border, rowSpan);

        return border;
    }

    private Border CreateCell(string text, int column, int row, bool isModule, int columnSpan = 1, int rowSpan = 1)
    {
        var background = isModule ? new SolidColorBrush(Color.Parse("#f8e1df")) : new SolidColorBrush(Colors.White);
        var border = new Border
        {
            BorderBrush = Brushes.LightGray,
            BorderThickness = new Thickness(0.5),
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Background = background,
            Child = new TextBlock
            {
                Text = text,
                FontWeight = FontWeight.Bold,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                Foreground = Brushes.Black,
                Margin = new Thickness(5)
            }
        };

        Grid.SetColumn(border, column);
        Grid.SetRow(border, row);
        Grid.SetColumnSpan(border, columnSpan);
        Grid.SetRowSpan(border, rowSpan);

        return border;
    }

    private Border CreateVerticalHeaderCell(string text, int column, int row, int columnSpan = 1, int rowSpan = 1)
    {
        var textBlock = new TextBlock
        {
            Text = text,
            FontWeight = FontWeight.Bold,
            FontSize = 12,
            TextWrapping = TextWrapping.Wrap,
            MaxWidth = 100,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            TextAlignment = TextAlignment.Center,
            Foreground = Brushes.White,
            Margin = new Thickness(5),
            RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative),
            RenderTransform = new RotateTransform(-90)
        };

        var border = new Border
        {
            BorderBrush = Brushes.White,
            BorderThickness = new Thickness(0.5),
            Background = Brush.Parse("#B44A45"),
            Child = textBlock,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };

        Grid.SetColumn(border, column);
        Grid.SetRow(border, row);
        Grid.SetColumnSpan(border, columnSpan);
        Grid.SetRowSpan(border, rowSpan);

        return border;
    }

    private List<AcademicPlanRecord> GetAcademicPlanAsync(
        int year, int specializationId)
    {
        var results = db.Set<AcademicPlanRecord>()
            .FromSqlRaw("SELECT * FROM get_academic_plan({0}, {1})", year, specializationId)
            .ToList();
        return results;
    }
}