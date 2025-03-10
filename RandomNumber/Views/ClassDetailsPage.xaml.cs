using RandomNumber.Services;
using System.Collections.ObjectModel;

namespace RandomNumber
{
    public partial class ClassDetailsPage : ContentPage
    {
        public string ClassName { get; private set; }
        public ObservableCollection<SelectableStudent> StudentsForRandomSelection { get; private set; } = new();
        private Queue<string> RecentlyDrawnStudents { get; set; } = new();

        public ClassDetailsPage(string className)
        {
            InitializeComponent();
            ClassName = className;
            Title = $"Klasa: {ClassName}";
            LoadStudents();
        }

        private async void LoadStudents()
        {
            var students = await DataService.GetStudentsAsync(ClassName);
            StudentsList.ItemsSource = students;

            StudentsForRandomSelection.Clear();
            foreach (var student in students)
            {
                StudentsForRandomSelection.Add(new SelectableStudent { Name = student, IsSelected = true });
            }
        }

        private async void OnAddStudentClicked(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Nowy Uczeń", "Podaj nazwę ucznia:");
            if (!string.IsNullOrEmpty(result))
            {
                await DataService.AddStudentAsync(ClassName, result);
                LoadStudents();
            }
        }

       private async void OnStudentTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            string selectedStudent = e.Item.ToString();
            string action = await DisplayActionSheet(selectedStudent, "Anuluj", null, "Edytuj", "Usuń");

            if (action == "Edytuj")
            {
                string newName = await DisplayPromptAsync("Edytuj Ucznia", "Podaj nową nazwę:", initialValue: selectedStudent);
                if (!string.IsNullOrEmpty(newName))
                {
                    await DataService.UpdateStudentAsync(ClassName, selectedStudent, newName);
                    LoadStudents();
                }
            }
            else if (action == "Usuń")
            {
                bool confirm = await DisplayAlert("Usuń Ucznia", $"Czy na pewno chcesz usunąć {selectedStudent}?", "Tak", "Nie");
                if (confirm)
                {
                    await DataService.DeleteStudentAsync(ClassName, selectedStudent);
                    LoadStudents();
                }
            }

            ((ListView)sender).SelectedItem = null;
        }


        private void OnStartRandomSelectionClicked(object sender, EventArgs e)
        {
            if (!StudentsForRandomSelection.Any())
            {
                DisplayAlert("Brak uczniów", "Lista uczniów jest pusta. Dodaj uczniów, aby kontynuować.", "OK");
                return;
            }

            RandomSelectionList.ItemsSource = StudentsForRandomSelection;
            RandomSelectionList.IsVisible = true;
            RandomButton.IsVisible = true;
        }

        private async void OnPerformRandomSelectionClicked(object sender, EventArgs e)
        {
            var selectedStudents = StudentsForRandomSelection.Where(s => s.IsSelected).Select(s => s.Name).ToList();

            if (!selectedStudents.Any())
            {
                await DisplayAlert("Brak Wyboru", "Nie wybrano żadnych uczniów do losowania.", "OK");
                return;
            }

            var availableStudents = selectedStudents.Where(s => !RecentlyDrawnStudents.Contains(s)).ToList();

            if (!availableStudents.Any())
            {
                await DisplayAlert("Brak dostępnych uczniów", "Wszyscy zaznaczeni uczniowie zostali wykluczeni z losowania na 3 rundy.", "OK");
                return;
            }

            var random = new Random();
            string randomStudent = availableStudents[random.Next(availableStudents.Count)];

            RecentlyDrawnStudents.Enqueue(randomStudent);
            if (RecentlyDrawnStudents.Count > 3)
            {
                RecentlyDrawnStudents.Dequeue();
            }

            await DisplayAlert("Wylosowany Uczeń", $"Wylosowano: {randomStudent}", "OK");

            await DataService.SaveRoundHistoryAsync(ClassName, selectedStudents, RecentlyDrawnStudents.ToList());
        }
    }

    public class SelectableStudent
    {
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}