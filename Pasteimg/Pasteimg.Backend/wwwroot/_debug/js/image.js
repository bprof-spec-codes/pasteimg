function setNsfwSession(value) {
    $.post("/DebugView/SetShowNsfw", {"value":value});
}

function onShowNsfwChanged(sender) {
    const elements = document.getElementsByClassName("nsfw");
    if (sender.checked) {
        setNsfwSession(true);
        for (let i = 0; i < elements.length; i++) {
            elements[i].removeAttribute("data-blur-nsfw");
        }
    }
    else {
        setNsfwSession(false);
        for (let i = 0; i < elements.length; i++) {
            elements[i].setAttribute("data-blur-nsfw", "");
        }
    }
}