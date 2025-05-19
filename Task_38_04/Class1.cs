using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
namespace StudentInfoApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Student> students = new List<Student>();
        private const string fileName = "students.dat";
        public MainWindow()
        {
            InitializeComponent();
            LoadStudentsFromFile();
            UpdateListBox();
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(FirstNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(GroupNameTextBox.Text) ||
                GenderComboBox.SelectedItem == null ||
                BirthDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }
            // Создание объекта Student
            Student newStudent = new Student
            {
                LastName = LastNameTextBox.Text,
                FirstName = FirstNameTextBox.Text,
                MiddleName = MiddleNameTextBox.Text,
                GroupName = GroupNameTextBox.Text,
                Gender = (Gender)Enum.Parse(typeof(Gender), GenderComboBox.SelectedItem.ToString()),
                BirthDate = BirthDatePicker.SelectedDate.Value
            };
            // Добавление студента в список
            students.Add(newStudent);

            // Обновление ListBox
            UpdateListBox();

            // Очистка полей ввода
            ClearInputFields();
        }
        private void UpdateListBox()
        {
            StudentListBox.Items.Clear();
            foreach (var student in students)
            {
                StudentListBox.Items.Add(student.ToString());  //Использование ToString()
            }
        }
        private void ClearInputFields()
        {
            LastNameTextBox.Text = "";
            FirstNameTextBox.Text = "";
            MiddleNameTextBox.Text = "";
            GroupNameTextBox.Text = "";
            GenderComboBox.SelectedItem = null;
            BirthDatePicker.SelectedDate = null;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveStudentsToFile();
        }
        private void SaveStudentsToFile()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Create))
                {
                    formatter.Serialize(fs, students);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сериализации данных: {ex.Message}");
            }
        }
        private void LoadStudentsFromFile()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                if (File.Exists(fileName))
                {
                    using (FileStream fs = new FileStream(fileName, FileMode.Open))
                    {
                        students = (List<Student>)formatter.Deserialize(fs);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при десериализации данных: {ex.Message}");
                students = new List<Student>(); //Иначе программа вылетит в следующий раз
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Заполнение ComboBox элементами перечисления Gender
            GenderComboBox.ItemsSource = Enum.GetValues(typeof(Gender)).Cast<Gender>();
        }
    }
    [Serializable]
    public class Student
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string GroupName { get; set; }
        public Gender Gender { get; set; }
        public DateTime BirthDate { get; set; }

        public override string ToString()
        {
            return $"{LastName} {FirstName} {MiddleName}, Группа: {GroupName}, Пол: {Gender}, Дата рождения: {BirthDate.ToShortDateString()}";
        }

    }
    public enum Gender
    {
        Мужской,
        Женский
    }
}