String.prototype.htmlEscape = function (){
  var escapedStr = String(this).replace(/&/g, '&amp;');
  escapedStr = escapedStr.replace(/</g, '&lt;');
  escapedStr = escapedStr.replace(/>/g, '&gt;');
  escapedStr = escapedStr.replace(/"/g, '&quot;');
  escapedStr = escapedStr.replace(/'/g, "&#39");
  return escapedStr;
}

String.prototype.htmlUnEscape = function () {
    var escapedStr = String(this).replace(/&amp;/g, '&');
    escapedStr = escapedStr.replace(/&lt;/g, '<');
    escapedStr = escapedStr.replace(/&gt;/g, '>');
    escapedStr = escapedStr.replace(/&quot;/g, '"');
    escapedStr = escapedStr.replace(/&#39/g, "'");
    return escapedStr;
}