function setMenuDropDownRtl(sender, arg) {
    sender.dropDown.dir = "rtl";
    sender.dropDownCssClass = "mnuLanguageDropDown";
}

function showFlexGridNoDataMessage(grid, message, dataItem) {
    grid.cells.rows.insert(0, new wijmo.grid.Row(dataItem));
    grid.setCellData(0, 0, htmlDecode(message));
}

function htmlEncode(value) {
    return $('<div/>').text(value).html();
}

function htmlDecode(value) {
    return $('<div/>').html(value).text();
}

function addSelectItem(id, text, value) {
    $('#' + id).append($('<option>', { value: value }).text(text));
}

function removeSelectedItems(id) {
    $('#' + id).find('option:selected').remove();
}

function moveSelectItemsUp(id) {
    $('#' + id + ' :selected').each(function (i, selected) {
        if (!$(this).prev().length) return false;
        $(this).insertBefore($(this).prev());
    });
    $('#' + id).focus().blur();
}

function moveSelectItemsDown(id) {
    $($('#' + id + ' :selected').get().reverse()).each(function (i, selected) {
        if (!$(this).next().length) return false;
        $(this).insertAfter($(this).next());
    });
    $('#' + id).focus().blur();
}

function loadListBoxFromJson(id, url, text, value, callback) {
    var data = [];
    $.getJSON(url, null, function (data) {
        data = $.map(data, function (item, a) {
            return ('<option value="' + item[value] + '">' + item[text] + '</option>');
        });
        $('#' + id).html(data.join(''));

        if (callback)
            callback();
    });
}

function loadListBoxFromArray(lst, array, text, value, addEmptyOption, emptyOptionText) {
    var data = $.map(array, function (item, a) {
        return ('<option value="' + item[value] + '">' + item[text] + '</option>');
    });
    var empty = '';
    if (addEmptyOption)
        empty = ('<option value="">' + emptyOptionText + '</option>');
    lst.html(empty + data.join(''));
}

function selectOptionByValue(id, value) {
    $('#' + id).val(value);
}

function selectOptionByText(id, text) {
    $('#' + id).val($('#' + id + ' option').filter(function () { return $(this).text() == text; }).val());
}

function addAntiForgeryTokenToForm(data) {
    data.append('__RequestVerificationToken', getAntiForgeryTokenValue());
    return data;
}

function addAntiForgeryTokenToJson(data) {
    data.__RequestVerificationToken = getAntiForgeryTokenValue();
    return data;
}

function appendAntiForgeryTokenToUrl(url) {
    return (url + (url.indexOf('?') == -1 ? '?' : '&') + '__RequestVerificationToken=' + getAntiForgeryTokenValue());
}

function getAntiForgeryTokenValue() {
    return $('#__AjaxAntiForgeryForm input[name=__RequestVerificationToken]').val();
}

function ajaxDownloadFile(url, data) {
    var $iframe, iframe_doc, iframe_html;

    if (($iframe = $('#download_iframe')).length === 0)
        $iframe = $("<iframe id='download_iframe'" + " style='display: none' src='about:blank'></iframe>").appendTo("body");

    iframe_doc = $iframe[0].contentWindow || $iframe[0].contentDocument;
    if (iframe_doc.document)
        iframe_doc = iframe_doc.document;

    iframe_html = "<html><head></head><body><form method='POST' action='" + url + "'>";

    if (data) {
        Object.keys(data).forEach(function (key) {
            iframe_html += "<input type='hidden' name='" + key + "' value='" + data[key] + "'>";
        });
    }

    iframe_html += "</form></body></html>";

    iframe_doc.open();
    iframe_doc.write(iframe_html);
    $(iframe_doc).find('form').submit();
}

function base64DownloadFile(data, fileName) {
    var a = document.createElement('a');
    a.href = data;
    a.setAttribute('download', fileName);
    a.click();
}

function setAreYouSure(message, id) {
    var selector = (id ? ('#' + id) : 'form');
    $(selector).areYouSure({ 'message': message });
}

function convertJsonDateToJavaScript(jsonDate) {
    if (jsonDate && jsonDate.substr && jsonDate.length > 6)
        return new Date(parseInt(jsonDate.substr(6)));
    return null;
}

function convertJavaScriptDateToJson(date) {
    if (date.getTime)
        return '/Date(' + date.getTime() + ')/';
    return null;
}

function offsetDateTimezone(date) {
    return new Date(date.getTime() + (date.getTimezoneOffset() * 60000));
}

function getDropdownSelectedText(id) {
    return $('#' + id + ' option:selected').text();
}

function enableButton(id, onclick) {
    var btn = $('#' + id);
    btn.removeClass('disabled');
    btn.off('click');
    if (onclick)
        btn.on('click', null, onclick);
}

function disableButton(id) {
    var btn = $('#' + id);
    btn.addClass('disabled');
    if (btn.is('a'))
        btn.on('click', function () { return false; });
    else
        btn.prop('onclick', null).off('click');
}

function ButtonSetDisabled(b) {
    //  alert('disabling');
    b.disabled = true;
}

function ButtonRemoveDisabled(b) {
    b.removeAttr('disabled');
}

function emptyStringToNull(value) {
    return value === '' ? null : value;
}

function getObjects(obj, key, val) {
    var objects = [];
    for (var i in obj) {
        if (!obj.hasOwnProperty(i)) continue;
        if (typeof obj[i] == 'object') {
            objects = objects.concat(getObjects(obj[i], key, val));
        } else if (i == key && obj[key] == val) {
            objects.push(obj);
        }
    }
    return objects;
}

function expandFlexGridDetailRows(grid) {
    // Expand all details rows if possible.
    var dp; // detailrowProvider
    for (var i in grid._c1Extenders) {
        var e = grid._c1Extenders[i];
        if (e.detailVisibilityMode) {
            dp = e;
            break;
        }
    }
    if (dp)
        for (var i = 0; i < grid.rows.length; i++)
            if (dp.isDetailAvailable(i))
                dp.showDetail(i, false);
}

function addDropDownListEmptyOption(lst, text) {
    lst.prepend('<option value="">' + (text || '') + '</option>');
    lst.val('');
}

function htmlEncode(value) {
    return $('<div/>').html(value).text();
}

function setText(id, text) {
    $('#' + id).text(text || '');
}

function setHtml(id, html) {
    $('#' + id).html(html || '');
}

function setGridToFillViewport(id) {
    var reservedHeight = 220; // height (in pixels) of the header, breadcrumbs, white space
    $('#' + id).css('max-height', ((window.innerHeight - reservedHeight).toString() + 'px'));
}

function objectToArray(object) {
    var array = [];
    if (object)
        for (var i in object)
            array.push(object[i]);
    return array;
}

function setCulture(culture) {
    var formData = new FormData();
    formData.append('culture', culture);
    addAntiForgeryTokenToForm(formData);

    $.ajax({
        url: '/Home/SetCulture',
        data: formData,
        processData: false,
        contentType: false,
        type: 'POST',
        success: function(){
            if ('setCultureCallback' in window) {
                setCultureCallback();
            } else {
                window.location.reload();
            }
        },
        error: function(result){
            alert(result);
        }
    });
}

function getShortText(value) {
    const charLimit = 40;

    if (value === null || value.trim().length === 0)
        return '';

    var abbreviate = value.length > charLimit;
    return (value.substr(0, abbreviate ? charLimit : value.Length) + (abbreviate ? "..." : ''));
}

function parsePunctuatedNumber(value) {
    if (value === null)
        return null;
    else
        return parseFloat(value.replace(/[^\d\.\-]/g, ''));

}