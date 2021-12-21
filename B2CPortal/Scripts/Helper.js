//form data to obj convert..
function FormDataToObject(formArray) {
    //serialize data function
    var returnArray = {};
    for (var i = 0; i < formArray.length; i++) {
        returnArray[formArray[i]['name']] = formArray[i]['value'];
    }
    return returnArray;
}

//get cookie by name 
function GetCookieByName(name) {
    var sym = unescape((document.cookie.match(name + '=([^;].+?)(;|$)') || [])[1] || '');
    sym = sym == "PKR" ? "PKR" : "$";
    return sym;
}

//*******************************************************************
var paymenttype = {
    Stripe: "Stripe",
    HBL: "HBL",
    JazzCash: "JazzCash",
    EasyPaisa: "EasyPaisa",
};
//*******************************************************************