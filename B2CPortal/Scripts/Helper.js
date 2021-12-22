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
//validate previous date.................
function validDate() {
    var today = new Date().toISOString().split('T')[0];
    var nextWeekDate = new Date(new Date().getTime() + 6 * 24 * 60 * 60 * 1000).toISOString().split('T')[0]
    document.getElementsByName("date")[0].setAttribute('min', today);
    document.getElementsByName("date")[0].setAttribute('max', nextWeekDate)
}
var now = new Date(),
    // minimum date the user can choose, in this case now and in the future
    minDate = now.toISOString().substring(0, 10);
$('#dateinput').prop('min', minDate);
//validate previous date.................

//password and confirm password..................
$('#password, #ConfirmPassword').on('keyup', function () {
    if ($('#password').val() == $('#ConfirmPassword').val()) {
        $('#message').html('Password Matching').css('color', 'green');
    } else
        $('#message').html('Password Not Matching').css('color', 'red');
});
//password and confirm password..................
//*******************************************************************
var paymenttype = {
    Stripe: "Stripe",
    HBL: "HBL",
    JazzCash: "JazzCash",
    EasyPaisa: "EasyPaisa",
};
//*******************************************************************