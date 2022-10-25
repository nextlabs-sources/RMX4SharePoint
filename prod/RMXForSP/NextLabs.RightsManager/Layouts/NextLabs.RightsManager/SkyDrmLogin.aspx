<%@ Assembly Name="NextLabs.RightsManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=c98953f573c68e1d" %>
<%@ Page Language="C#" AutoEventWireup="true" Inherits="NextLabs.RightsManager.SkyDrmLogin" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">

        <div>
            <label style="width:50px">User Name:</label>
	        <asp:TextBox ID="UserName" runat="server" Width="300"></asp:TextBox>
            <br />
            <br />
            <label style="width:50px;margin-right:13px;">Password:</label><asp:TextBox ID="SkyDrmPassword" runat="server" Width="300" TextMode="Password"></asp:TextBox>
            <br />
            <br />
            <asp:Button ID="LoginButton" runat="server" Text="Log in" OnClick="Login" /> 
            <asp:Label ID="LoginErrorTip" runat="server" Width="600" Visible="true" ForeColor="Red"></asp:Label>
            <br />
            <br />
        </div>
        <asp:Label ID="Action" runat="server" Width="600" Visible="false"></asp:Label>
        <asp:Label ID="ItemId" runat="server" Width="600" Visible="false"></asp:Label>
        <asp:Label ID="ListId" runat="server" Width="600" Visible="false"></asp:Label>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Log in to SkyDrm
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Log in to SkyDrm
</asp:Content>

 

