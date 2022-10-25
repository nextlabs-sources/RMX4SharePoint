using System;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web.UI.HtmlControls;

namespace NextLabs.RightsManager.Layouts.NextLabs.RightsManager
{
    public partial class FailedItemsList : UnsecuredLayoutsPageBase
    {
		protected HtmlGenericControl ilist;
		
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                SPUser user = this.Web.CurrentUser;
                if (!user.IsSiteAdmin)
                {
                    AppendDiv("Please log in with Site Administrator.", 0);
                    return;
                }
                string listGuid = Request.QueryString["List"];
                SPList list = this.Web.Lists[new Guid(listGuid)];
                ListSetInfor listSetInfor = Global.GetListSetInfor(list);
                List<BatchModeFailedModel> FailedItems = new List<BatchModeFailedModel>(listSetInfor.FailedItems);
                int total = FailedItems.Count;
                if(total == 0)
                {
                    AppendDiv("Failed items count is 0", 0);
                    return;
                }
                AppendTable(FailedItems);
            }
            catch (Exception ee)
            {
                ULSLogger.LogError(ee.ToString());
            }
        }
        protected void AppendDiv(string text, int qnum)
        {
            HtmlGenericControl createDiv = new HtmlGenericControl("Div" + qnum);
            createDiv.InnerHtml = text;
            ilist.Controls.Add(createDiv);
        }
        private void AppendTable(List<BatchModeFailedModel> FailedItems)
        {
            HtmlGenericControl table = new HtmlGenericControl("Table");
            table.Attributes.Add("cellpadding", "1");
            table.Attributes.Add("cellspacing", "0");
            table.Attributes.Add("border", "1");

            HtmlGenericControl thead = new HtmlGenericControl("Thead");
            HtmlGenericControl tbody = new HtmlGenericControl("Tbody");
            HtmlGenericControl theadTr = new HtmlGenericControl("Tr");
            HtmlGenericControl theadThForItem = new HtmlGenericControl("Th");
            HtmlGenericControl theadThForReason = new HtmlGenericControl("Th");
            theadThForItem.InnerText = "Item";
            theadThForReason.InnerText = "Reason";
            theadTr.Controls.Add(theadThForItem);
            theadTr.Controls.Add(theadThForReason);
            thead.Controls.Add(theadTr);
            table.Controls.Add(thead);
            table.Controls.Add(tbody);
            foreach (var item in FailedItems)
            {
                HtmlGenericControl tbodyTr = new HtmlGenericControl("Tr");
                HtmlGenericControl tdForItem = new HtmlGenericControl("Td");
                HtmlGenericControl tdForReason = new HtmlGenericControl("Td");
                tdForItem.InnerText = item.FileUrl;
                tdForReason.InnerText = item.Reason;
                tbodyTr.Controls.Add(tdForItem);
                tbodyTr.Controls.Add(tdForReason);
                tbody.Controls.Add(tbodyTr);
            }
            ilist.Controls.Add(table);
        }
        protected void AppendBr()
        {
            HtmlGenericControl createDiv = new HtmlGenericControl("br");
            ilist.Controls.Add(createDiv);
        }
    }
}
