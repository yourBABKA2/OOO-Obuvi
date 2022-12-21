using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOO_Obuvi
{
    public partial class antefication : Form
    {
        string _login;
        string _password;
        bool showCapcha = false;
        string symbols = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm123456789";
        public static string userFullName;
        public static int roleId = 4;

        public antefication()
        {
            InitializeComponent();
            this.StartPosition= FormStartPosition.CenterScreen;
        }

        private void antefication_Load(object sender, EventArgs e)
        {
            tableLayoutHead.BackColor = ColorTranslator.FromHtml("#A5E887");
            btnExit.BackColor = ColorTranslator.FromHtml("#B3F4E9");
            btnGuest.BackColor = ColorTranslator.FromHtml("#B3F4E9");
            btnEnter.BackColor = ColorTranslator.FromHtml("#B3F4E9");

            showUsers();
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            _login = textBoxLogin.Text;
            _password = textBoxPassword.Text;

            var user = Helper.DbContext.User.Where(x => x.UserLogin == _login && x.UserPassword == _password).FirstOrDefault();

            if (textBoxLogin.Text == "" || textBoxPassword.Text == "")
            {
                MessageBox.Show("Не все данные пользователя введены");
                return;
            }

            if (user != null && showCapcha == false)
            {
                userFullName = user.UserFullName;
                roleId = user.RoleId;
                showCatalog();
                return;
            }

            if (user == null && showCapcha == false)
            {
                MessageBox.Show("Пользователь не найден");
                labelCapcha.Visible = textBoxCapcha.Visible = showCapcha = true;
                capcha();
                return;
            }
            else
            {
                if (user != null && labelCapcha.Text == textBoxCapcha.Text && showCapcha == true)
                {
                    userFullName = user.UserFullName;
                    roleId = user.RoleId;
                    showCatalog();
                }
                else
                {
                    MessageBox.Show("Данные неверны");
                    capcha();
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGuest_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Вы вошли в систему как гость");
            userFullName = "Гостевой аккаунт";
            showCatalog();
        }

        private void capcha() 
        {
            Random rnd = new Random();
            Bitmap bm = new Bitmap(labelCapcha.Width, labelCapcha.Height);
            Graphics g = Graphics.FromImage(bm);
            Pen p = new Pen(Color.Black);

            for (int i = 0; i < 2200; i++)
            {
                bm.SetPixel(rnd.Next(labelCapcha.Width), rnd.Next(labelCapcha.Height), Color.Black);
            }
            labelCapcha.Image = bm;

            string str = "";
            for (int i = 0; i < 4; i++)
            {
                str += symbols[rnd.Next(symbols.Length)].ToString();
            }
            labelCapcha.Text = str;

            g.DrawLine(p, new Point(0, 0), new Point(labelCapcha.Width, labelCapcha.Height));
            g.DrawLine(p, new Point(0, labelCapcha.Height), new Point(labelCapcha.Width, 0));
        }

        private void showUsers()
        {
            var users = Helper.DbContext.User.ToList();

            dataGridViewUsers.Rows.Clear();

            int row = 0;
            foreach(var items in users)
            {
                dataGridViewUsers.Rows.Add();

                dataGridViewUsers.Rows[row].Cells[0].Value = users[row].UserFullName;
                dataGridViewUsers.Rows[row].Cells[1].Value = users[row].UserLogin;
                dataGridViewUsers.Rows[row].Cells[2].Value = users[row].UserPassword;
                dataGridViewUsers.Rows[row].Cells[3].Value = users[row].Role.RoleName;
                row++;
            }
        }

        private void showCatalog() 
        {
            this.Hide();
            var frm = new Catalog();
            frm.ShowDialog();
            this.Close();
        }
    }
}
