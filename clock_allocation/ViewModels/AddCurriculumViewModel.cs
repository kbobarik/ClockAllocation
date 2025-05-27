using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using clock_allocation.Models;
using clock_allocation.Resources;
using clock_allocation.Views;
using DynamicData;
using Npgsql;
using ReactiveUI;

namespace clock_allocation.ViewModels;

public class AddCurriculumViewModel : ViewModelBase
{
    private List<AcademicPlanRecord> _academicPlanRecords;
    private Grid mainGrid;
    private Grid frozenGrid;
    private Grid scrollableGrid ;
    
    private Curriculum selectedCurriculum;
    private Specialization currentSpecialization;
    private int _currentRow = 6;
    private string year;
    private int countOfSemesters;
    private List<FormOfIntermediateCertification> _certifications;
    private bool isLoading;
    private string message;
    private string title;

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
    public string Title
    {
        get => title;
        set => this.RaiseAndSetIfChanged(ref title, value);
    }

    public Curriculum SelectedCurriculum
    {
        get => selectedCurriculum;
        set => this.RaiseAndSetIfChanged(ref selectedCurriculum, value);
    }

    public string Message
    {
        get => message;
        set => this.RaiseAndSetIfChanged(ref message, value);
    }

    public bool IsLoading
    {
        get => isLoading;
        set => this.RaiseAndSetIfChanged(ref isLoading, value);
    }

    public List<FormOfIntermediateCertification> Certifications
    {
        get => _certifications;
        set => this.RaiseAndSetIfChanged(ref _certifications, value);
    }

    public string Year
    {
        get => year;
        set => this.RaiseAndSetIfChanged(ref year, value);
    }

    public Grid MainGrid
    {
        get => mainGrid;
        set => this.RaiseAndSetIfChanged(ref mainGrid, value);
    }

    private Specialization CurrentSpecialization
    {
        get => currentSpecialization;
        set => this.RaiseAndSetIfChanged(ref currentSpecialization, value);
    }

    private List<AcademicPlanRecord> AcademicPlanRecords
    {
        get => _academicPlanRecords;
        set => this.RaiseAndSetIfChanged(ref _academicPlanRecords, value);
    }
    public AddCurriculumViewModel(Specialization specialization, Curriculum existingCurriculum)
    {
        SelectedCurriculum = existingCurriculum;
        Year = existingCurriculum.Year.ToString();
        CurrentSpecialization = specialization;
        Title = "Редактирование учебный план";
        Certifications = new List<FormOfIntermediateCertification>();
        MainGrid = new Grid();
        ScrollableGrid = new Grid();
        FrozenGrid = new Grid();
        LoadCertifications();

        countOfSemesters = CurrentSpecialization.DurationOfTraining switch
        {
            "3 года 10 месяцев" => 8,
            "2 года 10 месяцев" => 6,
            "1 год 10 месяцев" => 4,
            _ => 0
        };

        LoadExistingCurriculum();
        IsLoading = false;
        Message = string.Empty;
    }

    private async void LoadCertifications()
    {
        try
        {
            await using var localDb = new ClockAllocationContext();
        
            var certifications = await localDb.FormOfIntermediateCertifications
                .AsNoTracking() 
                .ToListAsync();

            certifications.Add(new FormOfIntermediateCertification()
            {
                Title = "",
                IdFormOfIntermediateCertification = 0
            });

            Certifications = certifications;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ошибка загрузки форм аттестации: {ex.Message}");
        }
    }

    private async void LoadExistingCurriculum()
    {
        try
        {
            IsLoading = true;
            Message = "Загрузка учебного плана...";

            // Полная очистка текущих данных
             MainGrid.Children.Clear();
            MainGrid = new Grid();
            _currentRow = 6; 

            var loadedCurriculum = await db.Curricula
                .Include(x => x.IdSpecializationNavigation)
                .Include(c => c.Modules)
                .ThenInclude(m => m.Subjects)
                .ThenInclude(s => s.Semesters)
                .Include(c => c.Modules)
                .ThenInclude(m => m.InverseIdHeadModuleNavigation)
                .ThenInclude(m => m.Subjects)
                .ThenInclude(s => s.Semesters)
                .ThenInclude(x => x.IdFormOfIntermediateCertificationNavigation)
                .FirstOrDefaultAsync(c => c.IdCurriculum == SelectedCurriculum.IdCurriculum);

            if (loadedCurriculum != null)
            {
                Year = loadedCurriculum.Year.ToString();
                CurrentSpecialization = loadedCurriculum.IdSpecializationNavigation;

                // Воссоздаем основную структуру Grid
                CreateComplexHeader();

                // Копируем модули с правильным расчетом строк
                SelectedCurriculum.Modules = CopyModules(loadedCurriculum.Modules.Where(m => m.IdHeadModule == null).ToList());

                _currentRow = 6;
                // Добавляем модули в Grid
                foreach (var module in  SelectedCurriculum.Modules)
                {
                    _currentRow = AddModuleWithControls(module, _currentRow);
                }
            }
        }
        catch (Exception ex)
        {
            Message = $"Ошибка загрузки: {ex.Message}";
            Debug.WriteLine(ex);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private List<Module> CopyModules(List<Module> sourceModules)
    {
        var result = new List<Module>();
        int currentBaseRow = _currentRow;

        foreach (var module in sourceModules)
        {
            var newModule = new Module
            {
                IdModule = module.IdModule,
                IdCurriculum = module.IdCurriculum,
                Code = module.Code,
                Title = module.Title,
                CurrentRow = currentBaseRow,
                Subjects = new ObservableCollection<Subject>()
            };

            // Копируем предметы
            foreach (var subject in module.Subjects)
            {
                var newSubject = new Subject
                {
                    IdSubject = subject.IdSubject,
                    Code = subject.Code,
                    Title = subject.Title,
                    Lessons = subject.Lessons,
                    LaboratoryAndPracticalExercises = subject.LaboratoryAndPracticalExercises,
                    CourseDesign = subject.CourseDesign,
                    Semesters = new ObservableCollection<Semester>()
                };

                foreach (var semester in subject.Semesters)
                {
                    var certification = Certifications.FirstOrDefault(
                        c => c.IdFormOfIntermediateCertification == semester.IdFormOfIntermediateCertification
                    ) ?? Certifications.FirstOrDefault(c => c.IdFormOfIntermediateCertification == 4);
                    
                    newSubject.Semesters.Add(new Semester
                    {
                        IdSemester = semester.IdSemester,
                        Number = semester.Number,
                        IdFormOfIntermediateCertification = certification.IdFormOfIntermediateCertification,
                        IdFormOfIntermediateCertificationNavigation = certification,
                        Lessons = semester.Lessons,
                        Consultations = semester.Consultations,
                        Practice = semester.Practice,
                        IndependentWork = semester.IndependentWork,
                        IntermediateCertification = semester.IntermediateCertification
                    });
                }

                newModule.Subjects.Add(newSubject);
            }

            // Копируем вложенные модули с правильным смещением строк
            if (module.InverseIdHeadModuleNavigation != null && module.InverseIdHeadModuleNavigation.Any())
            {
                int nestedRow = currentBaseRow + 2 + newModule.Subjects.Count;
                newModule.InverseIdHeadModuleNavigation = new ObservableCollection<Module>();

                foreach (var nested in module.InverseIdHeadModuleNavigation)
                {
                    var nestedCopy = CopyModuleWithRowOffset(nested, nestedRow);
                    newModule.InverseIdHeadModuleNavigation.Add(nestedCopy);
                    nestedRow = GetLastRowOfModule(nestedCopy) + 1;
                }
            }

            result.Add(newModule);
            currentBaseRow = GetLastRowOfModule(newModule) + 1;
        }

        _currentRow = currentBaseRow;
        return result;
    }

    private Module CopyModuleWithRowOffset(Module source, int baseRow)
    {
        var copy = new Module
        {
            IdModule = source.IdModule,
            IdCurriculum = source.IdCurriculum,
            Code = source.Code,
            Title = source.Title,
            CurrentRow = baseRow,
            Subjects = new ObservableCollection<Subject>()
        };

        foreach (var subject in source.Subjects)
        {
            var newSubject = new Subject
            {
                IdSubject = subject.IdSubject,
                Code = subject.Code,
                Title = subject.Title,
                Lessons = subject.Lessons,
                LaboratoryAndPracticalExercises = subject.LaboratoryAndPracticalExercises,
                CourseDesign = subject.CourseDesign,
                Semesters = new ObservableCollection<Semester>()
            };

            foreach (var semester in subject.Semesters)
            {
                var certification = Certifications.FirstOrDefault(
                    c => c.IdFormOfIntermediateCertification == semester.IdFormOfIntermediateCertification
                ) ?? Certifications.FirstOrDefault(c => c.IdFormOfIntermediateCertification == 4);

                newSubject.Semesters.Add(new Semester
                {
                    IdSemester = semester.IdSemester,
                    Number = semester.Number,
                    IdFormOfIntermediateCertification = certification.IdFormOfIntermediateCertification,
                    IdFormOfIntermediateCertificationNavigation = certification,
                    Lessons = semester.Lessons,
                    Consultations = semester.Consultations,
                    Practice = semester.Practice,
                    IndependentWork = semester.IndependentWork,
                    IntermediateCertification = semester.IntermediateCertification
                });
            }

            copy.Subjects.Add(newSubject);
        }

        if (source.InverseIdHeadModuleNavigation != null && source.InverseIdHeadModuleNavigation.Any())
        {
            int nestedRow = baseRow + 2 + copy.Subjects.Count;
            copy.InverseIdHeadModuleNavigation = new ObservableCollection<Module>();

            foreach (var nested in source.InverseIdHeadModuleNavigation)
            {
                var nestedCopy = CopyModuleWithRowOffset(nested, nestedRow);
                copy.InverseIdHeadModuleNavigation.Add(nestedCopy);
                nestedRow = GetLastRowOfModule(nestedCopy) + 1;
            }
        }

        return copy;
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
        IsLoading = false;
        var headers = new List<HeaderCellInfo>();
        var column = countOfSemesters switch
        {
            8 => 68,
            6 => 54,
            4 => 25,
            _ => countOfSemesters
        };
        headers.AddRange([
            new HeaderCellInfo
            {
                Text = "Форма промежуточной аттестации", Column = 2, Row = 0, ColumnSpan = countOfSemesters, RowSpan = 5,
                IsVertical = true
            },
            new HeaderCellInfo
                { Text = "Учебная нагрузка обучающихся, час", Column = countOfSemesters+2, Row = 0, ColumnSpan = 8, RowSpan = 1 },
            new HeaderCellInfo
                { Text = "Всего объем образовательной нагрузки", Column = countOfSemesters+2, Row = 1, RowSpan = 5, IsVertical = true },
            new HeaderCellInfo
                { Text = "Промежуточная аттестация", Column =  countOfSemesters+3, Row = 1, RowSpan = 5, IsVertical = true },
            new HeaderCellInfo
                { Text = "Самостоятельная учебная работа", Column =  countOfSemesters+4, Row = 1, RowSpan = 5, IsVertical = true },
            new HeaderCellInfo
            {
                Text = "Работа обучающихся во взаимодействии с преподавателями", Column =  countOfSemesters+5, Row = 1, ColumnSpan = 5
            },
            new HeaderCellInfo { Text = "занятия по дисциплинам и МДК", Column =  countOfSemesters+5, Row = 2, ColumnSpan = 4 },
            new HeaderCellInfo { Text = "в том числе", Column =  countOfSemesters+6, Row = 3, ColumnSpan = 3 },
            new HeaderCellInfo { Text = "всего занятий", Column =  countOfSemesters+5, Row = 3, RowSpan = 3, IsVertical = true },
            new HeaderCellInfo { Text = "уроков", Column =  countOfSemesters+6, Row = 4, RowSpan = 2, IsVertical = true },
            new HeaderCellInfo
                { Text = "лабораторных и практических занятий", Column =  countOfSemesters+7, Row = 4, RowSpan = 2, IsVertical = true },
            new HeaderCellInfo
                { Text = "курсовое проектирование", Column =  countOfSemesters+8, Row = 4, RowSpan = 2, IsVertical = true },
            new HeaderCellInfo
                { Text = "учебная и производственная практика", Column =  countOfSemesters+9, Row = 2, RowSpan = 4, IsVertical = true }
        ]);

        for (int i = 0; i < countOfSemesters; i++)
        {
            headers.Add(new HeaderCellInfo { Text = (i+1).ToString(), Column = 2 + i, Row = 5 });
        }

        var startColumn = countOfSemesters switch
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
        for (var i = 0; i < column-2; i++)
        {
            ScrollableGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        }
        FrozenGrid.Children.Add(CreateVerticalHeaderCell("Индекс",0,0,1,6));
        FrozenGrid.Children.Add(CreateHeaderCell("Наименование циклов, дисциплин...",1,0,1,6));
        foreach (var header in headers)
        {
            ScrollableGrid.Children.Add(header.IsVertical
                ? CreateVerticalHeaderCell(header.Text, header.Column, header.Row, header.ColumnSpan, header.RowSpan)
                : CreateHeaderCell(header.Text, header.Column, header.Row, header.ColumnSpan, header.RowSpan));
        }
        MainGrid = new Grid
        {
            ColumnDefinitions = 
            {
                new ColumnDefinition(GridLength.Auto), // Для FrozenGrid
                new ColumnDefinition(GridLength.Star)   // Для ScrollViewer
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
        MainGrid.Children.Add(FrozenGrid);

        Grid.SetRow(scrollViewer, 0);
        Grid.SetColumn(scrollViewer, 1);
        MainGrid.Children.Add(scrollViewer);
    }

    private void CreateModuleHeader(Module module, int row)
    {
        var codeBox = CreateTextBox("Code", module, 0, row);
        var titleBox = CreateTextBox("Title", module, 1, row);
        FrozenGrid.Children.Add(codeBox);
        FrozenGrid.Children.Add(titleBox);

        var column = countOfSemesters + 2;
        var propertyNames = new List<string>
        {
            "TotalHours",
            "TotalIntermediateCertification",
            "TotalIndependentWork",
            "Classes",
            "Lessons",
            "Practice",
            "CourseDesign",
            "EducationalAndIndustrialPractice"
        };

        for (var i = 1; i <= countOfSemesters; i++)
        {
            propertyNames.Add($"Semester{i}TotalHours");
            propertyNames.Add($"Semester{i}IntermediateCertification");
            propertyNames.Add($"Semester{i}Lessons");
            propertyNames.Add($"Semester{i}Consultations");
            propertyNames.Add($"Semester{i}Practice");
            propertyNames.Add($"Semester{i}IndependentWork");
        }

        foreach (var textBox in propertyNames.Select(property => CreateDefaultTextBox(column, row)))
        {
            ScrollableGrid.Children.Add(textBox);
            column++;
        }
    }
    private Control CreateComboBox(string propertyName, object dataContext,
        List<FormOfIntermediateCertification> certifications, int column, int row)
    {
        var comboBox = new ComboBox
        {
            BorderBrush = new SolidColorBrush(Color.Parse("#B44A45")),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(4),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Padding = new Thickness(8, 4),
            Background = Brushes.White,
            Foreground = new SolidColorBrush(Color.Parse("#333333")),
            FontSize = 14,
            DataContext = dataContext,
            ItemsSource = certifications,
            ItemTemplate = new FuncDataTemplate<FormOfIntermediateCertification>((item, _) =>
                new TextBlock { Text = item.Title == "РК" ? "" : item.Title, FontSize = 14, Padding = new Thickness(4) })
        };

        // Improved binding setup
        comboBox.Bind(ComboBox.SelectedItemProperty, 
            new Binding(propertyName)
            {
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

        // Set initial value through binding rather than directly
        var semester = (Semester)dataContext;
        if (semester.IdFormOfIntermediateCertificationNavigation == null && semester.IdFormOfIntermediateCertification > 0)
        {
            semester.IdFormOfIntermediateCertificationNavigation = certifications
                .FirstOrDefault(c => c.IdFormOfIntermediateCertification == semester.IdFormOfIntermediateCertification);
        }

        Grid.SetColumn(comboBox, column);
        Grid.SetRow(comboBox, row);

        return comboBox;
    }

    private Control CreateTextBox(string propertyName, object dataContext, int column, int row)
    {
        var backgroundColor = dataContext is Module
            ? new SolidColorBrush(Color.Parse("#f8e1df"))
            : new SolidColorBrush(Colors.White);

        var tb = new TextBox
        {
            BorderBrush = new SolidColorBrush(Color.Parse("#B44A45")),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(4),
            Background = backgroundColor,
            Foreground = new SolidColorBrush(Color.Parse("#333333")),
            FontSize = 14,
            Padding = new Thickness(8, 4),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            IsReadOnly = true,
            DataContext = dataContext,
            Watermark = propertyName switch
            {
                "Code" => "Код",
                "Title" => "Название",
                _ => null
            }
        };

        tb.Bind(TextBox.TextProperty, new Binding(propertyName)
        {
            Source = dataContext,
            Mode =  BindingMode.OneWay
        });

        Grid.SetColumn(tb, column);
        Grid.SetRow(tb, row);

        return tb;
    }

    private Control CreateDefaultTextBox(int column, int row)
    {
        var backgroundColor = new SolidColorBrush(Color.Parse("#f8e1df"));

        var tb = new TextBox
        {
            Text = "-",
            BorderBrush = new SolidColorBrush(Color.Parse("#B44A45")),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(4),
            Background = backgroundColor,
            Foreground = new SolidColorBrush(Color.Parse("#333333")),
            FontSize = 14,
            Padding = new Thickness(8, 4),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            IsReadOnly = true,
        };

        Grid.SetColumn(tb, column);
        Grid.SetRow(tb, row);

        return tb;
    }

    private Control CreateTextBoxForDigit(string propertyName, object dataContext, int column, int row,
        bool isReadOnly = false)
    {
        var tb = new TextBox
        {
            BorderBrush = new SolidColorBrush(Color.Parse("#B44A45")),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(4),
            Background = isReadOnly ? new SolidColorBrush(Color.Parse("#f5f5f5")) : Brushes.White,
            Foreground = new SolidColorBrush(Color.Parse("#333333")),
            FontSize = 14,
            Padding = new Thickness(8, 4),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            IsReadOnly = isReadOnly,
            DataContext = dataContext,
            TextAlignment = TextAlignment.Right,
            Watermark = "0"
        };

        if (!isReadOnly)
        {
            tb.Classes.Add("HoverTextBox");
            tb.AddHandler(TextBox.TextInputEvent, (sender, e) =>
            {
                if (!e.Text.All(char.IsDigit))
                {
                    e.Handled = true;
                }
            }, RoutingStrategies.Tunnel);
        }

        tb.Bind(TextBox.TextProperty, new Binding(propertyName)
        {
            Source = dataContext,
            Mode = isReadOnly ? BindingMode.OneWay : BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            Converter = new IntConverter()
        });

        Grid.SetColumn(tb, column);
        Grid.SetRow(tb, row);

        return tb;
    }

    private Border CreateHeaderCell(string text, int column, int row, int columnSpan = 1, int rowSpan = 1)
    {
        var border = new Border
        {
            BorderBrush = new SolidColorBrush(Colors.White),
            BorderThickness = new Thickness(1),
            Background = new SolidColorBrush(Color.Parse("#B44A45")),
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Child = new TextBlock
            {
                Text = text,
                FontWeight = FontWeight.SemiBold,
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                Foreground = Brushes.White,
                Padding = new Thickness(8)
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
            FontWeight = FontWeight.SemiBold,
            FontSize = 14,
            TextWrapping = TextWrapping.Wrap,
            MaxWidth = 100,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            TextAlignment = TextAlignment.Center,
            Foreground = Brushes.White,
            Padding = new Thickness(8),
            RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative),
            RenderTransform = new RotateTransform(-90)
        };

        var border = new Border
        {
            BorderBrush = new SolidColorBrush(Colors.White),
            BorderThickness = new Thickness(1),
            Background = new SolidColorBrush(Color.Parse("#B44A45")),
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

    private int AddModuleWithControls(Module module, int startRow)
    {
        int row = startRow;
        ScrollableGrid.RowDefinitions.Insert(row, new RowDefinition(GridLength.Auto));
        FrozenGrid.RowDefinitions.Insert(row, new RowDefinition(GridLength.Auto));
        CreateModuleHeader(module, row);
        row++;

        foreach (var subject in module.Subjects)
        {
            ScrollableGrid.RowDefinitions.Insert(row, new RowDefinition(GridLength.Auto));
            FrozenGrid.RowDefinitions.Insert(row, new RowDefinition(GridLength.Auto));
            CreateExistingSubjectControlsForModules(module, subject, row);
            row++;
        }
        if (module.InverseIdHeadModuleNavigation != null)
        {
            foreach (var nested in module.InverseIdHeadModuleNavigation)
            {
                row = AddModuleWithControls(nested, row);
            }
        }

        return row;
    }

    private int GetLastRowOfModule(Module module)
    {
        if (module == null) return 0;

        int lastRow = module.CurrentRow; // Начинаем с текущей строки модуля

        lastRow++;

        lastRow += module.Subjects.Count;

        lastRow++;

        if (module.InverseIdHeadModuleNavigation != null && module.InverseIdHeadModuleNavigation.Count > 0)
        {
            foreach (var nested in module.InverseIdHeadModuleNavigation)
            {
                lastRow = Math.Max(lastRow, GetLastRowOfModule(nested));
            }
        }

        return lastRow;
    }

    private void CreateExistingSubjectControlsForModules(Module module, Subject subject, int row)
    {
        var codeBox = CreateTextBox("Code", subject, 0, row);
        var titleBox = CreateTextBox("Title", subject, 1, row);
        FrozenGrid.Children.Add(codeBox);
        FrozenGrid.Children.Add(titleBox);

        int column = 2;
        var semesters = subject.Semesters.OrderBy(x => x.Number).ToList();
        var lastSemester = semesters.OrderBy(x => x.Number)
            .LastOrDefault()?.Number ?? 0;
        for (var i = lastSemester; i <= countOfSemesters; i++)
        {
            semesters.Add(new Semester
            {
                Number = i,
                IdFormOfIntermediateCertificationNavigation =
                    Certifications.FirstOrDefault(x => x.IdFormOfIntermediateCertification == 0),
                IdFormOfIntermediateCertification = 0
            });
        }

        subject.Semesters = new ObservableCollection<Semester>(semesters);
        for (var i = 0; i < countOfSemesters; i++)
        {
            var combo = CreateComboBox(
                "IdFormOfIntermediateCertificationNavigation",
                semesters[i],
                Certifications,
                column: column,
                row: row
            );
            ScrollableGrid.Children.Add(combo);
            column++;
        }

        var propertyNames = new List<PropertyName>
        {
            new() { Name = "TotalHours", IsReadOnly = true },
            new() { Name = "TotalIntermediateCertification", IsReadOnly = true },
            new() { Name = "TotalIndependentWork", IsReadOnly = true },
            new() { Name = "Classes", IsReadOnly = true },
            new() { Name = "Lessons", IsReadOnly = false },
            new() { Name = "LaboratoryAndPracticalExercises", IsReadOnly = false },
            new() { Name = "CourseDesign", IsReadOnly = false },
            new() { Name = "EducationalAndIndustrialPractice", IsReadOnly = true },
        };

        foreach (var textBox in propertyNames.Select(item =>
                     CreateTextBoxForDigit(item.Name, subject, column, row, item.IsReadOnly)))
        {
            column++;
            ScrollableGrid.Children.Add(textBox);
        }

        var semesterPropertyNames = new List<PropertyName>
        {
            new() { Name = "TotalHours", IsReadOnly = true },
            new() { Name = "IntermediateCertification", IsReadOnly = false },
            new() { Name = "Lessons", IsReadOnly = false },
            new() { Name = "Consultations", IsReadOnly = false },
            new() { Name = "Practice", IsReadOnly = false },
            new() { Name = "IndependentWork", IsReadOnly = false }
        };

        foreach (var semester in subject.Semesters)
        {
            foreach (var textBox in semesterPropertyNames.Select(property =>
                         CreateTextBoxForDigit(property.Name, semester, column, row, property.IsReadOnly)))
            {
                column++;
                ScrollableGrid.Children.Add(textBox);
            }
        }
    }

    private bool ValidateCurriculum()
    {
        if (int.TryParse(Year, out int year) && year >= 2000 && year <= 2100) return true;
        Message = "Укажите корректный год (2000-2100)";
        return false;
    }

    public async Task Save()
    {
        try
        {
            if (!ValidateCurriculum())
            {
                IsLoading = true;
                await Task.Delay(3000);
                IsLoading = false;
                return;
            }
            IsLoading = true;
            Message = "Идет сохранение";
            await using var newDbContext = new ClockAllocationContext();
            UpdateFormCertification();
            newDbContext.Curricula.Update(SelectedCurriculum);
            await newDbContext.SaveChangesAsync();
            IsLoading = false;
            MainViewModel.Instance.UserControl = new CurriculumView();
        }
        catch (Exception e)
        {
            Message = e.Message;
            await Task.Delay(3000);
            IsLoading = false;
            Debug.WriteLine(e.Message);
        }
    }
    public void Cancel()
    {
        MainViewModel.Instance.UserControl = new CurriculumView();
    }

    private void UpdateFormCertification()
    {
        try
        {
            foreach (var module in SelectedCurriculum.Modules)
            {
                Debug.WriteLine(module.Title);
                foreach (var subject in module.Subjects)
                {
                    Debug.WriteLine(subject.Title);
                    foreach (var semester in subject.Semesters)
                    {
                        Debug.WriteLine(semester.Number);
                        if (semester.IdFormOfIntermediateCertification != 0 ) continue;
                        semester.IdFormOfIntermediateCertification = 4;
                        semester.IdFormOfIntermediateCertificationNavigation =
                            Certifications.FirstOrDefault(x => x.IdFormOfIntermediateCertification == 4);
                    }
                }
            }
        }
        catch (Exception e)
        {
            
            throw;
        }
    }
}

public class PropertyName
{
    public string Name { get; set; }
    public bool IsReadOnly { get; set; }
}