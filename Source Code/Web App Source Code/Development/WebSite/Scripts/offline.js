// Extracts a name-value pair's value from the fragment of a URL.
function getUrlFragmentValue(name) {
    var href = window.location.href;
    var hashIndex = href.indexOf('#');
    if (hashIndex >= 0) {
        var fragment = href.substring(hashIndex);
        var matches = fragment.match(new RegExp('[#&]' + name + '=([^&]+)'));
        if (matches && matches.length > 1)
            return matches[1];
    }
    return null;
}

function getYesNo(value) {
    return localized[value ? 'yes' : 'no'];
}

function getLocalStorageEntity(keyPrefix, ids) {
    var key = getLocalStorageEntityKey(keyPrefix, ids);
    var o = localStorage.getItem(key);
    return o ? JSON.parse(o) : null;
}

function setLocalStorageEntity(data, keyPrefix, ids) {
    var key = getLocalStorageEntityKey(keyPrefix, ids);
    localStorage.setItem(key, JSON.stringify(data));
}

function removeLocalStorageEntity(keyPrefix, ids) {
    var key = getLocalStorageEntityKey(keyPrefix, ids);
    localStorage.removeItem(key);
}

function getLocalStorageEntityKey(keyPrefix, ids) {
    var key = keyPrefix;
    if (ids) {
        if (Array.isArray(ids))
            for (var i in ids)
                key += ('_' + ids[i]);
        else
            key += ('_' + ids);
    }
    return key;
}

function addToLocalStorageArray(value, keyPrefix, ids) {
    var o = getLocalStorageEntity(keyPrefix, ids);
    if (!o)
        o = [];
    if (o.indexOf(value) < 0) {
        o.push(value);
        setLocalStorageEntity(o, keyPrefix, ids);
    }
}

function removeFromLocalStorageArray(value, keyPrefix, ids) {
    var o = getLocalStorageEntity(keyPrefix, ids);
    if (o && o.indexOf(value) >= 0) {
        o.pop(value);
        setLocalStorageEntity(o, keyPrefix, ids);
    }
}

function getFieldIdValue(fieldId) {
    if (fieldId)
        return localStorage.getItem('FieldId_' + fieldId.toString().replace(/\./g));
    return '';
}

function getUserName(userId) {
    if (userId)
        return localStorage.getItem('UserName_' + userId);
    return '';
}

function getIndicatorAgeRange(ageRangeId) {
    if (ageRangeId)
        return localStorage.getItem('IndicatorAgeRange_' + ageRangeId);
    return '';
}



// Loaders
function loadActivity(activityId) {
    var activity = getLocalStorageEntity('Activity', activityId);
    if (activity) {
        var project = getLocalStorageEntity('Project', activity.ProjectId);
        var country = getLocalStorageEntity('Country', activity.CountryId);
        var additionalManagerIds = getLocalStorageEntity('ActivityAdditionalManagerIds', activityId);
        var additionalTechnicalAreaSubTypes = getLocalStorageEntity('ActivityTechnicalAreaSubTypes', activityId);

        // Load activity details.
        setText('spnProjectName', project.Name);
        setText('spnActivityName', activity.Name);
        setText('spnCountryName', country.Name);
        setText('spnFunderValue', getFieldIdValue(activity.FunderFieldId));
        if (activity.StartDate)
            setText('spnStartDate', convertJsonDateToJavaScript(activity.StartDate).format(localized.dateFormat));
        if (activity.EndDate)
            setText('spnEndDate', convertJsonDateToJavaScript(activity.EndDate).format(localized.dateFormat));
        setText('spnPrimaryManagerName', getUserName(activity.PrimaryManagerUserId));
        if (additionalManagerIds)
            setHtml('spnAdditionalManagerNames', additionalManagerIds.map(function (x) { return getUserName(x); }).join(',<br />'));
        setText('spnTechnicalAreaValue', getFieldIdValue(activity.TechnicalAreaFieldId));
        if (additionalTechnicalAreaSubTypes)
            setHtml('spnTechnicalAreaSubtypeValues', additionalTechnicalAreaSubTypes.map(function (x) { return getFieldIdValue(x); }).join(',<br />'));
        setText('spnOtherKeyInformation', activity.OtherKeyInformation);

        // Load aims.
        var aims = getActivityAims(activityId);
        var fgAims = new c1.mvc.grid._FlexGridWrapper('#fgAims');
        fgAims.initialize({
            "columns": [{ "width": "*", "name": "Name", "header": "Aim", "binding": "Name" }],
            "loadedRows": fgAims_LoadedRows,
            "autoGenerateColumns": false,
            "itemFormatter": fgAims_ItemFormatter,
            "itemsSource": aims,
            "selectionMode": 0,
            "isReadOnly": true
        });
        var fgdpAims = new c1.mvc.grid.detail.FlexGridDetailProvider(fgAims.control);
        fgdpAims.initialize({
            "detailVisibilityMode": 3,
            "rowHasDetail": hasIndicators,
            "detailRowTemplateId": c1._registerService('indicator', c1.mvc.Template, ['indicator', true])
        });
        c1._addExtender(fgAims.control, 'fgdpAims', fgdpAims);

        // Load activity sites.
        loadActivitySitesGrid('fgActivitySites', activityId, false);

        //// Load activity notes.
        //var notes = [];
        //var activityNoteIds = getLocalStorageEntity('ActivityNoteIds', activityId);
        //if (activityNoteIds) {
        //    for (var i in activityNoteIds) {
        //        var note = getLocalStorageEntity('Note', activityNoteIds[i]);
        //        if (note)
        //            notes.push(note);
        //    }
        //}
        //loadNotesGrid('fgNotes', notes);
    }
}

function loadIndicator(indicatorId) {
    var indicator = getLocalStorageEntity('Indicator', indicatorId);
    if (indicator) {
        var aim = getLocalStorageEntity('Aim', indicator.AimId);
        var ageRangeIds = getLocalStorageEntity('IndicatorAgeRangeIds', indicatorId);

        // Load indicator details.
        setText('spnAimName', aim.Name);
        setText('spnName', indicator.Name);
        setText('spnTypeValue', getFieldIdValue(indicator.TypeFieldId));
        setText('spnGroupValue', getFieldIdValue(indicator.GroupFieldId));
        setText('spnDefinition', indicator.Definition);
        setText('spnNumeratorName', indicator.NumeratorName);
        setText('spnNumeratorDefinition', indicator.NumeratorDefinition);
        setText('spnNumeratorSource', indicator.NumeratorSource);
        setText('spnDenominatorName', indicator.DenominatorName);
        setText('spnDenominatorDefinition', indicator.DenominatorDefinition);
        setText('spnDenominatorSource', indicator.DenominatorSource);
        setText('spnDataCollectionFrequencyValue', getFieldIdValue(indicator.DataCollectionFrequencyFieldId));
        setText('spnSamplingValue', getFieldIdValue(indicator.SamplingFieldId));
        setText('spnChangeVariable', indicator.ChangeVariable);
        setText('spnDisaggregateBySex', getYesNo(indicator.DisaggregateBySex));
        setText('spnDisaggregateByAge', getYesNo(indicator.DisaggregateByAge));
        if (ageRangeIds)
            setHtml('spnAgeRanges', ageRangeIds.map(function (x) { return getIndicatorAgeRange(x); }).join(', '));
        else
            $('#trAgeRanges').hide();
        setText('spnTargetPerformance', indicator.TargetPerformance);
        setText('spnThresholdGoodPerformance', indicator.ThresholdGoodPerformance);
        setText('spnThresholdPoorPerformance', indicator.ThresholdPoorPerformance);
        setText('spnIncreaseIsGoodText', indicator.IncreaseIsGoodText);
        setText('spnRatePerValue', getFieldIdValue(indicator.RatePerFieldId));

        // Show/hide content based on indicator type.
        indicatorTypeChanged(getFieldIdValue(indicator.TypeFieldId));

        // Load activity sites.
        loadActivitySitesGrid('fgIndicatorSites', aim.ActivityId, true);

        // Load other indicators.
        var otherIndicators = getAimIndicators(indicator.AimId);
        var fgOtherIndicators = new c1.mvc.grid._FlexGridWrapper('#fgOtherIndicators');
        fgOtherIndicators.initialize({
            "columns": [
                { "width": "4*", "name": "Name", "header": "Name", "binding": "Name" },
                { "width": "*", "header": "Data Collection Frequency", "name": "DataCollectionFrequency", "align": "Center" },
                { "width": "*", "header": "Disaggregation", "binding": "DisaggregationText", "align": "Center" }
            ],
            "loadedRows": fgIndicators_LoadedRows,
            "autoGenerateColumns": false,
            "itemsSource": otherIndicators,
            "itemFormatter": fgIndicators_ItemFormatter,
            "selectionMode": 0,
            "isReadOnly": true,
            "headersVisibility": 1
        });
    }
}

function loadObservations(hostElementId, indicatorId, siteId, beginDate) {
    var indicator = getLocalStorageEntity('Indicator', indicatorId);

    var datePeriods;
    if (beginDate) {
        // Fake a single date period.
        var observationId = getLocalStorageEntity('ObservationIdIndex', [indicatorId, siteId, beginDate]);
        if (!isNaN(observationId))
            observationId = parseInt(observationId);
        var observation = getLocalStorageEntity('Observation', observationId);
        datePeriods = [{ ObservationId: observationId, BeginDate: observation.BeginDate, EndDate: observation.EndDate }];
    } else {
        // Get all date periods for indicator and site.
        datePeriods = getLocalStorageEntity('ObservationDatePeriods', [indicatorId, siteId]);
    }

    // Set Aim name.
    var aim = getLocalStorageEntity('Aim', indicator.AimId);
    $('#spnAimName').text(aim.Name);

    // Initialize the entries grid.
    var columns = [
        { "visible": false, "binding": "age_range_name" },
        { "visible": false, "binding": "sex_description" },
        { "isReadOnly": true, "width": "*", "minWidth": 400, "binding": "type" }
    ];
    if (beginDate) {
        var align = 'Right';
        switch (localized.indicatorType) {
            case 'indcnt':
            case 'indyes':
                align = 'Center';
                break;
        }
        columns.push({ "format": "N0", "binding": "value", "align": align });
    } else {
        for (i in datePeriods) {
            var datePeriod = datePeriods[i];
            var binding = (
                offsetDateTimezone(convertJsonDateToJavaScript(datePeriod.BeginDate)).format('yyyy-mm-dd') + '|' +
                offsetDateTimezone(convertJsonDateToJavaScript(datePeriod.EndDate)).format('yyyy-mm-dd'));
            columns.push({ "binding": binding, "align": "Center" });
        }
    }

    var cv = new c1.mvc.collections.CallbackCollectionView({ items: [], groupDescriptions: [] });
    fgEntries = new c1.mvc.grid._FlexGridWrapper('#' + hostElementId);
    fgEntries.initialize({
        "columns": columns,
        "loadedRows": loadedRows,
        "autoGenerateColumns": false,
        "itemFormatter": itemFormatter,
        "itemsSource": cv,
        "selectionMode": (beginDate ? 1 : 0),
        "isReadOnly": (beginDate ? false : true),
        "headersVisibility": (beginDate ? 0 : 1),
        "frozenColumns": (beginDate ? 0 : 3),
        "allowSorting": false
    });

    var entryCollection = primeObservationEntries();
    if (beginDate === null || Object.keys(observation.EntriesCollection).length > 0) {
        for (var i in datePeriods) {
            var observationId = datePeriods[i].ObservationId;
            if (observationId) {
                var observation = getLocalStorageEntity('Observation', observationId);

                // Build the date key for all entries of this observation.
                var observationDatesKey = (
                    offsetDateTimezone(convertJsonDateToJavaScript(observation.BeginDate)).format('yyyy-mm-dd') + '|' +
                    offsetDateTimezone(convertJsonDateToJavaScript(observation.EndDate)).format('yyyy-mm-dd'));

                // Iterate observation entries.
                for (var j in observation.EntriesCollection) {
                    // Get the singular entry.
                    var entry = observation.EntriesCollection[j];

                    // Unique entry key based on type (Num/Denom/Result), age range, sex.
                    var entryKey = ((entry.age_range_id || '') + '|' + (entry.sex_code || '') + '|' + (entry.type || ''));
                    var valueKey = (beginDate === null ? observationDatesKey : 'value');

                    // Add value to entry collection.
                    entryCollection[entryKey][valueKey] = entry.value;
                }
            }
        }
    }
    var entries = objectToArray(entryCollection);
    rebindGrid(entries);

    if (beginDate) {
        // Set Aim Indicators menu.
        var strBeginDate = convertJsonDateToJavaScript(datePeriods[0].BeginDate).format('yyyy-mm-dd');
        var itemsSource = [];
        var aimIds = getLocalStorageEntity('ActivityAimIds', aim.ActivityId);
        for (var i in aimIds) {
            var a = getLocalStorageEntity('Aim', aimIds[i]);
            itemsSource.push({
                'Header': ('<div class="color-white font-weight-bold">' + a.Name + '</div>'),
                'CommandParameter': 'Aim'
            });

            var indicatorIds = getLocalStorageEntity('AimIndicatorIds', a.AimId);
            for (var j in indicatorIds) {
                var id = indicatorIds[j];
                var ind = getLocalStorageEntity('Indicator', id);
                var cssClass = (id === indicatorId ? 'font-weight-bold font-style-italic' : '');
                var url = ('/Offline/ObservationRecord#indicatorId=' + id + '&siteId=' + siteId + '&beginDate=' + strBeginDate);
                itemsSource.push({
                    "Header":
                        ('<a class="' + cssClass + '" href="' + url + '" onclick="redirect(\'' +
                         url + '\');" title="' + ind.Name + '">' + getShortText(ind.Name) + '</a>'),
                    'CommandParameter': 'Indicator'
                });
            }
        }
        var divIndicator = new c1.mvc.input._MenuWrapper('#divIndicator');
        divIndicator.initialize({
            "itemsSource": itemsSource,
            "displayMemberPath": "Header",
            "uniqueId": "divIndicator",
            "header": getShortText(indicator.Name),
            "formatItem": mnuIndicators_FormatItem,
            "dropDownCssClass": "border-dark-blue"
        });

        // Add selection and edit handlers.
        fgEntries.control.selectionChanging.addHandler(fgEntries_SelectionChanging);
        fgEntries.control.beginningEdit.addHandler(fgEntries_BeginningEdit);
        fgEntries.control.cellEditEnding.addHandler(fgEntries_CellEditEnding);
        fgEntries.control.cellEditEnded.addHandler(fgEntries_CellEditEnded);

        // Changes
        var changes = [];
        var changeIds = getLocalStorageEntity('ObservationChangeIds', observationId);
        for (var i in changeIds)
            changes.push(getLocalStorageEntity('ObservationChange', changeIds[i]));
        fgChanges = new c1.mvc.grid._FlexGridWrapper('#fgChanges');
        fgChanges.initialize({
            "columns": [
                { "width": "4*", "name": "Description", "header": localized.change, "binding": "Description" },
                { "width": 100, "header": localized.startDate, "name": "StartDate", "binding": "StartDate" },
                { "width": 150, "header": localized.createdBy, "name": "CreatedBy", "binding": "CreatedBy" },
                { "width": 100, "header": localized.createdOn, "name": "CreatedOn", "binding": "CreatedOn" },
                { "width": 150, "header": localized.updatedBy, "name": "UpdatedBy", "binding": "UpdatedBy" },
                { "width": 100, "header": localized.updatedOn, "name": "UpdatedOn", "binding": "UpdatedOn" },
                { "width": 70, "name": "Edit", "header": localized.edit, "align": "Center" },
                { "width": 70, "name": "Delete", "header": localized.delete, "align": "Center" },
                { "visible": false, "binding": "ChangeId" }
            ],
            "loadedRows": fgChanges_LoadedRows,
            "autoGenerateColumns": false,
            "itemsSource": changes,
            "itemFormatter": fgChanges_ItemFormatter,
            "selectionMode": 0,
            "isReadOnly": true,
            "headersVisibility": 1
        });

        // Attachments
        var attachments = [];
        var attachmentIds = getLocalStorageEntity('ObservationAttachmentIds', observationId);
        for (var i in attachmentIds) {
            var a = getLocalStorageEntity('ObservationAttachment', attachmentIds[i]);
            delete a.Attachment;
            //if (a.CreatedOn)
            //    a.CreatedOn = convertJsonDateToJavaScript(a.CreatedOn);
            attachments.push(a);
        }
        fgAttachments = new c1.mvc.grid._FlexGridWrapper('#fgAttachments');
        fgAttachments.initialize({
            "columns": [
                { "width": "2*", "name": "FileName", "header": localized.fileName },
                { "width": 100, "name": "FileSize", "header": localized.fileSize, "align": "Right" },
                { "width": 100, "name": "Approved", "header": localized.approved, "dataType": 1, "align": "Center" },
                { "width": 150, "header": localized.createdBy, "binding": "CreatedBy" },
                { "width": 100, "header": localized.createdOn, "name": "CreatedOn", "binding": "CreatedOn" },
                { "width": 70, "name": "Delete", "header": localized.delete, "align": "Center" },
                { "visible": false, "binding": "AttachmentId" }
            ],
            "loadedRows": fgAttachments_LoadedRows,
            "autoGenerateColumns": false,
            "itemsSource": attachments,
            "itemFormatter": fgAttachments_ItemFormatter,
            "selectionMode": 0,
            "isReadOnly": true,
            "headersVisibility": 1
        });

        // Comments
        var comments = [];
        var commentIds = getLocalStorageEntity('ObservationCommentIds', observationId);
        for (var i in commentIds)
            comments.push(getLocalStorageEntity('ObservationComment', commentIds[i]));
        fgComments = new c1.mvc.grid._FlexGridWrapper('#fgComments');
        fgComments.initialize({
            "columns": [
                { "width": "4*", "name": "Comment", "header": localized.comment, "binding": "Comment" },
                { "width": 150, "header": localized.createdBy, "name": "CreatedBy", "binding": "CreatedBy" },
                { "width": 100, "header": localized.createdOn, "name": "CreatedOn", "binding": "CreatedOn" },
                { "width": 150, "header": localized.updatedBy, "name": "UpdatedBy", "binding": "UpdatedBy" },
                { "width": 100, "header": localized.updatedOn, "name": "UpdatedOn", "binding": "UpdatedOn" },
                { "width": 70, "name": "Edit", "header": localized.edit, "align": "Center" },
                { "width": 70, "name": "Delete", "header": localized.delete, "align": "Center" },
                { "visible": false, "binding": "CommentId" }
            ],
            "loadedRows": fgComments_LoadedRows,
            "autoGenerateColumns": false,
            "itemsSource": comments,
            "itemFormatter": fgComments_ItemFormatter,
            "selectionMode": 0,
            "isReadOnly": true,
            "headersVisibility": 1
        });
    }
}



// Aims and Indicators
function getActivityAims(activityId) {
    var aims = [];

    var activityAimIds = getLocalStorageEntity('ActivityAimIds', activityId);
    if (activityAimIds) {
        for (var i in activityAimIds) {
            var aim = getLocalStorageEntity('Aim', activityAimIds[i]);
            if (aim) {
                // Load child indicators.
                aim.Indicators = getAimIndicators(aim.AimId);
                aims.push(aim);
            }
        }
    }

    return aims;
}

function getAimIndicators(aimId) {
    var indicators = [];

    var aimIndicatorIds = getLocalStorageEntity('AimIndicatorIds', aimId);
    if (aimIndicatorIds)
        for (var i in aimIndicatorIds)
            indicators.push(getLocalStorageEntity('Indicator', aimIndicatorIds[i]));

    return indicators;
}

function fgIndicators_LoadedRows(s, e) {
    if (s.rows.length === 0)
        showFlexGridNoDataMessage(s, 'No ' + localized.indicators.toLowerCase() + ' available.');
    s.columnHeaders.rows[0].wordWrap = true;
    s.autoSizeRow(0, true);
}

function fgIndicators_ItemFormatter(panel, r, c, cell) {
    if (panel.cellType === wijmo.grid.CellType.Cell) {
        var grid = panel.grid;
        var dataItem = grid.rows[r].dataItem;
        if (dataItem && dataItem.IndicatorId) {
            switch (panel.columns[c].name) {
                case 'Name':
                    var url = ('/Offline/Indicator#indicatorId=' + dataItem.IndicatorId);
                    cell.innerHTML = (
                        '<a href="' + url + '" onclick="redirect(\'' + url + '\'); return false;">' +
                        dataItem.Name + '</a>');
                    break;

                case 'DataCollectionFrequency':
                    cell.innerHTML = getFieldIdValue(dataItem.DataCollectionFrequencyFieldId);
            }
        }
    }
}



// Activity Sites
function loadActivitySitesGrid(hostElementId, activityId, showViewData) {
    var sites = [];
    var activitySiteIds = getLocalStorageEntity('ActivitySiteIds', activityId);
    if (activitySiteIds) {
        for (var i in activitySiteIds) {
            var activitySite = getLocalStorageEntity('ActivitySite', activityId + '_' + activitySiteIds[i]);
            var site = getLocalStorageEntity('Site', activitySiteIds[i]);
            if (activitySite && site)
                sites.push(Object.assign(activitySite, site));
        }
    }

    var columns = [
        { "sortMemberPath": "SiteName", "width": "3*", "name": "Name", "header": localized.name, "binding": "SiteName" },
        { "width": "2*", "header": localized.type, "binding": "SiteTypeValue", "align": "Center" },
        { "width": "3*", "binding": "CoachUserName", "header": localized.coach },
        { "width": 140, "name": "SupportStartDate", "header": localized.supportStartDate, "binding": "SupportStartDate", "align": "Center" },
        { "width": 140, "name": "SupportEndDate", "header": localized.supportEndDate, "binding": "SupportEndDate", "align": "Center" },
        { "width": 70, "header": localized.wave, "name": "Wave", "binding": "WaveFieldId", "align": "Center" }
    ];
    if (showViewData)
        columns.push({ "width": 80, "name": "ViewData", "header": "View Data", "align": "Center" });
    else
        columns.push({ "width": 70, "header": localized.coachReport, "name": "CoachReport", "align": "Center" });

    var grid = new c1.mvc.grid._FlexGridWrapper('#' + hostElementId);
    grid.initialize({
        "columns": columns,
        "loadedRows": fgActivitySites_LoadedRows,
        "autoGenerateColumns": false,
        "itemsSource": sites,
        "itemFormatter": fgActivitySites_ItemFormatter,
        "selectionMode": 0,
        "isReadOnly": true,
        "headersVisibility": 1,
        "autoSizeMode": 2
    });
}

function fgActivitySites_LoadedRows(s, e) {
    if (s.rows.length === 0)
        showFlexGridNoDataMessage(s, 'No ' + localized.sites.toLowerCase() + ' available.');
    s.columnHeaders.rows[0].wordWrap = true;
    s.autoSizeRow(0, true);
}

function fgActivitySites_ItemFormatter(panel, r, c, cell) {
    if (panel.cellType === wijmo.grid.CellType.Cell) {
        var grid = panel.grid;
        var dataItem = grid.rows[r].dataItem;
        if (dataItem && dataItem.ActivityId) {
            switch (panel.columns[c].name) {
                case 'Name':
                    cell.innerHTML = (
                        '<a href="/Offline/Site#siteId=' + dataItem.SiteId + '">' + dataItem.SiteName + '</a>');
                    break;

                case 'SupportStartDate':
                case 'SupportEndDate':
                    var date = dataItem[panel.columns[c].binding];
                    if (date)
                        cell.innerHTML = convertJsonDateToJavaScript(date).format(localized.dateFormat);
                    break;

                case 'Wave':
                    cell.innerHTML = getFieldIdValue(dataItem.WaveFieldId);
                    break;

                case 'ViewData':
                    cell.innerHTML = (
                        '<a class="dataIcon" href="/Offline/ObservationView#indicatorId=' +
                            indicatorId + '&siteId=' + dataItem.SiteId + '"></a>&nbsp;&nbsp;' +
                        '<a class="coachReportIcon" href="/Offline/CoachReport#activityId=' +
                            dataItem.ActivityId + '&siteId=' + dataItem.SiteId + '"></a>');
                    break;

                case 'CoachReport':
                    cell.innerHTML = (
                        '<a class="coachReportIcon" href="/Offline/CoachReport#activityId=' + dataItem.ActivityId +
                        '&siteId=' + dataItem.SiteId + '"></a>');
                    break;
            }
        }
    }
}



// Notes
function loadNotesGrid(hostElementId, data) {
    var fgNotes = new c1.mvc.grid._FlexGridWrapper('#' + hostElementId);
    fgNotes.initialize({
        "columns": [
            { "width": "4*", "name": "Subject", "header": "Subject", "binding": "Subject" },
            { "width": "*", "header": localized.createdBy, "binding": "CreatedBy" },
            { "width": 100, "name": localized.createdOn, "header": localized.createdOn, "binding": "CreatedOn" },
            { "width": "*", "header": "Updated By", "binding": "UpdatedBy" },
            { "width": 100, "name": "Updated On", "header": "Updated On", "binding": "UpdatedOn" },
            { "width": 70, "name": "Delete", "header": "Delete", "align": "Center" }
        ],
        "loadedRows": fgNotes_LoadedRows,
        "autoGenerateColumns": false,
        "itemsSource": data,
        "itemFormatter": fgNotes_ItemFormatter,
        "selectionMode": 0,
        "isReadOnly": true,
        "headersVisibility": 1
    });
}

function fgNotes_ItemFormatter(panel, r, c, cell) {
    if (panel.cellType === wijmo.grid.CellType.Cell) {
        var grid = panel.grid;
        var dataItem = grid.rows[r].dataItem;
        if (dataItem && dataItem.NoteId) {
            switch (panel.columns[c].name) {
                case 'Subject':
                    cell.innerHTML = (
                        '<a class="cursor-pointer" href="#" onclick="showNoteDialog(\'edit\', ' +
                        r.toString() + ', afterEditNote);">' + htmlEncode(dataItem.Subject) + '</a>');
                    break;

                case 'Created On':
                case 'Updated On':
                    var date = dataItem[panel.columns[c].binding];
                    if (date)
                        cell.innerHTML = convertJsonDateToJavaScript(date).format(localized.dateFormat);
                    break;

                case 'Delete':
                    cell.innerHTML = (
                        '<img src="/Images/icons/16/delete_2.png" class="cursor-pointer" ' +
                            'onclick="confirmDeleteNote(' + r + ')" title="' + localized.delete + '" />');
                    break;
            }
        }
    }
}

function fgNotes_LoadedRows(s, e) {
    if (s.rows.length === 0)
        showFlexGridNoDataMessage(s, 'No ' + localized.notes.toLowerCase() + ' available.');
}

function afterAddNote(result) {
    if (!fgNotes.control.rows[0].dataItem || !fgNotes.control.rows[0].dataItem.NoteId)
        fgNotes.control.rows.removeAt(0);

    var cv = fgNotes.control.collectionView;
    var itm = cv.addNew();
    itm.NoteId = result.NoteId;
    itm.SiteId = result.SiteId;
    itm.ActivityId = result.ActivityId;
    itm.Subject = result.Subject;
    itm.NoteText = result.NoteText;
    itm.CreatedBy = result.CreatedBy;
    itm.CreatedOn = result.CreatedOn;
    cv.commitNew();
}

function afterEditNote(result, rowIdx) {
    var cv = fgNotes.control.collectionView;
    var itm = cv.items[rowIdx];
    cv.editItem(itm);
    itm.Subject = result.Subject;
    itm.NoteText = result.NoteText;
    itm.UpdatedBy = result.UpdatedBy;
    itm.UpdatedOn = result.UpdatedOn;
    cv.commitEdit();
}

function confirmDeleteNote(rowIdx) {
    var dataItem = fgNotes.control.rows[rowIdx].dataItem;
    if (confirm('Are you sure you want to delete the Note \'' + dataItem.Subject + '\'?')) {
        var formData = new FormData();
        formData.append('id', dataItem.NoteId);
        addAntiForgeryTokenToForm(formData);

        $.ajax({
            url: '/Note/Delete',
            data: formData,
            processData: false,
            contentType: false,
            type: 'POST',
            success: function (result) {
                fgNotes.control.collectionView.remove(dataItem);
                fgNotes_LoadedRows(fgNotes.control);
            },
            error: function (result) {
                alert(result.responseText);
            }
        });
    }
}



// Breadcrumbs
function setBreadcrumbs(page, keys, setHyperlink) {
    switch (page) {
        case 'Activity':
            // Expected key(s): activityId.
            var activity = getLocalStorageEntity('Activity', keys);
            var x = $('#breadCrumbActivity');
            x.text(activity.Name);
            if (setHyperlink)
                x.prop('href', ('/Offline/Activity#activityId=' + keys));
            break;

        case 'Indicator':
            // Expected key(s): indicatorId.
            var indicator = getLocalStorageEntity('Indicator', keys);
            var aim = getLocalStorageEntity('Aim', indicator.AimId);
            var indicatorIds = getLocalStorageEntity('AimIndicatorIds', aim.AimId);
            var itemsSource = [];
            for (var i in indicatorIds) {
                var indicatorId = indicatorIds[i];
                var ind = getLocalStorageEntity('Indicator', indicatorId);
                var cssClass = (keys === indicatorId ? 'font-weight-bold font-style-italic' : '');
                var url = ('/Offline/Indicator#indicatorId=' + indicatorId);
                itemsSource.push({
                    "Header":
                        ('<a class="' + cssClass + ' cursor-pointer" href="' + url + '" onclick="redirect(\'' +
                         url + '\');" title="' + ind.Name + '">' + getShortText(ind.Name) + '</a>')
                });
            }
            var breadCrumbIndicator = new c1.mvc.input._MenuWrapper('#breadCrumbIndicator');
            breadCrumbIndicator.initialize({
                "itemsSource": itemsSource,
                "displayMemberPath": "Header",
                "uniqueId": "breadCrumbIndicator",
                "header": indicator.Name
            });

            setBreadcrumbs('Activity', aim.ActivityId, true);
            break;

        case 'ObservationView':
            // Expected key(s): indicatorId, siteId.
            var indicator = getLocalStorageEntity('Indicator', keys[0]);
            var aim = getLocalStorageEntity('Aim', indicator.AimId);
            var activitySiteIds = getLocalStorageEntity('ActivitySiteIds', aim.ActivityId);
            var site = getLocalStorageEntity('Site', keys[1]);
            var itemsSource = [];
            for (var i in activitySiteIds) {
                var siteId = activitySiteIds[i];
                var s = getLocalStorageEntity('Site', siteId);
                var cssClass = (keys[1] === s.SiteId ? 'font-weight-bold font-style-italic' : '');
                var url = ('/Offline/ObservationView#indicatorId=' + keys[0] + '&siteId=' + s.SiteId);
                itemsSource.push({
                    "Header":
                        ('<a class="' + cssClass + ' cursor-pointer" href="' + url + '" onclick="redirect(\'' +
                         url + '\');" title="' + s.SiteName + '">' + getShortText(s.SiteName) + '</a>')
                });
            }
            var breadCrumbSites = new c1.mvc.input._MenuWrapper('#breadCrumbSites');
            breadCrumbSites.initialize({
                "itemsSource": itemsSource,
                "displayMemberPath": "Header",
                "uniqueId": "breadCrumbSites",
                "header": site.SiteName
            });

            setBreadcrumbs('Indicator', keys[0], true);
            break;

        case 'ObservationRecord':
            // Expected key(s): indicatorId, siteId, begin date, end date.
            var x = $('#breadCrumbDatePeriod');
            x.text(keys[2].format(localized.dateFormat) + ' ~ ' + keys[3].format(localized.dateFormat));
            setBreadcrumbs('ObservationView', keys, true);
            break;

        case 'Site':
            // Expected key(s): siteId.
            var site = getLocalStorageEntity('Site', keys);
            $('#breadCrumbSite').text(site.SiteName);
            break;

        case 'CoachReport':
            // Expected key(s): activityId, siteId.
            setBreadcrumbs('Activity', keys[0], true);

            var activitySiteIds = getLocalStorageEntity('ActivitySiteIds', keys[0]);
            var site = getLocalStorageEntity('Site', keys[1]);
            var itemsSource = [];
            for (var i in activitySiteIds) {
                var siteId = activitySiteIds[i];
                var s = getLocalStorageEntity('Site', siteId);
                var cssClass = (keys[1] === s.SiteId ? 'font-weight-bold font-style-italic' : '');
                var url = ('/Offline/CoachReport#activityId=' + keys[0] + '&siteId=' + s.SiteId);
                itemsSource.push({
                    "Header":
                        ('<a class="' + cssClass + ' cursor-pointer" href="' + url + '" onclick="redirect(\'' +
                         url + '\');" title="' + s.SiteName + '">' + getShortText(s.SiteName) + '</a>')
                });
            }
            var breadCrumbSites = new c1.mvc.input._MenuWrapper('#breadCrumbSites');
            breadCrumbSites.initialize({
                "itemsSource": itemsSource,
                "displayMemberPath": "Header",
                "uniqueId": "breadCrumbSites",
                "header": site.SiteName
            });
            break;
    }
}

// Navigation Tree
function getAimIndicatorTree(hostId, activityId, indicatorId, siteId) {
    var activity, aims, aim, indicator;
    if (activityId) {
        activity = getLocalStorageEntity('Activity', activityId);
    } else if (indicatorId) {
        indicator = getLocalStorageEntity('Indicator', indicatorId);
        aim = getLocalStorageEntity('Aim', indicator.AimId);
        activity = getLocalStorageEntity('Activity', aim.ActivityId);
    } else {
        throw 'No activity or indicator provided.';
    }
    aims = getActivityAims(activity.ActivityId);

    var host = $('#' + hostId);
    var section = $('<section class="height-100p width-300 flexbox flex-v margin-0"></section>');
    // Build header.
    var header = $('<div class="header"></div>');
    header.append('<div class="title">' + activity.Name + '</div>');
    section.append(header);
    // Build subheader.
    section.append(
        '<div class="aimsIndicators font-weight-bold padding-4" style="border-bottom: 1px solid #CCCCCC;">' +
        localized.aims + ' &amp; ' + localized.indicators + '</div>');
    // Build content.
    var content = $('<div class="flex-2" style="height: 0px; overflow: scroll;"></div>');
    var treeView = $('<div class="treeview"></div>');
    for (var i in aims) {
        // Aim header
        aim = aims[i];
        var treeViewAim = $('<div class="item"></div>');
        treeViewAim.append('<div class="folder"></div>');
        treeViewAim.append('<b class="color-gray">' + aim.Name + '</b>');
        treeView.append(treeViewAim);

        // Indicator items
        var indicators = getAimIndicators(aim.AimId);
        for (var j in indicators) {
            indicator = indicators[j];
            var treeViewInd = $('<div class="item" style="padding-left: 20px;"></div>');
            treeViewInd.append('<div class="document"></div>');
            if (indicator.IndicatorId == indicatorId) {
                treeViewInd.append('<i class="color-dark-blue font-weight-bold">' + indicator.Name + '</i>');
            } else {
                var url = ('/Offline/ObservationView#indicatorId=' + indicator.IndicatorId + '&siteId=' + siteId);
                treeViewInd.append('<a href="' + url + '" onclick="redirect(\'' + url + '\'); return false;">' + indicator.Name + '</a>');
            }
            treeView.append(treeViewInd);
        }
    }
    content.append(treeView);
    section.append(content);
    host.append(section);
}

// NOTE: Redirect using an intermediate page. Useful when the source pages is the same as the destination (only parameters are changing).
function redirect(url) {
    window.location.replace('/Offline/Redirect#url=' + encodeURIComponent(url));
}



// Data collection for online sync
function collectOfflineChanges() {
    var changes = { };
    var entityTypes = ['Note', 'Observation', 'ObservationChange', 'ObservationAttachment', 'ObservationComment'];
    for (var i in entityTypes) {
        var entityType = entityTypes[i];
        changes[entityType + 's'] = collectOfflineEntityChanges(entityType);
    }
    return changes;
}

function collectOfflineEntityChanges(entityType) {
    var changes = { };
    var ops = ['Upserts', 'Deletes'];
    for (var i in ops) {
        var op = ops[i];
        changes[op] = [];

        var ids = getLocalStorageEntity(entityType + op);
        if (ids) {
            if (op === 'Deletes')
                changes[op] = ids;
            else
                for (var j in ids)
                    changes[op].push(getLocalStorageEntity(entityType, ids[j]));
        }
    }
    return changes;
}

function syncChanges() {
    var changes = collectOfflineChanges();
    var form = $('<form action="/Home/OnlineSync" method="post"></form>');
    var hdn = $('<input type="hidden" name="changes" />');
    hdn.val(JSON.stringify(changes));
    form.append(hdn);
    $(document.body).append(form);
    clearChangeKeys();
    $(form).submit();
}

function clearChangeKeys() {
    var entityTypes = ['Note', 'Observation', 'ObservationChange', 'ObservationAttachment', 'ObservationComment'];
    var ops = ['Upsert', 'Delete'];
    for (var i in entityTypes)
        for (var j in ops)
            localStorage.removeItem(entityTypes[i] + ops[j] + 's');
}



function getOfflineSyncTime() {
    return convertJsonDateToJavaScript(localStorage.OfflineSyncTime);
}

function setOfflineSyncTime() {
    localStorage.OfflineSyncTime = convertJavaScriptDateToJson(new Date());
}