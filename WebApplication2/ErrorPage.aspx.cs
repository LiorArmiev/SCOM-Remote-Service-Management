using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Services
{
    public partial class ErrorPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string ErrorId = Request.QueryString["Error"];
            if (ErrorId == "0")
                ErrorMsg.Text = "<h1>Failed to connect</h1><br>Failed to connect to the Management Server,<br>Check that the Port 5724 is open and that all SCOM services are running.<br>if its all running check webConfig file for the SCOMMSSERVER value";

            if (ErrorId == "1")
                ErrorMsg.Text = "<h1>No ID or DisplayName in input</h1><br>Missing Input Object Information,<br>You didn't specify the ?Id=&lt;Computer Object Id Guid or Computer DisplayName&gt;<br>Example:<br>http://localhost/GetServices.aspx?id=eeef6516-fa69-1852-ed97-477c599cc77a<br>http://localhost/GetServices.aspx?id=&lt;Computer Name&gt;";

            if (ErrorId == "2")
                ErrorMsg.Text = "<h1>Missing Task</h1><br>The Task is missing in SCOM.<br>Provide the right task name in the WebConfig file or check that the Tasks are in SCOM with the right name";

            if (ErrorId == "3")
                ErrorMsg.Text = "<h1>Object Missing</h1><br>The Object ID was not found in SCOM, check that the object exist.";

            if (ErrorId == "4")
                ErrorMsg.Text = "<h1>Task Failed to run</h1><br>Check that SCOM is working propely";

            if (ErrorId == "5")
                ErrorMsg.Text = "<h1>Oops!</h1><br>Missing Class,<br>Check that the class that hold the object exists";
            if (ErrorId == "6")
                ErrorMsg.Text = "<h1>Permission denied,</h1><br>The User does not have right permisions to connect to SCOM and run the task. Conntact SCOM Administrator and ask to have the task enabled in your User Role.";
            if (ErrorId == "7")
                ErrorMsg.Text = "<h1>Oops!</h1><br>No services matching your filter,<br>try to check the filters &Include=&lt;regex value&gt; &Sort=&lt;regex value&gt; &Status=&lt;regex value&gt;";
        }
    }
}