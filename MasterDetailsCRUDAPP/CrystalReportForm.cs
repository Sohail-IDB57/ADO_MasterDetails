using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MasterDetailsCRUDAPP.Report
{
    public partial class CrystalReportForm : Form
    {
        SqlConnection con = new SqlConnection(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=TarineeDB;Integrated Security=True");
        public CrystalReportForm()
        {
            InitializeComponent();
        }

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {
            con.Open();
            string q = "select * from Trainee;";
            SqlCommand cmd = new SqlCommand(q, con);
            SqlDataAdapter adap = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adap.Fill(ds, "Trainee");
            CrystalReport1 cr1 = new CrystalReport1();
            cr1.SetDataSource(ds);
            crystalReportViewer1.ReportSource = cr1;
            con.Close();
            crystalReportViewer1.Refresh();
            con.Close();




        }
    }
}
