var siteFavoriteButton, siteFavoriteImage;

function modifySiteFavorite(siteId, btn, img) {
    siteFavoriteButton = btn;
    siteFavoriteImage = img;

    var data = { id: siteId };
    addAntiForgeryTokenToJson(data);

    var url = ('/Site/' + (btn.value == 'true' ? 'RemoveFromFavorites' : 'AddToFavorites'));

    $.post(url, data, modifySiteFavoriteCallback, 'json');
}

function modifySiteFavoriteCallback(result) {
    if (result.success) {
        siteFavoriteButton.title = result.Tooltip;
        siteFavoriteButton.value = result.IsFavorite;
        siteFavoriteImage.src = result.IconUrl;
    } else {
        alert(result.responseText);
    }
}