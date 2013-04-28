function generateTagCloud(container, wordsArray, smallFontSize, largeFontSise){
    var words = {};
    var count = wordsArray.length;

    for(var i = 0; i < count; i++){
        if(wordsArray[i] === "") continue;
        if(words[wordsArray[i]]){
            words[wordsArray[i]]++;
        }
        else{
            words[wordsArray[i]] = 1;
        }
    }

    var smallest = Number.MAX_VALUE;
    var biggest = 0;
    for(var i in words){
        if(isNaN(words[i])) continue;
        if(words[i] > biggest) biggest = words[i];
        if(words[i] < smallest) smallest = words[i];
    }

    var a = document.createElement('a');
    var empty = document.createTextNode(' ');
    a.setAttribute('href', '#');
    var fragment = document.createDocumentFragment();
    var currentA;
    for(var i in words){
        if(isNaN(words[i])) continue;
        currentA = a.cloneNode(true);
        currentA.style.fontSize = smallFontSize + ((words[i] - smallest) / (biggest - smallest)) * (largeFontSise - smallFontSize) + "px";
        currentA.innerHTML = i;
        fragment.appendChild(currentA);
        fragment.appendChild(empty.cloneNode(true));
    }
    container.appendChild(fragment);
}