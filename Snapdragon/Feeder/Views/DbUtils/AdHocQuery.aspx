<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<% 
    string[] propertyNames = ViewData["propertyNames"] as string[];
    List<string[]> propertiesList = ViewData["propertiesList"] as List<string[]>;
%>
    <h2>AdHocQuery</h2>
    <table border="1">
        <tr>
        <% foreach( string propName in propertyNames ) { %>
            <th><%= propName %></th>        
        <% } %>
        </tr>
        
        <% foreach( string[] properties in propertiesList ) {  %>
        <tr>
            <% foreach( string prop in properties ) { %>
            <td><%= prop %></td>        
            <% } %> 
        </tr>            
        <% } %>
    
    </table>
    

</asp:Content>
