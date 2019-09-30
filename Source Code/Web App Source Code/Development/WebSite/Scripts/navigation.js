$(document).ready(function () {
    setNavigationDropdown('search-menu-anchor', 'search-menu');
    setNavigationDropdown('language-menu-anchor', 'language-menu');
    setNavigationDropdown('requests-menu-anchor', 'requests-menu');
    setNavigationDropdown('admin-menu-anchor', 'admin-menu');
});

function setNavigationDropdown(anchorId, menuId) {
    var anchor = $('#' + anchorId);
    var menu = $('#' + menuId);
    anchor.mouseenter(function () { menu.show(); });
    menu.mouseleave(function () { menu.hide(); });
}