function testQuerySelector() {
    var input = document.getElementById("inputQuery");
    var output = document.getElementById("outputQuery");
    output.value = eval(input.value);
}