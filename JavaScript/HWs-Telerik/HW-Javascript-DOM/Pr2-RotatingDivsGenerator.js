function generateRotatingDivs(parent, divsCount) {
    var fragment = document.createDocumentFragment();
    var div = document.createElement("div");
    var container = document.createElement("div");
    container.setAttribute("class", "rotatingContainer");
    div.style.position = "absolute";

    var parentHeight = parent.offsetHeight;
    var parentWidth = parent.offsetWidth;
    var radius = parseInt(Math.min(parentHeight, parentWidth) / 3);
    var centerX = parseInt(radius + Math.random() * (parentWidth - radius));
    var centerY = parseInt(radius + Math.random() * (parentHeight - radius));
    var angle = 2 * Math.PI / divsCount;

    var x;
    var y;
    var halfSize;
    var currentDiv;
    for (var i = 0; i < 2 * Math.PI; i += angle) {
        halfSize = parseInt(Math.random() * 10 + 10);
        x = parseInt(radius * Math.sin(i) + centerX - halfSize);
        y = parseInt(radius * Math.cos(i) + centerY - halfSize);

        currentDiv = div.cloneNode(true);
        currentDiv.style.height = 2 * halfSize + "px";
        currentDiv.style.width = 2 * halfSize + "px";
        currentDiv.style.borderRadius = halfSize + "px";
        currentDiv.style.background = getRandomRGB();
        currentDiv.style.top = y + "px";
        currentDiv.style.left = x + "px";
        container.appendChild(currentDiv);
    }
    fragment.appendChild(container);
    parent.appendChild(fragment);
    window.setTimeout(function () { rotateDivs(container, centerX, centerY, radius, 0); }, 100);
}

function getRandomRGB() {
    return "RGB(" + parseInt(Math.random() * 255) + ","
         + parseInt(Math.random() * 255) + "," + parseInt(Math.random() * 255) + ")";
}

function rotateDivs(container, centerX, centerY, radius, groupAngle) {
    var divs = container.getElementsByTagName('div');
    var divsCount = divs.length;
    var angle = 2 * Math.PI / divsCount;
    groupAngle += angle / 2;

    var halfSize;
    var x;
    var y;
    var i = groupAngle;
    for (var count = 0; count < divsCount; count++) {
        halfSize = parseInt(divs[count].style.height) / 2;
        x = parseInt(radius * Math.sin(i) + centerX - halfSize);
        y = parseInt(radius * Math.cos(i) + centerY - halfSize);
        divs[count].style.top = y + "px";
        divs[count].style.left = x + "px";
        i += angle;
    }
    window.setTimeout(function () { rotateDivs(container, centerX, centerY, radius, groupAngle); }, 100);
}