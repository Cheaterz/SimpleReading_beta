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
using System.Security.Cryptography;

//
/*
 * TODO:
 * класс Article?
 * добавление статей (или изменение тэгов, заметок и т.д.)
 * внешний вид статьи!!!!!!
 * плагин?
 * try-catch (таймаут)
 * функция filter?
 * 
 * отдельные функции коннекта, аутентификации и т.д.
 * на мскл сервере - юзер с правами доступа только к функциям проверки юзера
 * процедура check_tags?
 * 
 * админская часть
 * 
 * рассказать, почему я выбрал md5
 */

namespace Simple_Reading_client_beta
{
    public partial class Form1 : Form
    {
        SqlConnection conn = null;
        DataSet set = null;
        SqlDataAdapter da = null;
        UserHelper user = null;
        bool logged = false;

        public Form1()
        {
            InitializeComponent();
            plLogin.Dock = DockStyle.Fill;
            tbText.Dock = DockStyle.Fill;
            panel1.Dock = DockStyle.Fill;
            label1.ForeColor = Color.Red;
            tbText.BackColor = Color.AliceBlue;
            //tbText.ForeColor = Color.Beige;
            tbLink.ForeColor = Color.DarkSlateGray;
            tbLink.BackColor = Color.White;

            listView1.Columns.Add("Название статьи");
            listView1.Columns[0].Width = listView1.Width;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
        }

        private void btLogin_Click(object sender, EventArgs e)
        {
            try
            {
                login(tbLogin.Text, getMD5Hash(tbPassword.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private string getMD5Hash(string p)
        {
            //http://msdn.microsoft.com/en-us/library/system.security.cryptography.md5%28v=vs.110%29.aspx
            MD5 md5H = MD5.Create();
            byte[] data = md5H.ComputeHash(Encoding.UTF8.GetBytes(tbPassword.Text));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
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
            try
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
            lbCat.Text = cat;
            lbTags.Text = ah.Tags;
            tbLink.Text = ah.Link;
            lbDate.Text = ah.Date;
        }

        private void plLogin_VisibleChanged(object sender, EventArgs e)
        {
            if (!logged)
            {
                return;
            }
            listView1.Items.Clear();

            set = new DataSet();
            string cs = ConfigurationManager.ConnectionStrings["home"].ConnectionString;
            conn = new SqlConnection(cs);
            da = new SqlDataAdapter();

            //получим актуальные данные со всех таблиц
            getData(da, conn, set);

                int i = 0;
                foreach (DataRow row in set.Tables["articles"].Rows)
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

                    //linq2objects; заменить на EntityFramework или чистый ADO
                    var result = from info in set.Tables["Cats"].AsEnumerable().Distinct()
                                 where info.Field<int>("idarticle") == (listView1.Items[i].Tag as ArticleHelper).Id
                                 select new 
                                 {
                                    Category = info.Field<string>("title"),
                                    Tag = info.Field<string>("tag_title")
                                 };

                    foreach (var item in result)
                    {
                        (listView1.Items[i].Tag as ArticleHelper).Cat = item.Category;
                        (listView1.Items[i].Tag as ArticleHelper).Tags += item.Tag + ", ";
                    }

                    i++;
                }

                //достать тэги и категории
                //SqlCommand getCat = new SqlCommand("SELECT ac.idarticle, c.title, t.tag_title FROM categories c, tags t, articles_cats ac WHERE ac.idtag=t.id AND ac.idcat=c.id AND ac.idarticle=@p1", conn);
                //da.SelectCommand = getCat;
                //SqlParameter p1;
                //p1 = getCat.Parameters.Add("@p1", SqlDbType.Int);

                //foreach (ListViewItem lt in listView1.Items)
                //{
                //    p1.Value = (lt.Tag as ArticleHelper).Id;
                //    da = new SqlDataAdapter(getCat);
                //    da.Fill(set, "cats");
                //    (lt.Tag as ArticleHelper).Cat = set.Tables["cats"].Rows[0]["title"].ToString();
                //    if (set.Tables["cats"].Rows.Count == 1)
                //        (lt.Tag as ArticleHelper).Tags = set.Tables["cats"].Rows[0]["tag_title"].ToString();
                //    else
                //    {
                //        int cnt = set.Tables["cats"].Rows.Count;
                //        foreach (DataRow row in set.Tables["cats"].Rows)
                //        {
                //            (lt.Tag as ArticleHelper).Tags += row["tag_title"].ToString();
                //            if (cnt != cnt - 1)
                //                (lt.Tag as ArticleHelper).Tags += ", ";
                //            cnt++;
                //        }
                //    }
                //    //set.Tables["cats"].Clear();
                //}
                

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

        }

        private void getData(SqlDataAdapter da, SqlConnection conn, DataSet set)
        {
            SqlCommand getBook = new SqlCommand("SELECT id, title, article_text, iduser, convert(varchar(15),date_add,105) as 'date_add', link_original FROM articles WHERE iduser=" + user.Id, conn);
            da.SelectCommand = getBook;
            da = new SqlDataAdapter(getBook);
            da.Fill(set, "articles");

            SqlCommand getComments = new SqlCommand("SELECT * FROM notes WHERE iduser=" + user.Id, conn);
            da.SelectCommand = getComments;
            da = new SqlDataAdapter(getComments);
            da.Fill(set, "notes");

            SqlCommand getCat = new SqlCommand("SELECT ac.idarticle, c.title, t.tag_title FROM categories c, tags t, articles_cats ac WHERE ac.idtag=t.id AND ac.idcat=c.id", conn);
            da.SelectCommand = getCat;
            da = new SqlDataAdapter(getCat);
            da.Fill(set, "cats");

        }

        private void tbLink_Click(object sender, EventArgs e)
        {
            tbLink.SelectAll();
        }
    }
}
