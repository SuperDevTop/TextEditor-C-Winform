using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Text_Editor
{
    public partial class Register : Form
    {
        public Register()
        {
            InitializeComponent();
        }

        private void Register_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool isExisted = false;
            List<String> userList;

            userList = new List<string>();

            string path = AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\" + "userlist.txt";
            StreamReader reader = new StreamReader(path);
            string readStr = reader.ReadToEnd();
            string temp = "";

            for(int i = 0; i < readStr.Length; i++)
            {
                if (readStr[i] == '\n')
                {
                    temp = "";
                    continue;
                }

                if (readStr[i] == ',')
                {
                    userList.Add(temp);
                    temp = "";
                    continue;
                }             

                temp += readStr[i];
            }

            reader.Close();

            for(int i = 0; i < userList.Count; i++)
            {
                if(userList[i] == textBox1.Text)
                {
                    isExisted = true;
                    break;
                }
            }

            if (textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "" && textBox4.Text != "" && textBox5.Text != "" && textBox6.Text != "")
            {
                if (!isExisted)
                {
                    if(textBox2.Text == textBox3.Text)
                    {
                        WriteString();

                        Login login = new Login();
                        login.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Please enter password corretly.");
                    }
                }
                else
                {
                    MessageBox.Show("Registered.");
                }                                                                
            }
            else
            {
                MessageBox.Show("Please fill in all fields.");
            }                        
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Hide();
        }

        private void Register_FormClosing(object sender, FormClosingEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Hide();
        }

        public void WriteString()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\" + "login.txt";
            string writeTemp = "";
            StreamWriter writer = new StreamWriter(path, true);
            writeTemp += textBox1.Text + "," + textBox2.Text + "," + comboBox1.Text + "," + textBox4.Text + "," + textBox5.Text + "," + textBox6.Text;

            writer.WriteLine(writeTemp);
            writer.Close();

            path = AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\" + "userlist.txt";
            writeTemp = "";
            writer = new StreamWriter(path, true);
            writeTemp += textBox1.Text + "," + textBox2.Text + ":" + comboBox1.Text + "+";

            writer.WriteLine(writeTemp);
            writer.Close();
        }
    }
}
