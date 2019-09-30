var chartIdRegEx = /chtIndicator([\d]+)/;

function itemFormatter(engine, hitTestInfo, defaultFormat) {
    if (hitTestInfo.chartElement === wijmo.chart.ChartElement.SeriesSymbol) {
        var rgb = (hitTestInfo.item.ChangeDescriptions === null ? '255,255,255' : '0,32,96');
        engine.fill = ('rgba(' + rgb + ',1.0)');
        defaultFormat();
    }
}

function fgChanges_LoadedRows(s, e) {
    if (s.rows.length === 0)
        showFlexGridNoDataMessage(s, 'No ' + localized.changes.toLowerCase() + ' available.');
}

function fgChanges_ItemFormatter(panel, r, c, cell) {
    //var row = panel.grid.rows[r];
    var col = panel.grid.columns[c];
    var binding = col.binding;

    if (panel.cellType === wijmo.grid.CellType.ColumnHeader) {
        switch (binding) {
            case 'IndicatorName':
                cell.className += ' indicatorColumn';
                break;

            case 'Change':
                cell.className += ' changeColumn';
                break;

            case 'CreatedBy':
                cell.className += ' createdByColumn';
                break;
        }
    }
}

function getCoachObservationsAndChanges(siteId, indicatorId) {
    var observations = [], changes = [];
    var datePeriods = getLocalStorageEntity('ObservationDatePeriods', [indicatorId, siteId]);

    for (var i in datePeriods) {
        var datePeriod = datePeriods[i];
        if (datePeriod.ObservationId) {
            if (datePeriod.ObservationId) {
                // Get the observation value (all age ranges, all sexes).
                var observation = getLocalStorageEntity('Observation', datePeriod.ObservationId);
                var entry = observation.EntriesCollection['||result'];
                if (entry && entry.value) {
                    // Add the observation from this date period.
                    var beginDate = convertJsonDateToJavaScript(datePeriod.BeginDate);
                    var item = { Date: beginDate, Value: entry.value, ChangeDescriptions: null };

                    // Get the change descriptions (if any).
                    var changeIds = getLocalStorageEntity('ObservationChangeIds', datePeriod.ObservationId);
                    if (changeIds) {
                        var indicator = getLocalStorageEntity('Indicator', indicatorId);

                        var changeTexts = [];
                        for (var j in changeIds) {
                            var change = getLocalStorageEntity('ObservationChange', changeIds[j]);
                            changeTexts.push(change.Description);

                            // Push individual changes for the grid.
                            changes.push({ Date: beginDate, IndicatorName: indicator.Name, Change: change.Description, CreatedBy: change.CreatedBy });
                        }
                        // Concat all changes for a single chart point.
                        item.ChangeDescriptions = changeTexts.join('<br />');
                    }

                    observations.push(item);
                }
            }
        }
    }
    return { Observations: observations, Changes: changes };
}

// NOTE: I used onRendering because none of the axis settings or the axis itemFormatter seem to work.
function onRendering(e) {
    // Set the X-Axis labels using the date values. However, if > 10 dates exist,
    // pick a date interval in order to keep the labels looking decent.
    console.log(e.hostElement.id);
    e.axisX._actualAngle = 0;
    var lbls = e.axisX._lbls;
    var items = e.series[e.series.length - 1].itemsSource.items;
    // Logic for short daily frequency ranges:
    var mod = 1;
    if ('@Model.Indicator.DataCollectionFrequencyFieldId' === 'frqdai') {
        if (lbls.length <= 15) {
            mod = 1;
        } else if (lbls.length <= 31) {
            mod = 2;
        } else if (lbls.length <= 62) {
            mod = 7;
        } else {
            mod = 1;
        }
    }
    for (var i = items.length - 1; i >= 0; i--) {
        if ((i % mod != 0) || (i > 0 && items[i].Label == items[i - 1].Label))
            lbls[i] = '';
        else
            lbls[i] = items[i].Label;
    }
}

function chtIndicatorRendering(e) {
    // Set the X-Axis labels using the values from the first chart series.
    // If the chart shows > 3 months' data, date is condenses to MMM yyyy, so don't repeat.
    e.axisX._actualAngle = 0;
    var lbls = e.axisX._lbls;
    var items = e.series[0].itemsSource.items;
    var monthFormat = 'm/yy';
    var dayFormat = 'd mmm';
    var maximumLabelsToShow = 7;

    // Determine the chart's data collection frequency.
    var frequency = indicatorFrequencies[chartIdRegEx.exec(e.hostElement.id)[1]];

    // Determine an appropriate interval for displaying labels.
    var dateFormat = dayFormat;
    var intervalMod = 1;
    switch (frequency) {
        case 'frqdai':
            if (items.length > 70) {
                // Use monthly format and some interval of month(s).
                maximumLabelsToShow = 10;
                dateFormat = monthFormat;
                intervalMod = getLabelInterval(items.length, maximumLabelsToShow);
            } else if (items.length > 14) {
                // Use weekly interval.
                intervalMod = 7;
            }
            break;

        case 'frqwee':
        case 'frqbiw':
            if (items.length > maximumLabelsToShow) {
                intervalMod = getLabelInterval(items.length, maximumLabelsToShow);
            }
            break;

        case 'frqmon':
        case 'frqqua':
            // Use monthly format and some interval of month(s).
            maximumLabelsToShow = 10;
            dateFormat = monthFormat;
            if (items.length > maximumLabelsToShow) {
                intervalMod = getLabelInterval(items.length, maximumLabelsToShow);
            }
            break;
    }

    for (var i = items.length - 1; i >= 0; i--) {
        if ((i % intervalMod != 0) || (i > 0 && items[i].Date == items[i - 1].Date))
            lbls[i] = '';
        else
            lbls[i] = items[i].Date.format(dateFormat);
    }
}

function getLabelInterval(itemCount, maxLabelsToShow) {
    return Math.floor((itemCount - 1) / maxLabelsToShow) + 1;
}