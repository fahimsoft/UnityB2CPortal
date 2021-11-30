
//get cookie by name 
function GetCookieByName(name) {
    var sym = unescape((document.cookie.match(name + '=([^;].+?)(;|$)') || [])[1] || '');
    if (sym == "PKR") {
        return sym
    } else {
        sym = "$";
        return sym;
    }
}

//*******************************************************************
var paymenttype = {
    Stripe: "Stripe",
    HBL: "HBL",
    JazzCash: "JazzCash",
    EasyPaisa: "EasyPaisa",
};
//*******************************************************************