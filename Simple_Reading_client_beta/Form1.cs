using System;
using System.Collections;
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
 * чистить базу от пустых строк, заметок?
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
        bool edited = false;

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
            byte[] data = md5H.ComputeHash(Encoding.UTF8.GetBytes(p));
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
                    conn.Close();
                    
                    conn.Open();
                    sql = @"SELECT title FROM categories";
                    comm = new SqlCommand(sql, conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        cbCat.Items.Add(reader.GetString(reader.GetOrdinal("title")));
                    }
                    reader.Close();
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
                if (edited)
                {
                    //http://msdn.microsoft.com/ru-ru/library/33y2221y(v=vs.110).aspx
                    SqlCommand updNotes = new SqlCommand(@"UPDATE notes SET note_text=@text WHERE idarticle=@ida");
                    da.UpdateCommand = updNotes;
                    updNotes.Connection = conn;
                    updNotes.Parameters.Add("@ida", SqlDbType.Int, 4, "idarticle");
                    
                    SqlParameter text = da.UpdateCommand.Parameters.Add("@text", SqlDbType.Text);
                    text.SourceColumn = "note_text";
                    text.SourceVersion = DataRowVersion.Current;

                    SqlCommand insNotes = new SqlCommand(@"insert into notes(idarticle, note_text, iduser, date_edit) values(@ida, @note, @idu, getdate())");
                    da.InsertCommand = insNotes;
                    insNotes.Connection = conn;
                    insNotes.Parameters.Add("@ida", SqlDbType.Int, 4, "idarticle");
                    insNotes.Parameters.Add("@idu", SqlDbType.Int, 4, "iduser");

                    SqlParameter notetext = da.InsertCommand.Parameters.Add("@note", SqlDbType.Text);
                    notetext.SourceColumn = "note_text";
                    notetext.SourceVersion = DataRowVersion.Current;

                    da.Update(set.Tables["notes"]);

                    //попытка апдейта через функцию с помощью нового адаптера

                    da = new SqlDataAdapter();
                    DataTable tbl = new DataTable();
                    tbl.Columns.Add("ida", typeof(int));
                    tbl.Columns.Add("cat", typeof(string));
                    foreach (ListViewItem lv in listView1.Items)
                    {
                        DataRow row = tbl.NewRow();
                        row["ida"] = (lv.Tag as ArticleHelper).Id;
                        row["cat"] = (lv.Tag as ArticleHelper).Cat;
                        tbl.Rows.Add(row);
                    }

                    SqlCommand upd = new SqlCommand("update_cats", conn);
                    upd.CommandType = CommandType.StoredProcedure;
                    da.SelectCommand = upd;
                    SqlParameter cat, ida;
                    cat = upd.Parameters.Add("@cat", SqlDbType.NVarChar, 100);
                    ida = upd.Parameters.Add("@ida", SqlDbType.Int);
                    //da.SelectCommand.Parameters.Add("@cat", SqlDbType.NVarChar);
                    foreach (DataRow r in tbl.Rows)
                    {
                        cat.Value = r["cat"];
                        ida.Value = r["ida"];
                        da = new SqlDataAdapter(upd);
                        da.Fill(tbl);
                    }


                    //то же самое, но для тегов
                    string[] delim = { ", ", "," };
                    foreach (ListViewItem lv in listView1.Items)
                    {
                        string[] tagz = (lv.Tag as ArticleHelper).Tags.Split(delim, System.StringSplitOptions.RemoveEmptyEntries);

                        foreach (string s in tagz)
                        {
                            DataRow[] foundRows;
                            foundRows = set.Tables["cats"].Select("tag_title = '" + s + "'");
                            if (foundRows.Length == 0)
                            {
                                DataRow r1 = set.Tables["cats"].NewRow();
                                r1["idarticle"] = (listView1.SelectedItems[0].Tag as ArticleHelper).Id;
                                r1["tag_title"] = s;
                                r1["title"] = (listView1.SelectedItems[0].Tag as ArticleHelper).Cat;
                                set.Tables["cats"].Rows.Add(r1);
                            }
                        }
                    }

                    foreach (ListViewItem lv in listView1.Items)
                    {
                        string[] tagz = (lv.Tag as ArticleHelper).Tags.Split(delim, System.StringSplitOptions.RemoveEmptyEntries);
                        int i = 0;
                        foreach (DataRow row in set.Tables["cats"].Rows)
                        {
                            if (!tagz.Contains(row["tag_title"]) && (int)row["idarticle"] == (lv.Tag as ArticleHelper).Id)
                            {
                                set.Tables["cats"].Rows[i]["title"] = "";
                            }
                            i++;
                        }
                    }

                    foreach (DataRow row in set.Tables["cats"].Rows)
                    {
                        da = new SqlDataAdapter();

                        SqlCommand updateTags = new SqlCommand("update_tags", conn);
                        updateTags.CommandType = CommandType.StoredProcedure;
                        da.SelectCommand = updateTags;
                        SqlParameter tag, idarticle, editmode;
                        editmode = updateTags.Parameters.Add("@mode", SqlDbType.NVarChar, 4);
                        tag = updateTags.Parameters.Add("@tag", SqlDbType.NVarChar, 100);
                        idarticle = updateTags.Parameters.Add("@idarticle", SqlDbType.Int);
                        tag.Value = row["tag_title"];
                        idarticle.Value = row["idarticle"];

                        if (row["title"] == "")
                        {
                            editmode.Value = "del";
                        }
                        else
                        {
                            editmode.Value = "upd";
                        }
                        da = new SqlDataAdapter(updateTags);
                        da.Fill(set.Tables["cats"]);
                    }
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
            cbCat.SelectedIndex = cbCat.FindString(ah.Cat);
            tbTags.Text = ah.Tags;
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
                    //просто потому, что так удобней
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

        //TextChanged не подошел, ибо он срабатывал после каждого ввода символа
        private void tbNotes_Leave(object sender, EventArgs e)
        {
            try
            {
                if ((listView1.SelectedItems[0].Tag as ArticleHelper).Notes != tbNotes.Text)
                {
                    (listView1.SelectedItems[0].Tag as ArticleHelper).Notes = tbNotes.Text;
                    edited = true;

                    DataRow[] r = set.Tables["notes"].Select("idarticle = " + (listView1.SelectedItems[0].Tag as ArticleHelper).Id);
                    if(r.Length == 1)
                    {
                        foreach (DataRow row in set.Tables["notes"].Rows)
                        {
                            if ((int)row["idarticle"] == (listView1.SelectedItems[0].Tag as ArticleHelper).Id)
                            {
                                row["note_text"] = (listView1.SelectedItems[0].Tag as ArticleHelper).Notes;
                            }
                    
                        }
                    }
                    else if (r.Length < 1)
                    {
                        DataRow r1 = set.Tables["notes"].NewRow();
                        r1["idarticle"] = (listView1.SelectedItems[0].Tag as ArticleHelper).Id;
                        r1["note_text"] = (listView1.SelectedItems[0].Tag as ArticleHelper).Notes;
                        r1["iduser"] = user.Id;
                        set.Tables["notes"].Rows.Add(r1);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cbCat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbCat.Text != "")
            {
                (listView1.SelectedItems[0].Tag as ArticleHelper).Cat = cbCat.SelectedItem.ToString();
                edited = true;
            }
        }

        private void tbTags_Leave(object sender, EventArgs e)
        {
            if ((listView1.SelectedItems[0].Tag as ArticleHelper).Tags != tbTags.Text)
            {
                (listView1.SelectedItems[0].Tag as ArticleHelper).Tags = tbTags.Text;
                edited = true;
            }
        }
    }
}
