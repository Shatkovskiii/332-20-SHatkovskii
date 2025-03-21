using System;
using System.Windows.Forms;
using System.Linq;
using Shatkovskii_student.Models;
using Shatkovskii_student.Services;

namespace Shatkovskii_student.Forms
{
    public partial class MainForm : Form
    {
        private readonly StudentManager _studentManager;
        private BindingSource _bindingSource;

        public MainForm()
        {
            InitializeComponent();
            _studentManager = new StudentManager();
            _bindingSource = new BindingSource();
            InitializeControls();
            SetupDataGridView();
            SetupEventHandlers();
        }

        private void InitializeControls()
        {
            dataGridView.AutoGenerateColumns = false;
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "LastName",
                HeaderText = "Фамилия",
                DataPropertyName = "LastName"
            });
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FirstName",
                HeaderText = "Имя",
                DataPropertyName = "FirstName"
            });
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MiddleName",
                HeaderText = "Отчество",
                DataPropertyName = "MiddleName"
            });
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Course",
                HeaderText = "Курс",
                DataPropertyName = "Course"
            });
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Group",
                HeaderText = "Группа",
                DataPropertyName = "Group"
            });
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "BirthDate",
                HeaderText = "Дата рождения",
                DataPropertyName = "BirthDate",
                DefaultCellStyle = { Format = "dd.MM.yyyy" }
            });
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Email",
                HeaderText = "Email",
                DataPropertyName = "Email"
            });

            cbCourse.Items.AddRange(Enumerable.Range(1, 6).Cast<object>().ToArray());
            cbCourse.SelectedIndexChanged += FilterStudents;

            txtSearch.TextChanged += FilterStudents;
        }

        private void SetupDataGridView()
        {
            _bindingSource.DataSource = _studentManager.GetAllStudents();
            dataGridView.DataSource = _bindingSource;
        }

        private void SetupEventHandlers()
        {
            this.FormClosing += MainForm_FormClosing;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new StudentForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _studentManager.AddStudent(form.Student);
                        RefreshDataGrid();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView.CurrentRow?.DataBoundItem is Student student)
            {
                using (var form = new StudentForm(student))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            _studentManager.UpdateStudent(dataGridView.CurrentRow.Index, form.Student);
                            RefreshDataGrid();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView.CurrentRow != null)
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить этого студента?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _studentManager.RemoveStudent(dataGridView.CurrentRow.Index);
                    RefreshDataGrid();
                }
            }
        }

        private void FilterStudents(object sender, EventArgs e)
        {
            var course = cbCourse.SelectedItem as int?;
            var lastName = txtSearch.Text;

            var filteredStudents = _studentManager.FilterStudents(course, null, lastName);
            _bindingSource.DataSource = filteredStudents;
        }

        private void RefreshDataGrid()
        {
            _bindingSource.DataSource = null;
            _bindingSource.DataSource = _studentManager.GetAllStudents();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_studentManager.HasUnsavedChanges)
            {
                var result = MessageBox.Show(
                    "У вас есть несохраненные изменения. Хотите сохранить их перед выходом?",
                    "Несохраненные изменения",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning);

                switch (result)
                {
                    case DialogResult.Yes:
                        SaveData();
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        private void SaveData()
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "JSON files (*.json)|*.json";
                dialog.Title = "Сохранить данные студентов";
                dialog.DefaultExt = "json";
                
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var directory = System.IO.Path.GetDirectoryName(dialog.FileName);
                        if (!System.IO.Directory.Exists(directory))
                        {
                            System.IO.Directory.CreateDirectory(directory);
                        }

                        _studentManager.SaveToJson(dialog.FileName);
                        MessageBox.Show("Сохранено (ура)", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("Нет прав на запись (о нет(", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (System.IO.IOException ex)
                    {
                        MessageBox.Show($"Файл занят (о нет(", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка (о нет(", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "JSON files (*.json)|*.json";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _studentManager.LoadFromJson(dialog.FileName);
                        RefreshDataGrid();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка загрузки (о нет(", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "CSV files (*.csv)|*.csv";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _studentManager.ImportFromCsv(dialog.FileName);
                        RefreshDataGrid();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка импорта (о нет(", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "CSV files (*.csv)|*.csv";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _studentManager.ExportToCsv(dialog.FileName);
                        MessageBox.Show("Экспортировано (ура)", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка экспорта (о нет(", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
} 