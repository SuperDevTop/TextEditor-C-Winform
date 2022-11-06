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
    public partial class Login : Form
    {
        public static string myType = "";
        
        public Login()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool isExisted = false;
            string myPassword = "";
            myType = "";
            List<String> userList;
            List<String> password;
            List<String> userType;

            userList = new List<string>();
            password = new List<string>();
            userType = new List<string>();

            string path = AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\" + "userlist.txt";
            StreamReader reader = new StreamReader(path);
            string readStr = reader.ReadToEnd();
            string temp = "";

            for (int i = 0; i < readStr.Length; i++)
            {
                if(readStr[i] == '\n')
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

                if(readStr[i] == ':')
                {
                    password.Add(temp);
                    temp = "";
                    continue;
                }

                if (readStr[i] == '+')
                {
                    userType.Add(temp);
                    temp = "";
                    continue;
                }

                temp += readStr[i];
            }

            reader.Close();

            for (int i = 0; i < userList.Count; i++)
            {
                if (userList[i] == textBox1.Text.ToString())
                {                    
                    isExisted = true;
                    myPassword = password[i];
                    myType = userType[i];
                    break;
                }
            }

            if (textBox1.Text != "" && textBox2.Text != "")
            {
                if (isExisted)
                {
                    if (textBox2.Text == myPassword)
                    {
                        frmEditor frm = new frmEditor();
                        frm.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Invalid password.");
                    }
                }
                else
                {
                    MessageBox.Show("Not registered.");
                }                                
            }
            else
            {
                MessageBox.Show("Please fill in all fields.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Register register = new Register();
            register.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.ExitThread();
        }

        private void Login_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.ExitThread();
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }
    }
}
