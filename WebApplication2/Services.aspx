<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Services.aspx.cs" Inherits="Services.ServicesForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="services" runat="server">
        <div>
            <asp:LoginView ID="LoginView1" runat="server">
                <AnonymousTemplate>
                    You can run with dif user
                </AnonymousTemplate>
                <LoggedInTemplate>
                    You are logged in as
                </LoggedInTemplate>
            </asp:LoginView>
            <asp:LoginName ID="LoginName1" runat="server" />
            <asp:Label ID="logedinuser" runat="server"></asp:Label>
            <asp:LoginStatus ID="LoginStatus1" runat="server" LogoutAction="Redirect" LogoutPageUrl="~/login.aspx" LogoutText="Change User" />
        </div>
        <asp:GridView ID="ServiceView" runat="server" Height="100%" Width="100%" AutoGenerateColumns="False" style="margin-right: 0px" onrowcommand="ButtonClick" OnRowDataBound="CreateTable" CellPadding="4" ForeColor="#333333" GridLines="None" AllowSorting="True" OnSelectedIndexChanged="ServiceView_SelectedIndexChanged">
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            <Columns>
                <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name"/>
                <asp:BoundField DataField="DisplayName" HeaderText="DisplayName" SortExpression="DisplayName"/>
                <asp:BoundField DataField="StartType" HeaderText="StartType" ItemStyle-HorizontalAlign="Center" SortExpression="StartType">
<ItemStyle HorizontalAlign="Center"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="Status" HeaderText="Status" ItemStyle-HorizontalAlign="Center" SortExpression="Status">
<ItemStyle HorizontalAlign="Center"></ItemStyle>
                </asp:BoundField>
                <asp:ButtonField ButtonType="Button" CommandName="startService" Text="Start" HeaderText="Start Service" ItemStyle-HorizontalAlign="Center" >
                <ControlStyle Width="100px" />
<ItemStyle HorizontalAlign="Center" Width="100px"></ItemStyle>
                </asp:ButtonField>
                <asp:ButtonField ButtonType="Button" CommandName="stopService" Text="Stop" HeaderText="Stop Service" ItemStyle-HorizontalAlign="Center">
                <ControlStyle Width="100px" />
<ItemStyle HorizontalAlign="Center" Width="100px"></ItemStyle>
                </asp:ButtonField>
                <asp:ButtonField ButtonType="Button" CommandName="killService" HeaderText="KillService" Text="Kill" >
                <ControlStyle Width="100px" />
                <ItemStyle Width="100px" />
                </asp:ButtonField>
                <asp:ButtonField ButtonType="Button" CommandName="restartService" HeaderText="Restart Service" Text="Restart" >
                <ItemStyle Width="100px" />
                </asp:ButtonField>
            </Columns>
            <EditRowStyle BackColor="#999999" />
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <SortedAscendingCellStyle BackColor="#E9E7E2" />
            <SortedAscendingHeaderStyle BackColor="#506C8C" />
            <SortedDescendingCellStyle BackColor="#FFFDF8" />
            <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
        </asp:GridView>
    </form>
</body>
</html>
