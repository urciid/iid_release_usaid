var activityFavoriteButton, activityFavoriteImage;

function modifyActivityFavorite(activityId, btn, img) {
    activityFavoriteButton = btn;
    activityFavoriteImage = img;

    var data = { id: activityId };
    addAntiForgeryTokenToJson(data);

    var url = ('/Activity/' + (btn.value == 'true' ? 'RemoveFromFavorites' : 'AddToFavorites'));

    $.post(url, data, modifyActivityFavoriteCallback, 'json');
}

function modifyActivityFavoriteCallback(result) {
    if (result.success) {
        activityFavoriteButton.title = result.Tooltip;
        activityFavoriteButton.value = result.IsFavorite;
        activityFavoriteImage.src = result.IconUrl;
    } else {
        alert(result.responseText);
    }
}