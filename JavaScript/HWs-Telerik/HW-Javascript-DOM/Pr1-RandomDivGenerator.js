function generateRandomDivs(parent, divCount) {
    var fragment = document.createDocumentFragment();
    var div = document.createElement("div");
    var strong = document.createElement("strong");
    strong.innerHTML = "div";
    div.appendChild(strong);
    var currentDiv;
    var height;
    var width;
    var parentHeight = parent.offsetHeight;
    var parentWidth = parent.offsetWidth;
    for (var i = 0; i < divCount; i++) {
        currentDiv = div.cloneNode(true);

        currentDiv.style.display = "block";
        height = parseInt(Math.random() * 80 + 20);
        width = parseInt(Math.random() * 80 + 20);
        currentDiv.style.height = height + "px";
        currentDiv.style.width = width + "px";

        currentDiv.style.position = "absolute";
        currentDiv.style.top = parseInt(Math.random() * (parentHeight - height)) + "px";
        currentDiv.style.left = parseInt(Math.random() * (parentWidth - width)) + "px";
        
        currentDiv.style.color = getRandomRGB();
        currentDiv.style.background = getRandomRGB();
        currentDiv.style.borderRadius = parseInt(Math.random() * 30) + "px";
        currentDiv.style.border = parseInt(Math.random() * 10) + "px" + " solid " + getRandomRGB(); 

        fragment.appendChild(currentDiv);
    }
    parent.appendChild(fragment);
}

function getRandomRGB() {
    return "RGB(" + parseInt(Math.random() * 255) + ","
         + parseInt(Math.random() * 255) + "," + parseInt(Math.random() * 255) + ")";
}