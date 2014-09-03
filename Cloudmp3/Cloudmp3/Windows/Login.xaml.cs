using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Cloudmp3.Windows
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public string UserName { get; private set; }
        public string Password { get; private set; }

        public Login()
        {
            InitializeComponent();
            UserNameBox.Focus();
            UserName = null;
            Password = null;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(UserNameBox.Text) && !string.IsNullOrEmpty(PasswordBox.Text)){
                UserName = UserNameBox.Text;
                Password = PasswordBox.Text;
                this.Close();
            }
            else
            {
                MessageBox.Show("User Name and Password must not be blank.");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CreateAcc_Click(object sender, RoutedEventArgs e)
        {
            ValidateUserName(this.UserNameBox.Text, this.PasswordBox.Text);   
        }
        public void ValidateUserName(string usrName, string pass)
        {
            using (var context = new CloudMp3SQLContext())
            {
                var query = (from u in context.Users
                             where u.U_UserName == usrName
                             select u).SingleOrDefault();
                if (query == null)
                {
                    User u = new User();
                    u.U_UserName = usrName;
                    u.U_Password = pass;
                    context.Users.Add(u);
                    context.SaveChanges();
                    MessageBox.Show("Account added");
                }
            }
        }
    }
}
