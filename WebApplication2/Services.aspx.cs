using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;
using Microsoft.EnterpriseManagement.Monitoring;
using System.Xml;
using System.Data;
using System.Net;
using System.Security;




namespace Services
{
    public partial class ServicesForm : System.Web.UI.Page
    {
        public static string username = "";
        public static string userpassword = "";
        public static string userdomain = "";
        public static string objectid = "";
        public static Boolean userchanged = false;
        string msserver = System.Configuration.ConfigurationManager.AppSettings["SCOMMSSERVER"];
        DataTable dt = new DataTable();


        protected void Page_Load(object sender, EventArgs e)
        {
            if(userchanged)
            {
                logedinuser.Text = ". Tasks will run with the user:" + userdomain + "\\" + username;
            }
            
            //Connect to SCOM
            ManagementGroup mg = new ManagementGroup(msserver);

            if (!(mg.IsConnected))
            {
                Server.Transfer("~/ErrorPage.aspx?Error=6");
            }

            string query = "DisplayName = '" + System.Configuration.ConfigurationManager.AppSettings["GetServices"] + "'";
            ManagementPackTaskCriteria taskCriteria = new ManagementPackTaskCriteria(query);
            IList<ManagementPackTask> tasks =
                mg.TaskConfiguration.GetTasks(taskCriteria);
            ManagementPackTask task = null;
            if (tasks.Count == 1)
                task = tasks[0];
            else
                Server.Transfer("~/ErrorPage.aspx?Error=2");

            // Get the agent class.
            query = "Name = 'Microsoft.Windows.Computer'";
            ManagementPackClassCriteria criteria = new ManagementPackClassCriteria(query);

            IList<ManagementPackClass> classes =
                mg.EntityTypes.GetClasses(criteria);
            if (classes.Count != 1)
                Server.Transfer("~/ErrorPage.aspx?Error=5");

            // Create a MonitoringObject list containing a specific agent (the 
            // target of the task).
            string fullAgentName;
            if (Request.QueryString["id"] == null && Request.QueryString["DisplayName"] == null)
                Server.Transfer("~/ErrorPage.aspx?Error=1");
            else if (Request.QueryString["id"] != null)
            {
                fullAgentName = Request.QueryString["id"];
                query = "Id = '" + fullAgentName + "'";
                objectid = fullAgentName;
            }
            else if (Request.QueryString["DisplayName"] != null)
            {
                fullAgentName = Request.QueryString["DisplayName"];
                query = "DisplayName = '" + fullAgentName + "'";
                objectid = fullAgentName;
            }
            else
            {
                //to be developed
            }
            
                List<MonitoringObject> targets = new List<MonitoringObject>();
                MonitoringObjectCriteria targetCriteria =
                    new MonitoringObjectCriteria(query, classes[0]);
                targets.AddRange(mg.EntityObjects.GetObjectReader<MonitoringObject>(targetCriteria, ObjectQueryOptions.Default));
                if (targets.Count != 1)
                    Server.Transfer("~/ErrorPage.aspx?Error=3");
            
            //Get user filter options
            Microsoft.EnterpriseManagement.Runtime.TaskConfiguration config = new Microsoft.EnterpriseManagement.Runtime.TaskConfiguration();

            try
            {
                IList<Microsoft.EnterpriseManagement.Configuration.ManagementPackOverrideableParameter> overrideparams = task.GetOverrideableParameters();
                //user Service State Filter
                string State = Request.QueryString["state"];
                State = $"($_.startmode -match \"{State}\")";
                //user Service Status filter
                string Status = Request.QueryString["status"];
                Status = $"($_.state -match \"{Status}\")";
                //user Service Name like filter
                string ServiceName = Request.QueryString["name"];
                //user Service sorting
                string Sort = Request.QueryString["sort"];
                string overridevalue = "";
                if (ServiceName != null)
                    {
                        overridevalue = $"gwmi -Query \"select * from win32_service where name like '%{ServiceName}%'\" | select displayname,name,startmode,state";
                    }
                else
                    {
                        overridevalue = "gwmi -Query \"select * from win32_service\" | select displayname,name,startmode,state";
                    }

                if ((Request.QueryString["state"] != null) && (Request.QueryString["status"]) != null)
                {
                    overridevalue += "| ?{" + State + " -and " + Status + "}";
                }
                else if ((Request.QueryString["state"] != null))
                {
                    overridevalue += "| ?{" + State + "}";
                }
                else if ((Request.QueryString["status"] != null))
                {
                    overridevalue += "| ?{" + Status + "}";
                }
                else
                {
                    //do nothing
                }


                if ((Request.QueryString["sort"] != null))
                {
                    string sort = Request.QueryString["sort"];
                    sort = sort.Replace("|", ",");
                    overridevalue = overridevalue + "| sort " + sort;
                }
                else
                {
                    overridevalue = overridevalue + "| sort-object -property @{Expression = {$_.State}; Ascending = $false},startmode ,name";
                }


                config.Overrides.Add(new Pair<ManagementPackOverrideableParameter, string>(overrideparams[1], overridevalue));

            }
            catch
            {
                Server.Transfer("~/ErrorPage.aspx?Error=7");
            }
            // Run the task.

            try
            {
                IList<Microsoft.EnterpriseManagement.Runtime.TaskResult> results =
                    mg.TaskRuntime.ExecuteTask(targets, task, config);
                if (results.Count == 0)
                    Server.Transfer("~/ErrorPage.aspx?Error=4");


            // Display the task results.
            int resultNo = 0;
            foreach (Microsoft.EnterpriseManagement.Runtime.TaskResult res in results)
            {
                resultNo++;
                if (res.Status == Microsoft.EnterpriseManagement.Runtime.TaskStatus.Failed)
                {
                }
                else
                {
                    //convert task result
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(res.Output);
                    string xPathQry = @"/DataItem/Description";
                    System.Xml.XmlNode TaskDescription = xmlDoc.SelectSingleNode(xPathQry);
                    string Servicelist = TaskDescription.OuterXml.ToString();
                    Servicelist = Servicelist.Replace("<Description>", "").Replace("</Description>", "");
                    // put result in table
                    //DataTable dt = new DataTable();
                    dt.Columns.AddRange(new DataColumn[4] {
                        new DataColumn("Name", typeof(string)),
                        new DataColumn("DisplayName", typeof(string)),
                        new DataColumn("StartType", typeof(string)),
                        new DataColumn("Status", typeof(string))
                    });
                    

                    

                    //Read the contents of CSV file.
                    string csvData = Servicelist;
                    
                    string test = csvData.Split('\r')[0].ToString();

                    if (test == "<Description />")
                    {
                        Server.Transfer("~/ErrorPage.aspx?Error=7");
                    }


                    string[] stringSeparators = new string[] { "\r\n" };
                    
                    foreach (string row in csvData.Split(stringSeparators,StringSplitOptions.None))
                    {
                        int i = 0;
                        try
                        {
                            if (!string.IsNullOrEmpty(row))
                            {
                                dt.Rows.Add();
                                dt.Rows[dt.Rows.Count - 1][0] = row.Split(',')[0];
                                dt.Rows[dt.Rows.Count - 1][1] = row.Split(',')[1];
                                dt.Rows[dt.Rows.Count - 1][2] = row.Split(',')[2];
                                dt.Rows[dt.Rows.Count - 1][3] = row.Split(',')[3].Replace(";","");
                                
                                i++;
                            }
                            string privrew = row.ToString();
                        }
                        catch
                        {
                            
                        }
                    }

                    ServiceView.DataSource = dt;
                    ServiceView.DataBind();

                }
            }
            }
            catch
            {
                Server.Transfer("~/ErrorPage.aspx?Error=0");
            }
        }

        //Out put the service list to the website
        protected void CreateTable(object sender, GridViewRowEventArgs e)
        {
               
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.Cells[3].Text == "2" && e.Row.Cells[2].Text == "1")
                {
                    e.Row.Cells[3].ForeColor = System.Drawing.Color.White;
                    e.Row.Cells[3].BackColor = System.Drawing.Color.Red;
                    e.Row.Cells[3].Text = "Stopped";
                }
                else if (e.Row.Cells[3].Text == "2" && e.Row.Cells[2].Text == "2")
                {
                    e.Row.Cells[3].ForeColor = System.Drawing.Color.Red;
                    e.Row.Cells[3].Text = "Stopped";
                }
                else if (e.Row.Cells[3].Text == "2" && e.Row.Cells[2].Text == "3")
                {
                    e.Row.Cells[3].ForeColor = System.Drawing.Color.Gray;
                    e.Row.Cells[3].Text = "Stopped";
                }
                else if (e.Row.Cells[3].Text == "1")
                {
                    e.Row.Cells[3].ForeColor = System.Drawing.Color.Green;
                    e.Row.Cells[3].Text = "Running";
                }
                else
                {
                    e.Row.Cells[3].ForeColor = System.Drawing.Color.Gray;
                }


                if (e.Row.Cells[2].Text == "2")
                {
                    e.Row.Cells[2].Text = "Manual";
                   e.Row.Cells[2].ForeColor = System.Drawing.Color.LightBlue;
                }
                else if (e.Row.Cells[2].Text == "1")
                {
                    e.Row.Cells[2].Text = "Automatic";
                   e.Row.Cells[2].ForeColor = System.Drawing.Color.Gold;
                }
                else if (e.Row.Cells[2].Text == "3")
                {
                   e.Row.Cells[2].Text = "Disabled";
                   e.Row.Cells[2].ForeColor = System.Drawing.Color.Gray;
                }
                else
                {
                    e.Row.Cells[2].Text = "Unknown";
                    e.Row.Cells[2].ForeColor = System.Drawing.Color.Gray;
                }



            }
            

        }

        //This is sending the Command in the button the user selected to SCOM, looking for the task
        protected void ButtonClick(object sender, GridViewCommandEventArgs e)
        {
            string query = "";
            if (e.CommandName == "startService" || e.CommandName == "stopService" || e.CommandName == "killService" || e.CommandName == "restartService" || e.CommandName == "Cred")
            {
               
                query = "DisplayName = '" + System.Configuration.ConfigurationManager.AppSettings[e.CommandName] + "'";

                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = ServiceView.Rows[index];
                string ServiceName = row.Cells[0].Text;
                ManagementGroup mg = new ManagementGroup(msserver);
                ManagementPackTaskCriteria taskCriteria = new ManagementPackTaskCriteria(query);
                IList<ManagementPackTask> tasks =
                    mg.TaskConfiguration.GetTasks(taskCriteria);
                ManagementPackTask task = null;
                if (tasks.Count == 1)
                    task = tasks[0];
                else
                    Server.Transfer("~/ErrorPage.aspx?Error=6");

                // Get the agent class.
                query = "Name = 'Microsoft.Windows.Computer'";
                ManagementPackClassCriteria criteria = new ManagementPackClassCriteria(query);

                IList<ManagementPackClass> classes =
                    mg.EntityTypes.GetClasses(criteria);
                if (classes.Count != 1)
                    Server.Transfer("~/ErrorPage.aspx?Error=5");

                // Create a MonitoringObject list containing a specific agent (the 
                // target of the task).
                if (Request.QueryString["id"] == null)
                    Server.Transfer("~/ErrorPage.aspx?Error=1");

                string fullAgentName = Request.QueryString["id"];

                List<MonitoringObject> targets = new List<MonitoringObject>();
                query = "Id = '" + fullAgentName + "'";
                MonitoringObjectCriteria targetCriteria =
                    new MonitoringObjectCriteria(query, classes[0]);
                targets.AddRange(mg.EntityObjects.GetObjectReader<MonitoringObject>(targetCriteria, ObjectQueryOptions.Default));
                if (targets.Count != 1)
                    Server.Transfer("~/ErrorPage.aspx?Error=3");

               

                // Use the default task configuration.
                
                Microsoft.EnterpriseManagement.Runtime.TaskConfiguration config = new Microsoft.EnterpriseManagement.Runtime.TaskConfiguration();

                if (userchanged)
                {
                    SecureString theSecureString = new NetworkCredential("", userpassword).SecurePassword;

                    Microsoft.EnterpriseManagement.Runtime.TaskCredentials cred = new Microsoft.EnterpriseManagement.Runtime.WindowsTaskCredentials(userdomain, username, theSecureString);
                    config.Credentials = cred;
                }
                IList<Microsoft.EnterpriseManagement.Configuration.ManagementPackOverrideableParameter> overrideparams = task.GetOverrideableParameters();
                config.Overrides.Add(new Pair<ManagementPackOverrideableParameter, string>(overrideparams[1], ServiceName));

                // Run the task.
                IList<Microsoft.EnterpriseManagement.Runtime.TaskResult> results =
                    mg.TaskRuntime.ExecuteTask(targets, task, config);
                if (results.Count == 0)
                    Server.Transfer("~/ErrorPage.aspx?Error=4");

            }
            Response.Redirect(Request.RawUrl);
        }

        protected void ServiceView_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}