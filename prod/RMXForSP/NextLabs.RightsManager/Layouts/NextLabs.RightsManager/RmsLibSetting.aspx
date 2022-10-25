<!--#include file="RmsLibSettingHeader.aspx" -->
<%@ Page language="C#" MasterPageFile="~/_layouts/application.master" Inherits="NextLabs.RightsManager.RmsLibSetting, NextLabs.RightsManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=c98953f573c68e1d" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 

<%@ Register TagPrefix="wssuc" TagName="LinksTable" src="~/_controltemplates/15/LinksTable.ascx" %> 
<%@ Register TagPrefix="wssuc" TagName="InputFormSection" src="~/_controltemplates/15/InputFormSection.ascx" %> 
<%@ Register TagPrefix="wssuc" TagName="InputFormControl" src="~/_controltemplates/15/InputFormControl.ascx" %> 
<%@ Register TagPrefix="wssuc" TagName="LinkSection" src="~/_controltemplates/15/LinkSection.ascx" %> 
<%@ Register TagPrefix="wssuc" TagName="ButtonSection" src="~/_controltemplates/15/ButtonSection.ascx" %> 
<%@ Register TagPrefix="wssuc" TagName="ActionBar" src="~/_controltemplates/15/ActionBar.ascx" %> 
<%@ Register TagPrefix="wssuc" TagName="ToolBar" src="~/_controltemplates/15/ToolBar.ascx" %> 
<%@ Register TagPrefix="wssuc" TagName="ToolBarButton" src="~/_controltemplates/15/ToolBarButton.ascx" %> 
<%@ Register TagPrefix="wssuc" TagName="Welcome" src="~/_controltemplates/15/Welcome.ascx" %>


<asp:Content contentplaceholderid="PlaceHolderPageTitle" runat="server">
<wssawc:EncodedLiteral runat="server" Text="NextLabs Rights Management" EncodeMethod="HtmlEncode" />
</asp:Content>
<asp:Content ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    <wssawc:EncodedLiteral runat="server" Text="NextLabs Rights Management" EncodeMethod="HtmlEncode" />
</asp:Content>
<asp:Content ContentPlaceHolderID="PlaceHolderPageDescription" runat="server">
    <wssawc:EncodedLiteral runat="server" ID="PageDescription" Text="Library Settings" EncodeMethod="HtmlEncode" />
</asp:Content>

<asp:Content contentplaceholderid="PlaceHolderMain" runat="server" style="margin-left:75px;">
    <wssawc:StyleBlock runat="server">
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



.switch{
  width:40px;
  height:20px;
  border-radius:30px;
  overflow: hidden;
  vertical-align:middle;
  position:relative;
  display: inline-block;
  background:#ccc;
  box-shadow: 0 0 1px #36a6d4;
}
.switch input{
  visibility: hidden;
}
.switch span{
  position:absolute;
  top:0;
  left:0;
  border-radius: 50%;
  background:#fff;
  width:50%;
  height:100%;
  transition:all linear 0.2s;
}
.switch span::before{
  position: absolute;
  top:0;
  left:-100%;
  content:'';
  width:200%;
  height:100%;
  border-radius: 30px;
  background:#36a6d4;
}
.switch span::after{
  content:'';
  position:absolute;
  left:0;
  top:0;
  width:100%;
  height:100%;
  border-radius: 50%;
  background:#fff;
}
.switch input:checked +span{
  transform:translateX(100%);
}



        #td-box {
            padding: 0;
        }
        #box {
            width: 350px;
        }
        #box .title{
            width: 350px;
            height: 35px;
            background: #EEEEEE;
            border-bottom: 1px solid #77778D;
            box-sizing: border-box;
            display: flex;
            align-items: center;
        }
        #box .title input{
            width: 15px;
            height: 15px;
        }

        #box .title span {
            width: 300px;
            height: 20px;
            line-height: 20px;
            text-align: left;
            margin: 0 10px;
        }

        #box .list {
            width: 350px;
            height: 340px;
            overflow-y: scroll;
            overflow-x: scroll;
            
        }
        #box .list li {
            height: 29px;
            text-align: left;
            list-style: none;
            line-height: 29px;
        }

        #box .list li input {
            width: 15px;
            height: 15px;
            margin: 0px 10px;
            position: relative;
            top: -2px
        }

        #box .list li label {
            white-space: nowrap;
            display: inline-block;
            width: 288px;
            line-height: 29px;
            height: 29px;
        }
     </wssawc:StyleBlock>
    <link  href="zTreeStyle.css" rel="stylesheet" />
     <link  href="simpleArtDialog.css" rel="stylesheet" />
    <script src="jquery-1.10.2.js"></script>
    <script src="jquery.artDialog.min.js?skin=simple"></script>
     <script src="jquery.ztree.all.min.js"></script>
     <div style="display:flex;margin-top: 15px;margin-left: -155px;margin-bottom: 5px;">
        <a id="backSite" style="margin-right:15px;font-size:25px;color:#003759; float:left;" href="/">NextLabs Rights Management setup</a>
        <div class="triangle_border_right">
        </div>
        <label id="titleLab" style="margin-left: 15px;font-size: 25px;color:#5d6878;">Library settings</label>
    </div>
    <div style="width: 2200px;margin-left: -165px;position: absolute;border-bottom: 1px solid #003399;"></div>
    <h2 id="titleH2" style="margin-bottom:5px;font-size:2.46em;margin-top:30px;">Library settings</h2>
    <h2 style="margin-bottom:10px;">Rights protection settings</h2>
    <input type="hidden" id="bList" value="" runat="server" name="bList" />
	<!--RMS BatchMode Setting-->
	<TABLE border="1" cellspacing="0" cellpadding="30px" class="ms-propertysheet" width="100%" style="max-width:900px;margin-bottom:30px;"> 
        <tr style="display:none;">
                <td valign="top" class="ms-formdescriptioncolumn-wide">
                    <table border="0" cellpadding="1" cellspacing="0" width="100%" summary="" role="presentation">
                        <tbody>
                            <tr>
                                <td class="ms-sectionheader" style="padding-top: 4px;" valign="top" height="22">
                                    <h3 class="ms-standardheader ms-inputformheader">Batch mode Schedule</h3>
                                    <input type="hidden" name="scheduleDiv" style="display:none;" id="scheduleDiv" value="" runat="server">
                                </td>
                            </tr>
                            <tr>
                                <td class="ms-descriptiontext ms-inputformdescription">Select the schedules for current library or list. </td>
                                <td width="10" height="2"></td>
                            </tr>
                            <tr>
                                <td width="150" height="19"></td>
                            </tr>
                        </tbody>
                    </table>
                </td>
                <td valign="top" align="left">
                    <table summary="" role="presentation" width="100%" cellspacing="0" cellpadding="0" border="0">
                        <tbody>
                            <tr>
                                <td width="9px" height="17px"></td>
                                <td height="17px" width="549.7px"></td>
                                <td height="17px" width="10px"></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td class="ms-authoringcontrols">
                                    <table class="ms-authoringcontrols" summary="" role="presentation" width="100%" cellspacing="0" cellpadding="0" border="0">
                                        <tbody>
                                            <tr>
                                                <td width="11px" valign="top">
                                                </td>
                                                <td>
                                                    <label>Schedule List</label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="20px" height="23px"></td>
                                                <td class="ms-authoringcontrols">
                                                    <select id="scheduleList" style="width:100%;">
                                                        <option value="None">None</option>
                                                    </select>
                                                    <div id="createScheduleDiv">
                                                        <a id="createScheduleHref" href="javascript:void(0)" onclick="popScheduleView()">Create schedule</a>
                                                    </div>
                                                    <div id="updateScheduleDiv" style="display:none;">
                                                        <a id="updateScheduleHref" href="javascript:void(0)" onclick="popScheduleView()">Edit schedule</a>
                                                        <a id="deleteScheduleHref" href="javascript:void(0)" onclick="deleteScheduleData()">Delete all schedule</a>
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" height="2px"></td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                                <td width="10px" height="60.25px"></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td></td>
                                <td></td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
        <tr>
             <td style="max-width:500px;">
                 <div style="margin-bottom:5px;font-weight:bold;">Batch processing</div>
                 <div style="margin-bottom:10px;">Run NextLabs rights protection in batch mode.Click on the slider to run the batch mode.</div>
                 <wssawc:EncodedLiteral runat="server" id="BatchLogEX" text="" EncodeMethod='HtmlEncode'/>
				<asp:HyperLink ID="FailedItemsLink" runat="server" Visible="false" NavigateUrl="" Text="Click this link to view unencrypted items."></asp:HyperLink>
             </td>
             <td style="min-width:80px;vertical-align:middle;text-align:center;">
                  <input type="hidden" id="hiddenCAStatusCheckBox" name="hiddenCAStatusCheckBox" value="" runat="server" />
                 <%--<input type="checkbox" id="CAStatusCheckBox" />--%>
                <label class='switch'>
                    <input type='checkbox' id="CAStatusCheckBox">
                    <span></span>
                </label>
             </td>
         </tr>
        <tr>
             <td>
                 <div style="margin-bottom:5px;font-weight:bold;">Apply protection to all versions</div>
                 <div>Protect all versions of the file, from the oldest to the newest versions.Click on the slider to enable this option.</div>
             </td>
             <td style="min-width:80px;vertical-align:middle;text-align:center;">
                  <input type="hidden" id="hiddenVersionsRMSCheckBox" name="hiddenVersionsRMSCheckBox" value="" runat="server" />
                 <%--<input type="checkbox" id="VersionsRMSCheckBox" />--%>
                <label class='switch'>
                    <input type='checkbox' id="VersionsRMSCheckBox">
                    <span></span>
                </label>
             </td>
         </tr>
        <tr>
             <td>
                 <div style="margin-bottom:5px;font-weight:bold;">Delete source file</div>
                 <div>Delete the source file after the rights protection process is completed.Click on the slider to enable this option.</div>
             </td>
             <td style="min-width:80px;vertical-align:middle;text-align:center;">
                 <input type="hidden" id="hiddenDeleteFileCheck" name="hiddenDeleteFileCheck" value="" runat="server" />
                 <%--<input type="checkbox" id="DeleteFileCheck" />--%>
                <label class='switch'>
                    <input type='checkbox' id="DeleteFileCheck">
                    <span></span>
                </label>
             </td>
         </tr>
         <tr>
             <td>
                 <div style="margin-bottom:5px;font-weight:bold;">Column configuration</div>
                 <div>Select the columns to use for policy evaluation.</div>
             </td>
             <td style="min-width:80px;vertical-align:middle;text-align:center;" id="td-box">
                <input type="hidden" name="columnJson" style="display:none;" id="columnJson" value="" runat="server">
                <div id="box">
                    <div class="title">
                        <input type="checkbox" class="select-all"/>
                        <span>Documents</span>
                    </div>
                    <div class="list"></div>
                </div>
             </td>
         </tr>
	</TABLE>
   <asp:Button runat="server" class="btn" style="margin-left:0px;" OnClientClick="UpdateSelectedColumn()" OnClick="BtnOK_Click" Text="Save" id="BtnOK" accesskey="<%$Resources:wss,okbutton_accesskey%>"/>
   <asp:Button runat="server" class="btn" OnClick="BtnCancel_Click" Text="<%$Resources:wss,multipages_cancelbutton_text%>" id="BtnCancel" CausesValidation="false"/>

     <div id="scheduleView" style="display:none;width:540px;font-family: tahoma;font-size: 11pt;">
        <div>
            <h1 style="font-size: 1.5em;font-weight: 100;">Manage Schedules</h1>
        </div>
        <div id="container">
            <table>
                <tbody>
                    <tr valign="top">
                        <td>
                            <table style="table-layout:fixed;">
                                <tbody>
                                    <tr>
                                        <td colspan="3" style="padding-top: 8px;padding-bottom: 10px;font-size:11pt;">
                                            <span style="color: #ff0000;font-family: verdana;font-size: 8pt;">*</span>
                                            Indicates a required field
                                        </td>
                                    </tr>
                                    <tr style="background-color:#d9d9d9;">
                                        <td colspan="3" height="1"></td>
                                    </tr>
                                    <tr>
                                        <td name="左一" style="padding-bottom:20px;width:200px;" valign="top">
                                            <h3 style="padding-bottom:8px;margin:0em;">Type</h3>
                                            <div style="padding-bottom:10px;">Select the type of schedule.</div>
                                        </td>
                                        <td style="font-family: verdana;font-size: .7em;text-align: left;color: #4c4c4c;"></td>
                                        <td name="右一" style="padding-bottom: 20px; padding-left: 8px; border-top: 0px solid white; padding-right: 10px;background-color: #ebf3ff;color: #525252;" valign="top">
                                            <div>
                                                <div style="margin-bottom: 5px;">
                                                    <input id="scheduleMinutely" type="radio" name="scheduleDetails" value="scheduleMinutely" onclick="SelectScheduleType(1)">
                                                    <label for="scheduleMinutely">Minutely</label>
                                                </div>
                                                <div style="margin-bottom: 5px;">
                                                    <input id="scheduleHourly" type="radio" name="scheduleDetails" value="scheduleHourly" onclick="SelectScheduleType(2)">
                                                    <label for="scheduleHourly">Hourly</label>
                                                </div>
                                                <div style="margin-bottom: 5px;">
                                                    <input id="scheduleDaily" type="radio" name="scheduleDetails" value="scheduleDaily"  onclick="SelectScheduleType(3)">
                                                    <label for="scheduleDaily">Daily</label>
                                                </div>
                                                <div style="margin-bottom: 5px;">
                                                    <input id="scheduleWeekly" type="radio" name="scheduleDetails" value="scheduleWeekly" onclick="SelectScheduleType(4)">
                                                    <label for="scheduleWeekly">Weekly</label>
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr style="background-color:#d9d9d9;">
                                        <td colspan="3" height="1"></td>
                                    </tr>
                                    <tr>
                                        <td name="左二" style="padding-bottom: 20px;" valign="top">
                                            <h3 style="padding-bottom: 8px; margin: 0em;">Settings</h3>
                                            <div style="padding-bottom: 10px;">Type the schedule settings.</div>
                                        </td>
                                        <td style="font-family: verdana;font-size: .7em;text-align: left;color: #4c4c4c;"></td>
                                        <td name="右二" style="background-color: #ebf3ff;color: #525252;">
                                          <table style="table-layout:fixed;">
                                              <tbody id="MinutelyBody" style="display:none;">
                                                  <tr valign="top">
                                                      <td colspan="2" style="width:85px;">
                                                          Run every:
                                                          <span style="color: #ff0000;font-family: verdana;font-size: 8pt;">*</span>
                                                      </td>
                                                      <td style="width:200px;">
                                                          <input id="minutelyValue" type="text" value="1" style="width:50px;" />
                                                          <span> minutes</span>
                                                      </td>
                                                  </tr>
                                              </tbody>
                                              <tbody id="HourlylyBody" style="display:none;">
                                                  <tr valign="top">
                                                      <td colspan="2" style="width:85px;">
                                                          Run every:
                                                          <span style="color: #ff0000;font-family: verdana;font-size: 8pt;">*</span>
                                                      </td>
                                                      <td style="width:200px;">
                                                          <input id="hourlyValue" type="text" value="1" style="width:50px;" />
                                                          <span> hours</span>
                                                      </td>
                                                  </tr>
                                              </tbody>
                                              <tbody id="DailyBody" style="display:none;">
                                                  <tr valign="top">
                                                      <td colspan="2" style="width:85px;">
                                                          Run every:
                                                          <span style="color: #ff0000;font-family: verdana;font-size: 8pt;">*</span>
                                                      </td>
                                                      <td style="width:200px;">
                                                          <input id="dailyValue" type="text" value="1" style="width:50px;" />
                                                          <span>  days</span>
                                                      </td>
                                                  </tr>
                                                  <tr>
                                                      <td colspan="2">
                                                          <label>Starting time:</label>
                                                      </td>
                                                      <td>
                                                          <select id="dailyStartTimeList" style="">
                                                              <option value="12:00 AM">12:00 AM</option>
                                                              <option selected="selected" value="1:00 AM">1:00 AM</option>
                                                              <option value="2:00 AM">2:00 AM</option>
                                                              <option value="3:00 AM">3:00 AM</option>
                                                              <option value="4:00 AM">4:00 AM</option>
                                                              <option value="5:00 AM">5:00 AM</option>
                                                              <option value="6:00 AM">6:00 AM</option>
                                                              <option value="7:00 AM">7:00 AM</option>
                                                              <option value="8:00 AM">8:00 AM</option>
                                                              <option value="9:00 AM">9:00 AM</option>
                                                              <option value="10:00 AM">10:00 AM</option>
                                                              <option value="11:00 AM">11:00 AM</option>
                                                              <option value="12:00 PM">12:00 PM</option>
                                                              <option value="1:00 PM">1:00 PM</option>
                                                              <option value="2:00 PM">2:00 PM</option>
                                                              <option value="3:00 PM">3:00 PM</option>
                                                              <option value="4:00 PM">4:00 PM</option>
                                                              <option value="5:00 PM">5:00 PM</option>
                                                              <option value="6:00 PM">6:00 PM</option>
                                                              <option value="7:00 PM">7:00 PM</option>
                                                              <option value="8:00 PM">8:00 PM</option>
                                                              <option value="9:00 PM">9:00 PM</option>
                                                              <option value="10:00 PM">10:00 PM</option>
                                                              <option value="11:00 PM">11:00 PM</option>
                                                          </select>
                                                      </td>
                                                  </tr>
                                              </tbody>
                                              <tbody id="weeklyBody" style="display:none;">
                                                  <tr valign="top">
                                                      <td colspan="2" style="width:85px;">
                                                          Run every:
                                                          <span style="color: #ff0000;font-family: verdana;font-size: 8pt;">*</span>
                                                      </td>
                                                      <td style="width:200px;">
                                                          <input id="weeklyValue" type="text" value="1" style="width:50px;" />
                                                          <span>weeks</span>
                                                      </td>
                                                  </tr>
                                                  <tr>
                                                      <td colspan="2" >
                                                          <label>On:</label>
                                                          <span style="color: #ff0000;font-family: verdana;font-size: 8pt;">*</span>
                                                      </td>
                                                      <td>
                                                          <div>
                                                              <input id="weeklyDayMonday" type="checkbox" name="weeklyDayMonday">
                                                              <label for="weeklyDayMonday">Monday</label>
                                                          </div>
                                                          <div>
                                                              <input id="weeklyDayTuesday" type="checkbox" name="weeklyDayTuesday">
                                                              <label for="weeklyDayTuesday">Tuesday</label>
                                                          </div>
                                                          <div>
                                                              <input id="weeklyDayWednesday" type="checkbox" name="weeklyDayWednesday">
                                                              <label for="weeklyDayWednesday">Wednesday</label>
                                                          </div>
                                                          <div>
                                                              <input id="weeklyDayThursday" type="checkbox" name="weeklyDayThursday">
                                                              <label for="weeklyDayThursday">Thursday</label>
                                                          </div>
                                                          <div>
                                                              <input id="weeklyDayFriday" type="checkbox" name="weeklyDayFriday">
                                                              <label for="weeklyDayFriday">Friday</label>
                                                          </div>
                                                          <div>
                                                              <input id="weeklyDaySaturday" type="checkbox" name="weeklyDaySaturday">
                                                              <label for="weeklyDaySaturday">Saturday</label>
                                                          </div>
                                                          <div>
                                                              <input id="weeklyDaySunday" type="checkbox" checked="checked" name="weeklyDaySunday">
                                                              <label for="weeklyDaySunday">Sunday</label>
                                                          </div>
                                                      </td>
                                                  </tr>
                                                  <tr>
                                                      <td colspan="2">
                                                          <label>Starting time:</label>
                                                      </td>
                                                      <td>
                                                          <select id="weeklyStartTimeList" style="">
                                                              <option value="12:00 AM">12:00 AM</option>
                                                              <option selected="selected" value="1:00 AM">1:00 AM</option>
                                                              <option value="2:00 AM">2:00 AM</option>
                                                              <option value="3:00 AM">3:00 AM</option>
                                                              <option value="4:00 AM">4:00 AM</option>
                                                              <option value="5:00 AM">5:00 AM</option>
                                                              <option value="6:00 AM">6:00 AM</option>
                                                              <option value="7:00 AM">7:00 AM</option>
                                                              <option value="8:00 AM">8:00 AM</option>
                                                              <option value="9:00 AM">9:00 AM</option>
                                                              <option value="10:00 AM">10:00 AM</option>
                                                              <option value="11:00 AM">11:00 AM</option>
                                                              <option value="12:00 PM">12:00 PM</option>
                                                              <option value="1:00 PM">1:00 PM</option>
                                                              <option value="2:00 PM">2:00 PM</option>
                                                              <option value="3:00 PM">3:00 PM</option>
                                                              <option value="4:00 PM">4:00 PM</option>
                                                              <option value="5:00 PM">5:00 PM</option>
                                                              <option value="6:00 PM">6:00 PM</option>
                                                              <option value="7:00 PM">7:00 PM</option>
                                                              <option value="8:00 PM">8:00 PM</option>
                                                              <option value="9:00 PM">9:00 PM</option>
                                                              <option value="10:00 PM">10:00 PM</option>
                                                              <option value="11:00 PM">11:00 PM</option>
                                                          </select>
                                                      </td>
                                                  </tr>
                                              </tbody>
                                          </table>
                                        </td>
                                    </tr>
                                    <tr style="background-color:#d9d9d9;">
                                        <td colspan="3" height="1"></td>
                                    </tr>
                                </tbody>
                            </table>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
       <div style="margin-top:15px;">
           <input type="button" style="margin-left:340px;" class="btn" value="OK" onclick="SaveScheduleData()" />
           <span>
               <input type="button" class="btn" value="Cancel" onclick="CloseDialog()" />
           </span>
       </div>
    </div>
     <script type="text/javascript">
         var batchModeCheck = $("#<%=hiddenCAStatusCheckBox.ClientID %>").val();
          var deleteFileCheck = $("#<%=hiddenDeleteFileCheck.ClientID %>").val();
         var protectVersionCheck = $("#<%=hiddenVersionsRMSCheckBox.ClientID %>").val();
         var bList = $("#<%=bList.ClientID %>").val();

         var batchModeEnabled =$("#<%=hiddenCAStatusCheckBox.ClientID %>").prop("disabled");
         var deleteFileEnabled = $("#<%=hiddenDeleteFileCheck.ClientID %>").prop("disabled");
         var protectVersionEnabled = $("#<%=hiddenVersionsRMSCheckBox.ClientID %>").prop("disabled");
         $("#<%=hiddenCAStatusCheckBox.ClientID %>").prop("disabled", false);
         $("#<%=hiddenDeleteFileCheck.ClientID %>").prop("disabled",false);
         $("#<%=hiddenVersionsRMSCheckBox.ClientID %>").prop("disabled",false);
         if (bList == "true") {
             $("#titleLab").text("List Settings");
             $("#titleH2").html("List Settings");
         }
         if (deleteFileCheck == "false") {
             $("#DeleteFileCheck").prop("checked", false);
         } else {
             $("#DeleteFileCheck").prop("checked", true);
         }
         if (protectVersionCheck == "false") {
             $("#VersionsRMSCheckBox").prop("checked", false);
         } else {
             $("#VersionsRMSCheckBox").prop("checked", true);
         }
         $("#<%=BtnOK.ClientID %>").prop("disabled", batchModeEnabled);
         $("#CAStatusCheckBox").prop("disabled", batchModeEnabled);
         $("#VersionsRMSCheckBox").prop("disabled", protectVersionEnabled);
         $("#DeleteFileCheck").prop("disabled", deleteFileEnabled);
         $("#DeleteFileCheck").change(function () {
             var bList = $("#<%=bList.ClientID %>").val();
             if (bList == "false") {
                 if ($("#DeleteFileCheck").prop('checked')) {
                     $("#VersionsRMSCheckBox").prop("checked", true);
                     $("#VersionsRMSCheckBox").prop("disabled", true);
                 } else {
                     $("#VersionsRMSCheckBox").prop("disabled", false);
                 }
             } else {
                 $("#VersionsRMSCheckBox").prop("disabled", true);
             }
             $("#<%=hiddenDeleteFileCheck.ClientID %>").val($("#DeleteFileCheck").prop('checked'));
             $("#<%=hiddenVersionsRMSCheckBox.ClientID %>").val($("#VersionsRMSCheckBox").prop('checked'));
         });
         $("#CAStatusCheckBox").change(function () {
             $("#<%=hiddenCAStatusCheckBox.ClientID %>").val($("#CAStatusCheckBox").prop('checked'));
         });
         $("#VersionsRMSCheckBox").change(function () {
             $("#<%=hiddenVersionsRMSCheckBox.ClientID %>").val($("#VersionsRMSCheckBox").prop('checked'));
          }); 
        var columnData = $("#<%=columnJson.ClientID %>").val();
        var data = JSON.parse(columnData);
        $('#box .title span').text("Column display name(InternalName)");
        data = data.filter(function (ele, index) {
            return ele.pId != 0;
        });
        var checkAll = true;
        var selectNum = 0;
        for (var i = 0; i < data.length; i++) {

            if (data[i].checked == false) {
                checkAll = false;
            } else {
                selectNum ++;
            }
            var oLi = document.createElement('li'),
                oInp = document.createElement('input'),
                oLabel = document.createElement('label');
            oInp.setAttribute('type', 'checkbox');
            oInp.setAttribute('id', i);
            oInp.setAttribute('name', i);
            oInp.checked = data[i].checked;
            oLabel.innerText = data[i].name;
            oLabel.setAttribute('for', i);
            oLi.appendChild(oInp);
            oLi.appendChild(oLabel);
            $('#box .list').append(oLi);
        };

        $('.select-all').prop('checked', checkAll);
        
        $('.select-all').change(function (e) {
            $('#box .list li input').each(function (index, ele) {
                ele.checked = e.currentTarget.checked;
                data[index].checked = ele.checked;
            });
            if (!e.currentTarget.checked) {
                checkAll = false;
            } else {
                selectNum = $('#box .list li input').size();
            }
        });

        $('#box .list li input').each(function (index, ele) {
            $(ele).change(function (e) {
                data[index].checked = e.currentTarget.checked;
                if (!e.currentTarget.checked) {
                    selectNum --;
                    checkAll = false;
                    $('.select-all').prop('checked', checkAll);
                } else {
                    selectNum ++;
                    if (selectNum == $('#box .list li input').size()) {
                        checkAll = true;
                        $('.select-all').prop('checked', checkAll);
                    }
                }
            });
            
        });

        function UpdateSelectedColumn() {
            $("#<%=columnJson.ClientID %>").val(JSON.stringify(data)) ;
        }


        

     
         function getQueryString(name) {
             var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
             var r = window.location.search.substr(1).match(reg);
             if (r != null) return unescape(r[2]); return null;
         }
         var url = window.location.href;
         var backUrl = url.replace("NextLabs.RightsManager/RmsLibSetting.aspx", "listedit.aspx");
         $('#backSite').attr('href', backUrl);
         var scheduleDataList = document.getElementById("<%=scheduleDiv.ClientID %>").value;
        if (scheduleDataList != '') {
            scheduleDataList = JSON.parse(scheduleDataList);
            initScheduleList(scheduleDataList);
        } else {
            scheduleDataList = [];
            $("#scheduleList").val("None");
            $("#createScheduleDiv").css('display', 'block');
            $("#updateScheduleDiv").css('display','none');
            $("#scheduleDaily").prop("checked",true);
            $("#DailyBody").css('display','table-row-group');
        }
        function saveScheduleData() {
            document.getElementById("<%=scheduleDiv.ClientID %>").value = JSON.stringify(scheduleDataList);
           // $("#scheduleDiv").val(JSON.stringify(scheduleDataList));
        }
        function initScheduleList(scheduleList){
            var hasSelect=false;
            for(var i =0;i<scheduleList.length;i++){
                var htmlText = "<option value='" + scheduleList[i].DisplayText + "'>" + scheduleList[i].DisplayText + "</option>";
                $("#scheduleList").append(htmlText);
                if(scheduleList[i].IsSelected==true){
                    hasSelect=true;
                    $("#scheduleList").val(scheduleList[i].DisplayText);
                    initScheduleView(scheduleList[i]);
                    $("#createScheduleDiv").css('display', 'none');
                    $("#updateScheduleDiv").css('display','block');
                }
            }
            if(!hasSelect){
                $("#createScheduleDiv").css('display', 'block');
                $("#updateScheduleDiv").css('display','none');
                SelectScheduleType(3);
            }
        }
        function initScheduleView(schedule){
            if(schedule.ScheduleType=="Minutely"){
                SelectScheduleType(1);
                $("#minutelyValue").val(schedule.TimeInterval);
            }else if(schedule.ScheduleType=="Hourly"){
                SelectScheduleType(2);
                $("#hourlyValue").val(schedule.TimeInterval);
            }else if(schedule.ScheduleType=="Daily"){
                SelectScheduleType(3);
                $("#dailyValue").val(schedule.TimeInterval);
                $("#dailyStartTimeList").val(schedule.StartTime);
            }else if(schedule.ScheduleType=="Weekly"){
                SelectScheduleType(4);
                $("#weeklyValue").val(schedule.TimeInterval);
                $("#weeklyDayMonday").prop("checked", schedule.SpecificDays.indexOf("Mon")!=-1);
                $("#weeklyDayTueseday").prop("checked", schedule.SpecificDays.indexOf("Tue")!=-1);
                $("#weeklyDayWednesday").prop("checked", schedule.SpecificDays.indexOf("Wed")!=-1);
                $("#weeklyDayThursday").prop("checked", schedule.SpecificDays.indexOf("Thu")!=-1);
                $("#weeklyDayFriday").prop("checked", schedule.SpecificDays.indexOf("Fri")!=-1);
                $("#weeklyDaySaturday").prop("checked", schedule.SpecificDays.indexOf("Sat")!=-1);
                $("#weeklyDaySunday").prop("checked", schedule.SpecificDays.indexOf("Sun")!=-1);
                $("#weeklyStartTimeList").val(schedule.StartTime);
            }
        }
        var dialog={};
        function popScheduleView() {
            dialog = $.dialog({
                lock:true,
                title: 'Manage Schedules',
                content: $("#scheduleView")[0]
            });
        }
        function CloseDialog(){
            dialog.close();
        }
        function SaveScheduleData() {
            var scheduleData = {};
            if ($("#scheduleMinutely").is(':checked')) {
                var minutelyValue = $("#minutelyValue").val();
                scheduleData.ScheduleType = "Minutely";
                scheduleData.TimeInterval = minutelyValue;
                scheduleData.SpecificDays = "";
                scheduleData.StartTime = "";
                var displayText = "Every " + minutelyValue + " minute(s)";
                scheduleData.DisplayText = displayText;
                var htmlText = "<option value='" + displayText + "'>" + displayText + "</option>";
                $("#scheduleList").append(htmlText);
                $("#scheduleList").val(displayText);
            } else if ($("#scheduleHourly").is(':checked')) {
                var hourlyValue = $("#hourlyValue").val();
                scheduleData.ScheduleType = "Hourly";
                scheduleData.TimeInterval = hourlyValue;
                scheduleData.SpecificDays = "";
                scheduleData.StartTime = "";
                var displayText = "Every " + hourlyValue + "hour(s)";
                var htmlText = "<option value='" + displayText + "'>" + displayText + "</option>";
                scheduleData.DisplayText = displayText;
                $("#scheduleList").append(htmlText);
                $("#scheduleList").val(displayText);
            } else if ($("#scheduleDaily").is(':checked')) {
                var dailyValue = $("#dailyValue").val();
                var startTime = $("#dailyStartTimeList").val();
                scheduleData.ScheduleType = "Daily";
                scheduleData.TimeInterval = dailyValue;
                scheduleData.SpecificDays = "";
                scheduleData.StartTime = startTime;
                var displayText = "At " + startTime + " every " + dailyValue + " day(s)";
                var htmlText = "<option value='" + displayText + "'>" + displayText + "</option>";
                scheduleData.DisplayText = displayText;
                $("#scheduleList").append(htmlText);
                $("#scheduleList").val(displayText);

            } else if ($("#scheduleWeekly").is(':checked')) {
                var mondayChecked =$("#weeklyDayMonday").is(':checked');
                var tuesdayChecked =$("#weeklyDayTuesday").is(':checked');
                var wednesdayChecked =$("#weeklyDayWednesday").is(':checked');
                var thursChecked =$("#weeklyDayThursday").is(':checked');
                var fridayChecked =$("#weeklyDayFriday").is(':checked');
                var saturdayChecked =$("#weeklyDaySaturday").is(':checked');
                var sundayChecked =$("#weeklyDaySunday").is(':checked');
                if(!(mondayChecked||tuesdayChecked||wednesdayChecked||thursChecked||fridayChecked||saturdayChecked||sundayChecked)){
                    alert("At least one day must be selected.");
                    return false;
                }
                var weeklyValue = $("#weeklyValue").val();
                var startTime = $("#weeklyStartTimeList").val();
                scheduleData.ScheduleType = "Weekly";
                scheduleData.TimeInterval = weeklyValue;
                scheduleData.SpecificDays = "";
                scheduleData.StartTime = startTime;
                var specificDays="";
                if(mondayChecked){
                    scheduleData.SpecificDays+="monday,";
                    specificDays+="Mon,";
                }
                if(tuesdayChecked){
                    scheduleData.SpecificDays+="tuesday,";
                    specificDays+="Tue,";
                }
                if(wednesdayChecked){
                    scheduleData.SpecificDays+="wednesday,";
                    specificDays+="Wed,";
                }
                if(thursChecked){
                    scheduleData.SpecificDays+="thursday,";
                    specificDays+="Thu,";
                }
                if(fridayChecked){
                    scheduleData.SpecificDays+="friday,";
                    specificDays+="Fri,";
                }
                if(saturdayChecked){
                    scheduleData.SpecificDays+="saturday,";
                    specificDays+="Sat,";
                }
                if(sundayChecked){
                    scheduleData.SpecificDays+="sunday";
                    specificDays+="Sun";
                }
                var displayText = "At " + startTime + " every " + specificDays + ",of every " + weeklyValue + " week(s)";
                var htmlText = "<option value='" + displayText + "'>" + displayText + "</option>";
                scheduleData.DisplayText = displayText;
                $("#scheduleList").append(htmlText);
                $("#scheduleList").val(displayText);
            }
            scheduleData.IsSelected = true;
            $("#createScheduleDiv").css('display', 'none');
            $("#updateScheduleDiv").css('display', 'block');
            for (var i = 0; i < scheduleDataList.length; i++) {
                scheduleDataList[i].IsSelected = false;
            }
            scheduleDataList.push(scheduleData);
            CloseDialog();
        }
        function SelectScheduleType(type) {
            if(type==1){
                $("#scheduleMinutely").prop("checked",true);
                $("#MinutelyBody").css('display', 'table-row-group');
                $("#HourlylyBody").css('display', 'none');
                $("#DailyBody").css('display', 'none');
                $("#weeklyBody").css('display', 'none');
            } else if (type == 2) {
                $("#scheduleHourly").prop("checked",true);
                $("#MinutelyBody").css('display', 'none');
                $("#HourlylyBody").css('display', 'table-row-group');
                $("#DailyBody").css('display', 'none');
                $("#weeklyBody").css('display', 'none');
            } else if (type == 3) {
                $("#scheduleDaily").prop("checked",true);
                $("#MinutelyBody").css('display', 'none');
                $("#HourlylyBody").css('display', 'none');
                $("#DailyBody").css('display', 'table-row-group');
                $("#weeklyBody").css('display', 'none');
            } else if (type == 4) {
                $("#scheduleWeekly").prop("checked",true);
                $("#MinutelyBody").css('display', 'none');
                $("#HourlylyBody").css('display', 'none');
                $("#DailyBody").css('display', 'none');
                $("#weeklyBody").css('display', 'table-row-group');
            }
        }
        function deleteScheduleData(scheduleData) {
            SelectScheduleType(3);
            $("#minutelyValue").val(1);
            $("#hourlyValue").val(1);
            $("#dailyValue").val(1);
            $("#dailyStartTimeList").val(1);
            $("#weeklyValue").val(1);
            $("#weeklyDayMonday").prop("checked", false);
            $("#weeklyDayTueseday").prop("checked", false);
            $("#weeklyDayWednesday").prop("checked", false);
            $("#weeklyDayThursday").prop("checked", false);
            $("#weeklyDayFriday").prop("checked", false);
            $("#weeklyDaySaturday").prop("checked", false);
            $("#weeklyDaySunday").prop("checked", true);
            $("#weeklyStartTimeList").val(1);
        }
        $("#scheduleList").change(function () {
            if ($("#scheduleList").val() != "None") {
                for (var i = 0; i < scheduleDataList.length; i++) {
                    if (scheduleDataList[i].DisplayText == $("#scheduleList").val()) {
                        scheduleDataList[i].IsSelected = true;
                        initScheduleView(scheduleDataList[i]);
                        break;
                    }
                }
                $("#createScheduleDiv").css('display', 'none');
                $("#updateScheduleDiv").css('display', 'block');
            } else {
                $("#createScheduleDiv").css('display', 'block');
                $("#updateScheduleDiv").css('display', 'none');
                for (var i = 0; i < scheduleDataList.length; i++) {
                    scheduleDataList[i].IsSelected = false;
                }
            }
        })
        function deleteScheduleData() {
            $("#scheduleList option:not(:first)").remove();
            $("#createScheduleDiv").css('display', 'block');
            $("#updateScheduleDiv").css('display', 'none');
            scheduleDataList = [];
        }
    </script>
</asp:Content>
