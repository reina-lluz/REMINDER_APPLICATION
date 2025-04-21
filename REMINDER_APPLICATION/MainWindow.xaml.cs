using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace REMINDER_APPLICATION
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Reminder> Reminders = new List<Reminder>();
        private Reminder SelectedReminder;
        private readonly string filePath = @"C:\Users\23-0119c\source\repos\REMINDER_APPLICATION\reminder_application.txt";

        public MainWindow()
        {
            InitializeComponent();
            LoadReminders(); // Load reminders from the file when the application starts
        }

        // Event handler for the "Add Reminder" button click
        private void AddReminderButton_Click(object sender, RoutedEventArgs e)
        {
            string title = TitleTextBox.Text;
            string description = DescriptionTextBox.Text;
            DateTime? reminderTime = ReminderDatePicker.SelectedDate;
            string priority = (PriorityComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string category = (CategoryComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description) || reminderTime == null || string.IsNullOrWhiteSpace(priority) || string.IsNullOrWhiteSpace(category))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            if (reminderTime < DateTime.Now)
            {
                MessageBox.Show("Please choose a date in the present or future.");
                return;
            }

            if (SelectedReminder != null)
            {
                // Update existing reminder
                SelectedReminder.Title = title;
                SelectedReminder.Description = description;
                SelectedReminder.Time = reminderTime.Value.ToString("g");
                SelectedReminder.Priority = priority;
                SelectedReminder.Category = category;

                SelectedReminder = null; // Clear after edit
            }
            else
            {
                // Create a new reminder
                Reminder newReminder = new Reminder
                {
                    Title = title,
                    Description = description,
                    Time = reminderTime.Value.ToString("g"),
                    Priority = priority,
                    Category = category
                };

                Reminders.Add(newReminder);
            }

            // Sort reminders by priority
            Reminders = Reminders.OrderBy(r => r.Priority == "Low" ? 3 : r.Priority == "Medium" ? 2 : 1).ToList();

            // Refresh list
            ReminderListView.ItemsSource = null;
            ReminderListView.ItemsSource = Reminders;

            SaveReminders();

            // Clear input fields
            TitleTextBox.Clear();
            DescriptionTextBox.Clear();
            ReminderDatePicker.SelectedDate = null;
            PriorityComboBox.SelectedIndex = -1;
            CategoryComboBox.SelectedIndex = -1;
        }


        // Event handler for the "Edit Reminder" button click
        private void EditReminderButton_Click(object sender, RoutedEventArgs e)
        {
            if (ReminderListView.SelectedItem == null)
            {
                MessageBox.Show("Please select a reminder to edit.");
                return;
            }

            SelectedReminder = (Reminder)ReminderListView.SelectedItem;

            // Fill input fields with the selected reminder's details
            TitleTextBox.Text = SelectedReminder.Title;
            DescriptionTextBox.Text = SelectedReminder.Description;
            ReminderDatePicker.SelectedDate = DateTime.Parse(SelectedReminder.Time);
            PriorityComboBox.SelectedItem = PriorityComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item.Content.ToString() == SelectedReminder.Priority);
            CategoryComboBox.SelectedItem = CategoryComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item.Content.ToString() == SelectedReminder.Category);
        }

        // Event handler for the "Delete Reminder" button click
        private void DeleteReminderButton_Click(object sender, RoutedEventArgs e)
        {
            if (ReminderListView.SelectedItem == null)
            {
                MessageBox.Show("Please select a reminder to delete.");
                return;
            }

            // Remove the selected reminder from the list
            Reminders.Remove((Reminder)ReminderListView.SelectedItem);

            // Refresh the list view
            ReminderListView.ItemsSource = null;
            ReminderListView.ItemsSource = Reminders;

            // Save the updated reminders to the file
            SaveReminders();
        }

        // Method to save reminders to a text file
        private void SaveReminders()
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var reminder in Reminders)
                {
                    writer.WriteLine($"{reminder.Title}|{reminder.Description}|{reminder.Time}|{reminder.Priority}|{reminder.Category}");
                }
            }
        }

        // Method to load reminders from a text file
        private void LoadReminders()
        {
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 5)
                    {
                        Reminders.Add(new Reminder
                        {
                            Title = parts[0],
                            Description = parts[1],
                            Time = parts[2],
                            Priority = parts[3],
                            Category = parts[4]
                        });
                    }
                }

                // Sort reminders by priority and display them
                Reminders = Reminders.OrderBy(r => r.Priority == "Low" ? 3 : r.Priority == "Medium" ? 2 : 1).ToList();
                ReminderListView.ItemsSource = Reminders;
            }
        }

        private void ReminderListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TitleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }

    // A class to represent a Reminder object
    public class Reminder
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Time { get; set; }
        public string Priority { get; set; }
        public string Category { get; set; }
    }
}