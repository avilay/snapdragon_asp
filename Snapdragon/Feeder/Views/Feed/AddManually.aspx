<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="AddManually.aspx.cs" Inherits="Feeder.Views.Feed.AddManually" %>
<%@ Import Namespace="Feeder.Models" %>
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
    <% }
       else {  %>
       No new feeds were found and added.
       
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
    
</div>
</asp:Content>
