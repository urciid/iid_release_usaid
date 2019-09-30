var criteriaTabContainer, criteriaTabTemplate, addLine, criteriaContainer, criteriaTemplate,
    activeCriteria, criteriaCount = 0, divAddMedian, chkaddMedian, addMedian;

$(document).ready(function () {
    // Load these menus in the template, as they are always the same, no matter the country selection.
    loadListBoxFromArray($('[name=lstCountry]'), chartFilters.Countries, 'CountryName', 'CountryId');
    loadListBoxFromArray($('[name=lstReportClass]'), chartFilters.ReportClasses, 'Value', 'FieldId', false);

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
    
    // Multi Country Multi-Select
    var inpCountries = criteriaClone.find('#inpCountries');
    inpCountries.attr('id', 'inpCountries' + index);
    var mnuCountries = new wijmo.input.MultiSelect(inpCountries);
    mnuCountries.displayMemberPath = 'CountryName';
    mnuCountries.maxHeaderItems = 2;
    mnuCountries.selectedValuePath = 'CountryId';
    mnuCountries.itemsSource = null;
    mnuCountries.checkedItemsChanged.addHandler(function (s, e) {
        if (mnuCountries.checkedItems.length === 0)
            mnuCountries.inputElement.value = allCountriesText;
        
    });

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
        setTab($('#criteriaTab' + i), i - 1);
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
    
    var lstCountries = wijmo.Control.getControl(tbody.find('#inpCountries' + index));

    // Countries
    loadCountries(lstCountries, tbody, chartCriteria);
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

function loadCountries(lstCountries, tbody, chartCriteria) {
    var selectCountryIds = [], selectItems = [];
        
    if (chartCriteria && chartCriteria.CountryIds) {
        var strCountryIds = chartCriteria.CountryIds.split(',');
        for (var i in strCountryIds){
            selectCountryIds.push(strCountryIds[i]);
            }
    }
    var countries = [];
    for (i in chartFilters.Countries) {
        var co = chartFilters.Countries[i];
        var itm = $.extend(true, {}, co);
        countries.push(itm);
        for (k in selectCountryIds) {
            if (selectCountryIds[k] == itm.CountryId) {
                selectItems.push(itm);
            }

        }
                
    }
    lstCountries.itemsSource = countries;
    lstCountries.checkedItems = selectItems;
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
            ReportClassFieldId: criteria.find('[name=lstReportClass]').val(),
            CountryIds: emptyStringToNull($.map(wijmo.Control.getControl(criteria.find('#inpCountries' + i)).checkedItems, function (n) { return n.CountryId; }).join(',')),
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