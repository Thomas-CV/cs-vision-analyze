function setAsFormatedJson(data, destination) {
    if (data) document.getElementById(destination).innerHTML = JSON.stringify(data, null, 4);
}

function attachLoaderOnClick(imageIndex, destination, loader) {
    document.getElementById(destination).addEventListener("click", function () {
        document.getElementById(loader).style.visibility = "visible";
        window.location.href = '/' + imageIndex;
    });
}
