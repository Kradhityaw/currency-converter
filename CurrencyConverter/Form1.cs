using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CurrencyConverter
{
    public partial class Form1 : Form
    {
        CurrencyConverterEntities db = new CurrencyConverterEntities();
        public Form1()
        {
            InitializeComponent();
        }

        private void GetCrossExchangeRate()
        {
            int periodId = (int)comboBox1.SelectedValue;
            int fromCurrencyId = (int)comboBox2.SelectedValue;
            int toCurrencyId = (int)comboBox3.SelectedValue;

            decimal rate;
            decimal total;

            decimal amount = string.IsNullOrEmpty(textBox1.Text) ? 0 : Convert.ToDecimal(textBox1.Text);

            var fromCurrencyRate = db.USDExchangeRates
                .Where(f => f.currency_id == fromCurrencyId && f.period_id == periodId)
                .Select(f => f.rate)
                .FirstOrDefault();

            var toCurrencyRate = db.USDExchangeRates
                .Where(f => f.currency_id == toCurrencyId && f.period_id == periodId)
                .Select(f => f.rate)
                .FirstOrDefault();

            if (fromCurrencyId == 28)
            {
                // tidak usah dibagi dengan from karena patokannya sudah 1 USD
                rate = toCurrencyRate;
            }
            else if (toCurrencyId == 28)
            {
                // kalau ini kebalikannya, karena di database tidak ada patokan dari USD
                // karena seluruh patokan di tabel USDExchngeRate itu per 1 USD
                // karena patokan di tabelnya itu 1 USD itulah mengapa alasannya harus dibagikan dengan 1
                rate = 1 / fromCurrencyRate;
            }
            else
            {
                rate = toCurrencyRate / fromCurrencyRate;
            }

            total = amount * rate;

            textBox2.Text = total.ToString("F2");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            periodBindingSource.DataSource = db.Periods.ToList();
            currencyBindingSource.DataSource = db.Currencies.ToList();
            currencyBindingSource1.DataSource = db.Currencies.ToList();

            label3.Text = db.Currencies.FirstOrDefault(f => f.id == (int)comboBox2.SelectedValue).name;
            label4.Text = db.Currencies.FirstOrDefault(f => f.id == (int)comboBox3.SelectedValue).name;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            GetCrossExchangeRate();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem is Currency currency)
            {
                label3.Text = currency.name;
                GetCrossExchangeRate();
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedItem is Currency currency)
            {
                label4.Text = currency.name;
                GetCrossExchangeRate();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var cb2 = (int)comboBox2.SelectedValue;
            var cb3 = (int)comboBox3.SelectedValue;
            var tb1 = textBox1.Text;
            var tb2 = textBox2.Text;
            comboBox2.SelectedValue = cb3;
            comboBox3.SelectedValue = cb2;
            textBox1.Text = tb2;
            textBox2.Text = tb1;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetCrossExchangeRate();
        }
    }
}
