<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="AddOpmlForm.aspx.cs" Inherits="Feeder.Views.Feed.AddOpmlForm" %>
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
<h3>Add Feeds Using OPML</h3>
<form method="post" enctype="multipart/form-data" action="/Feed/AddOpml/"    >
    <table>
        <tr>
            <td>Add OPML file: </td><td><input type="file" name="opmlFile" id="opmlFile" /></td><br />                        
        </tr>        
    </table>
    <input type="submit" value="Add" />
</form>
</div>
</asp:Content>
