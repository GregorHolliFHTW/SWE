function sendToLower() {
    var xhttp = new XMLHttpRequest();
    var text = document.getElementById("text").value;
    xhttp.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            alert(this.responseText);
        }
    };
    xhttp.open("POST", "tolow", true);
    xhttp.setRequestHeader("Content-Type", "text/plain");
    xhttp.send(text);
}
function sendGenerateData() {
    var xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        6
        if (this.readyState == 4 && this.status == 200) {
            alert(this.responseText);
        }
    };
    xhttp.open("POST", "GetTemperature/generateData", true);
    xhttp.setRequestHeader("Content-Type", "text/plain");
    xhttp.send(text);
}