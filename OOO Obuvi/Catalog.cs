using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOO_Obuvi
{
    public partial class Catalog : Form
    {
        string exePath = Application.StartupPath;
        public static string productArticle;
        int count;
        string sort, search;
        double[,] arrayDisc = new double[,] { { 0, 100 }, { 0, 10 }, { 10, 15 }, { 15, 100 } };
        int filterDisc, filterCat, countFilter;

        public Catalog()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }
        private void Catalog_Load(object sender, EventArgs e)
        {
            //Установка цвета
            tableLayoutHead.BackColor = ColorTranslator.FromHtml("#A5E887");
            tableLayoutPanelBottom.BackColor = ColorTranslator.FromHtml("#A5E887");
            btnBack.BackColor = ColorTranslator.FromHtml("#B3F4E9");
            btnWatchOrder.BackColor = ColorTranslator.FromHtml("#B3F4E9");
            btnWorkWithOrders.BackColor = ColorTranslator.FromHtml("#B3F4E9");
            btnAddNewProduct.BackColor = ColorTranslator.FromHtml("#B3F4E9");

            dataGridViewProducts.Font = new Font("Candara", 14, FontStyle.Regular);

            btnAddNewProduct.Visible = false;

            count = Helper.DbContext.Product.Count();

            comboBoxSort.Text = comboBoxSort.Items[0].ToString();
            comboBoxSale.Text = comboBoxSale.Items[0].ToString();
            comboType.Text = comboType.Items[0].ToString();

            labelFullName.Text = antefication.userFullName;
            labelCount.Text = "Количество записей: " + count.ToString();

            if (antefication.roleId == 1)
            {
                btnAddNewProduct.Visible = true;
            }
            showItems();
        }

        //Кнопка назад
        private void btnBack_Click(object sender, EventArgs e)
        {
            antefication.userFullName = "";
            var aunt = new antefication();
            this.Hide();
            aunt.ShowDialog();
            this.Close();
        }

        //ComboBox сортировки цены
        private void comboBoxSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            sort = "ASC";
            if (comboBoxSort.SelectedIndex == 1)
            {
                sort = "DESC";
            }
            showItems();
        }

        //Сортировка по скидке
        private void comboBoxSale_SelectedIndexChanged(object sender, EventArgs e)
        {
            filterDisc = comboBoxSale.SelectedIndex;
            showItems();
        }

        //Редактирование товара
        private void dataGridViewProducts_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (antefication.roleId == 1)
            {
                var red = MessageBox.Show("Вы точно хотите начать редактирование данного товара?", "Редактирование товара", MessageBoxButtons.YesNo);
                if (red == DialogResult.Yes)
                {
                    this.Hide();
                    var frm = new Redaction(productArticle = (string)dataGridViewProducts.Rows[e.RowIndex].Cells[2].Value);
                    frm.ShowDialog();
                    this.Close();
                }
                else
                {
                    return;
                }
            }
            else
                MessageBox.Show("Возможность редактирования и удаления товаров доступна только администратору!!!");
        }

        //Добавление нового товара
        private void btnAddNewProduct_Click(object sender, EventArgs e)
        {
            this.Hide();
            var frm = new Redaction("");
            frm.ShowDialog();
            this.Close();
        }

        //Сортировка по типу товара
        private void comboType_SelectedIndexChanged(object sender, EventArgs e)
        {
            filterCat = comboType.SelectedIndex;
            showItems();
        }

        //Поиск по названию
        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            search = textBoxSearch.Text;
            showItems();
        }

        //Вывод данных о продуктах из БД
        public void showItems()
        {
            var products = Helper.DbContext.Product.ToList();
            int row = 0;

            //Сортировка по убыванию возрастанию || цены
            if (sort == "ASC")
            {
                products = Helper.DbContext.Product.OrderBy(x => x.ProductCost).ToList();
            }
            else
            {
                products = Helper.DbContext.Product.OrderByDescending(x => x.ProductCost).ToList();
            }

            //Сортировка по скидке
            products = products.Where(x => (x.ProductMaxSale >= arrayDisc[filterDisc, 0]
                                         && x.ProductMaxSale <= arrayDisc[filterDisc, 1])).ToList();
            //Фильтр по типу
            if (filterCat != 0)
            {
                products = products.Where(x => x.CategoryId == filterCat).ToList();
            }

            //Поиск по названию
            if (!String.IsNullOrEmpty(search))
            {
                products = products.Where(x => x.ProductName.Contains(search)).ToList();
            }

            countFilter = products.Count();

            dataGridViewProducts.Rows.Clear();
            foreach (var item in products)
            {
                dataGridViewProducts.Rows.Add();
                //Добавление изображение товара (если есть)
                Bitmap bm;
                if (String.IsNullOrEmpty(products[row].ProductPhoto))
                {
                    bm = Properties.Resources.picture;
                }
                else 
                {
                    string s = exePath + @"\Import\" + products[row].ProductPhoto;
                    if (!File.Exists(s))
                    {
                        bm = Properties.Resources.picture;
                    }
                    else
                    {
                        bm = new Bitmap(s);
                    }
                }

                //Если скидка > 15%
                if (products[row].ProductMaxSale > 15) 
                {
                    dataGridViewProducts.Rows[row].DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#7fff00");
                }

                dataGridViewProducts.Rows[row].Cells[0].Value = bm;
                string desc = "Название: " + products[row].ProductName + Environment.NewLine;
                desc += "Описание товара: " + products[row].ProductDescription + Environment.NewLine;
                desc += "Производитель: " + products[row].Manufacture.ManufactureName + Environment.NewLine;
                desc += "Цена: " + products[row].ProductCost + Environment.NewLine; 
                
                dataGridViewProducts.Rows[row].Cells[1].Value = desc;
                dataGridViewProducts.Rows[row].Cells[2].Value = products[row].ProductArticle;
                dataGridViewProducts.Rows[row].Height = 140;

                row++;
            }
            labelCount.Text = countFilter + " из " + count;
        }
    }
}
