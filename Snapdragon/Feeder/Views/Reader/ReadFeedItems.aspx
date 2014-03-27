<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="ReadFeedItems.aspx.cs" Inherits="Feeder.Views.Reader.ReadFeedItems" %>
<%@ Import Namespace="Feeder.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<%
    IOrderedEnumerable<ItemUserDetails> sortedItems = (IOrderedEnumerable<ItemUserDetails>)ViewData["sortedItems"];
    Feed feed = (Feed)ViewData["feed"];
    IOrderedEnumerable<Feed> subscribedFeeds = (IOrderedEnumerable<Feed>)ViewData["subscribedFeeds"];
    int[] starts = (int[])ViewData["starts"];
    int[] ends = (int[])ViewData["ends"];
    int totPages = (int)ViewData["totPages"];
    int currentFeedId = (int)ViewData["currentFeedId"];
%>
<!-- Begin Sidebar -->
<div id="sidebar">
    <div id="main_menu">
        <ul>
            <li><a href="/Reader/ReadInterestingItems">Read Interesting Items</a></li>            
        </ul>
    </div>
    <div id="sub_menu">
        <h4>Subscribed Feeds</h4>
        <ul>
        <% foreach( Feed subscribedFeed in subscribedFeeds ) { %>
        <li style="list-style:none">
            <% if (subscribedFeed.Id == currentFeedId) { %>
                <a style="BACKGROUND-COLOR: #ffff00" title="Last updated on <%= subscribedFeed.LastPublished %> UTC" href="/Reader/ReadFeedItems/<%= subscribedFeed.Id %>">
                    <%= subscribedFeed.Title%> 
                </a>
            <% }
               else {%>
                <a title="Last updated on <%= subscribedFeed.LastPublished %> UTC" href="/Reader/ReadFeedItems/<%= subscribedFeed.Id %>">
                    <%= subscribedFeed.Title%> 
                </a> 
            <% } %>
        </li>                    
        <% } %>    
        </ul>
    </div>
</div>
<!-- End Sidebar -->


<div id="content">
<h2>Items from <%= feed.Title %></h2>
<p>Last published on <%= feed.LastPublished %> UTC and last updated on <%= feed.LastChecked %> UTC</p>

<%  for (int i = 0; i < totPages; i++) {
        string itemIds = "";
        for (int j = starts[i]; j < ends[i]; j++) {
            itemIds += sortedItems.ElementAt(j).ItemId.ToString() + ",";            
        }
        itemIds = itemIds.Trim(new char[] { ',' });
        if (i == 0) {%>
            <div id="page<%= i%>" itemids="<%= itemIds%>" style="display: block">
<%      }
        else {%>
            <div id="page<%= i%>" itemids="<%= itemIds%>" style="display: none">
<%      } %>
<%      if (i > 0) {%>
            <a href="#" onclick="showPage(<%= (i-1)%>)">Prev Page</a>
<%      } %>
Page <%= (i+1)%> of <%= totPages%> 
<%      if ( (i+1) < totPages) {%>
            <a href="#" onclick="showPage(<%= (i+1)%>)">Next Page</a>
<%      } %>

        <ul>
        <%
        for (int j = starts[i]; j < ends[i]; j++) {
            ItemUserDetails item = sortedItems.ElementAt(j);
        %>


<li style="list-style:none">
<% if (item.IsClicked) { %>
<div id="headline<%= item.ItemId %>" class="rounded_box headline clicked" onclick="showStory(<%= item.ItemId %>)">
<%}
   else if (item.IsRead) {%>
<div id="headline<%= item.ItemId %>" class="rounded_box headline read" onclick="showStory(<%= item.ItemId %>)">
<%} %>
<%else { %>
<div id="headline<%= item.ItemId %>" class="rounded_box headline" onclick="showStory(<%= item.ItemId %>)">
<%} %>
            <b class="rtop"> <b class="r1"></b> <b class="r2"></b> <b class="r3"></b> <b class="r4"></b> </b>
            <div class="rounded_box_content">
                <div class="item_date">
                    <%= item.PubDate.ToString("h:mm tt, MMM d, yy")%> UTC (Like-ness: <%= item.Score.ToString("F") %>)
                </div>
                <span class="title"><%= item.Title%></span>
                <span class="excerpt"><%= item.Excerpt%></span>
            </div>
            <b class="rbottom"> <b class="r4"></b> <b class="r3"></b> <b class="r3"></b> <b class="r1"></b> </b> 
        </div>
        <div id="story<%= item.ItemId %>" 
             class="box story" style="display: none;">
            <div class="title_bar" onclick="hideStory(<%= item.ItemId %>)">
                <a class="title" href="<%= item.Link %>" target="_blank">
                    <%= item.Title%>
                </a>
                <span class="item_date">
                    (<%= feed.Title%>)
                </span>
                <span class="item_date">
                    <%= item.PubDate.ToString("h:mm tt, MMM d, yy")%> UTC (Like-ness: <%= item.Score.ToString("F") %>)
                </span>
            </div>
            <div class="description"><%= item.Description%></div>
        </div>
</li>

<%      } %>
</ul>
<%  if (i > 0) {%>
        <a href="#" onclick="showPage(<%= (i-1)%>)">Prev Page</a>
<%  } %>
Page <%= (i+1)%> of <%= totPages%> 
<%  if ( (i+1) < totPages) {%>
        <a href="#" onclick="showPage(<%= (i+1)%>)">Next Page</a>
<%  } %>

</div>
<%    }     %>

</div>
</asp:Content>
