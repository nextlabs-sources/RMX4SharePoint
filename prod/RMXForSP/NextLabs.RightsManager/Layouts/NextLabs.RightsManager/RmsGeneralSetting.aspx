<!--#include file="RmsGeneralSettingHeader.aspx" -->
<%@ Assembly Name="NextLabs.RightsManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=c98953f573c68e1d" %>
<%@ Page Language="C#" AutoEventWireup="true" Inherits="NextLabs.RightsManager.RmsGeneralSetting" MasterPageFile="~/_layouts/application.master" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Import Namespace="Microsoft.SharePoint" %>

<%@ Register TagPrefix="wssuc" TagName="InputFormSection" src="~/_controltemplates/15/InputFormSection.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormControl" src="~/_controltemplates/15/InputFormControl.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="ButtonSection" src="~/_controltemplates/15/ButtonSection.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="TemplatePickerControl" src="~/_controltemplates/15/TemplatePickerControl.ascx" %>


<asp:Content ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    <wssawc:EncodedLiteral runat="server" Text="NextLabs Rights Management setUp" EncodeMethod='HtmlEncode' />
</asp:Content>
<asp:Content ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    <wssawc:EncodedLiteral runat="server" Text="NextLabs Rights Management setUp" EncodeMethod='HtmlEncode' />
</asp:Content>
<asp:Content ContentPlaceHolderID="PlaceHolderPageDescription" runat="server">
    <wssawc:EncodedLiteral runat="server" ID="PageDescription" Text="Configuration" EncodeMethod='HtmlEncode' />
</asp:Content>

<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <wssawc:StyleBlock runat="server">
        .hide{
        display:none;
        }

.file-input-wrapper {
    width: 360px;
    height: 23px;
    overflow: hidden;
    position: relative;
}
.file-input-wrapper > input[type="file"] {
    font-size: 360px;
    position: absolute;
    top: 0;
    right: 0;
    opacity: 0;
    filter:Alpha(opacity=0);
}
.file-input-wrapper > .btn-file-input {
    display: inline-block;
    width: 80px;
    height: 20px;
    vertical-align:bottom;
}
.file-input-wrapper:hover > .btn-file-input {
    background-color: #aaa;
}
 .btn{
         min-width: 6em;
         padding: 7px 10px;
         border: 1px solid #ababab;
         border-top-color: rgb(171, 171, 171);
         border-right-color: rgb(171, 171, 171);
         border-bottom-color: rgb(171, 171, 171);
         border-left-color: rgb(171, 171, 171);
         background-color: #fdfdfd;
         background-color: #fdfdfd;
         margin-left: 10px;
         font-family: "Segoe UI","Segoe",Tahoma,Helvetica,Arial,sans-serif;
         font-size: 11px;
         color: #444;
         }
         .btn:hover {
            background-color: rgb(230, 242, 250);
            border-color: rgb(146, 192, 224);
         }
         #s4-titlerow{
         display:none !important;
         }
        #s4-ribbonrow{
        display:none !important;
        }
        #s4-leftpanel-content{
         display:none !important;
         }
         .triangle_border_right{
    width:0;
    height:0;
    border-width:6px 0 6px 6px;
    border-style:solid;
    border-color:transparent transparent transparent #333;
    margin-top:12px;
    float:left;
}
        </wssawc:StyleBlock>
     <script src="jquery-1.10.2.js"></script>
     <div style="display:flex;margin-top: 15px;margin-left: -155px;margin-bottom: 5px;">
        <a style="margin-right:15px;font-size:25px;color:#003759; float:left;" href="/">NextLabs Rights Management setup</a>
        <div class="triangle_border_right">
        </div>
        <label style="margin-left: 15px;font-size: 25px;color:#5d6878;">Configuration</label>
    </div>
    <div style="width: 2200px;margin-left: -165px;position: absolute;border-bottom: 1px solid #003399;"></div>
    <div style="display:none;" runat="server" id="columnJson"></div>
    <table border="0" style="margin-top:25px;" cellspacing="0" cellpadding="0" class="ms-propertysheet">
        <tr>
            <td class="ms-formdescriptioncolumn-wide" valign="top">
                <table border="0" cellpadding="1" cellspacing="0" width="100%" summary="" role="presentation">
                    <tbody>
                        <tr>
                            <td class="ms-sectionheader" style="padding-top: 4px;" height="22" valign="top">
                                <h2 style="font-size:2em;margin-bottom:15px;" class="ms-standardheader ms-inputformheader">Configuration
                                </h2>
                            </td>
                        </tr>
                </table>
            </td>
        </tr>
        <wssuc:InputFormSection ID="JavaPCSetting" Title="Policy Controller configuration" Visible="true" Description="Specify the host and port information for Control Center and Policy Controller, Client ID and Client secure key." runat="server">
        <template_inputformcontrols>
        <wssuc:InputFormControl LabelText="" runat="server">
          <Template_Control>
            <table cellpadding="0" cellspacing="0">
                <tr>
                <td class="ms-descriptiontext ms-inputformdescription" style="padding-bottom:5px;padding-left:5px"></td>
                <td></td>
                </tr>
                <tr><td>
	            <asp:Label ID="Label1" Text="<br/><br/>Policy Controller host address<br/>" runat="server" > </asp:Label>
                <wssawc:InputFormTextBox ID="JavaPcHost" ForeColor=Blue class="ms-input" Width=360 runat="server"></wssawc:InputFormTextBox>
                <asp:Label ID="Label2" Text="<br/><br/>Control Center host address<br/>" runat="server" > </asp:Label>
                <wssawc:InputFormTextBox ID="OAUTHHost" ForeColor=Blue class="ms-input" Width=360 runat="server"></wssawc:InputFormTextBox>      
                <asp:Label ID="Label3" Text="<br/><br/>Client ID<br/>" runat="server" > </asp:Label>
                <wssawc:InputFormTextBox ID="ClientSecureID" ForeColor=Blue class="ms-input" Width=360 runat="server"></wssawc:InputFormTextBox>      
                <asp:Label ID="Label4" Text="<br/><br/>Client secure key<br/>" runat="server" > </asp:Label>
                <wssawc:InputFormTextBox ID="ClientSecureKey" ForeColor=Blue class="ms-input" Width=360 TextMode="Password" runat="server"></wssawc:InputFormTextBox>
                <asp:Label ID="CheckStatus" ForeColor=Red  Text="" runat="server" > </asp:Label>
              </td></tr>
            </table>
        </Template_Control>
        </wssuc:InputFormControl>

        </template_inputformcontrols> 
        </wssuc:InputFormSection>
                
        <wssuc:InputFormSection ID="RMSSetting" Title="Rights Management Server (RMS) configuration" Visible="true" Description="Specify the RMS host URL, router URL, App ID, App Key, and Certificate information." runat="server">
        <template_inputformcontrols>
        <wssuc:InputFormControl LabelText="" runat="server">
          <Template_Control>
            <table cellpadding="0" cellspacing="0">
              <tr>
              <td class="ms-descriptiontext ms-inputformdescription" style="padding-bottom:5px;padding-left:5px"></td>
              <td></td>
              </tr>
              <tr><td>
                 <asp:Label ID="Label10" Text="<br/><br/>RMS host URL<br/>" runat="server" > </asp:Label>
                <wssawc:InputFormTextBox ID="SecureViewURL" ForeColor=Blue class="ms-input" Width=360 runat="server"></wssawc:InputFormTextBox>
	            <asp:Label ID="Label6" Text="<br/><br/>Router URL<br/>" runat="server" > </asp:Label>
                <wssawc:InputFormTextBox ID="RouterURL" ForeColor=Blue class="ms-input" Width=360 runat="server"></wssawc:InputFormTextBox>
                <asp:Label ID="Label7" Text="<br/><br/>App ID<br/>" runat="server" > </asp:Label>
                <wssawc:InputFormTextBox ID="AppId" ForeColor=Blue class="ms-input" Width=360 runat="server"></wssawc:InputFormTextBox>      
                <asp:Label ID="Label8" Text="<br/><br/>App key<br/>" runat="server" > </asp:Label>
                <wssawc:InputFormTextBox ID="AppKey" ForeColor=Blue class="ms-input" Width=360 TextMode="Password" runat="server"></wssawc:InputFormTextBox>      
                <asp:Label ID="Label9" Text="<br/><br/>Api user email<br/>" runat="server" > </asp:Label>
                <wssawc:InputFormTextBox ID="TenantName" ForeColor=Blue class="ms-input" Width=360 runat="server"></wssawc:InputFormTextBox>
                <asp:Label ID="Label11" Text="<br/><br/>Certificate file<br/>" runat="server" > </asp:Label>
                  <div class="file-input-wrapper">
  <asp:TextBox ID="CertificateFileFileName" ForeColor=Blue class="ms-input" Width=270 runat="server" /> <input type="button" class="btn-file-input" ID="BrowserCertFileButton" Value="Browse" runat="server" Width=85 /> 
  <asp:FileUpload ID="CertificateFile"   runat="server"/> 
</div>
                <asp:Label ID="Label12" Text="<br/><br/>Certificate password<br/>" runat="server" > </asp:Label>
                <wssawc:InputFormTextBox ID="CertificatePassword"   TextMode="Password" ForeColor=Blue class="ms-input" Width=360 runat="server"></wssawc:InputFormTextBox>
              </td></tr>
            </table>
        </Template_Control>
        </wssuc:InputFormControl>
          </template_inputformcontrols>
             
        </wssuc:InputFormSection>
               <%-- <wssuc:InputFormSection id="SelectColumn" Title="Select column" runat="server">
			<Template_Description>
				<wssawc:EncodedLiteral runat="server" id="EncodedLiteral4" text="Select the checkbox to select all columns." EncodeMethod='HtmlEncode'/>
                <br/>
				<wssawc:EncodedLiteral runat="server" id="EncodedLiteral2" text="Clear the checkbox to specify columns." EncodeMethod='HtmlEncode'/>
			</Template_Description>
			
            <Template_InputFormControls>
                <wssawc:InputFormCheckBox ID="SelectedAll" style="margin-left:8px;" LabelText="Select all columns" AutoPostBack="true" OnCheckedChanged="SelectAllCheck_Changed" runat="server">
                </wssawc:InputFormCheckBox>
            </Template_InputFormControls>                                   
        </wssuc:InputFormSection>--%>

     <%--   <wssuc:InputFormSection ID="ColumnsSetting" Title="Column configuration" Visible="true" Description="Enter columns you would like to use for policy evaluation." runat="server">
        <template_inputformcontrols>
        <wssuc:InputFormControl LabelText="" runat="server">
          <Template_Control>
            <table cellpadding="0" cellspacing="0">
              <tr>
                  <td>
	            <asp:Label ID="Label5" Text="Column Display Name" runat="server" > </asp:Label>
                 <ul id="regionZTree" class="ztree" style="margin-left:20px;overflow: scroll;margin-top:75px;"></ul>
                <wssawc:InputFormTextBox ID="ColumnNames" ForeColor=Blue Visible="false" class="ms-input" TextMode="MultiLine" Width=360 Height=100 runat="server"></wssawc:InputFormTextBox>
              </td>
              </tr>
            </table>
        </Template_Control>
        </wssuc:InputFormControl>
          </template_inputformcontrols> 
        </wssuc:InputFormSection>--%>
          <tr>
             <td valign="top"></td>
            <td class="ms-formdescriptioncolumn-wide" valign="top" align="left">
                <table border="0" cellpadding="1" cellspacing="0" width="100%" summary="" role="presentation">
                    <tbody>
                        <tr>
                            <td class="ms-sectionheader" style="padding-top: 4px;" height="22" valign="top">
                                 <asp:Button ID="SaveButton" style="margin-left:20px;" runat="server" Text="Save" class="btn" OnClick="SaveButton_Click"  />
		                 		<asp:Button ID="ReturnButton" runat="server" Text="Cancel" class="btn"  OnClick="ReturnButton_Click" />
                                 </td>
                        </tr>
                </table>
            </td>
        </tr>
       <%-- <wssuc:ButtonSection runat="server" ShowStandardCancelButton="false">
            <template_buttons>
                <asp:Button ID="SaveButton" runat="server" Text="Save" class="btn" OnClick="SaveButton_Click"  />
				<asp:Button ID="ReturnButton" runat="server" Text="Cancel" class="btn"  OnClick="ReturnButton_Click" />
		    </template_buttons>
        </wssuc:ButtonSection>--%>

    </table>
    <script>
      <%--  var setting = {
            view: {
                dblClickExpand: false,
                showLine: true,
                fontCss: { 'color': 'black', 'font-weight': 'bold' },
                selectedMulti: true
            },
            async: {
                enable: true,
                url: "RmsGeneralSetting.aspx",
                autoParam: ["id", "isParent"],
                otherParam: ["ajaxMethod", 'AsyncSubSiteNode'],
                dataType: "json"
            },
            check: {
                chkboxType: { "Y": "s", "N": "s" },
                chkStyle: "checkbox",
                enable: true
            },
            data: {
                simpleData: {
                    enable: true,
                    idKey: "id",
                    pIdKey: "pId",
                    rootPId: 0
                }
            },
            callback: {
                onAsyncSuccess: zTreeOnAsyncSuccess
            }
        };
        var columnData = $("#<%=columnJson.ClientID %>").text();
        var data = JSON.parse(columnData);
        $(document).ready(function () {
            $.fn.zTree.init($("#regionZTree"), setting, data);
            var zTree = $.fn.zTree.getZTreeObj("regionZTree");
            var nodes = zTree.getNodes();
            if (nodes.length > 0) {
                for (var i = 0; i < nodes.length; i++) {
                    zTree.expandNode(nodes[i], true, false, false);
                }
            }
            var rootNode = zTree.getNodesByFilter(function (node) { return node.level == 0 }, true);
            asyncSubNode(zTree, rootNode);
        });
        function asyncSubNode(zTree, rootNode) {
            var subsiteNode = zTree.getNodesByFilter(function (node) { return node.isParent == true }, false, rootNode);
            for (var i = 0; i < subsiteNode.length; i++) {
                zTree.reAsyncChildNodes(subsiteNode[i], "refresh", "", zTreeOnAsyncSuccess)
            }
        }
        function zTreeOnAsyncSuccess(event, treeId, treeNode, msg) {
            if (treeNode) {
                var zTree = $.fn.zTree.getZTreeObj("regionZTree");
                asyncSubNode(zTree, treeNode);
            }
        }--%>
    </script>
</asp:Content>
