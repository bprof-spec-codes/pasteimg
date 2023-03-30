function onShowNsfwChanged(sender) {
    const elements = document.getElementsByClassName("nsfw");
    if (sender.checked) {
        for (let i = 0; i < elements.length; i++) {
            elements[i].removeAttribute("data-blur-nsfw");
        }
    }
    else {
        for (let i = 0; i < elements.length; i++) {
            elements[i].setAttribute("data-blur-nsfw", "");
        }
    }
}