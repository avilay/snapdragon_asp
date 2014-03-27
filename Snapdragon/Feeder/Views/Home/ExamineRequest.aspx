<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="ExamineRequest.aspx.cs" Inherits="Feeder.Views.Home.ExamineRequest" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<div>
Client accepts the following MIME types:
<ul>
<%foreach( string acceptType in Request.AcceptTypes ) { %>
<li><%= acceptType %></li>
<%} %>
</ul>
</div>
<p></p>

<div>
Anonymouse user ID for this request is <strong><%=Request.AnonymousID %></strong>
</div>
<p></p>

<div>
ASP.NET application's virtual path is <strong><%=Request.ApplicationPath %></strong>
</div>
<p></p>

<div>
ASP.NET application's relative virtual path is <strong><%=Request.AppRelativeCurrentExecutionFilePath %></strong>
</div>
<p></p>

<div>
Client browser has the following capabilities:
<ul>
<li>Browser supports ActiveX controls? <strong><%=Request.Browser.ActiveXControls.ToString() %></strong></li>
<li>Browser supports Background sounds? <strong><%=Request.Browser.BackgroundSounds.ToString() %></strong></li>
<li>Browser is in beta? <strong><%=Request.Browser.Beta.ToString() %></strong></li>
<li>Browser supports Background sounds? <strong><%=Request.Browser.BackgroundSounds.ToString() %></strong></li>
<li>Browser string set by user-agent <strong><%=Request.Browser.Browser %></strong></li>
<li>Browser supports forms in deck? <strong><%=Request.Browser.CanCombineFormsInDeck.ToString() %></strong></li>
<li>Browser can initiate voice call? <strong><%=Request.Browser.CanInitiateVoiceCall.ToString() %></strong></li>
<li>Browser supports Background sounds? <strong><%=Request.Browser.BackgroundSounds.ToString() %></strong></li>
<li>And a whole lot of other capabilities that I was too lazy to enumerate. For details on all the properties see <a href="http://msdn.microsoft.com/en-us/library/system.web.httpbrowsercapabilities_members.aspx">msdn documentation</a></li>
</ul>
</div>
<p></p>

<div>
Client certificate <strong><%=Request.ClientCertificate.ToString() %></strong>
</div>
<p></p>

<div>
Content encoding name is<strong><%=Request.ContentEncoding.EncodingName %></strong>
</div>
<p></p>

<div>
Content length sent by the client is <strong><%=Request.ContentLength.ToString() %></strong>
</div>
<p></p>

<div>
Content type sent by the client is <strong><%=Request.ContentType %></strong>
</div>
<p></p>

<div>
Client has the following cookies set:
<ul>
<%foreach( HttpCookie cookie in Request.Cookies ) { %>
<li>Domain: <strong><%=cookie.Domain %></strong></li>
<li>Expires: <strong><%=cookie.Expires.ToShortDateString() %></strong></li>
<li>Client side access allowed: <strong><%=cookie.HttpOnly.ToString() %></strong></li>
<li>Name: <strong><%=cookie.Name %></strong></li>
<li>Path: <strong><%=cookie.Path %></strong></li>
<li>Is cookie secure: <strong><%=cookie.Secure.ToString() %></strong></li>
<li>Value: <strong><%=cookie.Value %></strong></li>
<%if( cookie.HasKeys ) { %>
<li>Cookie has keys but too lazy to figure out how to enumerate the key/value pairs</li>
<%}
  else { %>
<li>Cookie does not have any sub keys</li>
<%} %>
<%} %>
</ul>
</div>
<p></p>

<div>
Current execution file path is <strong><%=Request.CurrentExecutionFilePath %></strong>
</div>
<p></p>

<div>
File path is <strong><%=Request.FilePath %></strong>
</div>
<p></p>

<div>
Files uploaded by the client:
<ul>
<% 
    HttpFileCollection files = Request.Files;
    foreach( string key in Request.Files.AllKeys ) {
%>
<li>File: <strong><%=key %></strong></li>
<li>Size: <strong><%=files[key].ContentLength %></strong></li>
<li>Content type: <strong><%=files[key].ContentType %></strong></li>
<%
    }
%>
</ul>
</div>
<p></p>

<div>
Form parameters:
<ul>
<%foreach( string key in Request.Form.Keys ) { %>
<li><strong><%=key %>: <%=Request.Form[key] %></strong></li>
<%} %>
</ul>
</div>
<p></p>

<div>
HTTP Headers:
<ul>
<%foreach( string key in Request.Headers.Keys ) { %>
<li><strong><%=key %>: <%=Request.Headers[key] %></strong></li>
<%} %>
</ul>
</div>
<p></p>

<div>
HTTP Method: <strong><%=Request.HttpMethod %></strong>
</div>
<p></p>

<div>
Is the request authenticated: <strong><%=Request.IsAuthenticated.ToString() %></strong>
</div>
<p></p>

<div>
Is the request local: <strong><%=Request.IsLocal.ToString() %></strong>
</div>
<p></p>

<div>
Is the request authenticated: <strong><%=Request.IsSecureConnection.ToString() %></strong>
</div>
<p></p>

<div>
Logon user identity name: <strong><%=Request.LogonUserIdentity.Name %></strong>
</div>
<p></p>

<div>
All params:
<ul>
<%foreach( string key in Request.Params.Keys ) { %>
<li><strong><%=key %>: <%=Request.Params[key] %></strong></li>
<%} %>
</ul>
</div>
<p></p>

<div>
Path info: <strong><%=Request.PathInfo %></strong>
</div>
<p></p>

<div>
Physical application path: <strong><%=Request.PhysicalApplicationPath %></strong>
</div>
<p></p>

<div>
Physical path: <strong><%=Request.PhysicalPath %></strong>
</div>
<p></p>

<div>
Querystring params:
<ul>
<%foreach( string key in Request.QueryString.Keys ) { %>
<li><strong><%=key %>: <%=Request.QueryString[key] %></strong></li>
<%} %>
</ul>
</div>
<p></p>

<div>
Raw url: <strong><%=Request.RawUrl %></strong>
</div>
<p></p>

<div>
Request type: <strong><%=Request.RequestType %></strong>
</div>
<p></p>

<div>
Server variables:
<ul>
<%foreach( string key in Request.ServerVariables.Keys ) { %>
<li><strong><%=key %>: <%=Request.ServerVariables[key] %></strong></li>
<%} %>
</ul>
</div>
<p></p>

<div>
Total bytes: <strong><%=Request.TotalBytes.ToString() %></strong>
</div>
<p></p>

<div>
URL: <strong><%=Request.Url.ToString() %></strong>
</div>
<p></p>

<div>
URL Referer: <strong><%=Request.UrlReferrer %></strong>
</div>
<p></p>

<div>
User agent: <strong><%=Request.UserAgent %></strong>
</div>
<p></p>

<div>
User host address: <strong><%=Request.UserHostAddress %></strong>
</div>
<p></p>

<div>
User host name: <strong><%=Request.UserHostName %></strong>
</div>
<p></p>

<div>
User languages:
<ul>
<%foreach( string lang in Request.UserLanguages ) { %>
<li><strong><%=lang%></strong></li>
<%} %>
</ul>
</div>
<p></p>

</asp:Content>
