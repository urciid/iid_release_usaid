var criteriaTabContainer, criteriaTabTemplate, addLine, criteriaContainer, criteriaTemplate,
    activeCriteria, criteriaCount = 0, divAddMedian, chkaddMedian, addMedian;

$(document).ready(function () {
    // Load these menus in the template, as they are always the same, no matter the country selection.
    loadListBoxFromArray($('[name=lstCountry]'), chartFilters.Countries, 'CountryName', 'CountryId');
    loadListBoxFromArray($('[name=lstPopulationDensity]'), chartFilters.PopulationDensities, 'Value', 'FieldId', true, 'All');
    loadListBoxFromArray($('[name=lstWave]'), chartFilters.Waves, 'Value', 'FieldId', true, 'All');
    loadListBoxFromArray($('[name=lstAgeRange]'), chartFilters.AgeRanges, 'Name', 'Id', true, 'All');
    loadListBoxFromArray($('[name=lstSex]'), chartFilters.Sexes, 'Description', 'Code', true, 'All');

    criteriaTabContainer = $('#criteriaTabContainer');
    criteriaTabTemplate = $('#criteriaTabTemplate');
    addLine = $('#addLine');
    criteriaContainer = $('#criteriaContainer');
    criteriaTemplate = $('#criteriaTemplate');
    activeCriteria = $('#activeCriteria');
    divAddMedian = $('#divAddMedian');
    addMedian = $('#addMedian');
    chkaddMedian = $('#chkaddMedian');

    // Capture the initial active criteria.
    var activeCriteriaId = getActiveCriteria();
    // Load the criteria.
    if (chartCriterias && chartCriterias.length > 0) {
        for (var i in chartCriterias) {
            addCriteria(chartCriterias[i]);
        }
    } else {
        addCriteria(null);
    }
    // Show the initial active criteria.
    showCriteria(activeCriteriaId);

    // Initialize checkbox checked state.
    chkaddMedian.prop('checked', (addMedian.val() === 'true' && criteriaCount === 1));

    showHideRunChartRules();
});

function addCriteria(chartCriteria) {
    var index = criteriaCount;

    // Initialize a new tab node.
    var tabClone = criteriaTabTemplate.clone(true);
    setTab(tabClone, index);
    tabClone.removeClass('display-none');
    addLine.before(tabClone);

    // Initialize a new criteria node.
    var criteriaClone = criteriaTemplate.clone(true);
    criteriaClone.removeClass("display-none");
    criteriaClone.attr('id', ('criteria' + index));
    criteriaClone.removeClass('display-none');
    criteriaContainer.append(criteriaClone);

    // Changes Multi-Select
    var inpChange = criteriaClone.find('#inpChange');
    inpChange.attr('id', 'inpChange' + index);
    var mnuChanges = new wijmo.input.MultiSelect(inpChange);
    mnuChanges.displayMemberPath = 'Description';
    mnuChanges.dropDownCssClass = 'width-400';
    mnuChanges.maxHeaderItems = 1;
    mnuChanges.selectedValuePath = 'ObservationChangeId';
    mnuChanges.itemsSource = null;

    // Sites Multi-Select
    var inpSites = criteriaClone.find('#inpSites');
    inpSites.attr('id', 'inpSites' + index);
    var mnuSites = new wijmo.input.MultiSelect(inpSites);
    mnuSites.displayMemberPath = 'SiteName';
    mnuSites.maxHeaderItems = 2;
    mnuSites.selectedValuePath = 'SiteId';
    mnuSites.itemsSource = null;
    mnuSites.checkedItemsChanged.addHandler(function (s, e) {
        if (mnuSites.checkedItems.length === 0)
            mnuSites.inputElement.value = allSitesText;
        loadChanges(mnuChanges, criteriaClone, null);
    });

    // Site Types Multi-Select
    var inpSiteTypes = criteriaClone.find('#inpSiteTypes');
    inpSiteTypes.attr('id', 'inpSiteTypes' + index);
    var mnuSiteTypes = new wijmo.input.MultiSelect(inpSiteTypes);
    mnuSiteTypes.displayMemberPath = 'Value';
    mnuSiteTypes.maxHeaderItems = 2;
    mnuSiteTypes.selectedValuePath = 'FieldId';
    mnuSiteTypes.itemsSource = null;
    mnuSiteTypes.checkedItemsChanged.addHandler(function (s, e) {
        if (mnuSiteTypes.checkedItems.length === 0)
            mnuSiteTypes.inputElement.value = allSiteTypesText;
        loadSites(mnuSites, criteriaClone, null);
    });

    // Age Disaggregation
    if (chartCriteria && chartCriteria.AgeRangeId)
        criteriaClone.find('[name=lstAgeRange]').val(chartCriteria.AgeRangeId);

    // Sex Disaggregation
    if (chartCriteria && chartCriteria.SexCode)
        criteriaClone.find('[name=lstSex]').val(chartCriteria.SexCode);

    // Start Date Picker
    var inpStartDate = criteriaClone.find('#inpStartDate');
    inpStartDate.attr('id', 'inpStartDate' + index);
    var mnuStartDate = new wijmo.input.InputDate(inpStartDate, { isRequired: false });
    if (chartCriteria && chartCriteria.BeginDateString)
        mnuStartDate.value = chartCriteria.BeginDateString;
    else
        mnuStartDate.value = null;

    // End Date Picker
    var inpEndDate = criteriaClone.find('#inpEndDate');
    inpEndDate.attr('id', 'inpEndDate' + index);
    var mnuEndDate = new wijmo.input.InputDate(inpEndDate, { isRequired: false });
    if (chartCriteria && chartCriteria.EndDateString)
        mnuEndDate.value = chartCriteria.EndDateString;
    else
        mnuEndDate.value = null;

    // Color Picker
    var inpColor = criteriaClone.find('#inpColor');
    inpColor.attr('id', 'inpColor' + index);
    var mnuColor = new wijmo.input.Menu(inpColor);
    mnuColor.displayMemberPath = 'Name';
    mnuColor.selectedValuePath = 'ColorId';
    mnuColor.dropDown.dir = 'rtl';
    $(mnuColor.hostElement).addClass('width-150');
    mnuColor.formatItem.addHandler(function (s, e) {
        e.item.innerHTML = getColorMenuItemHtml(e.data.Hexadecimal, e.data.Name);
    });
    mnuColor.selectedIndexChanged.addHandler(function (s, e) {
        if (mnuColor.selectedItem)
            mnuColor.header = getColorMenuItemHtml(mnuColor.selectedItem.Hexadecimal, mnuColor.selectedItem.Name);
    });
    // NOTE: Conver the JSON object to a JavaScript array in order to bind.
    mnuColor.itemsSource = $.map($.extend(true, {}, chartFilters.Colors), function (x) { return x; });
    if (chartCriteria && chartCriteria.ColorId)
        mnuColor.selectedValue = chartCriteria.ColorId;
    else
        mnuColor.selectedIndex = 0;

    // Set the new active tab.
    criteriaCount += 1;
    showCriteria(index);

    // Load the country list.
    var lstCountry = criteriaClone.find('[name=lstCountry]');
    lstCountry.change(function () { loadCountry(index, criteriaClone, lstCountry.val()); });
    // Select the country from criteria, or default to first in the list.
    if (chartCriteria && chartCriteria.CountryId)
        lstCountry.val(chartCriteria.CountryId);
    loadCountry(index, criteriaClone, lstCountry.val(), chartCriteria);

    showHideRunChartRules();
}

function getColorMenuItemHtml(hexadecimal, name) {
    return (
        '<div class="display-inline-block border-gray" style="height: 10px; width: 10px; background-color: #' +
        hexadecimal + ';"></div>&nbsp;&nbsp;' + name);
}

function setTab(tab, newIndex) {
    tab.click(function () { showCriteria(newIndex); });
    tab.find('span').text('Line ' + (newIndex + 1).toString()).click(function () { showCriteria(newIndex); });
    tab.find('div').click(function () { removeCriteria(newIndex); });

    // Hack to set the new criteria id.
    var match = tab.attr('id').match(/criteriaTab([\d]+)/);
    if (match) {
        var oldIndex = match[1];
        if (oldIndex != newIndex) {
            var div = $('#criteria' + oldIndex).attr('id', ('criteria' + newIndex));
        }
    }

    // Set the new tab id.
    tab.attr('id', ('criteriaTab' + newIndex));
}

function getActiveCriteria() {
    return parseInt(activeCriteria.val());
}

function setActiveCriteria(value) {
    activeCriteria.val(value);
}

function setActiveTabStyles(tab) {
    tab.removeClass('background-color-light-gray color-blue').addClass("background-color-blue color-white");
}

function setInactiveTabStyles(tab) {
    tab.removeClass('background-color-blue color-white').addClass('background-color-light-gray color-blue');
}

function showCriteria(id) {
    var n = criteriaCount;
    for (var i = 0; i < n; i++) {
        if (i === id) {
            setActiveTabStyles($('#criteriaTab' + i));
            $('#criteria' + i).show();
            setActiveCriteria(i);
        } else {
            setInactiveTabStyles($('#criteriaTab' + i));
            $('#criteria' + i).hide();
        }
    }

    setActiveCriteria(id);
}

function removeCriteria(id) {
    $('#criteria' + id).remove();
    $('#criteriaTab' + id).remove();

    for (var i = id + 1; i < criteriaCount; i++)
        setTab($('#criteriaTab' + i), i -1);
    criteriaCount -= 1;

    showCriteria(id >= criteriaCount ? criteriaCount - 1 : id);
    showHideRunChartRules();

    event.stopPropagation();
}

function loadCountry(index, tbody, countryId, chartCriteria) {
    // Administrative Division Types
    var admTypes;
    for (var i in chartFilters.AdministrativeDivisionTypes) {
        var adt = chartFilters.AdministrativeDivisionTypes[i];
        if (adt.CountryId == countryId) {
            admTypes = adt;
            break;
        }
    }
    for (i = 1; i <= 4; i++) {
        var tr = tbody.find('#trAdministrativeDivision' + i);
        var type = adt['AdministrativeDivisionType' + i];
        if (type === null) {
            tr.hide();
        } else {
            tr.find('td').first().text(type + ':');
            tr.show();
        }
    }

    var lstAdm1 = tbody.find('[name=lstAdministrativeDivision1]'),
        lstAdm2 = tbody.find('[name=lstAdministrativeDivision2]'),
        lstAdm3 = tbody.find('[name=lstAdministrativeDivision3]'),
        lstAdm4 = tbody.find('[name=lstAdministrativeDivision4]'),
        lstWave = tbody.find('[name=lstWave]'),
        lstSiteTypes = wijmo.Control.getControl(tbody.find('#inpSiteTypes' + index)),
        lstPopDen = tbody.find('[name=lstPopulationDensity]'),
        lstSites = wijmo.Control.getControl(tbody.find('#inpSites' + index)),
        fncLoadSites = function () { loadSites(lstSites, tbody, null); };

    // Administrative Divisions
    lstAdm1.change(function () { loadAdministrativeDivisions(2, lstAdm2, countryId, lstAdm1.val()); fncLoadSites(); });
    lstAdm2.change(function () { loadAdministrativeDivisions(3, lstAdm3, countryId, lstAdm2.val()); fncLoadSites(); });
    lstAdm3.change(function () { loadAdministrativeDivisions(4, lstAdm4, countryId, lstAdm3.val()); fncLoadSites(); });
    lstAdm4.change(fncLoadSites);
    loadAdministrativeDivisions(1, lstAdm1, countryId, null, chartCriteria);

    // Wave
    lstWave.change(fncLoadSites);
    if (chartCriteria && chartCriteria.WaveFieldId)
        lstWave.val(chartCriteria.WaveFieldId);

    // Population Density
    lstPopDen.change(fncLoadSites);
    if (chartCriteria && chartCriteria.PopDenFieldId)
        lstPopDen.val(chartCriteria.PopDenFieldId);

    // Site Types
    loadSiteTypes(lstSiteTypes, tbody, chartCriteria);
}

function loadAdministrativeDivisions(level, lst, countryId, parentId, chartCriteria) {
    var admDivId = null;
    if (chartCriteria && chartCriteria['AdministrativeDivisionId' + level])
        admDivId = chartCriteria['AdministrativeDivisionId' + level];
        
    if (level === 1 || parentId > 0) {
        var divisions = [];
        for (var i in chartFilters.AdministrativeDivisions) {
            var ad = chartFilters.AdministrativeDivisions[i];
            if (ad.CountryId == countryId && ((ad.AdministrativeDivisionParentId || 0) == (parentId || 0)))
                divisions.push(ad);
        }
        loadListBoxFromArray(lst, divisions, 'AdministrativeDivisionName', 'AdministrativeDivisionId', true, 'All');

        // Select the first administrative division if a chart criteria was passed.
        if (admDivId)
            lst.val(admDivId);
    } else {
        lst.empty();
    }

    var tbody = lst.parent().parent().parent();
    if (admDivId) {
        // Load the next level.
        nextLevel = level + 1;
        loadAdministrativeDivisions(nextLevel, tbody.find('[name=lstAdministrativeDivision' + nextLevel + ']'), countryId, admDivId, chartCriteria);
    } else {
        for (i = level + 1; i <= 4; i++)
            tbody.find('[name=lstAdministrativeDivision' + i + ']').empty();
    }
}

function loadSiteTypes(lstSiteTypes, tbody, chartCriteria) {
    var lstCountry = tbody.find('[name=lstCountry]');
    var countryId = lstCountry.val();

    var selectSiteTypeIds = [], selectItems = [];
    if (chartCriteria && chartCriteria.SiteTypeFieldIds) {
        var strSiteTypeIds = chartCriteria.SiteTypeFieldIds.split(',');
        for (var i in strSiteTypeIds)
            selectSiteTypeIds.push(strSiteTypeIds[i]);
    }

    var siteTypes = [];
    for (i in chartFilters.SiteTypes) {
        var st = chartFilters.SiteTypes[i];
        if (st.CountryId == countryId) {
            // Add the site to the list.
            var itm = $.extend(true, {}, st);
            siteTypes.push(itm);

            if (selectSiteTypeIds.indexOf(itm.FieldId) >= 0)
                selectItems.push(itm);
        }
    }
    lstSiteTypes.itemsSource = siteTypes;
    lstSiteTypes.checkedItems = selectItems;

    // Load sites.
    var lstSites = wijmo.Control.getControl(tbody.find('[id^=inpSites]'));
    loadSites(lstSites, tbody, chartCriteria);
}

function loadSites(lstSites, tbody, chartCriteria) {
    var lstCountry = tbody.find('[name=lstCountry]');
    var countryId = lstCountry.val();

    var adm1 = tbody.find('[name=lstAdministrativeDivision1]');
    var adm2 = tbody.find('[name=lstAdministrativeDivision2]');
    var adm3 = tbody.find('[name=lstAdministrativeDivision3]');
    var adm4 = tbody.find('[name=lstAdministrativeDivision4]');
    var administrativeDivisionId = adm4.val() || adm3.val() || adm2.val() || adm1.val();

    var lstPopDen = tbody.find('[name=lstPopulationDensity]');
    var populationDensity = lstPopDen.val();

    var lstWave = tbody.find('[name=lstWave]');
    var wave = lstWave.val();

    var lstSiteTypes = wijmo.Control.getControl(tbody.find('[id^=inpSiteTypes]'));
    var siteTypes = $.map(lstSiteTypes.checkedItems, function (n) { return n.FieldId; });

    var selectSiteIds = [], selectItems = [];
    if (chartCriteria && chartCriteria.SiteIds) {
        var strSiteIds = chartCriteria.SiteIds.split(',');
        for (var i in strSiteIds)
            selectSiteIds.push(parseInt(strSiteIds[i]));
    }

    var sites = [];
    for (i in chartFilters.Sites) {
        var s = chartFilters.Sites[i];
        if (s.CountryId == countryId &&
            (administrativeDivisionId === '' || $.inArray(parseInt(administrativeDivisionId), Array(s.AdministrativeDivisionId1, s.AdministrativeDivisionId2, s.AdministrativeDivisionId3, s.AdministrativeDivisionId4)) >= 0) &&
            (wave === '' || s.Wave === wave) &&
            (populationDensity === '' || s.PopulationDensity === populationDensity) &&
            (siteTypes === null || siteTypes.length === 0 || $.inArray(s.SiteType, siteTypes) >= 0)) {
            // Add the site to the list.
            var itm = $.extend(true, {}, s);
            sites.push(itm);

            if (selectSiteIds.indexOf(itm.SiteId) >= 0)
                selectItems.push(itm);
        }
    }
    lstSites.itemsSource = sites;
    lstSites.checkedItems = selectItems;

    // Load changes.
    var lstChange = wijmo.Control.getControl(tbody.find('[id^=inpChange]'));
    loadChanges(lstChange, tbody, chartCriteria);
}

function loadChanges(lstChange, tbody, chartCriteria) {
    var lstCountry = tbody.find('[name=lstCountry]');
    var countryId = lstCountry.val();

    var lstSites = wijmo.Control.getControl(tbody.find('[id^=inpSites]'));
    var siteIds = $.map(lstSites.checkedItems, function (n) { return n.SiteId; });

    var selectChangeIds = [], selectItems = [];
    if (chartCriteria && chartCriteria.ChangeIds) {
        var strChangeIds = chartCriteria.ChangeIds.split(',');
        for (var i in strChangeIds)
            selectChangeIds.push(parseInt(strChangeIds[i]));
    }

    var changes = [];
    for (i in chartFilters.Changes) {
        var c = chartFilters.Changes[i];
        if (((!siteIds || siteIds.length == 0) && c.CountryId == countryId) ||
            (siteIds.indexOf(c.SiteId) >= 0)) {
            // Add the change to the list.
            var itm = $.extend(true, {}, c);
            changes.push(itm);

            if (selectChangeIds.indexOf(itm.ObservationChangeId) >= 0)
                selectItems.push(itm);
        }
    }
    lstChange.itemsSource = changes;
    lstChange.checkedItems = selectItems;

    if (!changes || changes.length === 0)
        lstChange.inputElement.value = 'No changes available.';
}

function getChartCriteria() {
    var results = [];

    var n = criteriaCount;
    for (var i = 0; i < n; i++) {
        var criteria = $('#criteria' + i);
        var beginDate = wijmo.Control.getControl(criteria.find('#inpStartDate' + i)).value;
        if (beginDate)
            beginDate = beginDate.format('yyyy-mm-dd');
        var endDate = wijmo.Control.getControl(criteria.find('#inpEndDate' + i)).value;
        if (endDate)
            endDate = endDate.format('yyyy-mm-dd');
        results.push({
            CountryId: criteria.find('[name=lstCountry]').val(),
            AdministrativeDivisionId1: emptyStringToNull(criteria.find('[name=lstAdministrativeDivision1]').val()),
            AdministrativeDivisionId2: emptyStringToNull(criteria.find('[name=lstAdministrativeDivision2]').val()),
            AdministrativeDivisionId3: emptyStringToNull(criteria.find('[name=lstAdministrativeDivision3]').val()),
            AdministrativeDivisionId4: emptyStringToNull(criteria.find('[name=lstAdministrativeDivision4]').val()),
            PopulationDensityFieldId: emptyStringToNull(criteria.find('[name=lstPopulationDensity]').val()),
            SiteTypeFieldIds: emptyStringToNull($.map(wijmo.Control.getControl(criteria.find('#inpSiteTypes' + i)).checkedItems, function (n) { return n.FieldId; }).join(',')),
            WaveFieldId: emptyStringToNull(criteria.find('[name=lstWave]').val()),
            SiteIds: emptyStringToNull($.map(wijmo.Control.getControl(criteria.find('#inpSites' + i)).checkedItems, function (n) { return n.SiteId; }).join(',')),
            ChangeIds: emptyStringToNull($.map(wijmo.Control.getControl(criteria.find('#inpChange' + i)).checkedItems, function (n) { return n.ObservationChangeId; }).join(',')),
            AgeRangeId: emptyStringToNull(criteria.find('[name=lstAgeRange]').val()),
            SexCode: emptyStringToNull(criteria.find('[name=lstSex]').val()),
            BeginDate: emptyStringToNull(beginDate),
            EndDate: emptyStringToNull(endDate),
            ColorId: wijmo.Control.getControl(criteria.find('#inpColor' + i)).selectedValue
        });
    }

    return results;
}

function setCriteriasFormValue() {
    $('#criterias').val(JSON.stringify(getChartCriteria()));
    addMedian.val(chkaddMedian.is(':checked'));
}

function showHideRunChartRules() {
    // Show or hide the chart rules option.
    divAddMedian.addClass(criteriaCount === 1 ? 'display-inline-block' : 'display-none');
    divAddMedian.removeClass(criteriaCount === 1 ? 'display-none' : 'display-inline-block');
}