<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Feeder.Views.Feed.Index" %>
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
<h2>You are subscribed to the following feeds</h2>
<div style="float:left">To add feeds as you visit content websites, add &nbsp;&nbsp;</div>
<div style="margin:0px; padding:5px; float:left; background-color:#5c87b2; color: White">
<a style="color:White; text-decoration:none;" href="javascript:(function(){url='http://avilay.info/Feed/SearchOnPage?url='+encodeURIComponent(window.location.href); window.open(url,'Title','height=500,width=500,resizable=yes,scrollbars=yes,toolbar=yes,menubar=no,location=no,directories=no,status=yes')})()">
    Feeder
</a>
</div>
<div style="float:left">&nbsp;&nbsp;button to your favorites toolbar. (<a href="/Help/AddButton" title="How do I add to favorites?" >?</a>)</div>
<p>&nbsp;</p>
<ul>
<% foreach( Feed feed in (IEnumerable<Feed>)ViewData.Model ) {%>
<li>
<div>
<a href="<%=feed.ContentUrl %>"><%=feed.Title %></a> [<a style="font-size:smaller" href="/Feed/RemoveFeed/<%=feed.Id %>">Remove</a>]    <br />
<%=feed.Description %> <br />
<span style="font-size: small">Last checked: <%=feed.LastChecked %>. Last updated: <%=feed.LastPublished %></span><br />
</div>
</li>
<% } %>
</ul>
</div>
</asp:Content>
