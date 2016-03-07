<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TIS.ScoringDaemon.Web.UI._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <h1>Configuration Settings</h1>
        <asp:Table id="Settings" BorderWidth="1" GridLines="Both" runat="server" >
        </asp:Table>
        
        <h1>Hubs Monitored</h1>
        <asp:Table id="HubInfo" BorderWidth="1" GridLines="Both" runat="server">
        </asp:Table>
        
        <h1>Item Scoring Callback</h1>
        <asp:Table id="CallBackInfo" BorderWidth="1" GridLines="Both" runat="server">
        </asp:Table>

    </form>
</body>
</html>