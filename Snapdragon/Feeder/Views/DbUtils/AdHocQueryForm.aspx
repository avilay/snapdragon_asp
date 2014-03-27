<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>AdHocQueryForm</h2>
    <form method="post" action="/DbUtils/AdHocQuery/">
    <input id="tableName" name="tableName" type="text" /><br />
    <textarea id="query" name="query" cols="40" rows="20"></textarea><br />    
    <input id="submit" type="submit" value="Submit" />
    </form>
</asp:Content>
