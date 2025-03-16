using System;
using System.Linq;
using System.Windows.Forms;
using Shatkovskii_student.Models;

namespace Shatkovskii_student.Forms
{
    public partial class StudentForm : Form
    {
        private Student _student;

        public Student Student => _student;

        public StudentForm(Student student = null)
        {
            InitializeComponent();
            SetupControls();
            
            if (student != null)
            {
                _student = student;
                LoadStudentData();
            }
            else
            {
                _student = new Student();
            }
        }

        private void SetupControls()
        {
            // Настройка DateTimePicker
            dtpBirthDate.MinDate = new DateTime(1992, 1, 1);
            dtpBirthDate.MaxDate = DateTime.Now;
            dtpBirthDate.Format = DateTimePickerFormat.Short;

            // Настройка ComboBox для курса
            cbCourse.Items.AddRange(Enumerable.Range(1, 6).Cast<object>().ToArray());
        }

        private void LoadStudentData()
        {
            txtLastName.Text = _student.LastName;
            txtFirstName.Text = _student.FirstName;
            txtMiddleName.Text = _student.MiddleName;
            cbCourse.SelectedItem = _student.Course;
            txtGroup.Text = _student.Group;
            dtpBirthDate.Value = _student.BirthDate;
            txtEmail.Text = _student.Email;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                try
                {
                    _student.LastName = txtLastName.Text.Trim();
                    _student.FirstName = txtFirstName.Text.Trim();
                    _student.MiddleName = txtMiddleName.Text.Trim();
                    _student.Course = (int)cbCourse.SelectedItem;
                    _student.Group = txtGroup.Text.Trim();
                    _student.BirthDate = dtpBirthDate.Value;
                    _student.Email = txtEmail.Text.Trim();

                    DialogResult = DialogResult.OK;
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("Введите фамилию", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtLastName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show("Введите имя", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtFirstName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtMiddleName.Text))
            {
                MessageBox.Show("Введите отчество", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtMiddleName.Focus();
                return false;
            }

            if (cbCourse.SelectedItem == null)
            {
                MessageBox.Show("Выберите курс", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cbCourse.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtGroup.Text))
            {
                MessageBox.Show("Введите группу", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtGroup.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Введите email", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtEmail.Focus();
                return false;
            }

            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
} 