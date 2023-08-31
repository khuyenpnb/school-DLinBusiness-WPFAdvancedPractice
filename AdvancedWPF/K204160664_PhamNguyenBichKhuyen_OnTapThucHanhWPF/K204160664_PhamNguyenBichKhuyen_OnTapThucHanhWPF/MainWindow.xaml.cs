using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using K204160664_PhamNguyenBichKhuyen_OnTapThucHanhWPF.Model;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.ComponentModel;

namespace K204160664_PhamNguyenBichKhuyen_OnTapThucHanhWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        string connectionString = @"server=ACER\KHUYEN;database=DbEmployeeManagement;uid=sa;pwd=khuyenpnb@12345";
        public MainWindow()
        {
            InitializeComponent();
            DefaultFormStyle();
            LoadEmployeeToListView();
        }

        private void DefaultFormStyle()
        {
            txtId.Text = string.Empty;
            txtName.Text = string.Empty;
            radBoy.IsChecked = true;
            txtPhone.Text = string.Empty;
            radSaler.IsChecked = true;
            lblRevenueOrAllowance.Content = "Doanh số:";
            dtpHireDate.SelectedDate = DateTime.Now;
            txtRevenueOrAllowance.Text = string.Empty;
            dtpHireDate.Focusable = true;
            dtpHireDate.IsHitTestVisible = true;
        }

        private void LoadEmployeeToListView()
        {
            lvEmployeeList.Items.Clear();
            SqlConnection connection = new SqlConnection(connectionString);
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string sql = "SELECT * FROM Employee WHERE IsDeleted = 0 ORDER BY EmployeeId";
            SqlCommand command = new SqlCommand(sql, connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string employeeId = reader.GetString(1);
                string employeeName = reader.GetString(2);
                string gender = reader.GetString(3);
                string phoneNumber = reader.GetString(4);
                DateTime hireDate_dt = reader.GetDateTime(5).Date;
                string hireDate = hireDate_dt.ToString("dd/MM/yyyy");
                string employeeType = reader.GetString(6);
                object revenueOrAllowanceObj = reader.GetValue(7);
                float revenueOrAllowance = float.Parse(revenueOrAllowanceObj.ToString());
                int isDeleted = reader.GetInt32(8);

                Employee employee = new Employee()
                {
                    EmployeeId = employeeId,
                    EmployeeName = employeeName,
                    Gender = gender,
                    PhoneNumber = phoneNumber,
                    HireDate = hireDate,
                    EmployeeType = employeeType,
                    RevenueOrAllowance=revenueOrAllowance,
                    IsDeleted = isDeleted,
                    Seniority5Years = (DateTime.Now.Date.Year - hireDate_dt.Year) >= 5
                };
                lvEmployeeList.Items.Add(employee);

            }
            reader.Close();
            connection.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Bạn có chắc muốn thoát chương trình?",
                "Question",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
                );
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            };
        }

        private void radSaler_Checked(object sender, RoutedEventArgs e)
        {
            lblRevenueOrAllowance.Content = "Doanh số:";
        }

        private void radShipper_Checked(object sender, RoutedEventArgs e)
        {
            lblRevenueOrAllowance.Content = "PC Nhiên liệu:";
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            DefaultFormStyle();
            txtId.Focus();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if ( txtId.Text == string.Empty ||
                txtName.Text == string.Empty ||
                txtPhone.Text == string.Empty ||
                txtRevenueOrAllowance.Text == string.Empty)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Vui lòng điền đầy đủ thông tin.",
                    "Warning",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                    );
            }
            else if (dtpHireDate.SelectedDate >= DateTime.Now)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Ngày vào làm không thể lớn hơn ngày hiện tại.",
                    "Warning",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                    );
                dtpHireDate.Focus();
            }
            else
            {
                saveToDatabase();
            }
        }

        private void saveToDatabase()
        {

            string gender = string.Empty;
            string employeeType = string.Empty;
            if (radBoy.IsChecked == true)
            {
                gender = "Nam";
            }
            else if (radGirl.IsChecked == true)
            {
                gender = "Nữ";
            }
            if (radSaler.IsChecked == true)
            {
                employeeType = "Saler";
            }
            else if (radShipper.IsChecked == true)
            {
                employeeType = "Shipper";
            }

            Employee employee = new Employee()
            {
                EmployeeId = txtId.Text,
                EmployeeName = txtName.Text,
                Gender = gender,
                PhoneNumber = txtPhone.Text,
                HireDate = dtpHireDate.SelectedDate.ToString(),
                EmployeeType = employeeType,
                RevenueOrAllowance = float.Parse(txtRevenueOrAllowance.Text)
            };

            SqlConnection connection = new SqlConnection(connectionString);
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string sql = 
                "INSERT INTO Employee(EmployeeId,EmployeeName,Gender,PhoneNumber,HireDate,EmployeeType,RevenueOrAllowance,IsDeleted) VALUES (@empId, @empName, @empGender, @empPhone, @empHireDate, @empType, @revOrAll, 0)";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.Add("@empId", SqlDbType.NVarChar).Value = txtId.Text;
            command.Parameters.Add("@empName", SqlDbType.NVarChar).Value = txtName.Text;
            command.Parameters.Add("@empGender", SqlDbType.NVarChar).Value = gender;
            command.Parameters.Add("@empPhone", SqlDbType.NVarChar).Value = txtPhone.Text;
            command.Parameters.Add("@empHireDate", SqlDbType.Date).Value = dtpHireDate.SelectedDate;
            command.Parameters.Add("@empType", SqlDbType.NVarChar).Value = employeeType;
            command.Parameters.Add("@revOrAll", SqlDbType.Float).Value = float.Parse(txtRevenueOrAllowance.Text);
            command.ExecuteNonQuery();
            connection.Close();

            LoadEmployeeToListView();

            foreach (Employee item in lvEmployeeList.Items)
            {
                if (item.EmployeeId == employee.EmployeeId)
                {
                    lvEmployeeList.SelectedIndex = lvEmployeeList.Items.IndexOf(item);
                    break;
                }
            }

            MessageBox.Show(
                "Thêm thành công.",
                "Info",
                MessageBoxButton.OK,
                MessageBoxImage.Information
                );

        }

        private void lvEmployeeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvEmployeeList.SelectedIndex != -1)
            {
                Employee employee = lvEmployeeList.Items[lvEmployeeList.SelectedIndex] as Employee;
                txtId.Text = employee.EmployeeId.ToString();
                txtName.Text = employee.EmployeeName.ToString();
                txtPhone.Text = employee.PhoneNumber.ToString();
                txtRevenueOrAllowance.Text = employee.RevenueOrAllowance.ToString();
                if (employee.Gender == "Nam")
                {
                    radBoy.IsChecked = true;
                }
                else if (employee.Gender == "Nữ")
                {
                    radGirl.IsChecked = true;
                }
                if (employee.EmployeeType == "Saler")
                {
                    radSaler.IsChecked = true;
                }
                else if (employee.EmployeeType == "Shipper")
                {
                    radShipper.IsChecked = true;
                }
                dtpHireDate.SelectedDate = DateTime.ParseExact(employee.HireDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                txtId.IsReadOnly = true;
                txtName.IsReadOnly = true;
                dtpHireDate.Focusable = false;
                dtpHireDate.IsHitTestVisible = false;
            }
            else
            {
                DefaultFormStyle();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lvEmployeeList.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Vui lòng chọn dữ liệu muốn xóa",
                    "Warning",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                    );
                return;
            }
            else
            {
                MessageBoxResult result = MessageBox.Show(
                    "Bạn có chắc muốn xóa dữ liệu này?",
                    "Question",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                    );
                if ( result == MessageBoxResult.No )
                {
                    return;
                }
                else
                {
                    deleteInDatabase();
                }
            }
        }

        private void deleteInDatabase()
        {
            Employee employee = lvEmployeeList.SelectedItem as Employee;
            string employeeId1 = employee.EmployeeId;
            Employee employee2 = new Employee();
            
            if (lvEmployeeList.SelectedIndex == lvEmployeeList.Items.Count - 1) 
            {
                employee2 = lvEmployeeList.Items[lvEmployeeList.Items.Count - 2] as Employee;
            }
            else
            {
                employee2 = lvEmployeeList.Items[lvEmployeeList.SelectedIndex + 1] as Employee;
            }

            SqlConnection connection = new SqlConnection(connectionString);
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string sql =
                "UPDATE Employee SET IsDeleted = 1, UpdatedDate = @updatedDate WHERE EmployeeId = @empId";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.Add("@updatedDate", SqlDbType.DateTime).Value = DateTime.Now;
            command.Parameters.Add("@empId", SqlDbType.NVarChar).Value = employeeId1;
            command.ExecuteNonQuery();
            connection.Close();

            LoadEmployeeToListView();

            foreach (Employee item in lvEmployeeList.Items)
            {
                if (item.EmployeeId == employee2.EmployeeId)
                {
                    lvEmployeeList.SelectedIndex = lvEmployeeList.Items.IndexOf(item);
                    break;
                }
            }

            MessageBox.Show(
                "Xóa thành công.",
                "Info",
                MessageBoxButton.OK,
                MessageBoxImage.Information
                );
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (txtId.Text == string.Empty)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Vui lòng chọn thông tin cần chỉnh sửa.",
                    "Warning",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                    );
            }
            else if (
                txtPhone.Text == string.Empty ||
                txtRevenueOrAllowance.Text == string.Empty)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Vui lòng điền đầy đủ thông tin.",
                    "Warning",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                    );
            }
            else
            {
                updateToDatabase();
            }
        }

        private void updateToDatabase()
        {
            Employee employee = lvEmployeeList.SelectedItem as Employee;
            string employeeId = employee.EmployeeId;

            string gender = string.Empty;
            string employeeType = string.Empty;
            if (radBoy.IsChecked == true)
            {
                gender = "Nam";
            }
            else if (radGirl.IsChecked == true)
            {
                gender = "Nữ";
            }
            if (radSaler.IsChecked == true)
            {
                employeeType = "Saler";
            }
            else if (radShipper.IsChecked == true)
            {
                employeeType = "Shipper";
            }

            SqlConnection connection = new SqlConnection(connectionString);
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string sql =
                "UPDATE Employee SET Gender = @gender, PhoneNumber = @phone, EmployeeType = @empType, RevenueOrAllowance = @revOrAll, UpdatedDate = @updatedDate WHERE EmployeeId = @empId";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.Add("@gender", SqlDbType.NVarChar).Value = gender;
            command.Parameters.Add("@phone", SqlDbType.NVarChar).Value = txtPhone.Text;
            command.Parameters.Add("@empType", SqlDbType.NVarChar).Value = employeeType;
            command.Parameters.Add("@revOrAll", SqlDbType.Float).Value = float.Parse(txtRevenueOrAllowance.Text);
            command.Parameters.Add("@updatedDate", SqlDbType.DateTime).Value = DateTime.Now;
            command.Parameters.Add("@empId", SqlDbType.NVarChar).Value = employeeId;
            command.ExecuteNonQuery();
            connection.Close();

            LoadEmployeeToListView();

            foreach (Employee item in lvEmployeeList.Items)
            {
                if (item.EmployeeId == employee.EmployeeId)
                {
                    lvEmployeeList.SelectedIndex = lvEmployeeList.Items.IndexOf(item);
                    break;
                }
            }

            MessageBox.Show(
                "Sửa thành công.",
                "Info",
                MessageBoxButton.OK,
                MessageBoxImage.Information
                );
        }

        private void btnArrange_Click(object sender, RoutedEventArgs e)
        {
            lvEmployeeList.Items.SortDescriptions.Add(new SortDescription("Seniority5Years", ListSortDirection.Descending));
            lvEmployeeList.Items.SortDescriptions.Add(new SortDescription("EmployeeName", ListSortDirection.Ascending));
        }

        private void btnStatistic_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            int sumSaler = 0;
            int sumShipper = 0;
            float sumRevenue = 0;
            float sumAllowance = 0;
            foreach (Employee employee in lvEmployeeList.Items)
            {
                if (employee.EmployeeType == "Saler")
                {
                    sumSaler ++;
                    sumRevenue += employee.RevenueOrAllowance;
                }
                else
                {
                    sumShipper ++;
                    sumAllowance += employee.RevenueOrAllowance;
                }
            }

            stringBuilder.AppendLine("Tổng NV bán hàng: " + sumSaler.ToString());
            stringBuilder.AppendLine("Tổng NV giao nhận: " + sumShipper.ToString());
            stringBuilder.AppendLine("----------------");
            stringBuilder.AppendLine("Tổng doanh số: " + sumRevenue.ToString());
            stringBuilder.AppendLine("Tổng PC nhiên liệu: " + sumAllowance.ToString());

            MessageBox.Show(stringBuilder.ToString(), "Statistic", MessageBoxButton.OK, MessageBoxImage.Information);

        }
    }
}
