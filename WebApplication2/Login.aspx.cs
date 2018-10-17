using System;

namespace Services
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            ServicesForm.username = TextBox1.Text;
            ServicesForm.userpassword = TextBox2.Text;
            ServicesForm.userdomain = TextBox3.Text;
            ServicesForm.userchanged = true;
            string link = "~/Services.aspx?id=" + ServicesForm.objectid;
            Server.Transfer(link);
        }
    }
}