<!--#include file="NxlProtectionHeader.aspx" -->
<%@ Assembly Name="NextLabs.RightsManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=c98953f573c68e1d" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Page Language="C#" AutoEventWireup="true" Inherits="NextLabs.RightsManager.NxlProtection" DynamicMasterPageFile="~masterurl/default.master" EnableSessionState="true" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <SharePoint:StyleBlock runat="server">
        .catoryTitle{
        display: inline-block;
        background-color: #FFFCDE;
        width: 100%;  
        margin: 0 0 20px 0;
        padding: 10px 0px 5px 0px;
        }
        .catoryLabel{
        margin-right: 5px;
        }
        .catoryContext{
        background-color: #ECECF1;
         padding: 20px; 
        margin-bottom: 20px;
        }
        .catoryContextButton{
        display: inline-block;
        margin-bottom: 5px;
        }
        .cancelBtn{
        margin-right: 10px;
        width: 81px;
        margin-bottom: 5px;
        color: #333;
        background-color: #fff;
        border-color: #ccc;
        }
        .uploadBtn{
        margin-bottom: 5px;
       <%-- background-color: #399649!important;
        color: #fff!important;--%>
        background-color: #fff;
        color: #333;
        border-color: #ccc;
        width: 81px;
        margin-right: 10px;
        }
        .btn {
    display: inline-block;
    padding: 6px 12px;
    margin-bottom: 0;
    font-size: 14px;
    font-weight: 400;
    line-height: 1.42857143;
    text-align: center;
    white-space: nowrap;
    vertical-align: middle;
    -ms-touch-action: manipulation;
    touch-action: manipulation;
    cursor: pointer;
    -webkit-user-select: none;
    -moz-user-select: none;
    -ms-user-select: none;
    user-select: none;
    background-image: none;
    border: 1px solid transparent;
    border-radius: 4px;
}
        .fileTitle{
        padding: 10px;
        margin-bottom: 0;
        background-color: #EFF0F2;
        word-break: break-all;
        }
        p {
    display: block;
    margin-block-start: 1em;
    margin-block-end: 1em;
    margin-inline-start: 0px;
    margin-inline-end: 0px;
}
        .rms-settings-button-color{
    background-color: #399649!important;
    color: #fff!important;
        }
        .hide{
        display:none;
        }
        .mainView{
        background: rgb(255, 252, 222); margin: 10px; padding: 0px 20px; border-radius: 3px; border: 1px solid rgb(255, 246, 138); text-align: left; overflow: hidden; min-width: 250px; box-sizing: border-box;
        }
        .mainContent{
        display: inline-block; background-color: #FFFCDE; width: 100%; padding-left: 0; padding-right: 0;
        }
}
    </SharePoint:StyleBlock>
    <script type="text/javascript" src="jquery-1.10.2.js"></script>
    <script type="text/javascript">
        function updateTagBtn(btn)
        {
            var parent = btn.parentNode;
            var labelId = parent.id.replace("catoryContext", "catoryLabel");
            var catgoryLabel = document.getElementById(labelId);
            if (catgoryLabel.getAttribute("multiSel") == "True") {
                if (btn.getAttribute("class").indexOf("rms-settings-button-color") > -1) {
                    btn.classList.remove("rms-settings-button-color");
                } else {
                    btn.classList.add("rms-settings-button-color");
                }
            } else {
                if (btn.getAttribute("class").indexOf("rms-settings-button-color") > -1) {
                    btn.classList.remove("rms-settings-button-color");
                }
                else {
                    for (var i = 0; i < parent.children.length; i++) {
                        parent.children[i].classList.remove("rms-settings-button-color");
                        parent.children[i].classList.add("catoryContextButton");
                    }
                    btn.classList.add("rms-settings-button-color");
                }
            }
        }
    </script>
    <script type="text/javascript">
        function Upload()
        {
            var dicArray = new Array();
            var ItemId = document.getElementsByName("ItemId")[0].innerText;
            var listId = document.getElementsByName("ListId")[0].innerText;
            var SiteUrl = document.getElementsByName("SiteUrl")[0].innerText;
            var tagCount = document.getElementsByName("tagCountDiv")[0].getAttribute("tagCount");
            for (var i = 1; i <= tagCount; i++)
            {
                var tagName = document.getElementsByName("catoryLabel" + i)[0].innerText;
                var catoryContextControl = document.getElementsByName("catoryContext" + i)[0];
                var obj = new Object();
                obj.tagName = tagName;
                obj.tagValue = new Array();
                for(var j=1;j<=catoryContextControl.children.length;j++)
                {
                    var tagBtn = document.getElementsByName("catoryContextButton" + i + j)[0];
                    if (tagBtn.getAttribute("class").indexOf("rms-settings-button-color") > -1)
                    {
                        var tagValue = tagBtn.value;
                        obj.tagValue.push(tagValue);
                    }
                }
                var catoryLabel = document.getElementsByName("catoryLabel" + i)[0];
                if (catoryLabel.getAttribute("mandatory") == "True" && obj.tagValue.length==0) {
                    alert("you must select a value for: " + tagName);
                    return;
                }
                if (obj.tagName && obj.tagValue.length>0) {
                    dicArray.push(obj);
                }
            }
            var strJsonData = JSON.stringify(dicArray);
            $.ajax({
                type: "post",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                url: "NxlProtection.aspx/Upoload",
                data: "{ tagArray:'" + strJsonData + "' ,itemId:'" + ItemId + "',listId:'" + listId + "',siteUrl:'"+SiteUrl+"'}",
                success: function (data) {
                    if (data.d == "1")
                    {
                        alert("Failed to protect the file, may the file was locked or checkouted.")
                    }
                    else if(data.d == "0")
                    {
                        alert("The file was protected successfully.");
                    }
                    else if (data.d == "2")
                    {
                        alert("Successfully protected some attachments but failed others, may you want to try again. ");
                    }
                    closeWindow();
                },
                error: function () {
                    alert("Failed to protect the file , please check admin.")
                    closeWindow();
                }
            });
        }
    </script>
    <script type="text/javascript">
        function closeWindow()
        {
            if (history.length>1) {
                history.back(-1);
            } else {
                var userAgent = navigator.userAgent;
                window.opener = null;
                window.open("", "_self");
                window.close();
            }
        }
    </script>
    <div style="text-align: center;max-width: 500px;">
              <div class="fileTitle">
            <p style="max-width: 500px; margin: 0px;">
                <b id="FileName" style="font-weight: 700;" runat="server"></b>
            </p>
        </div>
        <div style="max-width: 500px;">
            <div style="font-weight: 700;">Company-defined Rights </div>
            <div>Company-defined rights are permissions determined by centralized policies defined by your administrator.</div>
            <asp:panel ID="MainView" runat="server" CssClass="mainView">
                <div id="MainContent" class="mainContent" runat="server">
                </div>
                <div style="text-align: center; display: block;">
                     <div id="TagCountDiv" name="tagCountDiv" class="hide" runat="server"></div>
                    <div id="ItemId" name="ItemId" class="hide" runat ="server"></div>
                    <div id="ListId" name="ListId" class="hide" runat ="server"></div>
                    <div id="SiteUrl" name="SiteUrl" class="hide" runat ="server"></div>
					<input type="button" value="Protect" class="uploadBtn btn" onclick="Upload()" />
                    <input type="button" value="Cancel" class="cancelBtn btn" onclick="closeWindow()" />
                </div>
            </asp:panel>
            <asp:panel ID="ErrorView" runat="server" CssClass="hide" >
                <div id="Div2" style="min-height:100px;" >
                    <span id="InfoText" style="color:red;" runat="server"> Warning:You are uploading file with no classification.</span>
                   </div>
                <div style="text-align: center; display: block;">
                    <input type="button" value="Protect" class="uploadBtn btn" onclick="Upload()" />
					<input type="button" value="Cancel" class="cancelBtn btn" onclick="closeWindow()" /> 
                </div>
            </asp:panel>
        </div>
    </div>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    NextLabs File Protection
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">

</asp:Content>
