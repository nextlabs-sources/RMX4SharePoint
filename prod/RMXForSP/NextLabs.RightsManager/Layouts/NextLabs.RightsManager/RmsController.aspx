<!--#include file="RmsControllerHeader.aspx" -->
<%@ Assembly Name="NextLabs.RightsManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=c98953f573c68e1d" %>
<%@ Page Language="C#" AutoEventWireup="true" Inherits="NextLabs.RightsManager.RmsController" MasterPageFile="~/_layouts/application.master" %>
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
    <wssawc:EncodedLiteral runat="server" ID="PageDescription" Text="Activation" EncodeMethod='HtmlEncode' />
</asp:Content>

<asp:Content ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script type="text/javascript" src="TreeUtils.js"></script>
    <script type="text/javascript" src="UITools.js"></script>
    <script type="text/javascript">
        var FeatureTreeClientID = "<%=FeatureTree.ClientID%>";
    </script>
</asp:Content>

<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">
     <wssawc:StyleBlock runat="server">
        .hide{
        display:none;
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
         .triangle_border_right{
    width:0;
    height:0;
    border-width:6px 0 6px 6px;
    border-style:solid;
    border-color:transparent transparent transparent #333;
    margin-top:12px;
    float:left;
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
     </wssawc:StyleBlock>
    <div style="display:flex;margin-top: 15px;margin-left: -155px;margin-bottom: 5px;">
        <a style="margin-right:15px;font-size:25px;color:#003759; float:left;" href="/">NextLabs Rights Management setup</a>
        <div class="triangle_border_right">
        </div>
        <label style="margin-left: 15px;font-size: 25px;color:#5d6878;">Activation</label>
    </div>
    <div style="width: 2200px;margin-left: -165px;position: absolute;border-bottom: 1px solid #003399;"></div>
    <table border="0" cellspacing="0" cellpadding="0" class="ms-propertysheet">
    <tr><td>
   <%-- <wssuc:InputFormSection ID="Description"  Title="Description" Description="NextLabs Rights Management (RMX) can be activated or deactivated at the web application or site collection level. Select the web application and/or site collection that you want to activate or deactivate and click Update." runat="server">
    </wssuc:InputFormSection>--%>
         <tr>
            <td class="ms-formdescriptioncolumn-wide" valign="top">
                <table border="0" cellpadding="1" cellspacing="0" width="100%" summary="" role="presentation">
                    <tbody>
                        <tr>
                            <td class="ms-sectionheader" style="padding-top: 4px;" height="22" valign="top">
                                <h2 style="font-size:2em;margin-bottom:40px;margin-top:40px;" class="ms-standardheader ms-inputformheader">Activation
                                </h2>
                            </td>
                        </tr>
                </table>
            </td>
        </tr>
    <wssuc:InputFormSection ID="CentralAdminSection" Title="Web application activation settings" TextAlign="Right" Visible="true" Description="Select the web application and/or site collection that you want to activate or deactivate." runat="server">
        <template_inputformcontrols>  
            <div>Web application URL</div>     
   		    <%--<asp:Label ID="WebAppDropDownText" Text="Web application" runat="server" > </asp:Label>--%>
            <asp:DropDownList ID="WebAppDropDown" runat="server" ActivateViewState="true" style="min-width:420px;" AutoPostBack="true" OnSelectedIndexChanged="WebAppDropDown_SelectedIndexChanged"></asp:DropDownList>
            <br>
            <br>
            <div style="margin-bottom:2px;">Web application status</div>
            <div style="display:flex;">
               <div>
                    <input type="radio" id="OptionCheckBox" runat="server" />
                    <label style="vertical-align:bottom;" for="<%=OptionCheckBox.ClientID %>">Activate</label>
               </div>
                <div style="margin-left:40px;">
                      <input type="radio" id="DeactivateCheckBox" runat="server" />
                     <label style="vertical-align:bottom;" for="<%=DeactivateCheckBox.ClientID %>">Deactivate</label>
                </div>
            </div>     
           <%-- <wssawc:InputFormRadioButton ID="OptionCheckBox" ForeColor=Blue Width=400 Text="Activate"  runat="server" GroupName="GroupActiveMode">
            </wssawc:InputFormRadioButton>
                
		    <wssawc:InputFormRadioButton ID="DeactivateCheckBox" ForeColor=Blue Width=400 Text="Deactivate"  runat="server" GroupName="GroupActiveMode">
            </wssawc:InputFormRadioButton>--%>
            </td></tr>
		</template_inputformcontrols>
	</wssuc:InputFormSection>	
		
    <wssuc:InputFormSection ID="Feedback" Visible="true" runat="server">
        <template_inputformcontrols>
            <wssuc:InputFormControl ID="ProgressBar" LabelText="" runat="server">
            </wssuc:InputFormControl>
	    </template_inputformcontrols>
    </wssuc:InputFormSection>

    <wssuc:InputFormSection ID="OverviewSection" Title="" Description="" runat="server">
        <template_description>
        </template_description>
        <template_inputformcontrols>
			<wssuc:InputFormControl LabelText="" runat="server">
				<Template_Control>
					<table cellpadding="0" cellspacing="0" style="margin-top:-40px;">
						<tr>
						<td class="ms-descriptiontext ms-inputformdescription" style="padding-bottom:5px;padding-left:5px">
						</td>
						<td>
                        </td>
						</tr>
                        <tr>
                        <td>
							<asp:TreeView ShowCheckBoxes="All" ActivateViewState="true" ShowLines="true" onClick="OnTreeClick(event, false)" onDblClick="OnTreeClick(event, true)" ID="FeatureTree" runat="server" NodeIndent="12" ExpandImageUrl="/_layouts/15/images/tvplus.gif" CollapseImageUrl="/_layouts/15/images/tvminus.gif" NoExpandImageUrl="/_layouts/15/images/tvblank.gif">
								<NodeStyle CssClass="ms-authoringcontrols" />
							</asp:TreeView>
						</td>
						<td id="loaderCell" style="vertical-align:middle;display:none;width:100%;text-align:center">
							<img id="loader" src="/_layouts/15/images/NextLabs.RightsManager/ajax-loader.gif" border="0" />
						</td>
						</tr>
					</table>
			    </Template_Control>
			</wssuc:InputFormControl>
		</template_inputformcontrols>
    </wssuc:InputFormSection>
         <tr>
             <td valign="top"></td>
            <td class="ms-formdescriptioncolumn-wide" valign="top" align="left">
                <table border="0" cellpadding="1" cellspacing="0" width="100%" summary="" role="presentation">
                    <tbody>
                        <tr>
                            <td class="ms-sectionheader" style="padding-top: 4px;" height="22" valign="top">
                               <asp:Button ID="UpdateButton" runat="server" Text="Save" class="btn" OnClientClick="UpdateButton_Click" OnClick="UpdateButton_Click"  />
                            	<asp:Button ID="ReturnButton" runat="server" Text="Cancel" class="btn"  OnClick="ReturnButton_Click" />	
                            </td>
                        </tr>
                </table>
            </td>
        </tr>
    
   <%-- <wssuc:ButtonSection runat="server" ShowStandardCancelButton="false">
        <template_buttons>
                <asp:Button ID="UpdateButton" runat="server" Text="Save" class="ms-ButtonHeightWidth" OnClientClick="UpdateButton_Click" OnClick="UpdateButton_Click"  />
				<asp:Button ID="ReturnButton" runat="server" Text="Cancel" class="ms-ButtonHeightWidth"  OnClick="ReturnButton_Click" />		
		</template_buttons>
    </wssuc:ButtonSection>--%>
    </table>
</asp:Content>
