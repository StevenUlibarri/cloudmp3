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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cloudmp3.Windows
{
    /// <summary>
    /// Interaction logic for Create.xaml
    /// </summary>
    public partial class CreateNum : Window
    {
        public CreateNum()
        {
            InitializeComponent();
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
            this.Close();
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
