<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<% string ver = ViewData["ver"] as string; %>

    <h2>TestAction</h2>
    <p>Version: <%= ver %></p>

</asp:Content>
