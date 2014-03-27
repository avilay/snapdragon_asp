var currentStory = -1;
var xmlHttp;
var currentPageIndex = 0;

function createXmlHttpRequest() {
    if (window.ActiveXObject) {
        xmlHttp = new ActiveXObject("Microsoft.XMLHTTP");
    }
    else if (window.XMLHttpRequest) {
        xmlHttp = new XMLHttpRequest();
    }
}

function showStory(num) {
    document.getElementById("headline" + num).style.display = "none";
    document.getElementById("story" + num).style.display = "block";
    if (currentStory != num) {
        hideStory(currentStory);
    }
    currentStory = num;
    markStoryAsClicked(num);
}

function hideStory(num) {
    if (currentStory > -1) {
        document.getElementById("headline" + num).style.display = "block";
        document.getElementById("story" + num).style.display = "none";
    }
}

function markStoryAsClicked(num) {
    document.getElementById("headline" + num).style.color = "#94B5C5";
    //inform server about this
    createXmlHttpRequest();
    xmlHttp.onreadystatechange = handleStateChange;
    var rnd = new Date();
    var url = "/Reader/MarkItemAsClicked/" + num + "?rnd=" + rnd.getHours();
    xmlHttp.open("GET", url, true);                        
    xmlHttp.send(null);
}

function handleStateChange() {
    if (xmlHttp.readyState == 4) {
        if (xmlHttp.status != 200) {
            alert("DEBUG: Request failed with " + xmlHttp.status + " (" + xmlHttp.statusText + ").");
        }
    }
}

function hidePage(index) {
    if (index >= 0) {
        document.getElementById("page" + index).style.display = "none";        
    }
}

function showPage(index) {
    if (index >= 0) {
        document.getElementById("page" + index).style.display = "block";
        var itemids = document.getElementById("page" + index).getAttribute("itemids");
        markPageAsRead(itemids);
        if (currentPageIndex != index) {
            hidePage(currentPageIndex);
        }
        currentPageIndex = index;
    }
}

function markPageAsRead(itemids) {
    createXmlHttpRequest();
    xmlHttp.onreadystatechange = handleStateChange;
    var rnd = new Date();
    var url = "/Reader/MarkItemsAsRead?csvitemids=" + itemids + "&rnd=" + rnd.getHours();
    xmlHttp.open("GET", url, true);
    xmlHttp.send(null);    
}