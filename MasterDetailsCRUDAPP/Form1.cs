using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using MasterDetailsCRUDAPP.Report;

namespace MasterDetailsCRUDAPP
{
    public partial class Form1 : Form
    {
        int inTraineeID = 0;
        bool isDefaultImage = true;
        string strConnectionString = "Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=TarineeDB;Integrated Security=True", strPreviousImage = "";
       
        OpenFileDialog ofd = new OpenFileDialog();
        public Form1()
        {
            InitializeComponent();
            lblTraineeID.Visible = false;
            txtTraineeID.Visible = false;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Clear();
            lblTraineeID.Visible = false;
            txtTraineeID.Visible = false;
        }

        void Clear()
        {
            txtTraineeID.Text = "";
            txtTraineeName.Text = "";
            cmbGender.SelectedIndex = 0;
            dtpDOB.Value = DateTime.Now;
            
            if (dgvPayment.DataSource == null)
                dgvPayment.Rows.Clear();
            else
                dgvPayment.DataSource = (dgvPayment.DataSource as DataTable).Clone();
            inTraineeID = 0;
            btnSave.Text = "Save";
            btnDelete.Enabled = false;
            pbxPhoto.Image = Image.FromFile(Application.StartupPath + "\\Images\\defaultImage.jpg");
            isDefaultImage = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'tarineeDBDataSet1.Course' table. 
            this.courseTableAdapter.Fill(this.tarineeDBDataSet1.Course);
            // TODO: This line of code loads data into the 'tarineeDBDataSet.Payment' table. 
            this.paymentTableAdapter.Fill(this.tarineeDBDataSet.Payment);
            CourseComboBoxFill();
            FillTraineeDataGridView();
            Clear();
        }

        void CourseComboBoxFill()
        {
            using (SqlConnection sqlCon = new SqlConnection(strConnectionString))
            {
                sqlCon.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("SELECT * FROM Course", sqlCon);
                DataTable dtbl = new DataTable();
                sqlDa.Fill(dtbl);
                DataRow topItem = dtbl.NewRow();
                topItem[0] = 0;
                topItem[1] = "-Select-";
                dtbl.Rows.InsertAt(topItem, 0);
                cmbCourse.ValueMember = dgvCmbCourse.ValueMember = "CourseID";
                cmbCourse.DisplayMember = dgvCmbCourse.DisplayMember = "Course";
                cmbCourse.DataSource = dtbl;
                dgvCmbCourse.DataSource = dtbl.Copy();
            }
        }

        private void btnImageBrowse_Click(object sender, EventArgs e)
        {
            ofd.Filter = "Images(.jpg,.png)|*.png;*.jpg";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                pbxPhoto.Image = new Bitmap(ofd.FileName);
                isDefaultImage = false;
                strPreviousImage = "";
            }
        }

        private void btnImageClear_Click(object sender, EventArgs e)
        {
            pbxPhoto.Image = new Bitmap(Application.StartupPath + "\\Images\\defaultImage.jpg");
            isDefaultImage = true;
            strPreviousImage = "";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateMasterDetailForm())
            {
                int _TraineeID = 0;
                using (SqlConnection sqlCon = new SqlConnection(strConnectionString))
                {
                    sqlCon.Open();
                    
                    SqlCommand sqlCmd = new SqlCommand("TraineeAddOrEdit", sqlCon);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@TraineeID", inTraineeID);
                    sqlCmd.Parameters.AddWithValue("@TraineeName", txtTraineeName.Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@CourseID", Convert.ToInt32(cmbCourse.SelectedValue));
                    sqlCmd.Parameters.AddWithValue("@DOB", dtpDOB.Value);
                    sqlCmd.Parameters.AddWithValue("@Gender", cmbGender.Text);
                    if (isDefaultImage)
                        sqlCmd.Parameters.AddWithValue("@ImagePath", DBNull.Value);
                    else if (inTraineeID > 0 && strPreviousImage != "")
                        sqlCmd.Parameters.AddWithValue("@ImagePath", strPreviousImage);
                    else
                        sqlCmd.Parameters.AddWithValue("@ImagePath", SaveImage(ofd.FileName));
                    _TraineeID = Convert.ToInt32(sqlCmd.ExecuteScalar());
                }
                
                using (SqlConnection sqlCon = new SqlConnection(strConnectionString))
                {
                    sqlCon.Open();
                    foreach (DataGridViewRow dgvRow in dgvPayment.Rows)
                    {
                        if (dgvRow.IsNewRow) break;
                        else
                        {
                            SqlCommand sqlCmd = new SqlCommand("PaymentAddOrEdit", sqlCon);
                            sqlCmd.CommandType = CommandType.StoredProcedure;
                            sqlCmd.Parameters.AddWithValue("@PaymentID", Convert.ToInt32(dgvRow.Cells["dgvtxtPaymentID"].Value == DBNull.Value ? "0" : dgvRow.Cells["dgvtxtPaymentID"].Value));
                            sqlCmd.Parameters.AddWithValue("@TraineeID", _TraineeID);
                            DateTime paymentDate;
                            if (DateTime.TryParse(dgvRow.Cells["dgvtxtPaymentDate"].Value.ToString(), out paymentDate))
                            {
                                sqlCmd.Parameters.AddWithValue("@PaymentDate", paymentDate);
                            }
                            else
                            {                               
                                sqlCmd.Parameters.AddWithValue("@PaymentDate", DBNull.Value);
                            }
                            sqlCmd.Parameters.AddWithValue("@CourseID", Convert.ToInt32(dgvRow.Cells["dgvCmbCourse"].Value == DBNull.Value ? "0" : dgvRow.Cells["dgvcmbCourse"].Value));
                            sqlCmd.Parameters.AddWithValue("@PaymentPurpose", dgvRow.Cells["dgvcmbPaymentPurpose"].Value == DBNull.Value ? "" : dgvRow.Cells["dgvcmbPaymentPurpose"].Value.ToString());
                            sqlCmd.Parameters.AddWithValue("@PaymentMethod", dgvRow.Cells["dgvcmbPaymentMethod"].Value == DBNull.Value ? "" : dgvRow.Cells["dgvcmbPaymentMethod"].Value.ToString());
                            sqlCmd.Parameters.AddWithValue("@Amount", Convert.ToInt32(dgvRow.Cells["dgvtxtPaymentFee"].Value == DBNull.Value ? "0" : dgvRow.Cells["dgvtxtPaymentFee"].Value));
                            sqlCmd.Parameters.AddWithValue("@Note", dgvRow.Cells["dgvtxtNote"].Value == DBNull.Value ? "" : dgvRow.Cells["dgvtxtNote"].Value.ToString());
                            sqlCmd.ExecuteNonQuery();
                        }
                    }
                }
                FillTraineeDataGridView();
                FillPaymentInfoDataGridView();
                Clear();
                MessageBox.Show("Submitted Successfully");
            }
        }

        bool ValidateMasterDetailForm()
        {
            bool _isValid = true;
            if (txtTraineeName.Text.Trim() == "")
            {
                MessageBox.Show("Trainee Name is required");
                _isValid = false;
            }
            
            return _isValid;
        }

        string SaveImage(string _imagePath)
        {
            string _fileName = Path.GetFileNameWithoutExtension(_imagePath);
            string _extension = Path.GetExtension(_imagePath);
            
            _fileName = _fileName.Length <= 15 ? _fileName : _fileName.Substring(0, 15);
            _fileName = _fileName + DateTime.Now.ToString("yymmssfff") + _extension;
            pbxPhoto.Image.Save(Application.StartupPath + "\\Images\\" + _fileName);
            return _fileName;
        }

        void FillTraineeDataGridView()
        {
            using (SqlConnection sqlCon = new SqlConnection(strConnectionString))
            {
                sqlCon.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("TraineeViewAll", sqlCon);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dtbl = new DataTable();
                sqlDa.Fill(dtbl);

                
                dgvTrainee.DataSource = dtbl;
                
                dgvTrainee.Columns["TraineeID"].Visible = true; 
                dgvTrainee.Columns["TraineeName"].HeaderText = "Name";               
                dgvTrainee.Columns["DOB"].HeaderText = "Date of Birth";
                  
                dgvTrainee.Columns["TraineeName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvTrainee.Columns["DOB"].DefaultCellStyle.Format = "yyyy-MM-dd";
            }

        }

        void FillPaymentInfoDataGridView()
        {
            using (SqlConnection sqlCon = new SqlConnection(strConnectionString))
            {
                sqlCon.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("PaymentViewAll", sqlCon);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dtbl2 = new DataTable();
                sqlDa.Fill(dtbl2);

                
                dgvPaymentInfo.DataSource = dtbl2;
                
            }

        }

        private void dgvTrainee_DoubleClick(object sender, EventArgs e)
        {
            if (dgvTrainee.CurrentRow.Index != -1)
            {
                DataGridViewRow _dgvCurrentRow = dgvTrainee.CurrentRow;
                inTraineeID = Convert.ToInt32(_dgvCurrentRow.Cells[0].Value);
                using (SqlConnection sqlCon = new SqlConnection(strConnectionString))
                {
                    sqlCon.Open();
                    SqlDataAdapter sqlDa = new SqlDataAdapter("TraineeViewByID", sqlCon);
                    sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                    sqlDa.SelectCommand.Parameters.AddWithValue("@TraineeID", inTraineeID);
                    DataSet ds = new DataSet();
                    sqlDa.Fill(ds);

                   
                    DataRow dr = ds.Tables[0].Rows[0];
                    lblTraineeID.Visible = true;
                    txtTraineeID.Visible = true;
                    txtTraineeID.Text = dr["TraineeID"].ToString();
                    txtTraineeName.Text = dr["TraineeName"].ToString();
                    cmbCourse.SelectedValue = Convert.ToInt32(dr["CourseID"].ToString());
                    dtpDOB.Value = Convert.ToDateTime(dr["DOB"].ToString());
                    
                    if (dr["ImagePath"] == DBNull.Value)
                    {
                        pbxPhoto.Image = new Bitmap(Application.StartupPath + "\\Images\\defaultImage.jpg");
                        isDefaultImage = true;
                    }
                    else
                    {
                        pbxPhoto.Image = new Bitmap(Application.StartupPath + "\\Images\\" + dr["ImagePath"].ToString());
                        strPreviousImage = dr["ImagePath"].ToString();
                        isDefaultImage = false;
                    }
                    dgvPayment.AutoGenerateColumns = false;
                    dgvPayment.DataSource = ds.Tables[1];
                    btnDelete.Enabled = true;
                    btnSave.Text = "Update";
                    tabControl.SelectedIndex = 0;
                }
            }
        }

        private void dgvTraineeTSP_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            DataGridViewRow dgvRow = dgvPayment.CurrentRow;
            if (dgvRow.Cells["dgvtxtPaymentID"].Value != DBNull.Value)
            {
                if (MessageBox.Show("Are You Sure to Delete this Record ?", "Master Detail CRUD", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    using (SqlConnection sqlCon = new SqlConnection(strConnectionString))
                    {
                        sqlCon.Open();
                        SqlCommand sqlCmd = new SqlCommand("PaymentDelete", sqlCon);
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@PaymentID", Convert.ToInt32(dgvRow.Cells["dgvtxtPaymentID"].Value));
                        sqlCmd.ExecuteNonQuery();
                    }
                }
                else
                    e.Cancel = true;
            }
        }


        private void Print_Click(object sender, EventArgs e)
        {
            CrystalReportForm cf = new CrystalReportForm();
            cf.Show();
        }

        private void btnAddCourse_Click(object sender, EventArgs e)
        {

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you Sure to Delete this Record ?", "Master Detail CRUD", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (SqlConnection sqlCon = new SqlConnection(strConnectionString))
                {
                    sqlCon.Open();
                    SqlCommand sqlCmd = new SqlCommand("TraineeDelete", sqlCon);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@TraineeID", inTraineeID);
                    sqlCmd.ExecuteNonQuery();
                    Clear();
                    FillTraineeDataGridView();
                    MessageBox.Show("Deleted Successfully");
                };
            }
        }
    }
}
