const images="images";
const csUpload = "img-upload";
const csFile = "upload-file";
const csFileSelect = "upload-file-select";
const csFilePreview = "upload-file-preview";
const csFileSize = "upload-file-size"
const idPasswordText = "upload-password-text";
const idPasswordEnabled = "upload-password-enabled";
const idModelStateJson = "model-state-json";
const csNsfw = "upload-nsfw";

const csDescription = "upload-description";
const csDescriptionText = "upload-description-text";
const csDescriptionCounter = "upload-description-counter";

const counterIndicator="data-indicator";
const counterNormal="normal";
const counterYellow="yellow";
const counterRed="red";

const descriptionMaxLength = 120;
const csRemove="upload-remove";

let blankItem;
let index=0;
addEventListener("load",function(ev){

    $.get("GetBlankItem", 
        function (data, status) {
            blankItem = data;

        },
    );
});


function addImage() {  
   let item=blankItem.replaceAll("Images[0].",`Images[${index}].`).replaceAll("Images_0__",`Images_${index}__`)
   
   $("#images").append(item);
   index++;
}

function removeImage(sender) 
{
    $(sender).closest("."+csUpload).remove();

}

function fileSizeToString(size) {
    const kb = 1000;
    const mb = 1000000;
    let unit = kb;
    let unitType = "KB";
    if (size >= mb) {
        unit = mb;
        unitType = "MB";
    }

    return parseFloat((size / unit).toFixed(4)) + " " + unitType
}

function onFileChanged(sender) {

    const ancestor = $(sender).closest("." + csFile);
    const file = $(ancestor).find("." + csFileSelect)[0].files[0];
    const img = $(ancestor).find("." + csFilePreview);
    const size = $(ancestor).find("." + csFileSize);

    const path = window.URL.createObjectURL(file);
    $(img).attr("src", path);
    $(size).html(fileSizeToString(file.size));
}

function onPasswordEnabledChanged() {
    const passwordEnabled = $("#" + idPasswordEnabled)[0];
    const passwordText = $("#" + idPasswordText)[0];

    if (passwordEnabled.checked) {
        passwordText.removeAttribute("hidden");
    }
    else {
        passwordText.setAttribute("hidden", "");
    }
}


function onDescriptionKeyUpDown(sender) {
    const text = sender;
    const ancestor = $(text).closest("." + csDescription);
    const counter = $(ancestor).find("." + csDescriptionCounter);

    const length = text.value == '' ? descriptionMaxLength : descriptionMaxLength - text.value.length;
    let indicator = counterNormal;
    if (length < 0) {
        indicator = counterRed;
    }
    else if (length < descriptionMaxLength / 2) {
        indicator = counterYellow;
    }
    $(counter).attr(counterIndicator, indicator);
    $(counter).html(length);
}

