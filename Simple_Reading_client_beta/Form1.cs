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

//
/*
 * TODO:
 * класс Article?
 * добавление статей (или изменение тэгов, заметок и т.д.)
 * внешний вид статьи!!!!!!
 * плагин?
 * try-catch (таймаут)
 * 
 * отдельные функции коннекта, аутентификации и т.д.
 * на мскл сервере - юзер с правами доступа только к функциям проверки юзера
 * процедура check_tags?
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
        UserHelper user = null;
        bool logged = false;

        public Form1()
        {
            InitializeComponent();
            plLogin.Dock = DockStyle.Fill;
            tbText.Dock = DockStyle.Fill;
            panel1.Dock = DockStyle.Fill;
            label1.ForeColor = Color.Red;
            listView1.Columns.Add("Название статьи");
            listView1.Columns[0].Width = listView1.Width;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
        }

        private void btLogin_Click(object sender, EventArgs e)
        {
            login(tbLogin.Text, tbPassword.Text);
        }

        private void login(string login, string pass)
        {
            string cs = ConfigurationManager.ConnectionStrings["home"].ConnectionString;
            conn = new SqlConnection(cs);

            string sql = @"SELECT dbo.check_user (@log, @passw)";
            SqlCommand comm = new SqlCommand(sql, conn);
            (comm as SqlCommand).Parameters.Add("@log", SqlDbType.VarChar).Value = login;
            (comm as SqlCommand).Parameters.Add("@passw", SqlDbType.VarChar).Value = pass;

            try
            {
                conn.Open();
                if ((int)comm.ExecuteScalar() == 1)
                {
                    sql = @"SELECT id FROM users WHERE ulogin = '" + login + "'";
                    comm = new SqlCommand(sql, conn);
                    user = new UserHelper((int)comm.ExecuteScalar());
                    logged = true;
                    plLogin.Visible = false;
                    this.MinimizeBox = true;
                    this.MaximizeBox = true;
                }
                else
                    label1.Text = "Пользователь не найден \nлибо пароль введен неправильно";
            }
            catch (Exception ex)
            {
                label1.Text = ex.Message;
            }
            finally
            {
                if(conn != null)
                    conn.Close();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (conn != null)
            {
                conn.Close();
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count < 1)
                return;
            ArticleHelper ah = (ArticleHelper)listView1.SelectedItems[0].Tag;
            tbText.Text = ah.Text;
            tbNotes.Text = ah.Notes;
            string cat = ah.Cat;
            lbCat.Text = "Категория: " + cat;
            lbTags.Text = "Теги: " + ah.Tags;
            lbLink.Text = ah.Link;
            lbDate.Text = ah.Date;
            //richTextBox1.Text = listView1.SelectedItems[0].Tag.ToString();
            //int i = (int)listView1.SelectedItems[0].Index;
            //MessageBox.Show(listView1.SelectedItems[0].Tag.ToString());
        }

        private void plLogin_VisibleChanged(object sender, EventArgs e)
        {
            if (!logged)
            {
                return;
            }
                //richTextBox1.BackColor = Color.AliceBlue;
                //richTextBox1.ForeColor = Color.Beige;
                listView1.Items.Clear();

                set = new DataSet();
                string cs = ConfigurationManager.ConnectionStrings["home"].ConnectionString;
                conn = new SqlConnection(cs);
                //da = new SqlDataAdapter("SELECT * FROM articles WHERE iduser="+uh.Id, conn);
                da = new SqlDataAdapter();
                //SqlCommandBuilder cmd = new SqlCommandBuilder(da);

                //получим актуальные данные со всех таблиц
                SqlCommand getBook = new SqlCommand("SELECT id, title, article_text, iduser, convert(varchar(15),date_add,105) as 'date_add', link_original FROM articles WHERE iduser=" + user.Id, conn);
                da.SelectCommand = getBook;
                da = new SqlDataAdapter(getBook);
                da.Fill(set, "book");

                SqlCommand getComments = new SqlCommand("SELECT * FROM notes WHERE iduser=" + user.Id, conn);
                da.SelectCommand = getComments;
                da = new SqlDataAdapter(getComments);
                da.Fill(set, "notes");

                int i = 0;
                foreach (DataRow row in set.Tables["book"].Rows)
                {
                    string note = "";
                    foreach (DataRow rowNote in set.Tables["notes"].Rows)
                    {
                        if ((int)rowNote["idarticle"] == (int)row["id"])
                        {
                            note = rowNote["note_text"].ToString();
                        }
                    }
                    listView1.Items.Add(row["title"].ToString());
                    listView1.Items[i].Tag = new ArticleHelper(row["article_text"].ToString(), note);
                    (listView1.Items[i].Tag as ArticleHelper).Id = (int)row["id"];
                    (listView1.Items[i].Tag as ArticleHelper).Link = row["link_original"].ToString();
                    (listView1.Items[i].Tag as ArticleHelper).Date = row["date_add"].ToString();
                    i++;
                }

                //достать тэги и категории
                SqlCommand getCat = new SqlCommand("SELECT distinct c.title FROM categories c, articles_cats ac WHERE c.id = ac.idcat AND ac.idarticle=@p1", conn);
                da.SelectCommand = getComments;
                SqlParameter p1;
                p1 = getCat.Parameters.Add("@p1", SqlDbType.Int);
                i = 0;
                foreach (ListViewItem it in listView1.Items)
                {
                    p1.Value = (listView1.Items[i].Tag as ArticleHelper).Id;
                    da = new SqlDataAdapter(getCat);
                    da.Fill(set, "cats");
                    (listView1.Items[i].Tag as ArticleHelper).Cat = set.Tables["cats"].Rows[0]["title"].ToString();
                    i++;
                    set.Tables["cats"].Clear();
                }
                SqlCommand getTag = new SqlCommand("SELECT t.tag_title FROM tags t, articles_cats ac WHERE t.id = ac.idtag AND ac.idarticle=@p1", conn);
                da.SelectCommand = getTag;
                p1 = getTag.Parameters.Add("@p1", SqlDbType.Int);
                i = 0;
                foreach (ListViewItem it in listView1.Items)
                {
                    p1.Value = (listView1.Items[i].Tag as ArticleHelper).Id;
                    da = new SqlDataAdapter(getTag);
                    da.Fill(set, "cats");
                    foreach (DataRow row in set.Tables["cats"].Rows)
                    {
                        (listView1.Items[i].Tag as ArticleHelper).Tags += row["tag_title"] + ", ";
                    }
                    i++;
                    set.Tables["cats"].Clear();
                }

                //SqlCommand getComments = new SqlCommand("SELECT * FROM comments WHERE iduser=" + uh.Id, conn);
                //da.SelectCommand = getComments;
                //da = new SqlDataAdapter(getComments);
                //da.Fill(set, "comments");

                //foreach (DataRow row in set.Tables["comments"].Rows)
                //{
                //    MessageBox.Show(row["comment_text"].ToString());
                //}
                //foreach (DataRow row in set.Tables["book"].Rows)
                //{
                //    MessageBox.Show(row["title"].ToString());
                //}

                table = set.Tables["book"];
                //var q = from t in table.AsEnumerable()
                //        where t.Field<int>("iduser") == 1
                //        select new 
                //        {
                //            Title = t.Field<string>("title"),
                //            Text = t.Field<string>("article_text"),
                //        };

                //int i = 0;
                //foreach (DataRow row in set.Tables["book"].Rows)
                //{
                //    listView1.Items.Add(row["title"].ToString());
                //    listView1.Items[i].Tag = new ArticleHelper(row["article_text"].ToString());//, row["Notes"].ToString());
                //    //listView1.Items[i].Tag = row["article_text"].ToString();
                //    //SubItems.Add(row["iduser"].ToString());
                //    i++;
                //    //MessageBox.Show(row["title"].ToString());
                //}

                //var res = from n in set.Tables[0].Rows
                //          select n;

                //int i = 0;
                //foreach (var article in q)
                //{
                //    listView1.Items.Add(article.Title);
                //    listView1.Items[i].Tag = article.Text;
                //    i++;
                //}
            //}
        }
    }
}
