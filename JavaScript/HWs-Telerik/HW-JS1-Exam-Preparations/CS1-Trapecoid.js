String.prototype.PadLeft = function (count, symbol) {
    var result = this;
    var boundary = count - result.length;
    for (var i = 0; i < boundary; i++) result = symbol + result;
    return result;
}

String.prototype.PadRight = function (count, symbol) {
    var result = this;
    var boundary = count - result.length;
    for (var i = 0; i < boundary; i++) result = result + symbol;
    return result;
}

function Solve(args) {
    var result = "";
    var n = parseInt(args);
    result += "*".PadRight(n,'*').PadLeft(2*n,'.') + "\n";
    for (var i = 0; i < n-1; i++) {
        result += "*".PadLeft(n - i, '.').PadRight(2 * n - 1, '.') + "*\n";
    }
    result += "*".PadRight(2 * n, '*');
    return result;
}