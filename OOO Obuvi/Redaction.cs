using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace OOO_Obuvi
{
    public partial class Redaction : Form
    {
        string productArticle;
        string exePath = Application.StartupPath;
        string photo, sourcePhoto = "";
        OpenFileDialog dlgOpenPhoto = new OpenFileDialog();

        public Redaction(string productArticle)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.productArticle = productArticle;
        }

        private void Redaction_Load(object sender, EventArgs e)
        {
            //Установка цвета
            tableLayoutHead.BackColor = ColorTranslator.FromHtml("#A5E887");
            btnBack.BackColor = ColorTranslator.FromHtml("#B3F4E9");
            btnAdd.BackColor = ColorTranslator.FromHtml("#B3F4E9");
            btnDelete.BackColor = ColorTranslator.FromHtml("#B3F4E9");
            btnDeleteProd.BackColor = ColorTranslator.FromHtml("#B3F4E9");
            btnSave.BackColor = ColorTranslator.FromHtml("#B3F4E9");
            btnCancel.BackColor = ColorTranslator.FromHtml("#B3F4E9");

            textBoxArticle.Text = productArticle;
            

            //ComboBox для производителя
            List<Entities.Manufacture> manufactures = Helper.DbContext.Manufacture.ToList();
            comboBoxManufacture.DataSource = manufactures;
            comboBoxManufacture.DisplayMember = "ManufactureName";
            comboBoxManufacture.ValueMember = "ManufactureId";

            //comboBoxManufacture.

            //ComboBox для категории
            List<Entities.Category> category = Helper.DbContext.Category.ToList();
            comboBoxCat.DataSource = category;
            comboBoxCat.DisplayMember = "CategoryName";
            comboBoxCat.ValueMember = "CategoryId";

            //ComboBox для поставщика
            List<Entities.Provider> provider = Helper.DbContext.Provider.ToList();
            comboBoxProvider.DataSource = provider;
            comboBoxProvider.DisplayMember = "ProviderName";
            comboBoxProvider.ValueMember = "ProviderId";

            dlgOpenPhoto.Filter = "JPG|*.jpg";

            //Если добавление новго товара
            if (productArticle == "")
            {
                labelName.Text = "Добавление нового товара";
                textBoxArticle.Text = textBoxCost.Text = textBoxCount.Text = textBoxSale.Text = textBoxName.Text = "";
                pictureBoxImg.Image = Properties.Resources.picture;
            }
            //Если редактирование товара
            else
            {
                textBoxArticle.Enabled = false;

                //Заполнение данными о товаре
                Entities.Product product = Helper.DbContext.Product.Where(x => x.ProductArticle == productArticle).FirstOrDefault();
                textBoxName.Text = product.ProductName;
                textBoxCost.Text = product.ProductCost.ToString();
                textBoxSale.Text = product.ProductSale.ToString();
                textBoxCount.Text = product.ProductCount.ToString();
                comboBoxManufacture.SelectedValue = product.ManufactureId;
                comboBoxCat.SelectedValue = product.CategoryId;
                comboBoxProvider.SelectedValue = product.ProviderId;
                richTextBoxDesc.Text = product.ProductDescription.ToString();
                photo = product.ProductPhoto;

                //Если изображения нет
                if (String.IsNullOrEmpty(product.ProductPhoto))
                {
                    pictureBoxImg.Image = Properties.Resources.picture;
                }
                //Если изображение есть
                else
                {
                    Bitmap bm = Properties.Resources.picture;
                    string s = exePath + @"\Import\" + photo;
                    if (!File.Exists(s))
                    {

                        pictureBoxImg.Image = bm;
                    }
                    else
                    {
                        pictureBoxImg.Load(s);
                    }
                }
            }
        }

        //Вернуться на предыдущу форму
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Hide();
            var frm = new Catalog();
            frm.ShowDialog();
            this.Close();
        }

        //Добавление фотографии
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (dlgOpenPhoto.ShowDialog() == DialogResult.OK)
            {
                sourcePhoto = dlgOpenPhoto.FileName;
                photo = textBoxArticle.Text + ".jpg";
                pictureBoxImg.Load(sourcePhoto);
            }
        }

        //Сохрание данных в БД
        private void btnSave_Click(object sender, EventArgs e)
        {
            //Запись артикула
            string article;
            article = textBoxArticle.Text;

            //Запись наименования
            string name;
            name = textBoxName.Text;

            //Запись производителя
            int manufacture;
            manufacture = (int)comboBoxManufacture.SelectedValue;

            //Запись поставщика
            int provider;
            provider = (int)comboBoxProvider.SelectedValue;

            //Запись категории
            int category;
            category = (int)comboBoxCat.SelectedValue;

            //Запись стоимости
            double cost;
            try
            {
                cost = Convert.ToDouble(textBoxCost.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("Стоимость должна быть введена в качестве числового значения!");
                textBoxCost.Clear();
                return;
            }

            if (cost < 0)
            {
                MessageBox.Show("Стоиость не может быть меньше 0!");
                textBoxCost.Clear();
                return;
            }

            //Запись скидки
            double disc;
            try
            {
                disc = Convert.ToDouble(textBoxSale.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("Стоимость должна быть введена в качестве числового значения!");
                textBoxSale.Clear();
                return;
            }

            if (disc < 0 || disc > 100)
            {
                MessageBox.Show("Скидка должна находится в диапазоне от 0% до 100%!");
                textBoxSale.Clear();
                return;
            }

            //Запись кол-ва товара на складе
            int count;
            try
            {
                count = Convert.ToInt32(textBoxCount.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("Кол-во товара должно быть задано в числовом формате!");
                textBoxCount.Clear();
                return;
            }

            if (count < 0)
            {
                MessageBox.Show("Кол-во товара не может быть отрицательным!");
                textBoxCount.Clear();
                return;
            }

            //Добавление новго товара
            Entities.Product product = null;
            if (String.IsNullOrEmpty(productArticle))
            {
                Entities.Product findProduct = Helper.DbContext.Product.Find(article);
                if (findProduct != null)
                {
                    MessageBox.Show("Такой артикль товара уже существует");
                    return;
                }
                product = new Entities.Product();
            }
            //Если товар уже существует
            else
            {
                product = Helper.DbContext.Product.Find(article);
            }

            //Заполнение полей товара
            product.ProductArticle = article;
            product.ProductName = name;
            product.UnitId = 1;
            product.ProductCost = cost;
            product.ProductMaxSale = disc;
            product.ManufactureId = manufacture;
            product.ProviderId = provider;
            product.CategoryId = category;
            product.ProductSale = disc;
            product.ProductCount = count;
            product.ProductDescription = richTextBoxDesc.Text;
            product.ProductPhoto = photo;

            if (!String.IsNullOrEmpty(photo))
            {
                string s = exePath + @"\Import\" + photo;
                if (File.Exists(s))
                {
                    File.Delete(s);
                }
                File.Copy(sourcePhoto, s);
            }

            if (String.IsNullOrEmpty(productArticle))
            {
                Helper.DbContext.Product.Add(product);
            }
            try
            {
                Helper.DbContext.SaveChanges();
                MessageBox.Show("Информация изменена");
                this.Close();
            }
            catch
            {
                MessageBox.Show("При внесении изменений возникла ошибка");
                return;
            }
        }

        //Удаление товара
        private void btnDeleteProd_Click(object sender, EventArgs e)
        {
            var delMsg = MessageBox.Show("Вы точно хотите удалить товар с артиклем " + productArticle +  "?", "Удаление товара", MessageBoxButtons.YesNo);
            if (delMsg == DialogResult.Yes)
            {
                Entities.Product prodDel = Helper.DbContext.Product.Find(productArticle);
                Helper.DbContext.Product.Remove(prodDel);
                try
                {
                    Helper.DbContext.SaveChanges();
                    MessageBox.Show("Товар успешно удалён");
                    this.Close();
                }
                catch
                {
                    MessageBox.Show("Товар нельзя удалить, так как он используется в заказах!");
                    return;
                }
            }
        }

        //Удаление фотографии
        private void btnDelete_Click(object sender, EventArgs e)
        {
            pictureBoxImg.Image = Properties.Resources.picture;
            photo = null;
        }
    }
}
