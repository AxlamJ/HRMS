var UserDateFormate = "DD/MM/YYYY";
var UserScheduleDateFormate = "YYYY-MM-DD";
var UserDateFormateShort = "DD/MM/YY";
var UserDateTimeSecondsFormate = "DD/MM/YYYY HH:mm:ss";
var UserDateTimeFormate = "DD/MM/YYYY HH:mm";
var UserDateTimeMillisecondsFormate = "DD/MM/YYYY HH:mm:ss.SSS";
var UserTimeFormate = "HH:mm";


function FormatUserDate(date, options) {
    options = options || {}
    //if (convertToUtc) {
    //    return moment(date, UserDateFormate).add(new Date().getTimezoneOffset(), 'minutes').format();
    //}
    if (date) {
        var mDate = moment(date, UserDateFormate);
        if (options.addDays) {
            mDate.add(options.addDays, "days");
        }
        return mDate.format();
    }

    return null;
}

function FormatDate(date) {
    if (date)
        return format_date_time(date, UserDateFormate);
    return "";
}

function FormatScheduleDate(date) {
    if (date)
        return format_date_time(date, UserScheduleDateFormate);
    return "";
}

function FormatDateTime(date) {
    if (date)
        return format_date_time(date, UserDateTimeFormate);
    return "";
}

function format_date_time(date, format) {
    if (date.indexOf && date.indexOf('Z') < 0 && date.indexOf('z') < 0 && date.indexOf('+') < 0 && (date.indexOf('t') > -1 || date.indexOf('T') > -1)) {
        date += "Z";
    }
    return moment(date).format(format);
}
function getVirtualDirectory() {
    var url = window.location.href;
    var url_parts = url.split('/');

    var newURL = '';
    for (var i = 0; i < url_parts.length; i++) {
        if (i < 3)
            newURL += url_parts[i] + "/";
        else if (i == 3 && (url_parts[i].toLowerCase() == "hrms")) {
            newURL += url_parts[i] + "/";
        }
        else break;
    }

    return newURL;
}

function RestrictClose() {

    ///Check if browser back button has been pressed
    window.onload = function () {
        history.pushState("RestrictBackButton", null, null);
        window.onpopstate = function () {
            history.pushState('BackButtonRestrict', null, null);
            window.location.href = localStorage.getItem('PreviousPage').toString().replace(/&amp;/g, "&");
        };

    }

    window.onbeforeunload = function (e) {
        // Prevent the default behavior
        e.preventDefault();
        //var confirmationMessage = $.i18n('ConfirmationMessage');
        var confirmationMessage = ('ConfirmationMessage');
        (e || window.event).returnValue = confirmationMessage; //Gecko + IE
        return confirmationMessage;
    }
}


function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}

var spinnerQueue = [];
function enqueueSpinner() {
    if (spinnerQueue.length == 0) { spin(); }
    spinnerQueue.push("spinner");
}
function dequeueSpinner() {
    spinnerQueue.pop("spinner");
    if (spinnerQueue.length == 0) { unspin(); }
}


if (!Array.prototype.find) {
    Object.defineProperty(Array.prototype, 'find', {
        enumerable: false,
        configurable: true,
        writable: true,
        value: function (predicate) {
            if (this == null) {
                throw new TypeError('Array.prototype.find called on null or undefined');
            }
            if (typeof predicate !== 'function') {
                throw new TypeError('predicate must be a function');
            }
            var list = Object(this);
            var length = list.length >>> 0;
            var thisArg = arguments[1];
            var value;

            for (var i = 0; i < length; i++) {
                if (i in list) {
                    value = list[i];
                    if (predicate.call(thisArg, value, i, list)) {
                        return value;
                    }
                }
            }
            return undefined;
        }
    });
}


if (!String.prototype.padStart) {
    String.prototype.padStart = function padStart(targetLength, padString) {
        targetLength = targetLength >> 0; //truncate if number or convert non-number to 0;
        padString = String((typeof padString !== 'undefined' ? padString : ' '));
        if (this.length > targetLength) {
            return String(this);
        }
        else {
            targetLength = targetLength - this.length;
            if (targetLength > padString.length) {
                padString += padString.repeat(targetLength / padString.length); //append to original to ensure we are longer than needed
            }
            return padString.slice(0, targetLength) + String(this);
        }
    };
}




// Updates the inputs with the validation errors
function showErrors(form, errors) {
    // We loop through all the inputs and show the errors for that input
    form.querySelectorAll("input[name], select[name], textarea[name], img[name]").forEach(function (input) {
        // Since the errors can be null if no errors were found we need to handle
        // that
        showErrorsForInput(input, errors && errors[input.name]);
    });
}

// Shows the errors for a specific input
function showErrorsForInput(input, errors) {
    // This is the root of the input
    var formGroup = closestParent(input.parentNode, "form-group");
    // Find where the error messages will be inserted
    var messages = formGroup.querySelector(".messages");

    // First we remove any old messages and reset the classes
    resetFormGroup(formGroup);

    // If we have errors
    if (errors) {
        // Mark the group as having errors
        formGroup.classList.add("has-error");

        // Append all the errors
        errors.forEach(function (error) {
            addError(messages, error);
        });
    } else {
        // Otherwise, mark it as success
        formGroup.classList.add("has-success");
    }
}


// Recusively finds the closest parent that has the specified class
function closestParent(child, className) {
    if (!child || child == document) {
        return null;
    }
    if (child.classList.contains(className)) {
        return child;
    } else {
        return closestParent(child.parentNode, className);
    }
}

function resetFormGroup(formGroup) {
    // Remove the success and error classes
    formGroup.classList.remove("has-error");
    formGroup.classList.remove("has-success");
    // and remove any old messages
    formGroup.querySelectorAll(".help-block.error").forEach(function (el) {
        el.parentNode.removeChild(el);
    });
}

// Adds the specified error with the following markup
// <p class="help-block error">[message]</p>
function addError(messages, error) {
    messages.innerText = "";
    var block = document.createElement("p");
    block.classList.add("help-block");
    block.classList.add("error");
    block.innerText = error;
    messages.appendChild(block);
}

function calculateAge(dateString) {
    // Split the date (assumes DD/MM/YYYY format)
    const [day, month, year] = dateString.split('/').map(Number);

    // Create date objects
    const birthDate = new Date(year, month - 1, day);
    const today = new Date();

    // Calculate age
    let age = today.getFullYear() - birthDate.getFullYear();

    // Check if birthday has occurred this year
    const monthDiff = today.getMonth() - birthDate.getMonth();
    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
        age--;
    }

    return age;
}