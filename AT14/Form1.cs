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

namespace AT14
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        SqlConnection myConnection = new SqlConnection("Server=.\\SQLEXPRESS; database=banque; Integrated security=SSPI");
        SqlCommand myCommand;
        SqlDataReader drd_banque;

        void remplirCombo()
        {
            myCommand = new SqlCommand("select *from compte", myConnection);
            try
            {
                myConnection.Open();
                drd_banque = myCommand.ExecuteReader();
                while (drd_banque.Read())
                {
                    comboBox1.Items.Add(drd_banque.GetSqlInt32(0));
                    comboBox2.Items.Add(drd_banque.GetSqlInt32(0));

                }
                comboBox1.SelectedIndex = 0;
                comboBox2.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                myConnection.Close();
            }

        }
        private void button1_Click(object sender, EventArgs e)
        {

            myConnection.Open();
            SqlTransaction myTrans = myConnection.BeginTransaction();
            myCommand = myConnection.CreateCommand();
            myCommand.Transaction = myTrans;
            try
            {
                myCommand.CommandText = $"update compte set solde =solde-{int.Parse(textBox1.Text)} where num_compte ={int.Parse(comboBox1.Text)}";
                myCommand.ExecuteNonQuery();
                MessageBox.Show("Crédit effectué", "Confirmation");
                myCommand.CommandText = $"update compte set solde =solde+{int.Parse(textBox1.Text)} where num_compte ={int.Parse(comboBox2.Text)}";
                myCommand.ExecuteNonQuery();
                MessageBox.Show("Débit effectué", "Confirmation");
                //----------------------------------------------------------------------
                myCommand.CommandText = $"insert into virement values({int.Parse(comboBox2.Text)},{int.Parse(comboBox1.Text)},{int.Parse(textBox1.Text)},'{DateTime.Today.ToString()}')";
                myCommand.ExecuteNonQuery();
                MessageBox.Show("Insertion effectué", "Confirmation");
                myTrans.Commit();

            }
            catch (SqlException ex)
            {
                myTrans.Rollback();
                MessageBox.Show(ex.Message,"Error");
            }
            finally
            {
                myConnection.Close();
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            remplirCombo();
        }
    }
}
