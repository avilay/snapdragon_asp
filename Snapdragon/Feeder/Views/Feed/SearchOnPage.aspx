<%@ Page Title="" Language="C#" Debug="true" MasterPageFile="~/Views/Shared/PopUp.Master" AutoEventWireup="true" CodeBehind="SearchOnPage.aspx.cs" Inherits="Feeder.Views.Feed.SearchOnPage" %>
<%@ Import Namespace="Feeder.Models" %>


<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
<form method="post" action="/Feed/AddPopUp/">
    <% 
        ICollection<Feed> newFeedsOnPage = (ICollection<Feed>)ViewData["newFeedsOnPage"];
        ICollection<Feed> oldFeedsOnPage = (ICollection<Feed>)ViewData["oldFeedsOnPage"];
        if( newFeedsOnPage.Count > 0 ) {
    %>
            <h3>New Feeds Found On This Page</h3>
            <div>
            <ul style="list-style: none">
            <%foreach( Feed feed in newFeedsOnPage ) { %>
                <li>
                    <input type="checkbox" name="<%= feed.Url %>" /><a href="<%= feed.Url %>"><%= feed.Title%></a>    <%= feed.Description%>
                </li> 
            <%} %>
            </ul>
            <input type="submit" value="Add Selected" />
            </div>
    <%
        }
        else {
    %>
           <h3>No New Feeds Found On This Page</h3>
    <%} %>    
        
    <%  if( oldFeedsOnPage.Count > 0 ) {%>
            <h3>The Following Feeds Had Already Been Added Previously:</h3>
            <div>Click <a href="#" onclick="javascript:(function(){document.getElementById('oldFeeds').style.visibility = 'visible';})()">here</a> 
                 to view feeds found on this page that you have already added.</div>
            <div id="oldFeeds" style="visibility: hidden;">
            <ul style="list-style: none">
            <%foreach( Feed feed in oldFeedsOnPage ) { %>
                <li>
                    <a href="<%= feed.Url %>"><%= feed.Title%></a>    <%= feed.Description%>
                </li> 
            <%} %>
            </ul>
            </div>
    <%  } %>
    
    <%  if( failures.Count > 0 ) {%>
            <h3>The Following Feeds Cannot Be Added: </h3>
            <div>Click <a href="#" onclick="javascript:(function(){document.getElementById('failures').style.visibility = 'visible';})()">here</a> 
                 to view feeds found on this page that cannot be added.</div>
            <div id="failures" style="visibility: hidden;">
            <ul style="list-style: none">
            <%foreach( Uri key in failures.Keys ) { %>
            <li>
                <a href="<%= key.ToString() %>"><%= key.ToString()%></a> <%= failures[key]%>
            </li> 
            <%} %>
            </ul>
            </div>
    <%  } %>
</form>
</asp:Content>
