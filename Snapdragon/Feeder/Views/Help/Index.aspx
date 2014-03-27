<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Help</h2>
    
    <div style="font-style:italic; font-weight:bold; float:left">
    1. How do I use the &nbsp;&nbsp;
    </div>
    <div style="margin:0px; padding:5px; float:left; background-color:#5c87b2; color: White">
    <a style="color:White; text-decoration:none;" href="javascript:(function(){url='http://avilay.info/Feed/SearchOnPage?url='+encodeURIComponent(window.location.href); window.open(url,'Title','height=500,width=500,resizable=yes,scrollbars=yes,toolbar=yes,menubar=no,location=no,directories=no,status=yes')})()">
        Feeder
    </a>
    </div>
    <div style="font-style:italic; font-weight:bold;">
    &nbsp; &nbsp; button?
    </div>
    <div style="margin:5px; padding:5px;">
        <div style="margin:10px; padding:10px;"><em>For Firefox users:</em></div>
        <div style="margin:10px; padding:10px;">Simly drag this button to the favorites toolbar as shown here<br />&nbsp;<br />
            <img src="/Content/Images/firefox_button.jpg" alt="firefox screenshot" /><br />
        </div>
        <div style="margin:10px; padding:10px;"><em>For Internet Explorer users</em><br /></div>
        <div style="margin:5px; padding:5px;">
            <ol>
                <li style="margin:5px; padding:5px;">Right click on the button and select "Add to Favorites..."</li>
                <li style="margin:5px; padding:5px;">You will get a warning prompt saying "You are adding a favorite that might not be safe. Do you want to continue?"</li>
                <li style="margin:5px; padding:5px;">Click "yes". This warning is displayed because the Feeder button contains javascript code.</li>
                <li style="margin:5px; padding:5px;">
                Create the Favorite in the Favorites Bar folder as shown here<br />
                <img src="/Content/Images/ie_button.jpg" alt="IE screen shot" /> <br />    
                </li>
            </ol>
        </div>
        <div>I am working on a better solution for IE usage but for now this gets the job done.</div>
    </div>

</asp:Content>
