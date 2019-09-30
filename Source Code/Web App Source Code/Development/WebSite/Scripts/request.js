function fgRequests_ItemFormatter(panel, r, c, cell) {
    if (panel.cellType == wijmo.grid.CellType.Cell) {
        var grid = panel.grid;
        var dataItem = grid.rows[r].dataItem;
        if (dataItem && dataItem.ObjectId) {
            switch (panel.columns[c].name) {
                case 'Name':
                    switch (dataItem.Type) {
                        case 'Activity':
                            cell.innerHTML = (
                                '<a href="/Activity/View/' + dataItem.ObjectId + '">' +
                                dataItem.Name + '</a>');
                            break;

                        case 'Indicator':
                            cell.innerHTML = (
                                '<a href="/Indicator/View/' + dataItem.ObjectId + '">' +
                                dataItem.Name + '</a>');
                            break;

                        case 'Site':
                            cell.innerHTML = (
                                '<a href="/Site/View/' + dataItem.ObjectId + '">' +
                                dataItem.Name + '</a>');
                            break;

                        case 'User':
                            cell.innerHTML = (
                                '<a href="/User/View/' + dataItem.ObjectId + '">' +
                                dataItem.Name + '</a>');
                            break;

                        case 'Attachment':
                            cell.innerHTML = (
                                '<a href="/Observation/RecordByAttachment/' + dataItem.ObjectId + '">' +
                                dataItem.Name + '</a>');
                            break;

                        //default:
                        //    cell.innerHTML
                    }
                    break;

                case 'Actions':
                    if (dataItem.Status == 'Pending') {
                        cell.innerHTML = (
                            '<a href="#" onclick="processRequest(' + r + ', true); return false;">Approve</a>' +
                            '&nbsp;&nbsp;&nbsp;' + 
                            '<a href="#" onclick="processRequest(' + r + ', false); return false;">Deny</a>');
                    }
                    break;
            }
        }
    }
}

function fgRequests_LoadedRows(s, e) {
    if (s._rows.length == 0)
        showFlexGridNoDataMessage(s, 'No requests available.');
}

var processRequestRowIdx, processRequestStatus;
function processRequest(rowIdx, approve) {
    processRequestRowIdx = rowIdx;
    processRequestStatus = approve ? 'Approved' : 'Denied';

    var dataItem = fgRequests.control.rows[rowIdx].dataItem;

    var formData = new FormData();
    formData.append('objectTypeId', dataItem.ObjectType);
    formData.append('objectId', dataItem.ObjectId);
    formData.append('active', approve);
    addAntiForgeryTokenToForm(formData);

    $.ajax({
        url: '/Request/Process',
        data: formData,
        processData: false,
        contentType: false,
        type: 'POST',
        success: processRequestSuccess,
        error: processRequestError
    });
}

function processRequestSuccess(result) {
    if (result.success) {
        var itm = itm = fgRequests.control.rows[processRequestRowIdx].dataItem;
        var cv = fgRequests.control.collectionView;
        cv.editItem(itm);
        itm.Status = processRequestStatus;
        cv.commitEdit();
    } else {
        processRequestError(result);
    }
}

function processRequestError(result) {
    alert(result.responseText);
}