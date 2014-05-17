using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

/*
 * TODO:
 * класс Article?
 * многотабличный запрос, чтобы вытащить комменты, дату, заголовок, текст, тэги, категорию и заметки
 * добавление статей (или изменение тэгов и т.д.)
 * внешний вид статьи!!!!!!
 * плагин?
 * 
 * отдельные функции коннекта, аутентификации и т.д.
 * на мскл сервере - юзер с правами доступа только к функциям проверки юзера
 * 
 * админская часть
 */

namespace Simple_Reading_client_beta
{
    public partial class Form1 : Form
    {
        SqlConnection conn = null;
        DataSet set = null;
        SqlDataAdapter da = null;
        DataTable table = null;

        public Form1()
        {
            InitializeComponent();
            label1.ForeColor = Color.Red;
            listView1.Columns.Add("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
            //listView1.Columns.Add("bbbbbbbbbbbbbbbbbbbb");
        }

        private void btLogin_Click(object sender, EventArgs e)
        {
            string cs = ConfigurationManager.ConnectionStrings["home"].ConnectionString;
            conn = new SqlConnection(cs);
            string login = tbLogin.Text;
            string pass = tbPassword.Text;

            string sql = @"SELECT dbo.check_user (@log, @passw)";
            SqlCommand comm = new SqlCommand(sql, conn);
            (comm as SqlCommand).Parameters.Add("@log", SqlDbType.VarChar).Value = login;
            (comm as SqlCommand).Parameters.Add("@passw", SqlDbType.VarChar).Value = pass;

            try
            {
                conn.Open();
                if ((int)comm.ExecuteScalar() == 1)
                    panel1.Visible = false;
                else
                    label1.Text = "Пользователь не найден \nлибо пароль введен неправильно";
            }
            catch (Exception ex)
            {
                label1.Text = ex.Message;
            }
            finally
            {
                conn.Close();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (conn != null) //|| conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //richTextBox1.BackColor = Color.AliceBlue;
            //richTextBox1.ForeColor = Color.Beige;
            listView1.Items.Clear();
            UserHelper uh = new UserHelper(1);

            set = new DataSet();
            string cs = ConfigurationManager.ConnectionStrings["home"].ConnectionString;
            conn = new SqlConnection(cs);
            //da = new SqlDataAdapter("SELECT * FROM articles WHERE iduser="+uh.Id, conn);
            da = new SqlDataAdapter();
            //SqlCommandBuilder cmd = new SqlCommandBuilder(da);
            SqlCommand getBook = new SqlCommand("SELECT * FROM articles WHERE iduser=" + uh.Id, conn);
            da.SelectCommand = getBook;
            da = new SqlDataAdapter(getBook);
            da.Fill(set, "book");

            SqlCommand getComments = new SqlCommand("SELECT * FROM comments WHERE iduser=" + uh.Id, conn);
            da.SelectCommand = getBook;
            da = new SqlDataAdapter(getBook);
            da.Fill(set, "book");


            table = set.Tables["book"];
            //var q = from t in table.AsEnumerable()
            //        where t.Field<int>("iduser") == 1
            //        select new 
            //        {
            //            Title = t.Field<string>("title"),
            //            Text = t.Field<string>("article_text"),
            //        };

            int i = 0;
            foreach (DataRow row in set.Tables["book"].Rows)
            {
                listView1.Items.Add(row["title"].ToString());
                listView1.Items[i].Tag = new ArticleHelper(row["article_text"].ToString());//, row["Notes"].ToString());
                //listView1.Items[i].Tag = row["article_text"].ToString();
                //SubItems.Add(row["iduser"].ToString());
                i++;
                //MessageBox.Show(row["title"].ToString());
            }

            //var res = from n in set.Tables[0].Rows
            //          select n;

            //int i = 0;
            //foreach (var article in q)
            //{
            //    listView1.Items.Add(article.Title);
            //    listView1.Items[i].Tag = article.Text;
            //    i++;
            //}

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count < 1)
                return;
            ArticleHelper ah = (ArticleHelper)listView1.SelectedItems[0].Tag;
            richTextBox1.Text = ah.Text;
            //tbNotes.Text = ah.Notes;
            //richTextBox1.Text = listView1.SelectedItems[0].Tag.ToString();
            //int i = (int)listView1.SelectedItems[0].Index;
            //MessageBox.Show(listView1.SelectedItems[0].Tag.ToString());
        }
    }
}
