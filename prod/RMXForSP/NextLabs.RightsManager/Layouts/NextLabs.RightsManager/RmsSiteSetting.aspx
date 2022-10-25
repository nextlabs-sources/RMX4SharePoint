<!--#include file="RmsSiteSettingHeader.aspx" -->
<%@ Page language="C#" MasterPageFile="~/_layouts/application.master" Inherits="NextLabs.RightsManager.RmsSiteSetting, NextLabs.RightsManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=c98953f573c68e1d" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 

<%@ Register TagPrefix="wssuc" TagName="InputFormSection" src="~/_controltemplates/15/InputFormSection.ascx" %> 
<%@ Register TagPrefix="wssuc" TagName="InputFormControl" src="~/_controltemplates/15/InputFormControl.ascx" %> 
<%@ Register TagPrefix="wssuc" TagName="ButtonSection" src="~/_controltemplates/15/ButtonSection.ascx" %> 

<asp:Content contentplaceholderid="PlaceHolderPageTitle" runat="server">
<wssawc:EncodedLiteral runat="server" Text="NextLabs Rights Management" EncodeMethod="HtmlEncode" />
</asp:Content>
<asp:Content ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    <wssawc:EncodedLiteral runat="server" Text="NextLabs Rights Management" EncodeMethod="HtmlEncode" />
</asp:Content>
<asp:Content ContentPlaceHolderID="PlaceHolderPageDescription" runat="server">
    <wssawc:EncodedLiteral runat="server" ID="PageDescription" Text="Site Property" EncodeMethod="HtmlEncode" />
</asp:Content>

<asp:Content contentplaceholderid="PlaceHolderMain" runat="server">
      
     <wssawc:StyleBlock runat="server">
         * {
            margin: 0;
            padding: 0;
            list-style: none;
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
            margin-left: 10px;
            font-family: "Segoe UI","Segoe",Tahoma,Helvetica,Arial,sans-serif;
            font-size: 11px;
            color: #444;
         }

        .btn:hover {
            background-color: rgb(230, 242, 250);
            border-color: rgb(146, 192, 224);
         }

        .displayText{
            font-size: 12px;
            margin-bottom: 5px;
         }
        #s4-leftpanel-content{
            display:none !important;
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


        #box {
            width: 426px;
            position: relative;
         }

         #box > p.site {
            width: 174px;
            height: 25px;
            color: #77778D;
            font-family: Segoe UI;
            font-size: 13px;
            line-height: 25px;
            margin: 0;
         }

         #box > div.select-box {
            width: 424px;
            border: 1px solid #77778D;
         }

         #box > div.select-box span.site-name {
            margin-left: 10px;
            display: inline-block;
            width: 380px;
            overflow-x: auto;
            overflow-y: hidden;
         }

         #box > div.select-box span.drop-btn {
            width: 12px;
            height: 12px;
            float: right;
            margin: 9px 10px 0 0;
            position: relative;
         }

         #box > div.select-box span.drop-btn:after {
            content: '';
            position: absolute;
            width: 0;
            height: 0;
            border: 6px solid;
            border-color: #000 transparent transparent transparent;
            top: 0;
            left: 0;
         }

         #box > div.zTree-box {
            width: 424px;
            height: 141px;
            position: absolute;
            left: 0;
            top: 54px;
            display: none;
            border: 1px solid #77778D;
            background: #fff;
            overflow-y: scroll;
         }

         #box > p.properties {
            margin: 29px 0 0 0;
            width: 160px;
            height: 20px;
            font-family: Segoe UI;
            font-size: 13px;
            line-height: 20px;
            font-weight: 700;
            color: #000;
         }

         #box > div.property-name {
            width: 424px;
            height: 222px;
            border: 1px solid #77778D;
         }

         #box > div.property-name .title {
            width: 424px;
            height: 35px;
            background: #eee;
            border-bottom: 1px solid #77778D;
            line-height: 35px;
         }

         #box > div.property-name .title input {
            margin-left: 10px;
         }

         #box > div.property-name .list {
            width: 424px;
            height: 184px;
            overflow-y: scroll;
            overflow-x: auto;
         }

         #box .list > li {
            height: 29px;
            line-height: 29px;
            white-space: nowrap;
         }

         #box .list > li > input {
            width: 13px;
            height: 13px;
            margin: 8px 13px;
         }

         #box .list > li > label {
            display: inline-block;
            height: 29px;
            line-height: 29px;
         }

    </wssawc:StyleBlock>
    <link rel="stylesheet" type="text/css" href="zTreeStyle.css"> 
    <script src="jquery-1.10.2.js"></script>
    <script type="text/javascript" src="jquery.ztree.all.min.js"></script>
        <div style="display:flex;margin-top: 15px;margin-left: -155px;margin-bottom: 5px;">
            <a id="backSite" style="margin-right:15px;font-size:25px;color:#003759;float:left;" href="/">NextLabs Rights Management setup</a>
            <div class="triangle_border_right">
            </div>
            <label style="margin-left: 15px;font-size: 25px;color:#5d6878;">Site property</label>
        </div>
        <input type="hidden" name="sitePropertyJson" style="display: none;" id="sitePropertyJson" value="" runat="server"/> 
        <div style="width: 2200px;margin-left: -165px;position: absolute;border-bottom: 1px solid #003399;"></div>
        <h2 style="margin-bottom:5px;font-size:2.46em;margin-top:30px;">Site property</h2>
	    <TABLE border="0" cellspacing="0" style="margin-top:50px;max-width:1000px;font-size:15px;" cellpadding="0" class="ms-propertysheet" width="100%"> 
		<wssuc:InputFormSection id="SiteState" style="font-size:12px;"  Title="Evaluation level" runat="server">
			<Template_Description>
                 <div style="display:list-item;margin-left: 12px;">
                    <div class="displayText" id="EncodedLiteral3" >None indicates no site properties are used for evaluation.</div>              
                 </div>
                 <div style="display:list-item;margin-left: 12px;">
                    <div class="displayText" id="EncodedLiteral1">Subsite indicates that the properties of only the direct parent subsite are used for evaluation.</div>
                </div> 
                 <div style="display:list-item;margin-left: 12px;">
			      	<div class="displayText"  id="HelpLiteral" >Site Collection indicates that the properties of site collection are used for evaluation.</div>
                </div>
                <div style="display:list-item;margin-left: 12px;">
                    <div class="displayText" id="EncodedLiteral5">Both indicates that the properties of site collection and subsite are used for evaluation.</div>
                </div>
			</Template_Description>

			<template_inputformcontrols>
            <wssuc:InputFormControl LabelText="" runat="server">
            <Template_Control>
            <table cellpadding="0" cellspacing="0">
	        <asp:Label ID="Label1" Text="<br/><br/>Level<br/>" runat="server"> </asp:Label>
            <asp:DropDownList ID="SiteLevel" runat="server">
                <asp:ListItem Text="None" Value="None" Selected="true"></asp:ListItem>
                <asp:ListItem Text="Subsite" Value="Subsite"></asp:ListItem>
                <asp:ListItem Text="SiteCollection" Value="SiteCollection"></asp:ListItem>
                <asp:ListItem Text="Both" Value="Both"></asp:ListItem>
            </asp:DropDownList>
            </table>
            </Template_Control>
            </wssuc:InputFormControl>
            </template_inputformcontrols>
               
        </wssuc:InputFormSection>

        <wssuc:InputFormSection ID="PropertySetting" Title="Site property configuration" Visible="true" Description="Select the properties to use for policy evaluation." runat="server">
        <template_inputformcontrols>
        <wssuc:InputFormControl LabelText="" runat="server">
          <Template_Control>

            <table cellpadding="0" cellspacing="0">
              <tr>
              <td class="ms-descriptiontext ms-inputformdescription" style="padding-bottom:5px;padding-left:5px"></td>
              <td></td>
              </tr>
              <tr><td>
                <div class="box" id="box">
                    <p class="site">Site</p>
                    <div class="select-box">
                        <span class="site-name"></span>
                        <span class="drop-btn"></span>
                    </div>
                    <div class="zTree-box">
                        <ul id="regionZTree" class="ztree"></ul>
                    </div>
                    <p class="properties">Properties(<span class="select-num"></span>)</p>
                    <div class="property-name">
                        <div class="title">
                            <input type="checkbox" class="select-all"/>
                            <span>Property name</span>
                        </div>
                        <div class="list"></div>
                    </div>
                </div>
                <wssawc:InputFormTextBox ID="PropertyNames" visible="false" ForeColor=Blue class="ms-input" TextMode="MultiLine" Width=360 Height=100 runat="server"></wssawc:InputFormTextBox>
	            <asp:Label ID="AliasLabel" visible="false" Text="<br/><br/>Property alias<br/>" runat="server" > </asp:Label>
                <wssawc:InputFormTextBox visible="false" ID="MapedNames" ForeColor=Blue class="ms-input" TextMode="MultiLine" Width=360 Height=100 runat="server"></wssawc:InputFormTextBox>
              </td></tr>
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
                                <asp:Button runat="server" style="margin-left:20px;" class="btn" OnClientClick="return onSaveClick()"  OnClick="BtnOK_Click" Text="Save" id="Button1" accesskey="<%$Resources:wss,okbutton_accesskey%>"/>
						        <asp:Button runat="server" class="btn" OnClick="BtnCancel_Click"  Text="Cancel" id="Button3" CausesValidation="false"/>
                            </td>
                        </tr>
                </table>
            </td>
        </tr>
	</TABLE>
    <script type="text/javascript">
        var url = window.location.href;
        var backUrl = url.replace("NextLabs.RightsManager/RmsSiteSetting.aspx", "settings.aspx");
        $('#backSite').attr('href', backUrl);
    </script>
    <script>

        var setting = {
            view: {
                dblClickExpand: true,
                showLine: false,
                fontCss: { 'color': 'black', 'font-weight': 'bold' },
                selectedMulti: true,
                showIcon: false
            },
            edit: {
                enable: false,
                editNameSelectAll: true,
                showRemoveBtn: false,
                showRenameBtn: false,
                removeTitle: "remove",
                renameTitle: "rename"
            },
            callback: {
                onClick: myOnClick
            },
            data: {
                simpleData: {
                    enable: true,
                    idKey: "id",
                    pIdKey: "pId",
                    rootPId: 0
                }
            },
            async: {
                enable: true,
                url: "RmsSiteSetting.aspx",
                autoParam: ["id", "isParent"],
                otherParam: ["ajaxMethod", 'AsyncSubSiteNode'],
                dataType: "json"
            }
        };

        var columnData = $("#<%=sitePropertyJson.ClientID %>").val(),
            data = JSON.parse(columnData);

        $(document).ready(function () {
            
            $('#box .list').html('');

            //init zTree data and create object
            $.fn.zTree.init($("#regionZTree"), setting, data);
            var zTree = $.fn.zTree.getZTreeObj("regionZTree"),
                nodes = zTree.getNodes();

            //get the value of select-box
            var selectValue = $('#<%=SiteLevel.ClientID %>').val();

            if (selectValue == "SiteCollection") {
                $('#box > div.select-box span.drop-btn').css({display: 'none'});
                $('#box .select-box').off('click');
            } else if (selectValue == "None"){
                $('#<%=PropertySetting.ClientID %>').css({ display: 'none' });
                $('#box').css({ display: 'none' });
                $('h3.ms-standardheader').eq(1).text('');
                $('#<%=PropertySetting.ClientID %> .ms-descriptiontext').eq(0).text('');
            }else {
                $('#box .select-box').off('click').on("click", function () {
                    $('#box .zTree-box').slideToggle();
                });
            }
            
            if (nodes.length > 0) {
                for (var i = 0; i < nodes.length; i++) {
                    zTree.expandNode(nodes[i], true, false, false);
                }
            }

            var rootNode = zTree.getNodesByFilter(function (node) {
                return node.level == 0;
            }, true);

            asyncSubNode(zTree, rootNode);

            var checkAll = true,
                selectNum = 0;

            if (selectValue == "Subsite") {
                $('#box .list').html('');
                checkAll = false;
                selectNum = '';
                $('#box .site-name').text(nodes[0].name);
            } else if (selectValue == "Both" || selectValue == "SiteCollection") {
                nodes[0].siteProperties.forEach(initList);
                $('.site-name').text(nodes[0].name);
            }

            $('#box .list li input').each(function (index, ele) {
                if (ele.checked == false) {
                    checkAll = false;
                } else {
                    ++selectNum;
                }
            });

            $('.select-all').unbind('change').change(function (e) {
                $('#box .list li input').each(function (index, ele) {
                    ele.checked = e.currentTarget.checked;
                    nodes[0].siteProperties[index].checked = ele.checked;
                });
                selectNum = e.currentTarget.checked ? $('#box .list li input').size() : 0;
                $('.select-num').text(selectNum);
            });
            

            $('#box .list li input').each(function (index, ele) {
                $(ele).change(function (e) {
                    nodes[0].siteProperties[index].checked = e.currentTarget.checked;
                    if (e.currentTarget.checked == false) {
                        --selectNum;
                        $('.select-all').prop('checked', false);
                    } else {
                        ++selectNum;
                    }
                    if (selectNum == $('#box .list li input').size()) {
                        $('.select-all').prop('checked', true);
                    }
                    $('.select-num').text(selectNum);
                });
            });

            $('.select-all').prop('checked',  checkAll);
            $('.select-num').text(selectNum);
        });

        function asyncSubNode(zTree, rootNode) {
            var subsiteNode = zTree.getNodesByFilter(function (node) {
                return node.isParent == true
            }, false, rootNode);
            for (var i = 0; i < subsiteNode.length; i++) {
                zTree.reAsyncChildNodes(subsiteNode[i], "refresh", "", zTreeOnAsyncSuccess)
            }
        }

        function zTreeOnAsyncSuccess(event, treeId, treeNode, msg) {
            if (treeNode) {
                var zTree = $.fn.zTree.getZTreeObj("regionZTree"),
                    nodes = zTree.getNodes();

                asyncSubNode(zTree, treeNode); 
            }
        }

        $('#<%=SiteLevel.ClientID %>').on('change', function () {

            $('#box .list').html('');

            var zTree = $.fn.zTree.getZTreeObj("regionZTree"),
                nodes = zTree.getNodes();

            var selectValue = $('#<%=SiteLevel.ClientID %>').val();

            $('.site-name').text(nodes[0].name);

            var selectNum = 0,
                checkAll = true;

            if (selectValue == 'SiteCollection') {
                nodes[0].siteProperties.forEach(initList);
                $('#box .select-box').off('click');
                $('#box > div.select-box span.drop-btn').css({ display: 'none' });
                $('#<%=PropertySetting.ClientID %>').css({ display: 'table-row' });
                $('#box').css({ display: 'block' });
                $('h3.ms-standardheader').eq(1).text('Site property configuration');
                $('#<%=PropertySetting.ClientID %> .ms-descriptiontext').eq(0).text('Select the properties to use for policy evaluation.');
            } else if (selectValue == 'None') {
                $('#<%=PropertySetting.ClientID %>').css({ display: 'none' });
                $('#box').css({ display: 'none' });
                $('h3.ms-standardheader').eq(1).text('');
                $('#<%=PropertySetting.ClientID %> .ms-descriptiontext').eq(0).text('');
            } else if (selectValue == 'Both') {
                nodes[0].siteProperties.forEach(initList);
                $('#box .select-box').off('click').on("click", function () {
                    $('#box .zTree-box').slideToggle();
                });
                $('#box > div.select-box span.drop-btn').css({ display: 'inline-block' });
                $('#<%=PropertySetting.ClientID %>').css({ display: 'table-row' });
                $('#box').css({ display: 'block' });
                $('h3.ms-standardheader').eq(1).text('Site property configuration');
                $('#<%=PropertySetting.ClientID %> .ms-descriptiontext').eq(0).text('Select the properties to use for policy evaluation.');
            } else {
                checkAll = false;
                selectNum = '';
                $('#box .select-box').off('click').on("click", function () {
                    $('#box .zTree-box').slideToggle();
                });
                $('#box > div.select-box span.drop-btn').css({ display: 'inline-block' });
                $('#<%=PropertySetting.ClientID %>').css({ display: 'table-row' });
                $('#box').css({ display: 'block' });
                $('h3.ms-standardheader').eq(1).text('Site property configuration');
                $('#<%=PropertySetting.ClientID %> .ms-descriptiontext').eq(0).text('Select the properties to use for policy evaluation.');
            }

            $('#box .list > li > input').each(function (index, ele) {
                $(ele).change(function (e) {
                    nodes[0].siteProperties[index].checked = e.currentTarget.checked;
                    if (e.currentTarget.checked == false) {
                        --selectNum;
                        checkAll = false;
                    } else {
                        ++selectNum;
                    }
                    $('.select-num').text(selectNum);
                    if (selectNum == $('#box .list li input').size()) {
                        checkAll = true;
                    }
                    $('.select-all').prop('checked', checkAll);
                });
            });

            $('.select-all').unbind('change').change(function (e) {
                var len = $('#box .list li input').size();
                for (var i = 0; i < len; i++) {
                    $('#box .list li input').eq(i).prop('checked', e.currentTarget.checked);
                    nodes[0].siteProperties[i].checked = e.currentTarget.checked;
                }
                selectNum = e.currentTarget.checked ? len : 0;
                $('.select-num').text(selectNum);
            });

            $('#box .zTree-box').slideUp();

            $('#box .list li input').each(function (index, ele) {
                if (ele.checked == false) {
                    checkAll = false;
                } else {
                    ++selectNum;
                }
            });
            $('.select-num').text(selectNum);
            $('.select-all').prop('checked', checkAll);
        });




        function myOnClick(event, treeId, treeNode, clickFlag) {

            var selectValue = $('#<%=SiteLevel.ClientID %>').val();
            
            if (selectValue == "Subsite" && treeNode.pId == "0") {
                return false;
            } else {
                $('#box .list').html('');

                treeNode.siteProperties.forEach(initList);
                var len = $('#box .list li input').size();

                $('#box .select-box .site-name').html(treeNode.name);

                var selectNum = 0,
                    checkAll = true;

                $('#box .list li input').each(function (index, ele) {
                    if (ele.checked == false) {
                        checkAll = false;
                    } else {
                        ++selectNum;
                    }
                });

                $('#box .list > li > input').each(function (index, ele) {
                    $(ele).change(function (e) {
                        treeNode.siteProperties[index].checked = e.currentTarget.checked;
                        if (e.currentTarget.checked == false) {
                            --selectNum;
                            checkAll = false;
                            $('.select-all').prop('checked', checkAll);
                        } else {
                            ++selectNum;
                        }
                        $('.select-num').text(selectNum);
                        if (selectNum == len) {
                            checkAll = true;
                            $('.select-all').prop('checked', checkAll);
                        }
                    });
                });

                $('.select-all').unbind('change').change(function (e) {

                    for (var i = 0; i < len; i++) {
                        $('#box .list li input').eq(i).prop('checked', e.currentTarget.checked);
                        treeNode.siteProperties[i].checked = e.currentTarget.checked;
                    }
                    if (e.currentTarget.checked) {
                        selectNum = len;
                        $('.select-num').html(selectNum);
                    } else {
                        selectNum = 0;
                        $('.select-num').html(selectNum);
                    }
                });

                $('.select-num').html(selectNum);
                $('.select-all').prop('checked', checkAll);

                $('#box .zTree-box').slideUp();
            }
        }

        $(document).on('click', function (e) {
            var e = e || window.event;
            if (e.target.className == "select-box" || e.target.id.indexOf('regionZTree') != -1 || e.target.className == "site-name") {
                return false;
            } else {
                $('#box .zTree-box').slideUp();
            }
        });

        function initList(item, index) {
                var oLi = document.createElement('li'),
                    oInp = document.createElement('input'),
                    oLabel = document.createElement('label');
                oInp.setAttribute('type', 'checkbox');
                oInp.setAttribute('name', index);
                oInp.setAttribute('id', index);
                oInp.checked = item.checked;
                oLabel.innerText = item.displayName;
                oLabel.setAttribute('for', index);
                oLi.appendChild(oInp);
                oLi.appendChild(oLabel);
                $('#box .list').append(oLi);
        }

        function updateData() {
            var jsondata =[];
            var zTree = $.fn.zTree.getZTreeObj("regionZTree");
            var node = zTree.getNodes();
            var nodes = zTree.transformToArray(node);
            for(var i = 0;i<nodes.length;i++) {
                var obj = {};
                obj.id = nodes[i].id;
                obj.pId = nodes[i].pId;
                obj.name = nodes[i].name;
                obj.checked = nodes[i].checked;
                obj.isParent = nodes[i].isParent;
                obj.isLoaded = nodes[i].isLoaded;
                obj.siteProperties = nodes[i].siteProperties;
                jsondata.push(obj);
            }
            return jsondata;
        }

        function trimData(target) {
            var len = target.length;
            for (var i = 0; i < len; i++) {
                var tempArr = [];
                for (var j = 0; j < target[i]["siteProperties"].length; j++) {

                    if (target[i]["siteProperties"][j].checked == true) {
                        tempArr.push(target[i]["siteProperties"][j]);
                    }
                }
                target[i]["siteProperties"] = tempArr;
            }

            return target;
        }
        
        function onSaveClick() {
            var newData = updateData();
            $("#<%=sitePropertyJson.ClientID %>").val(JSON.stringify(trimData(newData)));
        }

    </script>

    
</asp:Content>
