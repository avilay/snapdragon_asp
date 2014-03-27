<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/PopUp.Master" AutoEventWireup="true" CodeBehind="AddPopUp.aspx.cs" Inherits="Feeder.Views.Feed.AddPopUp" %>
<%@ Import Namespace="Feeder.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <% ICollection<Feed> successes = ViewData["successes"] as ICollection<Feed>;
       if( successes.Count > 0 ) { %>
    <h3>The Following Feeds Were Added:</h3>
    <div>
        <ul>
        <%foreach( Feed feed in successes ) { %>
            <li>
                <a href="<%= feed.Url %>"><%= feed.Title%></a>    <%= feed.Description%>
            </li> 
        <%} %>
        </ul>
    </div>
    <% } %>
    
    <% ICollection<Feed> oldFeeds = ViewData["oldFeeds"] as ICollection<Feed>;
       if( oldFeeds.Count > 0 ) { %>
    <h3>The Following Feeds Had Already Been Added Previously:</h3>
    <div>
        <ul>
        <%foreach( Feed feed in oldFeeds ) { %>
            <li>
                <a href="<%= feed.Url %>"><%= feed.Title%></a>    <%= feed.Description%>
            </li> 
        <%} %>
        </ul>
    </div>
    <% } %>
    
    <% if( failures.Count > 0 ) { %>
    <h3>The Following Feeds Were Not Added:</h3>
    <div>
        <ul>
        <%foreach( Uri key in failures.Keys ) { %>
            <li>
                <a href="<%= key.ToString() %>"><%= key.ToString()%></a> <%= failures[key]%>
            </li> 
        <%} %>
        </ul>
    </div>
    <% } %>
    
</asp:Content>
