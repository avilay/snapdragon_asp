<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Logs</h2>
    <% IQueryable<string> logs = ViewData["logs"] as IQueryable<string>; %>
    <% foreach(string log in logs) { %>
           <p><%= log.Replace("\n", "<br />") %></p> 
    <% }; %>
</asp:Content>
