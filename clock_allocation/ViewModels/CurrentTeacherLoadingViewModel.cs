using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using clock_allocation.Models;
using clock_allocation.Resources;
using clock_allocation.Views;
using DynamicData;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;

namespace clock_allocation.ViewModels;

public class CurrentTeacherLoadingViewModel : ViewModelBase
{
    private Grid _teachingLoad;

    private Grid frozenGrid;
    private Grid scrollableGrid;

    private string _message;
    private bool _isLoading;
    private bool _isDelete;
    private List<TeachingLoad> _teachingLoads;

    public List<TeachingLoad> TeachingLoads
    {
        get => _teachingLoads;
        set => this.RaiseAndSetIfChanged(ref _teachingLoads, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    public bool IsDelete
    {
        get => _isDelete;
        set => this.RaiseAndSetIfChanged(ref _isDelete, value);
    }

    public string Message
    {
        get => _message;
        set => this.RaiseAndSetIfChanged(ref _message, value);
    }


    private int countOfSemesters;
    private TeachingLoad _idTeachingLoad;

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

    public Grid TeachingLoad
    {
        get => _teachingLoad;
        set => this.RaiseAndSetIfChanged(ref _teachingLoad, value);
    }


    public CurrentTeacherLoadingViewModel(int year, int idTeacher)
    {
        ScrollableGrid = new Grid();
        FrozenGrid = new Grid();
        TeachingLoads = db.TeachingLoads.Include(x => x.IdGroupNavigation)
            .ThenInclude(x => x.IdSpecializationNavigation).Include(x => x.IdSemesterNavigation)
            .ThenInclude(x => x.IdFormOfIntermediateCertificationNavigation)
            .Include(x => x.IdSubjectNavigation).Where(x => x.Year == year && x.IdTeacher == idTeacher).ToList();
        CreateComplexHeader();
    }

    private void CreateComplexHeader()
    {
        var headers = new List<HeaderCellInfo>();
        headers.AddRange([
            new HeaderCellInfo
            {
                Text = "Курс и группа", Column = 0, Row = 0, ColumnSpan = 1,
                RowSpan = 4,
                IsVertical = false
            },
            new HeaderCellInfo
            {
                Text = "Наименование дисциплины", Column = 1, Row = 0, ColumnSpan = 1,
                RowSpan = 4,
                IsVertical = false
            },
            new HeaderCellInfo
            {
                Text = "Максимальное количество часов по учебному плану", Column = 2, Row = 0, ColumnSpan = 1,
                RowSpan = 4,
                IsVertical = true
            },
            new HeaderCellInfo
            {
                Text = "Очное отделение", Column = 3, Row = 0, ColumnSpan = 21,
                RowSpan = 1,
                IsVertical = false
            },
            new HeaderCellInfo
            {
                Text = "Часов за 1 семестр", Column = 3, Row = 1, ColumnSpan = 7,
                RowSpan = 1,
                IsVertical = false
            },
            new HeaderCellInfo
            {
                Text = "часов в неделю", Column = 3, Row = 2, ColumnSpan = 1,
                RowSpan = 2,
                IsVertical = true
            },
            new HeaderCellInfo
            {
                Text = "из них", Column = 4, Row = 2, ColumnSpan = 5,
                RowSpan = 1,
                IsVertical = false
            },
            new HeaderCellInfo
            {
                Text = "лекционных", Column = 4, Row = 3, ColumnSpan = 1,
                RowSpan = 1,
                IsVertical = true
            },
            new HeaderCellInfo
            {
                Text = "лабораторных", Column = 5, Row = 3, ColumnSpan = 1,
                RowSpan = 1,
                IsVertical = true
            },
            new HeaderCellInfo
            {
                Text = "консультации в КТП", Column = 6, Row = 3, ColumnSpan = 1,
                RowSpan = 1,
                IsVertical = true
            },
            new HeaderCellInfo
            {
                Text = "самостоятельная работа", Column = 7, Row = 3, ColumnSpan = 1,
                RowSpan = 1,
                IsVertical = true
            },
            new HeaderCellInfo
            {
                Text = "курсовое проектирование", Column = 8, Row = 3, ColumnSpan = 1,
                RowSpan = 1,
                IsVertical = true
            },
            new HeaderCellInfo
            {
                Text = "Часов за 2 семестр", Column = 9, Row = 1, ColumnSpan = 6,
                RowSpan = 1,
                IsVertical = false
            },
            new HeaderCellInfo
            {
                Text = "часов в неделю", Column = 9, Row = 2, ColumnSpan = 1,
                RowSpan = 2,
                IsVertical = true
            },
            new HeaderCellInfo
            {
                Text = "из них", Column = 10, Row = 2, ColumnSpan = 5,
                RowSpan = 1,
                IsVertical = false
            },
            new HeaderCellInfo
            {
                Text = "лекционных", Column = 10, Row = 3, ColumnSpan = 1,
                RowSpan = 1,
                IsVertical = true
            },
            new HeaderCellInfo
            {
                Text = "лабораторных", Column = 11, Row = 3, ColumnSpan = 1,
                RowSpan = 1,
                IsVertical = true
            },
            new HeaderCellInfo
            {
                Text = "консультации в КТП", Column = 12, Row = 3, ColumnSpan = 1,
                RowSpan = 1,
                IsVertical = true
            },
            new HeaderCellInfo
            {
                Text = "самостоятельная работа", Column = 13, Row = 3, ColumnSpan = 1,
                RowSpan = 1,
                IsVertical = true
            },
            new HeaderCellInfo
            {
                Text = "курсовое проектирование", Column = 14, Row = 3, ColumnSpan = 1,
                RowSpan = 1,
                IsVertical = true
            },
            new HeaderCellInfo
            {
                Text = "Учебных часов в год, бюджет", Column = 15, Row = 1, ColumnSpan = 1,
                RowSpan = 3,
                IsVertical = true
            },
            new HeaderCellInfo
            {
                Text = "Учебных часов в год, внебюджет", Column = 16, Row = 1, ColumnSpan = 1,
                RowSpan = 3,
                IsVertical = true
            },
            new HeaderCellInfo
            {
                Text = "Экзамены", Column = 17, Row = 1, ColumnSpan = 2,
                RowSpan = 1,
                IsVertical = false
            },
            new HeaderCellInfo
            {
                Text = "Первый семестр", Column = 17, Row = 2, ColumnSpan = 1,
                RowSpan = 2,
                IsVertical = true
            },
            new HeaderCellInfo
            {
                Text = "Второй семестр", Column = 18, Row = 2, ColumnSpan = 1,
                RowSpan = 2,
                IsVertical = true
            },
            new HeaderCellInfo
            {
                Text = "Консультации с почасовой оплатой", Column = 19, Row = 1, ColumnSpan = 1,
                RowSpan = 3,
                IsVertical = true
            },
            new HeaderCellInfo
            {
                Text = "Проверка курс.раб.", Column = 20, Row = 1, ColumnSpan = 1,
                RowSpan = 3,
                IsVertical = true
            },
            new HeaderCellInfo
            {
                Text = "Форма аттестации", Column = 21, Row = 1, ColumnSpan = 1,
                RowSpan = 3,
                IsVertical = true
            },
            new HeaderCellInfo
            {
                Text = "обзорно-лекционные", Column = 22, Row = 1, ColumnSpan = 1,
                RowSpan = 3,
                IsVertical = true
            },
            new HeaderCellInfo
            {
                Text = "Заочное отделение", Column = 23, Row = 0, ColumnSpan = 8,
                RowSpan = 1,
                IsVertical = false
            },
            new HeaderCellInfo
            {
                Text = "лабораторн.", Column = 23, Row = 1, ColumnSpan = 1,
                RowSpan = 3,
                IsVertical = true
            },
            new HeaderCellInfo
            {
                Text = "Учебных часов в год, бюджет", Column = 24, Row = 1, ColumnSpan = 1,
                RowSpan = 3,
                IsVertical = true
            },
            new HeaderCellInfo
            {
                Text = "Учебных часов в год, внебюджет", Column = 25, Row = 1, ColumnSpan = 1,
                RowSpan = 3,
                IsVertical = true
            },
            new HeaderCellInfo
            {
                Text = "проверка контр.работ", Column = 26, Row = 1, ColumnSpan = 1,
                RowSpan = 3,
                IsVertical = true
            },
            new HeaderCellInfo
            {
                Text = "Экзамены", Column = 27, Row = 1, ColumnSpan = 1,
                RowSpan = 3,
                IsVertical = true
            },
            new HeaderCellInfo
            {
                Text = "Консультации", Column = 28, Row = 1, ColumnSpan = 1,
                RowSpan = 3,
                IsVertical = true
            },
            new HeaderCellInfo
            {
                Text = "Проверка курсовых работ", Column = 29, Row = 1, ColumnSpan = 1,
                RowSpan = 3,
                IsVertical = true
            },
            new HeaderCellInfo
            {
                Text = "Форма аттестации", Column = 30, Row = 1, ColumnSpan = 1,
                RowSpan = 3,
                IsVertical = true
            },
            new HeaderCellInfo
            {
                Text = "Всего тарификация", Column = 31, Row = 0, ColumnSpan = 1,
                RowSpan = 4,
                IsVertical = true
            },
            new HeaderCellInfo
            {
                Text = "Всего год", Column = 32, Row = 0, ColumnSpan = 1,
                RowSpan = 4,
                IsVertical = false
            }
        ]);
        ScrollableGrid = new Grid
        {
            Background = Brushes.White,
            RowDefinitions =
            {
                new RowDefinition(GridLength.Parse("60")),
                new RowDefinition(GridLength.Parse("60")),
                new RowDefinition(GridLength.Parse("60")),
                new RowDefinition(GridLength.Parse("180"))
            }
        };
        FrozenGrid = new Grid
        {
            Background = Brushes.White,
            RowDefinitions =
            {
                new RowDefinition(GridLength.Parse("60")),
                new RowDefinition(GridLength.Parse("60")),
                new RowDefinition(GridLength.Parse("60")),
                new RowDefinition(GridLength.Parse("180"))
            },
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star),
            }
        };

        for (var i = 0; i < 34; i++)
        {
            ScrollableGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        }


        foreach (var header in headers)
        {
            if (header.Column is 0 or 1)
            {
                FrozenGrid.Children.Add(header.IsVertical
                    ? CreateVerticalHeaderCell(header.Text, header.Column, header.Row, header.ColumnSpan,
                        header.RowSpan)
                    : CreateHeaderCell(header.Text, header.Column, header.Row, header.ColumnSpan, header.RowSpan));
            }
            else
            {
                ScrollableGrid.Children.Add(header.IsVertical
                    ? CreateVerticalHeaderCell(header.Text, header.Column, header.Row, header.ColumnSpan,
                        header.RowSpan)
                    : CreateHeaderCell(header.Text, header.Column, header.Row, header.ColumnSpan, header.RowSpan));
            }
        }

        TeachingLoad = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Auto), // Для FrozenGrid
                new ColumnDefinition(GridLength.Star) // Для ScrollViewer
            },
            RowDefinitions = { new RowDefinition(GridLength.Auto) }
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
        TeachingLoad.Children.Add(FrozenGrid);

        Grid.SetRow(scrollViewer, 0);
        Grid.SetColumn(scrollViewer, 1);
        TeachingLoad.Children.Add(scrollViewer);
        var row = CreateContent();
        row++;
        FrozenGrid.Children.Add(CreateCell("", 0, row));
        FrozenGrid.Children.Add(CreateCell("ИТОГО", 1, row));
        ScrollableGrid.Children.Add(CreateCell("", 2, row));
        ScrollableGrid.Children.Add(CreateCell(
            TeachingLoads
                .Where(x => x.IdSemesterNavigation.Number is 1 or 3 or 5 or 7)
                .Select(x => x.HoursPerWeek).Sum().ToString(), 3, row));
        for (int i = 4; i <= 8; i++)
        {
            ScrollableGrid.Children.Add(CreateCell("", i, row));
        }

        ScrollableGrid.Children.Add(CreateCell(
            TeachingLoads
                .Where(x => x.IdSemesterNavigation.Number is 2 or 4 or 6 or 8)
                .Select(x => x.HoursPerWeek).Sum().ToString(), 9, row));
        for (int i = 10; i <=13; i++)
        {
            ScrollableGrid.Children.Add(CreateCell("", i, row));
        }
        ScrollableGrid.Children.Add(CreateCell(
            TeachingLoads
                .Where(x => x.IdGroupNavigation.FormOfEducation == "Бюджет")
                .Select(x => x.AcademicHoursPerYear).Sum().ToString(), 14, row));
        ScrollableGrid.Children.Add(CreateCell(
            TeachingLoads
                .Where(x => x.IdGroupNavigation.FormOfEducation == "Внебюджет")
                .Select(x => x.AcademicHoursPerYear).Sum().ToString(), 15, row));
        ScrollableGrid.Children.Add(CreateCell(
            TeachingLoads
                .Where(x => x.IdSemesterNavigation.Number is 1 or 3 or 5 or 7)
                .Select(x => x.Exam).Sum().ToString(), 16, row));
        ScrollableGrid.Children.Add(CreateCell(
            TeachingLoads
                .Where(x => x.IdSemesterNavigation.Number is 2 or 4 or 6 or 8)
                .Select(x => x.Exam).Sum().ToString(), 17, row));
        ScrollableGrid.Children.Add(CreateCell(
            TeachingLoads
                .Select(x => x.СonsultationsPayment).Sum().ToString(), 18, row));
        for (int i = 19; i <= 29; i++)
        {
            ScrollableGrid.Children.Add(CreateCell("", i, row));
        }
        ScrollableGrid.Children.Add(CreateCell(
            TeachingLoads
                .Select(x => x.Billing).Sum().ToString(), 30, row));
        ScrollableGrid.Children.Add(CreateCell(
            TeachingLoads
                .Select(x => x.TotalYear).Sum().ToString(), 31, row));
    }

    private int CreateContent()
    {
        var row = 4;
        var subjects = TeachingLoads.Select(x => x.IdSubject).Distinct().ToList();
        foreach (var subject in subjects)
        {
            FrozenGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            ScrollableGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
            var countGroups = TeachingLoads.Count(x => x.IdSubject == subject);
            var currentTeachingLoad =
                TeachingLoads.Where(x => x.IdSubject == subject).OrderBy(x => x.IdTeachingLoad).ToList();
            var title = TeachingLoads.FirstOrDefault(x => x.IdSubject == subject);
            FrozenGrid.Children.Add(CreateCell(title.IdSubjectNavigation.Title, 1, row, 1, countGroups / 2));
            for (var i = 0; i < currentTeachingLoad.Count; i += 2)
            {
                FrozenGrid.Children.Add(currentTeachingLoad[i].Split
                    ? CreateCell(currentTeachingLoad[i].IdGroupNavigation + "(2)", 0, row)
                    : CreateCell(currentTeachingLoad[i].IdGroupNavigation.ToString(), 0, row));

                ScrollableGrid.Children.Add(CreateCell(currentTeachingLoad[i].MaximumWorkload.ToString(), 2, row));
                if (currentTeachingLoad[i].IdGroupNavigation.IdSpecializationNavigation.FormOfEducation == "Очная")
                {
                    ScrollableGrid.Children.Add(CreateCell(currentTeachingLoad[i].HoursPerWeek.ToString(), 3, row));
                    ScrollableGrid.Children.Add(CreateCell(currentTeachingLoad[i].Lectures.ToString(), 4, row));
                    ScrollableGrid.Children.Add(CreateCell(currentTeachingLoad[i].Laboratory.ToString(), 5, row));
                    ScrollableGrid.Children.Add(
                        CreateCell(currentTeachingLoad[i].IdSemesterNavigation.Consultations.ToString(), 6, row));
                    ScrollableGrid.Children.Add(CreateCell(currentTeachingLoad[i].IndependentWork.ToString(), 7, row));
                    ScrollableGrid.Children.Add(CreateCell(currentTeachingLoad[i].CourseDesign.ToString(), 8, row));
                    ScrollableGrid.Children.Add(CreateCell(currentTeachingLoad[i + 1].HoursPerWeek.ToString(), 9, row));
                    ScrollableGrid.Children.Add(CreateCell(currentTeachingLoad[i + 1].Lectures.ToString(), 10, row));
                    ScrollableGrid.Children.Add(CreateCell(currentTeachingLoad[i + 1].Laboratory.ToString(), 11, row));
                    ScrollableGrid.Children.Add(
                        CreateCell(currentTeachingLoad[i + 1].Consultations.ToString(), 12, row));
                    ScrollableGrid.Children.Add(
                        CreateCell(currentTeachingLoad[i + 1].IndependentWork.ToString(), 13, row));
                    ScrollableGrid.Children.Add(CreateCell(currentTeachingLoad[i + 1].CourseDesign.ToString(), 14,
                        row));
                    if (currentTeachingLoad[i].IdGroupNavigation.FormOfEducation == "Бюджет")
                    {
                        var hoursPerYears = currentTeachingLoad[i + 1].AcademicHoursPerYear +
                                            currentTeachingLoad[i].AcademicHoursPerYear;
                        ScrollableGrid.Children.Add(CreateCell(hoursPerYears.ToString(), 15, row));
                        ScrollableGrid.Children.Add(CreateCell("", 16, row));
                    }
                    else
                    {
                        var hoursPerYears = currentTeachingLoad[i + 1].AcademicHoursPerYear +
                                            currentTeachingLoad[i].AcademicHoursPerYear;
                        ScrollableGrid.Children.Add(CreateCell(hoursPerYears.ToString(), 16, row));
                        ScrollableGrid.Children.Add(CreateCell("", 15, row));
                    }

                    ScrollableGrid.Children.Add(CreateCell(currentTeachingLoad[i].Exam.ToString(), 17,
                        row));
                    ScrollableGrid.Children.Add(CreateCell(currentTeachingLoad[i + 1].Exam.ToString(), 18,
                        row));
                    ScrollableGrid.Children.Add(CreateCell(currentTeachingLoad[i].СonsultationsPayment.ToString(), 19,
                        row));
                    ScrollableGrid.Children.Add(CreateCell(currentTeachingLoad[i].CheckingCoursework.ToString(), 20,
                        row));
                    if (!currentTeachingLoad[i].Split)
                    {
                        var formOfIntermediateCertification = "";
                        if (currentTeachingLoad[i].IdSemesterNavigation.IdFormOfIntermediateCertificationNavigation
                                .Title != "РК")
                        {
                            formOfIntermediateCertification += currentTeachingLoad[i].IdSemesterNavigation
                                .IdFormOfIntermediateCertificationNavigation
                                .Title;
                        }

                        if (currentTeachingLoad[i + 1].IdSemesterNavigation.IdFormOfIntermediateCertificationNavigation
                                .Title != "РК")
                        {
                            formOfIntermediateCertification += "," + currentTeachingLoad[i + 1].IdSemesterNavigation
                                .IdFormOfIntermediateCertificationNavigation
                                .Title;
                        }

                        ScrollableGrid.Children.Add(CreateCell(formOfIntermediateCertification, 21,
                            row));
                    }

                    ScrollableGrid.Children.Add(CreateCell(currentTeachingLoad[i].ReviewAndLecture.ToString(), 22,
                        row));
                    for (var j = 23; j <= 30; j++)
                    {
                        ScrollableGrid.Children.Add(CreateCell("", j,
                            row));
                    }
                }
                else
                {
                    for (var j = 2; j <= 22; j++)
                    {
                        ScrollableGrid.Children.Add(CreateCell("", j,
                            row));
                    }

                    ScrollableGrid.Children.Add(CreateCell(currentTeachingLoad[i].Laboratory.ToString(), 23,
                        row));
                    if (currentTeachingLoad[i].IdGroupNavigation.FormOfEducation == "Бюджет")
                    {
                        var hoursPerYears = currentTeachingLoad[i].AcademicHoursPerYear;
                        ScrollableGrid.Children.Add(CreateCell(hoursPerYears.ToString(), 24, row));
                        ScrollableGrid.Children.Add(CreateCell("", 16, row));
                    }
                    else
                    {
                        var hoursPerYears = currentTeachingLoad[i].AcademicHoursPerYear;
                        ScrollableGrid.Children.Add(CreateCell(hoursPerYears.ToString(), 24, row));
                        ScrollableGrid.Children.Add(CreateCell("", 15, row));
                    }

                    ScrollableGrid.Children.Add(CreateCell(currentTeachingLoad[i].CheckingCoursework.ToString(), 25,
                        row));
                    ScrollableGrid.Children.Add(CreateCell(currentTeachingLoad[i].Exam.ToString(), 26,
                        row));
                    ScrollableGrid.Children.Add(CreateCell(currentTeachingLoad[i].Consultations.ToString(), 27,
                        row));
                    ScrollableGrid.Children.Add(CreateCell(currentTeachingLoad[i].Laboratory.ToString(), 28,
                        row));
                    var formOfIntermediateCertification = "";
                    if (currentTeachingLoad[i].IdSemesterNavigation.IdFormOfIntermediateCertificationNavigation
                            .Title != "РК")
                    {
                        formOfIntermediateCertification += currentTeachingLoad[i].IdSemesterNavigation
                            .IdFormOfIntermediateCertificationNavigation
                            .Title;
                    }

                    if (currentTeachingLoad[i + 1].IdSemesterNavigation.IdFormOfIntermediateCertificationNavigation
                            .Title != "РК")
                    {
                        formOfIntermediateCertification += "," + currentTeachingLoad[i + 1].IdSemesterNavigation
                            .IdFormOfIntermediateCertificationNavigation
                            .Title;
                    }

                    ScrollableGrid.Children.Add(CreateCell(formOfIntermediateCertification, 29,
                        row));
                }

                ScrollableGrid.Children.Add(CreateCell(currentTeachingLoad[i].Billing.ToString(), 31,
                    row));
                ScrollableGrid.Children.Add(CreateCell(currentTeachingLoad[i].TotalYear.ToString(), 32,
                    row));

                FrozenGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
                ScrollableGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));

                row++;
            }

            row++;
        }

        return row;
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

    private Border CreateCell(string text, int column, int row, int columnSpan = 1, int rowSpan = 1)
    {
        if (text == "0") text = "";
        var border = new Border
        {
            BorderBrush = Brushes.Gray,
            BorderThickness = new Thickness(0.5),
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Background = Brushes.White,
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

    public void DeleteTeachingLoad()
    {
        try
        {
            db.TeachingLoads.RemoveRange(TeachingLoads);
            db.SaveChanges();
            IsDelete = false;
            MainViewModel.Instance.UserControl = new TeachingLoadingView();
        }
        catch (Exception e)
        {
            Message = "Ошибка при удалении";
            Debug.WriteLine(e.Message);
        }
    }

    public void CancelDeleteCommand()
    {
        IsDelete = false;
    }

    public void AskDeleteTeachingLoad()
    {
        IsDelete = true;
    }

    public void UpdateTeachingLoad()
    {
        MainViewModel.Instance.UserControl = new UpdateTeacherLoadingView(TeachingLoads);
    }
}