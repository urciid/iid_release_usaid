var tbdyNumeratorDenominator, tbdyCollection, tbdySampling, tbdyChangeVariable,
    tbdyDisaggregation, tbdyTarget, tbdyRatio;

$(document).ready(function () {
    tbdyNumeratorDenominator = document.getElementById('tbdyNumeratorDenominator');
    tbdyCollection = document.getElementById('tbdyCollection');
    tbdySampling = document.getElementById('tbdySampling');
    tbdyChangeVariable = document.getElementById('tbdyChangeVariable');
    tbdyDisaggregation = document.getElementById('tbdyDisaggregation');
    tbdyTarget = document.getElementById('tbdyTarget');
    tbdyRatio = document.getElementById('tbdyRatio');
});

function indicatorTypeChanged(value) {
    switch (value) {
        case 'Percentage':
            tbdyNumeratorDenominator.style.display = 'table-row-group';
            tbdyCollection.style.display = 'table-row-group';
            tbdyChangeVariable.style.display = 'table-row-group';
            tbdyDisaggregation.style.display = 'table-row-group';
            tbdyTarget.style.display = 'table-row-group';
            tbdyRatio.style.display = 'none';
            break;
        case 'Average':
            tbdyNumeratorDenominator.style.display = 'table-row-group';
            tbdyCollection.style.display = 'table-row-group';
            tbdyChangeVariable.style.display = 'table-row-group';
            tbdyDisaggregation.style.display = 'table-row-group';
            tbdyTarget.style.display = 'table-row-group';
            tbdyRatio.style.display = 'none';
            break;
        case 'Count':
            tbdyNumeratorDenominator.style.display = 'none';
            tbdyCollection.style.display = 'table-row-group';
            tbdyChangeVariable.style.display = 'table-row-group';
            tbdyDisaggregation.style.display = 'table-row-group';
            tbdyTarget.style.display = 'table-row-group';
            tbdyRatio.style.display = 'none';
            break;
        case 'Yes/No':
            tbdyNumeratorDenominator.style.display = 'none';
            tbdyCollection.style.display = 'table-row-group';
            tbdyChangeVariable.style.display = 'none';
            tbdyDisaggregation.style.display = 'none';
            tbdyTarget.style.display = 'none';
            tbdyRatio.style.display = 'none';
            break;
        case 'Ratio':
            tbdyNumeratorDenominator.style.display = 'table-row-group';
            tbdyCollection.style.display = 'table-row-group';
            tbdyChangeVariable.style.display = 'table-row-group';
            tbdyDisaggregation.style.display = 'table-row-group';
            tbdyTarget.style.display = 'table-row-group';
            tbdyRatio.style.display = 'table-row-group';
            break;
    }
}

function enableDisableAgeRangeControls() {
    var disabled = (!hasObservations && radDisAggAgeYes.checked) ? '' : 'disabled';
    lstAgeRanges.disabled = disabled;
    btnAgeRangeMoveUp.disabled = disabled;
    btnAgeRangeMoveDown.disabled = disabled;
    btnAgeRangeRemove.disabled = disabled;
    btnAgeRangeAdd.disabled = disabled;
    btnAgeRangeAdd.disabled = disabled;
    txtAgeRangeAdd.disabled = disabled;
}

function addAgeRangeItem() {
    if (txtAgeRangeAdd.value !== '') {
        addSelectItem('lstAgeRanges', txtAgeRangeAdd.value, txtAgeRangeAdd.value);
        txtAgeRangeAdd.value = '';
    }
    $(txtAgeRangeAdd).focus();
}

function submitIndicatorForm() {
    // Clear existing age range names. 
    // They could already exist if validation prevented the form from submitting.
    $('[name=AgeRangeNames]').remove();
    
    $('#lstAgeRanges option').each(function (index, element) {
        $('#frmIndicator').append('<input type="hidden" name="AgeRangeNames" value="' + element.text + '" />');
    });
}