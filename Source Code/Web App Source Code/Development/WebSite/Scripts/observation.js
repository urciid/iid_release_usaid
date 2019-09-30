var numericRegEx = /^[\d,]+$/;

function loadedRows(s, e) {
    s.columnHeaders.rows[0].wordWrap = true;
    // TODO: autoSizeRow works fine on the online page but for unknown reason requires deferred execution on offline page.
    setTimeout(function () { s.autoSizeRow(0, true); }, 200);
}

function itemFormatter(panel, r, c, cell) {
    var row = panel.grid.rows[r];
    var col = panel.grid.columns[c];
    var binding = col.binding;

    switch (panel.cellType) {
        case wijmo.grid.CellType.ColumnHeader:
            var header = panel.columns[c].header;

            cell.className = cell.className.replace('wj-header', 'wj-super-header');
            cell.className += ' font-weight-bold';
            if (c === 2) {
                cell.className += ' font-size-big';
                cell.style.lineHeight = '34px';
            } else {
                cell.style.lineHeight = '18px';
            }

            if (binding === 'type') {
                // Clear the header.
                cell.innerHTML = ('&nbsp;' + localized.viewData);
            } else if (binding !== 'value') {
                // Format date(s).
                // First check for Ty's cheesy hack of asterisk embedded in date range indicating comment/attachment/change
                var hasCACData = false;
                if (header.indexOf('*') > 0) {
                    hasCACData = true;
                    header.replace('*', '');
                }
                var dates = header.split('|');
                var date1 = offsetDateTimezone(new Date(dates[0]));
                var date2 = offsetDateTimezone(new Date(dates[1]));
                var content = ('<div class="text-align-center">' + date1.format(localized.dateFormat));
                if (dates[0] !== dates[1])
                    content += ('&nbsp;~&nbsp;&nbsp;<br />' + date2.format(localized.dateFormat));

                var items = panel.grid.collectionView.items;
                // This is quirky. I expect the last row of ever entry collections to be a total row
                // of some sort, or in the case of a totally aggregated Count or Yes/No indicator, have
                // a value. However, we are seeing some collections with missing data (bug? on purpose?).
                // Therefore, loop through each item until a value is found. (Done backward since most
                // of the time, the last row will have a value.)
                var hasData = false;
                for (var i = items.length - 1; i >= 0; i--) {
                    if ((items[i][header] || undefined) !== undefined) {
                        hasData = true;
                        break;
                    }
                }
                var strBeginDate = dates[0];//date1.toISOString().split('T')[0];
                var url = isOffline ?
                    ('/Offline/ObservationRecord#indicatorId=' + localized.indicatorId + '&siteId=' + localized.siteId + '&beginDate=' + strBeginDate) :
                    ('/Observation/Record/' + localized.indicatorId + '/' + localized.siteId + '/' + strBeginDate);
                content += (
                    '&nbsp;&nbsp;' +
                    '<a href="' + url + '">' +
                    '<img src="/Images/icons/16/' + (hasCACData ? 'spreadsheet_checked_changes' : (hasData ? 'spreadsheet_checked' : 'spreadsheet')) + '.png" title="' + localized.viewData + '" /></a>' +
                    '</div>');
                cell.innerHTML = content;
            }
            break;
            
            //'<img src="/Images/icons/16/spreadsheet' + (hasData ? '_checked': '') + '.png" title="' +localized.viewData + '" /></a>' +

        case wijmo.grid.CellType.Cell:
            var dataItem = row.dataItem;
            var type = dataItem.type;
            var content = '';

            if (row instanceof wijmo.grid.GroupRow) {
                if (!dataItem._isBottomLevel)
                    cell.classList.add('wj-top-group-header');

                switch (dataItem.groupDescription.propertyName) {
                    case 'age_range_name':
                        content += '<b>' + localized.ageRange + '</b>';
                        break;
                    case 'sex_description':
                        content += '<b>' + localized.sex + '</b>';
                        break;
                }
                content += (': ' + (dataItem.name || localized.total));
                cell.innerHTML = content;
            } else {
                switch (binding) {
                    case 'type':
                        switch (type) {
                            case 'numerator':
                                content = '<b>N:</b> <span title="' + localized.numeratorDefinition + '">' + localized.numeratorName + '</span>';
                                break;
                            case 'denominator':
                                content = '<b>D:</b> <span title="' + localized.denominatorDefinition + '">' + localized.denominatorName + '</span>';
                                break;
                            case 'result':
                                content = '<b>I:</b> <span title="' + localized.indicatorDefinition + '">' + localized.indicatorName + '</span>';
                                if (localized.indicatorType !== 'indcnt' &&
                                    localized.indicatorType !== 'indyes')
                                    row.cssClass = 'background-color-blue-4';
                                break;
                        }
                        cell.innerHTML = content;
                        break;

                    case 'value':
                        var value = dataItem.value;
                        switch (type) {
                            case 'result':
                                var hasValue = (value !== null && value !== undefined);
                                switch (localized.indicatorType) {
                                    case 'indper':
                                        cell.innerHTML = (hasValue ? ((value * 100).toFixed(0) + '%') : '');
                                        break;
                                    case 'indavg':
                                        cell.innerHTML = (hasValue ? value.toFixed(1) : '');
                                        break;
                                    case 'indrat':
                                        cell.innerHTML = (hasValue ? (value.toFixed(0) + ' per ' + localized.indicatorRatioPer.toLocaleString()) : '');
                                        break;

                                    case 'indyes':
                                        var itm = getEntryCollectionViewItem(dataItem.age_range_id, dataItem.sex_code, 'result');
                                        var ageRangeId = dataItem.age_range_id || 'null', sexCode = dataItem.sex_code || 'null';
                                        cell.innerHTML = (
                                            '<label>' +
                                            '   <input type="radio" id="radYes" name="YesNo" value="1" ' +
                                                (itm.value === true || itm.value === 1 ? 'checked="checked" ' : '') +
                                            '       onclick="yesNoChanged(' + ageRangeId + ', ' + sexCode + ', true);" />' +
                                            '   &nbsp;' + localized.yes +
                                            '</label>' +
                                            '&nbsp;&nbsp;&nbsp;&nbsp;' +
                                            '<label>' +
                                            '   <input type="radio" id="radNo" name="YesNo" value="0" ' +
                                                (itm.value === false || itm.value === 0 ? 'checked="checked" ' : '') +
                                            '       onclick="yesNoChanged(' + ageRangeId + ', ' + sexCode + ', false);" />' +
                                            '   &nbsp;' + localized.no +
                                            '</label>');
                                        break;

                                    case 'indcnt':
                                        cell.className += ' cursor-cell';
                                        break;
                                }
                                break;

                            default:
                                if ((isSexDisaggregated() && !dataItem.sex_code) ||
                                    (isAgeDisaggregated() && !dataItem.age_range_id))
                                    cell.className += ' cursor-not-allowed';
                                else
                                    cell.className += ' cursor-cell';
                                break;
                        }
                        break;

                    default:
                        var value = dataItem[binding];
                        if (value === undefined)
                            value = null;
                        switch (type) {
                            case 'result':
                                switch (localized.indicatorType) {
                                    case 'indper':
                                        cell.innerHTML = (value !== null ? ((value * 100).toFixed(0) + '%') : 'N/A');
                                        break;
                                    case 'indavg':
                                        cell.innerHTML = (value !== null ? value.toFixed(1) : 'N/A');
                                        break;
                                    case 'indrat':
                                        cell.innerHTML = (value !== null ? value.toFixed(0) : 'N/A');
                                        break;
                                    case 'indyes':
                                        cell.innerHTML = (value !== null ? (value === 1 ? localized.yes : localized.no) : '');
                                        break;
                                }
                                break;
                        }
                }
            }
    }
}

function fgEntries_SelectionChanging(s, e) {
    // Prevent default behavior when ending edit of a cell.
    if (entryEditEnd) {
        e.cancel = true;
        entryEditEnd = false;
    } else {
        var dataItem = (e.row < 0) ? null : s.rows[e.row].dataItem;
        e.cancel =
            // DataItem is not present (can happen when the grid is rebound).
            (dataItem === null) ||

            // Selecting a column other than value.
            (s.columns[e.col].binding !== 'value') ||

            // Selecting a sex-aggregated row when sex is disaggregated.
            (isSexDisaggregated() && !dataItem.sex_code) ||

            // Selecting an age-aggregated row when age is disaggregated.
            (isAgeDisaggregated() && !dataItem.age_range_id) ||

            // Selecting a result (except when indicator is Count).
            (dataItem.type === 'result' && localized.indicatorType !== 'indcnt');
    }
}

function fgEntries_BeginningEdit(s, e) {
    var dataItem = s.rows[e.row].dataItem;
    e.cancel =
        // Selecting an aggregated row when sex is disaggregated.
        (isSexDisaggregated() && !dataItem.sex_code) ||

        // Selecting an age-aggregated row when age is disaggregated.
        (isAgeDisaggregated() && !dataItem.age_range_id) ||

        // Selecting an indicator (except when indicator is Count).
        (dataItem.type === 'result' && localized.indicatorType !== 'indcnt');
}

function fgEntries_CellEditEnding(s, e) {
    if (!e.cancel && e.panel.cellType === wijmo.grid.CellType.Cell) {
        if (s.activeEditor.value && !numericRegEx.test(s.activeEditor.value)) {
            alert('Value must be numeric!');
            e.cancel = true;
        } else {
            switch (localized.indicatorType) {
                case 'indper':
                case 'indrat':
                    // Check if numerator is greater than denominator.
                    var numerator = null, denominator = null;
                    var dataItem = e.panel.grid.rows[e.row].dataItem;
                    if (dataItem.type === 'numerator') {
                        numerator = s.activeEditor.value;
                        denominator = getEntryCollectionViewItem(dataItem.age_range_id, dataItem.sex_code, 'denominator').value;
                    } else { // dataItem.type == 'denominator'
                        numerator = getEntryCollectionViewItem(dataItem.age_range_id, dataItem.sex_code, 'numerator').value;
                        denominator = s.activeEditor.value;
                    }
                    if (numerator || "" !== "" && denominator || "" !== "") {
                        // A numerator and denominator are both present.
                        if (parseFloat(numerator) > parseFloat(denominator)) {
                            alert('Numerator cannot be greater than denominator!');
                            e.cancel = true;
                        }
                    }
            }
        }
    }
}

var entryEditEnd = false;
function fgEntries_CellEditEnded(s, e) {
    var grid = fgEntries.control;
    if (localized.indicatorType == 'indcnt') {
        // Get the count
        var dataItem = grid.rows[e.row].dataItem;
        var ageRangeId = dataItem.age_range_id;
        var sexCode = dataItem.sex_code;

        var count = getEntryCollectionViewItem(ageRangeId, sexCode, 'result').value;     
        if (isSexDisaggregated()) {
            setTotalRowCountValues(ageRangeId, null);
        }
        if (isAgeDisaggregated()) {
            if (isSexDisaggregated()) {
                setTotalRowCountValues(null, 'M');
                setTotalRowCountValues(null, 'F');
            }
            setTotalRowCountValues(null, null);
        }
    }
    else
        {
            // Get the numerator and denominator.
            var dataItem = grid.rows[e.row].dataItem;
            var ageRangeId = dataItem.age_range_id;
            var sexCode = dataItem.sex_code;
            var numerator = getEntryCollectionViewItem(ageRangeId, sexCode, 'numerator').value;
            if (numerator)
                numerator = parseFloat(numerator);
            var denominator = getEntryCollectionViewItem(ageRangeId, sexCode, 'denominator').value;
            if (denominator)
                denominator = parseFloat(denominator);

            // Set the new result.
            var cv = grid.collectionView;
            var itm = getEntryCollectionViewItem(ageRangeId, sexCode, 'result');
            setResultItemValue(cv, itm, numerator, denominator);

            if (isSexDisaggregated()) {
                setTotalRowValues(ageRangeId, null);
            }
            if (isAgeDisaggregated()) {
                if (isSexDisaggregated()) {
                    setTotalRowValues(null, 'M');
                    setTotalRowValues(null, 'F');
                }
                setTotalRowValues(null, null);
            }
        }

    $('#spnSave').text(localized.save);
    enableButton('btnSave', saveEntries);

    // Select the next cell possible.
    // NOTE: The next row might be marked as readonly, in which case, proceed until an editable row is reached.
    var col = e.col,
        row = e.row + 1,
        _isSexDisaggregated = isSexDisaggregated(),
        _isAgeDisaggregated = isAgeDisaggregated();
    while (1 === 1) {
        if (row >= grid.rows.length) {
            // If the end of the grid is reached, just return to the edited cell to prevent scrolling.
            row = e.row;
            break;
        }

        var di = grid.rows[row].dataItem;
        if ((_isSexDisaggregated && !di.sex_code) ||
            (_isAgeDisaggregated && !di.age_range_id) ||
            (!di.type || (di.type === 'result' && localized.indicatorType !== 'indcnt')))
            row += 1;
        else
            break;
    }
    grid.select(new wijmo.grid.CellRange(row, col, row, col), false);
    
    entryEditEnd = true;
}

function getEntryValues(ageRangeId, sexCode) {
    var _isAgeDisaggregated = isAgeDisaggregated();
    var _isSexDisaggregated = isSexDisaggregated();

    var cv = fgEntries.control.collectionView;
    values = {};
    for (var i in cv.items) {
        var itm = cv.items[i];
        if ((_isAgeDisaggregated === !!itm.age_range_id && _isSexDisaggregated === !!itm.sex_code) &&
            (!ageRangeId || (ageRangeId === itm.age_range_id)) && (!sexCode || (sexCode === itm.sex_code))) {
            var key = ((itm.age_range_id || '') + '|' + (itm.sex_code || ''));
            if (!values[key])
                values[key] = {};

            values[key][itm.type] = itm.value;
        }
    }
    return values;
}

function setTotalRowValues(ageRangeId, sexCode) {
    var numerator = null, denominator = null;
    var values = getEntryValues(ageRangeId, sexCode);
    for (var i in values) {
        var val = values[i];
        if (val.numerator !== null && val.denominator !== null) {
            numerator += parseFloat(val.numerator);
            denominator += parseFloat(val.denominator);
        } else if (!!val.numerator !== !!val.denominator) {
            numerator = null;
            denominator = null;
            break;
        }
    }

    var cv = fgEntries.control.collectionView;

    var itmNumerator = getEntryCollectionViewItem(ageRangeId, sexCode, 'numerator');
    cv.editItem(itmNumerator);
    itmNumerator.value = numerator;
    cv.commitEdit();

    var itmDenominator = getEntryCollectionViewItem(ageRangeId, sexCode, 'denominator');
    cv.editItem(itmDenominator);
    itmDenominator.value = denominator;
    cv.commitEdit();

    var itmResult = getEntryCollectionViewItem(ageRangeId, sexCode, 'result');
    setResultItemValue(cv, itmResult, numerator, denominator);
}

function setTotalRowCountValues(ageRangeId, sexCode) {
    var count = 0;
    var values = getEntryValues(ageRangeId, sexCode);
    for (var i in values) {
        var val = values[i];
        if (val.result) {
            count += parseInt(val.result);
        }     
    }
    var cv = fgEntries.control.collectionView;

    var itmCount = getEntryCollectionViewItem(ageRangeId, sexCode, 'result');
    cv.editItem(itmCount);
    itmCount.value = count;
    cv.commitEdit();
    
    var itmResult = getEntryCollectionViewItem(ageRangeId, sexCode, 'result');
    setResultItemCountValue(cv, itmResult, count);
}

function setResultItemValue(cv, itm, numerator, denominator) {

    if (itm !== null) {
        cv.editItem(itm);
        switch (localized.indicatorType) {
            case 'indper':
            case 'indavg':
                itm.value = denominator ? (numerator || 0) / denominator : null;
                break;
            case 'indrat':
                itm.value = denominator ? (numerator || 0) / denominator * localized.indicatorRatioPer : null;
                break;
        }
        cv.commitEdit();
    }
}

function setResultItemCountValue(cv, itm, count) {

    if (itm !== null) {
        cv.editItem(itm);
        itm.value = count;
        cv.commitEdit();
    }
}

function yesNoChanged(ageRangeId, sexCode, value) {
    var cv = fgEntries.control.collectionView;
    var itm = getEntryCollectionViewItem(ageRangeId, sexCode, 'result');
    if (itm !== null) {
        cv.editItem(itm);
        itm.value = value;
        cv.commitEdit();
    }

    $('#spnSave').text(localized.save);
    enableButton('btnSave', saveEntries);
}

function getEntryCollectionViewItem(ageRangeId, sexCode, type) {
    var cv = fgEntries.control.collectionView;
    for (var j = 0; j < cv.itemCount; j++) {
        if ((type == cv.items[j].type) &&
            ((ageRangeId || '') == (cv.items[j].age_range_id || '')) &&
            ((sexCode || '') === (cv.items[j].sex_code || ''))) {          
            return cv.items[j];
        }
    }
    return null;
}


function confirmRebindGrid(e) {
    if (confirm('Are you sure you want to change aggregation settings? All changes will be lost.')) {
        //rebindGrid(getEmptyObservationEntries());
        rebindGrid(primeObservationEntries());
    } else {
        e.preventDefault();
        return false;
    }
}

function rebindGrid(entries) {
    var grid = fgEntries.control;
    var cv = grid.collectionView;
    grid.beginUpdate();

    // Rebuild groupings.
    cv.groupDescriptions.clear();
    if (isAgeDisaggregated())
        cv.groupDescriptions.push(new wijmo.collections.PropertyGroupDescription('age_range_name'));
    if (isSexDisaggregated())
        cv.groupDescriptions.push(new wijmo.collections.PropertyGroupDescription('sex_description'));

    // Rebuild collection view.
    for (var i = cv.itemCount - 1; i >= 0; i--)
        cv.removeAt(i);
    for (var i in entries) {
        var entry = entries[i];
        var itm = cv.addNew();
        for (var j in entry)
            itm[j] = entry[j];
    }
    cv.commitNew();

    grid.endUpdate();
}

function isAgeDisaggregated() {
    return (disAggByAge ? disAggByAge.is(':checked') : localized.disAggByAge);
}

function isSexDisaggregated() {
    return (disAggBySex ? disAggBySex.is(':checked') : localized.disAggBySex);
}

function primeObservationEntries() {
    // Create entries collection.
    var entries = {};

    var _isAgeDisaggregated = isAgeDisaggregated();
    var _ageRanges = _isAgeDisaggregated ? localized.ageRanges : [null];

    var _isSexDisaggregated = isSexDisaggregated();
    var _sexes = _isSexDisaggregated ? sexes : [null];

    var _types = [];
    switch (localized.indicatorType) {
        case 'indavg':
        case 'indper':
        case 'indrat':
            _types.push('numerator');
            _types.push('denominator');
    }
    _types.push('result');

    for (var i in _ageRanges) {
        for (var j in _sexes) {
            for (var k in _types) {
                // Unique entry key based on type (Num/Denom/Result), age range, sex.
                var ageRangeId = _isAgeDisaggregated && i !== 'null' ? parseInt(i) : null;
                var ageRangeName = _isAgeDisaggregated ? _ageRanges[i] : null;
                var sexCode = _isSexDisaggregated && j !== 'null' ? j : null;
                var sexDescription = _isSexDisaggregated ? _sexes[j] : null;
                var type = _types[k];

                var key = ((ageRangeId || '') + '|' + (sexCode || '') + '|' + (type || ''));
                entries[key] = {
                    age_range_id: ageRangeId,
                    age_range_name: ageRangeName,
                    sex_code: sexCode,
                    sex_description: sexDescription,
                    type: type
                };
            }
        }
    }
    return entries;
}

function handleAjaxError(result) {
    alert(result.responseText);
}



function fgChanges_LoadedRows(s, e) {
    if (s.rows.length == 0)
        showFlexGridNoDataMessage(s, 'No ' + localized.changes.toLowerCase() + ' available.');
}

function fgChanges_ItemFormatter(panel, r, c, cell) {
    if (panel.cellType == wijmo.grid.CellType.Cell) {
        var grid = panel.grid;
        var dataItem = grid.rows[r].dataItem;
        if (dataItem && dataItem.ChangeId) {
            var columnName = panel.columns[c].name;
            var value = dataItem[columnName];
            switch (columnName) {
                case 'StartDate':
                case 'CreatedOn':
                case 'UpdatedOn':
                    if (value && value.substr && value.substr(0, 5) === '/Date')
                        cell.innerHTML = convertJsonDateToJavaScript(value).format(localized.dateFormat);
                    break;

                case 'Approved':
                    var id = dataItem.ChangeId;
                    var group = ('approveChange' + id);
                    if (localized.isAdmin) {
                        cell.innerHTML = (
                            '<input id="' + group + 'Yes" type="radio" name="' + group + '" ' + (!!value ? 'checked ' : '') + ' onchange="approveChange(' + id + ', true)">' +
                            '&nbsp;<label for="' + group + 'Yes">' + localized.yes + '</label>' +
                            '&nbsp;&nbsp;&nbsp;&nbsp;' +
                            '<input id="' + group + 'No" type="radio" name="' + group + '" ' + (!value ? 'checked ' : '') + ' onchange="approveChange(' + id + ', false)">' +
                            '&nbsp;<label for="' + group + 'No">' + localized.no + '</label>');
                    } else {
                        cell.innerHTML = value ? localized.yes : localized.no;
                    }
                    break;

                case 'Edit':
                    cell.innerHTML = (
                        '<img src="/Images/icons/16/pencil_edit.png" class="cursor-pointer" ' +
                            'onclick="showChangeDialog(\'edit\', ' + r + ', afterEditChange)" title="' + localized.edit + '" />');
                    break;
                case 'Delete':
                    cell.innerHTML = (
                        '<img src="/Images/icons/16/delete_2.png" class="cursor-pointer" ' +
                            'onclick="confirmDeleteChange(' + r + ')" title="' + localized.delete + '" />');
                    break;
            }
        }
    }
}

var dlgChange, hidChangeMode, hidChangeRowIdx, hidChangeId, hidChangeObservationId,
    inpChangeStartDate, inpChangeDescription, spnChangeDialogTitle, btnChangeDialogSave, fncChangeDialogAfterSave;

function showChangeDialog(mode, rowIdx, fncAfterSave) {
    if (!dlgChange) {
        dlgChange = wijmo.Control.getControl("#change-dialog");
        hidChangeMode = $('#changeMode')[0];
        hidChangeRowIdx = $('#changeRowIdx')[0];
        hidChangeId = $('#changeId')[0];
        hidChangeObservationId = $('#changeObservationId')[0];
        inpChangeStartDate = changeStartDate.control;
        inpChangeDescription = $('#changeDescription')[0];
        spnChangeDialogTitle = $('#change-dialog-title')[0];
        btnChangeDialogSave = $('#change-dialog-save')[0];
    }
    fncChangeDialogAfterSave = fncAfterSave;

    hidChangeMode.value = mode;
    if (mode == 'edit') {
        var dataItem = fgChanges.control.rows[rowIdx].dataItem;
        hidChangeRowIdx.value = rowIdx;
        hidChangeId.value = dataItem.ChangeId;
        var startDate = dataItem.StartDate;
        if (startDate && startDate.substr && startDate.substr(0, 5) === '/Date')
            startDate = convertJsonDateToJavaScript(startDate);
        inpChangeStartDate.value = startDate;
        inpChangeDescription.value = dataItem.Description;
        spnChangeDialogTitle.innerText = (localized.edit + ' ' + localized.change);
        btnChangeDialogSave.onclick = editChange;
    } else {
        hidChangeRowIdx.value = null;
        hidChangeId.value = null;
        inpChangeStartDate.value = null;  //new Date("3/3/2001");   //value to indicate empty date
        inpChangeDescription.value = null;
        spnChangeDialogTitle.innerText = (localized.edit + ' ' + localized.change);
        btnChangeDialogSave.onclick = addChange;

    }

    dlgChange.show();

    var btnDialogSave;
    btnDialogSave = $('#change-dialog-save');
    ButtonRemoveDisabled(btnDialogSave);
}

function changeSaveSuccess(result) {
    btnChangeDialogSave.click = null;
    dlgChange.hide();

    fncChangeDialogAfterSave(result, hidChangeRowIdx.value);
}

function afterAddChange(result) {
    if (!fgChanges.control.rows[0].dataItem || !fgChanges.control.rows[0].dataItem.ChangeId)
        fgChanges.control.rows.removeAt(0);

    var cv = fgChanges.control.collectionView;
    var itm = cv.addNew();
    itm.ChangeId = result.ChangeId;
    itm.ObservationId = result.ObservationId;
    itm.StartDate = result.StartDate;
    itm.Description = result.Description;
    itm.CreatedBy = result.CreatedBy;
    itm.CreatedOn = result.CreatedOn;
    cv.commitNew();
}

function afterEditChange(result, rowIdx) {
    var cv = fgChanges.control.collectionView;
    var itm = cv.items[rowIdx];
    cv.editItem(itm);
    itm.StartDate = result.StartDate;
    itm.Description = result.Description;
    itm.UpdatedBy = result.UpdatedBy;
    itm.UpdatedOn = result.UpdatedOn;
    cv.commitEdit();
}

function confirmDeleteChange(rowIdx) {
    if (confirm('Are you sure you want to delete this ' + localized.change + '?'))
        deleteChange(rowIdx);
}

function afterDeleteChange(dataItem) {
    fgChanges.control.collectionView.remove(dataItem);

    if (fgChanges.control.rows.length == 0)
        showFlexGridNoDataMessage(fgChanges.control, 'No ' + localized.changes + ' available.');
}

function approveChange(changeId, approve) {
    var formData = new FormData();
    formData.append('changeId', changeId);
    formData.append('approve', approve);
    addAntiForgeryTokenToForm(formData);

    $.ajax({
        url: '/Observation/ApproveChange',
        data: formData,
        processData: false,
        contentType: false,
        type: 'POST',
        success: function (result) {
            if (!result.success)
                approveChangeError(result);
        },
        error: approveChangeError
    });
}

function approveChangeError(result) {
    alert('Error!\r\n' + result.responseText);
}



function fgAttachments_LoadedRows(s, e) {
    if (s.rows.length == 0)
        showFlexGridNoDataMessage(s, 'No ' + localized.attachments + ' available.');
}

function fgAttachments_ItemFormatter(panel, r, c, cell) {
    if (panel.cellType == wijmo.grid.CellType.Cell) {
        var grid = panel.grid;
        var dataItem = grid.rows[r].dataItem;
        if (dataItem && dataItem.AttachmentId) {
            var columnName = panel.columns[c].name;
            var value = dataItem[columnName];
            switch (panel.columns[c].name) {
                case 'FileName':
                    cell.innerHTML = (
                        '<a class="cursor-pointer" href="#" onclick="downloadFile(\'' +
                        dataItem.AttachmentId + '\'); return false;">' + htmlEncode(value) + '</a>');
                    break;

                case 'FileSize':
                    cell.innerHTML = ((value / 1000000).toFixed(2) + ' MB');
                    break;

                case 'Approved':
                    var id = dataItem.AttachmentId;
                    var group = ('approveAttachment' +id);
                    
                    if (dataItem.Active) {
                        cell.innerHTML = localized.yes;
                    } else if (localized.isAdmin && !dataItem.Active) {
                        cell.innerHTML = (
                            '<input id="' + group + 'Yes" type="radio" name="' +group + '" ' +(!!value ? 'checked ' : '') + ' onchange="approveAttachment(' +id + ', true)">' +
                            '&nbsp;<label for="' +group + 'Yes">' +localized.yes + '</label>' +
                            '&nbsp;&nbsp;&nbsp;&nbsp;' +
                            '<input id="' + group + 'No" type="radio" name="' + group + '" ' + (!value ? 'checked ' : '') + ' onchange="approveAttachment(' + id + ', false)">' +
                            '&nbsp;<label for="' +group + 'No">' +localized.no + '</label>');
                    } else {
                        cell.innerHTML = value ? localized.yes: localized.no;
                    }
                    break;

                case 'CreatedOn':
                    var value = dataItem.CreatedOn;
                    if (value && value.substr && value.substr(0, 5) === '/Date')
                        cell.innerHTML = convertJsonDateToJavaScript(value).format(localized.dateFormat);
                    break;

                case 'Delete':
                    cell.innerHTML = (
                        '<img src="/Images/icons/16/delete_2.png" class="cursor-pointer" ' +
                            'onclick="confirmDeleteAttachment(' + r + ')" title="' + localized.delete + '" />');
                    break;
            }
        }
    }
}

var dlgAttachment, hidAttachmentObservationId, inpAttachmentFile, inpAttachmentName, fncAttachmentDialogAfterSave, btnDialogSave;

function showAttachmentDialog(fncAfterSave) {
    if (!dlgAttachment) {
        dlgAttachment = wijmo.Control.getControl('#attachment-dialog');
        inpAttachmentName = $('#attachmentName')[0];
        inpAttachmentFile = $('#attachmentFile')[0];
    }
    fncAttachmentDialogAfterSave = fncAfterSave;

    // Reset the file upload control.
    inpAttachmentFile.value = '';

    dlgAttachment.show();

    btnDialogSave = $('#attachment-dialog-save');
    ButtonRemoveDisabled(btnDialogSave);

}

function attachmentSaveSuccess(result) {

    dlgAttachment.hide();

    fncAttachmentDialogAfterSave(result);
}

function afterAddAttachment(result) {
    if (!fgAttachments.control.rows[0].dataItem || !fgAttachments.control.rows[0].dataItem.AttachmentId)
        fgAttachments.control.rows.removeAt(0);

    var cv = fgAttachments.control.collectionView;
    var itm = cv.addNew();
    itm.AttachmentId = result.AttachmentId;
    itm.ObservationId = result.ObservationId;
    itm.FileName = result.FileName;
    itm.FileSize = result.FileSize;
    itm.CreatedBy = result.CreatedBy;
    itm.CreatedOn = result.CreatedOn;
    cv.commitNew();

}

function confirmDeleteAttachment(rowIdx) {
    if (confirm('Are you sure you want to delete this ' + localized.attachment + '?'))
        deleteAttachment(rowIdx);
}

function afterDeleteAttachment(dataItem) {
    fgAttachments.control.collectionView.remove(dataItem);

    if (fgAttachments.control.rows.length == 0)
        showFlexGridNoDataMessage(fgAttachments.control, 'No ' + localized.attachments + ' available.');
}

function approveAttachment(attachmentId, approve) {
    var formData = new FormData();
    formData.append('attachmentId', attachmentId);
    formData.append('approve', approve);
    addAntiForgeryTokenToForm(formData);

    $.ajax({
        url: '/Observation/ApproveAttachment',
        data: formData,
        processData: false,
        contentType: false,
        type: 'POST',
        success: function (result) {
            if (!result.success)
                approveAttachmentError(result);
        },
        error: approveAttachmentError
    });
}

function approveAttachmentError(result) {
    alert('Error!\r\n' + result.responseText);
}



function fgComments_LoadedRows(s, e) {
    if (s.rows.length == 0)
        showFlexGridNoDataMessage(s, 'No ' + localized.comments + ' available.');
}

function fgComments_ItemFormatter(panel, r, c, cell) {
    if (panel.cellType == wijmo.grid.CellType.Cell) {
        var grid = panel.grid;
        var dataItem = grid.rows[r].dataItem;
        if (dataItem && dataItem.CommentId) {
            var columnName = panel.columns[c].name;
            var value = dataItem[columnName];
            switch (columnName) {
                case 'CreatedOn':
                case 'UpdatedOn':
                    if (value && value.substr && value.substr(0, 5) === '/Date')
                        cell.innerHTML = convertJsonDateToJavaScript(value).format(localized.dateFormat);
                    break;

                case 'Edit':
                    cell.innerHTML = (
                        '<img src="/Images/icons/16/pencil_edit.png" class="cursor-pointer" ' +
                            'onclick="showCommentDialog(\'edit\', ' + r + ', afterEditComment)" title="' + localized.edit + '" />');
                    break;
                case 'Delete':
                    cell.innerHTML = (
                        '<img src="/Images/icons/16/delete_2.png" class="cursor-pointer" ' +
                            'onclick="confirmDeleteComment(' + r + ')" title="' + localized.delete + '" />');
                    break;
            }
        }
    }
}

var dlgComment, hidCommentMode, hidCommentRowIdx, hidCommentId, hidCommentObservationId,
    inpComment, spnCommentDialogTitle, btnCommentDialogSave, fncCommentDialogAfterSave;

function showCommentDialog(mode, rowIdx, fncAfterSave) {
    if (!dlgComment) {
        dlgComment = wijmo.Control.getControl("#comment-dialog");
        hidCommentMode = $('#commentMode')[0];
        hidCommentRowIdx = $('#commentRowIdx')[0];
        hidCommentId = $('#commentId')[0];
        inpComment = $('#comment')[0];
        spnCommentDialogTitle = $('#comment-dialog-title')[0];
        btnCommentDialogSave = $('#comment-dialog-save')[0];
    }
    
    fncCommentDialogAfterSave = fncAfterSave;

    hidCommentMode.value = mode;
    if (mode == 'edit') {
        var dataItem = fgComments.control.rows[rowIdx].dataItem;
        hidCommentRowIdx.value = rowIdx;
        hidCommentId.value = dataItem.CommentId;
        inpComment.value = dataItem.Comment;
        spnCommentDialogTitle.innerText = (localized.edit + ' ' + localized.comment);
        btnCommentDialogSave.onclick = editComment;
    } else {
        hidCommentRowIdx.value = null;
        hidCommentId.value = null;
        inpComment.value = null;
        spnCommentDialogTitle.innerText = (localized.add + ' ' + localized.comment);
        btnCommentDialogSave.onclick = addComment;
    }

    dlgComment.show();

    var btnDialogSave;
    btnDialogSave = $('#comment-dialog-save');
    ButtonRemoveDisabled(btnDialogSave);
}

function commentSaveSuccess(result) {
    btnCommentDialogSave.click = null;
    dlgComment.hide();

    fncCommentDialogAfterSave(result, hidCommentRowIdx.value);
}

function afterAddComment(result) {
    if (!fgComments.control.rows[0].dataItem || !fgComments.control.rows[0].dataItem.CommentId)
        fgComments.control.rows.removeAt(0);

    var cv = fgComments.control.collectionView;
    var itm = cv.addNew();
    itm.CommentId = result.CommentId;
    itm.ObservationId = result.ObservationId;
    itm.Comment = result.Comment;
    itm.CreatedBy = result.CreatedBy;
    itm.CreatedOn = result.CreatedOn;
    cv.commitNew();
}

function afterEditComment(result, rowIdx) {
    var cv = fgComments.control.collectionView;
    var itm = cv.items[rowIdx];
    cv.editItem(itm);
    itm.Comment = result.Comment;
    itm.UpdatedBy = result.UpdatedBy;
    itm.UpdatedOn = result.UpdatedOn;
    cv.commitEdit();
}

function confirmDeleteComment(rowIdx) {
    if (confirm('Are you sure you want to delete this ' + localized.comment + '?'))
        deleteComment(rowIdx);
}

function afterDeleteComment(dataItem) {
    fgComments.control.collectionView.remove(dataItem);
    fgComments_LoadedRows(fgComments.control);
}



var numberOfDatePeriods = 10;
function getDatePeriods(periodOffset) {
    // Since datePeriods are an object, we cannot iterate numerically.
    // Extract keys and seek the selectedDate. (If not found, default to last date.)
    var keys = Object.keys(datePeriods);
    var n = keys.length - 1, periodId = n;
    var strPreferredLastDateShown = offsetDateTimezone(preferredLastDateShown).format('yyyy-mm-dd');
    for (var i = 0; i <= periodId; i++) {
        if (keys[i] === strPreferredLastDateShown) {
            periodId = i;
            break;
        }
    }

    // Find the first and last indices. Ideally, use numberOfDatePeriods, but adjust if not enough periods exist.
    var first, last;
    if (periodOffset >= 0) {
        last = Math.min(Math.max(Math.min(periodId + periodOffset, n), numberOfDatePeriods - 1), n);
        first = last - numberOfDatePeriods + 1;
        if (first < 0)
            first = 0;
    } else {
        last = Math.max(Math.min(periodId + periodOffset, n), numberOfDatePeriods - 1);
        first = Math.max(last - numberOfDatePeriods + 1, 0);
    }

    // Collect the periods.
    var r = [];
    for (var i = first; i <= last; i++)
        r.push(datePeriods[keys[i]]);
    addDatePeriods(r);

    preferredLastDateShown = new Date(keys[last]);
}

function addDatePeriods(periods) {
    var ph = $('#datePeriods');

    // Clear existing periods.
    ph.empty();

    // Add new date periods.
    for (var i in periods) {
        var datePeriod = periods[i];
        var beginDate = convertJsonDateToJavaScript(datePeriod.BeginDate);
        var endDate = convertJsonDateToJavaScript(datePeriod.EndDate);

        var div = $('<div>'), a = $('<a>'), img = $('<img />');
        div.addClass('float-left');
        div.addClass('border-light-gray');
        div.addClass('no-wrap');
        div.addClass('text-align-center');
        div.addClass('width-10p');
        if (beginDate.getTime() === thisObservationBeginDate.getTime()) {
            div.addClass('background-color-light-gray');
            a.addClass('color-blue font-weight-bold');
        } else {
            div.addClass('background-color-blue');
            a.addClass('color-white');
        }

        if (datePeriod.HasChangeCommentAttachment) {
            if (beginDate.getTime() === thisObservationBeginDate.getTime()) {
            a.addClass('font-weight-bold');
            img.attr('src', '/Images/icons/16/spreadsheet_checked_changes_selected.png');
            } else {
                a.addClass('font-weight-bold');
                img.attr('src', '/Images/icons/16/spreadsheet_checked_changes.png');
            }
        } else if (datePeriod.ObservationId > 0) {
            a.addClass('font-weight-bold');
            img.attr('src', '/Images/icons/16/spreadsheet_checked.png');
        } else {
            img.attr('src', '/Images/icons/16/spreadsheet.png');
        }

        var isoDate = beginDate.toISOString().split('T')[0];
        if (isOffline) {
            a.attr('href', '#');
            a.attr('onclick', ('redirect(\'/Offline/ObservationRecord#indicatorId=' + localized.indicatorId + '&siteId=' + localized.siteId + '&beginDate=' + isoDate + '\'); return false;'));
        } else {
            a.attr('href', ('/Observation/Record/' + localized.indicatorId + '/' + localized.siteId + '/' + isoDate));
        }
        a.append((offsetDateTimezone(beginDate).format(localized.dateFormat) + ' ~&nbsp&nbsp;'));
        a.append($('<br />'));
        a.append((offsetDateTimezone(endDate).format(localized.dateFormat) + '&nbsp;'));
        a.append(img);
        div.append(a);

        ph.append(div);
    }
}



function mnuIndicators_FormatItem(s, e) {
    if (e.data.CommandParameter === 'Aim')
        e.item.classList.add('background-color-blue-3');
}