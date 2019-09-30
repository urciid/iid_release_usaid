/*
Scripts to create flowText (rectangular) in SVG 1.1 UAs
Copyright (C) <2007>  <Andreas Neumann>
Version 1.0, 2007-02-26
neumann@karto.baug.ethz.ch
http://www.carto.net/
http://www.carto.net/neumann/

original document site: http://www.carto.net/papers/svg/textFlow/

*/

function textFlow(texts, svgElement, maxWidth, x, ddy) {
    // Remove existing children.
    $(svgElement).empty();

    var cumulY = 0;

    //extract and add line breaks for start
    for (var i in texts) {
        var t = texts[i];
        t.dashArray = new Array();
        var dashFound = true;
        var indexPos = 0;

        while (dashFound) {
            var result = t.text.indexOf("-", indexPos);
            if (result == -1) {
                //could not find a dash
                dashFound = false;
            }
            else {
                t.dashArray.push(result);
                indexPos = result + 1;
            }
        }
    }

    //split the text at all spaces and dashes
    var line = "";
    var dy = 0;
    var computedTextLength = 0;
    var tspan1, tspan2;

    tspan1 = getNewOuterTspan(svgElement, tspan1, x, dy);

    for (var h = 0; h < texts.length; h++) {
        var t = texts[h];
        var words = t.text.match(/ ?[^ -]+[ -]*/g);

        for (i = 0; i < words.length; i++) {
            var word = words[i];
            if (computedTextLength > maxWidth || i == 0) {
                if (computedTextLength > maxWidth) {
                    var tempText = '';
                    if (i > 1) {
                        tempText = tspan2.firstChild.nodeValue;
                        tempText = tempText.slice(0, (tempText.length - words[i - 1].length));
                    }
                    dy += ddy;
                    tspan2.firstChild.nodeValue = tempText;


                    tspan1 = getNewOuterTspan(svgElement, tspan1, x, ddy);
                    line = words[i - 1];
                }

                tspan2 = getNewInnerTspan(tspan1, tspan2, line, t.hexColor);
            }

            if (i == 0)
                line = word;
            else
                line += word;

            tspan2.firstChild.nodeValue = line;
            computedTextLength = tspan1.getComputedTextLength();

            if (i == words.length - 1) {
                if (computedTextLength > maxWidth) {
                    var tempText = tspan2.firstChild.nodeValue;
                    tspan2.firstChild.nodeValue = tempText.slice(0, (tempText.length - words[i].length));

                    tspan1 = getNewOuterTspan(svgElement, tspan1, x, ddy);
                    tspan2 = getNewInnerTspan(tspan1, tspan2, (words[i] + ' '), t.hexColor);

                    computedTextLength = 0;
                    dy += ddy;
                }

                if (h == texts.length - 1)
                    dy += ddy;
            }
        }
    }
    return dy;
}

function getNewOuterTspan(svgElement, tspan1, x, dy) {
    tspan1 = document.createElementNS(svgNS, "tspan");
    tspan1.setAttributeNS(null, "x", x);
    tspan1.setAttributeNS(null, "dy", dy);
    svgElement.appendChild(tspan1);
    return tspan1;
}

function getNewInnerTspan(tspan1, tspan2, text, hexColor) {
    tspan2 = document.createElementNS(svgNS, "tspan");
    tspan2.setAttributeNS(null, 'fill', ('#' + hexColor));
    tspan2.appendChild(document.createTextNode(text));
    tspan1.appendChild(tspan2);
    return tspan2;
}