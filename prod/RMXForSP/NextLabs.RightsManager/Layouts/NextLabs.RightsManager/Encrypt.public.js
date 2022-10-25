function getItemIds(){
    var itemIds = '';
    var items = SP.ListOperation.Selection.getSelectedItems();
    var item;
    for(var i in items)
    {
        item = items[i];
        if(itemIds != '')
        {
            itemIds = itemIds + ',';
        }
        itemIds = itemIds + item.id;
    }
    return itemIds;
}


function checkRPIsEnabled() {
    var bEnable = false;
    var selectedItems = SP.ListOperation.Selection.getSelectedItems();
    var count = selectedItems.length;
    if (count > 1 || count == 0) return bEnable;

    var listItem = null;
    try {
        listItem = ctx.ListData.Row.filter(function (row) {
            return row.ID == selectedItems[0].id;
        })[0];
    }
    catch (error) {
    }

    if (listItem == null) return bEnable;
    try{
        if (listItem.HTML_x0020_File_x0020_Type.startsWith('OneNote.Notebook')) return bEnable;
    }
    catch(error){}
    try{
        if (listItem.ContentType != null &&
       (listItem.ContentType == 'Folder' || listItem.ContentType == 'Document Set')) return bEnable;
    }
    catch (error) { }
   
    if (!listItem.FileLeafRef.endsWith('.nxl')) bEnable = true;
    return bEnable;
}

function checkSVIsEnabled() {
    var bEnable = false;
    var selectedItems = SP.ListOperation.Selection.getSelectedItems();
    var count = selectedItems.length;
    if (count > 1 || count == 0) return bEnable;

    var listItem = null;
        try {
        listItem = ctx.ListData.Row.filter(function (row) {
            return row.ID == selectedItems[0].id;
        })[0];
    }
    catch (error) {
    }
   
    if (listItem == null) return bEnable;

    if (listItem.HTML_x0020_File_x0020_Type.startsWith('OneNote.Notebook')) return bEnable;

    if (listItem.ContentType != null &&
        (listItem.ContentType == 'Folder' || listItem.ContentType == 'Document Set')) return bEnable;
    if (listItem.FileLeafRef.endsWith('.nxl')) bEnable = true;

    return bEnable;
}

function getSelectedListId(){
	var id = "";
	var selectedListId = SP.ListOperation.Selection.getSelectedList();
	var pageListid = _spPageContextInfo.pageListId;
    if(selectedListId != null){
		id = selectedListId;
	}else if(pageListid!=null){
		id = pageListid;
	}
    return id;
}
