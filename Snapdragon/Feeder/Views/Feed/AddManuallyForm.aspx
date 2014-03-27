<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="AddManuallyForm.aspx.cs" Inherits="Feeder.Views.Feed.AddManuallyForm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<!-- Begin Sidebar -->
<div id="sidebar">
    <div id="main_menu">
        <ul>
            <li><a href="/Feed/Index">Subscribed Feeds</a></li>
            <li><a href="/Feed/AddManuallyForm">Add Feeds By Url</a></li>            
            <li><a href="/Feed/AddOpmlForm">Add OPML</a></li>                                                
        </ul>
    </div>
</div>
<!-- End Sidebar -->
<div id="content">
<h3>Add Feeds</h3>
<form method="post" action="/Feed/AddManually/">
    <table>
        <tr>
            <td>Enter Feed URL: </td><td><input id="feedUrl1" name="feedUrl1" type="text" /></td>
        </tr>
        <tr>
            <td>Enter Feed URL: </td><td><input id="feedUrl2" name="feedUrl2" type="text" /></td>
        </tr>
        <tr>
            <td>Enter Feed URL: </td><td><input id="feedUrl3" name="feedUrl3" type="text" /></td>
        </tr>
        <tr>
            <td>Enter Feed URL: </td><td><input id="feedUrl4" name="feedUrl4" type="text" /></td>
        </tr>
        <tr>
            <td>Enter Feed URL: </td><td><input id="feedUrl5" name="feedUrl5" type="text" /></td>
        </tr>
    </table>
    <input type="submit" value="Add" />
</form>
</div>
</asp:Content>
